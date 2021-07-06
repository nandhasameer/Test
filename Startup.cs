using DaVinciAdminApi.Helper;
using DaVinciAdminApi.Repositories;
using DaVinciAdminApi.Repositories.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;

namespace DaVinciAdminApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // Disable Form Attribute in controller action, 
            // for more info please visit https://docs.microsoft.com/en-us/aspnet/core/web-api/jsonpatch?view=aspnetcore-3.1 

            //services.AddControllers().ConfigureApiBehaviorOptions(options =>
            //{
            //    options.SuppressConsumesConstraintForFormFileParameters = true;
            //    options.SuppressInferBindingSourcesForParameters = true;
            //    options.SuppressModelStateInvalidFilter = true;
            //    options.SuppressMapClientErrors = true;
            //    options.ClientErrorMapping[404].Link =
            //        "https://httpstatuses.com/404";
            //});

            services.AddControllers()
             .AddNewtonsoftJson(options =>
             {
                 options.SerializerSettings.ContractResolver = new DefaultContractResolver();
             });


            // services.AddControllers();
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            // --< set  appsetting value    >--
            var appSettings = appSettingsSection.Get<AppSettings>();


            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "DaVinciAdmin API",
                    Description = "DaVinciAdmin ASP.NET Core Web API",
                    TermsOfService = new Uri("http://ysecit.com"),
                    Contact = new OpenApiContact
                    {
                        Name = "YsecIT",
                        Email = "tamilan.subramani@ysecit.com",
                        Url = new Uri("https://twitter.com/tamils1809"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("http://ysecit.com"),
                    }
                });
                //var filePath = Path.Combine(System.AppContext.BaseDirectory, "MyApi.xml");
                //c.IncludeXmlComments(filePath);
            });


            services.AddControllersWithViews().AddNewtonsoftJson();

            services.AddTransient(typeof(IDaVinciHos), typeof(DaVinciHosRepository));
            services.AddTransient(typeof(IDaVinci), typeof(DaVinciRepository));
            services.AddTransient(typeof(IDaVinciItemMgmt), typeof(DaVinciItemMgmtRepository));
            services.AddTransient(typeof(IDaVinciStockPerCompany), typeof(DaVinciStockPerCompanyRepository));
            services.AddTransient(typeof(IDaVinciCoupen), typeof(DaVinciCoupenRepository));
            services.AddTransient(typeof(IDaVinciExchangeRates), typeof(DaVinciExchangeRatesRepository));
            services.AddTransient(typeof(IDaVinciContacts), typeof(DaVinciContactsRepository));
            services.AddTransient(typeof(IDaVinciReport), typeof(DaVinciReportRepository));

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger - ui(HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DaVinciAdmin API V1");

            });



            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

        //    app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


    }
}
