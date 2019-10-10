using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Dtos.RestaurantTable;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class RestaurantTableBusiness : IRestaurantTableBusiness
    {
        private readonly IMapper _mapper;
        private readonly IRestaurantTableRepository _restaurantTableRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IRestaurantBranchRepository _restaurantBranchRepository;
        public RestaurantTableBusiness(IMapper mapper,
            IRestaurantTableRepository restaurantTableRepository,
            IRestaurantRepository restaurantRepository,
            IRestaurantBranchRepository restaurantBranchRepository)
        {
            _mapper = mapper;
            _restaurantTableRepository = restaurantTableRepository;
            _restaurantRepository = restaurantRepository;
            _restaurantBranchRepository = restaurantBranchRepository;
        }
        public async Task<RestaurantTable> Add(RestaurantTable model)
        {
            var entity = _restaurantTableRepository.Add(model);
            await _restaurantTableRepository.SaveChangeAsync();

            return model;
        }
        public async Task<bool> Update(RestaurantTable model)
        {
            var result = false;
            var record = await _restaurantTableRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.Name = model.Name;
                record.Capacity = model.Capacity;
                record.Location = model.Location;
                record.Description = model.Description;

                await _restaurantTableRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _restaurantTableRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;
                await _restaurantTableRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }
        public Task<bool> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
        public async Task<IPaginatedList<RestaurantTableDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize)
        {
            var RestaurantTableRepo = _restaurantTableRepository.Repo;

            var result = await (from restaurantTable in RestaurantTableRepo
                          join restaurant in _restaurantRepository.Repo on restaurantTable.RestaurantId equals restaurant.Id into rs
                          from restaurant in rs.DefaultIfEmpty()
                          join branch in _restaurantBranchRepository.Repo on restaurantTable.BranchId equals branch.Id into bs
                          from branch in bs.DefaultIfEmpty()
                          select new RestaurantTableDto
                          {
                              Id = restaurantTable.Id,
                              RestaurantId = restaurantTable.RestaurantId,
                              RestaurantIdName = restaurant.Name,
                              BranchId = restaurantTable.BranchId,
                              BranchIdName = branch.Name,
                              Name = restaurantTable.Name,
                              Capacity = restaurantTable.Capacity,
                              Location = restaurantTable.Location,
                              Description = restaurantTable.Description,
                              Status = restaurantTable.Status
                          })
                          .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                          .Where(c => c.Status < (int)EStatus.All)
                          .OrderBy(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<RestaurantTableDto> GetById(int restaurantId, int branchId, int id)
        {
            var result = await _restaurantTableRepository.Repo.ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0).Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<RestaurantTableDto>(result);
        }
        public async Task<List<RestaurantTable>> GetAllNotPaginate(int restaurantId, int branchId)
        {
            var result = await _restaurantTableRepository.Repo.ToFilterByRole(f => f.RestaurantId, f => f.BranchId, restaurantId, branchId)
                .Where(c => c.Status == (int)EStatus.Using)
                .ToListAsync();

            return result;
        }
    }
}
