using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces.Incomes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository.Incomes
{
    public class IncomeReceiptReasonRepository : BaseRepository<IncomeReceiptReason>, IIncomeReceiptReasonRepository
    {
        public IncomeReceiptReasonRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
