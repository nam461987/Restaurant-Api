using Restaurant.Entities.ModelExtensions;
using Restaurant.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Repository.Interfaces
{
    public interface IAdminGroupPermissionRepository : IRepository<AdminGroupPermission>
    {
        Task<List<AdminGroupPermission_View00>> GetPermissionByGroupAndModule(int restaurantId, int branchId, int curGroupId,
            int groupId, string module);
        Task<int> InsertOrUpdatePermission(int restaurantId, int branchId, int groupId, int permissionId, int status);
        Task<string[]> GetPermissionByGroup(int restaurantId, int branchId, int groupId);
    }
}
