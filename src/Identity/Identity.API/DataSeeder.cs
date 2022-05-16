using Duende.IdentityServer.EntityFramework.DbContexts;
using Identity.API.Database;
using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.API
{
    public static class DataSeeder
    {
        public static void MigrateDatabase(WebApplication app)
        {
            using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
            scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
        }

        public static async Task SeedData(WebApplication app)
        {
            using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            
            if (!context.Clients.Any())
            {
                logger.LogDebug("Clients being populated.");
                context.SaveChanges();
            }
            else
            {
                logger.LogDebug("Clients already populated.");
            }

            if (!context.ClientCorsOrigins.Any())
            {
                logger.LogDebug("Client Cors Origins being populated.");
                context.SaveChanges();
            }
            else
            {
                logger.LogDebug("Client Cors Origins already populated.");
            }

            if (!context.IdentityResources.Any())
            {
                logger.LogDebug("Identity Resources being populated.");
                context.SaveChanges();
            }
            else
            {
                logger.LogDebug("Identity Resources already populated.");
            }

            if (!context.ApiResources.Any())
            {
                logger.LogDebug("Api Resources being populated.");
                context.SaveChanges();
            }
            else
            {
                logger.LogDebug("Api Resources already populated.");
            }

            if (!context.ApiScopes.Any())
            {
                logger.LogDebug("Api Scopes being populated.");
                context.SaveChanges();
            }
            else
            {
                logger.LogDebug("Api Scopes already populated.");
            }

            if (!context.IdentityProviders.Any())
            {
                logger.LogDebug("Identity Providers being populated.");
                context.SaveChanges();
            }
            else
            {
                logger.LogDebug("Identity Providers already populated.");
            }

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.FindByNameAsync("admin");
            if (user is null)
            {
                user = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@site.com",
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(user, "Test123!");
                if (!result.Succeeded)
                    throw new InvalidOperationException($"Admin user can't be created. Result: {result.Errors.First().Description}");

                // TODO: Create roles class
               /* result = await userManager.AddToRoleAsync(user, "Administrator");
                if (!result.Succeeded)
                    throw new InvalidOperationException($"Admin claims can't be added. Result: {result.Errors.First().Description}");*/
            }
        }
    }
}
