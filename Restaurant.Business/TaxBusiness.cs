using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Dtos.Tax;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using Restaurant.Repository.Interfaces.Menus;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class TaxBusiness : ITaxBusiness
    {
        private readonly IMapper _mapper;
        private readonly ITaxRepository _taxRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IMenuUnitRepository _menuUnitRepository;
        public TaxBusiness(IMapper mapper,
            ITaxRepository taxRepository,
            IRestaurantRepository restaurantRepository,
            IMenuUnitRepository menuUnitRepository)
        {
            _mapper = mapper;
            _taxRepository = taxRepository;
            _restaurantRepository = restaurantRepository;
            _menuUnitRepository = menuUnitRepository;
        }
        public async Task<Tax> Add(Tax model)
        {
            var entity = _taxRepository.Add(model);
            await _taxRepository.SaveChangeAsync();

            return model;
        }
        public async Task<bool> Update(Tax model)
        {
            var result = false;
            var record = await _taxRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.Name = model.Name;
                record.Percentage = model.Percentage;

                await _taxRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _taxRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;
                await _taxRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }
        public Task<bool> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
        public async Task<IPaginatedList<TaxDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize)
        {
            var TaxRepo = _taxRepository.Repo;

            var result = await (from tax in TaxRepo
                          join restaurant in _restaurantRepository.Repo on tax.RestaurantId equals restaurant.Id into rs
                          from restaurant in rs.DefaultIfEmpty()
                          select new TaxDto
                          {
                              Id = tax.Id,
                              RestaurantId = tax.RestaurantId,
                              RestaurantIdName = restaurant.Name,
                              BranchId = tax.BranchId,
                              Name = tax.Name,
                              Percentage = tax.Percentage,
                              Status = tax.Status,
                          })
                          .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                          .Where(c => c.Status < (int)EStatus.All)
                          .OrderBy(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<TaxDto> GetById(int restaurantId, int branchId, int id)
        {
            var result = await _taxRepository.Repo.ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0).Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<TaxDto>(result);
        }
    }
}
