using System.Security.Claims;
using System.Text;
using MarkdownWebApp.DataAccess.Postgres;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MarkdownWebApi.Application;
using MarkdownWebApi.Application.Interfaces.Auth;
using MarkdownWebApi.Application.Interfaces.Repositories;
using MarkdownWebApi.Application.Interfaces.Services;
using MarkdownWebApi.Application.Services;
using MarkdownWebApi.Application.Services.Options;
using MarkdownWebApi.Application.Validators;
using MarkdownWebApi.Infrastructure;
using MarkdownWebApp.Api.Filters;
using MarkdownWebApp.Api.Filters.UserFilters;
using MarkdownWebApp.DataAccess.Postgres.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true);
builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection(nameof(MinioOptions)));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MarkdownDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("MarkdownDb"));
    });

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(nameof(JwtOptions)));
builder.Services.AddAuthentication(
        options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(
        options =>
        {
            var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions!.SecretKey)),
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
builder.Services.AddScoped<IMinioService, MinioService>();
builder.Services.AddScoped<IPasswordHashier, PasswordHashier>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<RegisterValidationFilter>();
builder.Services.AddScoped<LoginValidationFilter>();

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();


app.Run();
