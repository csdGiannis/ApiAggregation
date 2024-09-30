# Project Title

API Aggregation Service

### Description

This API Aggregation Service is designed to simplify data access by aggregating information from multiple external APIs
into a single, unified endpoint. This service enhances the efficiency of data retrieval, allowing users to access various data sources 
without the need to interact with each API individually. Additionally, techniques such as asynchronous programming, caching, parallelism, circuit breaker, 
mapping, cancellation token, dependency injection, have been implemented to improve reliability and resilience.

The external APIs utilized in this service include the following: REST Countries API which provides country data,
News API which provides article data and OpenLibrary API which provides book data. 

The functionality of this API is to provide basic information, articles and books related to countries. For example, by inputting "Greece" and "Italy", 
the user has the ability to find articles and books that associate with both of them. The user also has the ability to use keywords for more specific results, 
filter, sort and paginate the results.The country names work as the logical AND between them, while the keywords as the logical OR.

### Notices

**User Input Validation**: In order to distinguish country inputs from keyword inputs, a local immutable array of country names is used for validating at each request.
However, in a real-world scenario, a frequently updated database with formal country names would be preferred to ensure the data is always current and reliable.

**Parallelism**: Parallelism is implemented during the asychronous requests to the three external APIs and when updating shared metrics while multiple threads are running,
utilizing Interlocked for accurate incrementation (thread-safe).

**Caching**: Caching is utilized by storing country data locally each time a request is made, enhancing retrieval efficiency. For demo purposes caching for 5 minutes although country data 
is static for a long period of time.

**Metrics**: Implemented functionality to return the total number of requests and average response time for each API, grouped into performance buckets, 
featuring two endpoints: one for viewing results and another for resetting metrics. The metrics update each time a request is made.

**Access Modifiers**: In order to perform tests, internal accessibilities were given to the relative test projects (configured at proj files).

**Media Type**: Please request JSON/XML format or error 406 will occur.

### Documentation

- Utilized OpenAPI Swagger to efficiently document and test the endpoints
- Added frequent and useful comments throughout the code to enhance clarity and maintainability

### Setup Requirements

- .Net 8.0 or higher
- Visual Studio minimum edition 17.8

### Installation

- News API: API key is required at appsetings.json to access their endpoints. (contact me if needed. In previous commits demo keys were used and are to be revoked, in real-world scenario this information must be always hidden.)

### Nuget packages installed

- Microsoft.Extensions.Caching.Memory //used for caching
- Microsoft.Extensions.Logging.Abstractions //used to log from other projects
- Microsoft.Extensions.DependencyInjection.Abstractions //provides abstractions for dependency injection 
- Microsoft.Extensions.Configuration.Abstractions //provides abstractions for configuration 
- Microsoft.Extensions.Http //to use HttpClient factory for external api requests
- Newtonsoft.Json //Serialization and allow xml output convertion
- Serilog.AspNetCore //used for console and file logging
- Serilog.Sinks.Console
- Serilog.Sinks.File
- FluentValidation.AspNetCore //used for controller validation
- Polly //used implement circuit breaker