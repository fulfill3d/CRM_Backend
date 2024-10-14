using FluentValidation;
using Newtonsoft.Json;

namespace CRM.API.Business.Store.Data.Models
{
    public class StoreRequest
    {
        [JsonProperty("id")] public int? Id { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("address")] public AddressRequest Address { get; set; }
    }

    public class StoreRequestValidator : AbstractValidator<StoreRequest>
    {
        public StoreRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name must not be empty");
            RuleFor(x => x.Address)
                .NotEmpty()
                .WithMessage("Address must not be empty");
        }
        
    }
}