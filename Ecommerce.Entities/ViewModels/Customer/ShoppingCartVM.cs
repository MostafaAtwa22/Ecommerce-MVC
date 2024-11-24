using Ecommerce.Entities.Models;

namespace Ecommerce.Entities.ViewModels.Customer
{
    public class ShoppingCartVM
    {
        public Product Product { get; set; } = default!;

        [Range(1, 10, ErrorMessage ="Range from 1 to 10")]
        public int Count { get; set; }
    }
}
