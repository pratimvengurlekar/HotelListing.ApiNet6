using HotelListing.Api.Models.Hotel;

namespace HotelListing.Api.Models.Country
{
    public class CountryDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        public virtual IList<HotelDto> Hotels { get; set; }
    }
}
