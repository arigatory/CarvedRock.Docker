using CarvedRock.Api.Domain;
using CarvedRock.Api.Integrations;
using CarvedRock.Api.Interfaces;
using CarvedRock.Api.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

namespace CarvedRock.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //var connectionString = "hello";
            var connectionString = Configuration.GetConnectionString("Db");

            //var simpleProperty = "hey";
            var simpleProperty = Configuration.GetValue<string>("SimpleProperty");

            //var nestedProp = "here we go";
            var nestedProp = Configuration.GetValue<string>("Inventory:NestedProperty");

            //Log.ForContext("ConnectionString", connectionString)
            //    .ForContext("SimpleProperty", simpleProperty)
            //    .ForContext("Inventory:NestedProperty", nestedProp)
            //    .Information("Loaded configuration!", connectionString);

            var dbgView = (Configuration as IConfigurationRoot).GetDebugView();
            Log.ForContext("ConfigurationRoot", dbgView)
                .Information("Configuration dump.");

            services.AddScoped<IProductLogic, ProductLogic>();
            services.AddScoped<IQuickOrderLogic, QuickOrderLogic>();
            services.AddSingleton<IOrderProcessingNotification, OrderProcessingNotification>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CarvedRock.Api", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<CustomExceptionHandlingMiddleware>();
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CarvedRock.Api v1"));
            }

            app.UseCustomRequestLogging();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
