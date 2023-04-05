using IdentityProviderSSO.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddIdentityServer()
                .AddInMemoryIdentityResources(
                    builder.Configuration.GetSection("IdentityServer:IdentityResources"))
                .AddInMemoryApiResources(
                    builder.Configuration.GetSection("IdentityServer:ApiResources"))
                .AddInMemoryClients(
                    builder.Configuration.GetSection("IdentityServer:Clients"))
                .AddDeveloperSigningCredential()
                .AddAspNetIdentity<IdentityUser>();

//var tokenValidationParameters = new TokenValidationParameters
//{
//    ValidateIssuerSigningKey = true,
//    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("o90IbCACXKUkunXoa18cODcLKnQTbjOo5ihEw9j58+8=")) ,
//    ValidateIssuer = true,
//    ValidIssuer = "issuer",
//    ValidateAudience = true,
//    ValidAudience = "audience",
//    ValidateLifetime = true,
//    ClockSkew = TimeSpan.Zero
//};

//builder.Services.AddAuthentication(o => {
//    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//        .AddCookie(cfg => cfg.SlidingExpiration = true)
//        .AddJwtBearer(cfg =>
//        {
//            cfg.Audience = "http://localhost:4200/";
//            cfg.Authority = "http://localhost:5000/";
//            cfg.RequireHttpsMetadata = false;
//            cfg.SaveToken = true;
//            cfg.TokenValidationParameters = tokenValidationParameters;
//            cfg.Configuration = new OpenIdConnectConfiguration();
//        });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    //app.UseMigrationsEndPoint();
}
else
{
    app.UseDeveloperExceptionPage();
    //app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();////////////////////////////////

//app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
