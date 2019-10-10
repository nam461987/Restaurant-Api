using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Dtos.Menu;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using Restaurant.Repository.Interfaces.Menus;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class MenuBusiness : IMenuBusiness
    {
        private readonly IMapper _mapper;
        private readonly IMenuRepository _menuRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IMenuCategoryRepository _menuCategoryRepository;
        private readonly IMenuUnitRepository _menuUnitRepository;
        public MenuBusiness(IMapper mapper,
            IMenuRepository menuRepository,
            IRestaurantRepository restaurantRepository,
            IMenuCategoryRepository menuCategoryRepository,
            IMenuUnitRepository menuUnitRepository)
        {
            _mapper = mapper;
            _menuRepository = menuRepository;
            _restaurantRepository = restaurantRepository;
            _menuCategoryRepository = menuCategoryRepository;
            _menuUnitRepository = menuUnitRepository;
        }
        public async Task<Menu> Add(Menu model)
        {
            var entity = _menuRepository.Add(model);
            await _menuRepository.SaveChangeAsync();

            return model;
        }
        public async Task<bool> Update(Menu model)
        {
            var result = false;
            var record = await _menuRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.CategoryId = model.CategoryId;
                record.Name = model.Name;
                record.Description = model.Description;
                record.Image = model.Image;
                record.Price = model.Price;
                record.UnitId = model.UnitId;

                await _menuRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _menuRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;
                await _menuRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }
        public Task<bool> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
        public async Task<IPaginatedList<MenuDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize)
        {
            var MenuRepo = _menuRepository.Repo;

            var result = await (from menu in MenuRepo
                                join restaurant in _restaurantRepository.Repo on menu.RestaurantId equals restaurant.Id into rs
                                from restaurant in rs.DefaultIfEmpty()
                                join category in _menuCategoryRepository.Repo on menu.CategoryId equals category.Id into cs
                                from category in cs.DefaultIfEmpty()
                                join unit in _menuUnitRepository.Repo on menu.UnitId equals unit.Id into us
                                from unit in us.DefaultIfEmpty()
                                select new MenuDto
                                {
                                    Id = menu.Id,
                                    RestaurantId = menu.RestaurantId,
                                    RestaurantIdName = restaurant.Name,
                                    BranchId = menu.BranchId,
                                    CategoryId = menu.CategoryId,
                                    CategoryIdName = category.Name,
                                    Name = menu.Name,
                                    Description = menu.Description,
                                    Image = menu.Image,
                                    Price = menu.Price,
                                    UnitId = menu.UnitId,
                                    UnitIdName = unit.Name,
                                    Status = menu.Status
                                })
                          .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                          .Where(c => c.Status < (int)EStatus.All)
                          .OrderBy(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<MenuDto> GetById(int restaurantId, int branchId, int id)
        {
            var result = await _menuRepository.Repo.ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0).Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<MenuDto>(result);
        }
        public async Task<List<Menu>> GetAllNotPaginate(int restaurantId, int branchId)
        {
            var result = await _menuRepository.Repo.ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                .Where(c => c.Status == (int)EStatus.Using)
                .ToListAsync();

            return result;
        }
    }
}
