using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository
{
    public class CheckoutRepository : BaseRepository<Checkout>, ICheckoutRepository
    {
        public CheckoutRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
