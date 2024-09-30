using ApiAggregation.Application.DTOs;
using ApiAggregation.Application.Errors;
using ApiAggregation.Application.Interfaces.ExternalData;
using ApiAggregation.Domain.DomainModels;
using ApiAggregation.Infrastructure.NewsApi.ResponseObjects;
using Newtonsoft.Json;
using System.Net;

namespace ApiAggregation.Infrastructure.NewsApi
{
    public class NewsDataProvider : INewsDataProvider
    {
        private readonly HttpClient _httpClient;

        public NewsDataProvider(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// The data provider responsible for returning the News API data after mapping it to the News Domain Model
        /// </summary>
        public async Task<News> GetNews(RequestQuery requestParameters, CancellationToken cancellationToken)
        {
            var newsFromExternalApi = await GetNewsByCountryFromExternalApi(requestParameters, cancellationToken);

            if (newsFromExternalApi != null)
            {
                News mappedNews = newsFromExternalApi.ToNews(); //map extention
                return mappedNews;
            }
            return new News();
        }


        /// <summary>
        /// This method is responsible for fetching the new data from the News API,deserializing it with the help of Newtonsoft.Json to a response object
        /// </summary>
        private async Task<NewsApiResponse> GetNewsByCountryFromExternalApi(RequestQuery requestParameters, CancellationToken cancellationToken)
        {
            var countrySearchQuery = string.Join(" AND ", requestParameters.CountryNames);
            if (requestParameters.KeyWords.Any())
            {
                var keyWordSearchQuery = " AND (" + string.Join(" OR ", requestParameters.KeyWords) + ')';
                countrySearchQuery += keyWordSearchQuery;
            }
     
            var requestUrl = $"everything?q={WebUtility.UrlEncode(countrySearchQuery)}&searchIn=title,description&pageSize=100"; //max pageSize is 100 so no more results can be retrieved with a single request

            var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var responseAsString = await response.Content.ReadAsStringAsync();
                    var newsDeserialized = JsonConvert.DeserializeObject<NewsApiResponse>(responseAsString);

                    //this status is provided by the News API
                        if (newsDeserialized != null && newsDeserialized.Status == "ok")
                        return FilterIncomingArticles(newsDeserialized, countryNames: requestParameters.CountryNames, keyWords: requestParameters.KeyWords);
                }
                catch (Exception ex)
                {
                    throw new RestException(HttpStatusCode.BadRequest, $"Error occured deserializing data from News API: {ex.Message}");
                }
            }
            return new NewsApiResponse();
        }

        /// <summary>
        /// This method accepts the news result from the News API and filters the articles based on an IEnumerable of strings
        /// </summary>
        private static NewsApiResponse FilterIncomingArticles(NewsApiResponse newsResponse, IEnumerable<string> countryNames,
                                                              IEnumerable<string> keyWords)
        {
            if(newsResponse.Articles.Any())
            {
                var filteredArticles = new List<ArticleResponse>();

                /*Response data has to be filtered through this api because sometimes it returns results without containing the keywords
                  while following the documentation. Also a 50 result limit is set for this project's purpose*/
                var articlesToGetFiltered = newsResponse.Articles;
                //The result must 1) contain all country names at the title or description 2) contain any of the keywords at the title or description
                filteredArticles = articlesToGetFiltered.Where(article => countryNames
                                                            .All(country => article.Title.Contains(country, StringComparison.OrdinalIgnoreCase)
                                                                || article.Description.Contains(country, StringComparison.OrdinalIgnoreCase)))
                                                         .Where(article => keyWords.Any() ?
                                                            keyWords.Any(keyWord => article.Title.Contains(keyWord, StringComparison.OrdinalIgnoreCase)
                                                                    || article.Description.Contains(keyWord, StringComparison.OrdinalIgnoreCase))
                                                                : true)
                                                        .Take(50).ToList();

                return new NewsApiResponse
                {
                    TotalResults = filteredArticles.Count,
                    Articles = filteredArticles
                };
            }
            return new NewsApiResponse();
        }
    }
}
