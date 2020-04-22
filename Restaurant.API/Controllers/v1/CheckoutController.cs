using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Restaurant.Business.Interfaces;
using Restaurant.Entities.Models;

namespace Restaurant.API.Controllers.v1
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutBusiness _checkoutBusiness;

        public CheckoutController(ICheckoutBusiness checkoutBusiness)
        {
            _checkoutBusiness = checkoutBusiness;
        }
        [HttpGet]
        public string Get()
        {
            var client_secret = _checkoutBusiness.AcceptPayment();

            return client_secret;
        }
    }
}