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
using Restaurant.Common.Dtos.OrderChannelTime;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class OrderChannelTimeController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IOrderChannelTimeBusiness _orderChannelTimeBusiness;

        public OrderChannelTimeController(IHttpContextAccessor httpContextAccessor,
            IOrderChannelTimeBusiness orderChannelTimeBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _orderChannelTimeBusiness = orderChannelTimeBusiness;
        }
        // GET: /OrderChannelTime
        [ClaimRequirement("", "category_order_channel_list")]
        [HttpGet]
        public Task<IPaginatedList<OrderChannelTimeDto>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return _orderChannelTimeBusiness.GetAll(_authenticationDto.RestaurantId, _authenticationDto.BranchId, pageIndex, pageSize);
        }
        // GET: /OrderChannelTime/5
        [ClaimRequirement("", "category_order_channel_update")]
        [HttpGet("{id}")]
        public Task<OrderChannelTimeDto> Get(int id)
        {
            return _orderChannelTimeBusiness.GetById(_authenticationDto.RestaurantId, _authenticationDto.BranchId, id);
        }
        // POST: /OrderChannelTime
        [ClaimRequirement("", "category_order_channel_create")]
        [HttpPost]
        public async Task<int> Post(OrderChannelTime model)
        {

            var result = 0;
            if (ModelState.IsValid)
            {
                model.Status = 1;
                var modelInsert = await _orderChannelTimeBusiness.Add(model);
                result = modelInsert.Id;
            }
            return result;
        }
        // PUT: /OrderChannelTime/5
        [ClaimRequirement("", "category_order_channel_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(OrderChannelTime model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = await _orderChannelTimeBusiness.Update(model);
            }
            return result;
        }

        // PUT: /OrderChannelTime/active
        [ClaimRequirement("", "category_order_channel_delete")]
        [HttpPut("active")]
        public Task<bool> Put(int id, int Status)
        {
            return _orderChannelTimeBusiness.SetActive(id, Status);
        }
    }
}
