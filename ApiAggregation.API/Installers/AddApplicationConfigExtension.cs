using AggregatedApi.Application.Services;
using ApiAggregation.Application.Interfaces;

namespace ApiAggregation.API.Installers
{
    public static class AddApplicationConfigExtension
    {
        public static IServiceCollection AddApplicationConfig(this IServiceCollection services)
        {
            services.AddScoped<IDataAggregationService, DataAggregationService>();

            return services;
        }
    }

}
