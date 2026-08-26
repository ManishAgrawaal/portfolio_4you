using DevPortfolio.API.Data;
using DevPortfolio.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using DevPortfolio.API.Models;
using Microsoft.AspNetCore.Identity;

// ==========================================
// OpenTelemetry - Grafana Cloud
// ==========================================
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;


var builder = WebApplication.CreateBuilder(args);


// =========================
// Controllers
// =========================

builder.Services.AddControllers();


// =========================
// Database - SQLite
// =========================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));


// =========================
// CORS
// =========================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUI", policy =>
    {
        policy
            .WithOrigins(
                "https://manishtechnologysolution.com",
                "https://www.manishtechnologysolution.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


// =========================
// Email Service
// =========================

builder.Services.AddScoped<IEmailService, EmailService>();


// =========================
// JWT Authentication
// =========================

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException(
        "JWT Key is missing from configuration."
    );
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ClockSkew = TimeSpan.Zero
        };
    });


// =========================
// Authorization
// =========================

builder.Services.AddAuthorization();

builder.Services.AddHttpClient();


// ==========================================
// OpenTelemetry - Grafana Cloud
// ==========================================

var otlpEndpoint =
    Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");

var otlpHeaders =
    Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS");


builder.Services
    .AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource.AddService("DevPortfolio.API"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();

        // Export traces only when Grafana configuration exists
        if (!string.IsNullOrWhiteSpace(otlpEndpoint))
        {
            tracing.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otlpEndpoint);

                if (!string.IsNullOrWhiteSpace(otlpHeaders))
                {
                    options.Headers = otlpHeaders;
                }
            });
        }
    })
    .WithMetrics(metrics =>
    {
        metrics.AddAspNetCoreInstrumentation();

        // Export metrics only when Grafana configuration exists
        if (!string.IsNullOrWhiteSpace(otlpEndpoint))
        {
            metrics.AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otlpEndpoint);

                if (!string.IsNullOrWhiteSpace(otlpHeaders))
                {
                    options.Headers = otlpHeaders;
                }
            });
        }
    });


// ==========================================
// OpenTelemetry Logging
// ==========================================

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
    logging.ParseStateValues = true;

    // Export logs only when Grafana configuration exists
    if (!string.IsNullOrWhiteSpace(otlpEndpoint))
    {
        logging.AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri(otlpEndpoint);

            if (!string.IsNullOrWhiteSpace(otlpHeaders))
            {
                options.Headers = otlpHeaders;
            }
        });
    }
});


// =========================
// Swagger
// =========================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // JWT Bearer definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",

        Type = SecuritySchemeType.Http,

        Scheme = "bearer",

        BearerFormat = "JWT",

        In = ParameterLocation.Header,

        Description =
            "Enter JWT token as: Bearer {your JWT token}"
    });


    // Apply JWT security globally
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },

            Array.Empty<string>()
        }
    });
});


var app = builder.Build();


// =========================
// DATABASE MIGRATION
// =========================
// Creates SQLite database/tables
// automatically on Render startup.

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();


    // Apply EF Core migrations first
    db.Database.Migrate();


    // ==========================================
    // ADMIN USER SEED
    // ==========================================

    var adminUsername =
        builder.Configuration["Admin:Username"];

    var adminPassword =
        builder.Configuration["Admin:Password"];


    if (!string.IsNullOrWhiteSpace(adminUsername) &&
        !string.IsNullOrWhiteSpace(adminPassword))
    {
        var existingAdmin = db.AdminUsers
            .FirstOrDefault(x =>
                x.Username == adminUsername);


        if (existingAdmin == null)
        {
            var admin = new AdminUser
            {
                Username = adminUsername,
                Role = "Admin",
                IsActive = true
            };


            var passwordHasher =
                new PasswordHasher<AdminUser>();


            admin.PasswordHash =
                passwordHasher.HashPassword(
                    admin,
                    adminPassword);


            db.AdminUsers.Add(admin);

            db.SaveChanges();
        }
    }


    // ==========================================
    // DEFAULT PROJECT SEED
    // ==========================================

    // This is useful for a fresh Render/SQLite deployment.
    var hasProjects = db.Database
        .SqlQueryRaw<int>(
            "SELECT COUNT(*) AS \"Value\" FROM Projects")
        .AsEnumerable()
        .FirstOrDefault() > 0;


    if (!hasProjects)
    {
        db.Database.ExecuteSqlRaw(@"
            INSERT INTO Projects
                (
                    Title,
                    Description,
                    Technologies,
                    ImageUrl,
                    ProjectUrl,
                    GithubUrl,
                    CreatedAt
                )
            VALUES
                (
                    'GraphLens – Wexa AI',
                    'A recruiter, engineering manager or delivery lead wants to discover people who are relevant to a person based on shared technical skills and shared project experience.',
                    'ASP.NET Core Razor Pages + C# + the official Neo4j .NET driver + CognodDB',
                    '',
                    '',
                    '',
                    datetime('now')
                );
        ");
    }
}


// =========================
// Swagger
// =========================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// =========================
// CORS
// =========================

app.UseCors("AllowUI");


// =========================
// Static Files
// =========================

app.UseDefaultFiles();
app.UseStaticFiles();


// =========================
// HTTPS
// =========================

app.UseHttpsRedirection();


// =========================
// Authentication
// =========================

app.UseAuthentication();


// =========================
// Authorization
// =========================

app.UseAuthorization();


// =========================
// Controllers
// =========================

app.MapControllers();


app.Run();