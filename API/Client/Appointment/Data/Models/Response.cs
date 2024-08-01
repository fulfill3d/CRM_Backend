using Newtonsoft.Json;

namespace CRM.API.Client.Appointment.Data.Models
{
    public class Response
    {
        [JsonProperty("value")] public string Value { get; set; }
    }
}