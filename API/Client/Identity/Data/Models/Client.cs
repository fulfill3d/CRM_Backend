using Newtonsoft.Json;

namespace CRM.API.Client.Identity.Data.Models
{
    public class Client
    {
        [JsonProperty("first-name")]
        public string FirstName { get; set; }
        
        [JsonProperty("last-name")]
        public string LastName { get; set; }
        
        [JsonProperty("oid")]
        public string OID { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}