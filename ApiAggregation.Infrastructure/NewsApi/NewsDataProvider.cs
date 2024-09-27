using ApiAggregation.Application.DTOs;
using ApiAggregation.Application.Errors;
using ApiAggregation.Application.Interfaces.ExternalData;
using ApiAggregation.Domain.DomainModels;
using ApiAggregation.Infrastructure.NewsApi.ResponseObjects;
using ApiAggregation.Infrastructure.RestCountries.ResponseObjects;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Net;

namespace ApiAggregation.Infrastructure.NewsApi;

public class NewsDataProvider : INewsDataProvider
{
    private readonly HttpClient _httpClient;

    public NewsDataProvider(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
    public async Task<IEnumerable<NewsInformation>> GetNewsInformation(List<string> countryNames)
    {
        var newsData = new List<NewsInformation>();

        var newsInformationFromExternalApi = await GetNewsByCountryFromExternalApi(countryNames);

        if (newsInformationFromExternalApi.Status == "ok")
        {
            var articlesFromExternalApi = newsInformationFromExternalApi.Articles;
            foreach (var countryName in countryNames)
            {
                var articlesToMap = articlesFromExternalApi.Where(x => (x.Title != null && x.Title.ToLower().Contains(countryName))
                                                                || (x.Description != null && x.Description.ToLower().Contains(countryName))
                                                                || (x.Content != null && x.Content.ToLower().Contains(countryName))
                                                            ).ToList();
                if (articlesToMap.Any())
                {
                    newsData.Add(new NewsInformation
                    {
                        Country = countryName,
                        Articles = articlesToMap.ToArticles()
                    });
                }
                else
                    newsData.Add(new NewsInformation
                    {
                        Country = countryName,
                        Articles = new List<Article>()
                    });
            }
        }
        else
            throw new RestException(HttpStatusCode.BadRequest, $"Error retrieving {nameof(newsData)}");

        return newsData;
    }

    private async Task<NewsApiResponse> GetNewsByCountryFromExternalApi(List<string> countries)
    {
        var countrySearchQuery = string.Join(" OR ", countries);

        var requestUrl = $"everything?q={WebUtility.UrlEncode(countrySearchQuery)}&apiKey=be6c74549e2f499f9a405d96328f77f0";

        var response = await _httpClient.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            return new NewsApiResponse();
        }

        var responseAsString = await response.Content.ReadAsStringAsync();

        var news = JsonConvert.DeserializeObject<NewsApiResponse>(responseAsString);

        if (news == null)
        {   
            return new NewsApiResponse();
        }

        return news;
    }

}
