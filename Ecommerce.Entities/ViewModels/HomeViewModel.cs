using Ecommerce.Entities.Models;
using Ecommerce.Entities.ViewModels.Categories;

namespace Ecommerce.Models.ViewModels
{
    public class HomeViewModel
    {
        public Dictionary<EditCategoryVM, List<Product>> ProductsByCategory { get; set; } = new();
        public List<Product> BannerProducts { get; set; } = new();
    }

}
