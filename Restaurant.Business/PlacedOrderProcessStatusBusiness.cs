using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Common.Dtos.PlacedOrderProcessStatus;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using Restaurant.Repository.Interfaces.Orders;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class PlacedOrderProcessStatusBusiness : IPlacedOrderProcessStatusBusiness
    {
        private readonly IMapper _mapper;
        private readonly IPlacedOrderProcessStatusRepository _placedOrderProcessStatusRepository;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IRestaurantBranchRepository _restaurantBranchRepository;
        private readonly IOrderProcessRepository _orderProcessRepository;
        private readonly IAdminAccountRepository _adminAccountRepository;
        private readonly IPlacedOrderRepository _placedOrderRepository;
        public PlacedOrderProcessStatusBusiness(IMapper mapper,
            IPlacedOrderProcessStatusRepository placedOrderProcessStatusRepository,
            IRestaurantRepository restaurantRepository,
            IRestaurantBranchRepository restaurantBranchRepository,
            IOrderProcessRepository orderProcessRepository,
            IAdminAccountRepository adminAccountRepository,
            IPlacedOrderRepository placedOrderRepository)
        {
            _mapper = mapper;
            _placedOrderProcessStatusRepository = placedOrderProcessStatusRepository;
            _restaurantRepository = restaurantRepository;
            _restaurantBranchRepository = restaurantBranchRepository;
            _orderProcessRepository = orderProcessRepository;
            _adminAccountRepository = adminAccountRepository;
            _placedOrderRepository = placedOrderRepository;
        }
        public async Task<PlacedOrderProcessStatus> Add(PlacedOrderProcessStatus model)
        {
            var entity = _placedOrderProcessStatusRepository.Add(model);

            if (entity != null)
            {
                var placedOrder = new PlacedOrder()
                {
                    RestaurantId = entity.RestaurantId,
                    BranchId = entity.BranchId,
                    Id = entity.PlacedOrderId,
                    OrderProcessId = entity.OrderProcessId
                };

                // Update Order Process to Placed Order
                var result = await UpdateOrderProcess(placedOrder);

                if (result)
                {
                    await _placedOrderProcessStatusRepository.SaveChangeAsync();
                    return model;
                }
            }
            return null;
        }

        private async Task<bool> UpdateOrderProcess(PlacedOrder model)
        {
            var result = false;
            var record = await _placedOrderRepository.Repo
                .ToFilterByRole(f => f.RestaurantId,
                f => f.BranchId, model.RestaurantId, model.BranchId)
                .FirstOrDefaultAsync(c => c.Id == model.Id);

            if (record != null)
            {
                record.OrderProcessId = model.OrderProcessId;

                await _placedOrderRepository.SaveChangeAsync();

                result = true;
            }
            return result;
        }

        //public async Task<List<PlacedOrderProcessStatus>> GetByOrderId(int restaurantId, int branchId, int orderId)
        //{
        //    var result = await _placedOrderProcessStatusRepository.Repo.ToFilterByRole(f => f.RestaurantId,
        //        f => f.BranchId, restaurantId, branchId).Where(c => c.Id == orderId)
        //        .ToListAsync();

        //    return result;
        //}
        public async Task<int> GetLastStatusByOrderId(int restaurantId, int branchId, int orderId)
        {
            var result = await _placedOrderProcessStatusRepository.Repo.ToFilterByRole(f => f.RestaurantId,
                f => f.BranchId, restaurantId, branchId).Where(c => c.Id == orderId)
                .MaxAsync(c => c.Id);

            return result;
        }
        public async Task<bool> CheckProcessStatusExist(int restaurantId, int branchId, int orderId, int processId)
        {
            var result = false;
            var maxId = await _placedOrderProcessStatusRepository.Repo.ToFilterByRole(f => f.RestaurantId,
                f => f.BranchId, restaurantId, branchId).Where(c => c.PlacedOrderId == orderId)
                .MaxAsync(c => c.Id);
            var record = await _placedOrderProcessStatusRepository.Repo.ToFilterByRole(f => f.RestaurantId,
                f => f.BranchId, restaurantId, branchId).Where(c => c.PlacedOrderId == orderId && c.Id == maxId)
                .FirstOrDefaultAsync();
            if (record != null && record.OrderProcessId == processId)
            {
                result = true;
            }
            return result;
        }
        public async Task<List<PlacedOrderProcessStatusDto>> GetByOrderId(int restaurantId, int branchId, int orderId)
        {
            var PlacedOrderProcessStatusRepo = _placedOrderProcessStatusRepository.Repo;

            var result = await (from placedOrderProcessStatus in PlacedOrderProcessStatusRepo
                                join restaurant in _restaurantRepository.Repo on placedOrderProcessStatus.RestaurantId equals restaurant.Id into rs
                                from restaurant in rs.DefaultIfEmpty()
                                join branch in _restaurantBranchRepository.Repo on placedOrderProcessStatus.BranchId equals branch.Id into bs
                                from branch in bs.DefaultIfEmpty()
                                join process in _orderProcessRepository.Repo on placedOrderProcessStatus.OrderProcessId equals process.Id into ps
                                from process in ps.DefaultIfEmpty()
                                join account in _adminAccountRepository.Repo on placedOrderProcessStatus.OrderProcessId equals account.Id into aas
                                from account in aas.DefaultIfEmpty()
                                select new PlacedOrderProcessStatusDto
                                {
                                    Id = placedOrderProcessStatus.Id,
                                    RestaurantId = placedOrderProcessStatus.RestaurantId,
                                    RestaurantIdName = restaurant.Name,
                                    BranchId = placedOrderProcessStatus.BranchId,
                                    BranchIdName = branch.Name,
                                    PlacedOrderId = placedOrderProcessStatus.PlacedOrderId,
                                    OrderProcessId = placedOrderProcessStatus.OrderProcessId,
                                    OrderProcessIdName = process.Name,
                                    OrderProcessIdColor = process.Color,
                                    CreatedStaffId = placedOrderProcessStatus.CreatedStaffId,
                                    CreatedStaffIdName = account.UserName,
                                    CreatedDate = placedOrderProcessStatus.CreatedDate,
                                    Status = placedOrderProcessStatus.Status,
                                })
                          .ToFilterByRole(f => f.RestaurantId, null, restaurantId, 0)
                          .Where(c => c.Status == (int)EStatus.Using && c.PlacedOrderId == orderId)
                          .OrderBy(c => c.Id)
                          .ToListAsync();
            return result;
        }
    }
}
