using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Arrays;
using Restaurant.Common.Dtos.Restaurant;
using Restaurant.Common.Enums;
using Restaurant.Repository.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class RestaurantBusiness : IRestaurantBusiness
    {
        private readonly IMapper _mapper;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IStateCityRepository _stateCityRepository;
        public RestaurantBusiness(IMapper mapper,
            IRestaurantRepository restaurantRepository,
            IStateRepository stateRepository,
            IStateCityRepository stateCityRepository)
        {
            _mapper = mapper;
            _restaurantRepository = restaurantRepository;
            _stateRepository = stateRepository;
            _stateCityRepository = stateCityRepository;
        }
        public async Task<Entities.Models.Restaurant> Add(Entities.Models.Restaurant model)
        {
            var entity = _restaurantRepository.Add(model);
            await _restaurantRepository.SaveChangeAsync();

            return model;
        }
        public async Task<bool> Update(Entities.Models.Restaurant model)
        {
            var result = false;
            var record = await _restaurantRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.TypeId = model.TypeId;
                record.Name = model.Name;
                record.Phone = model.Phone;
                record.Email = model.Email;
                record.StateId = model.StateId;
                record.CityId = model.CityId;
                record.Zip = model.Zip;
                record.Address = model.Address;
                record.Description = model.Description;

                await _restaurantRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _restaurantRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;
                await _restaurantRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }
        public Task<bool> Delete(int id)
        {
            //_customerRepository.Delete(id);
            //var recordUpdated = await _customerRepository.SaveChangeAsync();
            //return recordUpdated > 0;
            throw new System.NotImplementedException();
        }        
        public async Task<IPaginatedList<RestaurantDto>> GetAll(int pageIndex, int pageSize)
        {
            var restaurantRepo = _restaurantRepository.Repo;

            var result = await (from restaurant in restaurantRepo                          
                          join state in _stateRepository.Repo on restaurant.StateId equals state.Id into ss
                          from state in ss.DefaultIfEmpty()
                          join city in _stateCityRepository.Repo on restaurant.CityId equals city.Id into cs
                          from city in cs.DefaultIfEmpty()
                          select new RestaurantDto
                          {
                              Id = restaurant.Id,
                              TypeId = restaurant.TypeId,
                              TypeIdName = ARestaurantType.RestaurantType[restaurant.TypeId],
                              Name = restaurant.Name,
                              Email = restaurant.Email,
                              StateId = restaurant.StateId,
                              StateIdName = state.Name,
                              CityId = restaurant.CityId,
                              CityIdName = city.Name,
                              Zip = restaurant.Zip,
                              Address = restaurant.Address,
                              Phone = restaurant.Phone,
                              Description = restaurant.Description,
                              Status = restaurant.Status,
                          })
                          .Where(c => c.Status < (int)EStatus.All)
                          .OrderBy(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<RestaurantDto> GetById(int id)
        {
            var result = await _restaurantRepository.Repo.Where(c => c.Id == id)
                .FirstOrDefaultAsync();
            return _mapper.Map<RestaurantDto>(result);
        }
    }
}
