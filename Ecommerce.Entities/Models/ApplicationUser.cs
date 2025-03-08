using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Entities.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MinLength(3), MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        public byte[] ProfilePicture { get; set; } = default!;
    }
}
