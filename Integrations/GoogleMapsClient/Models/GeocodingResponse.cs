using Newtonsoft.Json;

namespace CRM.Integrations.GoogleMapsClient.Models
{

    public class GeocodingResponse
    {
        [JsonProperty("results")] public List<Result> Results { get; set; }

        [JsonProperty("status")] public string Status { get; set; }

        public class Result
        {
            [JsonProperty("address_components")] public List<AddressComponent> AddressComponents { get; set; }

            [JsonProperty("formatted_address")] public string FormattedAddress { get; set; }

            [JsonProperty("geometry")] public Geometry Geometry { get; set; }

            [JsonProperty("place_id")] public string PlaceId { get; set; }

            [JsonProperty("types")] public List<string> Types { get; set; }
        }

        public class AddressComponent
        {
            [JsonProperty("long_name")] public string LongName { get; set; }

            [JsonProperty("short_name")] public string ShortName { get; set; }

            [JsonProperty("types")] public List<string> Types { get; set; }
        }

        public class Geometry
        {
            [JsonProperty("bounds")] public Bounds Bounds { get; set; }

            [JsonProperty("location")] public Location Location { get; set; }

            [JsonProperty("location_type")] public string LocationType { get; set; }

            [JsonProperty("viewport")] public Viewport Viewport { get; set; }
        }

        public class Bounds
        {
            [JsonProperty("northeast")] public Location Northeast { get; set; }

            [JsonProperty("southwest")] public Location Southwest { get; set; }
        }

        public class Viewport
        {
            [JsonProperty("northeast")] public Location Northeast { get; set; }

            [JsonProperty("southwest")] public Location Southwest { get; set; }
        }
    }
}
