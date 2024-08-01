using FluentValidation;
using Newtonsoft.Json;

namespace CRM.API.Client.Appointment.Data.Models.Request
{
    public class UpdateAppointmentRequest
    {
        [JsonProperty("appointment-id")] public int AppointmentId { get; set; }
        [JsonProperty("start-date")] public DateTime StartDate { get; set; }
        [JsonProperty("note")] public string? Note { get; set; }
    }

    public class UpdateAppointmentRequestValidator : AbstractValidator<UpdateAppointmentRequest>
    {
        public UpdateAppointmentRequestValidator()
        {
            RuleFor(x => x.AppointmentId)
                .NotNull();
            RuleFor(x => x.StartDate)
                .NotNull()
                .GreaterThan(DateTime.UtcNow).WithMessage("Start date must be greater than the current UTC date and time.");
        }
    }
}