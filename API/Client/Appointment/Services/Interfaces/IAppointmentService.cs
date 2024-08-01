using CRM.API.Client.Appointment.Data.Models.Request;
using CRM.API.Client.Appointment.Data.Models.Response;

namespace CRM.API.Client.Appointment.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<AppointmentViewModel>> GetAppointments(int clientId);
        Task<bool> SetNewAppointment(NewAppointmentRequest request, int clientId);
        Task<bool> UpdateAppointment(UpdateAppointmentRequest request, int clientId);
        Task<bool> CancelAppointment(int appointmentId, int clientId);
        
    }
}