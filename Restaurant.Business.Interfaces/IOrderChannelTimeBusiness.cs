using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.OrderChannelTime;
using Restaurant.Entities.Models;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IOrderChannelTimeBusiness
    {
        Task<OrderChannelTime> Add(OrderChannelTime model);
        Task<bool> Update(OrderChannelTime model);
        Task<bool> SetActive(int id, int Active);
        Task<IPaginatedList<OrderChannelTimeDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize);
        Task<OrderChannelTimeDto> GetById(int restaurantId, int branchId, int id);
    }
}
