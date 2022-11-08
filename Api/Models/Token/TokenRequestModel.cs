namespace Api.Models.Token
{

    public class TokenRequestModel
    {
        // Модель нужная для аутентификации
        public string Login { get; set; }
        public string Pass { get; set; }

        public TokenRequestModel(string login, string pass)
        {
            Login = login;
            Pass = pass;
        }
    }

    // Модель нужная  для авторизации
    public class RefreshTokenRequestModel
    {
        public string RefreshToken { get; set; }

        public RefreshTokenRequestModel(string refreshToken)
        {
            RefreshToken = refreshToken;
        }
    }
}
