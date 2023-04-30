using AutoMapper;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.Models.ApiUser;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListing.Api.Repository
{
    public class AuthManager : IAuthManager
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;
        private ApiUser _user;

        public AuthManager(IMapper mapper,UserManager<ApiUser> userManager,IConfiguration configuration)
        {
            this._mapper = mapper;
            this._userManager = userManager;
            this._configuration = configuration;
        }

        public async Task<string> CreateRefreshToken()
        {
            await _userManager.RemoveAuthenticationTokenAsync(_user, "HotelListingApi", "RefreshToken");
            var refreshToken = await _userManager.GenerateUserTokenAsync(_user, "HotelListingApi", "RefreshToken");
            var result = await _userManager.SetAuthenticationTokenAsync(_user, "HotelListingApi", "RefreshToken",refreshToken);
            return refreshToken;
        }

        public async Task<AuthResponseDto> Login(LoginDto loginDto)
        {
           
                _user  = await _userManager.FindByEmailAsync(loginDto.EmailId);
                var isValidUser = await _userManager.CheckPasswordAsync(_user, loginDto.Password);
                if (_user is null || isValidUser ==false)
                {
                    return null;

                }

                var token = await GenerateToken();
            return new AuthResponseDto
            {
                Token = token,
                UserId= _user.Id,
                RefreshToken = await CreateRefreshToken()

            };
            
            


        }

        public async Task<IEnumerable<IdentityError>> Register(ApiUserDto apiUserDto)
        {
            var user = _mapper.Map<ApiUser>(apiUserDto);
            user.UserName = apiUserDto.EmailId;
            user.Email = apiUserDto.EmailId;
            var result = await _userManager.CreateAsync(user, apiUserDto.Password);
            if(result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
            }

            return result.Errors;
        }

        public async Task<AuthResponseDto> ValidateRefreshToken(AuthResponseDto responseDto)
        {
            var jwtSecurityHandler = new JwtSecurityTokenHandler();
            var tokenContent = jwtSecurityHandler.ReadJwtToken(responseDto.Token);
            var username = tokenContent.Claims.ToList().FirstOrDefault(q => q.Type == JwtRegisteredClaimNames.Email)?.Value;
            _user = await _userManager.FindByNameAsync(username);
            if (_user == null) { return null; }
            var isValidRefreshToken = await _userManager.VerifyUserTokenAsync(_user, "HotelListingApi", "RefreshToken", responseDto.RefreshToken);
            if (isValidRefreshToken)
            {
                var token = await GenerateToken();
                return new AuthResponseDto()
                {
                    Token = token,
                    UserId = _user.Id,
                    RefreshToken = await CreateRefreshToken()


                };


            }
            await _userManager.UpdateSecurityStampAsync(_user);
            return null;

        }

        private async Task<string> GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var roles = await _userManager.GetRolesAsync(_user);
            var roleClaims = roles.Select(x=> new Claim(ClaimTypes.Role,x)).ToList();
            var userClaims = await _userManager.GetClaimsAsync(_user);
            var claims =new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub,_user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email,_user.Email),
                new Claim("uid",_user.Id)
            }.Union(userClaims).Union(roleClaims);
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
                
                );
            return new JwtSecurityTokenHandler().WriteToken(token);



        }
    }
}
