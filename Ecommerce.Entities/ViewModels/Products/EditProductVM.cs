namespace Ecommerce.Entities.ViewModels.Products
{
    public class EditProductVM : CommonProductVM
    {
        public int Id { get; set; }

        public string? CurrentImage { get; set; }

        [AllowedExtensions(FileSettings.AllowedExtensions),
            MaxFileSize(FileSettings.MaxFileSizeInBytes)]
        public IFormFile? Image { get; set; } = default!;
    }
}
