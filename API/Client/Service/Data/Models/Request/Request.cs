using FluentValidation;
using Newtonsoft.Json;

namespace CRM.API.Client.Service.Data.Models.Request
{
    public class Request
    {
        [JsonProperty("value")] public string Value { get; set; }
    }

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Value.Trim())
                .NotEmpty()
                .WithMessage("Value is required")
                .MaximumLength(255)
                .WithMessage("Value must be less than 255 characters");
        }
    }
}