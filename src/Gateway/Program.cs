using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Carga de JSONs
        builder.Configuration
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
               .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

        // 1) Registrar MVC y Ocelot
        builder.Services.AddControllers();
        builder.Services.AddOcelot(builder.Configuration);

        // 2) Registrar Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // 3) CORS y Auth (como antes)
        builder.Services.AddCors(o =>
            o.AddPolicy("CorsPolicy", p =>
                p.AllowAnyOrigin()
                 .AllowAnyHeader()
                 .AllowAnyMethod()));
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(opts =>
               {
                   var jwt = builder.Configuration.GetSection("JwtSettings");
                   opts.Authority = jwt["Authority"];
                   opts.Audience = jwt["Audience"];
                   opts.RequireHttpsMetadata = false;
               });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            // 4) Habilitar Swagger UI en Development
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway API V1");
                c.RoutePrefix = "swagger";  // /swagger
            });
        }

        app.UseCors("CorsPolicy");

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        // 5) Ocelot al final
        await app.UseOcelot();

        // 6) Mantén vivo el host
        app.Run();
    }
}
