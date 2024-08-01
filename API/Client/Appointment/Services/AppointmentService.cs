using CRM.API.Client.Appointment.Data.Database;
using CRM.API.Client.Appointment.Data.Models.Request;
using CRM.API.Client.Appointment.Data.Models.Response;
using Microsoft.Extensions.Logging;
using CRM.API.Client.Appointment.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Client.Appointment.Services
{
    public class AppointmentService(
        ILogger<AppointmentService> logger, 
        AppointmentContext dbContext)
        : IAppointmentService
    {
        public async Task<List<AppointmentViewModel>> GetAppointments(int clientId)
        {
            var appointments = await dbContext.Appointments
                .Include(a => a.Client)
                .Include(a => a.StoreService)
                .Include(a => a.StoreLocation)
                .Include(a => a.StoreLocation.Address)
                .Where(a => a.IsEnabled && a.ClientId == clientId)
                .Select(a => new AppointmentViewModel
                {
                    AppointmentId = a.Id,
                    AppointmentNotes = a.Notes,
                    AppointmentStartDate = a.StartDate,
                    AppointmentStatus = a.StatusId,
                    AppointmentAddress = new AddressViewModel
                    {
                        Lon = a.StoreLocation.Location.Coordinate.X,
                        Lat = a.StoreLocation.Location.Coordinate.Y,
                        Street1 = a.StoreLocation.Address.Street1,
                        Street2 = a.StoreLocation.Address.Street2,
                        City = a.StoreLocation.Address.City,
                        State = a.StoreLocation.Address.State,
                        Country = a.StoreLocation.Address.Country,
                        ZipCode = a.StoreLocation.Address.ZipCode,
                    },
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
        public async Task<bool> SetNewAppointment(NewAppointmentRequest request, int clientId)
        {
            // TODO PAYMENT
            // TODO CHECK AVAILABILITY
            // TODO GET AVAILABLE EMPLOYEE
            
            var appointment = await dbContext.StoreServices
                .Include(ss => ss.Store)
                .Include(ss => ss.Store.StoreEmployees)
                .Include(ss => ss.Store.StoreLocations)
                .Where(ss =>
                    ss.IsEnabled &&
                    ss.Id == request.StoreServiceId)
                .Select(ss => new Common.Database.Data.Appointment
                {
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsEnabled = true,
                    Notes = request.Note,
                    StartDate = request.StartDate,
                    StatusId = (int)CRM.Common.Core.Enums.AppointmentStatus.SCHEDULED,
                    ClientId = clientId,
                    StoreServiceId = request.StoreServiceId,
                    StoreLocationId = ss.Store.StoreLocations.Select(sl => sl.Id).FirstOrDefault(),
                    StoreEmployeeId = ss.Store.StoreEmployees.Select(se => se.Id).FirstOrDefault(),
                    StoreId = ss.Store.Id
                }).FirstOrDefaultAsync();
            
            if (appointment == null) return false; // Service Not Found
            
            await dbContext.Appointments.AddAsync(appointment);
            
            // TODO RE-SET STORE AVAILABILITY COSMOS
            
            await dbContext.SaveChangesAsync();
            
            return true;
        }

        public async Task<bool> UpdateAppointment(UpdateAppointmentRequest request, int clientId)
        {
            var appointment = await dbContext.Appointments
                .Include(a => a.Client)
                .Where(a =>
                    a.IsEnabled &&
                    a.Id == request.AppointmentId &&
                    a.Client.Id == clientId)
                .FirstOrDefaultAsync();
            
            if (appointment == null) return false; // Appointment Not Found

            appointment.StartDate = request.StartDate;
            appointment.Notes = request.Note;
            
            // TODO RE-SET STORE AVAILABILITY

            await dbContext.SaveChangesAsync();
            
            return true;
        }

        public async Task<bool> CancelAppointment(int appointmentId, int clientId)
        {
            var appointment = await dbContext.Appointments
                .Include(a => a.Client)
                .Where(a =>
                    a.IsEnabled &&
                    a.Id == appointmentId &&
                    a.Client.Id == clientId)
                .FirstOrDefaultAsync();
            
            if (appointment == null) return false; // Appointment Not Found
            
            appointment.StatusId = (int)CRM.Common.Core.Enums.AppointmentStatus.CANCELED;
            
            // TODO RE-SET STORE AVAILABILITY

            await dbContext.SaveChangesAsync();
            
            return true;
        }
    }
}