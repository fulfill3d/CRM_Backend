using FluentValidation;
using Newtonsoft.Json;

namespace CRM.API.Client.Appointment.Data.Models.Request
{
    public class NewAppointmentRequest
    {
        [JsonProperty("store-service-id")] public int StoreServiceId { get; set; }
        [JsonProperty("start-date")] public DateTime StartDate { get; set; }
        [JsonProperty("note")] public string? Note { get; set; }
    }

    public class AppointmentRequestValidator : AbstractValidator<NewAppointmentRequest>
    {
        public AppointmentRequestValidator()
        {
            RuleFor(x => x.StoreServiceId)
                .NotNull();
            RuleFor(x => x.StartDate)
                .NotNull()
                .GreaterThan(DateTime.UtcNow).WithMessage("Start date must be greater than the current UTC date and time.");
        }
    }
}