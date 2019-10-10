using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Dtos.PlacedOrderDetail;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class PlacedOrderDetailBusiness : IPlacedOrderDetailBusiness
    {
        private readonly IMapper _mapper;
        private readonly IPlacedOrderDetailRepository _placedOrderDetailRepository;
        private readonly IPlacedOrderRepository _placedOrderRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IMenuRepository _menuRepository;
        private readonly IMenuSizeRepository _menuSizeRepository;
        public PlacedOrderDetailBusiness(IMapper mapper,
            IPlacedOrderDetailRepository placedOrderDetailRepository,
            IRestaurantRepository restaurantRepository,
            IPlacedOrderRepository placedOrderRepository,
            IMenuRepository menuRepository,
            IMenuSizeRepository menuSizeRepository)
        {
            _mapper = mapper;
            _placedOrderDetailRepository = placedOrderDetailRepository;
            _restaurantRepository = restaurantRepository;
            _placedOrderRepository = placedOrderRepository;
            _menuRepository = menuRepository;
            _menuSizeRepository = menuSizeRepository;
        }
        public async Task<PlacedOrderDetail> Add(PlacedOrderDetail model)
        {
            var entity = _placedOrderDetailRepository.Add(model);

            var totalDetailPrice = await GetTotalDetailPriceByOrderId(entity.RestaurantId, entity.BranchId, entity.PlacedOrderId);

            var placedOrder = await _placedOrderRepository.Repo.ToFilterByRole(f => f.RestaurantId,
            f => f.BranchId, entity.RestaurantId, entity.BranchId).Where(c => c.Id == entity.PlacedOrderId)
            .FirstOrDefaultAsync();

            if (placedOrder != null)
            {
                placedOrder.Price = (entity.MenuPrice * entity.Quantity) + totalDetailPrice;
                if (placedOrder.DiscountType == (int)EDiscountType.Percent)
                {
                    placedOrder.FinalPrice = placedOrder.Price + placedOrder.Tax - ((placedOrder.Price * placedOrder.Discount) / 100);
                }
                else if (placedOrder.DiscountType == (int)EDiscountType.Money)
                {
                    placedOrder.FinalPrice = placedOrder.Price + placedOrder.Tax - placedOrder.Discount;
                }

                await _placedOrderDetailRepository.SaveChangeAsync();

                return entity;
            }


            return null;
        }
        public async Task<bool> Update(PlacedOrderDetail model)
        {
            var result = false;
            var record = await _placedOrderDetailRepository.Repo.FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.OfferId = model.OfferId;
                record.MenuId = model.MenuId;
                record.SizeId = model.SizeId;
                record.Quantity = model.Quantity;
                record.MenuPrice = model.MenuPrice;
                record.Price = model.Price;
                record.IsFinish = model.IsFinish;
                record.Description = model.Description;

                var totalDetailPrice = await GetTotalDetailPriceByOrderId(record.RestaurantId, record.BranchId, record.PlacedOrderId);

                var placedOrder = await _placedOrderRepository.Repo.ToFilterByRole(f => f.RestaurantId,
                f => f.BranchId, record.RestaurantId, record.BranchId).Where(c => c.Id == record.PlacedOrderId)
                .FirstOrDefaultAsync();

                if (placedOrder != null)
                {
                    placedOrder.Price = (record.MenuPrice * record.Quantity) + totalDetailPrice;

                    if (placedOrder.DiscountType == (int)EDiscountType.Percent)
                    {
                        placedOrder.FinalPrice = placedOrder.Price + placedOrder.Tax - ((placedOrder.Price * placedOrder.Discount) / 100);
                    }
                    else if (placedOrder.DiscountType == (int)EDiscountType.Money)
                    {
                        placedOrder.FinalPrice = placedOrder.Price + placedOrder.Tax - placedOrder.Discount;
                    }

                    await _placedOrderDetailRepository.SaveChangeAsync();

                    result = true;
                }
            }
            return result;
        }
        public async Task<bool> SetActive(int id, int Active)
        {
            var result = false;
            var record = await _placedOrderDetailRepository.Repo.FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.Status = Active == 1 ? 0 : 1;

                var totalDetailPrice = await GetTotalDetailPriceByOrderId(record.RestaurantId, record.BranchId, record.PlacedOrderId);

                var placedOrder = await _placedOrderRepository.Repo.ToFilterByRole(f => f.RestaurantId,
                f => f.BranchId, record.RestaurantId, record.BranchId).Where(c => c.Id == record.PlacedOrderId)
                .FirstOrDefaultAsync();

                if (placedOrder != null)
                {
                    if (record.Status == (int)EStatus.Using)
                    {
                        placedOrder.Price = (record.MenuPrice * record.Quantity) + totalDetailPrice;
                    }
                    else
                    {
                        placedOrder.Price = totalDetailPrice - (record.MenuPrice * record.Quantity);
                    }

                    if (placedOrder.DiscountType == (int)EDiscountType.Percent)
                    {
                        placedOrder.FinalPrice = placedOrder.Price + placedOrder.Tax - ((placedOrder.Price * placedOrder.Discount) / 100);
                    }
                    else if (placedOrder.DiscountType == (int)EDiscountType.Money)
                    {
                        placedOrder.FinalPrice = placedOrder.Price + placedOrder.Tax - placedOrder.Discount;
                    }

                    await _placedOrderDetailRepository.SaveChangeAsync();

                    result = true;
                }

            }
            return result;
        }
        public Task<bool> Delete(int id)
        {
            throw new System.NotImplementedException();
        }
        public async Task<IPaginatedList<PlacedOrderDetailDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize)
        {
            var PlacedOrderDetailRepo = _placedOrderDetailRepository.Repo;

            var result = await (from placedOrderDetail in PlacedOrderDetailRepo
                                join order in _placedOrderRepository.Repo on placedOrderDetail.PlacedOrderId equals order.Id into os
                                from order in os.DefaultIfEmpty()
                                join menu in _menuRepository.Repo on placedOrderDetail.MenuId equals menu.Id into ms
                                from menu in ms.DefaultIfEmpty()
                                join size in _menuSizeRepository.Repo on placedOrderDetail.SizeId equals size.Id into ss
                                from size in ss.DefaultIfEmpty()
                                select new PlacedOrderDetailDto
                                {
                                    Id = placedOrderDetail.Id,
                                    RestaurantId = placedOrderDetail.RestaurantId,
                                    BranchId = placedOrderDetail.BranchId,
                                    PlacedOrderId = placedOrderDetail.PlacedOrderId,
                                    PlacedOrderIdCode = order.Code,
                                    OfferId = placedOrderDetail.OfferId,
                                    MenuId = placedOrderDetail.MenuId,
                                    MenuIdName = menu.Name,
                                    MenuIdImage = menu.Image,
                                    SizeId = placedOrderDetail.SizeId,
                                    SizeIdName = size.Name,
                                    Quantity = placedOrderDetail.Quantity,
                                    MenuPrice = placedOrderDetail.MenuPrice,
                                    Price = placedOrderDetail.Price,
                                    IsFinish = placedOrderDetail.IsFinish,
                                    Description = placedOrderDetail.Description,
                                    Status = placedOrderDetail.Status
                                })
                          .ToFilterByRole(f => f.RestaurantId, f => f.BranchId, restaurantId, branchId)
                          .Where(c => c.Status < (int)EStatus.All)
                          .OrderBy(c => c.Id)
                          .ToPaginatedListAsync(pageIndex, pageSize);
            return result;
        }
        public async Task<PlacedOrderDetailDto> GetById(int restaurantId, int branchId, int id)
        {
            var result = await _placedOrderDetailRepository.Repo.ToFilterByRole(f => f.RestaurantId, f => f.BranchId, restaurantId, branchId).Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return _mapper.Map<PlacedOrderDetailDto>(result);
        }
        private async Task<double> GetTotalDetailPriceByOrderId(int restaurantId, int branchId, int orderId)
        {
            double total = 0;
            var result = await _placedOrderDetailRepository.Repo.ToFilterByRole(f => f.RestaurantId, f => f.BranchId, restaurantId, branchId)
                .Where(c => c.PlacedOrderId == orderId && c.Status == (int)EStatus.Using)
                .ToListAsync();

            if (result.Count > 0)
            {
                foreach (var item in result)
                {
                    total += item.Price.GetValueOrDefault();
                }
            }

            return total;
        }
        public async Task<List<PlacedOrderDetailDto>> GetWaitingOrderDetail(int restaurantId, int branchId)
        {
            var PlacedOrderDetailRepo = _placedOrderDetailRepository.Repo;

            var result = await (from placedOrderDetail in PlacedOrderDetailRepo
                                join order in _placedOrderRepository.Repo on placedOrderDetail.PlacedOrderId equals order.Id into os
                                from order in os.DefaultIfEmpty()
                                join menu in _menuRepository.Repo on placedOrderDetail.MenuId equals menu.Id into ms
                                from menu in ms.DefaultIfEmpty()
                                join size in _menuSizeRepository.Repo on placedOrderDetail.SizeId equals size.Id into ss
                                from size in ss.DefaultIfEmpty()
                                select new PlacedOrderDetailDto
                                {
                                    Id = placedOrderDetail.Id,
                                    RestaurantId = placedOrderDetail.RestaurantId,
                                    BranchId = placedOrderDetail.BranchId,
                                    PlacedOrderId = placedOrderDetail.PlacedOrderId,
                                    PlacedOrderIdCode = order.Code,
                                    PlacedOrderIdOrderProcessId = order.OrderProcessId.GetValueOrDefault(),
                                    OfferId = placedOrderDetail.OfferId,
                                    MenuId = placedOrderDetail.MenuId,
                                    MenuIdName = menu.Name,
                                    MenuIdImage = menu.Image,
                                    SizeId = placedOrderDetail.SizeId,
                                    SizeIdName = size.Name,
                                    Quantity = placedOrderDetail.Quantity,
                                    MenuPrice = placedOrderDetail.MenuPrice,
                                    Price = placedOrderDetail.Price,
                                    IsFinish = placedOrderDetail.IsFinish,
                                    Description = placedOrderDetail.Description,
                                    Status = placedOrderDetail.Status
                                })
                          .ToFilterByRole(f => f.RestaurantId, f => f.BranchId, restaurantId, branchId)
                          .Where(c => c.Status < (int)EStatus.All && (c.PlacedOrderIdOrderProcessId == (int)EOrderProcess.WaitingOrder ||
                          c.PlacedOrderIdOrderProcessId == (int)EOrderProcess.PreparingOrder ||
                          c.PlacedOrderIdOrderProcessId == (int)EOrderProcess.AddMoreOrder))
                          .OrderBy(c => c.Id)
                          .ToListAsync();
            return result;
        }
        public async Task<PlacedOrderDetail> SetFinishOrderDetail(int restaurantId, int branchId, int id, int isFinish)
        {
            var record = await _placedOrderDetailRepository.Repo
                .ToFilterByRole(f => f.RestaurantId, f => f.BranchId, restaurantId, branchId)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (record != null)
            {
                record.IsFinish = isFinish == 1 ? 0 : 1;
                await _placedOrderDetailRepository.SaveChangeAsync();
            }
            return record;
        }
        public async Task<List<PlacedOrderDetailDto>> GetWaitingOrderDetailByOrderId(int restaurantId, int branchId, int orderId)
        {
            var PlacedOrderDetailRepo = _placedOrderDetailRepository.Repo;

            var result = await (from placedOrderDetail in PlacedOrderDetailRepo
                                join order in _placedOrderRepository.Repo on placedOrderDetail.PlacedOrderId equals order.Id into os
                                from order in os.DefaultIfEmpty()
                                join menu in _menuRepository.Repo on placedOrderDetail.MenuId equals menu.Id into ms
                                from menu in ms.DefaultIfEmpty()
                                join size in _menuSizeRepository.Repo on placedOrderDetail.SizeId equals size.Id into ss
                                from size in ss.DefaultIfEmpty()
                                select new PlacedOrderDetailDto
                                {
                                    Id = placedOrderDetail.Id,
                                    RestaurantId = placedOrderDetail.RestaurantId,
                                    BranchId = placedOrderDetail.BranchId,
                                    PlacedOrderId = placedOrderDetail.PlacedOrderId,
                                    PlacedOrderIdCode = order.Code,
                                    PlacedOrderIdOrderProcessId = order.OrderProcessId.GetValueOrDefault(),
                                    OfferId = placedOrderDetail.OfferId,
                                    MenuId = placedOrderDetail.MenuId,
                                    MenuIdName = menu.Name,
                                    MenuIdImage = menu.Image,
                                    SizeId = placedOrderDetail.SizeId,
                                    SizeIdName = size.Name,
                                    Quantity = placedOrderDetail.Quantity,
                                    MenuPrice = placedOrderDetail.MenuPrice,
                                    Price = placedOrderDetail.Price,
                                    IsFinish = placedOrderDetail.IsFinish,
                                    Description = placedOrderDetail.Description,
                                    Status = placedOrderDetail.Status
                                })
                          .ToFilterByRole(f => f.RestaurantId, f => f.BranchId, restaurantId, branchId)
                          .Where(c => c.Status < (int)EStatus.All && c.PlacedOrderId == orderId)
                          .OrderBy(c => c.Id)
                          .ToListAsync();
            return result;
        }
    }
}
