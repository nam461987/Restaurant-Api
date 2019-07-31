using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.PlacedOrderDetail;
using Restaurant.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IPlacedOrderDetailBusiness
    {
        Task<PlacedOrderDetail> Add(PlacedOrderDetail model);
        Task<bool> Update(PlacedOrderDetail model);
        Task<bool> SetActive(int id, int Active);
        Task<IPaginatedList<PlacedOrderDetailDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize);
        Task<PlacedOrderDetailDto> GetById(int restaurantId, int branchId, int id);
        Task<double> GetTotalDetailPriceByOrderId(int restaurantId, int branchId, int orderId);
        Task<List<PlacedOrderDetailDto>> GetWaitingOrderDetail(int restaurantId, int branchId);
        Task<PlacedOrderDetail> SetFinishOrderDetail(int restaurantId, int branchId, int id, int isFinish);
    }
}
