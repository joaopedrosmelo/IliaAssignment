// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace IliaAssignment.Auth
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId()
            };
        public static IEnumerable<ApiScope> ApiScope =>
            new ApiScope[]
            {
                new ApiScope{
                    Name = "app.api.iliaassignment",
                    DisplayName = "IliaAssignment"
                }
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource {
                    Name = "app.api.iliaassignment",
                    DisplayName = "IliaAssignment",
                    ApiSecrets = { new Secret("A325674CA6312EF129CB7664ED194".Sha256()) },
                    Scopes = new List<string> { "app.api.iliaassignment" }
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientName = "IliaAssignment",
                    ClientId = "iliaassignment",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("C9C724B65F16464F88F24F87647AF".Sha256()) },
                    AllowedScopes = { "app.api.iliaassignment" }
                }
            };
    }
}