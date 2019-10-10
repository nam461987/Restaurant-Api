using System;
using System.Threading.Tasks;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;

namespace Restaurant.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly restaurantContext _context;

        public UnitOfWork(restaurantContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
