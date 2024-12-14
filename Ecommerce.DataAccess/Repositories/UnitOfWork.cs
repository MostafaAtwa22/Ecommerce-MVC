using Ecommerce.DataAccess.Data;
using Ecommerce.Entities.Interfaces;

namespace Ecommerce.DataAccess.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICategoryRepository Categories { get; private set; }
        public IProductRepository Products { get; private set; }
        public IShoppingCartRepository ShoppingCarts { get; private set; }

        public IOrderHeaderRepository OrderHeaders { get; private set; }

        public IOrderDetailsRepository OrderDetails { get; private set; }

        public IApplicationUserRepository ApplicationUsers { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Categories = new CategoryRepository(_context);
            Products = new ProductRepository(_context);
            ShoppingCarts = new ShoppingCartRepository(_context);
            OrderHeaders = new OrderHeaderRepository(_context);
            OrderDetails = new OrderDetailsRepository(_context);
            ApplicationUsers = new ApplicationUserRepository(_context);
        }

        public async Task<int> Complete()
            =>  await _context.SaveChangesAsync();
        
        public void Dispose()
            => _context.Dispose();
    }
}
