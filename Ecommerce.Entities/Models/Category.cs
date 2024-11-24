namespace Ecommerce.Entities.Models
{
    public class Category : BaseEntity
    {
        [Required]
        [MinLength(3), MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(3000)]
        public string Description { get; set; } = string.Empty;

        public DateTime CreatedTime { get; set; } = DateTime.Now;

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
