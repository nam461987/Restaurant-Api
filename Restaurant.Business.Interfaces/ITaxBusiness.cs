using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.Tax;
using Restaurant.Entities.Models;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface ITaxBusiness
    {
        Task<Tax> Add(Tax model);
        Task<bool> Update(Tax model);
        Task<bool> SetActive(int id, int Active);
        Task<IPaginatedList<TaxDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize);
        Task<TaxDto> GetById(int restaurantId, int branchId, int id);
    }
}
