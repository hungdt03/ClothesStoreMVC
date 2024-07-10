using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebBanQuanAo.Clients;
using WebBanQuanAo.Data;
using WebBanQuanAo.Helper;
using WebBanQuanAo.Models;
using WebBanQuanAo.Services.Implementations;
using WebBanQuanAo.Services.Interfaces;
using WebBanQuanAo.SignalR;
using WebBanQuanAo.SignalR.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddRouting(options =>
{
    options.LowercaseQueryStrings = true; 
    options.AppendTrailingSlash = false; 
});



builder.Services.AddSingleton<PresenceTracker>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IConnectionService, ConnectionService>();
builder.Services.AddScoped<VnpayHelper>();
builder.Services.AddScoped<VnPayUtils>();
builder.Services.AddSingleton(x =>
    new PaypalClient(
        builder.Configuration["PayPalOptions:ClientId"],
        builder.Configuration["PayPalOptions:ClientSecret"],
        builder.Configuration["PayPalOptions:Mode"]
    )
);

builder.Services.AddSignalR();

builder.Services.AddDbContext<StoreDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("MyDb"));
    option.EnableSensitiveDataLogging();
});

builder.Services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<StoreDbContext>()
        .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options => {
    options.Password.RequireDigit = false; 
    options.Password.RequireLowercase = false; 
    options.Password.RequireNonAlphanumeric = false; 
    options.Password.RequireUppercase = false; 
    options.Password.RequiredLength = 6; 
    options.Password.RequiredUniqueChars = 1; 

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lần thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại

});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

})
   .AddGoogle(googleOptions => {
       googleOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
       IConfigurationSection googleProviderSection = builder.Configuration.GetSection("GoogleProvider");
       googleOptions.ClientId = googleProviderSection["ClientId"]!;
       googleOptions.ClientSecret = googleProviderSection["ClientSecret"]!;
   })               
   .AddFacebook(facebookOptions => {
       facebookOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
       IConfigurationSection facebookProviderSection = builder.Configuration.GetSection("FacebookProvider");
       facebookOptions.AppId = facebookProviderSection["ClientId"]!;
       facebookOptions.AppSecret = facebookProviderSection["ClientSecret"]!;
   })
    .AddTwitter(twitterOptions =>
    {
       twitterOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
       IConfigurationSection twitterProviderSection = builder.Configuration.GetSection("TwitterProvider");
       twitterOptions.ConsumerKey = twitterProviderSection["ClientId"];
       twitterOptions.ConsumerSecret = twitterProviderSection["ClientSecret"];
       twitterOptions.RetrieveUserDetails = true;
    })
    .AddGitHub(githubOption =>
    {
        githubOption.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        IConfigurationSection githubProviderSection = builder.Configuration.GetSection("GithubProvider");
        githubOption.ClientId = githubProviderSection["ClientId"]!;
        githubOption.ClientSecret = githubProviderSection["ClientSecret"]!;
        githubOption.Scope.Add("user:email");
    })
   .AddCookie(options =>
   {
       options.Cookie.IsEssential = true;
       options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
       options.SlidingExpiration = true;
       options.LoginPath = "/Auth/Login"; // Specify the login page URL
       options.AccessDeniedPath = "/Auth/AccessDenied"; // Specify the access denied page URL
   });

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.AddSingleton<UploadFileHelper>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    //app.UseStatusCodePagesWithReExecute("/Error/HandleErrorCode/{0}");
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseCookiePolicy();

//app.UseMiddleware<AdminAreaAuthorizationMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "Admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<OrderNotificationHub>("/orderNotificationHub");
app.MapHub<MessageHub>("/messageHub");

app.Run();
