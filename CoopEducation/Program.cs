using CoopEducation.Controllers;
using CoopEducation.Models;
using CoopEducation.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);


NSTools.ConfigurationHelper.Initialize(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AllServices>();
builder.Services.AddScoped<DocumentService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentUpdateService, StudentUpdateService>();
builder.Services.AddDbContext<CoopEducationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Key"]!);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                if (ctx.Request.Cookies.ContainsKey("access_token"))
                    ctx.Token = ctx.Request.Cookies["access_token"];
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(
                        "http://localhost:3000",
                        "http://localhost:3001",
                        "http://localhost:3002",
                        "http://localhost:3003",
                        "http://localhost:3004",
                        "http://localhost:3005"
                    )
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});
builder.Services.AddAuthorization();

// ───── Rate Limiting ─────
// Protect /login from brute-force: 5 attempts per 15 minutes per IP
// TODO (production): ถ้า deploy หลัง reverse proxy ต้องเพิ่ม UseForwardedHeaders
//                    ไม่งั้น RemoteIpAddress จะเป็น IP ของ proxy (ทุก user แชร์ IP เดียว)
// TODO (scale):      In-memory state — ถ้า scale horizontal ให้ย้ายไป Redis-backed limiter
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("login", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(15),
                QueueLimit = 0
            }));

    options.OnRejected = async (context, cancellationToken) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();
        await context.HttpContext.Response.WriteAsync(
            "Too many login attempts. Please try again later.", cancellationToken);
    };
});
//builder.Services.AddSwaggerGen(c =>
//{
//    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
//});
builder.Services.AddSingleton(sp =>
{
    var supabaseUrl = builder.Configuration["SUPABASE_URL"];
    var supabaseKey = builder.Configuration["SUPABASE_KEY"];
    var options = new Supabase.SupabaseOptions
    {
        AutoConnectRealtime = false
    };
    var client = new Supabase.Client(supabaseUrl, supabaseKey, options);
    client.InitializeAsync().GetAwaiter().GetResult();
    return client;
});

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapControllers();
app.Run();