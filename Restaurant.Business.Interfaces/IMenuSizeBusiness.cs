using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.MenuSize;
using Restaurant.Entities.Models;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IMenuSizeBusiness
    {
        Task<MenuSize> Add(MenuSize model);
        Task<bool> Update(MenuSize model);
        Task<bool> SetActive(int id, int Active);
        Task<IPaginatedList<MenuSizeDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize);
        Task<MenuSizeDto> GetById(int restaurantId, int branchId, int id);
    }
}
