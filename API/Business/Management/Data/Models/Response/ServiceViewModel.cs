using Newtonsoft.Json;

namespace CRM.API.Business.Management.Data.Models.Response
{
    public class ServiceViewModel
    {
        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("duration")] public int Duration { get; set; }

        [JsonProperty("price")] public decimal Price { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("description")] public string Description { get; set; }

        [JsonProperty("categories")] public List<ServiceCategoryViewModel> Categories { get; set; }

        [JsonProperty("sub_categories")] public List<ServiceSubCategoryViewModel> SubCategories { get; set; }
    }

    public class ServiceCategoryViewModel
    {
        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("description")] public string Description { get; set; }
    }

    public class ServiceSubCategoryViewModel
    {
        [JsonProperty("id")] public int Id { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("description")] public string Description { get; set; }
    }

}