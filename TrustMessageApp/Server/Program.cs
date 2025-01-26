using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Services.Interfaces;
using Server.Services.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using FluentValidation;
using FluentValidation.AspNetCore;
using Server.Validators;
using AspNetCoreRateLimit;
using Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS to allow Blazor client access
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7068")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
    });
});

// Configure database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageService, MessageService>();

// Add SignalR
builder.Services.AddSignalR();

// Add FluentValidation
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<RegisterRequestValidator>());

// Configure rate limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule { Endpoint = "POST:/api/auth/login", Limit = 5, Period = "1m" },
        new RateLimitRule { Endpoint = "*", Limit = 100, Period = "10m" }
    };
});
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("Session:IdleTimeout"));
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = builder.Configuration["Session:CookieName"];
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login"; // Redirect to login if unauthorized
        options.AccessDeniedPath = "/api/auth/denied"; // Redirect if access is denied
        options.Cookie.HttpOnly = true; // Prevent client-side script access to the cookie
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensure cookies are only sent over HTTPS
        options.Cookie.SameSite = SameSiteMode.Strict; // Prevent CSRF attacks
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Set cookie expiration
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware pipeline setup
app.UseCors("AllowBlazorClient");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseIpRateLimiting();

// Map SignalR hub
app.MapHub<MessageHub>("/messagesHub");

app.MapGet("/", () => "Server is running!");

app.MapControllers();
app.Run();
