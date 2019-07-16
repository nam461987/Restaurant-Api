using Microsoft.EntityFrameworkCore;
using Restaurant.Entities.Models;
using Restaurant.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Repository
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly restaurantContext _context;

        public virtual IQueryable<TEntity> Repo => _context.Set<TEntity>();

        public BaseRepository(restaurantContext context)
        {
            _context = context;
        }

        public virtual TEntity Add(TEntity entity)
        {
            return _context.Set<TEntity>().Add(entity).Entity;
        }

        public virtual void AddRange(List<TEntity> entities)
        {
            _context.Set<TEntity>().AddRange(entities);
        }

        public virtual void Update(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual async Task<IList<TEntity>> GetAllAsync()
        {
            var models = await _context.Set<TEntity>().ToListAsync();
            return models;
        }

        public virtual async Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression)
        {
            var models = await _context.Set<TEntity>().Where(expression).ToListAsync();
            return models;
        }

        public virtual async Task<TEntity> GetAsync(int id)
        {
            var model = await _context.Set<TEntity>().FindAsync(id);
            return model;
        }

        public virtual async Task<int> SaveChangeAsync()
        {
            int result = 0;
            try
            {
                result = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                //TODO: write log here
            }
            return result;
        }

        public virtual void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public virtual void DeleteRange(List<TEntity> entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }

    }
}
