namespace Api.Models.Token
{
    public class TokenModel
    {
        // Модель токенов хранящаа Acces и Refresh токены пользователя
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public TokenModel(string accessToken, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
