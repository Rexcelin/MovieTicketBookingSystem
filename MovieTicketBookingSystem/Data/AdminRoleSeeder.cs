
using Microsoft.AspNetCore.Identity;
using MovieTicketBookingSystem.Models;

namespace MovieTicketBookingSystem.Data;

public static class AdminRoleSeeder
{
    public static async Task AssignAdminRoleAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        var userManager = serviceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        // Get Admin email from User Secrets
        var adminEmail = configuration["Admin:Email"];

        if (string.IsNullOrWhiteSpace(adminEmail))
        {
            throw new InvalidOperationException(
                "Admin email is missing from User Secrets.");
        }

        // Find the existing Admin account
        var adminUser = await userManager.FindByEmailAsync(
            adminEmail.Trim());

        if (adminUser is null)
        {
            throw new InvalidOperationException(
                $"Admin account {adminEmail} was not found.");
        }

        // Assign Admin role
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            var result = await userManager.AddToRoleAsync(
                adminUser,
                "Admin");

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(errors);
            }
        }

        // Remove Customer role from this Admin account
        if (await userManager.IsInRoleAsync(adminUser, "Customer"))
        {
            var result = await userManager.RemoveFromRoleAsync(
                adminUser,
                "Customer");

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(e => e.Description));

                throw new InvalidOperationException(errors);
            }
        }
    }
}
