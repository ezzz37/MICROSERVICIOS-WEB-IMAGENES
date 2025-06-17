using System.Text;
using AuthService.Data;
using AuthService.Helpers;
using AuthService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1) Solo HTTP en el puerto 8080
builder.WebHost.UseUrls("http://*:8080");

// 2) DbContext con cadena AuthDb
builder.Services.AddDbContext<AuthDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("AuthDb")));

// 3) Carga de opciones desde appsettings.json
builder.Services.Configure<EncryptionOptions>(
    builder.Configuration.GetSection("Encryption"));
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("JwtOptions"));

// 4) Servicios de aplicación
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// 5) Configura autenticación JWT
var jwtOpts = builder.Configuration
    .GetSection("JwtOptions")
    .Get<JwtOptions>();

if (jwtOpts is null)
    throw new InvalidOperationException("JwtOptions no configurado correctamente.");

var keyBytes = Encoding.UTF8.GetBytes(jwtOpts.Key);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        opts.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidIssuer = jwtOpts.Issuer,
            ValidAudience = jwtOpts.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// 6) MVC y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 7) Aplicar migraciones al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.Migrate();
}

// 8) Middleware HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
