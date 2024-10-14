using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using CRM.Common.Database.Data;
using CRM.API.Business.Identity.Data.Database;
using CRM.API.Business.Identity.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Business.Identity.Services
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
            
            var business = DecodeIdToken(idToken);

            if (update)
            {
                await UpdateNewBusiness(business);
            }
            else
            {
                await CreateNewBusiness(business);
                await CreateNewClient(business); // SEE EXPLANATION below on the METHOD on WHY THIS is CALLED
            }

            return true;
        }
        
        // PRIVATE METHODS

        private CRM.API.Business.Identity.Data.Models.Business DecodeIdToken(string idToken)
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

            return new CRM.API.Business.Identity.Data.Models.Business
            {
                Name = payload["name"].ToString() ?? string.Empty,
                OID = payload["oid"].ToString() ?? string.Empty,
                Email = firstEmail,
            };
        }


        private async Task CreateNewBusiness(CRM.API.Business.Identity.Data.Models.Business business)
        {
            var doesBusinessExists = await dbContext.Businesses
                .AnyAsync(b => b.IsEnabled == true && b.RefId == Guid.Parse(business.OID));

            if (doesBusinessExists)
            {
                return;
            }

            await dbContext.Businesses.AddAsync(new Common.Database.Data.Business
            {
                RefId = Guid.Parse(business.OID),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsEnabled = true,
                Name = business.Name,
                Email = business.Email,
            });

            await dbContext.SaveChangesAsync();
            
            // TODO SendGrid / Send Welcome Email
        }

        private async Task UpdateNewBusiness(CRM.API.Business.Identity.Data.Models.Business business)
        {
            var existingBusiness = await dbContext.Businesses
                .FirstOrDefaultAsync(b => b.IsEnabled == true && b.RefId == Guid.Parse(business.OID));

            if (existingBusiness == null)
            {
                return;
            }
            
            existingBusiness.Name = business.Name;
            existingBusiness.Email = business.Email;

            await dbContext.SaveChangesAsync();
            
            // TODO SendGrid / Send Updated Email
        }
        
        // THIS IS IMPLEMENTED SINCE WE HAVE SINGLE B2C and SINGLE FRONTEND for BUSINESS and CLIENT
        // ONCE THE USER LOGIN in SINGLE DEMO APP, both CLIENT and BUSINESS ENTITIES will be CREATED
        // THEREFORE, the USER would both test BUSINESS and CLIENT in DEMO APP
        private async Task CreateNewClient(CRM.API.Business.Identity.Data.Models.Business business)
        {
            var doesClientExists = await dbContext.Clients
                .AnyAsync(b => b.IsEnabled == true && b.RefId == Guid.Parse(business.OID));

            if (doesClientExists)
            {
                return;
            }

            await dbContext.Clients.AddAsync(new Common.Database.Data.Client
            {
                RefId = Guid.Parse(business.OID),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsEnabled = true,
                FirstName = business.Name,
                LastName = business.Name,
                Email = business.Email,
                Phone = string.Empty,
            });

            await dbContext.SaveChangesAsync();
            
        }
    }
}