DevPortfolio.API — Personal Portfolio & Admin Communication System

DevPortfolio.API is a full-stack personal portfolio application built with ASP.NET Core Web API + C# + Entity Framework Core + SQLite + JWT Authentication + Resend.

It is designed as more than a static portfolio: visitors can submit contact messages, administrators can manage portfolio content and reply to visitors, and visitor replies can be captured back into the application through a Resend webhook.

The application is deployed to Render and connected to a custom domain.

1. Use Case

Problem

A personal portfolio normally provides only static information about a developer.

This project extends that idea into a small production-style application where a visitor can:

Explore the portfolio

View projects

Submit a contact message

Receive an email response

Continue the conversation by replying to email

The administrator can:

Securely log in

View visitor messages

Open complete conversations

Reply directly from the Admin Dashboard

Manage portfolio projects

Receive visitor email replies through a webhook

Example flow:

"A visitor sends a message from my portfolio. I receive the notification, reply from the Admin Dashboard, and if the visitor replies to that email, the reply appears back inside the Admin Dashboard."

2. Why a Backend API?

The portfolio contains data and operations that should not be handled only in browser JavaScript.

The backend is responsible for:

Authentication

Authorization

Database operations

Contact message persistence

Project management

Email delivery

Email reply processing

Webhook handling

Production configuration

The architecture separates the UI from the application logic:

Browser
   ↓
ASP.NET Core REST API
   ↓
Entity Framework Core
   ↓
SQLite

External email communication is handled separately:

ASP.NET Core API
      ↓
   Resend API
      ↓
    Visitor

And inbound visitor replies return through:

Visitor Email
      ↓
    Resend
      ↓
 WebhookController
      ↓
    SQLite
      ↓
 Admin Dashboard

3. Application Architecture

flowchart TD
    Visitor["Visitor<br/>Portfolio Website"]

    Admin["Administrator<br/>Admin Dashboard"]

    API["ASP.NET Core Web API<br/>.NET 8"]

    Auth["AuthController<br/>JWT Authentication"]
    Contact["ContactController<br/>Messages & Replies"]
    Projects["ProjectsController<br/>Project Management"]
    Webhook["WebhookController<br/>Inbound Email"]

    EF["Entity Framework Core"]
    DB[("SQLite<br/>DevPortfolio.db")]

    Email["EmailService"]
    Resend["Resend API"]

    Visitor --> API
    Admin --> API

    API --> Auth
    API --> Contact
    API --> Projects
    API --> Webhook

    Auth --> EF
    Contact --> EF
    Projects --> EF
    Webhook --> EF

    EF --> DB

    Contact --> Email
    Email --> Resend
    Resend --> Visitor

    Visitor -->|Email Reply| Resend
    Resend -->|Webhook| Webhook

4. Main Application Flow

Visitor Contact Flow

Visitor
   ↓
Contact Form
   ↓
POST /api/Contact
   ↓
Validate Request
   ↓
Save ContactMessage
   ↓
Send Email Notification
   ↓
Admin

Admin Reply Flow

Admin Login
   ↓
JWT Token
   ↓
Admin Dashboard
   ↓
Open Conversation
   ↓
Write Reply
   ↓
POST /api/Contact/{id}/reply
   ↓
Save EmailReply
   ↓
Resend API
   ↓
Visitor

Visitor Reply Flow

Visitor replies to email
   ↓
Resend receives email
   ↓
Webhook
   ↓
POST /api/Webhook/...
   ↓
WebhookController
   ↓
Create EmailReply
   ↓
Admin Dashboard

5. Technology Stack

Backend

.NET 8

C#

ASP.NET Core Web API

Entity Framework Core

SQLite

JWT Bearer Authentication

ASP.NET Core Authorization

Dependency Injection

REST APIs

Swagger / OpenAPI

Frontend

HTML5

CSS3

JavaScript

Fetch API

DOM manipulation

Responsive UI

Admin Dashboard

Modal-based conversation UI

Email & Cloud

Resend API

Resend Webhooks

GitHub

Render

Custom Domain

6. Project Structure

portfolio_4you/
│
├── README.md
│
└── DevPortfolio.API/
    │
    ├── wwwroot/
    │   ├── css/
    │   ├── images/
    │   ├── js/
    │   │   └── f.js
    │   ├── Resume/
    │   ├── admin.html
    │   └── index.html
    │
    ├── Controllers/
    │   ├── AuthController.cs
    │   ├── ContactController.cs
    │   ├── ProjectsController.cs
    │   └── WebhookController.cs
    │
    ├── Data/
    │   └── ApplicationDbContext.cs
    │
    ├── Migrations/
    │
    ├── Models/
    │   ├── AdminUser.cs
    │   ├── ContactMessage.cs
    │   ├── ContactRequest.cs
    │   ├── EmailReply.cs
    │   ├── LoginRequest.cs
    │   ├── LoginResponse.cs
    │   └── Project.cs
    │
    ├── services/
    │   ├── EmailService.cs
    │   └── IEmailService.cs
    │
    ├── publish/
    ├── Program.cs
    ├── appsettings.json
    └── DevPortfolio.API.csproj

7. Controllers

AuthController

Responsible for:

Admin login

Credential validation

JWT generation

Authentication-related operations

Authentication flow:

Username + Password
        ↓
   AuthController
        ↓
 Validate AdminUser
        ↓
 Generate JWT
        ↓
 Return LoginResponse

ContactController

Responsible for:

Receiving visitor contact requests

Saving contact messages

Reading conversation data

Sending admin replies

Managing protected conversation operations

ProjectsController

Responsible for portfolio project operations such as:

Reading projects

Adding projects

Editing projects

Deleting projects

WebhookController

Responsible for processing inbound email events from Resend.

Resend
  ↓
WebhookController
  ↓
Validate / Process Event
  ↓
EmailReply
  ↓
Database

This is what allows the portfolio to move beyond one-way email notifications.

8. Data Model

The main application entities are:

AdminUser
ContactMessage
EmailReply
Project

Contact relationship

ContactMessage
      │
      │ 1 : Many
      ▼
 EmailReply

A single visitor conversation can therefore contain multiple replies.

Example:

ContactMessage #1
│
├── Visitor Message
│
├── Admin Reply
│
├── Visitor Reply
│
└── Admin Reply

9. Database

The application currently uses SQLite.

Connection:

Data Source=DevPortfolio.db

Entity Framework Core is used for:

Database access

Entity mapping

Migrations

CRUD operations

Relationships

Main tables / entities

AdminUsers
ContactMessages
EmailReplies
Projects

Local database inspection

The SQLite database can be inspected using:

DB Browser for SQLite

It is useful for:

Viewing tables

Checking records

Inspecting conversations

Running SQL queries

Debugging local data

The local SQLite database and production database should be treated as separate environments.

10. Authentication & Authorization

The Admin Dashboard is protected using JWT Bearer Authentication.

Login

POST /api/Auth/login

The API validates the administrator credentials and returns a JWT.

The frontend then sends the token with protected requests:

Authorization: Bearer <JWT>

Protected administrative operations use authorization rules such as:

[Authorize(Roles = "Admin")]

Security principle

Secrets are not intended to live in source code.

Production secrets are configured through environment variables.

11. Admin User Seeding

The production application can create the initial admin user when no admin exists, using production configuration.

Conceptually:

Environment Variables
        ↓
Admin Username
Admin Password
        ↓
Application Startup
        ↓
Check AdminUser
        ↓
If missing → Create Admin
        ↓
Password stored as hash

The actual production password should never be committed to GitHub.

12. Email Integration with Resend

The project uses Resend for email delivery.

Admin notification

When a visitor submits the contact form:

Visitor
   ↓
ContactController
   ↓
Save message
   ↓
EmailService
   ↓
Resend
   ↓
Admin Email

Admin reply

Admin Dashboard
   ↓
ContactController
   ↓
EmailService
   ↓
Resend
   ↓
Visitor Email

Visitor reply

Visitor
   ↓
Reply to email
   ↓
Resend
   ↓
WebhookController
   ↓
EmailReply
   ↓
Admin Dashboard

13. Two-Way Conversation System

The important feature of this project is that email communication is connected to the database.

Instead of:

Contact Form → Email → END

the application supports:

Visitor
   ↓
Contact Form
   ↓
Database
   ↓
Admin
   ↓
Admin Reply
   ↓
Visitor
   ↓
Visitor Reply
   ↓
Webhook
   ↓
Database
   ↓
Admin Dashboard

This creates a lightweight conversation system inside the portfolio application.

14. CORS

The production API allows requests from the portfolio domains.

Example production origins:

https://manishtechnologysolution.com
https://www.manishtechnologysolution.com

The production configuration should avoid unrestricted CORS such as:

.AllowAnyOrigin()

when a specific frontend origin is known.

15. Configuration & Secrets

Local application configuration can contain non-sensitive defaults.

Sensitive production values should be supplied through environment variables.

Typical configuration sections include:

ConnectionStrings
EmailSettings
Jwt
Admin

Render environment variable naming

ASP.NET Core uses __ to represent nested configuration.

Examples:

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

Never commit

API keys
JWT signing keys
Admin passwords
Email passwords
Access tokens
Private credentials

16. Swagger / OpenAPI

Swagger is available for API development and local testing.

It is useful for testing:

Authentication

Contact APIs

Project APIs

Conversation APIs

Webhook-related endpoints

Typical local development flow:

Run API
   ↓
Open Swagger
   ↓
Authenticate / test endpoints
   ↓
Verify database changes
   ↓
Verify email integration

Swagger should be treated as a development/testing tool and should be configured appropriately for production exposure.

17. Local Development

Prerequisites

Install:

.NET 8 SDK

Visual Studio or VS Code

Git

Modern web browser

Optional: DB Browser for SQLite

Clone repository

git clone https://github.com/ManishAgrawaal/portfolio_4you.git
cd portfolio_4you

Restore packages

dotnet restore

Build

dotnet build

Expected:

Build succeeded

Run

dotnet run --project .\DevPortfolio.API\DevPortfolio.API.csproj

Or run the project through Visual Studio / IIS Express.

18. Local Verification

Verify the complete application locally.

Website

Homepage loads

CSS loads

JavaScript loads

Images load

Projects display

Contact form works

Authentication

Admin login works

Wrong username/password is rejected

JWT is generated

Protected endpoints require authentication

Admin Dashboard

Dashboard loads

Visitor messages appear

Conversation modal opens

Visitor messages are readable

Admin replies can be sent

Projects can be managed

Email

Visitor message reaches admin

Admin reply reaches visitor

Visitor reply reaches webhook

Visitor reply appears in conversation

Database

ContactMessage is stored

EmailReply is stored

Projects are stored

Migrations are applied

19. Render Deployment

Production deployment follows:

Local Development
        ↓
dotnet build
        ↓
git add .
        ↓
git commit
        ↓
git push origin main
        ↓
GitHub
        ↓
Render detects commit
        ↓
Build
        ↓
Deploy
        ↓
Production

Production application

The application is hosted on Render.

Render provides:

Application hosting

Automatic deployment from GitHub

Environment variables

Deployment logs

Production service management

Custom domain

Production domain:

https://manishtechnologysolution.com

Admin:

https://manishtechnologysolution.com/admin.html

20. Render Environment Variables

Configure the sensitive production values in:

Render
 → Service
 → Environment
 → Environment Variables

After modifying environment variables:

Save
   ↓
Rebuild / Deploy
   ↓
Check Logs
   ↓
Test Production

Verify deployment logs

Look for messages indicating:

Application started
Hosting environment: Production
Now listening on: http://0.0.0.0:10000

The exact Render port is controlled by the hosting environment/application configuration.

21. Production Testing Checklist

Before considering the deployment complete:

Render service is Live

Custom domain opens

Homepage loads

Admin login works

Invalid login is rejected

Contact form works

Visitor message is stored

Admin receives email

Admin reply works

Visitor receives reply

Visitor reply webhook works

Conversation updates in dashboard

Projects load

Project CRUD works

CORS works with production domain

Secrets are stored only in Render

GitHub contains no sensitive credentials

22. Git Workflow

Check changes:

git status

Review changes:

git diff

Stage:

git add .

Commit:

git commit -m "Describe the change"

Push:

git push origin main

Verify:

git status

Expected:

Your branch is up to date with 'origin/main'.
nothing to commit, working tree clean

23. Screenshots

Recommended screenshots for the repository:

Portfolio Homepage

docs/screenshots/home.png

Admin Login

docs/screenshots/admin-login.png

Admin Dashboard

docs/screenshots/admin-dashboard.png

Conversation View

docs/screenshots/conversation.png

Swagger

docs/screenshots/swagger.png

Example Markdown:

![Portfolio Homepage](docs/screenshots/home.png)

![Admin Dashboard](docs/screenshots/admin-dashboard.png)

![Conversation](docs/screenshots/conversation.png)

![Swagger](docs/screenshots/swagger.png)

24. Skills Demonstrated

Backend Development

C#

.NET 8

ASP.NET Core Web API

REST API design

Entity Framework Core

SQLite

Database migrations

Dependency Injection

JWT Authentication

Authorization

Password hashing

Configuration management

Webhooks

External API integration

Frontend Development

HTML5

CSS3

JavaScript

Fetch API

DOM manipulation

Responsive design

Forms

Modals

Admin dashboard

API integration

Cloud / DevOps

Git

GitHub

Render

Environment variables

Production deployment

Custom domain configuration

Deployment logs

Production troubleshooting

Third-Party Integration

Resend API

Email delivery

Inbound email webhook

Admin notification

Visitor reply processing

25. What This Project Demonstrates

This project demonstrates the complete lifecycle of a modern small web application:

UI
 ↓
REST API
 ↓
Authentication
 ↓
Business Logic
 ↓
Entity Framework Core
 ↓
SQLite
 ↓
External Email API
 ↓
Webhook
 ↓
Admin Dashboard
 ↓
GitHub
 ↓
Render
 ↓
Custom Domain

It also demonstrates practical production concerns such as:

Secret management

CORS configuration

Authentication

Database migrations

External service integration

Webhook processing

Deployment debugging

Production verification

26. Future Improvements

Possible next steps:

⭐ Migrate SQLite to PostgreSQL for stronger production persistence.

🧪 Add unit and integration tests.

🛡️ Add API rate limiting.

🔏 Add webhook signature verification.

📊 Add structured logging.

❤️ Add health-check monitoring.

📄 Add pagination for large conversation lists.

🔔 Add unread message indicators.

📬 Add email delivery status tracking.

💾 Add database backup strategy.

🔄 Add CI/CD validation.

📈 Add production monitoring.

👤 Add admin profile/password management.

🧾 Add audit logging for administrative actions.

27. Learning / Reference Order

If the project needs to be understood again later, follow this order:

01. wwwroot/index.html
        ↓
02. wwwroot/css + wwwroot/js
        ↓
03. Controllers
        ↓
04. Models
        ↓
05. ApplicationDbContext
        ↓
06. services/EmailService.cs
        ↓
07. JWT Authentication
        ↓
08. Admin Dashboard
        ↓
09. Resend Integration
        ↓
10. WebhookController
        ↓
11. Program.cs
        ↓
12. appsettings / Environment Variables
        ↓
13. GitHub
        ↓
14. Render

This provides a simple path from:

Frontend → API → Database → Authentication → Email → Webhook → Deployment

28. GitHub Repository

Source code:

https://github.com/ManishAgrawaal/portfolio_4you

Live application:

https://manishtechnologysolution.com

29. Author

Manish Kumar

.NET / Full Stack Developer

Core technologies:

C#
.NET
ASP.NET Core
Entity Framework Core
REST APIs
JWT
SQLite
HTML
CSS
JavaScript
Resend
Git
GitHub
Render

<p align="center">

🚀 Built with ASP.NET Core, C# and practical production engineering.

Portfolio • API • Authentication • Database • Email • Webhooks • Deployment

</p
