using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.MenuDefinition;
using Restaurant.Entities.Models;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IMenuDefinitionBusiness
    {
        Task<MenuDefinition> Add(MenuDefinition model);
        Task<bool> Update(MenuDefinition model);
        Task<bool> SetActive(int id, int Active);
        Task<IPaginatedList<MenuDefinitionDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize, int menuId, int sizeId);
        Task<MenuDefinitionDto> GetById(int restaurantId, int branchId, int id);
        Task<int> CheckExist(int restaurantId, int branchId, int menuId, int sizeId, int ingredientId);
    }
}
