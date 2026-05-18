using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Dynamic.Core;
using TicketBookingProject.Server.Helpers;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class EventRepository : BaseRepository<Event>, IEventRepository
{
    public EventRepository(TicketBookingProjectContext db) : base(db)
    {
    }
    public async Task<(IQueryable<Event>, int TotalCount)> GetEventsAsync(EventListRequest req, bool published = false, int? organizerId = null)
    {
        var events = _dbset.AsQueryable();

        if (organizerId != null) events = events.Where(b => b.OrganizerId == organizerId);

        if (published) events = events.Where(e => e.Status == EventStatus.Published);

        if (req.Category != null && req.Category.Any(x => x != null)) events = events.Where(c => req.Category.Contains(c.Category.Name));

        if (req.DatePreset != null)
        {
            var (from, to) = DateTimeResolve.Resolve(req.DatePreset);
            events = events.Where(d => (!from.HasValue || d.ActiveAt >= from) && (!to.HasValue || d.EndAt <= to));
        } else if (req.DateFrom.HasValue && req.DateTo.HasValue)
        {
            events = events.Where(d => (d.ActiveAt >= req.DateFrom) && (d.ActiveAt <= req.DateTo));
        }

        if (!string.IsNullOrWhiteSpace(req.Venue))
        {
            events = events.Where(v => v.Venue.Name != null && req.Venue.Contains(v.Venue.Name));
        }

        if (!string.IsNullOrWhiteSpace(req.Search))
        {
            events = events.Where(s => (s.Name == null || s.Name.Contains(req.Search)));
        }

        if (req.Status != null) events = events.Where(s => s.Status == req.Status);

        if (req.OnSaleOnly == true) events = events.Where(o => o.SaleStartAt <= DateTime.UtcNow && o.SaleEndAt >= DateTime.UtcNow);

        //if (!string.IsNullOrWhiteSpace(req.SortBy))
        //{
        //    if (req.SortDesc) events = events.OrderBy($"{req.SortBy} descending");
        //    else events = events.OrderBy(x => x.CreatedAt);
        //}

        //if (req.SortDesc == true) events = events.OrderBy(x => x.CreatedAt);

        events = events.OrderByDescending(x => x.CreatedAt);

        var total = await events.CountAsync();

        events = events.Skip((req.Page - 1) * req.PageSize).Take(req.PageSize);

        return (events, total);
    }

    public async Task<Event> CreateEventAsync(Event evt) {
        evt.CreatedAt = DateTime.UtcNow;
        evt.UpdatedAt = DateTime.UtcNow;
        _dbset.Add(evt);
        return evt;
    }

    public async Task<Event?> GetEventByIdAsync(int id)
    {
        return await _dbset
            .Include(e => e.Category)
            .Include(e => e.Venue)
            .Include(e => e.Organizer)
            .Include(e => e.EventPosters)
            .Include(e => e.TicketTypes)
            .FirstOrDefaultAsync(e => e.Id == id) ?? null;
    }

    public async Task<IQueryable<Event>> GetFavEvent(int currentUserId)
    {
        var events = _dbset.AsQueryable();
        events = events.Where(e => e.EventSubscriptions.Any(s => s.UserId == currentUserId));

        return events;
    }

    public async Task<bool> DeleteEventAsync(int id)
    {
        var evt = await _dbset.FindAsync(id);
        if (evt == null) return false;
        _dbset.Remove(evt);
        return true;
    }

    public async Task AddEventWithPosters(Event evt, List<UploadPosterRequest> posters)
    {
        evt.EventPosters = posters.Select(x => new EventPoster
        {
            EventId = evt.Id,
            ImageUrl = x.Url,
            ImageType = ImageType.Poster,
            IsPrimary = x.Index == 0,
        }).ToList();
    }

    public async Task AddTicketTypeWithPoster(Event evt, List<TicketType> ticketTypes)
    {
        evt.TicketTypes = ticketTypes;
    }

    public async Task UpdateEventWithPosters(Event evt, List<EventPoster> newPosters, List<PosterMetaDto>? posterMeta)
    {
        var existingPosters = evt.EventPosters.ToList();

        var keptPosterIds = posterMeta?
        .Where(m => m.PosterId.HasValue)
        .Select(m => m.PosterId!.Value)
        .ToHashSet() ?? [];

        var toDelete = existingPosters
            .Where(p => !keptPosterIds.Contains(p.Id))
            .ToList();

        _db.EventPosters.RemoveRange(toDelete);

        if (posterMeta != null)
        {
            foreach (var meta in posterMeta.Where(m => m.PosterId.HasValue))
            {
                    var existing = existingPosters.FirstOrDefault(p => p.Id == meta.PosterId!.Value);
                    if (existing != null)
                    {
                        existing.IsPrimary = meta.IsPrimary;
                    }
            }
        }

        var newMetas = posterMeta?.Where(m => !m.PosterId.HasValue).ToList() ?? new List<PosterMetaDto>();
        for (int i = 0; i < newPosters.Count; i++)
        {
            newPosters[i].EventId = evt.Id;
            newPosters[i].IsPrimary = newMetas.ElementAtOrDefault(i)?.IsPrimary ?? false;
        }
        await _db.EventPosters.AddRangeAsync(newPosters);
    }

    public async Task UpdateEventWithTicket(Event evt, List<TicketType> newTicketTypes)
    {
        var existingTickets = evt.TicketTypes.ToList();
        foreach (var incoming in newTicketTypes)
        {
            if (incoming.Id > 0)
            {
                var existing = existingTickets.FirstOrDefault(t => t.Id == incoming.Id);
                if (existing != null)
                {
                    existing.Name = incoming.Name;
                    existing.Price = incoming.Price;
                    existing.Quantity = incoming.Quantity;
                    existing.MaxPerUser = incoming.MaxPerUser;
                }
            }
            else
            {
                incoming.EventId = evt.Id;
                await _db.TicketTypes.AddAsync(incoming);
            }
        }

        var incomingIds = newTicketTypes
            .Where(t => t.Id > 0)
            .Select(t => t.Id)
            .ToHashSet();

        var toDelete = existingTickets
            .Where(t => !incomingIds.Contains(t.Id))
            .ToList();

        _db.TicketTypes.RemoveRange(toDelete);
    }

    public async Task<List<EventPoster>> GetEventPosterByIdAsync(int id)
    {
        return await _db.EventPosters.Where(p => p.EventId == id).ToListAsync();
    }

    public async Task<List<SeatHold>> HoldTicketsAsync(HoldTicketsRequest request, int userId)
    {
        using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(15);

            var seatHolds = new List<SeatHold>();
            foreach (var item in request.Items)
            {
                var ticketType = await _db.TicketTypes.FirstOrDefaultAsync(t => t.Id == item.Id);

                if (ticketType == null) throw new Exception($"Ticket type with ID {item.Id} not found.");

                if ((ticketType.Quantity - ticketType.SoldQuantity) < item.Quantity) throw new Exception($"Not enough tickets available for {ticketType.Name}.");

                //ticketType.Quantity -= item.Quantity; 
                ticketType.SoldQuantity += item.Quantity;

                var seatHold = new SeatHold
                {
                    EventSeatId = null,
                    BookingId = null,
                    TicketTypeId = item.Id,
                    Quantity = item.Quantity,
                    UserId = userId,
                    Status = SeatHoldStatus.Active,
                    ExpiresAt = expiresAt,
                    CreatedAt = now
                };

                seatHolds.Add(seatHold);
            }

            _db.SeatHolds.AddRange(seatHolds);

            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            return seatHolds;
        }
        catch {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateHoldStatusAsync(List<SeatHold> sh, BookingResponse booking)
    {
        await _db.SeatHolds
        .Where(s => sh.Select(o => o.Id).Contains(s.Id))
        .ExecuteUpdateAsync(s => s
            .SetProperty(x => x.BookingId, booking.Id)
        );
    }

    public async Task<List<EventTrendingResponse>> GetEventTrending()
    {
        return await _dbset.Select(e => new
        {
            EventName = e.Name,

            ImageUrl = e.EventPosters
                .Where(p => p.IsPrimary)
                .Select(p => p.ImageUrl)
                .FirstOrDefault(),

            Stock = e.TicketTypes
                .Sum(tt => tt.Quantity),

            Sold = e.TicketTypes
                .Sum(tt => tt.SoldQuantity)
        })
        .OrderByDescending(x => x.Sold)          // trending theo số bán
        .ThenByDescending(x => x.Stock)          // tie-break
        .Take(3)
        .Select(x => new EventTrendingResponse(
            x.ImageUrl ?? "",                   // tránh null
            x.EventName ?? "",
            x.Sold,
            x.Stock
        ))
        .ToListAsync();
    }

    public async Task<List<(Event Event, string oStatus, string nStatus)>> UpdateEventStatusAuto()
    {
        var now = DateTime.UtcNow;
        var events = await _dbset.Where(e =>
            (e.Status == EventStatus.Confirm && e.SaleStartAt <= now) ||
            (e.Status == EventStatus.Published && e.ActiveAt <= now) ||
            (e.Status == EventStatus.Ongoing && e.EndAt <= now)
            ).ToListAsync();
        var changedEvents = new List<(Event Event, string OldStatus, string NewStatus)>(); ;

        foreach (var e in events)
        {
            var oldStatus = e.Status;
            e.Status = e.Status switch
            {
                EventStatus.Confirm => EventStatus.Published,
                EventStatus.Published => EventStatus.Ongoing,
                EventStatus.Ongoing => EventStatus.Completed,
                _ => e.Status
            };
            e.UpdatedAt = now;
            changedEvents.Add((e, oldStatus.ToString(), e.Status.ToString()));
        }

        if (changedEvents.Any())
            await _db.SaveChangesAsync();

        return changedEvents;
    }

    public async Task<List<string>> GetEventName(string? req)
    {
        if (!string.IsNullOrWhiteSpace(req)) return await _dbset.Where(e => EF.Functions.Like(e.Name ?? "", $"%{req}%")).Take(10).Select(n => n.Name ?? n.Id.ToString()).ToListAsync();

        return await _dbset.Select(n => n.Name ?? n.Id.ToString()).ToListAsync();
    }

    public async Task<IQueryable<Event>> GetRelatedEvents(int id, int categoryId, int? numTake = null)
    {
        var events = _dbset.AsQueryable();

        if (numTake != null) events = events.Where(e => e.Id != id && e.CategoryId == categoryId).Take(numTake ?? 4);

        return events;
    }
}
