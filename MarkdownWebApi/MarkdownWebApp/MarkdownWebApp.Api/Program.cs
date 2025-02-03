using MarkdownWebApp.DataAccess.Postgres;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MarkdownWebApi.Application.Interfaces.Auth;
using MarkdownWebApi.Application.Services.Options;
using MarkdownWebApi.Application.Validators;
using MarkdownWebApi.Infrastructure;
using MarkdownWebApp.Api.Extensions;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddEnvironmentVariables();
builder.Services.Configure<MinioOptions>(builder.Configuration.GetSection(nameof(MinioOptions)));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Определение схемы безопасности
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Применение схемы безопасности ко всем операциям
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<MarkdownDbContext>(
    options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("MarkdownDb"));
    });

builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddScoped<IJwtWorker, JwtWorker>();
builder.Services.AddScoped<IPasswordHashier, PasswordHashier>();

builder.Services.AddServices();

builder.Services.AddRepositories();

builder.Services.AddFilters();

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    Secure = CookieSecurePolicy.Always,
    HttpOnly = HttpOnlyPolicy.Always
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MarkdownDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();


app.Run();
