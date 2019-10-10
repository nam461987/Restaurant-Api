using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Entities.Models;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IAdminPermissionBusiness
    {
        Task<AdminPermission> Add(AdminPermission model);
        Task<bool> Update(AdminPermission model);
        Task<bool> SetActive(int id, int Active);
        Task<IPaginatedList<AdminPermission>> GetAll(int pageIndex, int pageSize);
        Task<AdminPermission> GetById(int id);
    }
}
