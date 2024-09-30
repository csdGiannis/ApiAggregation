using ApiAggregation.Application.DTOs;
using ApiAggregation.Application.Errors;
using ApiAggregation.Application.Interfaces.ExternalData;
using ApiAggregation.Domain.DomainModels;
using ApiAggregation.Infrastructure.RestCountries.ResponseObjects;
using Newtonsoft.Json;
using System.Net;

namespace ApiAggregation.Infrastructure.RestCountries
{
    public class CountriesDataProvider : ICountriesDataProvider
    {
        private readonly HttpClient _httpClient;

        public CountriesDataProvider(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// The data provider responsible for returning the RestCountries API data after mapping it to the Country Domain Model
        /// </summary>
        public async Task<IEnumerable<Country>> GetCountries(RequestQuery requestParameters, CancellationToken cancellationToken)
        {
            var countriesFromExternalApi = await GetCountriesFromExternalApi(requestParameters.CountryNames, cancellationToken);

            List<Country> mappedCountries = new();

            foreach (RestCountriesResponse externalCountry in countriesFromExternalApi)
            {
                //extention method for mapping response object into data model
                var mappedCountry = externalCountry?.ToCountry();
                if (mappedCountry != null)
                    mappedCountries.Add(mappedCountry);
            }

            return mappedCountries;
        }

        /// <summary>
        /// This method is responsible for fetching the country data from the RestCountries API,deserializing it with the help of Newtonsoft.Json to a response object
        /// </summary>
        private async Task<IEnumerable<RestCountriesResponse>> GetCountriesFromExternalApi(IEnumerable<string> countryNames, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"all", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var responseAsString = await response.Content.ReadAsStringAsync();
                    var countriesDeserialized = JsonConvert.DeserializeObject<List<RestCountriesResponse>>(responseAsString);

                    if (countriesDeserialized != null)
                        return countriesDeserialized.Where(x => countryNames.Contains(x.Name.Official.ToLower()) 
                                                            || countryNames.Contains(x.Name.Common.ToLower()));
                }
                catch (Exception ex)
                {
                    throw new RestException(HttpStatusCode.BadRequest, $"Error occured deserializing data from RestCountries API: {ex.Message}");
                }      
            }    
            return new List<RestCountriesResponse>();      
        }
    }
}
