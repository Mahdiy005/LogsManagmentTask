using LogsManagment.Core.Common;
using LogsManagment.Core.Features.Users.Queries.Models;
using LogsManagment.Services.Interfaces;
using MediatR;

namespace LogsManagment.Core.Features.Users.Queries.Handlers
{
    public class RefreshTokenHandler : ResponseHandler, IRequestHandler<RefreshTokenQueryModel, ResponseToken<string>>
    {
        private readonly IAuthService authService;

        public RefreshTokenHandler(IAuthService _authService)
        {
            authService = _authService;
        }
        public async Task<ResponseToken<string>> Handle(RefreshTokenQueryModel request, CancellationToken cancellationToken)
        {
            //var refreshToken = Request.Cookies["refreshToken"];

            var result = await authService.RefreshTokenAsync(request.RefreshToken);

            if (!result.Succeeded)
                return result;

            Helpers.SetRefreshTokenInCookie(request.Response, result.RefreshToken, result.RefreshTokenExpiration);

            return result;
        }
    }
}
