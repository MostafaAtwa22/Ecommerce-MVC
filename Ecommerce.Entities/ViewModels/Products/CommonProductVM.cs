using Microsoft.AspNetCore.Mvc.Rendering;
namespace Ecommerce.Entities.ViewModels.Products
{
    public class CommonProductVM
    {
        [Required]
        [MinLength(3), MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(3000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Categories")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> CategoriesList { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
