using Ecommerce.Entities.Models;
using Ecommerce.Entities.ViewModels.Categories;
using X.PagedList; 

namespace Ecommerce.Models.ViewModels
{
    public class HomeViewModel
    {
        public IPagedList<KeyValuePair<EditCategoryVM, List<Product>>> PaginatedCategories { get; set; } = default!;

        public List<Product> BannerProducts { get; set; } = new();
    }
}
