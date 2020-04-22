using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces.Incomes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository.Incomes
{
    public class IncomePaymentRepository : BaseRepository<IncomePayment>, IIncomePaymentRepository
    {
        public IncomePaymentRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
