using Microsoft.Azure.Functions.Worker.Http;

namespace CRM.Common.Services.Interfaces
{
    public interface IJwtValidatorService
    {
        Task<string?> AuthenticateAndAuthorize(HttpRequestData req, string[] acceptedScopes);
    }
}