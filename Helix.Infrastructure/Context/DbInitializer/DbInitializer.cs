using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Helix.Infrastructure.Context;
using Helix.Data.Enums;
using System.Data;
using Microsoft.Extensions.Hosting;
using System.Text.Json; 
using Microsoft.Extensions.Logging;
using Helix.Data.Entities;

namespace Helix.Infrastructure.Context.DbInitializer
{
    // 4. Add IHostEnvironment and ILogger to the constructor
    public class DbInitializer(
        UserManager<AppUser> _userManager,
        RoleManager<IdentityRole> _roleManager,
        ApplicationDbContext _dbContext,
        ILogger<DbInitializer> _logger) : IDbInitializer
    {
        public async Task Initialize()
        {
            // 1. Apply Any Pending Migrations
            try
            {
                if ((await _dbContext.Database.GetPendingMigrationsAsync()).Any())
                {
                    await _dbContext.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during migration.");
            }

            // 2. Create Admin User (if he doesn't exist)
            string adminEmail = "admin@Helix.com";
            string adminRoleName = EnRoles.SuperAdmin.ToString();
            var user = await _userManager.FindByEmailAsync(adminEmail);

            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "admin",
                    FullName = "Admin",
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Address = "Admin Address",
                };

                var result = await _userManager.CreateAsync(user, "Admin#123");
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create admin user. Errors: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            // 3. Create Roles if they don't exist
            // Assign the all Roles to the admin user
            // We loop through the enum and create any missing roles
            foreach (var roleValue in Enum.GetValues(typeof(EnRoles)))
            {
                string roleName = roleValue.ToString();

                // This is the correct check: check for the *current* role in the loop
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    // Create the role
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
                // Check if the user is *already* in the role before adding
                if (!await _userManager.IsInRoleAsync(user, roleName))
                {
                    // Await the async call
                    await _userManager.AddToRoleAsync(user, roleName);
                }
            }
        }
    }
}
