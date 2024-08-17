using FluentValidation;
using Newtonsoft.Json;

namespace CRM.API.Business.Management.Data.Models.Request
{
    public class StoreServiceRequest
    {
        [JsonProperty("store_service_id")] public int Id { get; set; }
        [JsonProperty("service_name")] public string Name { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("duration")] public int Duration { get; set; }
        [JsonProperty("price")] public decimal Price { get; set; }
        [JsonProperty("categories")] public IEnumerable<int> Categories { get; set; }
        [JsonProperty("sub_categories")] public IEnumerable<int> SubCategories { get; set; }
    }

    public class StoreServiceRequestValidator : AbstractValidator<StoreServiceRequest>
    {
        public StoreServiceRequestValidator()
        {
            RuleFor(x => x.Name.Trim())
                .NotEmpty()
                .WithMessage("Service name is required");
            RuleFor(x => x.Description.Trim())
                .NotEmpty()
                .WithMessage("Description is required");
            RuleFor(x => x.Duration)
                .GreaterThan(0)
                .WithMessage("Duration must be greater than zero");
            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than zero");
            RuleFor(x => x.Categories)
                .NotEmpty()
                .WithMessage("At least one category is required")
                .Must(categories => categories != null && categories.Any())
                .WithMessage("At least one category must be provided");
            RuleFor(x => x.SubCategories)
                .NotEmpty()
                .WithMessage("At least one sub_category is required")
                .Must(categories => categories != null && categories.Any())
                .WithMessage("At least one sub_category must be provided");
        }
    }
}