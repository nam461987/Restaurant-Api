using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces.Incomes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Restaurant.Repository.Incomes
{
    public class IncomeReceiptDetailRepository : BaseRepository<IncomeReceiptDetail>, IIncomeReceiptDetailRepository
    {
        public IncomeReceiptDetailRepository(restaurantContext context)
            : base(context)
        {
        }
    }
}
