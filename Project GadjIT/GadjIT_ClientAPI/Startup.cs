using System;
using GadjIT_ClientAPI.Repository.Chapters;
using GadjIT_ClientAPI.Repository.GeneralAccess;
using GadjIT_ClientAPI.Repository.Partner;
using GadjIT_ClientContext.P4W;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;



namespace GadjIT_ClientAPI
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
                    Configuration.GetConnectionString("GadjIT_API")
                    ,builder => {
                        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    }));

            //services.AddSwaggerDocument();

            services.AddScoped<IChapters_Service, Chapters_Service>();
            services.AddScoped<IPartner_Access_Service, Partner_Access_Service>();
            services.AddScoped<IGeneralAccessService, GeneralAccessService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseOpenApi();
            //app.UseSwaggerUi3();

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
