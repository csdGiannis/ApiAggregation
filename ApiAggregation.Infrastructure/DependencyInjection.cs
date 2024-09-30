using ApiAggregation.Application.Interfaces.ExternalData;
using ApiAggregation.Infrastructure.Metrics;
using ApiAggregation.Infrastructure.NewsApi;
using ApiAggregation.Infrastructure.OpenLibraryAPI;
using ApiAggregation.Infrastructure.RestCountries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace ApiAggregation.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ICountriesDataProvider, CountriesDataProvider>();

            services.AddSingleton<IMetricsService, MetricsService>();

            services.AddHttpClient<ICountriesDataProvider, CountriesDataProvider>("restcountries", client =>
            {
                client.BaseAddress = new Uri("https://restcountries.com/v3.1/"); //base of uri
                client.DefaultRequestHeaders.Add("User-Agent", "ApiAggregationService"); //Adds User-Agent to the header to identify to the external API
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); //Format of requested data
            })
            .AddStantandPolicyHandler()
            .AddHttpMessageHandler(provider => new MetricsHttpClientHandler(provider.GetRequiredService<IMetricsService>(), "restcountries"));

            services.AddScoped<INewsDataProvider, NewsDataProvider>();
            services.AddHttpClient<INewsDataProvider, NewsDataProvider>("newsapi", client =>
            {
                client.BaseAddress = new Uri("https://newsapi.org/v2/");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration.GetSection("NewsAPI:ApiKey").Value}");
                client.DefaultRequestHeaders.Add("User-Agent", "ApiAggregationService");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddStantandPolicyHandler()
            .AddHttpMessageHandler(provider => new MetricsHttpClientHandler(provider.GetRequiredService<IMetricsService>(), "newsapi"));

            services.AddScoped<ILibraryDataProvider, LibraryDataProvider>();
            services.AddHttpClient<ILibraryDataProvider, LibraryDataProvider>("openlibrary", client =>
            {
                client.BaseAddress = new Uri("https://openlibrary.org/");
                client.DefaultRequestHeaders.Add("User-Agent", "ApiAggregation Local (ioannis.s.spyridonidis@gmail.com)");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            })
            .AddStantandPolicyHandler()
            .AddHttpMessageHandler(provider => new MetricsHttpClientHandler(provider.GetRequiredService<IMetricsService>(), "openlibrary"));

            return services;
        }
    }
}
