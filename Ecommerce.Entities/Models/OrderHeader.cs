using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Entities.Models
{
    public class OrderHeader : BaseEntity
    {
        public string ApplicationUserId { get; set; } = string.Empty;

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; } = default!;

        public DateTime OrderDate { get; set; }

        public DateTime ShippingDate { get; set; }

        public decimal TotalPrice { get; set; }

        public string? OrderStatus { get; set; }

        public string? PaymentStatus { get; set; }

        public string? Carrier { get; set; }

        public DateTime PaymentDate { get; set; }
        
        // Strip Prop
        public string? SessionID { get; set; }

        public string? PaymentIntentId { get; set; }

        // Data Of users
        [Required]
        [MinLength(3), MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;
    }
}
