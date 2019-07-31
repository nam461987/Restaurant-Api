using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IPlacedOrderProcessStatusBusiness
    {
        Task<PlacedOrderProcessStatus> Add(PlacedOrderProcessStatus model);
        Task<List<PlacedOrderProcessStatus>> GetByOrderId(int restaurantId, int branchId, int orderId);
        Task<int> GetLastStatusByOrderId(int restaurantId, int branchId, int orderId);
        Task<bool> CheckProcessStatusExist(int restaurantId, int branchId, int orderId, int processId);
    }
}
