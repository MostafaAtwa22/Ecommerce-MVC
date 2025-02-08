using Ecommerce.Entities.Models;

namespace Ecommerce.Entities.ViewModels.Customer
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> CartList { get; set; } = new List<ShoppingCart>();
        public OrderHeader OrderHeader { get; set; } = default!;
    }

    public class OrderCartVM
    {

    }
}
