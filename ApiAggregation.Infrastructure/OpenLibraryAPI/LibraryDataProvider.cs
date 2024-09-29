using ApiAggregation.Application.DTOs;
using ApiAggregation.Application.Errors;
using ApiAggregation.Application.Interfaces.ExternalData;
using ApiAggregation.Domain.DomainModels;
using ApiAggregation.Infrastructure.OpenLibraryAPI.ResponseObjets;
using ApiAggregation.Infrastructure.RestCountries.ResponseObjects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace ApiAggregation.Infrastructure.OpenLibraryAPI
{
    public class LibraryDataProvider : ILibraryDataProvider
    {
        private readonly HttpClient _httpClient;

        public LibraryDataProvider(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// The data provider responsible for returning the OpenLibrary API data after mapping it to the Book Domain Model
        /// </summary>
        public async Task<Library> GetLibrary(RequestQuery requestParameters, CancellationToken cancellationToken)
        {
            var libraryFromExternalApi = await GetLibraryFromExternalApi(requestParameters, cancellationToken);

            if (libraryFromExternalApi != null)
            {
                Library mappedLibrary = libraryFromExternalApi.ToLibrary(); //map extention
                return mappedLibrary;
            }

            return new Library();
        }

        /// <summary>
        /// This method is responsible for fetching the country data from the OpenLibrary API,deserializing it with the help of Newtonsoft.Json to a response object
        /// </summary>
        private async Task<LibraryResponse> GetLibraryFromExternalApi(RequestQuery requestParameters, CancellationToken cancellationToken)
        {
            //setting the filters based on OpenLibrary API's documentation
            //using the title parameter provides "and"
            StringBuilder sbCountry = new();
            sbCountry.Append("title=");
            sbCountry.Append(string.Join(",", requestParameters.CountryNames));
            sbCountry.Append("&");

            //setting the desired fields according to the documentation
            var response = await _httpClient.GetAsync($"search.json?{sbCountry.ToString()}" +
                //Setting a default result limit.The final pagination happens to the DataAggregationService along with News API results
                $"limit=999&" + 
                $"&fields=title,publish_year,author_name,language", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var responseAsString = await response.Content.ReadAsStringAsync();
                    var libraryDeserialized = JsonConvert.DeserializeObject<LibraryResponse>(responseAsString);

                    if (libraryDeserialized != null)
                        return FilterIncomingBookTitles(libraryDeserialized, requestParameters);
                }
                catch (Exception ex)
                {
                    throw new RestException(HttpStatusCode.BadRequest, $"Error occured deserializing data from OpenLibrary API: {ex.Message}");
                }
            }
            return new LibraryResponse();
        }


        /// <summary>
        /// This method accepts the book results from the OpenLibrary API and filters their titles based on a IEnumerable of keywords
        /// </summary>
        private static LibraryResponse FilterIncomingBookTitles(LibraryResponse libraryResponse, RequestQuery requestParameters)
        {
            if (libraryResponse.Docs.Any())
            {
                var booksToGetFiltered = libraryResponse.Docs;
                //The response can contain books with the same title so filtering is needed
                //Also the OpenLibrary API does not contain a feature for logical OR so custom filtering must be applied
                var filteredBooks = booksToGetFiltered.DistinctBy(x => x.Title)
                                                  .Where(book => requestParameters.KeyWords.Any() ?
                                                        requestParameters.KeyWords.Any(keyWord => book.Title.Contains(keyWord, StringComparison.OrdinalIgnoreCase))
                                                            : true)
                                                  .ToList();

                return new LibraryResponse
                {
                    NumFound = filteredBooks.Count,
                    Docs = filteredBooks
                };
            }
            return new LibraryResponse();
        }
    }
}
