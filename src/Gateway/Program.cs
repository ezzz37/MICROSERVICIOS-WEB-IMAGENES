﻿using System;
using System.IO;
using System.Text;
using Gateway.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Cargar appsettings + ocelot.json + envvars
builder.Host.ConfigureAppConfiguration((ctx, cfg) =>
{
    cfg.SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
       .AddEnvironmentVariables();
});

// CORS: origen EXPLÍCITO para credenciales
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
        policy.WithOrigins("http://172.179.63.132", "http://localhost", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
    );
});

//JwtOptions + clave
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("JwtOptions")
);
var jwtOpts = builder.Configuration
    .GetSection("JwtOptions")
    .Get<JwtOptions>()
    ?? throw new InvalidOperationException("JwtOptions no configurado.");
var keyBytes = Encoding.UTF8.GetBytes(jwtOpts.Key);

// JWT Bearer
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

//Controllers, Swagger y Ocelot
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

//Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// ** CORS antes de Authentication y UseOcelot **
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

// Mapeo de controllers (e.g. HealthController)
app.MapControllers();

// Finalmente el proxy Ocelot
await app.UseOcelot();

app.Run();
