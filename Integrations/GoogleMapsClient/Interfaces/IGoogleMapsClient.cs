using CRM.Integrations.GoogleMapsClient.Models;

namespace CRM.Integrations.GoogleMapsClient.Interfaces
{
    public interface IGoogleMapsClient
    {
        Task<Location?> GetCoordinatesAsync(GeocodingRequest request);
    }
}