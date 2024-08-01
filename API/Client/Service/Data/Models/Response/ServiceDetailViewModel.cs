using Newtonsoft.Json;

namespace CRM.API.Client.Service.Data.Models.Response
{
    public class ServiceDetailViewModel
    {
        [JsonProperty("service-id")] public int ServiceId { get; set; }

        [JsonProperty("service-duration")] public int ServiceDuration { get; set; }

        [JsonProperty("service-price")] public decimal ServicePrice { get; set; }

        [JsonProperty("service-name")] public string ServiceName { get; set; }

        [JsonProperty("service-description")] public string ServiceDescription { get; set; }

        [JsonProperty("store-id")] public int StoreId { get; set; }
        
        [JsonProperty("store-name")] public string StoreName { get; set; }

        [JsonProperty("store-description")] public string StoreDescription { get; set; }
        
        [JsonProperty("location-lat")] public double Lat { get; set; }
        
        [JsonProperty("location-lon")] public double Lon { get; set; }

        [JsonProperty("address-street1")] public string Street1 { get; set; }

        [JsonProperty("address-street2")] public string? Street2 { get; set; }

        [JsonProperty("address-city")] public string City { get; set; }

        [JsonProperty("address-state")] public string State { get; set; }

        [JsonProperty("address-country")] public string Country { get; set; }

        [JsonProperty("address-zip-code")] public string ZipCode { get; set; }
        
        [JsonProperty("store-employees")] public List<StoreEmployeeViewModel> StoreEmployees { get; set; }

    }
    
    public class StoreEmployeeViewModel
    {
        [JsonProperty("employee-id")] public int Id {get; set; }
        
        [JsonProperty("employee-nick-name")] public string NickName {get; set; }
    }
}