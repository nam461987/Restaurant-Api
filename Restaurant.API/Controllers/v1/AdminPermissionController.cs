using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.API.Attributes;
using Restaurant.Business.Interfaces;
using Restaurant.Entities.Models;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Constants;
using Restaurant.API.Extensions;
using Restaurant.Common.Enums;

namespace Restaurant.API.Controllers.v1
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class AdminPermissionController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IAdminPermissionBusiness _adminPermissionBusiness;
        private readonly IAdminGroupPermissionBusiness _adminGroupPermissionBusiness;
        public AdminPermissionController(IHttpContextAccessor httpContextAccessor,
            IAdminPermissionBusiness adminPermissionBusiness,
            IAdminGroupPermissionBusiness adminGroupPermissionBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _adminPermissionBusiness = adminPermissionBusiness;
            _adminGroupPermissionBusiness = adminGroupPermissionBusiness;
        }
        // GET: /adminpermission
        [ClaimRequirement("", "admin_permission_list")]
        [HttpGet]
        public Task<IPaginatedList<AdminPermission>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return _adminPermissionBusiness.GetAll(pageIndex, pageSize);
        }
        // GET: /adminpermission/5
        [ClaimRequirement("", "admin_permission_update")]
        [HttpGet("{id}")]
        public Task<AdminPermission> Get(int id)
        {
            return _adminPermissionBusiness.GetById(id);
        }
        // POST: /adminpermission
        [ClaimRequirement("", "admin_permission_create")]
        [HttpPost]
        public async Task<int> Post(AdminPermission model)
        {
            var result = 0;
            if (ModelState.IsValid)
            {
                model.Status = 1;
                model.CreatedDate = DateTime.Now;
                var modelInsert = await _adminPermissionBusiness.Add(model);
                result = modelInsert.Id;
            }
            // If add permission success, create group permission for Admin
            if (result > 0)
            {
                AdminGroupPermission newGroupPer = new AdminGroupPermission();
                newGroupPer.GroupId = (int)EAccountType.Admin;
                newGroupPer.PermissionId = result;
                newGroupPer.Status = 1;
                newGroupPer.CreatedDate = DateTime.Now;
                await _adminGroupPermissionBusiness.Add(newGroupPer);
            }
            return result;
        }
        // PUT: /adminpermission/5
        [ClaimRequirement("", "admin_permission_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(AdminPermission model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = await _adminPermissionBusiness.Update(model);
            }
            return result;
        }
        // PUT: /adminpermission/active
        [ClaimRequirement("", "admin_permission_delete")]
        [HttpPut("active")]
        public Task<bool> Put(int id, int Status)
        {
            return _adminPermissionBusiness.SetActive(id, Status);
        }
    }
}