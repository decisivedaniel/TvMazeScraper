using Microsoft.EntityFrameworkCore;
using TVMazeScraper.Data;
using TVMazeScraper.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProgramContext>((serviceProvider, options) =>
    {
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        var dataDirectory = environment.ContentRootPath + "/App_Data";
        if (!Directory.Exists(dataDirectory))
        {
            Directory.CreateDirectory(dataDirectory);
        }
        var databaseFile = dataDirectory + ".db";
        options.UseSqlite("Data Source=" + databaseFile);
    });

builder.Services.AddScoped<IShowService, SqliteShowService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
