using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Attributes;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Business.Interfaces;
using Restaurant.API.Extensions;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Entities.Models;
using Restaurant.Common.Constants;

namespace Restaurant.API.Controllers.v1
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class AdminGroupController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IAdminGroupBusiness _adminGroupBusiness;
        public AdminGroupController(IHttpContextAccessor httpContextAccessor,
            IAdminGroupBusiness adminGroupBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _adminGroupBusiness = adminGroupBusiness;
        }
        // GET: /admingroup
        [ClaimRequirement("", "admin_group_list")]
        [HttpGet]
        public Task<IPaginatedList<AdminGroup>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return _adminGroupBusiness.GetAll(pageIndex, pageSize);
        }
        // GET: /admingroup/5
        [ClaimRequirement("", "admin_group_update")]
        [HttpGet("{id}")]
        public Task<AdminGroup> Get(int id)
        {
            return _adminGroupBusiness.GetById(id);
        }
        // POST: /admingroup
        [ClaimRequirement("", "admin_group_create")]
        [HttpPost]
        public async Task<int> Post(AdminGroup model)
        {
            var result = 0;
            if (ModelState.IsValid)
            {
                model.Status = 1;
                var modelInsert = await _adminGroupBusiness.Add(model);
                result = modelInsert.Id;
            }
            return result;
        }
        // PUT: /admingroup/5
        [ClaimRequirement("", "admin_group_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(AdminGroup model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = await _adminGroupBusiness.Update(model);
            }
            return result;
        }
        // PUT: /admingroup/active
        [HttpPut("active")]
        [ClaimRequirement("", "admin_group_delete")]
        public Task<bool> Put(int id, int Status)
        {
            return _adminGroupBusiness.SetActive(id, Status);
        }
    }
}