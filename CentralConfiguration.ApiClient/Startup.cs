using System;
using CentralConfiguration.Core;
using CentralConfiguration.MessageBroker;
using CentralConfiguration.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CentralConfiguration.ApiClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton(typeof(IConsumer<>), typeof(RabbitMqConsumer<>));
            services.AddSingleton(new ConsumerSettings
            {
                ConsumerConnStr = Configuration["RabbitMqConnection:ConnectionString"],
                ApplicationName = Configuration["AppSettings:ApplicationName"],
                LocalSettingsPath = Configuration["AppSettings:LocalSettingsPath"],
                ConsumerInterval = int.TryParse(Configuration["RabbitMqConnection:ConsumerInterval"], out var consumerInterval) ? consumerInterval : 10
            });
            services.AddSingleton<IConfigurationReader, ConfigurationReader>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
