using AutoMapper;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using TicketBookingProject.Server.Models;
using static System.Net.WebRequestMethods;

namespace TicketBookingProject.Server;

public class EventService : IEventService
{
    private readonly TicketBookingProjectContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IEventRepository _eventRepo;
    private readonly IMapper _mapper;
    public EventService(IEventRepository eventRepo, IMapper mapper, TicketBookingProjectContext db, ICurrentUserService currentUser) {
        _eventRepo = eventRepo;
        _mapper = mapper;
        _db = db;
        _currentUser = currentUser;
    } 

    public async Task<PagedResponse<EventListItemResponse>> ListEventAsync(EventListRequest request)
    {
        var events = await _eventRepo.GetEventsAsync(request);

        var items = _mapper.Map<List<EventListItemResponse>>(events.Items);

        return new PagedResponse<EventListItemResponse>(
                items,
                events.Page,
                events.PageSize,
                events.TotalCount
            );
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
}
