using Ecommerce.Entities.Models;

namespace Ecommerce.Entities.Interfaces
{
    public interface IOrderHeaderRepository : IGenericRepository<OrderHeader>
    {
        void UpdateOrderStatus(int id, string OrderStatus, string PaymentStatus);
    }
}
