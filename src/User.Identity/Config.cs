﻿using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;

namespace User.Identity
{
    public class Config
    {
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client()
                {
                    ClientId="android",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },
                    RefreshTokenExpiration=TokenExpiration.Sliding,
                    AllowOfflineAccess=true,
                    RequireClientSecret=false,
                    AllowedGrantTypes=new List<string>{ "sms_auth_code" },
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AllowedScopes = new List<string>
                    {
                        "gateway_api",
                        "user_api",
                        "contact_api",
                        "project_api",
                        "recommend_api",
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("gateway_api","gateway service"),
                new ApiResource("user_api","user service"),
                new ApiResource("contact_api","contact service"),
                new ApiResource("project_api","project service"),
                new ApiResource("recommend_api","recommend service")
            };
        }
    }
}
