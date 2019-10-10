using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.MenuCategory;
using Restaurant.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IMenuCategoryBusiness
    {
        Task<MenuCategory> Add(MenuCategory model);
        Task<bool> Update(MenuCategory model);
        Task<bool> SetActive(int id, int Active);
        Task<IPaginatedList<MenuCategoryDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize);
        Task<List<MenuCategory>> GetAllNotPaginate(int restaurantId, int branchId);
        Task<MenuCategoryDto> GetById(int restaurantId, int branchId, int id);
    }
}
