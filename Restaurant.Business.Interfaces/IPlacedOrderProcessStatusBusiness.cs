using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.PlacedOrderProcessStatus;
using Restaurant.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IPlacedOrderProcessStatusBusiness
    {
        Task<PlacedOrderProcessStatus> Add(PlacedOrderProcessStatus model);
        Task<int> GetLastStatusByOrderId(int restaurantId, int branchId, int orderId);
        Task<bool> CheckProcessStatusExist(int restaurantId, int branchId, int orderId, int processId);
        Task<List<PlacedOrderProcessStatusDto>> GetByOrderId(int restaurantId, int branchId, int orderId);
    }
}
