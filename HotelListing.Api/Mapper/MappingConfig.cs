using AutoMapper;
using HotelListing.Api.Data;
using HotelListing.Api.Models.Country;
using HotelListing.Api.Models.Hotel;

namespace HotelListing.Api.Mapper
{
    public class MappingConfig:Profile
    {
        public MappingConfig()
        {
            CreateMap<Country,CreateCountryDto>().ReverseMap();
            CreateMap<Hotel,HotelDto>().ReverseMap();
            CreateMap<Country,CountryDto>().ReverseMap();
            CreateMap<Country,CountryDetailsDto>().ReverseMap();
            CreateMap<Country,UpdateCountryDto>().ReverseMap();
            CreateMap<Hotel, HotelDto>().ReverseMap();
            CreateMap<Hotel, CreateHotelDto>().ReverseMap();    
            
        }

    }
}
