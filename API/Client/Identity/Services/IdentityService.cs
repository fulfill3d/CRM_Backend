using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using CRM.API.Client.Identity.Data.Database;
using CRM.API.Client.Identity.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CRM.API.Client.Identity.Services
{
    public class IdentityService(
        ILogger<IdentityService> logger,
        ITokenService tokenService,
        IdentityContext dbContext) : IIdentityService
    {

        public async Task<bool> VerifyAndProcess(string code, bool update = false)
        {
            var jObject = update
                ? await tokenService.ExchangeCodeForTokenAsync(code, true)
                : await tokenService.ExchangeCodeForTokenAsync(code);
            
            if (jObject == null)
            {
                return false;
            }

            var idToken = jObject.Value<string>("id_token");
            
            if (string.IsNullOrEmpty(idToken))
            {
                return false;
            }
            
            var client = DecodeIdToken(idToken);

            if (update)
            {
                await UpdateClient(client);
            }
            else
            {
                await CreateNewClient(client);
            }

            return true;
        }
        
        // PRIVATE METHODS

        private CRM.API.Client.Identity.Data.Models.Client DecodeIdToken(string idToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(idToken) as JwtSecurityToken;

            if (jsonToken == null)
            {
                throw new ArgumentException("Invalid token.");
            }

            var payload = jsonToken.Payload;

            var emailsJson = payload["emails"].ToString();
            string firstEmail = string.Empty;

            if (!string.IsNullOrEmpty(emailsJson))
            {
                try
                {
                    var emailsArray = JsonSerializer.Deserialize<string[]>(emailsJson);
                    firstEmail = emailsArray?.FirstOrDefault() ?? string.Empty;
                }
                catch (JsonException)
                {
                    throw new ArgumentException("Invalid token.");
                }
            }

            return new CRM.API.Client.Identity.Data.Models.Client
            {
                FirstName = payload["name"].ToString() ?? string.Empty,
                LastName = payload["name"].ToString() ?? string.Empty,
                OID = payload["oid"].ToString() ?? string.Empty,
                Email = firstEmail,
            };
        }


        private async Task CreateNewClient(CRM.API.Client.Identity.Data.Models.Client client)
        {
            var doesClientExists = await dbContext.Clients
                .AnyAsync(b => b.IsEnabled == true && b.RefId == Guid.Parse(client.OID));

            if (doesClientExists)
            {
                return;
            }

            await dbContext.Clients.AddAsync(new Common.Database.Data.Client
            {
                RefId = Guid.Parse(client.OID),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsEnabled = true,
                FirstName = client.FirstName,
                LastName = client.LastName,
                Email = client.Email,
                Phone = string.Empty,
            });

            await dbContext.SaveChangesAsync();
            
            // TODO SendGrid / Send Welcome Email
        }

        private async Task UpdateClient(CRM.API.Client.Identity.Data.Models.Client client)
        {
            var existingClient = await dbContext.Clients
                .FirstOrDefaultAsync(b => b.IsEnabled == true && b.RefId == Guid.Parse(client.OID));

            if (existingClient == null)
            {
                return;
            }
            
            existingClient.FirstName = client.FirstName;
            existingClient.LastName = client.LastName;
            existingClient.Email = client.Email;

            await dbContext.SaveChangesAsync();
            
            // TODO SendGrid / Send Updated Email
        }
    }
}