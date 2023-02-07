using IdentityServer4.Models;

namespace Server;

public class Config
{
   
    public static IEnumerable<IdentityResource> IdentityResources =>
        new[]
        {
            
            new IdentityResource()
            {
                Name = "role",
                UserClaims =new List<string> {"role" }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new[] { new ApiScope( "PsychToGo.read" ), new ApiScope( "PsychToGo.write" ), };

    public static IEnumerable<ApiResource> ApiResources =>
        new[]
        {
            new ApiResource("PsychToGo")
            {
                Scopes = new List<string> { "PsychToGo.read" , "PsychToGo.write" },
                ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
                UserClaims = new List<string> {"role"}
            }
        };

    public static IEnumerable<Client> Clients =>
        new[]
        {
            new Client()
            {
                ClientId = "m2m.client",
                ClientName = "Client credentiantials client",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = {new Secret("Pa$w0rd".Sha256())},
                AllowedScopes = { "PsychToGo.read", "PsychToGo.write" }
            },

            new Client()
            {
                ClientId = "interactive",               
                AllowedGrantTypes = GrantTypes.Code,
                ClientSecrets = {new Secret("Pa$w0rd".Sha256())},
                AllowedScopes = { "openid","profile", "PsychToGo.write" },
                RedirectUris = {"http://localhost:5279/signin-oidc"},
                FrontChannelLogoutUri =  "http://localhost:5279/signout-oidc",
                PostLogoutRedirectUris = { "http://localhost:5279/signout-callback-oidc" },
                AllowOfflineAccess= true,
                RequirePkce = true,
                RequireConsent = true,
                AllowPlainTextPkce= false
            }
        };
}
