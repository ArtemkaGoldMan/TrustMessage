using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Services.Interfaces;
using Server.Services.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using FluentValidation;
using FluentValidation.AspNetCore;
using Server.Validators;
using AspNetCoreRateLimit;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using BaseLibrary.DTOs;

var builder = WebApplication.CreateBuilder(args);

// Configure database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageService, MessageService>();

// Add FluentValidation
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<RegisterRequestValidator>());

// Configure rate limiting
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.GeneralRules = new List<RateLimitRule>
    {
        new RateLimitRule
        {
            Endpoint = "POST:/api/auth/login",
            Limit = 5,
            Period = "1m"
        },
        new RateLimitRule
        {
            Endpoint = "*",
            Limit = 100,
            Period = "10m"
        }
    };
});
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddInMemoryRateLimiting();

// Add Swagger
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

// Add authentication using cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login";
        options.AccessDeniedPath = "/api/auth/denied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseIpRateLimiting();

// WebSocket connection security check
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/ws/messages") && !context.User.Identity.IsAuthenticated)
    {
        context.Response.StatusCode = 401;
        return;
    }
    await next();
});

// Enable WebSockets
app.UseWebSockets();
app.Map("/ws/messages", async context =>
{
    if (context.WebSockets.IsWebSocketRequest && context.User.Identity.IsAuthenticated)
    {
        using var ws = await context.WebSockets.AcceptWebSocketAsync();
        var messageService = context.RequestServices.GetRequiredService<IMessageService>();
        var userService = context.RequestServices.GetRequiredService<IUserService>();

        var buffer = new byte[1024 * 4];

        while (true)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                break;
            }

            var messageContent = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var user = await userService.GetUserByUsernameAsync(context.User.Identity.Name);

            if (user == null)
            {
                context.Response.StatusCode = 401;
                return;
            }

            // Create the message
            var messageDto = new CreateMessageDTO { Content = messageContent };
            var newMessage = await messageService.CreateMessageAsync(user.Id, messageDto);

            // Send the verified message to all clients
            var response = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(newMessage));
            await ws.SendAsync(response, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.MapControllers();
app.Run();
