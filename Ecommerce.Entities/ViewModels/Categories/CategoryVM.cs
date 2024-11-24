namespace Ecommerce.Entities.ViewModels.Categories
{
    public class CategoryVM
    {
        [Required]
        [MinLength(3), MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(3000)]
        public string Description { get; set; } = string.Empty;
    }
}
