using Ecommerce.Entities.Models;

namespace Ecommerce.Entities.ViewModels
{
    public class HomeDetailsVM
    {
        public ShoppingCart Cart { get; set; } = default!;

        public IEnumerable<Product> Products { get; set;} = new List<Product>();
    }
}
