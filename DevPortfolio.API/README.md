# DevPortfolio.API --- Project Reference

## Project Overview

DevPortfolio.API is a full-stack personal portfolio application built
with **ASP.NET Core Web API, C#, Entity Framework Core, SQLite, HTML,
CSS and JavaScript**.

The application provides:

-   Public portfolio UI
-   Visitor contact form
-   Secure Admin Dashboard
-   JWT authentication and role-based authorization
-   SQLite persistence
-   Resend email integration
-   Resend webhook integration for visitor replies
-   Two-way visitor/admin conversation
-   Project management
-   Render production deployment
-   Environment-based production secrets

------------------------------------------------------------------------

## Technology Stack

### Backend

-   C#
-   .NET 8 / ASP.NET Core Web API
-   Entity Framework Core
-   SQLite
-   JWT Bearer Authentication
-   Role-based Authorization
-   ASP.NET Core PasswordHasher
-   Swagger / OpenAPI
-   HttpClient
-   Dependency Injection

### Frontend

-   HTML5
-   CSS3
-   JavaScript
-   Fetch API
-   Responsive UI
-   Admin Dashboard
-   Conversation Modal

### External Services / DevOps

-   Resend --- email delivery and inbound webhook
-   GitHub --- source control
-   Render --- production hosting and deployment
-   Custom domain --- `manishtechnologysolution.com`

------------------------------------------------------------------------

# Architecture

``` text
                    VISITOR
                       |
                       v
              +----------------+
              | Portfolio UI   |
              | index.html     |
              +-------+--------+
                      |
                 HTTPS / REST
                      |
                      v
              +----------------+
              | ASP.NET Core   |
              | Web API        |
              +---+--------+---+
                  |        |
                  |        +------------------+
                  v                           v
          +---------------+             +-----------+
          | SQLite / EF   |             |  Resend   |
          | Core Database |             | Email API |
          +---------------+             +-----+-----+
                                             |
                                           Webhook
                                             |
                                             v
                                      +--------------+
                                      | Webhook       |
                                      | Controller    |
                                      +------+--------+
                                             |
                                             v
                                          SQLite
                                             |
                                             v
                                      Admin Dashboard
```

------------------------------------------------------------------------

# Project Structure

``` text
DevPortfolio.API/
|
+-- Connected Services/
+-- Dependencies/
+-- Properties/
|
+-- wwwroot/
|   +-- css/
|   +-- images/
|   +-- js/
|   |   +-- f.js
|   +-- Resume/
|   +-- admin.html
|   +-- index.html
|
+-- Controllers/
|   +-- AuthController.cs
|   +-- ContactController.cs
|   +-- ProjectsController.cs
|   +-- WebhookController.cs
|
+-- Data/
|   +-- ApplicationDbContext.cs
|
+-- Migrations/
|
+-- Models/
|   +-- AdminUser.cs
|   +-- ContactMessage.cs
|   +-- ContactRequest.cs
|   +-- EmailReply.cs
|   +-- LoginRequest.cs
|   +-- LoginResponse.cs
|   +-- Project.cs
|
+-- publish/
|
+-- services/
|   +-- EmailService.cs
|   +-- IEmailService.cs
|
+-- appsettings.json
+-- Program.cs
+-- DevPortfolio.API.csproj
```

------------------------------------------------------------------------

# Folder Responsibilities

## Controllers

### AuthController.cs

Handles:

-   Admin login
-   Credential validation
-   JWT generation
-   Authentication-related operations

### ContactController.cs

Handles:

-   Visitor contact messages
-   Admin message management
-   Conversation history
-   Admin replies
-   Protected contact operations

Important conversation endpoint:

``` http
GET /api/Contact/{id}/replies
```

Admin reply endpoint:

``` http
POST /api/Contact/{id}/reply
```

### ProjectsController.cs

Handles portfolio project operations.

### WebhookController.cs

Handles:

-   Resend webhook events
-   Visitor email replies
-   Matching replies to existing conversations
-   Saving inbound replies

------------------------------------------------------------------------

# Data Layer

## ApplicationDbContext.cs

Entity Framework Core database context.

Main entities:

``` text
AdminUsers
ContactMessages
EmailReplies
Projects
```

Migrations are used to maintain the SQLite database schema.

------------------------------------------------------------------------

# Models

## AdminUser.cs

Represents the administrator.

Important properties:

``` text
Username
PasswordHash
Role
IsActive
```

Passwords are stored as hashes, not plaintext.

## ContactMessage.cs

Represents a visitor's original contact message.

Typical data:

``` text
Id
Name
Email
Subject
Message
CreatedAt
```

## EmailReply.cs

Stores inbound/outbound conversation reply information.

Typical data:

``` text
ContactMessageId
FromEmail
ToEmail
Subject
Message
ReceivedAt
ResendEmailId
MessageId
```

## ContactRequest.cs

Request DTO used by the visitor contact form.

## LoginRequest.cs

Request DTO used for administrator login.

## LoginResponse.cs

Response DTO returned after successful authentication.

## Project.cs

Represents portfolio project data.

------------------------------------------------------------------------

# Services

## IEmailService.cs

Defines the email-service contract.

The controller depends on the abstraction rather than directly depending
on the provider.

``` text
Controller
    |
    v
IEmailService
    |
    v
EmailService
    |
    v
Resend
```

## EmailService.cs

Responsible for:

-   Sending emails
-   Admin-to-visitor replies
-   Email configuration
-   Reply context
-   Resend API communication

------------------------------------------------------------------------

# Database

Current database:

``` text
SQLite
```

Connection:

``` text
Data Source=DevPortfolio.db
```

Entity Framework Core is used for database operations.

At startup, migrations are applied with:

``` csharp
db.Database.Migrate();
```

Useful tables:

``` text
AdminUsers
ContactMessages
EmailReplies
Projects
```

------------------------------------------------------------------------

# Database Relationship

The conversation model is:

``` text
ContactMessage
      |
      | 1 : Many
      v
EmailReply
```

Example:

``` text
ContactMessage #10
|
+-- Original visitor message
|
+-- EmailReply #1
|      +-- Admin reply
|
+-- EmailReply #2
|      +-- Visitor reply
|
+-- EmailReply #3
       +-- Admin reply
```

This allows the Admin Dashboard to display the full conversation.

------------------------------------------------------------------------

# Authentication and Authorization

The admin system uses:

``` text
JWT Bearer Authentication
```

Flow:

``` text
Admin Login
    |
    v
POST /api/Auth/login
    |
    v
Validate credentials
    |
    v
Verify password hash
    |
    v
Generate JWT
    |
    v
Frontend receives token
    |
    v
Protected API requests
```

Protected APIs use:

``` csharp
[Authorize(Roles = "Admin")]
```

------------------------------------------------------------------------

# Admin User Provisioning

The production application seeds the initial admin during startup when
the account does not already exist.

Configuration:

``` text
Admin:Username
Admin:Password
```

Render environment-variable names:

``` text
Admin__Username
Admin__Password
```

Logic:

``` text
Admin exists?
   |
   +-- YES --> keep existing admin
   |
   +-- NO ---> create admin
                 |
                 +--> hash password
                 |
                 +--> save AdminUser
```

The password is hashed using ASP.NET Core's `PasswordHasher<AdminUser>`.

------------------------------------------------------------------------

# Public UI

Main page:

``` text
wwwroot/index.html
```

Features:

-   Portfolio introduction
-   Projects
-   Technologies
-   Contact form
-   Responsive UI

Contact flow:

``` text
Visitor
   |
   v
Contact Form
   |
   v
JavaScript Fetch
   |
   v
POST /api/Contact
   |
   v
ContactMessages
   |
   v
Email notification
```

------------------------------------------------------------------------

# Admin Dashboard

Main page:

``` text
wwwroot/admin.html
```

Features:

-   Admin login
-   Visitor message list
-   Message statistics
-   View conversation
-   Delete message
-   Refresh messages
-   Admin reply
-   Conversation modal

Conversation UI displays:

-   Visitor email
-   Admin messages
-   Visitor messages
-   Subject
-   Date/time
-   Reply textarea
-   Close button
-   Send Reply button

------------------------------------------------------------------------

# Admin → Visitor Email Flow

``` text
Admin Dashboard
      |
      v
Write Reply
      |
      v
POST /api/Contact/{id}/reply
      |
      v
ContactController
      |
      v
IEmailService
      |
      v
EmailService
      |
      v
Resend API
      |
      v
Visitor Email
```

------------------------------------------------------------------------

# Visitor → Admin Email Flow

``` text
Visitor
   |
   v
Replies to email
   |
   v
Resend
   |
   v
Webhook
   |
   v
POST /api/Webhook/resend
   |
   v
WebhookController
   |
   v
Find related ContactMessage
   |
   v
Save EmailReply
   |
   v
Admin opens conversation
   |
   v
Visitor reply appears
```

This creates a two-way communication system.

------------------------------------------------------------------------

# Resend Integration

Resend is used for outbound email and inbound webhook processing.

## Outbound

``` text
ASP.NET Core
     |
     v
Resend API
     |
     v
Visitor / recipient
```

## Inbound

``` text
Visitor reply
     |
     v
Resend
     |
     v
Webhook
     |
     v
ASP.NET Core
     |
     v
EmailReplies
```

------------------------------------------------------------------------

# Configuration and Secrets

Production secrets must not be committed to GitHub.

Examples of sensitive values:

``` text
Resend API key
JWT signing key
Admin password
SMTP credentials
Other access tokens
```

Use environment variables / local secret storage instead.

## Render Environment Variables

Nested ASP.NET Core configuration uses double underscores:

``` text
ConnectionStrings__DefaultConnection

EmailSettings__ApiKey
EmailSettings__FromEmail
EmailSettings__Password
EmailSettings__Port
EmailSettings__SmtpServer
EmailSettings__ToEmail
EmailSettings__Username

Jwt__Key
Jwt__Issuer
Jwt__Audience
Jwt__ExpirationMinutes

Admin__Username
Admin__Password
```

Do not publish the actual values.

------------------------------------------------------------------------

# CORS

Production CORS is restricted to the portfolio domains:

``` text
https://manishtechnologysolution.com
https://www.manishtechnologysolution.com
```

Avoid unrestricted production CORS such as:

``` csharp
.AllowAnyOrigin()
```

------------------------------------------------------------------------

# Swagger

Swagger/OpenAPI is intended for development and local API testing.

``` text
Development -> Swagger enabled
Production  -> Swagger disabled
```

This keeps local development convenient while reducing unnecessary
production exposure.

------------------------------------------------------------------------

# Local Development

## Prerequisites

-   .NET SDK
-   Visual Studio or VS Code
-   Git
-   Browser
-   Optional: DB Browser for SQLite

## Step 1 --- Clone

``` bash
git clone <repository-url>
cd portfolio_4you
```

## Step 2 --- Open

Open the solution in Visual Studio:

``` text
DevPortfolio.API.sln
```

## Step 3 --- Restore

``` bash
dotnet restore
```

## Step 4 --- Build

``` bash
dotnet build
```

Expected:

``` text
Build succeeded
```

## Step 5 --- Run

Use Visual Studio / IIS Express or:

``` bash
dotnet run
```

## Step 6 --- Test

Use the local application and Swagger while running in Development.

------------------------------------------------------------------------

# SQLite Database Viewer

The local database file is:

``` text
DevPortfolio.db
```

A convenient GUI application for inspecting it is:

**DB Browser for SQLite**

Use it to:

-   Open the database
-   Browse tables
-   Inspect records
-   Run SQL queries
-   Check migrations/data

Important:

``` text
Local DevPortfolio.db
```

and the production database are separate environments.

------------------------------------------------------------------------

# Git Workflow

Check status:

``` bash
git status
```

Review changes:

``` bash
git diff
```

Stage:

``` bash
git add .
```

Commit:

``` bash
git commit -m "Describe the change"
```

Push:

``` bash
git push origin main
```

------------------------------------------------------------------------

# Render Deployment

Production hosting is handled by Render.

Deployment flow:

``` text
Local Development
       |
       v
Build / Test
       |
       v
Git Commit
       |
       v
GitHub main
       |
       v
Render detects commit
       |
       v
Build
       |
       v
Deploy
       |
       v
Live Production
```

After deployment:

``` text
Render
  -> Events / Deployments
  -> Confirm status = Live
```

------------------------------------------------------------------------

# Production Domain

Website:

``` text
https://manishtechnologysolution.com
```

Admin:

``` text
https://manishtechnologysolution.com/admin.html
```

------------------------------------------------------------------------

# Testing Checklist

## Local

``` text
[ ] Application builds
[ ] Admin login
[ ] Invalid admin login rejected
[ ] Swagger works
[ ] Visitor contact form
[ ] Contact saved
[ ] Contact email sent
[ ] Admin dashboard loads
[ ] Conversation opens
[ ] Close button works
[ ] Admin reply works
[ ] Visitor receives reply
[ ] Visitor can reply
[ ] Resend webhook works
[ ] Visitor reply appears in conversation
[ ] Delete works
```

## Production

``` text
[ ] Render deployment is Live
[ ] Website loads
[ ] Custom domain works
[ ] Admin login works
[ ] Visitor contact works
[ ] Email delivery works
[ ] Admin reply works
[ ] Visitor reply works
[ ] Webhook works
[ ] Conversation history works
[ ] Production CORS works
[ ] Production secrets are environment-based
[ ] Swagger is disabled
```

------------------------------------------------------------------------

# Skills Demonstrated

## Backend

-   C#
-   ASP.NET Core Web API
-   REST API development
-   Dependency Injection
-   Entity Framework Core
-   Database migrations
-   SQLite
-   JWT
-   Authentication
-   Authorization
-   Role-based security
-   Password hashing
-   Configuration
-   Environment variables
-   Webhooks
-   External API integration

## Frontend

-   HTML5
-   CSS3
-   JavaScript
-   Fetch API
-   DOM manipulation
-   Responsive design
-   Forms
-   Modals
-   Dashboard UI
-   API integration

## DevOps / Cloud

-   Git
-   GitHub
-   Render
-   Production deployment
-   Environment configuration
-   Custom domain
-   Deployment troubleshooting
-   Production testing

## Integrations

-   Resend API
-   Email delivery
-   Email replies
-   Webhooks
-   Conversation threading

------------------------------------------------------------------------

# Recommended Learning Order

If revisiting this project to understand how it was built, follow this
order:

``` text
1. HTML / CSS UI
       |
       v
2. JavaScript / Fetch API
       |
       v
3. ASP.NET Core Controllers
       |
       v
4. DTO / Request / Response Models
       |
       v
5. Entity Framework Core
       |
       v
6. SQLite
       |
       v
7. Authentication
       |
       v
8. JWT Authorization
       |
       v
9. Email Service
       |
       v
10. Resend Integration
       |
       v
11. Webhook Integration
       |
       v
12. Conversation System
       |
       v
13. CORS
       |
       v
14. Environment Variables
       |
       v
15. Git / GitHub
       |
       v
16. Render Deployment
```

------------------------------------------------------------------------

# Future Improvements

Possible next improvements:

1.  Migrate SQLite to PostgreSQL.
2.  Add unit tests.
3.  Add integration tests.
4.  Add API rate limiting.
5.  Add structured logging.
6.  Add global exception handling.
7.  Add health checks.
8.  Add webhook signature verification.
9.  Add pagination.
10. Add unread/read conversation status.
11. Add email delivery status tracking.
12. Add database backup strategy.
13. Add CI validation before deployment.
14. Add monitoring and alerts.
15. Add admin password management.
16. Add audit logging.

------------------------------------------------------------------------

# Production Notes

### SQLite

SQLite is currently used because it is lightweight and simple.

For a larger production application or important long-term visitor data,
consider migrating to a managed database such as PostgreSQL.

``` text
SQLite
  |
  v
PostgreSQL
```

### Render Free Instance

On a free/limited Render instance, inactivity can cause the service to
sleep. The first request after inactivity may therefore take longer.

------------------------------------------------------------------------

# Quick Reference

## Technologies

``` text
C#
ASP.NET Core
.NET
Entity Framework Core
SQLite
JWT
HTML
CSS
JavaScript
Resend
Git
GitHub
Render
```

## Controllers

``` text
AuthController
ContactController
ProjectsController
WebhookController
```

## Services

``` text
IEmailService
EmailService
```

## Models

``` text
AdminUser
ContactMessage
ContactRequest
EmailReply
LoginRequest
LoginResponse
Project
```

## Frontend

``` text
index.html
admin.html
```

------------------------------------------------------------------------

# Project Status

Current implementation includes:

-   Public portfolio
-   Admin authentication
-   JWT authorization
-   Visitor contact system
-   Admin Dashboard
-   Project management
-   Conversation modal
-   Admin-to-visitor replies
-   Visitor-to-admin replies
-   Resend email integration
-   Resend webhook integration
-   SQLite database
-   Environment-based secrets
-   Production CORS
-   Render deployment
-   Custom domain
-   Production testing

------------------------------------------------------------------------

# Author

**Manish Kumar**

.NET / Full Stack Developer

Core technologies:

``` text
C#
ASP.NET Core
.NET
Entity Framework Core
REST APIs
JWT
SQLite
HTML
CSS
JavaScript
Git
GitHub
Render
Resend
```

------------------------------------------------------------------------

## End

This README is intended as a long-term technical reference for
understanding, maintaining, deploying and extending the DevPortfolio.API
project.
