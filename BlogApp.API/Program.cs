using BlogApp.API.Configuration;
using BlogApp.API.Core.Interfaces;
using BlogApp.API.Core.Entities;
using BlogApp.API.Infrastructure.Persistence;
using BlogApp.API.Infrastructure.Persistence.Contexts;
using BlogApp.API.Infrastructure.Services;
using BlogApp.API.Application.CQRS;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AutoRegister;
using System.Reflection;
using MongoDB.Driver;
using BlogApp.API.Application.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFastEndpointsConfig();

builder.Services.AddDbContext<CommandDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<QueryDbContext>(options =>
    options.UseMongoDB(builder.Configuration.GetConnectionString("MongoConnection"), "BlogApp"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<CommandDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddAutoregister(Assembly.GetExecutingAssembly());

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Register OutboxService
builder.Services.AddScoped<IOutboxService, OutboxService>();
// Register IMongoDatabase for OutboxProcessor
builder.Services.AddSingleton<IMongoDatabase>(sp => {
    var config = sp.GetRequiredService<IConfiguration>();
    var client = new MongoClient(config.GetConnectionString("MongoConnection"));
    return client.GetDatabase("BlogApp");
});
// Register OutboxProcessor
builder.Services.AddHostedService<OutboxProcessor>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen();
}

app.UseMiddleware<BlogApp.API.Api.Middleware.GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints();

using (var scope = app.Services.CreateScope())
{
    await DbInitializer.Initialize(scope.ServiceProvider);
}

app.Run(); 