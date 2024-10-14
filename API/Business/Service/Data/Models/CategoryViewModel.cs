using Newtonsoft.Json;

namespace CRM.API.Business.Service.Data.Models
{
    public class CategoryViewModel
    {
        [JsonProperty("categories")] public IEnumerable<CategoriesViewModel> Categories { get; set; }
        [JsonProperty("sub_categories")] public IEnumerable<SubCategoriesViewModel> SubCategories { get; set; }
    }

    public class CategoriesViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class SubCategoriesViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}