using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces.Incomes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository.Incomes
{
    public class IncomePaymentReasonRepository : BaseRepository<IncomePaymentReason>, IIncomePaymentReasonRepository
    {
        public IncomePaymentReasonRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
