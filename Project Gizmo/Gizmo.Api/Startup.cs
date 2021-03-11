using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GadjIT.ClientAPI.Repository.Chapters;
using GadjIT.ClientAPI.Repository.Partner;
using GadjIT.ClientContext.P4W;
using GadjIT.ClientContext.P4W_Scaffold;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;



namespace GadjIT.ClientAPI
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
            services.AddControllers();
            services.AddDbContext<P4W_Context>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("OR_RESI_LIVE")));

            services.AddScoped<IChapters_Service, Chapters_Service>();
            services.AddScoped<IPartner_Access_Service, Partner_Access_Service>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
