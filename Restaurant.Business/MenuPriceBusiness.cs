using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Dtos.MenuPrice;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class MenuPriceBusiness : IMenuPriceBusiness
    {
        private readonly IMapper _mapper;
        private readonly IMenuPriceRepository _menuPriceRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IMenuSizeRepository _menuSizeRepository;
        public MenuPriceBusiness(IMapper mapper,
            IMenuPriceRepository menuPriceRepository,
            IRestaurantRepository restaurantRepository,
            IMenuRepository menuRepository,
            IMenuSizeRepository menuSizeRepository)
        {
            _mapper = mapper;
            _menuPriceRepository = menuPriceRepository;
            _restaurantRepository = restaurantRepository;
            _menuRepository = menuRepository;
            _menuSizeRepository = menuSizeRepository;
        }
        public async Task<MenuPrice> Add(MenuPrice model)
        {
            var entity = _menuPriceRepository.Add(model);
            await _menuPriceRepository.SaveChangeAsync();

            return model;
        }
        public async Task<bool> Update(MenuPrice model)
        {
            var result = false;
            var record = await _menuPriceRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.MenuId = model.MenuId;
                record.SizeId = model.SizeId;
                record.Price = model.Price;

                await _menuPriceRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _menuPriceRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;
                await _menuPriceRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }
        public Task<bool> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
        public async Task<IPaginatedList<MenuPriceDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize)
        {
            var MenuPriceRepo = _menuPriceRepository.Repo;

            var result = await (from menuPrice in MenuPriceRepo
                                join restaurant in _restaurantRepository.Repo on menuPrice.RestaurantId equals restaurant.Id into rs
                                from restaurant in rs.DefaultIfEmpty()
                                join menu in _menuRepository.Repo on menuPrice.MenuId equals menu.Id into mn
                                from menu in mn.DefaultIfEmpty()
                                join size in _menuSizeRepository.Repo on menuPrice.SizeId equals size.Id into sz
                                from size in sz.DefaultIfEmpty()
                                select new MenuPriceDto
                                {
                                    Id = menuPrice.Id,
                                    RestaurantId = menuPrice.RestaurantId,
                                    RestaurantIdName = restaurant.Name,
                                    BranchId = menuPrice.BranchId,
                                    MenuId = menuPrice.MenuId,
                                    MenuIdName = menu.Name,
                                    SizeId = menuPrice.SizeId,
                                    SizeIdName = size.Name,
                                    Price = menuPrice.Price,
                                    Status = menuPrice.Status,
                                })
                          .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                          .Where(c => c.Status < (int)EStatus.All)
                          .OrderBy(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<MenuPriceDto> GetById(int restaurantId, int branchId, int id)
        {
            var result = await _menuPriceRepository.Repo.ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0).Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<MenuPriceDto>(result);
        }
        public async Task<IPaginatedList<MenuPriceDto>> GetByMenuId(int restaurantId, int branchId, int pageIndex, int pageSize, int menuId)
        {
            var MenuPriceRepo = _menuPriceRepository.Repo;

            var result = await (from menuPrice in MenuPriceRepo
                                join restaurant in _restaurantRepository.Repo on menuPrice.RestaurantId equals restaurant.Id into rs
                                from restaurant in rs.DefaultIfEmpty()
                                join menu in _menuRepository.Repo on menuPrice.MenuId equals menu.Id into mn
                                from menu in mn.DefaultIfEmpty()
                                join size in _menuSizeRepository.Repo on menuPrice.SizeId equals size.Id into sz
                                from size in sz.DefaultIfEmpty()
                                select new MenuPriceDto
                                {
                                    Id = menuPrice.Id,
                                    RestaurantId = menuPrice.RestaurantId,
                                    RestaurantIdName = restaurant.Name,
                                    BranchId = menuPrice.BranchId,
                                    MenuId = menuPrice.MenuId,
                                    MenuIdName = menu.Name,
                                    SizeId = menuPrice.SizeId,
                                    SizeIdName = size.Name,
                                    Price = menuPrice.Price,
                                    Status = menuPrice.Status,
                                })
                          .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                          .Where(c => c.Status < (int)EStatus.All && c.MenuId == menuId)
                          .OrderBy(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<MenuPrice> CheckExistPrice(MenuPrice model)
        {
            var record = await _menuPriceRepository.Repo.FirstOrDefaultAsync(c => c.RestaurantId == model.RestaurantId && c.MenuId == model.MenuId
             && c.SizeId == model.SizeId);

            return record;
        }
        public async Task<List<MenuPriceDto>> GetAllNotPaginate(int restaurantId, int branchId)
        {
            var MenuPriceRepo = _menuPriceRepository.Repo;
            var result = await (from menuPrice in MenuPriceRepo
                                join restaurant in _restaurantRepository.Repo on menuPrice.RestaurantId equals restaurant.Id into rs
                                from restaurant in rs.DefaultIfEmpty()
                                join menu in _menuRepository.Repo on menuPrice.MenuId equals menu.Id into mn
                                from menu in mn.DefaultIfEmpty()
                                join size in _menuSizeRepository.Repo on menuPrice.SizeId equals size.Id into sz
                                from size in sz.DefaultIfEmpty()
                                select new MenuPriceDto
                                {
                                    Id = menuPrice.Id,
                                    RestaurantId = menuPrice.RestaurantId,
                                    RestaurantIdName = restaurant.Name,
                                    BranchId = menuPrice.BranchId,
                                    MenuId = menuPrice.MenuId,
                                    MenuIdName = menu.Name,
                                    SizeId = menuPrice.SizeId,
                                    SizeIdName = size.Name,
                                    Price = menuPrice.Price,
                                    Status = menuPrice.Status,
                                })
                          .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                          .Where(c => c.Status == (int)EStatus.Using)
                          .OrderBy(c => c.Id)
                          .ToListAsync();
            return result;
        }
    }
}
