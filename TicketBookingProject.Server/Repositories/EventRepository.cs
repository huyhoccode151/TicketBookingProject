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
    public async Task<PagedResponse<Event>> GetEventsAsync(EventListRequest request)
    {
        var query = _dbset
            .Include(e => e.Category)
            .Include(e => e.Venue)
            .Include(e => e.Organizer)
            .Include(e => e.EventPosters)
            .Include(e => e.TicketTypes).AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Category)) query = query.Where(c => c.Category.Name == request.Category);

        if (request.DatePreset != null)
        {
            var (from, to) = DateTimeResolve.Resolve(request.DatePreset);
            query = query.Where(d => (!from.HasValue || d.ActiveAt >= from) && (!to.HasValue || d.EndAt <= to));
        } else if (request.DateFrom.HasValue && request.DateTo.HasValue)
        {
            query = query.Where(d => (d.ActiveAt > request.DateFrom) && (d.EndAt <= request.DateTo));
        }

        if (!string.IsNullOrWhiteSpace(request.Venue))
        {
            query = query.Where(v => v.Venue.Name != null && request.Venue.Contains(v.Venue.Name));
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(s => (s.Name == null || s.Name.Contains(request.Search)));
        }

        if (request.Status != null) query = query.Where(s => s.Status == request.Status);

        if (request.OnSaleOnly == true) query = query.Where(o => o.SaleStartAt <= DateTime.UtcNow && o.SaleEndAt >= DateTime.UtcNow);

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            if (request.SortDesc) query = query.OrderBy($"{request.SortBy} descending");
            else query = query.OrderBy(x => x.CreatedAt);
        }

        var total = await query.CountAsync();

        var events = await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync();

        return new PagedResponse<Event>(events, request.Page, request.PageSize, total);
    }

    public async Task<Event> CreateEventAsync(Event evt) {
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

                if (ticketType.Quantity < item.Quantity) throw new Exception($"Not enough tickets available for {ticketType.Name}.");

                ticketType.Quantity -= item.Quantity;
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
}
