using Serilog;
using ApiAggregation.API.Installers;
using ApiAggregation.API.Middleware;
using Serilog.Events;
using ApiAggregation.Infrastructure;
using ApiAggregation.API;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/ApiAggregation.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Warning)
    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddApiConfig();

builder.Services.AddApplicationConfig();

builder.Services.AddInfrastructureConfig(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
