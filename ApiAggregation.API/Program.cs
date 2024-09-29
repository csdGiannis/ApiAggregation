using Serilog;
using ApiAggregation.API.Installers;
using ApiAggregation.API.Middleware;
using Serilog.Events;
using ApiAggregation.Infrastructure.NewsApi;
using ApiAggregation.API.Logging;
using ApiAggregation.SharedUtilites;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/ApiAggregation.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Warning)
    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
}).AddXmlDataContractSerializerFormatters();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//initializing the shared utility so the logger can be used in the Application lib
builder.Services.AddSingleton<IApiAggregationLogger, ApiAggregationLogger>(); 

builder.Services.AddApplicationConfig();
builder.Services.AddInfrastructureConfig(builder);

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
