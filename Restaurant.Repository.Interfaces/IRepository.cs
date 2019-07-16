using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Repository.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Repo { get; }
        Task<IList<TEntity>> GetAllAsync();
        Task<IList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression);
        Task<TEntity> GetAsync(int id);
        TEntity Add(TEntity entity);
        void AddRange(List<TEntity> entities);
        void Delete(TEntity entity);
        void DeleteRange(List<TEntity> entities);
        void Delete(int id);
        void Update(TEntity entity);
        Task<int> SaveChangeAsync();
    }
}
