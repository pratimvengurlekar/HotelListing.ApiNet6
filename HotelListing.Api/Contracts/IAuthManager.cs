using HotelListing.Api.Models.ApiUser;
using Microsoft.AspNetCore.Identity;

namespace HotelListing.Api.Contracts
{
    public interface IAuthManager
    {
        Task<IEnumerable<IdentityError>> Register(ApiUserDto apiUserDto);
        Task<AuthResponseDto> Login(LoginDto loginDto);

        Task<string> CreateRefreshToken();

        Task<AuthResponseDto> ValidateRefreshToken(AuthResponseDto responseDto);
    }
}
