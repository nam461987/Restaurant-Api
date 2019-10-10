using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Constants;
using Restaurant.Common.Dtos.RestaurantBranch;
using Restaurant.Common.Enums;
using Restaurant.Common.Filters;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class RestaurantBranchBusiness : IRestaurantBranchBusiness
    {
        private readonly IMapper _mapper;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IRestaurantBranchRepository _restaurantBranchRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IStateCityRepository _stateCityRepository;
        public RestaurantBranchBusiness(IMapper mapper,
            IRestaurantRepository restaurantRepository,
            IRestaurantBranchRepository restaurantBranchRepository,
            IStateRepository stateRepository,
            IStateCityRepository stateCityRepository)
        {
            _mapper = mapper;
            _restaurantRepository = restaurantRepository;
            _restaurantBranchRepository = restaurantBranchRepository;
            _stateRepository = stateRepository;
            _stateCityRepository = stateCityRepository;
        }
        public async Task<RestaurantBranch> Add(RestaurantBranch model)
        {
            var entity = _restaurantBranchRepository.Add(model);
            await _restaurantBranchRepository.SaveChangeAsync();

            return model;
        }
        public async Task<bool> Update(RestaurantBranch model)
        {
            var result = false;
            var record = await _restaurantBranchRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.Name = model.Name;
                record.StateId = model.StateId;
                record.CityId = model.CityId;
                record.Zip = model.Zip;
                record.Address = model.Address;
                record.Phone = model.Phone;
                record.OpenTime = model.OpenTime;
                record.CloseTime = model.CloseTime;
                record.AllDay = model.AllDay;

                await _restaurantBranchRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _restaurantBranchRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;
                await _restaurantBranchRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }
        public Task<bool> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
        public async Task<IPaginatedList<BranchDto>> GetAll(int restaurantId, int pageIndex, int pageSize)
        {
            var restaurantBranchRepo = _restaurantBranchRepository.Repo;

            var result = await (from restaurantBranch in restaurantBranchRepo
                          join restaurant in _restaurantRepository.Repo on restaurantBranch.RestaurantId equals restaurant.Id into rs
                          from restaurant in rs.DefaultIfEmpty()
                          join state in _stateRepository.Repo on restaurantBranch.StateId equals state.Id into ss
                          from state in ss.DefaultIfEmpty()
                          join city in _stateCityRepository.Repo on restaurantBranch.CityId equals city.Id into cs
                          from city in cs.DefaultIfEmpty()
                          select new BranchDto
                          {
                              Id = restaurantBranch.Id,
                              RestaurantId = restaurantBranch.RestaurantId,
                              RestaurantIdName = restaurant.Name,
                              Name = restaurantBranch.Name,
                              StateId = restaurantBranch.StateId.GetValueOrDefault(),
                              StateIdName = state.Name,
                              CityId = restaurantBranch.CityId.GetValueOrDefault(),
                              CityIdName = city.Name,
                              Zip = restaurantBranch.Zip.GetValueOrDefault(),
                              Address = restaurantBranch.Address,
                              Phone = restaurantBranch.Phone,
                              OpenTime = restaurantBranch.OpenTime,
                              CloseTime = restaurantBranch.CloseTime,
                              AllDay = restaurantBranch.AllDay,
                              Status = restaurantBranch.Status,
                          })
                          .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                          .Where(c => c.Status < (int)EStatus.All)
                          .OrderBy(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<BranchDto> GetById(int restaurantId, int id)
        {
            var restaurantBranchRepo = _restaurantBranchRepository.Repo;

            var result = await (from restaurantBranch in restaurantBranchRepo
                                join restaurant in _restaurantRepository.Repo on restaurantBranch.RestaurantId equals restaurant.Id into rs
                                from restaurant in rs.DefaultIfEmpty()
                                join state in _stateRepository.Repo on restaurantBranch.StateId equals state.Id into ss
                                from state in ss.DefaultIfEmpty()
                                join city in _stateCityRepository.Repo on restaurantBranch.CityId equals city.Id into cs
                                from city in cs.DefaultIfEmpty()
                                select new BranchDto
                                {
                                    Id = restaurantBranch.Id,
                                    RestaurantId = restaurantBranch.RestaurantId,
                                    RestaurantIdName = restaurant.Name,
                                    Name = restaurantBranch.Name,
                                    StateId = restaurantBranch.StateId.GetValueOrDefault(),
                                    StateIdName = state.Name,
                                    CityId = restaurantBranch.CityId.GetValueOrDefault(),
                                    CityIdName = city.Name,
                                    Zip = restaurantBranch.Zip.GetValueOrDefault(),
                                    Address = restaurantBranch.Address,
                                    Phone = restaurantBranch.Phone,
                                    OpenTime = restaurantBranch.OpenTime,
                                    CloseTime = restaurantBranch.CloseTime,
                                    AllDay = restaurantBranch.AllDay,
                                    Status = restaurantBranch.Status,
                                })
                          .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                          .Where(c => c.Id == id)
                          .OrderBy(c => c.Id)
                          .FirstOrDefaultAsync();
            return result;
        }
    }
}