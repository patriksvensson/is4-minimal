using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace MyIdentityServer.Configuration
{
    internal class MyClients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client.internal",
                    ClientSecrets = { new Secret("internalSecret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = new List<string>
                    {
                        "api.internal",
                        "api.thirdparty"
                    }
                },

                new Client
                {
                    ClientId = "client.thirdparty",
                    ClientSecrets = { new Secret("thirdpartySecret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = new List<string>
                    {
                        "api.thirdparty"
                    }
                },

                new Client
                {
                    ClientId = "openidconnect",
                    ClientName = "OIDC client",
                    ClientSecrets = { new Secret("openidconnectSecret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "role",
                        "api.thirdparty",
                        "api.internal",
                    },
                    RedirectUris = new List<string> { "http://localhost:8000/signin-oidc" },
                    PostLogoutRedirectUris = new List<string> { "http://localhost:8000" }
                },

                new Client
                {
                    ClientId = "openidconnect.is4",
                    ClientName = "ExampleOIDC client",
                    ClientSecrets = { new Secret("oidcis4Secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "role",
                        "api.thirdparty",
                        "api.internal",
                    },
                    RedirectUris = new List<string> { "http://localhost:5000/signin-oidc" },
                    PostLogoutRedirectUris = new List<string> { "http://localhost:5000" }
                },
            };
        }
    }
}
