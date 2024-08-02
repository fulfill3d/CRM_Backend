namespace CRM.Common.Services.Interfaces
{
    public interface ICommonClientService
    {
        Task<int> GetClientIdByRefId(string refId);
    }
}