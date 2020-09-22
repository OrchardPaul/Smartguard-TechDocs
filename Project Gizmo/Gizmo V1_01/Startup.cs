using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Gizmo_V1_01.Data;
using Gizmo_V1_01.Services;
using Gizmo.Context.OR_RESI;
using Microsoft.EntityFrameworkCore;
using Gizmo_V1_01.Data.OR_RESI_Chapters;

namespace Gizmo_V1_01
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();

            services.AddHttpClient<IChapterManagementService, ChapterManagementService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:44399/");
            });

            services.AddDbContext<P4W_OR_RESI_V5_DEVContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("OR_RESI_LIVE")));

            services.AddScoped<OR_RESI_Chapters_Service>();
            services.AddTransient<IOR_RESI_Chapters_Service, OR_RESI_Chapters_Service>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
