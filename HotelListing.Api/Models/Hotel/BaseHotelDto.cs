using System.ComponentModel.DataAnnotations;

namespace HotelListing.Api.Models.Hotel
{
    public class BaseHotelDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        public double? Rating { get; set; }
        public int CountryId { get; set; }

    }
}
