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

// 2) DbContext apuntando a la cadena AuthDb
builder.Services.AddDbContext<AuthDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("AuthDb")));

// 3) Configura opciones desde appsettings.json
builder.Services.Configure<EncryptionOptions>(
    builder.Configuration.GetSection("Encryption"));
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("JwtOptions"));

// 4) Inyección de servicios de usuario y token
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// 5) Configura autenticación JWT
var jwtOpts = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>()!;
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

// 6) Añade controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// (Opcional) Aplicar migraciones si las tienes
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.Migrate();
}

// 7) Pipeline de middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// 8) Mapear controladores
app.MapControllers();

app.Run();
