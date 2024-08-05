using AIGame_backend.Controllers;
using AIGame_backend.Utility;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "AI Game Backend API", Version = "v1" });
});
builder.Services.AddControllers();
builder.Services.AddDbContext<UserContext>(options =>
    options.UseSqlServer(
        $"Server={Environment.GetEnvironmentVariable("DB_SERVER")};" +
        $"Database={Environment.GetEnvironmentVariable("DB_BACKEND")};" +
        $"User Id={Environment.GetEnvironmentVariable("DB_USER")};" +
        $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
        "TrustServerCertificate=True;"));

builder.Services.AddSingleton<JwtService>();
builder.Services.AddSingleton<Hashing>();
builder.Services.AddSingleton<PasswordValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();