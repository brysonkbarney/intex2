using intex2.Models;
using intex2.CustomPolicy;
using intex2.wwwroot.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
var services = builder.Services;
var configuration = builder.Configuration;

var keyVaultUrl = "https://intexsecrets.vault.azure.net/";
var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

services.AddSingleton(client);

KeyVaultSecret id = await client.GetSecretAsync("Authentication-Google-ClientId");
KeyVaultSecret secret = await client.GetSecretAsync("Authentication-Google-ClientSecret");
KeyVaultSecret connection = await client.GetSecretAsync("ConnectionStrings-DefaultConnection");

builder.Services.AddAuthentication()
    .AddGoogle(opts =>
    {
        opts.ClientId = id.Value;
        opts.ClientSecret = secret.Value;
        opts.SignInScheme = IdentityConstants.ExternalScheme;
    });

services.AddSingleton<IConfiguration>(configuration);
services.AddScoped<PredictionService>();
services.AddTransient<EmailHelper>();

builder.Services.AddDbContext<Lego2IntexContext>(options =>
    options.UseSqlServer(connection.Value));
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<Lego2IntexContext>().AddDefaultTokenProviders();
builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(10));

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".AspNetCore.Identity.Application";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;
});

//add back to change default login route
//builder.Services.ConfigureApplicationCookie(opts => opts.LoginPath = "/Authenticate/Login");

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("AspManager", policy =>
    {
        policy.RequireRole("Admin");
        //policy.RequireClaim("Coding-Skill", "ASP.NET Core MVC");
    });
});

builder.Services.AddTransient<IAuthorizationHandler, AllowUsersHandler>();

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("AllowTom", policy =>
    {
        policy.AddRequirements(new AllowUserPolicy("tom"));
    });
});

builder.Services.AddTransient<IAuthorizationHandler, AllowPrivateHandler>();

builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("PrivateAccess", policy =>
    {
        policy.AddRequirements(new AllowPrivatePolicy());
    });
});

builder.Services.Configure<IdentityOptions>(opts =>
{
    opts.User.RequireUniqueEmail = true;
    opts.Lockout.AllowedForNewUsers = true;
    opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    opts.Lockout.MaxFailedAccessAttempts = 3;
    opts.Password.RequiredLength = 8;
    opts.Password.RequireLowercase = true;
    opts.SignIn.RequireConfirmedEmail = true;
    opts.Password.RequireDigit = true;
    opts.Password.RequireNonAlphanumeric = true;
});

/*builder.Services.Configure<IdentityOptions>(opts =>
{
    opts.SignIn.RequireConfirmedEmail = true;
});*/
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ILegoRepository, EFLegoRepository>();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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

app.UseSession();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ContentSecurityPolicyMiddleware>(); // Add this line

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();