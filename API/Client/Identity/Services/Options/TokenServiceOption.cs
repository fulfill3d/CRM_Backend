namespace CRM.API.Client.Identity.Services.Options
{
    public class TokenServiceOption
    {
        public string B2CPostRegisterRedirectUri { get; set; }
        public string B2CPostUpdateRedirectUri { get; set; }
        public string TokenEndpoint { get; set; }
        public string UpdatePolicy { get; set; }
        public string SignInUpPolicy { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string GrantType { get; set; }
        public string Scope { get; set; }
    }
}