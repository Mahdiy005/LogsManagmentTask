using LogsManagment.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LogsManagment.Core
{
    public class Helpers
    {
        public static void SetRefreshTokenInCookie(HttpResponse response, string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

        public static RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }

        public static async Task<JwtSecurityToken> CreateJwtToken(AppUser user, UserManager<AppUser> _userManager, IConfiguration _config)
        {
            var _jwt = _config.GetSection("Jwt");

            // Get All User Claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            // Get All Roles For User
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id.ToString())
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt["Key"]));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt["Issuer"],
                audience: _jwt["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_jwt["DurationInMinutes"])),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }


    }
}
