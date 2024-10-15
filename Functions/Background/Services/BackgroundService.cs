using CRM.Common.Core.Enums;
using CRM.Functions.Background.Data.Database;
using CRM.Functions.Background.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.Functions.Background.Services
{
    public class BackgroundService(BackgroundContext dbContext)
        : IBackgroundService
    {

        public async Task ExecuteAppointmentTask()
        {
            var now = DateTime.Now;

            await dbContext.Appointments
                .Where(a => a.StartDate <= now && a.Status.Id == (int)AppointmentStatus.SCHEDULED)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(a => a.Status.Id, (int)AppointmentStatus.COMPLETED)
                );
        }
    }
}