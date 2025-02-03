using System.Text;
using MarkdownWebApi.Application;
using MarkdownWebApi.Application.Interfaces.Repositories;
using MarkdownWebApi.Application.Interfaces.Services;
using MarkdownWebApi.Application.Services;
using MarkdownWebApi.Infrastructure;
using MarkdownWebApp.Api.Filters.AccessFilters;
using MarkdownWebApp.Api.Filters.DocumentFilters;
using MarkdownWebApp.Api.Filters.UserFilters;
using MarkdownWebApp.DataAccess.Postgres.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace MarkdownWebApp.Api.Extensions;

public static class ApiExtensions
{
    public static void AddFilters(this IServiceCollection services)
    {
        services.AddScoped<RegisterValidationFilter>();
        services.AddScoped<LoginValidationFilter>();
        services.AddScoped<AllowAccessFilter>();
        services.AddScoped<ChangeAccessFilter>();
        services.AddScoped<DeleteAccessFilter>();
        services.AddScoped<GetDocumentAccessFilter>();
        services.AddScoped<GiveAccessFilter>();
        services.AddScoped<DeleteDocumentFilter>();
        services.AddScoped<EditDocumentFilter>();
        services.AddScoped<GetDocumentFilter>();
        services.AddScoped<RenameDocumentFilter>();
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDocumentRepository, DocumentRepository>();
        services.AddScoped<IDocumentAccessRepository, DocumentAccessRepository>();
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMinioService, MinioService>();
        services.AddScoped<IDocumentService, DocumentService>();
        services.AddScoped<IMarkdownService, MarkdownService>();
        services.AddScoped<IDocumentAccessService, DocumentAccessService>();
    }

    public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
        services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
            .AddJwtBearer(
                options =>
                {
                    var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();
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
                });
    }
}