using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.RestaurantTable;
using Restaurant.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IRestaurantTableBusiness
    {
        Task<RestaurantTable> Add(RestaurantTable model);
        Task<bool> Update(RestaurantTable model);
        Task<bool> SetActive(int id, int Active);
        Task<IPaginatedList<RestaurantTableDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize);
        Task<RestaurantTableDto> GetById(int restaurantId, int branchId, int id);
        Task<List<RestaurantTable>> GetAllNotPaginate(int restaurantId, int branchId);
    }
}
