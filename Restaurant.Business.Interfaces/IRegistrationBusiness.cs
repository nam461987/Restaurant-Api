using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Entities.Models;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IRegistrationBusiness
    {
        Task<bool> Add(Registration model);
        //Task<bool> Update(Registration model);
        //Task<bool> SetActive(int id, int Active);
        //Task<IPaginatedList<RegistrationDto>> GetAll(int restaurantId, int branchId, int pageIndex, int pageSize);
        //Task<RegistrationDto> GetById(int restaurantId, int branchId, int id);
    }
}
