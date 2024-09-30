using Newtonsoft.Json;

namespace CRM.API.Business.Management.Data.Models.Response
{
    public class StoreEmployeeViewModel
    {
        [JsonProperty("id")] public int Id {get; set; }
        [JsonProperty("nick_name")] public string NickName {get; set; }
        [JsonProperty("first_name")] public string FirstName {get; set; }
        [JsonProperty("last_name")] public string LastName {get; set; }
        [JsonProperty("e_mail")] public string Email {get; set; }
        [JsonProperty("phone")] public string Phone {get; set; }
        [JsonProperty("created_at")] public DateTime CreatedAt {get; set; }
        [JsonProperty("updated_at")] public DateTime UpdatedAt {get; set; }
    }
}