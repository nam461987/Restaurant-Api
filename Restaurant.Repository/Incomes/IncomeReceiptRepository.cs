using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces.Incomes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository.Incomes
{
    public class IncomeReceiptRepository : BaseRepository<IncomeReceipt>, IIncomeReceiptRepository
    {
        public IncomeReceiptRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
