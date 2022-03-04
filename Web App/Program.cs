using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web_App.Data;
using Web_App.Data.Account;
using Web_App.Services;
using Web_App.Services.Contracts;
using Web_App.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build().GetConnectionString("Default"));
});
builder.Services.Configure<SmtpSetting>(new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build().GetSection("SMTP"));
var secretPath = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build().GetValue<string>("MyAppSecretPaths");
builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = new ConfigurationBuilder().AddJsonFile(secretPath).Build().GetValue<string>("FacebookAppId");
    options.AppSecret = new ConfigurationBuilder().AddJsonFile(secretPath).Build().GetValue<string>("FacebookAppSecret");
});
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.SignIn.RequireConfirmedEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
