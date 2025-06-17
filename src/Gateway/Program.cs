using System;
using System.IO;
using System.Text;
using Gateway.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1) Configuración de fuentes (appsettings, ocelot y envvars)
builder.Host.ConfigureAppConfiguration((ctx, cfg) =>
{
    cfg.SetBasePath(Directory.GetCurrentDirectory());
    cfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    cfg.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
    cfg.AddEnvironmentVariables();
});

// 2) Bind JwtOptions
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
var jwtOpts = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>()
             ?? throw new InvalidOperationException("JwtOptions no configurado.");
var keyBytes = Encoding.UTF8.GetBytes(jwtOpts.Key);

// 3) Configuración de autenticación JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.RequireHttpsMetadata = false;
        opt.SaveToken = true;
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = true,
            ValidIssuer = jwtOpts.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOpts.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// 4) Autorización, controllers y Swagger
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 5) Registro de Ocelot
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// 6) Pipeline de middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Tus controladores propios (p.ej. HealthController)
app.MapControllers();

// Configura Ocelot como proxy (esta llamada devuelve un Task que bloquea)
await app.UseOcelot();

// Finalmente, arranca Kestrel y mantén el proceso vivo
app.Run();
