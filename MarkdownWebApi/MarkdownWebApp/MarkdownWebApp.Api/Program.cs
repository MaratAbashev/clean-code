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
using MarkdownWebApp.Api.Extensions;
using MarkdownWebApp.Api.Filters;
using MarkdownWebApp.Api.Filters.AccessFilters;
using MarkdownWebApp.Api.Filters.DocumentFilters;
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

builder.Services.AddAuthentication(builder.Configuration);
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

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();


app.Run();
