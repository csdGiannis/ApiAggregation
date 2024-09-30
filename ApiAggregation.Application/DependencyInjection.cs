using AggregatedApi.Application.Services;
using ApiAggregation.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ApiAggregation.API.Installers
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationConfig(this IServiceCollection services)
        {
            services.AddScoped<IDataAggregationService, DataAggregationService>();

            return services;
        }
    }

}
