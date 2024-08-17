using Newtonsoft.Json;

namespace CRM.API.Client.Service.Data.Models.Response
{
    public class ServiceDetailViewModel
    {
        [JsonProperty("service_id")] public int ServiceId { get; set; }

        [JsonProperty("service_duration")] public int ServiceDuration { get; set; }

        [JsonProperty("service_price")] public decimal ServicePrice { get; set; }

        [JsonProperty("service_name")] public string ServiceName { get; set; }

        [JsonProperty("service_description")] public string ServiceDescription { get; set; }

        [JsonProperty("store_id")] public int StoreId { get; set; }
        
        [JsonProperty("store_name")] public string StoreName { get; set; }

        [JsonProperty("store_description")] public string StoreDescription { get; set; }
        
        [JsonProperty("location_lat")] public double Lat { get; set; }
        
        [JsonProperty("location_lon")] public double Lon { get; set; }

        [JsonProperty("address_street1")] public string Street1 { get; set; }

        [JsonProperty("address_street2")] public string? Street2 { get; set; }

        [JsonProperty("address_city")] public string City { get; set; }

        [JsonProperty("address_state")] public string State { get; set; }

        [JsonProperty("address_country")] public string Country { get; set; }

        [JsonProperty("address_zip_code")] public string ZipCode { get; set; }
        
        [JsonProperty("store_employees")] public List<StoreEmployeeViewModel> StoreEmployees { get; set; }

    }
    
    public class StoreEmployeeViewModel
    {
        [JsonProperty("employee_id")] public int Id {get; set; }
        
        [JsonProperty("employee_nick_name")] public string NickName {get; set; }
    }
}