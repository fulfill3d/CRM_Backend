using CRM.Integrations.GoogleMapsClient.Interfaces;
using CRM.Integrations.GoogleMapsClient.Models;
using CRM.Integrations.GoogleMapsClient.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace CRM.Integrations.GoogleMapsClient
{
    public class GoogleMapsClient(IOptions<GoogleMapsOptions> options, ILogger<GoogleMapsClient> logger): RestClient, IGoogleMapsClient
    {
        private const int MaxRetryCount = 2;
        private readonly GoogleMapsOptions _options = options.Value;
        private readonly RestClient _client = new (new RestClientOptions
        {
            BaseUrl = new Uri("https://maps.googleapis.com/maps/api/geocode/json"),
            Timeout = TimeSpan.FromSeconds(30)
        });

        public async Task<Location?> GetCoordinatesAsync(GeocodingRequest request)
        {
            var restRequest = new RestRequest();
            restRequest.AddQueryParameter("address", request.Address);
            restRequest.AddQueryParameter("key", _options.ApiKey);

            var response = await ExecuteWithPolicyAsync<GeocodingResponse>(restRequest);
            var content = response.Content;
            if (content == null)
            {
                return null;
            }
            var geocodingResponse = JsonConvert.DeserializeObject<GeocodingResponse>(content);

            if (geocodingResponse?.Results != null && geocodingResponse.Results.Any())
            {
                var location = geocodingResponse.Results.First().Geometry.Location;
                logger.LogInformation($"Manually deserialized Location: Latitude {location.Latitude}, Longitude {location.Longitude}");
                return location;
            }
            logger.LogError("Failed to deserialize the response content or no results found.");

            return null;
        }

        private async Task<RestResponse<T>> ExecuteWithPolicyAsync<T>(RestRequest request)
        {
            var retryCount = 0;
            RestResponse<T> response;

            do
            {
                response = await _client.ExecuteAsync<T>(request);

                if (response.IsSuccessful)
                {
                    return response;
                }

                logger.LogError($"Google Maps API request failed: {response.StatusCode}");
                logger.LogError(response.Content);

                retryCount++;

                if (retryCount >= MaxRetryCount)
                {
                    break;
                }

                await HandleRetry(response.StatusCode);
            }
            while (true);

            return response;
        }

        private async Task HandleRetry(System.Net.HttpStatusCode statusCode)
        {
            switch (statusCode)
            {
                case System.Net.HttpStatusCode.TooManyRequests:
                    logger.LogInformation("Too many requests. Retrying in 2 seconds...");
                    await Task.Delay(2000);
                    break;
                case System.Net.HttpStatusCode.BadGateway:
                    logger.LogInformation("Bad Gateway error. Retrying in 5 seconds...");
                    await Task.Delay(5000);
                    break;
                default:
                    await Task.CompletedTask;
                    break;
            }
        }
    }
}