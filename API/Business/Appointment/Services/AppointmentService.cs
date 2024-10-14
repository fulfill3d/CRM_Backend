using CRM.API.Business.Appointment.Data.Database;
using CRM.API.Business.Appointment.Data.Models;
using CRM.API.Business.Appointment.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Business.Appointment.Services
{
    public class AppointmentService(AppointmentContext dbContext): IAppointmentService
    {
        public async Task<List<AppointmentViewModel>> GetAll(string businessRefId, int storeId)
        {
            var appointments = await dbContext.Appointments
                .Include(a => a.Store)
                .Include(a => a.StoreService)
                .Where(a => 
                    a.IsEnabled && 
                    a.StoreId == storeId && 
                    a.Store.BusinessRefId == Guid.Parse(businessRefId))
                .Select(a => new AppointmentViewModel
                {
                    AppointmentId = a.Id,
                    AppointmentNotes = a.Notes,
                    AppointmentStartDate = a.StartDate,
                    AppointmentStatus = a.StatusId,
                    AppointmentService = new ServiceViewModel
                    {
                        ServiceId = a.StoreService.Id,
                        ServiceDuration = a.StoreService.Duration ?? 0,
                        ServicePrice = a.StoreService.Price ?? 0,
                        ServiceName = a.StoreService.Name,
                        ServiceDescription = a.StoreService.Description,
                    }
                }).ToListAsync();
            
            return appointments;
        }

        public async Task<bool> Cancel(string businessRefId, int appointmentId)
        {
            var appointment = await dbContext.Appointments
                .Include(a => a.Store)
                .Where(a =>
                    a.IsEnabled &&
                    a.Id == appointmentId &&
                    a.Store.BusinessRefId == Guid.Parse(businessRefId))
                .FirstOrDefaultAsync();
            
            if (appointment == null) return false; // Appointment Not Found
            
            appointment.StatusId = (int)CRM.Common.Core.Enums.AppointmentStatus.CANCELED;
            
            // TODO RE-SET STORE AVAILABILITY

            await dbContext.SaveChangesAsync();
            
            return true;
        }
    }
}