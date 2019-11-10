using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Dtos.OrderChannelTime;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using Restaurant.Repository.Interfaces.Orders;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class OrderChannelTimeBusiness : IOrderChannelTimeBusiness
    {
        private readonly IMapper _mapper;
        private readonly IOrderChannelTimeRepository _orderChannelTimeRepository;
        private readonly IOrderChannelRepository _orderChannelRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        public OrderChannelTimeBusiness(IMapper mapper,
            IOrderChannelTimeRepository orderChannelTimeRepository,
            IOrderChannelRepository orderChannelRepository,
            IRestaurantRepository restaurantRepository)
        {
            _mapper = mapper;
            _orderChannelTimeRepository = orderChannelTimeRepository;
            _orderChannelRepository = orderChannelRepository;
            _restaurantRepository = restaurantRepository;
        }
        public async Task<OrderChannelTime> Add(OrderChannelTime model)
        {
            var entity = _orderChannelTimeRepository.Add(model);
            await _orderChannelTimeRepository.SaveChangeAsync();

            return model;
        }
        public async Task<bool> Update(OrderChannelTime model)
        {
            var result = false;
            var record = await _orderChannelTimeRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.OrderChannelId = model.OrderChannelId;
                record.OpenTime = model.OpenTime;
                record.CloseTime = model.CloseTime;

                await _orderChannelTimeRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }
        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _orderChannelTimeRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;
                await _orderChannelTimeRepository.SaveChangeAsync();
                result = true;
            }
            return result;
        }
        public Task<bool> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
        public async Task<IPaginatedList<OrderChannelTimeDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize)
        {
            var OrderChannelTimeRepo = _orderChannelTimeRepository.Repo;

            var result = await (from orderChannelTime in OrderChannelTimeRepo
                          join restaurant in _restaurantRepository.Repo on orderChannelTime.RestaurantId equals restaurant.Id into rs
                          from restaurant in rs.DefaultIfEmpty()
                          join orderChannel in _orderChannelRepository.Repo on orderChannelTime.OrderChannelId equals orderChannel.Id into os
                          from orderchannel in os.DefaultIfEmpty()
                          select new OrderChannelTimeDto
                          {
                              Id = orderChannelTime.Id,
                              RestaurantId = orderChannelTime.RestaurantId,
                              RestaurantIdName = restaurant.Name,
                              BranchId = orderChannelTime.BranchId,
                              OrderChannelId = orderChannelTime.OrderChannelId,
                              OrderChannelIdName = orderchannel.Name,
                              OpenTime = orderChannelTime.OpenTime,
                              CloseTime = orderChannelTime.CloseTime,
                              Status = orderChannelTime.Status
                          })
                          .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                          .Where(c => c.Status < (int)EStatus.All)
                          .OrderBy(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<OrderChannelTimeDto> GetById(int restaurantId, int branchId, int id)
        {
            var result = await _orderChannelTimeRepository.Repo.ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0).Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<OrderChannelTimeDto>(result);
        }
    }
}
