using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using StackExchange.Redis;
using SMessenger.ChatService.API.Hubs;
using SMessenger.ChatService.API.Infrastructure;
using SMessenger.ChatService.API.Middleware;
using SMessenger.ChatService.Application.Interfaces;
using SMessenger.ChatService.Application.Services;
using SMessenger.ChatService.Domain.Interfaces;
using SMessenger.ChatService.Infrastructure.Persistence;
using SMessenger.ChatService.Infrastructure.Persistence.Repositories;
using SMessenger.ChatService.Infrastructure.Presence;
using SMessenger.ChatService.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));

// Repositories
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IChatMemberRepository, ChatMemberRepository>();
builder.Services.AddScoped<IChatEncryptedKeyRepository, ChatEncryptedKeyRepository>();
builder.Services.AddScoped<IUserPublicKeyRepository, UserPublicKeyRepository>();

// Application services
builder.Services.AddScoped<IChatService, SMessenger.ChatService.Application.Services.ChatService>();
builder.Services.AddScoped<IKeyDistributionService, KeyDistributionService>();
builder.Services.AddScoped<IPresenceService, PresenceService>();

// Infrastructure
builder.Services.AddSingleton<IPresenceCache, RedisPresenceCache>();
builder.Services.AddScoped<IChatNotifier, ChatHubNotifier>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // Allow SignalR clients to pass the JWT via query string (access_token)
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("DefaultCorsPolicy");
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
