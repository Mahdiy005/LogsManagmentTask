using LogsManagment.Core.Common;
using LogsManagment.Core.Features.Users.Commands.Models;
using LogsManagment.Data.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace LogsManagment.Core.Features.Users.Commands.Handlers
{
    public class LoginUserHandler : ResponseHandler, IRequestHandler<LoginUserCommandModel, ResponseToken<string>>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _config;

        public LoginUserHandler(UserManager<AppUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
        }

        public async Task<ResponseToken<string>> Handle(LoginUserCommandModel request, CancellationToken cancellationToken)
        {
            var responseToken = new ResponseToken<string>();
            // Find user by username
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return new ResponseToken<string>("Invalid username or password", false);


            var jwtSecurityToken = await Helpers.CreateJwtToken(user, _userManager, _config);

            string jwt = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            // check if user have active refresh token 
            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                // if have select it
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                // assign it to returned object to the front end `authModel`
                responseToken.RefreshToken = activeRefreshToken.Token;
                responseToken.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                // if not have , generate one
                var refreshToken = Helpers.GenerateRefreshToken();
                // assign it to returned object to the front end `authModel`
                responseToken.RefreshToken = refreshToken.Token;
                responseToken.RefreshTokenExpiration = refreshToken.ExpiresOn;
                // add the generated token to the user
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            responseToken.Message = jwt;
            responseToken.Succeeded = true;

            return responseToken;
        }

        //private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        //{
        //    var _jwt = _config.GetSection("Jwt");

        //    // Get All User Claims
        //    var userClaims = await _userManager.GetClaimsAsync(user);
        //    // Get All Roles For User
        //    var roles = await _userManager.GetRolesAsync(user);
        //    var roleClaims = new List<Claim>();

        //    foreach (var role in roles)
        //        roleClaims.Add(new Claim("roles", role));

        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        //        new Claim("uid", user.Id.ToString())
        //    }
        //    .Union(userClaims)
        //    .Union(roleClaims);

        //    var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt["Key"]));
        //    var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        //    var jwtSecurityToken = new JwtSecurityToken(
        //        issuer: _jwt["Issuer"],
        //        audience: _jwt["Audience"],
        //        claims: claims,
        //        expires: DateTime.Now.AddMinutes(double.Parse(_jwt["DurationInMinutes"])),
        //        signingCredentials: signingCredentials);

        //    return jwtSecurityToken;
        //}


    }
}
