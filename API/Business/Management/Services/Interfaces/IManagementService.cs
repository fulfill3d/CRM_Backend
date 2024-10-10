using CRM.API.Business.Management.Data.Models.Request;
using CRM.API.Business.Management.Data.Models.Response;

namespace CRM.API.Business.Management.Services.Interfaces
{
    public interface IManagementService
    {
        // STORE
        Task<List<StoreViewModel>> GetStores(string businessRefId);
        Task<StoreViewModel?> GetStore(string businessRefId, int storeId);
        Task<bool> AddStore(string businessRefId, StoreRequest request);
        Task<bool> EditStore(string businessRefId, StoreRequest request);
        Task<bool> DeleteStore(string businessRefId, int storeId);
        
        // EMPLOYEE
        Task<List<StoreEmployeeViewModel>> GetEmployees(string businessRefId, int storeId);
        Task<bool> AddEmployee(string businessRefId, int storeId, EmployeeRequest request);
        Task<bool> EditEmployee(string businessRefId, EmployeeRequest request);
        Task<bool> DeleteEmployee(string businessRefId, int employeeId);
        
        // STORE SERVICES
        Task<CategoryViewModel> GetServiceCategories();
        Task<List<ServiceViewModel>> GetStoreServices(string businessRefId, int storeId);
        Task<bool> AddStoreService(string businessRefId, int storeId, StoreServiceRequest request);
        Task<bool> EditStoreService(string businessRefId, StoreServiceRequest request);
        Task<bool> DeleteStoreService(string businessRefId, int storeServiceId);
    }
}