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
using Fun.Domain.Fun.Models;
using Fun.Application.Fun.IServices;
using Urb.Infrastructure.Fun.Services;
using Fun.Infrastructure.Fun.Services;
using Fun.Application.IComponentModels;
using Fun.Application.ComponentModels;
using Fun.Application.Fun.IRepositories;
using Fun.Persistance.Fun.Repositories;
using Fun.Application.Fun.Settings;
using Stripe;
using Fun.Application.ResponseModels;
using Microsoft.Extensions.Options;
//using Microsoft.AspNetCore.Server.Kestrel.Https;


var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
var dbProvider = configuration["DatabaseProvider"];

builder.Services.AddDbContext<MainDataContext>(options =>
{
    if (string.Equals(dbProvider, "Postgres", StringComparison.OrdinalIgnoreCase))
    {
        var cs = configuration.GetConnectionString("Postgres");
        options.UseNpgsql(cs);
    }
    else
    {
        var cs = configuration.GetConnectionString("SqlConnection");
        options.UseSqlServer(cs);
    }
});
//builder.Services.AddDbContext<MainDataContext>(opts =>
//{
//    if (dbProvider == "Postgres")
//    {
//        opts.UseNpgsql(
//            builder.Configuration.GetConnectionString("Postgres"),
//            b => b.MigrationsAssembly("Fun.Persistence"));
//    }
//    else
//    {
//        opts.UseSqlServer(
//            builder.Configuration.GetConnectionString("SqlConnection"));           
//    }
//});


//(options => options.UseSqlServer(configuration.GetConnectionString("SqlConnection")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy
            .AllowAnyOrigin()   // Дозволяємо запити з будь-якого origin
            .AllowAnyHeader()   // Дозволяємо будь-які заголовки (Content-Type, Authorization тощо)
            .AllowAnyMethod();  // Дозволяємо будь-які HTTP-методи (GET, POST, PUT, DELETE…)
        // .AllowCredentials() // НЕ викликайте AllowCredentials() разом із AllowAnyOrigin()! 
    });
});
builder.Services.AddIdentity<User, Role>(options =>
    {
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_-.";
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 6;     
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
    })
.AddEntityFrameworkStores<MainDataContext>()
.AddSignInManager<SignInManager<User>>()
.AddDefaultTokenProviders();
builder.Services.AddControllersWithViews()
.AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore); 
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always;
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;


    //options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
    //.AddCookie()
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => { /*options.Cookie.SameSite = SameSiteMode.Lax;*/
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, googleOptions =>
    {
        googleOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        googleOptions.CallbackPath = "/api/User/ExternalLoginCallback";
        var g = configuration.GetSection("Authentication:Google");    
        googleOptions.ClientId = g["ClientId"];
        googleOptions.ClientSecret = g["ClientSecret"];
        googleOptions.SaveTokens = true;
        googleOptions.Scope.Add("email");
        googleOptions.Scope.Add("profile");
        googleOptions.CorrelationCookie.SameSite = SameSiteMode.None;
        googleOptions.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

        googleOptions.CorrelationCookie.Name = ".AspNetCore.Correlation.Google";
        googleOptions.CorrelationCookie.Path = "/";

        //googleOptions.CorrelationCookie.HttpOnly = true;
        //googleOptions.CorrelationCookie.SameSite = SameSiteMode.Unspecified;

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

builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
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

builder.Services.Configure<StripeSettings>(
    builder.Configuration.GetSection("Stripe"));
var stripeOptions = builder.Configuration
    .GetSection("Stripe").Get<StripeSettings>();
StripeConfiguration.ApiKey = stripeOptions.SecretKey;


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
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, Fun.Infrastructure.Fun.Services.TokenService>();
builder.Services.AddScoped<IUserRegisterModel, UserRegisterModel>();
builder.Services.AddScoped<IInitiativeService, InitiativeService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IInitiativeComponentModel, InitiativeComponentModel>();
builder.Services.AddScoped<ICategoryComponentModel, CategoryComponentModel>();
builder.Services.AddScoped<ISubscribeComponentModel, SubscribeComponentModel>();
builder.Services.AddScoped<ISubscribeService, SubscribeService>();
builder.Services.AddScoped<IFundraisingService, FundraisingService>();
builder.Services.AddScoped<IDonateService, DonateService>();
builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddScoped(typeof(ICRUDRepository<>), typeof(CRUDRepository<>));
builder.Services.AddScoped<IFundraisingComponentModel, FundraisingComponentModel>();
builder.Services.AddScoped<IInitiativeResponseModel, InitiativeResponseModel>();
builder.Services.AddScoped<IFileService, Fun.Infrastructure.Fun.Services.FileService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();

//builder.Services.AddScoped<IFundraisingStatService, FundraisingStatService>();







//builder.WebHost.ConfigureKestrel(opts =>
//  opts.ListenLocalhost(5001, lo => lo.UseHttps(new HttpsConnectionAdapterOptions
//  {
//      ServerCertificate = CertificateLoader.LoadFromPemFile(
//      Path.Combine(AppContext.BaseDirectory, "localhost+2.pem"),
//      Path.Combine(AppContext.BaseDirectory, "localhost+2-key.pem"))
//  }))
//);



var app = builder.Build();
app.UseCors("AllowAllOrigins");
if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
    }
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MVCCallWebAPI");
});
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();