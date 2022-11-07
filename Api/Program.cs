using Api;
using DAL;
using Api.Configs;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ��������� � ������������ �������
        var authSection = builder.Configuration.GetSection(AuthConfig.Position);// ��������� ������� ��� �����������
        var authConfig = authSection.Get<AuthConfig>();// ��������� ���������� �� authSection
        builder.Services.Configure<AuthConfig>(authSection);// ������������ (���������� � ���������)

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        
        // builder.Services.AddSwaggerGen();
        // ��������� ��������� �������(��� ����������),�������� ����������� ������� ����� 
        builder.Services.AddSwaggerGen(c =>
        {
            // �������� �������������
            c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Description = "Input your auth token,pls",// ��� ������ ������������
                Name = "Authorization",
                In = ParameterLocation.Header,// ��� ��������� �����(� ������)
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
            });
            // ��������� ���������� �� �������������
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
        });


        // ����������� � ��
        builder.Services.AddDbContext<DAL.DataContext>(options =>
        {
            // ��������� ����� ��������� ����������,� ����� ��������� ������� �����������
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"), sql => { });
        }, contextLifetime: ServiceLifetime.Scoped);
        // contextLifetime: ServiceLifetime.Scoped); �������� ����� ����� ��������� �� ����


        builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);// ���������� �����������

        builder.Services.AddScoped<UserService>(); // ���������� ������ 

        // ������� middleware ��� JSON Web Token(��������������)
        builder.Services.AddAuthentication(o =>
        {
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o => // ��������  middleware ��� JSON Web Token
        {
            o.RequireHttpsMetadata = false; // ��������� �������� �� ssl(�� https ��� ����������� �����������)
            o.TokenValidationParameters = new TokenValidationParameters // ��������� ��������� ������
            {
                // ��������� �������� ������
                ValidateIssuer = true, // �������� ���� ��� �������� ����������
                ValidIssuer = authConfig.Issuer,// ��� ������ ����������� ����� �� �������
                ValidateAudience = true, // �������� ���� ���������
                ValidAudience = authConfig.Audience,
                ValidateLifetime = true,// �������� ������� ����� ������
                ValidateIssuerSigningKey = true, // �������� ������� 
                IssuerSigningKey = authConfig.SymmetricSecurityKey(), 
                ClockSkew = TimeSpan.Zero, // ����������� ������� ����� �������( �� ������� +5 �����)
            };
        });

        // ��������� ��������� �����������
        builder.Services.AddAuthorization(o =>
        {
            // ��������� �������� �������� ������
            o.AddPolicy("ValidAccessToken", p =>
            {
                p.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                p.RequireAuthenticatedUser();
            });
        });


        var app = builder.Build();

        // ���������,��� ��� ������ ������� ���������� ������ ����������� ��������,����� ���������� ����������� ����
        // ������� ��������� Scope � ������ �������� �������� ��������
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
            app.UseSwaggerUI();
        }

        // ���������� api
        app.UseHttpsRedirection();
        // ���������� �������������� 
        app.UseAuthentication();
        // ���������� �����������
        app.UseAuthorization();
        // ���������� ��������� �������
        app.UseTokenValidator();
        app.MapControllers();
        // ������
        app.Run();
    }
}