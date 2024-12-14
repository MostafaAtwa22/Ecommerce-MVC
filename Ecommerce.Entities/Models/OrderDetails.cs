using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Entities.Models
{
    public class OrderDetails : BaseEntity
    {
        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public OrderHeader OrderHeader { get; set; } = default!;

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = default!;

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
