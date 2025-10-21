using System.Text.Json.Serialization;

namespace LogsManagment.Core.Common
{
    public class ResponseToken<T> : Response<T>
    {
        [JsonIgnore]
        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiration { get; set; }

        public ResponseToken(string data, bool succeed) : base(data, succeed)
        {

        }
        public ResponseToken()
        {

        }

        public ResponseToken(string data, bool succeed, string refreshToken, DateTime refreshTokenExpire) : base(data, succeed)
        {
            RefreshToken = refreshToken;
            RefreshTokenExpiration = refreshTokenExpire;
        }
    }
}
