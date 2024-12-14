using Ecommerce.Entities.Models;

namespace Ecommerce.Entities.ViewModels.AdminOrders
{
    public class OrderVM
    {
        public OrderHeader OrderHeader { get; set; } = default!;

        public IEnumerable<OrderDetails> OrderDetails { get; set; } = default!;
    }
}
