using CRM.API.Business.Store.Data.Models;

namespace CRM.API.Business.Store.Services.Interfaces
{
    public interface IStoreService
    {
        Task<List<StoreViewModel>> GetStores(string businessRefId);
        Task<StoreViewModel?> GetStore(string businessRefId, int storeId);
        Task<bool> AddStore(string businessRefId, StoreRequest request);
        Task<bool> EditStore(string businessRefId, StoreRequest request);
        Task<bool> DeleteStore(string businessRefId, int storeId);
    }
}