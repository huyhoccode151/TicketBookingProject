using AutoMapper;
using Microsoft.Identity.Client;
using TicketBookingProject.Server.Models;

namespace TicketBookingProject.Server;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _cateRepo;
    private readonly IMapper _mapper;
    public CategoryService(ICategoryRepository cateRepo, IMapper mapper) { 
        _cateRepo = cateRepo;
        _mapper = mapper;
    }
    public async Task<List<string>> ListCategoryNames(string? req)
    {
        var categoryNames = await _cateRepo.GetCategoryNames(req);
        return categoryNames;
    }

    public async Task<Category?> GetCategoryByName(string name) 
    {
        var category = await _cateRepo.GetByName(name);
        if (category == null) return null;
        return category;
    }

    public async Task<List<CategoryResponse>> ListCategory()
    {
        var categories = await _cateRepo.ListCategory();
        return _mapper.Map<List<CategoryResponse>>(categories);
    }
}
