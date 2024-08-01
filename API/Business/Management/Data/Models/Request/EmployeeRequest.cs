using FluentValidation;
using Newtonsoft.Json;

namespace CRM.API.Business.Management.Data.Models.Request
{
    public class EmployeeRequest
    {
        [JsonProperty("id")]
        public int Id {get; set; }
        [JsonProperty("nick-name")]
        public string NickName {get; set; }
        [JsonProperty("first-name")]
        public string FirstName {get; set; }
        [JsonProperty("last-name")]
        public string LastName {get; set; }
        [JsonProperty("e-mail")]
        public string Email {get; set; }
        [JsonProperty("phone")]
        public string Phone {get; set; }
    }

    public class EmployeeRequestValidator : AbstractValidator<EmployeeRequest>
    {
        public EmployeeRequestValidator()
        {
            RuleFor(x => x.NickName.Trim())
                .NotEmpty()
                .WithMessage("NickName is required")
                .MaximumLength(255)
                .WithMessage("NickName must be less than 255 characters");
            RuleFor(x => x.NickName.Trim())
                .NotEmpty()
                .WithMessage("FirstName is required")
                .MaximumLength(255)
                .WithMessage("FirstName must be less than 255 characters");
            RuleFor(x => x.LastName.Trim())
                .NotEmpty()
                .WithMessage("LastName is required")
                .MaximumLength(255)
                .WithMessage("LastName must be less than 255 characters");
            RuleFor(x => x.Email.Trim())
                .NotEmpty()
                .WithMessage("Email is required")
                .MaximumLength(255)
                .WithMessage("Email must be less than 255 characters");
            RuleFor(x => x.Phone.Trim())
                .NotEmpty()
                .WithMessage("Phone is required")
                .MaximumLength(25)
                .WithMessage("Phone must be less than 25 characters");
        }
    }
}