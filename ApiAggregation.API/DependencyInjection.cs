using AggregatedApi.Application.Services;
using ApiAggregation.API.Controllers;
using ApiAggregation.Application.Interfaces;
using FluentValidation.AspNetCore;
using FluentValidation;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace ApiAggregation.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiConfig(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<AggregationControllerValidator>();

            services.AddControllers(options => options.ReturnHttpNotAcceptable = true)
                    .AddXmlDataContractSerializerFormatters(); //allows xml output


            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Aggregation Service",
                    Description = "This API Aggregation Service is designed to simplify data access by consolidating  information from multiple external APIs " +
                    "into a single, unified  endpoint. This service enhances the efficiency of data retrieval, " +
                    "allowing users to access various data sources without the need to interact with each API individually. " +
                    "Additionally, techniques such as asynchronous programming, parallelism, circuit breaker, mapping, cancellation token have been implemented to " +
                    "improve reliability and resilience.\n\n" +
                    "The external APIs utilized in this service include the following: REST Countries API which provides country data, " +
                    "News API which provides article data and OpenLibrary API which provides book data.\n" +
                    "The purpose of this API is to provide basic information, articles and books related to countries. \nFor example, by inputting \"Greece\" and " +
                    "\"Italy\", the user has the ability to find articles and books that associate with both of them. The user also has the ability to use keywords " +
                    "for more specific results, filter, sort and paginate the results. \nIn the current implementation, a local immutable array is used for validating " +
                    "the country inputs at the beggining to avoid reliance on external APIs, removing invalid inputs even if external services are down do implement this validation. " +
                    "However, in a real-world scenario, a frequently updated database with formal country names would be used to ensure the data is always current and reliable."
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
