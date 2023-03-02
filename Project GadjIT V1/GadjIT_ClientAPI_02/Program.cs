using GadjIT_ClientAPI.Services.Partner;
using GadjIT_ClientAPI.Services.Smartflow;
using GadjIT_ClientAPI_02.Middlewares;
using GadjIT_ClientContext.P4W;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

///********************************
///LOGGER with Serilog
/// 
//builder.Host.UseSerilog(); //(ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration)
var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddScoped<ISmartflow_DB_Service, Smartflow_DB_Service>();
builder.Services.AddScoped<IPartner_DB_Service, Partner_DB_Service>();


builder.Services.AddDbContext<P4W_Context>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("GadjIT_API")
                    , providerOptions => providerOptions.EnableRetryOnFailure(3,TimeSpan.FromSeconds(10), null)));



var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
    // specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("./swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
    
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

//run bespoke global error handling for controllers
//Controllers is past as a delegate and only if the delegate fails the error is handled
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
