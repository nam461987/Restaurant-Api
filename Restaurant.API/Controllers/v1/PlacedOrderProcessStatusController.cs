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
using Restaurant.Common.Dtos.PlacedOrderProcessStatus;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class PlacedOrderProcessStatusController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IPlacedOrderProcessStatusBusiness _placedOrderProcessStatusBusiness;

        public PlacedOrderProcessStatusController(IHttpContextAccessor httpContextAccessor,
            IPlacedOrderProcessStatusBusiness placedOrderProcessStatusBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _placedOrderProcessStatusBusiness = placedOrderProcessStatusBusiness;
        }
        // GET: /PlacedOrderProcessStatus
        //[HttpGet]
        //public async Task<IPaginatedList<PlacedOrderProcessStatusDto>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        //{
        //    return await _placedOrderProcessStatusBusiness.GetAll(_authenticationDto.RestaurantId, _authenticationDto.BranchId, pageIndex, pageSize);
        //}
        // GET: /PlacedOrderProcessStatus/5
        //[HttpGet("{id}")]
        //public async Task<PlacedOrderProcessStatusDto> Get(int id)
        //{
        //    return await _placedOrderProcessStatusBusiness.GetById(_authenticationDto.RestaurantId, _authenticationDto.BranchId, id);
        //}
        // POST: /PlacedOrderProcessStatus
        //[HttpPost]
        //public async Task<int> Post(PlacedOrderProcessStatus model)
        //{

        //    var result = 0;
        //    if (ModelState.IsValid)
        //    {
        //        model.Status = 1;
        //        var modelInsert = await _placedOrderProcessStatusBusiness.Add(model);
        //        result = modelInsert.Id;
        //    }
        //    return result;
        //}
        // PUT: /PlacedOrderProcessStatus/5
        //[HttpPut("{id}")]
        //public async Task<bool> Put(PlacedOrderProcessStatus model)
        //{
        //    var result = false;
        //    if (ModelState.IsValid)
        //    {
        //        result = await _placedOrderProcessStatusBusiness.Update(model);
        //    }
        //    return result;
        //}

        // PUT: /PlacedOrderProcessStatus/active
        //[HttpPut("active")]
        //public async Task<bool> Put(int id, int Status)
        //{
        //    return await _placedOrderProcessStatusBusiness.SetActive(id, Status);
        //}
        [HttpGet]
        [Route("getbyorderid")]
        public async Task<List<PlacedOrderProcessStatusDto>> GetByOrderId(int orderId)
        {
            return await _placedOrderProcessStatusBusiness.GetByOrderId(_authenticationDto.RestaurantId, _authenticationDto.BranchId, orderId);
        }
    }
}
