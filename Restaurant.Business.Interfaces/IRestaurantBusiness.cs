using Restaurant.Business.Interfaces.Paginated;
using System.Threading.Tasks;
using Restaurant.Entities.Models;
using Restaurant.Common.Filters;
using Restaurant.Common.Dtos.Restaurant;

namespace Restaurant.Business.Interfaces
{
    public interface IRestaurantBusiness
    {
        Task<Entities.Models.Restaurant> Add(Entities.Models.Restaurant model);
        Task<bool> Update(Entities.Models.Restaurant model);
        Task<bool> SetActive(int id, int isActive);
        Task<bool> Delete(int id);
        Task<IPaginatedList<RestaurantDto>> GetAll(int pageIndex, int pageSize);
        Task<RestaurantDto> GetById(int id);
    }
}
