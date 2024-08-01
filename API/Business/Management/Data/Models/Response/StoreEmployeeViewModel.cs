using Newtonsoft.Json;

namespace CRM.API.Business.Management.Data.Models.Response
{
    public class StoreEmployeeViewModel
    {
        [JsonProperty("id")] public int Id {get; set; }
        [JsonProperty("nick-name")] public string NickName {get; set; }
        [JsonProperty("first-name")] public string FirstName {get; set; }
        [JsonProperty("last-name")] public string LastName {get; set; }
        [JsonProperty("e-mail")] public string Email {get; set; }
        [JsonProperty("phone")] public string Phone {get; set; }
        [JsonProperty("craeted-at")] public DateTime CreatedAt {get; set; }
        [JsonProperty("updated-at")] public DateTime UpdatedAt {get; set; }
    }
}