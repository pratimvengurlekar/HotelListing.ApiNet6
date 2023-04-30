using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.Models.ApiUser
{
    public class LoginDto
    {
        [Required]
        [StringLength(15, ErrorMessage = "Your password between {2} to {10 characters}", MinimumLength = 7)]
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string EmailId { get; set; }


    }
}