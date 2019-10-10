using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Dtos.MenuCategory;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using Restaurant.Repository.Interfaces.Menus;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class MenuCategoryBusiness : IMenuCategoryBusiness
    {
        private readonly IMapper _mapper;
        private readonly IMenuCategoryRepository _menuCategoryRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        public MenuCategoryBusiness(IMapper mapper,
            IMenuCategoryRepository menuCategoryRepository,
            IRestaurantRepository restaurantRepository)
        {
            _mapper = mapper;
            _menuCategoryRepository = menuCategoryRepository;
            _restaurantRepository = restaurantRepository;
        }
        public async Task<MenuCategory> Add(MenuCategory model)
        {
            var entity = _menuCategoryRepository.Add(model);
            await _menuCategoryRepository.SaveChangeAsync();

            return model;
        }
        public async Task<bool> Update(MenuCategory model)
        {
            var result = false;
            var record = await _menuCategoryRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.Name = model.Name;
                record.Avatar = model.Avatar;
                record.Description = model.Description;

                await _menuCategoryRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _menuCategoryRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;
                await _menuCategoryRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }
        public Task<bool> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
        public async Task<IPaginatedList<MenuCategoryDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize)
        {
            var MenuCategoryRepo = _menuCategoryRepository.Repo;

            var result = await (from menuCategory in MenuCategoryRepo
                          join restaurant in _restaurantRepository.Repo on menuCategory.RestaurantId equals restaurant.Id into rs
                          from restaurant in rs.DefaultIfEmpty()
                          select new MenuCategoryDto
                          {
                              Id = menuCategory.Id,
                              RestaurantId = menuCategory.RestaurantId,
                              RestaurantIdName = restaurant.Name,
                              BranchId = menuCategory.BranchId,
                              Name = menuCategory.Name,
                              Avatar = menuCategory.Avatar,
                              Description = menuCategory.Description,
                              Status = menuCategory.Status,
                          })
                          .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                          .Where(c => c.Status < (int)EStatus.All)
                          .OrderBy(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<MenuCategoryDto> GetById(int restaurantId, int branchId, int id)
        {
            var result = await _menuCategoryRepository.Repo.ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0).Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<MenuCategoryDto>(result);
        }
        public async Task<List<MenuCategory>> GetAllNotPaginate(int restaurantId, int branchId)
        {
            var result = await _menuCategoryRepository.Repo.ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                .Where(c => c.Status == (int)EStatus.Using)
                .ToListAsync();

            return result;
        }
    }
}
