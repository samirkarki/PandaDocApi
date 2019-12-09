using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PandaDoc.Standard;

namespace Esign.Api
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
            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

            var eSignConfig = Configuration.GetSection("ESignConfig");
            var idServer = Configuration.GetSection("IDServer");

            ESignConfig.PandaDocApiKey = eSignConfig["PandaDocApiKey"];
            ESignConfig.PandaDocAuthUrl = eSignConfig["PandaDocAuthUrl"];
            ESignConfig.PandaDocApiUrl = eSignConfig["PandaDocApiUrl"];
            ESignConfig.DocumentLifetime = Convert.ToInt32(eSignConfig["DocumentLifetime"]);

            IDServer.Url= idServer["Url"];
            IDServer.Audience= idServer["Audience"];
            
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = IDServer.Url;
                    options.RequireHttpsMetadata = false;
                    options.Audience = IDServer.Audience;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
