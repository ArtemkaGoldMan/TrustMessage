using AspNetCoreRateLimit;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Server.Data;
using Server.Hubs;
using Server.Services.Implementations;
using Server.Services.Interfaces;
using Server.Validations;
using Server.Security;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<RegisterRequestValidator>());

// Add rate limiting services
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();


// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp",
        builder =>
        {
            builder.WithOrigins("https://localhost:7281") // Replace with your Blazor app's URL
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

// Add other services...
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger to support cookie authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TrustMessageApp API", Version = "v1" });

    // Add support for cookie authentication in Swagger
    c.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Cookie,
        Name = ".AspNetCore.Cookies", // Name of the authentication cookie
        Description = "Cookie-based authentication"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "cookieAuth"
                }
            },
            new string[] { }
        }
    });
});

// Register the ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the AuthService and UserService
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddSignalR();

// Add in-memory distributed cache (for development only)
builder.Services.AddDistributedMemoryCache();

// Add session support
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true; // Prevent client-side script access to the cookie
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensure cookies are only sent over HTTPS
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.SameSite = SameSiteMode.Strict; // Prevent CSRF attacks
});

// Add cookie authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = "/api/auth/login"; // Redirect to login if unauthorized
        options.AccessDeniedPath = "/api/auth/access-denied"; // Redirect if access is denied
    });

// Add this to your services configuration
builder.Services.AddSingleton<KeyManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TrustMessageApp API V1");
        c.ConfigObject.AdditionalItems["persistAuthorization"] = true; // Persist authorization across Swagger sessions
    });
}

app.UseCors("AllowBlazorApp");

// Enable rate limiting middleware
app.UseIpRateLimiting();

// Enable SignalR
app.MapHub<MessageHub>("/messageHub");

// Serve static files from wwwroot
app.UseStaticFiles();

// Enable session middleware
app.UseSession();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    // Add Content Security Policy
    context.Response.Headers.Add("Content-Security-Policy", 
        "default-src 'self'; " +
        "script-src 'self'; " +
        "style-src 'self'; " +
        "img-src 'self' data:; " +
        "frame-src 'none'; " +
        "connect-src 'self'; " +
        "font-src 'self'; " +
        "object-src 'none'; " +
        "base-uri 'self'; " +
        "form-action 'self'; " +
        "frame-ancestors 'none'; " +
        "upgrade-insecure-requests;");
    
    await next();
});

app.MapControllers();

app.Run();