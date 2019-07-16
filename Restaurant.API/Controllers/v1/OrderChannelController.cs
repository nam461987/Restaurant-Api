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
using Restaurant.Common.Dtos.OrderChannel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class OrderChannelController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IOrderChannelBusiness _orderChannelBusiness;

        public OrderChannelController(IHttpContextAccessor httpContextAccessor,
            IOrderChannelBusiness orderChannelBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _orderChannelBusiness = orderChannelBusiness;
        }
        // GET: /OrderChannel
        [ClaimRequirement("", "category_order_channel_list")]
        [HttpGet]
        public Task<IPaginatedList<OrderChannelDto>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return _orderChannelBusiness.GetAll(_authenticationDto.RestaurantId, _authenticationDto.BranchId, pageIndex, pageSize);
        }
        // GET: /OrderChannel/5
        [ClaimRequirement("", "category_order_channel_update")]
        [HttpGet("{id}")]
        public Task<OrderChannelDto> Get(int id)
        {
            return _orderChannelBusiness.GetById(_authenticationDto.RestaurantId, _authenticationDto.BranchId, id);
        }
        // POST: /OrderChannel
        [ClaimRequirement("", "category_order_channel_create")]
        [HttpPost]
        public async Task<int> Post(OrderChannel model)
        {

            var result = 0;
            if (ModelState.IsValid)
            {
                model.Status = 1;
                var modelInsert = await _orderChannelBusiness.Add(model);
                result = modelInsert.Id;
            }
            return result;
        }
        // PUT: /OrderChannel/5
        [ClaimRequirement("", "category_order_channel_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(OrderChannel model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = await _orderChannelBusiness.Update(model);
            }
            return result;
        }

        // PUT: /OrderChannel/active
        [ClaimRequirement("", "category_order_channel_delete")]
        [HttpPut("active")]
        public Task<bool> Put(int id, int Status)
        {
            return _orderChannelBusiness.SetActive(id, Status);
        }
    }
}
