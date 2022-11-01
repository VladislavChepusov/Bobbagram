using Api;
using Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Добавляем и регистрируем сервисы

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //Подключение к БД
        
        builder.Services.AddDbContext<DAL.DataContext>(options =>
        {
            //Описываем какой провайдер используем,а также указываем строчку подключения
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"), sql => { });
        });

        //Добавление автомаппера
        builder.Services.AddAutoMapper(typeof(MapperProfile).Assembly);
        //Добавление сервис 
        builder.Services.AddScoped<UserService>();

        var app = builder.Build();

        //Указываем,что при каждом запуске приложения должны выполняться миграции,чтобы обнолвения происходили сами
        //Создаем отдельный Scope в рамках которого вызываем миграции
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

        //Прописывае api
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}