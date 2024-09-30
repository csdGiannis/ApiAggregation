using ApiAggregation.API.Controllers;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.OpenApi.Models;
using System.Reflection;
using ApiAggregation.Domain.DomainModels;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyModel;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using Polly.Caching;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.ComponentModel.Design;
using System.Runtime.Intrinsics.X86;
using System.Threading;
using System;

namespace ApiAggregation.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiConfig(this IServiceCollection services)
        {
            //fluent validation configuration
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<AggregationControllerValidator>();

            //adjusting incoming/outcoming formats
            services.AddControllers(options => options.ReturnHttpNotAcceptable = true) //throws 406 error when format is not supported
            .AddXmlDataContractSerializerFormatters(); //allows xml output

            //adding caching dependency
            services.AddMemoryCache();

            //swagger configuration to accept XML documentation 
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Aggregation Service",
                    Description = "This API Aggregation Service is designed to simplify data access by aggregating information from " +
                    "multiple external APIs into a single,unified endpoint.This service enhances the efficiency of data retrieval," +
                    "allowing users to access various data sources without the need to interact with each API individually.Additionally," +
                    " techniques such as asynchronous programming, caching,parallelism, circuit breaker,mapping,cancellation token, dependency injection, " +
                    " have been implemented to improve reliability and resilience.\n\n The external APIs utilized in this service include the following: " +
                    "REST Countries API which provides country data, News API which provides article data and OpenLibrary API which provides book data.\n\n" +
                    "The functionality of this API is to provide basic information, articles and books related to countries.For example, by inputting Greece and Italy, " +
                    "the user has the ability to find articles and books that associate with both of them.The user also has the ability to use keywords for more specific results," +
                    "filter, sort and paginate the results.The country names work as the logical AND between them, while the keywords as the logical OR.\n\n" +
                    "**User Input Validation**: In order to distinguish country inputs from keyword inputs, a local immutable array of country names is used for validating at each request. " +
                    "However, in a real - world scenario, a frequently updated database with formal country names would be preferred to ensure the data is always current and reliable.\n\n" +
                    "**Parallelism**: Parallelism is implemented during the asychronous requests to the three external APIs and when updating shared metrics while multiple threads are running, " +
                    "utilizing Interlocked for accurate incrementation (thread - safe).\n\n" +
                    "**Caching**: Caching is utilized by storing country data locally each time a request is made, enhancing retrieval efficiency.For demo purposes caching for 5 minutes although country data " +
                    "is static for a long period of time.\n\n" +
                    "**Metrics**: Implemented functionality to return the total number of requests and average response time for each API, grouped into performance buckets, " +
                    "featuring two endpoints: one for viewing results and another for resetting metrics. The metrics update each time a request is made.\n\n" +
                    "**Access Modifiers**: In order to perform tests, internal accessibilities were given to the relative test projects(configured at proj files).\n\n" +
                    "**Media Type**: Please request JSON/XML format or error 406 will occur."
                });

                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);
                c.IncludeXmlComments(xmlCommentsFullPath);

                var appCommentsFile = "ApiAggregation.Application.xml";
                var appCommentsPath = Path.Combine(AppContext.BaseDirectory, @"..\..\..\..", @"ApiAggregation.Application\bin\Debug\net8.0", appCommentsFile);
                c.IncludeXmlComments(appCommentsPath);
            });



            return services;
        }
    }
}
