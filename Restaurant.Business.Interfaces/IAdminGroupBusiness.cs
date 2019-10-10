using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Entities.Models;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IAdminGroupBusiness
    {
        Task<AdminGroup> Add(AdminGroup model);
        Task<bool> Update(AdminGroup model);
        Task<bool> SetActive(int id, int Active);
        Task<IPaginatedList<AdminGroup>> GetAll(int pageIndex, int pageSize);
        Task<AdminGroup> GetById(int id);
    }
}
