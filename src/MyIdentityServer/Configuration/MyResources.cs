using System.Collections.Generic;
using IdentityServer4.Models;
using Microsoft.IdentityModel.Tokens;

namespace MyIdentityServer.Configuration
{
    internal class MyResources
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "role",
                    DisplayName = "Your user role",
                    Required = true,
                    UserClaims = new List<string> { "role" },
                },
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = "api",
                    ApiSecrets = new List<Secret> {new Secret("apiSecret".Sha256())},
                    Scopes = new List<Scope>
                    {
                        new Scope("api.thirdparty", "Third party access")
                        {
                            Required = true,
                            ShowInDiscoveryDocument = true,
                            UserClaims = new[] { "api.thirdparty" }
                        },
                        new Scope("api.internal", "Internal access")
                        {
                            Required = true,
                            ShowInDiscoveryDocument = false,
                            UserClaims = new[] { "api.thirdparty", "api.internal" }
                        },
                    }
                },
            };
        }
    }
}