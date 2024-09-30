using ApiAggregation.Application.Interfaces;
using ApiAggregation.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ApiAggregation.Application
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
