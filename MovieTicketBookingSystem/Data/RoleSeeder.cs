
using Microsoft.AspNetCore.Identity;

namespace MovieTicketBookingSystem.Data;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(
        IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = { "Admin", "Customer" };

        foreach (var role in roles)
        {
            var roleExists = await roleManager.RoleExistsAsync(role);

            if (!roleExists)
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole(role));

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        result.Errors.Select(error => error.Description));

                    throw new InvalidOperationException(
                        $"Failed to create role {role}: {errors}");
                }
            }
        }
    }
}
