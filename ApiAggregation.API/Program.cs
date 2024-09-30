using Serilog;
using ApiAggregation.API.Installers;
using ApiAggregation.API.Middleware;
using Serilog.Events;
using ApiAggregation.Infrastructure.NewsApi;
using ApiAggregation.API.Logging;
using ApiAggregation.SharedUtilites;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Diagnostics;

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
}).AddXmlDataContractSerializerFormatters(); //allows xml output
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Aggregation Service",
        Description = "This API Aggregation Service is designed to simplify data access by consolidating  information from multiple external APIs" +
        " into a single, unified  endpoint. This service enhances the efficiency of data retrieval," +
        " allowing users to access various data sources without the need to interact with each API individually. " +
        " Additionally, techniques such as asynchronous programming, parallelism, circuit breaker, mapping, have been implemented to " +
        "improve reliability and ensure consistent performance.\n\n" +
        "The external APIs utilized in this service include the following: REST Countries API which provides country data, " +
        "News API which provides article data and OpenLibrary API which provides book data.\n" +
        "The purpose of this API is to provide basic information, articles and books related to countries. For example, by inputting \"Greece\" and " +
        "\"Italy\", the user has the ability to find articles and books that associate with both of them. The user also has the ability to use keywords " +
        "for more specific results, filter, sort and paginate through the results."
    });

    var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
    c.IncludeXmlComments(xmlCommentsFullPath);

    var appCommentsFile = "ApiAggregation.Application.xml";
    var appCommentsPath = Path.Combine(AppContext.BaseDirectory, @"..\..\..\..", @"ApiAggregation.Application\bin\Debug\net8.0", appCommentsFile);
    c.IncludeXmlComments(appCommentsPath);
});

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
