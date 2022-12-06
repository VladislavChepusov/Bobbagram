using DAL;
using Api.Configs;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Api.Middlewares;
using Api.Mapper;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Добавляем и регистрируем сервисы
        var authSection = builder.Configuration.GetSection(AuthConfig.Position);// Получение конфига для авторизации
        var authConfig = authSection.Get<AuthConfig>();// Получение параметров из authSection
        builder.Services.Configure<AuthConfig>(authSection);// Регистрируем (добавлеяем в контейнер)

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        
        // builder.Services.AddSwaggerGen();
        // Расширяем настройки свагера(его интерфейса),добавляя возможность вводить токен 
        builder.Services.AddSwaggerGen(c =>
        {
            // Описание безопастности
            c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Description = "Input your auth token,pls",// Что увидит пользователь
                Name = "Authorization",
                In = ParameterLocation.Header,// Где находится токен(в хедере)
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
            });
            // Добавляем требования по безопастности
            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme,

                        },
                        Scheme = "oauth2",
                        Name = JwtBearerDefaults.AuthenticationScheme,
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });

            c.SwaggerDoc("Auth", new OpenApiInfo { Title = "Auth" });
            c.SwaggerDoc("Api", new OpenApiInfo { Title = "Api" });
        });


        // Подключение к БД
        builder.Services.AddDbContext<DAL.DataContext>(options =>
        {
            // Описываем какой провайдер используем,а также указываем строчку подключения
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"), sql => { });
        }, contextLifetime: ServiceLifetime.Scoped);
        // contextLifetime: ServiceLifetime.Scoped); изменил время жизни контекста на скоп


        builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);// Добавление автомаппера

        builder.Services.AddScoped<UserService>(); // Добавление сервис пользователя
        builder.Services.AddScoped<AuthService>();// Добавление сервис авторизации пользователя
        builder.Services.AddScoped<PostService>();// Добавление сервис постов пользователя
        builder.Services.AddScoped<LinkGeneratorService>();// Добавление сервиса генерации ссылок (аватар,контент)

        // Добавим middleware для JSON Web Token(Аутентификация),чтобы система знала как проверять токен
        builder.Services.AddAuthentication(o =>
        {
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o => // Настроим  middleware для JSON Web Token
        {
            o.RequireHttpsMetadata = false; // Выключаем проверку на ssl(тк https без нормального сертификата)
            o.TokenValidationParameters = new TokenValidationParameters // Параметры валидации токена
            {
                // Настройка проверок токена
                ValidateIssuer = true, // Провекра того кто выпустил сертификат
                ValidIssuer = authConfig.Issuer,// Имя автора сертификата берем из конфига
                ValidateAudience = true, // Провекра того аудиенцию
                ValidAudience = authConfig.Audience,
                ValidateLifetime = true,// Провекра времени жизни токена
                ValidateIssuerSigningKey = true, // Проверка подписи 
                IssuerSigningKey = authConfig.SymmetricSecurityKey(), 
                ClockSkew = TimeSpan.Zero, // Погрешность времени жизни токенов( по дефолту +5 минут)
            };
        });

        // Добавляем параметры авторизации,чтобы прога понимала что мы берем юзеров из токена
        builder.Services.AddAuthorization(o =>
        {
            // Добавляем политику проверки токена
            o.AddPolicy("ValidAccessToken", p =>
            {
                p.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                p.RequireAuthenticatedUser();//требуем аунтефиц юзера 
            });
        });


        var app = builder.Build();

        // Указываем,что при каждом запуске приложения должны выполняться миграции,чтобы обнолвения происходили сами
        // Создаем отдельный Scope в рамках которого вызываем миграции
        using (var serviceScope = ((IApplicationBuilder)app).ApplicationServices.GetService<IServiceScopeFactory>()?.CreateScope())
        {
            if (serviceScope != null)
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DAL.DataContext>();
                context.Database.Migrate();
            }
        }

        // Configure the HTTP request pipeline.
        //if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                // Разбиение на несколько сервисов
                c.SwaggerEndpoint("Api/swagger.json", "Api");
                c.SwaggerEndpoint("Auth/swagger.json", "Auth");
            });
        }

        // Прописывае api
        app.UseHttpsRedirection();
        // Используем аутентификацию 
        app.UseAuthentication();
        // Используем авторизацию
        app.UseAuthorization();
        // Используем (кастомную)валидацию токенов
        app.UseTokenValidator();
        app.UseGlobalErrorWrapper();
        app.MapControllers();
        // Запуск
        app.Run();
    }
}