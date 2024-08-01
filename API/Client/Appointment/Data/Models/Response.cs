using Newtonsoft.Json;

namespace Appointment.Data.Models
{
    public class Response
    {
        [JsonProperty("value")] public string Value { get; set; }
    }
}