using DevPortfolio.API.Data;
using DevPortfolio.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

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
            .AllowAnyOrigin()
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

    // Seed initial portfolio project if the database is empty.
    // This is useful for a fresh Render/SQLite deployment.
    var hasProjects = db.Database
        .SqlQueryRaw<int>("SELECT COUNT(*) AS \"Value\" FROM Projects")
        .AsEnumerable()
        .FirstOrDefault() > 0;

    if (!hasProjects)
    {
        db.Database.ExecuteSqlRaw(@"
            INSERT INTO Projects
                (Title, Description, Technologies, ImageUrl, ProjectUrl, GithubUrl, CreatedAt)
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

app.UseSwagger();

app.UseSwaggerUI();

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