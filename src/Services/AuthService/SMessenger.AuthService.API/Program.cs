using Microsoft.Extensions.Options;
using SMessenger.AuthService.Infrastructure.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<JwtSettings>>().Value);

var app = builder.Build();

app.Run();