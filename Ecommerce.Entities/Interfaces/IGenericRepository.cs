using Ecommerce.Entities.Models;
using System.Linq.Expressions;

namespace Ecommerce.Entities.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? criteria = null!, string[]? includes = null!);
        Task<T?> Find(Expression<Func<T, bool>> criteria, string[]? includes = null!);
        Task<IEnumerable<T?>> FindAllWithTrack(Expression<Func<T, bool>>? criteria = null, string[]? includes = null);
        Task<T?> FindWithTrack(Expression<Func<T, bool>> criteria, string[]? includes = null!);
        void Create(T model);
        void Update(T model);
        void Delete(T model);
        void RemoveRange(IEnumerable<T> entities);
    }
}
