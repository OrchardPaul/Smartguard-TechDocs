using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Gizmo_V1_02.Areas.Identity;
using Gizmo_V1_02.Data;
using Gizmo.Context.OR_RESI;
using Gizmo_V1_02.Services;
using Gizmo_V1_02.Data.OR_RESI_Chapters;
using Gizmo_V1_02.Data.Admin;
using Gizmo_V1_02.Data.MapperProfiles;
using AutoMapper;
using Plk.Blazor.DragDrop;

namespace Gizmo_V1_02
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("AuthentificationConnection")));
            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();

            services.AddHttpClient<IChapterManagementService, ChapterManagementService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:44399/");
            });

            services.AddDbContext<P4W_OR_RESI_V5_DEVContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("OR_RESI_LIVE")));

            services.AddTransient<IIdentityRoleAccess, IdentityRoleAccess>();
            services.AddScoped<IIdentityUserAccess, IdentityUserAccess>();

            services.AddTransient<IOR_RESI_Chapters_Service, OR_RESI_Chapters_Service>();

            services.AddBlazorDragDrop();
            services.AddAutoMapper(typeof(UserRoleProfile));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
