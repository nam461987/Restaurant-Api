using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.MenuPrice;
using Restaurant.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IMenuPriceBusiness
    {
        Task<MenuPrice> Add(MenuPrice model);
        Task<bool> Update(MenuPrice model);
        Task<bool> SetActive(int id, int Active);
        Task<IPaginatedList<MenuPriceDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize);
        Task<MenuPriceDto> GetById(int restaurantId, int branchId, int id);
        Task<IPaginatedList<MenuPriceDto>> GetByMenuId(int restaurantId, int branchId, int pageIndex, int pageSize, int menuId);
        Task<MenuPrice> CheckExistPrice(MenuPrice model); 
        Task<List<MenuPriceDto>> GetAllNotPaginate(int restaurantId, int branchId);
    }
}
