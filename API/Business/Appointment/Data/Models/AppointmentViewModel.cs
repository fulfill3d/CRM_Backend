using Newtonsoft.Json;

namespace CRM.API.Business.Appointment.Data.Models
{
    public class AppointmentViewModel
    {
        [JsonProperty("appointment_id")] public int AppointmentId { get; set; }
        
        [JsonProperty("appointment_notes")] public string? AppointmentNotes { get; set; }
        
        [JsonProperty("appointment_start_date")] public DateTime AppointmentStartDate { get; set; }
        
        [JsonProperty("appointment_status")] public int AppointmentStatus { get; set; }
        
        [JsonProperty("appointment_service")] public ServiceViewModel AppointmentService { get; set; }
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