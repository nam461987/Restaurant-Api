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
using Restaurant.Common.Dtos.PlacedOrderDetail;
using Restaurant.Common.Enums;
using AutoMapper;
using System.Collections.Generic;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class PlacedOrderDetailController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IPlacedOrderDetailBusiness _placedOrderDetailBusiness;
        private readonly IPlacedOrderBusiness _placedOrderBusiness;
        private readonly IMapper _mapper;
        private readonly IPlacedOrderProcessStatusBusiness _placedOrderProcessStatusBusiness;

        public PlacedOrderDetailController(IHttpContextAccessor httpContextAccessor,
            IPlacedOrderDetailBusiness placedOrderDetailBusiness,
            IPlacedOrderBusiness placedOrderBusiness,
            IMapper mapper,
            IPlacedOrderProcessStatusBusiness placedOrderProcessStatusBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _placedOrderDetailBusiness = placedOrderDetailBusiness;
            _placedOrderBusiness = placedOrderBusiness;
            _mapper = mapper;
            _placedOrderProcessStatusBusiness = placedOrderProcessStatusBusiness;
        }
        // GET: /PlacedOrderDetail
        [ClaimRequirement("", "placed_order_detail_list")]
        [HttpGet]
        public async Task<IPaginatedList<PlacedOrderDetailDto>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return await _placedOrderDetailBusiness.GetAll(_authenticationDto.RestaurantId, _authenticationDto.BranchId, pageIndex, pageSize);
        }
        // GET: /PlacedOrderDetail/5
        [ClaimRequirement("", "placed_order_detail_update")]
        [HttpGet("{id}")]
        public async Task<PlacedOrderDetailDto> Get(int id)
        {
            return await _placedOrderDetailBusiness.GetById(_authenticationDto.RestaurantId, _authenticationDto.BranchId, id);
        }
        // POST: /PlacedOrderDetail
        [ClaimRequirement("", "placed_order_detail_create")]
        [HttpPost]
        public async Task<int> Post(PlacedOrderDetail model)
        {
            var result = 0;

            //if current user is Restaurant Admin, don't let them add order detail
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
                model.Status = 1;
                model.IsFinish = 0;
                model.Price = model.Quantity * model.MenuPrice;

                var modelInsert = await _placedOrderDetailBusiness.Add(model);
                result = modelInsert.Id;

                if (result > 0)
                {
                    //get total price from order details
                    var totalDetailPrice = await _placedOrderDetailBusiness.GetTotalDetailPriceByOrderId
                        (model.RestaurantId, model.BranchId, model.PlacedOrderId);

                    // get and update order price by newest order details price
                    var order = await _placedOrderBusiness.GetById(model.RestaurantId, model.BranchId, model.PlacedOrderId);
                    if (order != null)
                    {
                        order.Price = totalDetailPrice;
                        await _placedOrderBusiness.UpdatePriceToPlacedOrder(_mapper.Map<PlacedOrder>(order));
                    }
                }
            }
            return result;
        }
        // PUT: /PlacedOrderDetail/5
        [ClaimRequirement("", "placed_order_detail_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(PlacedOrderDetail model)
        {
            var result = false;

            //if current user is Restaurant Admin, don't let them update order detail
            // because when update price will be wrong
            if (_authenticationDto.TypeId == (int)EAccountType.Admin || _authenticationDto.TypeId == (int)EAccountType.Mod
                || _authenticationDto.TypeId == (int)EAccountType.RestaurantAdmin)
            {
                return result;
            }
            if (ModelState.IsValid)
            {
                result = await _placedOrderDetailBusiness.Update(model);

                if (result)
                {
                    //get total price from order details
                    var totalDetailPrice = await _placedOrderDetailBusiness.GetTotalDetailPriceByOrderId
                    (_authenticationDto.RestaurantId, _authenticationDto.BranchId, model.PlacedOrderId);

                    // get and update order price by newest order details price
                    var order = await _placedOrderBusiness.GetById(_authenticationDto.RestaurantId, _authenticationDto.BranchId, model.PlacedOrderId);
                    if (order != null)
                    {
                        order.Price = totalDetailPrice;
                    }
                }
            }
            return result;
        }

        // PUT: /PlacedOrderDetail/active
        [ClaimRequirement("", "placed_order_detail_delete")]
        [HttpPut("active")]
        public async Task<bool> Put(int id, int Status)
        {
            var result = false;

            //if current user is Restaurant Admin, don't let them delete order detail
            // because when update price will be wrong
            if (_authenticationDto.TypeId == (int)EAccountType.Admin || _authenticationDto.TypeId == (int)EAccountType.Mod
                || _authenticationDto.TypeId == (int)EAccountType.RestaurantAdmin)
            {
                return result;
            }

            result = await _placedOrderDetailBusiness.SetActive(id, Status);

            if (result)
            {
                //get order detail need to delete
                var orderDetail = await _placedOrderDetailBusiness.GetById(_authenticationDto.RestaurantId, _authenticationDto.BranchId, id);

                //get total price from order details
                var totalDetailPrice = await _placedOrderDetailBusiness.GetTotalDetailPriceByOrderId
                    (orderDetail.RestaurantId, orderDetail.BranchId, orderDetail.PlacedOrderId);

                // get and update order price by newest order details price
                var order = await _placedOrderBusiness.GetById(orderDetail.RestaurantId, orderDetail.BranchId, orderDetail.PlacedOrderId);
                if (order != null)
                {
                    order.Price = totalDetailPrice;
                }
            }

            return result;
        }
        // GET: /PlacedOrder
        [ClaimRequirement("", "waiting_order_list")]
        [Route("getwaitingorderdetail")]
        [HttpGet]
        public async Task<List<PlacedOrderDetailDto>> GetWaitingOrderDetail()
        {
            return await _placedOrderDetailBusiness.GetWaitingOrderDetail(_authenticationDto.RestaurantId, _authenticationDto.BranchId);
        }
        // PUT: /PlacedOrderDetail/5
        [ClaimRequirement("", "placed_order_detail_update")]
        [Route("setfinishorderdetail")]
        public async Task<PlacedOrderDetail> SetFinishOrderDetail(int id, int isFinish)
        {
            //if current user is Restaurant Admin, don't let them update order detail
            // because when update price will be wrong
            if (_authenticationDto.TypeId == (int)EAccountType.Admin || _authenticationDto.TypeId == (int)EAccountType.Mod
                || _authenticationDto.TypeId == (int)EAccountType.RestaurantAdmin)
            {
                return null;
            }

            var result = await _placedOrderDetailBusiness.SetFinishOrderDetail(_authenticationDto.RestaurantId, _authenticationDto.BranchId,
                id, isFinish);

            if (result != null)
            {
                var checkProcessStatusExist = await _placedOrderProcessStatusBusiness.CheckProcessStatusExist(_authenticationDto.RestaurantId,
                    _authenticationDto.BranchId, result.PlacedOrderId, (int)EOrderProcess.PreparingOrder);
                if (!checkProcessStatusExist)
                {
                    var processStatus = new PlacedOrderProcessStatus()
                    {
                        RestaurantId = result.RestaurantId,
                        BranchId = result.BranchId,
                        PlacedOrderId = result.PlacedOrderId,
                        OrderProcessId = (int)EOrderProcess.PreparingOrder,
                        Status = 1,
                        CreatedStaffId = _authenticationDto.UserId,
                        CreatedDate = DateTime.Now
                    };
                    var lastProcessStatus = await _placedOrderProcessStatusBusiness.Add(processStatus);


                    // get the last Process status and update to the order
                    if (lastProcessStatus != null)
                    {
                        // set the order process to preparing
                        var order = await _placedOrderBusiness.GetById(result.RestaurantId, result.BranchId, result.PlacedOrderId);
                        if (order != null)
                        {
                            order.OrderProcessId = lastProcessStatus.OrderProcessId;
                            await _placedOrderBusiness.UpdateOrderProcess(_mapper.Map<PlacedOrder>(order));
                        }
                    }
                }
            }

            return result;
        }
    }
}
