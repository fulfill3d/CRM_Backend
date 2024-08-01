using Newtonsoft.Json;

namespace CRM.API.Client.Appointment.Data.Models.Response
{
    public class AppointmentViewModel
    {
        [JsonProperty("appointment-id")] public int AppointmentId { get; set; }
        
        [JsonProperty("appointment-notes")] public string? AppointmentNotes { get; set; }
        
        [JsonProperty("appointment-start-date")] public DateTime AppointmentStartDate { get; set; }
        
        [JsonProperty("appointment-status")] public int AppointmentStatus { get; set; }
        
        [JsonProperty("appointment-address")] public AddressViewModel AppointmentAddress { get; set; }
        
        [JsonProperty("appointment-service")] public ServiceViewModel AppointmentService { get; set; }
    }

    public class AddressViewModel
    {
        [JsonProperty("location-lat")] public double Lat { get; set; }
        
        [JsonProperty("location-lon")] public double Lon { get; set; }

        [JsonProperty("address-street1")] public string Street1 { get; set; }

        [JsonProperty("address-street2")] public string? Street2 { get; set; }

        [JsonProperty("address-city")] public string City { get; set; }

        [JsonProperty("address-state")] public string State { get; set; }

        [JsonProperty("address-country")] public string Country { get; set; }

        [JsonProperty("address-zip-code")] public string ZipCode { get; set; }
    }

    public class ServiceViewModel
    {
        [JsonProperty("service-id")] public int ServiceId { get; set; }

        [JsonProperty("service-duration")] public int ServiceDuration { get; set; }

        [JsonProperty("service-price")] public decimal ServicePrice { get; set; }

        [JsonProperty("service-currency")] public string ServiceCurrency { get; set; } = "USD";

        [JsonProperty("service-name")] public string ServiceName { get; set; }

        [JsonProperty("service-description")] public string ServiceDescription { get; set; }
    }
}