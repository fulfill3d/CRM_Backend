using CRM.API.Business.Identity.Services.Interfaces;
using CRM.API.Business.Identity.Services.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace CRM.API.Business.Identity.Services
{
    public class TokenService(
        IHttpClientFactory httpClientFactory, 
        IOptions<TokenServiceOption> opt): ITokenService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
        private readonly string _b2CTokenEndpoint = opt.Value.TokenEndpoint;
        private readonly string _b2CSignInUpPolicy = opt.Value.SignInUpPolicy;
        private readonly string _b2CUpdatePolicy = opt.Value.UpdatePolicy;
        private readonly string _b2CClientId = opt.Value.ClientId;
        private readonly string _b2CClientSecret = opt.Value.ClientSecret;
        private readonly string _b2CPostRegisterRedirectUri = opt.Value.B2CPostRegisterRedirectUri;
        private readonly string _b2CPostUpdateRedirectUri = opt.Value.B2CPostUpdateRedirectUri;
        private readonly string _b2CGrantType = opt.Value.GrantType;
        private readonly string _b2CScope = opt.Value.Scope;
        
        public async Task<JObject?> ExchangeCodeForTokenAsync(string code, bool update = false)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var policy = update ? _b2CUpdatePolicy.ToLower() : _b2CSignInUpPolicy.ToLower();
            var redirectUri = update ? _b2CPostUpdateRedirectUri : _b2CPostRegisterRedirectUri;
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_b2CTokenEndpoint}?p={policy}");
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", _b2CClientId),
                new KeyValuePair<string, string>("client_secret", _b2CClientSecret),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("grant_type", _b2CGrantType),
                new KeyValuePair<string, string>("scope", _b2CScope)
            });
            request.Content = content;

            var response = await httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var responseString = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseString);
        }
    }
}