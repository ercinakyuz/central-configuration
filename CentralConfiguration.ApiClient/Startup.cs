using System.Collections.Generic;
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

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.Configure<AppSettings>(options =>
            {
                options.StaticSettings = Configuration.GetSection("StaticSettings").Get<StaticSettings>();
                options.DynamicSettings = Configuration.GetSection("DynamicSettings").Get<IList<ConfigurationDto>>();
            });
            services.AddTransient(typeof(IConsumer<>), typeof(RabbitMqConsumer<>));
            services.AddHostedService<ConfigurationConsumerService>();
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
