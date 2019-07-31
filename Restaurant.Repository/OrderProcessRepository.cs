﻿using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository
{
    public class OrderProcessRepository : BaseRepository<OrderProcess>, IOrderProcessRepository
    {
        public OrderProcessRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}