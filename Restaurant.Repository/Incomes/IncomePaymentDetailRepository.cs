using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces.Incomes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository.Incomes
{
    public class IncomePaymentDetailRepository : BaseRepository<IncomePaymentDetail>, IIncomePaymentDetailRepository
    {
        public IncomePaymentDetailRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
