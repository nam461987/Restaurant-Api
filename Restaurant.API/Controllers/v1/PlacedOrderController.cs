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
using Restaurant.Common.Dtos.PlacedOrder;
using System;
using Restaurant.Common.Enums;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class PlacedOrderController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IPlacedOrderBusiness _placedOrderBusiness;
        private readonly IPlacedOrderProcessStatusBusiness _placedOrderProcessStatusBusiness;

        public PlacedOrderController(IHttpContextAccessor httpContextAccessor,
            IPlacedOrderBusiness placedOrderBusiness,
            IPlacedOrderProcessStatusBusiness placedOrderProcessStatusBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _placedOrderBusiness = placedOrderBusiness;
            _placedOrderProcessStatusBusiness = placedOrderProcessStatusBusiness;
        }
        // GET: /PlacedOrder
        [ClaimRequirement("", "placed_order_list")]
        [HttpGet]
        public async Task<IPaginatedList<PlacedOrderDto>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return await _placedOrderBusiness.GetAll(_authenticationDto.RestaurantId, _authenticationDto.BranchId, pageIndex, pageSize);
        }
        // GET: /PlacedOrder/5
        [ClaimRequirement("", "placed_order_update")]
        [HttpGet("{id}")]
        public async Task<PlacedOrderDto> Get(int id)
        {
            return await _placedOrderBusiness.GetById(_authenticationDto.RestaurantId, _authenticationDto.BranchId, id);
        }
        // POST: /PlacedOrder
        [ClaimRequirement("", "placed_order_create")]
        [HttpPost]
        public async Task<int> Post(PlacedOrder model)
        {
            var result = 0;

            //if current user is Restaurant Admin, don't let them create order
            // because when update price will be wrong
            if (_authenticationDto.TypeId == (int)EAccountType.Admin || _authenticationDto.TypeId == (int)EAccountType.Mod
                || _authenticationDto.TypeId == (int)EAccountType.RestaurantAdmin)
            {
                return result;
            }

            if (ModelState.IsValid)
            {
                model.RestaurantId = _authenticationDto.RestaurantId;
                model.BranchId = _authenticationDto.BranchId;
                model.OrderTime = DateTime.Now;
                model.CreatedDate = DateTime.Now;
                model.CreatedStaffId = _authenticationDto.UserId;
                model.Status = 1;
                var modelInsert = await _placedOrderBusiness.Add(model);
                result = modelInsert.Id;

                // add order process as a record
                // Waiting Order Status
                if (result > 0)
                {
                    var processStatus = new PlacedOrderProcessStatus()
                    {
                        RestaurantId = modelInsert.RestaurantId,
                        BranchId = modelInsert.BranchId,
                        PlacedOrderId = modelInsert.Id,
                        OrderProcessId = (int)EOrderProcess.WaitingOrder,
                        Status = 1,
                        CreatedStaffId = _authenticationDto.UserId,
                        CreatedDate = DateTime.Now
                    };
                    var lastProcessStatus = await _placedOrderProcessStatusBusiness.Add(processStatus);

                    // get the last Process status and update to the order
                    if (lastProcessStatus != null)
                    {
                        modelInsert.OrderProcessId = lastProcessStatus.OrderProcessId;
                        await _placedOrderBusiness.UpdateOrderProcess(modelInsert);
                    }
                }
            }
            return result;
        }
        // PUT: /PlacedOrder/5
        [ClaimRequirement("", "placed_order_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(PlacedOrder model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                model.UpdatedStaffId = _authenticationDto.UserId;
                result = await _placedOrderBusiness.Update(model);
            }
            return result;
        }

        // PUT: /PlacedOrder/active
        [ClaimRequirement("", "placed_order_delete")]
        [HttpPut("active")]
        public async Task<bool> Put(int id, int Status)
        {
            return await _placedOrderBusiness.SetActive(id, Status);
        }
        // GET: /PlacedOrder
        [ClaimRequirement("", "waiting_order_list")]
        [Route("getwaitingorder")]
        [HttpGet]
        public async Task<List<PlacedOrderDto>> GetWaitingOrder()
        {
            return await _placedOrderBusiness.GetWaitingOrder(_authenticationDto.RestaurantId, _authenticationDto.BranchId);
        }
        // GET: /PlacedOrder
        [ClaimRequirement("", "waiting_order_list")]
        [Route("setcompleteorder")]
        [HttpPut]
        public async Task<bool> SetCompleteOrder(PlacedOrder model)
        {
            var result = false;
            var process = 0;
            if (model.OrderTypeId == (int)EOrderType.DineIn)
            {
                process = (int)EOrderProcess.ServingOrder;
            }
            else if (model.OrderTypeId == (int)EOrderType.Delivery || model.OrderTypeId == (int)EOrderType.ToGo)
            {
                process = (int)EOrderProcess.AvailableOrder;
            }
            var processStatus = new PlacedOrderProcessStatus()
            {
                RestaurantId = _authenticationDto.RestaurantId,
                BranchId = _authenticationDto.BranchId,
                PlacedOrderId = model.Id,
                OrderProcessId = process,
                Status = 1,
                CreatedStaffId = _authenticationDto.UserId,
                CreatedDate = DateTime.Now
            };
            var lastProcessStatus = await _placedOrderProcessStatusBusiness.Add(processStatus);

            // get the last Process status and update to the order
            if (lastProcessStatus != null)
            {
                model.OrderProcessId = lastProcessStatus.OrderProcessId;
                await _placedOrderBusiness.UpdateOrderProcess(model);
                result = true;
            }
            return result;
        }
        // GET: /PlacedOrder
        [ClaimRequirement("", "placed_order_update")]
        [Route("setordermoreorder")]
        [HttpPut]
        public async Task<bool> SetOrderMoreOrder(int id)
        {
            var result = false;

            var checkProcessStatusExist = await _placedOrderProcessStatusBusiness.CheckProcessStatusExist(_authenticationDto.RestaurantId,
                    _authenticationDto.BranchId, id, (int)EOrderProcess.AddMoreOrder);

            if (!checkProcessStatusExist)
            {
                var processStatus = new PlacedOrderProcessStatus()
                {
                    RestaurantId = _authenticationDto.RestaurantId,
                    BranchId = _authenticationDto.BranchId,
                    PlacedOrderId = id,
                    OrderProcessId = (int)EOrderProcess.AddMoreOrder,
                    Status = 1,
                    CreatedStaffId = _authenticationDto.UserId,
                    CreatedDate = DateTime.Now
                };
                var lastProcessStatus = await _placedOrderProcessStatusBusiness.Add(processStatus);

                // get the last Process status and update to the order
                if (lastProcessStatus != null)
                {
                    var model = new PlacedOrder()
                    {
                        Id = id,
                        OrderProcessId = lastProcessStatus.OrderProcessId
                    };
                    await _placedOrderBusiness.UpdateOrderProcess(model);
                    result = true;
                }
            }
            return result;
        }
    }
}
