# Project Title

API Aggregation Service

## Description

This API Aggregation Service is designed to simplify data access by consolidating information from multiple external APIs
into a single, unified endpoint. This service enhances the efficiency of data retrieval, allowing users to access various data sources 
without the need to interact with each API individually. Additionally, techniques such as asynchronous programming, parallelism, circuit breaker, 
mapping, cancellation token have been implemented to improve reliability and resilience.

The external APIs utilized in this service include the following: REST Countries API which provides country data,
News API which provides article data and OpenLibrary API which provides book data. 

The purpose of this API is to provide basic information, articles and books related to countries. For example, by inputting "Greece" and "Italy", 
the user has the ability to find articles and books that associate with both of them. The user also has the ability to use keywords for more specific results, 
filter, sort and paginate the results.
In the current implementation, a local immutable array is used for validating the country inputs at the beggining to avoid reliance on external APIs, 
removing invalid inputs even if external services are down do implement this validation. However, in a real-world scenario, 
a frequently updated database with formal country names would be used to ensure the data is always current and reliable.

### Setup Requirements

- .Net 8.0 or higher
- Visual Studio minimum edition 17.8

### Installation

- News API - API key required at appsetings.json to access this endpoint. (contact me via email for more information)