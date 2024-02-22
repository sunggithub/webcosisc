using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMicroseconds(24); // Thời gian chờ phiên (24 giờ ở đây)
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
string? tokenKeyString = builder.Configuration.GetSection("AppSettings:TokenKey").Value;

 builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    tokenKeyString != null ? tokenKeyString : ""
                )),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });  

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("admin"));
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "YourAuthCookieName";
        options.LoginPath = "/Admin/Auth/Login";
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "adminAreaRoute",
    pattern: "Admin/{controller=HomeAdmin}/{action=Index}/{id?}",
    defaults: new { area = "Admin" }
).RequireAuthorization("AdminPolicy");

app.MapControllerRoute(
    name: "managerAreaRoute",
    pattern: "Manager/{controller=HomeManager}/{action=Index}/{id?}",
    defaults: new { area = "Manager" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);



app.Run();
