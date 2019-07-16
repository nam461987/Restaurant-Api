using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Dtos.MenuSize;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class MenuSizeBusiness : IMenuSizeBusiness
    {
        private readonly IMapper _mapper;
        private readonly IMenuSizeRepository _menuSizeRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        public MenuSizeBusiness(IMapper mapper,
            IMenuSizeRepository menuSizeRepository,
            IRestaurantRepository restaurantRepository)
        {
            _mapper = mapper;
            _menuSizeRepository = menuSizeRepository;
            _restaurantRepository = restaurantRepository;
        }
        public async Task<MenuSize> Add(MenuSize model)
        {
            var entity = _menuSizeRepository.Add(model);
            await _menuSizeRepository.SaveChangeAsync();

            return model;
        }
        public async Task<bool> Update(MenuSize model)
        {
            var result = false;
            var record = await _menuSizeRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.Name = model.Name;

                await _menuSizeRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _menuSizeRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;
                await _menuSizeRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }
        public Task<bool> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
        public Task<IPaginatedList<MenuSizeDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize)
        {
            var MenuSizeRepo = _menuSizeRepository.Repo;

            var result = (from menuSize in MenuSizeRepo
                          join restaurant in _restaurantRepository.Repo on menuSize.RestaurantId equals restaurant.Id into rs
                          from restaurant in rs.DefaultIfEmpty()
                          select new MenuSizeDto
                          {
                              Id = menuSize.Id,
                              RestaurantId = menuSize.RestaurantId,
                              RestaurantIdName = restaurant.Name,
                              BranchId = menuSize.BranchId,
                              Name = menuSize.Name,
                              Status = menuSize.Status,
                          })
                          .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                          .Where(c => c.Status < (int)EStatus.All)
                          .OrderBy(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<MenuSizeDto> GetById(int restaurantId, int branchId, int id)
        {
            var result = await _menuSizeRepository.Repo.ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0).Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<MenuSizeDto>(result);
        }
    }
}
