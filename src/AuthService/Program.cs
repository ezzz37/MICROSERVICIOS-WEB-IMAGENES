using System;
using System.Text;
using AuthService.Data;
using AuthService.Helpers;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1) Hacer que escuche HTTP en el puerto 8080
builder.WebHost.UseUrls("http://*:8080");

// 2) Registrar el DbContext apuntando a la cadena "AuthDb" de appsettings.json
builder.Services.AddDbContext<AuthDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("AuthDb")));

// 3) Cargar opciones de cifrado y JWT desde appsettings.json
builder.Services.Configure<EncryptionOptions>(
    builder.Configuration.GetSection("Encryption"));
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("JwtOptions"));

// 4) Registrar tus servicios de dominio
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// 5) Configurar autenticación JWT Bearer
var jwtOpts = builder.Configuration
    .GetSection("JwtOptions")
    .Get<JwtOptions>()
    ?? throw new InvalidOperationException("JwtOptions no configurado.");

var keyBytes = Encoding.UTF8.GetBytes(jwtOpts.Key);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.RequireHttpsMetadata = false;
        opts.SaveToken = true;
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer           = true,
            ValidIssuer              = jwtOpts.Issuer,
            ValidateAudience         = true,
            ValidAudience            = jwtOpts.Audience,
            ValidateLifetime         = true,
            ClockSkew                = TimeSpan.Zero
        };
    });

// 6) MVC + JSON options + Swagger
builder.Services
    // <-- Aquí está la magia: propertyNameCaseInsensitive
    .AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 7) Aplicar migraciones automáticamente al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.Migrate();
}

// 8) Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
