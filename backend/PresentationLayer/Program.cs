using BusinessLogicLayer;
using DataAccessLayer;
using DataAccessLayer.Constants.Config;
using DataAccessLayer.Constants.Messages;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using PresentationLayer.Common;
using PresentationLayer.Middlewares;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddBLLServices(builder.Configuration);
builder.Services.AddDALServices(builder.Configuration);


// Configure JWT settings
var jwtSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSection);

var jwtSettings = jwtSection.Get<JwtSettings>()
                  ?? throw new InvalidOperationException("JwtSettings is missing in configuration.");
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddAuthentication(o => {
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.ClaimsIssuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
    options.Events = new JwtBearerEvents
    {
        OnChallenge = ctx => {
            ctx.HandleResponse();
            ctx.Response.StatusCode = 401;
            return ctx.Response.WriteAsJsonAsync(new ApiResponse<object?>(false, "Unauthorized", null));
        }
    };
});

builder.Services.AddAuthorization();

// Rate limiter configuration
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var clientIp = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 100,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    });

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        var response = new ApiResponse<object?>(false, StatusCode.TooManyRequests429, null);
        await context.HttpContext.Response.WriteAsJsonAsync(response, cancellationToken);
    };
});

// Register check health
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!, name: "HerDatabase");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/app-log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// Serilog configuration
var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapSwaggerUI().RequireAuthorization();

app.MapGet("/test-log", (ILoggerFactory loggerFactory) =>
{
    var logger = loggerFactory.CreateLogger("TestApi");
    if (logger.IsEnabled(LogLevel.Information))
        logger.LogInformation("User {UserId} called API at {Time}", "12345", DateTime.UtcNow);

    return "Log information successfully!";
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API v1");
    });
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
