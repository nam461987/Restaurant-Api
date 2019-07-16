using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.MenuUnit;
using Restaurant.Entities.Models;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IMenuUnitBusiness
    {
        Task<MenuUnit> Add(MenuUnit model);
        Task<bool> Update(MenuUnit model);
        Task<bool> SetActive(int id, int Active);
        Task<IPaginatedList<MenuUnitDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize);
        Task<MenuUnitDto> GetById(int restaurantId, int branchId, int id);
    }
}
