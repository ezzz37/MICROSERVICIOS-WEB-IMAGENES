using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1) Cargamos las rutas de Ocelot.
builder.Configuration
       .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// 2) Registramos servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3) Registramos Ocelot
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

// 4) Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

// Mapeamos nuestros controladores normales (p. ej. HealthController)
app.MapControllers();

// 5) Finalmente arrancamos Ocelot…
await app.UseOcelot();

// 6) …y dejamos Kestrel escuchando hasta que lo paremos
await app.RunAsync();
