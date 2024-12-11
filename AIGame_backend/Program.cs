using AIGame_backend.Controllers;
using AIGame_backend.Services;
using AIGame_backend.Utility;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policyBuilder =>
    {
        var frontendUrls = Environment.GetEnvironmentVariable("FRONTEND_URL");

        var allowedOrigins = frontendUrls.Split(',', StringSplitOptions.RemoveEmptyEntries);

        foreach (var origin in allowedOrigins)
        {
            Console.WriteLine(origin);
        }

        policyBuilder.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(httpsOptions =>
    {
        httpsOptions.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
    });
});

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

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<Hashing>();
builder.Services.AddScoped<PasswordValidator>();
builder.Services.AddScoped<UserService>();

var app = builder.Build();

app.UseCors("AllowFrontend");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UserContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<JwtMiddleware>();
app.UseHttpsRedirection();
app.UseHsts();
app.MapControllers();
app.Run();
