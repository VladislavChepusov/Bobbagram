using Api.Configs;
using AutoMapper;
using Common.Consts;
using Common;
using DAL.Entities;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR.Protocol;
using AutoMapper.QueryableExtensions;
using Api.Models.Token;
using Api.Exceptions;

namespace Api.Services
{
    public class AuthService
    {

        private readonly IMapper _mapper;
        private readonly DAL.DataContext _context;
        private readonly AuthConfig _config;

        public AuthService(IMapper mapper, IOptions<AuthConfig> config, DataContext context)
        {
            _mapper = mapper;
            _context = context;
            _config = config.Value;
        }


        // Верунть пользователю токен авторизации(в нем хранится пользователь и текущая сессия)
        public async Task<TokenModel> GetToken(string login, string password)
        {
            var user = await GetUserByCredention(login, password);
            var session = await _context.UserSessions.AddAsync(new DAL.Entities.UserSession
            {
                User = user,
                RefreshToken = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                Id = Guid.NewGuid()
            });
            await _context.SaveChangesAsync();
            return GenerateTokens(session.Entity);
        }




        // Вовзращение онбовленные токены! (вводим рефреш старый получаем новые ацес и рефреш)
        public async Task<TokenModel> GetTokenByRefreshToken(string refreshToken)
        {
            // Параметры Валидации токена
            var validParams = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey = _config.SymmetricSecurityKey()
            };

            var principal = new JwtSecurityTokenHandler().ValidateToken(refreshToken, validParams, out var securityToken);
            //securityToken это refreshToken ток не строка а объект
            // тут проверка валидации токена
            if (securityToken is not JwtSecurityToken jwtToken
                || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("invalid token");
            }

            if (principal.Claims.FirstOrDefault(x => x.Type == "refreshToken")?.Value is String refreshIdString
                 && Guid.TryParse(refreshIdString, out var refreshId)
                 )
            {
                var session = await GetSessionByRefreshToken(refreshId);
                if (!session.IsActive)
                {
                    throw new SessionNotFoundException();
                }
                // новая сессия 
                session.RefreshToken = Guid.NewGuid();
                await _context.SaveChangesAsync();
                return GenerateTokens(session);
            }
            else
            {
                throw new SecurityTokenException("invalid token");
            }
        }


        // Вернуть сессию по id
        public async Task<UserSession> GetSessionById(Guid id)
        {
            var session = await _context.UserSessions.FirstOrDefaultAsync(x => x.Id == id);
            if (session == null)
            {
                throw new SessionNotFoundException();
            }
            return session;
        }


        // Проверить есть ли данный пользователь в БД(по логину и паролю).
        // Проверить верный ли пароль
        private async Task<User> GetUserByCredention(string login, string pass)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == login.ToLower());
            if (user == null)
                throw new UserNotFoundException();

            if (!HashHelper.Verify(pass, user.PasswordHash))
                throw new Exception("password is incorrect");

            return user;
        }


        // Генеррация токенов (внутри создание и обновление сессии)
        private TokenModel GenerateTokens(UserSession session)
        {
            var dtNow = DateTime.Now; // текущее дата+время для жизни токена

            //Описание токена
            var jwt = new JwtSecurityToken(  // json web token - настройки
                issuer: _config.Issuer,
                audience: _config.Audience,
                notBefore: dtNow,

                claims: new Claim[] {
                            new Claim(ClaimsIdentity.DefaultNameClaimType, session.User.Name),
                            new Claim("sessionId", session.Id.ToString()),
                            new Claim("id", session.User.Id.ToString()),
                            },

                expires: DateTime.Now.AddMinutes(_config.LifeTime),
                signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );
            // Подписываем токен 
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            // рефреш токен
            var refresh = new JwtSecurityToken(
                notBefore: dtNow,

                claims: new Claim[] {
                new Claim("refreshToken", session.RefreshToken.ToString()), // будет в токене ID
                },

                expires: DateTime.Now.AddHours(_config.LifeTime),
                signingCredentials: new SigningCredentials(_config.SymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
                );
            var encodedRefresh = new JwtSecurityTokenHandler().WriteToken(refresh);
            return new TokenModel(encodedJwt, encodedRefresh);
        }


        // Получить рефреш токен
        private async Task<UserSession> GetSessionByRefreshToken(Guid id)
        {
            var session = await _context.UserSessions.Include(x => x.User).FirstOrDefaultAsync(x => x.RefreshToken == id);
            if (session == null)
            {
                throw new SessionNotFoundException();
            }
            return session;
        }
    }
}
