using LogsManagment.Core;
using LogsManagment.Core.Common;
using LogsManagment.Data.Entities;
using LogsManagment.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;


namespace LogsManagment.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;

        public AuthService(UserManager<AppUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }
        public async Task<ResponseToken<string>> RefreshTokenAsync(string token)
        {
            var respnseToken = new ResponseToken<string>();
            // check if there are Users have that Token 
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            // if null so no users have that token
            if (user == null)
            {
                respnseToken.Message = "Invalid token";
                return respnseToken;
            }

            // select RefreshToken that user send
            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            // if not active return message
            if (!refreshToken.IsActive)
            {
                respnseToken.Message = "Inactive token";
                return respnseToken;
            }

            // IMPORTANT : each refresh take must refresh only one token , and the RevokedOn it 
            refreshToken.RevokedOn = DateTime.UtcNow;

            // Generate New Refresh Token 
            var newRefreshToken = Helpers.GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            // Generate JWT Token 
            var jwtToken = await Helpers.CreateJwtToken(user, _userManager, _config);
            respnseToken.Succeeded = true;
            respnseToken.Message = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            //respnseToken.Email = user.Email;
            //respnseToken.Username = user.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            //respnseToken.Roles = roles.ToList();
            respnseToken.RefreshToken = newRefreshToken.Token;
            respnseToken.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return respnseToken;
        }
    }
}
