using System.Threading.Tasks;

namespace Restaurant.Repository.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}
