using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MyIdentityServer.Configuration;
using MyIdentityServer.Data;
using MyIdentityServer.Data.Models;
using MyIdentityServer.Services;

namespace MyIdentityServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            const string connection = @"Data Source=(LocalDb)\MSSQLLocalDB;database=MyIdentityServer4;trusted_connection=yes;";
            var assembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connection));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders(); ;

            services.AddIdentityServer()
                .AddOperationalStore(builder => builder.UseSqlServer(connection, o => o.MigrationsAssembly(assembly)))
                .AddConfigurationStore(builder => builder.UseSqlServer(connection, o => o.MigrationsAssembly(assembly)))
                .AddAspNetIdentity<ApplicationUser>()
                .AddProfileService<ProfileService>()
                .AddTemporarySigningCredential();

            services.AddTransient<IProfileService, ProfileService>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policyAdmin =>
                {
                    policyAdmin.RequireClaim("role", "admin");
                });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            InitializeDbTestData(app);

            ConfigureAuthentication(app);

            app.UseIdentity();
            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }

        private static void InitializeDbTestData(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
                scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();

                var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                if (!context.Clients.Any())
                {
                    foreach (var client in MyClients.Get())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in MyResources.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in MyResources.GetApiResources())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                if (!userManager.Users.Any())
                {
                    foreach (var inMemoryUser in MyUsers.Get())
                    {
                        var identityUser = new ApplicationUser()
                        {
                            Id = inMemoryUser.SubjectId,
                            UserName = inMemoryUser.Username,
                            IsAdmin = true
                        };

                        foreach (var claim in inMemoryUser.Claims)
                        {
                            identityUser.Claims.Add(new IdentityUserClaim<string>
                            {
                                UserId = identityUser.Id,
                                ClaimType = claim.Type,
                                ClaimValue = claim.Value,
                            });
                        }

                        userManager.CreateAsync(identityUser, "Password123!").Wait();
                    }
                }
            }
        }

        private static void ConfigureAuthentication(IApplicationBuilder app)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "cookie"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                ClientId = "openidconnect.is4",
                ClientSecret = "oidcis4Secret",
                Authority = "http://localhost:5000/",
                SignInScheme = "cookie",
                ResponseType = "code id_token",
                AuthenticationScheme = "oidc",
                Scope = { "role", "profile", "api.internal" },
                GetClaimsFromUserInfoEndpoint = true,
                RequireHttpsMetadata = false,
                TokenValidationParameters = new TokenValidationParameters()
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                },
            });
        }
    }
}
