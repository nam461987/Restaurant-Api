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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Restaurant.API.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [BearerAuthorize]
    [ApiController]
    public class OrderProcessController : ControllerBase
    {
        private readonly AuthenticationDto _authenticationDto;
        private readonly IOrderProcessBusiness _orderProcessBusiness;

        public OrderProcessController(IHttpContextAccessor httpContextAccessor,
            IOrderProcessBusiness orderProcessBusiness)
        {
            _authenticationDto = httpContextAccessor.HttpContext.User.ToAuthenticationDto();
            _orderProcessBusiness = orderProcessBusiness;
        }
        // GET: /OrderProcess
        [ClaimRequirement("", "order_process_list")]
        [HttpGet]
        public async Task<IPaginatedList<OrderProcess>> Get(int pageIndex = Constant.PAGE_INDEX_DEFAULT, int pageSize = Constant.PAGE_SIZE_DEFAULT)
        {
            return await _orderProcessBusiness.GetAll(pageIndex, pageSize);
        }
        // GET: /OrderProcess/5
        [ClaimRequirement("", "order_process_update")]
        [HttpGet("{id}")]
        public async Task<OrderProcess> Get(int id)
        {
            return await _orderProcessBusiness.GetById(id);
        }
        // POST: /OrderProcess
        [ClaimRequirement("", "order_process_create")]
        [HttpPost]
        public async Task<int> Post(OrderProcess model)
        {

            var result = 0;
            if (ModelState.IsValid)
            {
                model.Status = 1;
                var modelInsert = await _orderProcessBusiness.Add(model);
                result = modelInsert.Id;
            }
            return result;
        }
        // PUT: /OrderProcess/5
        [ClaimRequirement("", "order_process_update")]
        [HttpPut("{id}")]
        public async Task<bool> Put(OrderProcess model)
        {
            var result = false;
            if (ModelState.IsValid)
            {
                result = await _orderProcessBusiness.Update(model);
            }
            return result;
        }

        // PUT: /OrderProcess/active
        [ClaimRequirement("", "order_process_delete")]
        [HttpPut("active")]
        public async Task<bool> Put(int id, int Status)
        {
            return await _orderProcessBusiness.SetActive(id, Status);
        }
    }
}
