using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace ApiAggregation.Infrastructure
{
    internal static class HttpExtensions
    {
        public static IHttpClientBuilder AddStantandPolicyHandler(this IHttpClientBuilder builder)
        {
            return builder.AddPolicyHandler(Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .OrResult(x => !x.IsSuccessStatusCode)
                .CircuitBreakerAsync(handledEventsAllowedBeforeBreaking: 3, durationOfBreak: TimeSpan.FromSeconds(15)));
        }
    }
}
