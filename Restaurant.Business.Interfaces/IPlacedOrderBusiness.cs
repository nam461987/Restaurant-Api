using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.PlacedOrder;
using Restaurant.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IPlacedOrderBusiness
    {
        Task<PlacedOrder> Add(PlacedOrder model);
        Task<bool> Update(PlacedOrder model);
        Task<bool> SetActive(int id, int Active);
        Task<IPaginatedList<PlacedOrderDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize);
        Task<PlacedOrderDto> GetById(int restaurantId, int branchId, int id);
        Task<bool> UpdatePriceToPlacedOrder(PlacedOrder model);
        Task<bool> UpdateOrderProcess(PlacedOrder model);
        Task<List<PlacedOrderDto>> GetWaitingOrder(int restaurantId, int branchId);

    }
}
