using Serilog;
using ApiAggregation.API.Middleware;
using Serilog.Events;
using ApiAggregation.Infrastructure;
using ApiAggregation.API;
using ApiAggregation.Application;

//In real life situation the logger would be configured in the appsettings so changes could be applied without having to rebuild.
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

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
