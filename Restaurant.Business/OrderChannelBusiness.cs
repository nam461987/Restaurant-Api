using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Dtos.OrderChannel;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class OrderChannelBusiness : IOrderChannelBusiness
    {
        private readonly IMapper _mapper;
        private readonly IOrderChannelRepository _orderChannelRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        public OrderChannelBusiness(IMapper mapper,
            IOrderChannelRepository orderChannelRepository,
            IRestaurantRepository restaurantRepository)
        {
            _mapper = mapper;
            _orderChannelRepository = orderChannelRepository;
            _restaurantRepository = restaurantRepository;
        }
        public async Task<OrderChannel> Add(OrderChannel model)
        {
            var entity = _orderChannelRepository.Add(model);
            await _orderChannelRepository.SaveChangeAsync();

            return model;
        }
        public async Task<bool> Update(OrderChannel model)
        {
            var result = false;
            var record = await _orderChannelRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.Name = model.Name;
                record.OpenTime = model.OpenTime;
                record.CloseTime = model.CloseTime;
                record.AllDay = model.AllDay;
                record.Description = model.Description;

                await _orderChannelRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _orderChannelRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;
                await _orderChannelRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }
        public Task<bool> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
        public Task<IPaginatedList<OrderChannelDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize)
        {
            var OrderChannelRepo = _orderChannelRepository.Repo;

            var result = (from orderChannel in OrderChannelRepo
                          join restaurant in _restaurantRepository.Repo on orderChannel.RestaurantId equals restaurant.Id into rs
                          from restaurant in rs.DefaultIfEmpty()
                          select new OrderChannelDto
                          {
                              Id = orderChannel.Id,
                              RestaurantId = orderChannel.RestaurantId,
                              RestaurantIdName = restaurant.Name,
                              BranchId = orderChannel.BranchId,
                              Name = orderChannel.Name,
                              OpenTime = orderChannel.OpenTime,
                              CloseTime = orderChannel.CloseTime,
                              AllDay = orderChannel.AllDay,
                              Description = orderChannel.Description,
                              Status = orderChannel.Status,
                          })
                          .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                          .Where(c => c.Status < (int)EStatus.All)
                          .OrderBy(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<OrderChannelDto> GetById(int restaurantId, int branchId, int id)
        {
            var result = await _orderChannelRepository.Repo.ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0).Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<OrderChannelDto>(result);
        }
    }
}
