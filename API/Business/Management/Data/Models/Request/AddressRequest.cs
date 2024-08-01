using FluentValidation;
using Newtonsoft.Json;

namespace CRM.API.Business.Management.Data.Models.Request
{
    public class AddressRequest
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("location-name")]
        public string LocationName { get; set; }
        [JsonProperty("first-name")]
        public string FirstName { get; set; }
        [JsonProperty("last-name")]
        public string LastName { get; set; }
        [JsonProperty("street1")]
        public string Street1 { get; set; }
        [JsonProperty("street2")]
        public string? Street2 { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
        [JsonProperty("zip-code")]
        public string ZipCode { get; set; }
    }

    public class AddressRequestValidator : AbstractValidator<AddressRequest>
    {
        public AddressRequestValidator()
        {
            RuleFor(x => x.FirstName.Trim())
                .NotEmpty()
                .WithMessage("FirstName is required")
                .MaximumLength(255)
                .WithMessage("FirstName must be less than 255 characters");

            RuleFor(x => x.LastName.Trim())
                .NotEmpty()
                .WithMessage("LastName is required")
                .MaximumLength(255)
                .WithMessage("LastName must be less than 255 characters");
        
            RuleFor(x => x.Street1.Trim())
                .NotEmpty()
                .WithMessage("Street1 is required");

            RuleFor(x => x.City.Trim())
                .NotEmpty()
                .WithMessage("City is required");

            RuleFor(x => x.State.Trim())
                .NotEmpty()
                .WithMessage("State is required")
                .Length(2)
                .WithMessage("State must be 2 characters");

            RuleFor(x => x.Country.Trim())
                .NotEmpty()
                .WithMessage("Country is required")
                .Length(2)
                .WithMessage("Country must be 2 characters");

            RuleFor(x => x.ZipCode.Trim())
                .NotEmpty()
                .WithMessage("ZipCode is required");
        }
    }
}