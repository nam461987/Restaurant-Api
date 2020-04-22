using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Business.Filter;
using Restaurant.Business.Interfaces;
using Restaurant.Business.Interfaces.Paginated;
using Restaurant.Business.Paginated;
using Restaurant.Common.Dtos.Menu;
using Restaurant.Common.Enums;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using Restaurant.Repository.Interfaces.Menus;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant.Business
{
    public class CheckoutBusiness : ICheckoutBusiness
    {
        private readonly IMapper _mapper;
        public CheckoutBusiness(IMapper mapper)
        {
            _mapper = mapper;
        }
        public string AcceptPayment()
        {
            // Set your secret key: remember to change this to your live secret key in production
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.ApiKey = "sk_test_h6CTufXmUkbx5ExBnk8UU2XG004ISN3kju";
            var client_secret = string.Empty;
            try
            {
                var service = new PaymentIntentService();
                var options = new PaymentIntentCreateOptions
                {
                    Amount = 1099,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>
                      {
                        "card"
                      }
                };
                var request = service.Create(options);
                client_secret = request.ClientSecret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return client_secret;
        }
    }
}
