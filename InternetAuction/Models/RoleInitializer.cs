using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace InternetAuction.Models
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string masterLogin = "master";
            string password = "fFRESHh12345@";
            if (await roleManager.FindByNameAsync("master") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("master"));
            }
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }

            if (await roleManager.FindByNameAsync("moderator") == null)
            {
                await roleManager.CreateAsync(new IdentityRole("moderator"));
            }
            if (await userManager.FindByNameAsync(masterLogin) == null)
            {
                User master = new User { Email = masterLogin, UserName = masterLogin };
                IdentityResult result = await userManager.CreateAsync(master, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(master, "master");
                }
            }
        }
    }
}