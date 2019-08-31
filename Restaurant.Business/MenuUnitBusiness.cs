using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Dtos.MenuUnit;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class MenuUnitBusiness : IMenuUnitBusiness
    {
        private readonly IMapper _mapper;
        private readonly IMenuUnitRepository _menuUnitRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        public MenuUnitBusiness(IMapper mapper,
            IMenuUnitRepository menuUnitRepository,
            IRestaurantRepository restaurantRepository)
        {
            _mapper = mapper;
            _menuUnitRepository = menuUnitRepository;
            _restaurantRepository = restaurantRepository;
        }
        public async Task<MenuUnit> Add(MenuUnit model)
        {
            var entity = _menuUnitRepository.Add(model);
            await _menuUnitRepository.SaveChangeAsync();

            return model;
        }
        public async Task<bool> Update(MenuUnit model)
        {
            var result = false;
            var record = await _menuUnitRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.Name = model.Name;

                await _menuUnitRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _menuUnitRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;
                await _menuUnitRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }
        public Task<bool> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
        public async Task<IPaginatedList<MenuUnitDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize)
        {
            var MenuUnitRepo = _menuUnitRepository.Repo;

            var result = await (from menuUnit in MenuUnitRepo
                          join restaurant in _restaurantRepository.Repo on menuUnit.RestaurantId equals restaurant.Id into rs
                          from restaurant in rs.DefaultIfEmpty()
                          select new MenuUnitDto
                          {
                              Id = menuUnit.Id,
                              RestaurantId = menuUnit.RestaurantId,
                              RestaurantIdName = restaurant.Name,
                              BranchId = menuUnit.BranchId,
                              Name = menuUnit.Name,
                              Status = menuUnit.Status,
                          })
                          .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                          .Where(c => c.Status < (int)EStatus.All)
                          .OrderBy(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<MenuUnitDto> GetById(int restaurantId, int branchId, int id)
        {
            var result = await _menuUnitRepository.Repo.ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0).Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<MenuUnitDto>(result);
        }
    }
}
