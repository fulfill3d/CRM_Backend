using Newtonsoft.Json;

namespace CRM.API.Client.Appointment.Data.Models.Response
{
    public class AppointmentViewModel
    {
        [JsonProperty("appointment_id")] public int AppointmentId { get; set; }
        
        [JsonProperty("appointment_notes")] public string? AppointmentNotes { get; set; }
        
        [JsonProperty("appointment_start_date")] public DateTime AppointmentStartDate { get; set; }
        
        [JsonProperty("appointment_status")] public int AppointmentStatus { get; set; }
        
        [JsonProperty("appointment_address")] public AddressViewModel AppointmentAddress { get; set; }
        
        [JsonProperty("appointment_service")] public ServiceViewModel AppointmentService { get; set; }
    }

    public class AddressViewModel
    {
        [JsonProperty("location_lat")] public double Lat { get; set; }
        
        [JsonProperty("location_lon")] public double Lon { get; set; }

        [JsonProperty("address_street1")] public string Street1 { get; set; }

        [JsonProperty("address_street2")] public string? Street2 { get; set; }

        [JsonProperty("address_city")] public string City { get; set; }

        [JsonProperty("address_state")] public string State { get; set; }

        [JsonProperty("address_country")] public string Country { get; set; }

        [JsonProperty("address_zip_code")] public string ZipCode { get; set; }
    }

    public class ServiceViewModel
    {
        [JsonProperty("service_id")] public int ServiceId { get; set; }

        [JsonProperty("service_duration")] public int ServiceDuration { get; set; }

        [JsonProperty("service_price")] public decimal ServicePrice { get; set; }

        [JsonProperty("service_currency")] public string ServiceCurrency { get; set; } = "USD";

        [JsonProperty("service_name")] public string ServiceName { get; set; }

        [JsonProperty("service_description")] public string ServiceDescription { get; set; }
    }
}