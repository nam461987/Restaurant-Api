using Restaurant.Business.Interfaces.Paginated;
using System.Threading.Tasks;
using Restaurant.Entities.Models;
using Restaurant.Common.Filters;
using Restaurant.Common.Dtos.RestaurantBranch;

namespace Restaurant.Business.Interfaces
{
    public interface IRestaurantBranchBusiness
    {
        Task<RestaurantBranch> Add(RestaurantBranch model);
        Task<bool> Update(RestaurantBranch model);
        Task<bool> SetActive(int id, int isActive);
        Task<bool> Delete(int id);
        Task<IPaginatedList<BranchDto>> GetAll(int restaurantId, int pageIndex, int pageSize);
        Task<BranchDto> GetById(int restaurantId, int id);
    }
}
