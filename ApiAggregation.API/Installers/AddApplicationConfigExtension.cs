using AggregatedApi.Application.Services;
using ApiAggregation.API.Controllers;
using ApiAggregation.Application.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace ApiAggregation.API.Installers
{
    public static class AddApplicationConfigExtension
    {
        public static IServiceCollection AddApplicationConfig(this IServiceCollection services)
        {
            services.AddScoped<IDataAggregationService, DataAggregationService>();

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<AggregationControllerValidator>();

            return services;
        }
    }

}
