using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.DataAccess.Repositories
{
    public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderHeaderRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public void UpdateOrderStatus(int id, string OrderStatus, string? PaymentStatus)
        {
            var order = _context.OrderHeaders
                .FirstOrDefault(o => o.Id == id);

            if (order is not null)
            {
                order.OrderStatus = OrderStatus;
                order.PaymentDate = DateTime.Now;
                if (PaymentStatus is not null)
                    order.PaymentStatus = PaymentStatus;
            }
        }
	}
}
