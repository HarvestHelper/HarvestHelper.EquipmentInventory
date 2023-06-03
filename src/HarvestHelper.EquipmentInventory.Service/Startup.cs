using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using HarvestHelper.Common.MassTransit;
using HarvestHelper.Common.MongoDB;
using HarvestHelper.EquipmentInventory.Service.Clients;
using HarvestHelper.EquipmentInventory.Service.Entities;
using Polly;
using Polly.Timeout;
using HarvestHelper.Common.Identity;
using GreenPipes;
using HarvestHelper.EquipmentInventory.Service.Exceptions;
using HarvestHelper.Common.HealthChecks;

namespace HarvestHelper.EquipmentInventory.Service
{
    public class Startup
    {
        private const string AllowedOriginSetting = "AllowedOrigin";
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            serviceSettings = Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

            services.AddMongo()
                    .AddMongoRepository<EquipmentInventoryItem>("equipmentinventoryitems")
                    .AddMongoRepository<EquipmentItem>("equipmentitems")
                    .AddMassTransitWithMessageBroker(Configuration)
                    .AddJwtBearerAuthentication();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.Read, policy =>
                {
                    policy.RequireRole("Admin");
                    policy.RequireClaim("scope", "equipmentInventory.fullaccess", "equipmentInventory.readaccess");
                });

                options.AddPolicy(Policies.Write, policy =>
                {
                    policy.RequireRole("Admin");
                    policy.RequireClaim("scope", "equipmentInventory.fullaccess", "equipmentInventory.writeaccess");
                });
            });

            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HarvestHelper.EquipmentInventory.Service", Version = "v1" });
            });

             services.AddHealthChecks()
                    .AddMongoDb();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HarvestHelper.EquipmentInventory.Service v1"));

                app.UseCors(builder =>
                {
                    builder.WithOrigins(Configuration[AllowedOriginSetting])
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });  
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHarvestHelperHealthChecks();
            });
        }
    }
}
