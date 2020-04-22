﻿using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces.Incomes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository.Incomes
{
    public class IncomePaymentEventRepository : BaseRepository<IncomePaymentEvent>, IIncomePaymentEventRepository
    {
        public IncomePaymentEventRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
