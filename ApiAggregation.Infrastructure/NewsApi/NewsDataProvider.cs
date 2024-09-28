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

    /// <summary>
    /// The data provider responsible for returning the News API data after mapping it to the Domain Model NewsInformation
    /// </summary>
    public async Task<IEnumerable<NewsInformation>> GetNewsInformation(List<string> countryNames, CancellationToken cancellationToken)
    {
        var newsData = new List<NewsInformation>();

        var newsInformationFromExternalApi = await GetNewsByCountryFromExternalApi(countryNames, cancellationToken);

        //this status is provided by the News API
        if (newsInformationFromExternalApi.Status == "ok")
        {
            var articlesFromExternalApi = newsInformationFromExternalApi.Articles;
            //making a seperate object of Articles for each Article based on containing the country keyword, mapping it and returning them as Domain model 
            foreach (var countryName in countryNames)
            {
                var articlesToMap = articlesFromExternalApi.Where(x => (x.Title != null && x.Title.ToLower().Contains(countryName))
                                                                || (x.Description != null && x.Description.ToLower().Contains(countryName))
                                                            ).ToList();
                if (articlesToMap.Any())
                {
                    newsData.Add(new NewsInformation
                    {
                        Country = countryName,
                        Articles = articlesToMap.ToArticles() //map extention for grouped response results into Domain model of NewsInformation
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


    /// <summary>
    /// This method is responsible for fetching the new data from the News API,deserializing it with the help of Newtonsoft.Json to a response object
    /// </summary>
    private async Task<NewsApiResponse> GetNewsByCountryFromExternalApi(List<string> countries, CancellationToken cancellationToken)
    {
        var countrySearchQuery = string.Join(" OR ", countries);

        var requestUrl = $"everything?q={WebUtility.UrlEncode(countrySearchQuery)}&apiKey=be6c74549e2f499f9a405d96328f77f0";

        var response = await _httpClient.GetAsync(requestUrl, cancellationToken);

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
