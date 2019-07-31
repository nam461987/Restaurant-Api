using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Dtos.PlacedOrder;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class PlacedOrderProcessStatusBusiness : IPlacedOrderProcessStatusBusiness
    {
        private readonly IMapper _mapper;
        private readonly IPlacedOrderProcessStatusRepository _placedOrderProcessStatusRepository;
        public PlacedOrderProcessStatusBusiness(IMapper mapper,
            IPlacedOrderProcessStatusRepository placedOrderProcessStatusRepository)
        {
            _mapper = mapper;
            _placedOrderProcessStatusRepository = placedOrderProcessStatusRepository;
        }
        public async Task<PlacedOrderProcessStatus> Add(PlacedOrderProcessStatus model)
        {
            var entity = _placedOrderProcessStatusRepository.Add(model);
            await _placedOrderProcessStatusRepository.SaveChangeAsync();

            return model;
        }
        public async Task<List<PlacedOrderProcessStatus>> GetByOrderId(int restaurantId, int branchId, int orderId)
        {
            var result = await _placedOrderProcessStatusRepository.Repo.ToFilterByRole(f => f.RestaurantId,
                f => f.BranchId, restaurantId, branchId).Where(c => c.Id == orderId)
                .ToListAsync();

            return result;
        }
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
    }
}
