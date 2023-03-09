using HotelListing.Api.Contracts;
using HotelListing.Api.Data;

namespace HotelListing.Api.Repository
{
    public class HotelsRepository : GenericRepository<Hotel>,IHotelsRepositoty
    {
        public HotelsRepository(HotelListingDbContext context) : base(context)
        {
        }
    }
}
