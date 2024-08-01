using Newtonsoft.Json;

namespace CRM.API.Business.Management.Data.Models.Response
{
    public class StoreLocationViewModel
    {
        [JsonProperty("location-id")]
        public int LocationId {get; set; }
        [JsonProperty("address-id")]
        public int AddressId {get; set; }
        [JsonProperty("created-at")]
        public DateTime CreatedAt {get; set; }
        [JsonProperty("updated-at")]
        public DateTime UpdatedAt {get; set; }
        [JsonProperty("location-name")]
        public string LocationName {get; set; }
        [JsonProperty("latitude")]
        public decimal Latitude {get; set; }
        [JsonProperty("longitude")]
        public decimal Longitude {get; set; }
        [JsonProperty("address-first-name")]
        public string AddressFirstName {get; set; }
        [JsonProperty("address-last-name")]
        public string AddressLastName {get; set; }
        [JsonProperty("street1")]
        public string Street1 {get; set; }
        [JsonProperty("street2")]
        public string? Street2 {get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("zip-code")]
        public string ZipCode { get; set; }
    }
}