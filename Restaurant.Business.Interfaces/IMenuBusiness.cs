using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.Menu;
using Restaurant.Entities.Models;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IMenuBusiness
    {
        Task<Menu> Add(Menu model);
        Task<bool> Update(Menu model);
        Task<bool> SetActive(int id, int Active);
        Task<IPaginatedList<MenuDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize);
        Task<MenuDto> GetById(int restaurantId, int branchId, int id);
    }
}
