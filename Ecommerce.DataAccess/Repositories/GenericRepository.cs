using System.Linq.Expressions;

namespace Ecommerce.DataAccess.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>>? criteria = null, string[]? includes = null)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (includes is not null)
                foreach (var include in includes)
                    query = query.Include(include);

            if (criteria is not null)
                query = query.Where(criteria);

            return await query.ToListAsync();
        }

        public async Task<T?> Find(Expression<Func<T, bool>> criteria, string[]? includes = null!)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (includes is not null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.SingleOrDefaultAsync(criteria);

        }

        public async Task<IEnumerable<T?>> FindAllWithTrack(Expression<Func<T, bool>> criteria
            , string[]? includes = null!)
        {
            IQueryable<T> query =_dbSet;

            if (includes is not null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.Where(criteria).ToListAsync();
        }

        public async Task<T?> FindWithTrack(Expression<Func<T, bool>> criteria, string[]? includes = null!)
        {
            IQueryable<T> query = _dbSet;

            if (includes is not null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.SingleOrDefaultAsync(criteria);
        }

        public void Create(T model)
            => _dbSet.Add(model);

        public void Update(T model)
            => _dbSet.Update(model);

        public void Delete(T model)
            => _dbSet.Remove(model);

        public void RemoveRange(IEnumerable<T> entities)
            => _dbSet.RemoveRange(entities);
    }
}
