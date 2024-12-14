
namespace Ecommerce.DataAccess.Repositories
{
    public class OrderDetailsRepository : GenericRepository<OrderDetails>, IOrderDetailsRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailsRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }
    }
}
