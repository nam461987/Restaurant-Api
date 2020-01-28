using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Restaurant.API.Attributes;
using Restaurant.Common.Dtos.AdminAccount;
using Restaurant.Business.Interfaces;
using Microsoft.AspNetCore.Http;
using Restaurant.API.Extensions;
using Restaurant.Entities.Models;
using Restaurant.Common.Constants;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Common.Dtos.Tax;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class TaxController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly ITaxBusiness _taxBusiness;

        public TaxController(IHttpContextAccessor httpContextAccessor,
            ITaxBusiness taxBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _taxBusiness = taxBusiness;
        }
        // GET: /restaurant
        [ClaimRequirement("", "category_tax_list")]
        [HttpGet]
        public Task<IPaginatedList<TaxDto>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return _taxBusiness.GetAll(_authenticationDto.RestaurantId, _authenticationDto.BranchId, pageIndex, pageSize);
        }
        // GET: /restaurant/5
        [ClaimRequirement("", "category_tax_update")]
        [HttpGet("{id}")]
        public Task<TaxDto> Get(int id)
        {
            return _taxBusiness.GetById(_authenticationDto.RestaurantId, _authenticationDto.BranchId, id);
        }
        // POST: /restaurant
        [ClaimRequirement("", "category_tax_create")]
        [HttpPost]
        public async Task<int> Post(Tax model)
        {

            var result = 0;
            if (ModelState.IsValid)
            {
                model.Status = 1;
                var modelInsert = await _taxBusiness.Add(model);
                result = modelInsert.Id;
            }
            return result;
        }
        // PUT: /restaurant/5
        [ClaimRequirement("", "category_tax_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(Tax model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = await _taxBusiness.Update(model);
            }
            return result;
        }

        // PUT: /restaurant/active
        [ClaimRequirement("", "category_tax_delete")]
        [HttpPut("active")]
        public Task<bool> Put(int id, int Status)
        {
            return _taxBusiness.SetActive(id, Status);
        }
    }
}
