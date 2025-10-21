using LogsManagment.Core.Common;

namespace LogsManagment.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<ResponseToken<string>> RefreshTokenAsync(string token);
    }
}
