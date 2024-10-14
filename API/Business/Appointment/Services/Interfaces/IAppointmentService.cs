using CRM.API.Business.Appointment.Data.Models;

namespace CRM.API.Business.Appointment.Services.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<AppointmentViewModel>> GetAll(string businessRefId, int storeId);
        Task<bool> Cancel(string businessRefId, int appointmentId);
    }
}