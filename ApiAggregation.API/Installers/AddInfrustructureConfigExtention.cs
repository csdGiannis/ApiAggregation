using ApiAggregation.Application.Interfaces.ExternalData;
using ApiAggregation.Infrastructure.NewsApi;
using ApiAggregation.Infrastructure.RestCountries;
using System.Net.Http.Headers;

namespace ApiAggregation.API.Installers
{
    public static class AddInfrustructureConfigExtention
    {
        public static IServiceCollection AddInfrastructureConfig(this IServiceCollection services)
        {
            services.AddHttpClient<NewsDataProvider>();
            services.AddScoped<ICountriesDataProvider, CountriesDataProvider>();
            services.AddScoped<INewsDataProvider, NewsDataProvider>();
            services.AddHttpClient<INewsDataProvider, NewsDataProvider>(client =>
            {
                client.BaseAddress = new Uri("https://newsapi.org/v2/");
                client.DefaultRequestHeaders.Add("User-Agent", "CSharpApp");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            });

            return services;
        }
    }
}
