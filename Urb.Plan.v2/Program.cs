using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Urb.Plan.v2.Mapper;
using Urb.Application.App.Settings;
using Urb.Domain.Urb.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
builder.Services.AddDbContext<MainDataContext>(options => options.UseSqlServer(configuration.GetConnectionString("SqlConnection")));
builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-.";
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 6;     
        options.User.RequireUniqueEmail = true;
    })
.AddEntityFrameworkStores<MainDataContext>()
.AddSignInManager<SignInManager<User>>()
.AddDefaultTokenProviders();
builder.Services.AddControllersWithViews()
.AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore); 
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
    .AddCookie()
    .AddGoogle(googleOptions =>
    {
        var g = configuration.GetSection("Authentication:Google");
        googleOptions.ClientId = g["ClientId"];
        googleOptions.ClientSecret = g["ClientSecret"];
        googleOptions.SaveTokens = true;
        googleOptions.Scope.Add("email");
        googleOptions.Scope.Add("profile");
    })
    .AddJwtBearer(jwt =>
    {
        var jwtKey = configuration["AppSettings:JWTkey"];
        jwt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                                          Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//})
//.AddCookie()
//.AddGoogle(googleOptions =>
//{
//    var googleAuth = configuration.GetSection("Authentication:Google");
//    googleOptions.ClientId = googleAuth["ClientId"];
//    googleOptions.ClientSecret = googleAuth["ClientSecret"];
//    googleOptions.SaveTokens = true;          
//    googleOptions.Scope.Add("email");
//    googleOptions.Scope.Add("profile");
//});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "MVCCallWebAPI", Version = "v2" });
});
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "Pinus",
    builder =>
    {
    builder.WithOrigins("https://red-pebble-049af5603.4.azurestaticapps.net/",
    "http://localhost:5173/")
    .AllowAnyHeader()
    .AllowAnyMethod();
    });
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Urb.API", Version = "Test" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                    }
                },
            new string[] {}
        }
    });
});

var app = builder.Build();
if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
    }
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v2/swagger.json", "MVCCallWebAPI");
});
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();


