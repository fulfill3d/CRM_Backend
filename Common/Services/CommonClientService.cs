using CRM.Common.Database.Data;
using CRM.Common.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRM.Common.Services
{
    public class CommonClientService(DatabaseContext dbContext): ICommonClientService
    {
        public async Task<int> GetClientIdByRefId(string refId)
        {
            return await dbContext.Clients
                .Where(c =>
                    c.IsEnabled &&
                    c.RefId == Guid.Parse(refId))
                .Select(c => c.Id)
                .FirstOrDefaultAsync();
        }
    }
}