using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Attributes;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Business.Interfaces;
using Restaurant.API.Extensions;
using Restaurant.Business.Interfaces.Responses;
using System.Threading.Tasks;
using Restaurant.Entities.Models;
using System.Collections.Generic;
using Restaurant.Common.Models;
using Restaurant.Entities.ModelExtensions;

namespace Restaurant.API.Controllers.v1
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class GroupPermissionController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IAdminGroupPermissionBusiness _groupPermissionBusiness;
        public GroupPermissionController(IHttpContextAccessor httpContextAccessor,
            IAdminGroupPermissionBusiness groupPermissionBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _groupPermissionBusiness = groupPermissionBusiness;
        }

        // GET: /grouppermission/getpermission
        [ClaimRequirement("", "admin_group_permission_list")]
        [Route("getpermission")]
        [HttpGet]
        public async Task<List<AdminGroupPermission_View00>> Get(int groupId, string module)
        {
            //Because we just get user permissions by restaurant,
            //we set BranchId = 0.
            //If we want to get by each Branch, just put current user branchId
            return await _groupPermissionBusiness.GetPermissionByGroupAndModule(_authenticationDto.RestaurantId,
                0, _authenticationDto.TypeId, groupId, module);
        }
        // GET: /grouppermission/getgroup
        [Route("getgroup")]
        [HttpGet]
        public async Task<List<AdminGroup>> GetGroup()
        {
            return await _groupPermissionBusiness.GetGroup(_authenticationDto.RestaurantId, _authenticationDto.BranchId,_authenticationDto.TypeId);
        }
        [Route("getmodule")]
        [HttpGet]
        public async Task<List<Option2Model>> GetModule()
        {
            return await _groupPermissionBusiness.GetModule();
        }
        // PUT: /grouppermission
        [ClaimRequirement("", "admin_group_permission_create,admin_group_permission_update")]
        [HttpPut]
        public async Task<int> Put(int groupId, int permissionId, int status)
        {
            return await _groupPermissionBusiness.InsertOrUpdatePermission(_authenticationDto.RestaurantId,
                _authenticationDto.BranchId, groupId, permissionId, status);
        }
    }
}