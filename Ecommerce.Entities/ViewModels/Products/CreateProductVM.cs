namespace Ecommerce.Entities.ViewModels.Products
{
    public class CreateProductVM : CommonProductVM
    {
        [Required]
        [AllowedExtensions(FileSettings.AllowedExtensions),
            MaxFileSize(FileSettings.MaxFileSizeInBytes)]
        public IFormFile Image { get; set; } = default!;
    }
}
