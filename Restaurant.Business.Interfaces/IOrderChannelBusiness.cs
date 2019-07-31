using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.OrderChannel;
using Restaurant.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IOrderChannelBusiness
    {
        Task<OrderChannel> Add(OrderChannel model);
        Task<bool> Update(OrderChannel model);
        Task<bool> SetActive(int id, int Active);
        Task<IPaginatedList<OrderChannelDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize);
        Task<OrderChannelDto> GetById(int restaurantId, int branchId, int id);
        Task<List<OrderChannel>> GetAllNotPaginate(int restaurantId, int branchId);
    }
}
