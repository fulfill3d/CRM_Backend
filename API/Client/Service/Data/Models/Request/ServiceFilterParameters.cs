namespace CRM.API.Client.Service.Data.Models.Request
{
    public class ServiceFilterParameters
    {
        public ServiceFilterParameters()
        {
            CategoryIds = new List<int>();
            SubCategoryIds = new List<int>();
        }

        public string? Query { get; set; }
        public double? Radius { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lon { get; set; }
        public List<int> CategoryIds { get; set; }
        public List<int> SubCategoryIds { get; set; }

        public override string ToString()
        {
            string result = string.Empty;

            if (!string.IsNullOrWhiteSpace(Query))
            {
                result += $"query={Query}";
            }

            if (Radius != null)
            {
                result += $"radius={Radius}";
            }

            if (Lat != null)
            {
                result += $"lat={Lat}";
            }

            if (Lon != null)
            {
                result += $"lon={Lon}";
            }

            if(CategoryIds.Count()>0)
            {
                result += $"&categoryIds={string.Join(",", CategoryIds.OrderBy(i => i).Select(i => i.ToString()))}";
            }

            if (SubCategoryIds.Count > 0)
            {
                result += $"&subcategoryIds={string.Join(",", SubCategoryIds.OrderBy(i => i).Select(i => i.ToString()))}";
            }

            return result;
        }
    }
}