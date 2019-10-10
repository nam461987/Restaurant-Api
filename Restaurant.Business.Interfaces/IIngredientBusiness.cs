using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.Ingredient;
using Restaurant.Entities.Models;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IIngredientBusiness
    {
        Task<Ingredient> Add(Ingredient model);
        Task<bool> Update(Ingredient model);
        Task<bool> SetActive(int id, int Active);
        Task<IPaginatedList<IngredientDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize);
        Task<IngredientDto> GetById(int restaurantId, int branchId, int id);
    }
}
