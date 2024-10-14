using CRM.API.Business.Service.Data.Models;

namespace CRM.API.Business.Service.Services.Interfaces
{
    public interface IServiceService
    {
        // STORE SERVICES
        Task<CategoryViewModel> GetServiceCategories();
        Task<List<ServiceViewModel>> GetStoreServices(string businessRefId, int storeId);
        Task<bool> AddStoreService(string businessRefId, int storeId, StoreServiceRequest request);
        Task<bool> EditStoreService(string businessRefId, StoreServiceRequest request);
        Task<bool> DeleteStoreService(string businessRefId, int storeServiceId);
    }
}