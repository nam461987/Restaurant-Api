using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository
{
    public class OrderChannelTimeRepository : BaseRepository<OrderChannelTime>, IOrderChannelTimeRepository
    {
        public OrderChannelTimeRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
