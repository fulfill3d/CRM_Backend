namespace CRM.API.Client.Identity.Services.Interfaces
{
    public interface IIdentityService
    {
        Task<bool> VerifyAndProcess(string code, bool update = false);
    }
}