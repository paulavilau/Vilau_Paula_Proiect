using Microsoft.EntityFrameworkCore;
using Nume_Pren_Lab2.Data;
using ToyStore.Hubs;
using Vilau_Paula_Proiect.Data;
using Microsoft.AspNetCore.Identity;
using ToyStore.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ToyStoreContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
     .AddRoles<IdentityRole>()
     .AddEntityFrameworkStores<IdentityContext>();

builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSignalR();

builder.Services.AddRazorPages();

builder.Services.Configure<IdentityOptions>(options => {
    // Userul va fi blocat dupa 3 incercari esuate
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
    options.Lockout.MaxFailedAccessAttempts = 2;
    options.Lockout.AllowedForNewUsers = true;

    // Constrangeri parola
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Fiecare user trebuie sa aiba un email unic
    options.User.RequireUniqueEmail = true;

    // Contul trebuie confirmat
    options.SignIn.RequireConfirmedAccount = false;
});

builder.Services.AddAuthorization(opts => {
    opts.AddPolicy("CustomerService", policy => {
        policy.RequireClaim("Department", "CustomerService");
    });
});

builder.Services.AddAuthorization(opts => {
    opts.AddPolicy("StockEmployees", policy => {
        policy.RequireRole("Employee");
        policy.RequireClaim("Department", "Stock");
    });
});

builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.AccessDeniedPath = "/Identity/Account/AccessDenied";

});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    DbInitializer.Initialize(services);
}

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Toys}/{action=Index}/{id?}");

app.MapHub<ForumHub>("/Chat");

app.MapRazorPages();

app.Run();
