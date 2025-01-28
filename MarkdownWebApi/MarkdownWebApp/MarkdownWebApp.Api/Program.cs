using System.Security.Claims;
using System.Text;
using MarkdownWebApp.DataAccess.Postgres;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MarkdownWebApi.Application;
using MarkdownWebApi.Application.Interfaces.Auth;
using MarkdownWebApi.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MarkdownDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("MarkdownDb"));
    });
builder.Services.AddAuthentication(
        options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(
        options =>
        {
            var jwtConfig = builder.Configuration.GetSection("JwtOptions");
            var key = Encoding.UTF8.GetBytes(jwtConfig.GetValue<string>("SecretKey") ?? string.Empty);
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                RequireExpirationTime = true,
                RequireSignedTokens = true,
            };
            options.Events = new JwtBearerEvents()
            {
                OnTokenValidated = context =>
                {
                    context.Properties.Items.Add("userId",
                        context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                    return Task.CompletedTask;
                }
            };
        });
builder.Services.AddScoped<IJwtWorker, JwtWorker>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}