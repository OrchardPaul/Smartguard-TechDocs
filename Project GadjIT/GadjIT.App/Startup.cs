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
using GadjIT_App.Areas.Identity;
using GadjIT_App.Data;
using GadjIT.ClientContext.P4W;
using GadjIT_App.Services;
using GadjIT_App.Data.Admin;
using GadjIT_App.Data.MapperProfiles;
using AutoMapper;
using Plk.Blazor.DragDrop;
using GadjIT_App.Services.Email;
using Microsoft.AspNetCore.Identity.UI.Services;
using GadjIT_App.Services.SessionState;
using Blazored.Modal;
using GadjIT_App.Pages.Chapters;
using GadjIT_App.FileManagement.FileProcessing.Interface;
using GadjIT_App.FileManagement.FileProcessing.Implementation;
using Microsoft.Extensions.FileProviders;
using System.IO;
using GadjIT_App.Services.AppState;
using GadjIT_App.Pages.Accounts.CompanyAccountManagement;
using Serilog;
using GadjIT_App.Data.Dropzone_Objects;

namespace GadjIT_App
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
                    Configuration.GetConnectionString("OR_GadjIT_Web")));
            services.AddDbContext<AuthorisationDBContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("OR_GadjIT_Web")));

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();

            services.AddHttpClient<IChapterManagementService, ChapterManagementService>();
            services.AddHttpClient<IPartnerAccessService, PartnerAccessService>();

            services.AddScoped<IIdentityRoleAccess, IdentityRoleAccess>();
            services.AddScoped<IIdentityUserAccess, IdentityUserAccess>();
            services.AddScoped<ICompanyDbAccess, CompanyDbAccess>();

            services.AddBlazorDragDrop();
            services.AddAutoMapper(typeof(UserRoleProfile));
            services.AddAutoMapper(typeof(SmartflowRecordProfile));

            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);

            services.AddScoped<IUserSessionState, UserSessionState>();
            services.AddScoped<IMappingSessionState, MappingSessionState>();
            services.AddScoped<IPageAuthorisationState, PageAuthorisationState>();
            services.AddScoped<IUserManagementSelectedUserState, UserManagementSelectedUserState>();
            services.AddScoped<IChapterState, ChapterState>();
            services.AddBlazoredModal();
            services.AddScoped<IFileHelper, FileHelper>();
            services.AddScoped<IPDFHelper, PDFHelper>();
            services.AddScoped<IExcelHelper, ExcelHelper>();
            services.AddScoped<IChapterFileUpload, ChapterFileUpload>();
            services.AddScoped<JsConsole>();
            services.AddScoped(typeof(DragDropService<>));

            services.AddSingleton<IAppChapterState, AppChapterStateList>();

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

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });



            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //    Path.Combine(Directory.GetCurrentDirectory(), "FileManagement")),
            //    RequestPath = "/FileManagement"
            //});

            app.UseStaticFiles();

        }
    }
}
