namespace Barber.IoT.Api
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Barber.IoT.Api.Bootstrap;
    using Barber.IoT.Api.Configuration;
    using Barber.IoT.Api.Mqtt;
    using Barber.IoT.Context;
    using Barber.IoT.Data.Model;
    using Barber.OpenApi.Extensions.Models;
    using Barber.OpenApi.Extensions.OperationFilter;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var builder = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json")
              .AddEnvironmentVariables();

            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            MqttServerService mqttServerService,
            MqttSettingsModel mqttSettings)
        {
            if (env.IsDevelopment())
            {
                _ = app.UseDeveloperExceptionPage();
            }
            else
            {
                _ = app.UseHsts();
            }

            // Swagger
            _ = app.UseSwagger(c =>
            {
                c.SerializeAsV2 = false;
                c.RouteTemplate = "api/{documentName}/openapi.json";
                ////c.PreSerializeFilters.Add(Swagger.TagGroupExtensions.AddGroups);
                c.PreSerializeFilters.Add((document, request) =>
                {
                    var server = new OpenApiServer()
                    {
                        Description = "Cobino API Server",
                        Url = request.IsHttps ? "https" : "http" + $"://{request.Host.Host}:{request.Host.Port}",
                    };

                    document.Servers.Add(server);

                    var securityJwt = new OpenApiSecurityScheme()
                    {
                        BearerFormat = "JWT",
                        Description = "Every API call needs to have a valid bearer JWT in the request header.",
                        Scheme = "bearer",
                        Type = SecuritySchemeType.Http,
                    };

                    var securityApiKey = new OpenApiSecurityScheme()
                    {
                        Description = "In special environment it is also possible to use a static API Key",
                        In = ParameterLocation.Header,
                        Name = "SecretApiKey",
                        Type = SecuritySchemeType.ApiKey,
                    };

                    var securitySchemas = new Dictionary<string, OpenApiSecurityScheme>
                        {
                            { "JWT", securityJwt },
                            { "SecretApiKey", securityApiKey },
                        };

                    document.Components.SecuritySchemes = securitySchemas;
                });
            });
            _ = app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "swagger";
                c.SwaggerEndpoint("/api/v1/openapi.json", "Barber IoT API");
            });
            _ = app.UseReDoc(c =>
            {
                c.DocumentTitle = "Barber IoT API";
                c.SpecUrl = "/api/v1/openapi.json";
                c.RoutePrefix = string.Empty;
            });

            _ = app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            ////.AllowCredentials());

            _ = app.UseStaticFiles();
            ////_ = app.UseHttpsRedirection();

            _ = app.UseRouting();

            // MQTT.Net
            _ = app.UseMqttNetWebSocketEndpoint(mqttServerService, mqttSettings);

            _ = app.UseAuthorization();
            _ = app.UseAuthentication();

            _ = app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _ = services.AddCors(c =>
            {
                c.AddPolicy("WebApp",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            ////.AllowCredentials()
                            .WithExposedHeaders("Content-Disposition");
                    });
            });

            _ = services.AddControllers();

            _ = services.AddDbContext<BarberIoTContext>(options =>
            {
                options.UseSqlite(this.Configuration.GetConnectionString("barber-main"));
            });

            // Swagger
            _ = services.AddSwaggerGen(c =>
            {
                var mainInfo = new OpenApiInfo()
                {
                    Version = "v1",
                    Title = "Cobino Web API",
                    ////Description = this.ReadFile("docs-api.md"),
                    Contact = new OpenApiContact()
                    {
                        Name = "Barber IoT",
                    },
                    License = new OpenApiLicense()
                    {
                        Name = "TBA",
                    },
                };

                mainInfo.Extensions.Add("x-logo", new OpenApiXLogo()
                {
                    Url = "/images/logo.png",
                    Text = "logo",
                });

                c.SwaggerDoc("v1", mainInfo);

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                // Operation Filters
                c.OperationFilter<DisplayNameOperationFilter>();
                c.OperationFilter<InternalServerErrorOperationFilter>();
            });

            // Barber.IoT
            _ = services.AddDeviceIdentityManager<Device, BarberIoTContext, string>();

            // MQTT.Net
            _ = services.AddMqttNetServer(this.Configuration);
        }
    }
}
