using Newtonsoft.Json;

namespace CRM.API.Business.Identity.Data.Models
{
    public class Business
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("oid")]
        public string OID { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}