using CRM.API.Client.Service.Data.Models.Response;
using Microsoft.Extensions.Primitives;

namespace CRM.API.Client.Service.Services.Interfaces
{
    public interface IServiceService
    {
        Task<List<ServiceViewModel>> GetServices(Dictionary<string,StringValues> parameters);
        Task<ServiceDetailViewModel?> GetService(int serviceId);
    }
}