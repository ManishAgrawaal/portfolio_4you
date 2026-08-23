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
// Database
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
// Swagger
// IMPORTANT:
// Enabled for Azure Production also
// =========================

app.UseSwagger();

app.UseSwaggerUI();


// =========================
// CORS
// =========================

app.UseCors("AllowUI");


// =========================
// HTTPS
// =========================
app.UseDefaultFiles();
app.UseStaticFiles();


//==========================
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