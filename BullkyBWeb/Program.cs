using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BullkyB.DataAccess.Data;
using BullkyB.DataAccess.Repository.IRepository;
using BullkyB.DataAccess.Repository;
using Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore.Internal;
using BullkyB.DataAccess.IDbInilizer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContextConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//options => options.SignIn.RequireConfirmedAccount = true
builder.Services.Configure<StripeSetting>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<IUnitOfWrok, UnitOfWrok>();
builder.Services.AddScoped<IDbInilizer, DbInilizer>();
builder.Services.AddSingleton<IEmailSender,EmailSender>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/Manage/AccessDenied";
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly=true;
    options.Cookie.IsEssential = true;
});
/*builder.Services.AddAuthentication().AddFacebook(options=>{
    options.AppId = "854907665685853";
    options.AppSecret = "c0fabb693df6c09bc3a65da7343435b3";
    
});*/
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
seedDatabase();
app.UseAuthentication();;
app.UseSession();
app.UseAuthorization();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
void seedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var DbIntilizer = scope.ServiceProvider.GetRequiredService<IDbInilizer>();
        DbIntilizer.intials();       
    }
}