using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using TicketBookingProject.Server.DTOs;

namespace TicketBookingProject.Server;

public class UiActionService : IUiActionService
{
    private readonly IUiActionRepository _uiActionRepo;
    private readonly IMapper _mapper;
    public UiActionService(IUiActionRepository uiActionRepo, IMapper mapper)
    {
        _uiActionRepo = uiActionRepo;
        _mapper = mapper;
    }

    public async Task<Result<List<UIActionDto>>> GetAllAsync()
    {
        var items = await _uiActionRepo.GetAllUiActionsAsync();
        return Result<List<UIActionDto>>.Success(_mapper.Map<List<UIActionDto>>(items), "Get list UI Action result successfully!!!");
    }

    public async Task<Result<PagedResponse<UIActionDto>>> GetAllUIActions(ListUIActionRequest req)
    {
        var (query, total) = await _uiActionRepo.GetListUIAction(req);

        if (!await query.AnyAsync())
            return Result<PagedResponse<UIActionDto>>.Failure("No UI actions found.");

        var items = await query
            .ProjectTo<UIActionDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        var result = new PagedResponse<UIActionDto>(items, req.Page, req.PageSize, total);

        return Result<PagedResponse<UIActionDto>>.Success(result, "Get list UI action successfully.");
    }

    public async Task<List<UIActionDto>> GetNavItemsAsync()
    {
        var items = await _uiActionRepo.GetActiveNavItemsAsync();
        return _mapper.Map<List<UIActionDto>>(items);
    }

    public async Task<Result<UIActionDto>> GetByIdAsync(int id)
    {
        var item = await _uiActionRepo.GetByUiActionIdAsync(id);
        return item is null ? Result<UIActionDto>.Failure("UI action not found.") : Result<UIActionDto>.Success(_mapper.Map<UIActionDto>(item));
    }

    public async Task<Result<UIActionDto>> CreateAsync(UIActionRequest request)
    {
        var entity = _mapper.Map<UiAction>(request);
        var created = await _uiActionRepo.CreateAsync(entity);
        return Result<UIActionDto>.Success(_mapper.Map<UIActionDto>(created), "UI action created successfully.");
    }

    public async Task<Result<UIActionDto>> UpdateAsync(int id, UIActionRequest request)
    {
        var entity = _mapper.Map<UiAction>(request);
        entity.Id = id;
        var updated = await _uiActionRepo.UpdateUiActionAsync(entity);
        return updated is null ? Result<UIActionDto>.Failure("UI action not found.") : Result<UIActionDto>.Success(_mapper.Map<UIActionDto>(updated), "UI action updated successfully.");
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var deleted = await _uiActionRepo.DeleteAsync(id);
        return deleted ? Result<bool>.Success(true, "UI action deleted successfully.") : Result<bool>.Failure("UI action not found.");
    }
}
