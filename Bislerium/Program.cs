using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Bislerium.Data;
using Microsoft.AspNetCore.Http;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("BisleriumContextConnection") ?? throw new InvalidOperationException("Connection string 'BisleriumContextConnection' not found.");

builder.Services.AddDbContext<BisleriumContext>(options => options.UseSqlServer(connectionString));



builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BisleriumContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "Admin", "Manager" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
}
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string email = "admin@admin.com";
    string password = "Test@1234";
    if (await userManager.FindByEmailAsync(email) == null)
    {

        var user = new IdentityUser();
        user.UserName = email;
        user.Email = email;
        user.EmailConfirmed = true;
        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {

            await userManager.AddToRoleAsync(user, "Admin");
    
        }
        else
        {
            foreach (var error in result.Errors)
            {
               
                Console.WriteLine(error.Description);
            }
        }

    }

}

app.Run();
