using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System.Net.WebSockets;
using System.Text.Json.Serialization;
using TicketBookingProject.Server.Enums;
using TicketBookingProject.Server.Models;
using static System.Net.WebRequestMethods;

namespace TicketBookingProject.Server;

public class EventService : IEventService
{
    private readonly TicketBookingProjectContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IEventRepository _eventRepo;
    private readonly IBookingRepository _bookingRepo;
    private readonly ITicketRepository _ticketRepo;
    private readonly IRefundRepository _refundRepo;
    private readonly IAuditLogRepository _auditLogRepo;
    private readonly IMapper _mapper;
    public EventService(
        IEventRepository eventRepo, 
        IMapper mapper, 
        TicketBookingProjectContext db, 
        ICurrentUserService currentUser, 
        IBookingRepository bookingRepo, 
        ITicketRepository ticketRepo, 
        IRefundRepository refundRepo,
        IAuditLogRepository auditRepo) {
        _eventRepo = eventRepo;
        _mapper = mapper;
        _db = db;
        _currentUser = currentUser;
        _bookingRepo = bookingRepo;
        _ticketRepo = ticketRepo;
        _refundRepo = refundRepo;
        _auditLogRepo = auditRepo;
    } 

    public async Task<Result<PagedResponse<EventListItemResponse>>> ListEventAsync(EventListRequest req)
    {
        var currentOrganizerId = _currentUser.UserId;

        var roles = _currentUser.Role ?? new List<string>();

        var actionOfAdmin = roles.Contains("admin");

        var actionOfOrganizer = roles.Contains("organizer");

        var actionOfCustomer = roles.Contains("customer");

        var guess = roles.Count == 0;

        var (events, total) = actionOfAdmin
            ? await _eventRepo.GetEventsAsync(req)
            : actionOfOrganizer
                ? await _eventRepo.GetEventsAsync(req, false, currentOrganizerId)
                : actionOfCustomer || guess
                    ? await _eventRepo.GetEventsAsync(req, true)
                    : (null, 0);

        if (events == null)
        {
            return Result<PagedResponse<EventListItemResponse>>
                .Failure("Get List Event Failed!!!", StatusCodes.Status203NonAuthoritative);
        }

        var pagedEvent = await events
                .ProjectTo<EventListItemResponse>(_mapper.ConfigurationProvider)
                .ToListAsync();

        var result = new PagedResponse<EventListItemResponse>(
                pagedEvent,
                req.Page,
                req.PageSize,
                total
            );

        return Result<PagedResponse<EventListItemResponse>>.Success(result, "Get List Event with admin Successfully");
    }

    public async Task<EventDetailResponse> CreateEventAsync(CreateEventRequest request)
    {
        using var transaction = _db.Database.BeginTransaction(); //sử dụng transaction, có thể rollback nếu lỗi

        try
        {
            var posters = await this.UploadPoster(request.Posters); //logic upload file

            var ticketTypes = _mapper.Map<List<TicketType>>(request.TicketTypes);

            var eventCreate = _mapper.Map<Event>(request);

            eventCreate.OrganizerId = (int)_currentUser.UserId!;

            var evt = await _eventRepo.CreateEventAsync(eventCreate);

            if (posters != null) { await _eventRepo.AddEventWithPosters(evt, posters); }

            if (ticketTypes != null) { await _eventRepo.AddTicketTypeWithPoster(evt, ticketTypes); }

            await _db.SaveChangesAsync();

            await transaction.CommitAsync();

            return _mapper.Map<EventDetailResponse>(evt);
        }
        catch
        {
            //await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<List<UploadPosterRequest>?> UploadPoster(List<IFormFile> request)
    {
        if (request == null || request.Count == 0) return await Task.FromResult<List<UploadPosterRequest>?>(null);

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
        var maxFileSize = 10 * 1024 * 1024;

        var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var result = new List<UploadPosterRequest>();

        var index = 0;

        foreach (var file in request)
        {
            if (file.Length == 0) continue;

            if (file.Length > maxFileSize) continue;

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(ext)) continue;

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            result.Add(new UploadPosterRequest(
                index,
                $"/uploads/{fileName}"));

            index++;
        }
        return result;
    }

    public async Task<EventDetailResponse?> GetEventByIdAsync(int id)
    {
        var evt = await _eventRepo.GetEventByIdAsync(id);
        if (evt == null) return null;
        return _mapper.Map<EventDetailResponse>(evt);
    }

    public async Task<EventDetailResponse?> UpdateEventAsync(int id, UpdateEventRequest request)
    {
        var evt = await _eventRepo.GetEventByIdAsync(id);
        if (evt == null) return null;

        _mapper.Map(request, evt);

        var posterMeta = JsonConvert.DeserializeObject<List<PosterMetaDto>>(request.PosterMeta);
        if (request.Posters != null && request.Posters.Count > 0)
        {
            var posters = await this.UploadPoster(request.Posters);
            var _posters = posters != null ? posters.Select(p => new EventPoster
            {
                EventId = evt.Id,
                ImageUrl = p.Url,
                ImageType = ImageType.Poster,
                IsPrimary = false,
            }).ToList() : new List<EventPoster>();
            if (posters != null)
                await _eventRepo.UpdateEventWithPosters(evt, _posters, posterMeta);
        } else if (posterMeta != null)
        {
            await _eventRepo.UpdateEventWithPosters(evt, [], posterMeta);
        }

        var ticketTypes = _mapper.Map<List<TicketType>>(request.TicketTypes);
        if (ticketTypes != null) { await _eventRepo.UpdateEventWithTicket(evt, ticketTypes); }

        await _db.SaveChangesAsync();

        return _mapper.Map<EventDetailResponse>(evt);
    }

    public async Task<bool> DeleteEventAsync(int id)
    {
        await _eventRepo.DeleteEventAsync(id);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<List<EventPosterResponse>> GetEventPosterByIdAsync(int id)
    {
        var posters = await _eventRepo.GetEventPosterByIdAsync(id);

        return _mapper.Map<List<EventPosterResponse>>(posters);
    }

    public async Task<List<TicketTypeResponse>> GetEventTicketTypesByIdAsync(int id)
    {
        var evt = await _eventRepo.GetEventByIdAsync(id);
        if (evt == null) return new List<TicketTypeResponse>();
        return _mapper.Map<List<TicketTypeResponse>>(evt.TicketTypes);
    }

    public async Task<List<SeatHold>> HoldTicketsAsync(HoldTicketsRequest request)
    {
        var userId = (int)_currentUser.UserId!;
        var seatHolds = await _eventRepo.HoldTicketsAsync(request, userId);
        return _mapper.Map<List<SeatHold>>(seatHolds);
    }

    public async Task UpdateHoldStatusAsync(List<SeatHold> sh, BookingResponse booking)
    {
        await _eventRepo.UpdateHoldStatusAsync(sh, booking);
    }

    public async Task<List<EventTrendingResponse>> GetEventTrending()
    {
        var events = await _eventRepo.GetEventTrending();

        return events;
    }

    public async Task<List<string>> GetEventName(string? req)
    {
        var events = await _eventRepo.GetEventName(req);

        return events;
    }

    public async Task CleanupExpiredHoldsAsync()
    {
        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var now = DateTime.UtcNow;

            var expiredHolds = await _db.SeatHolds
                .Where(sh => sh.Status == SeatHoldStatus.Released && sh.ExpiresAt < now)
                .ToListAsync();

            if (expiredHolds.Any())
            {
                foreach (var hold in expiredHolds)
                {
                    await _db.Database.ExecuteSqlRawAsync(
                        "UPDATE TicketTypes SET Quantity = Quantity + {0} WHERE Id = {1}",
                        hold.Quantity, hold.TicketTypeId);

                    hold.Status = SeatHoldStatus.Expired;

                    if (hold.BookingId.HasValue)
                    {
                        var booking = await _db.Bookings
                            .FirstOrDefaultAsync(b => b.Id == hold.BookingId && b.Status == BookingStatus.Pending);

                        if (booking != null)
                        {
                            booking.Status = BookingStatus.Cancelled;
                        }
                    }
                }

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                Console.WriteLine($"[CronJob] Just Rollback {expiredHolds.Count} seat holds.");
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"[Error] Error while rolling back seat holds: {ex.Message}");
        }
    }

    public async Task UpdateEventStatusAuto()
    {
        var changedEvents = await _eventRepo.UpdateEventStatusAuto();

        if (!changedEvents.Any()) return;

        var auditLogs = changedEvents.Select(x => new AuditLog
        {
            UserId = null, // system action
            Action = "UPDATE_STATUS",
            EntityType = "Event",
            EntityId = x.Event.Id,
            Description = $"Event '{x.Event.Name}' status changed from {x.oStatus} to {x.nStatus}",
            Metadata = JsonConvert.SerializeObject(new
            {
                EventId = x.Event.Id,
                OldStatus = x.oStatus,
                NewStatus = x.nStatus,
                ChangedAt = DateTime.UtcNow
            }),
            CreatedAt = DateTime.UtcNow
        }).ToList();

        await _db.AddRangeAsync(auditLogs);
        await _db.SaveChangesAsync();
    }

    public async Task<Result<EventDetailResponse>> UpdateEventStatusAsync(int id, UpdateEventStatusRequest request)
    {
        try
        {
            var roles = _currentUser.Role ?? new List<string>();

            var userId = _currentUser.UserId;

            var evt = await _eventRepo.GetEventByIdAsync(id);

            if (evt == null) return Result<EventDetailResponse>.Failure("Could not found event to update status!!!");

            if (roles.Contains("admin") && (evt.Status == EventStatus.Draft) && (request.Status == EventStatus.Confirm))
                evt.Status = request.Status;
            else if ((roles.Contains("organizer") || roles.Contains("admin")) && (evt.Status == EventStatus.Draft) && (request.Status == EventStatus.Cancelled))
                evt.Status = request.Status;
            else if ((roles.Contains("organizer") || roles.Contains("admin")) && evt.Status == EventStatus.Published && request.Status == EventStatus.Cancelled)
            {
                evt.Status = request.Status;

                var booking = await _bookingRepo.CancelBooking(id, request.CancelReason ?? ""); //return booking id and payment id under dictionary form

                if (booking.Count() == 0)
                {
                    await _eventRepo.SaveChanges();
                    return Result<EventDetailResponse>.Failure("Could not found any booking of this event!!!");
                }

                var ticket = _ticketRepo.CancelTicket(booking.Select(b => b.BookingId).ToList());

                var refund = _refundRepo.CreateRefund(booking, userId); //create pending refund, after that use cronjob to excute 10-50 refundation each time
            }

            evt.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Result<EventDetailResponse>.Success(_mapper.Map<EventDetailResponse>(evt), "Update event status successfully!!!");
        }catch (Exception ex)
        {
            //_logger.LogError(ex, "Error updating event status for event {EventId}", id);

            return Result<EventDetailResponse>.Failure("Unexpected error occurred while updating event status.");
        }
    }
}
