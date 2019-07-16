using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Interfaces.Responses;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Common.Models;
using Restaurant.Common.Responses.AdminAccount;
using Restaurant.Entities.ModelExtensions;
using Restaurant.Entities.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Restaurant.Business.Interfaces
{
    public interface IAdminGroupPermissionBusiness
    {
        Task<List<AdminGroupPermission_View00>> GetPermissionByGroupAndModule(int restaurantId, int branchId,int curGroupId, int groupId, string module);
        Task<int> InsertOrUpdatePermission(int restaurantId, int branchId, int groupId, int permissionId, int status);
        Task<string[]> GetPermissionByGroup(int restaurantId, int branchId, int groupId);
        Task<List<AdminGroup>> GetGroup(int restaurantId, int branchId,int groupId);
        Task<List<Option2Model>> GetModule();
        Task<AdminGroupPermission> Add(AdminGroupPermission model);
    }
}
