using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository
{
    public class OrderChannelRepository : BaseRepository<OrderChannel>, IOrderChannelRepository
    {
        public OrderChannelRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
