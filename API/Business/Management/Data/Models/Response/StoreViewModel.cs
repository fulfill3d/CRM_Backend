using Newtonsoft.Json;

namespace CRM.API.Business.Management.Data.Models.Response
{
    public class StoreViewModel
    {
        [JsonProperty("id")] public int Id {get; set; }
        [JsonProperty("name")] public string Name {get; set; }
        [JsonProperty("description")] public string Description {get; set; }
        [JsonProperty("created_at")] public DateTime CreatedAt {get; set; }
        [JsonProperty("updated_at")] public DateTime UpdatedAt {get; set; }
        [JsonProperty("location")] public StoreLocationViewModel Location {get; set; }
        [JsonProperty("employees")] public List<StoreEmployeeViewModel> Employees {get; set; }
    }
}