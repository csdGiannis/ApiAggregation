using ApiAggregation.Application.Interfaces.ExternalData;
using ApiAggregation.Infrastructure.NewsApi;
using ApiAggregation.Infrastructure.OpenLibraryAPI;
using ApiAggregation.Infrastructure.RestCountries;
using Polly;
using System.Net.Http.Headers;

namespace ApiAggregation.API.Installers
{
    public static class AddInfrustructureConfigExtention
    {
        public static IServiceCollection AddInfrastructureConfig(this IServiceCollection services, WebApplicationBuilder builder)
        {
            services.AddScoped<ICountriesDataProvider, CountriesDataProvider>();
            services.AddHttpClient<ICountriesDataProvider, CountriesDataProvider>(client =>
            {
                client.BaseAddress = new Uri("https://restcountries.com/v3.1/"); //base of uri
                client.DefaultRequestHeaders.Add("User-Agent", "ApiAggregationService"); //Adds User-Agent to the header to identify to the external API
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); //Format of requested data
            }).AddPolicyHandler(Policy<HttpResponseMessage>
               .Handle<HttpRequestException>() //condition for the policy, only works when httprequest exception happens
               .OrResult(x => !x.IsSuccessStatusCode)
               .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 3, durationOfBreak: TimeSpan.FromSeconds(10))); //using Polly initialize circuit breaker

            services.AddScoped<INewsDataProvider, NewsDataProvider>();
            services.AddHttpClient<INewsDataProvider, NewsDataProvider>(client =>
            {
                client.BaseAddress = new Uri("https://newsapi.org/v2/"); 
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration.GetSection("NewsAPI:ApiKey").Value}");
                client.DefaultRequestHeaders.Add("User-Agent", "ApiAggregationService"); 
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); 
            }).AddPolicyHandler(Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(x => !x.IsSuccessStatusCode)
                .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 3, durationOfBreak: TimeSpan.FromSeconds(10)));

            services.AddScoped<ILibraryDataProvider, LibraryDataProvider>();
            services.AddHttpClient<ILibraryDataProvider, LibraryDataProvider>(client =>
            {
                client.BaseAddress = new Uri("https://openlibrary.org/");
                client.DefaultRequestHeaders.Add("User-Agent", "ApiAggregation Local (ioannis.s.spyridonidis@gmail.com)");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }).AddPolicyHandler(Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(x => !x.IsSuccessStatusCode)
                .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 3, durationOfBreak: TimeSpan.FromSeconds(10)));

            return services;
        }
    }
}
