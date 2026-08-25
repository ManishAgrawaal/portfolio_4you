🚀 DevPortfolio.API

<p align="center">
  <strong>Modern ASP.NET Core Portfolio • Admin Dashboard • Email Conversations • Production Deployment</strong>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-8-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8">
  <img src="https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C#">
  <img src="https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="ASP.NET Core">
  <img src="https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite&logoColor=white" alt="SQLite">
  <img src="https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=jsonwebtokens&logoColor=white" alt="JWT">
  <img src="https://img.shields.io/badge/JavaScript-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black" alt="JavaScript">
  <img src="https://img.shields.io/badge/Resend-000000?style=for-the-badge&logo=resend&logoColor=white" alt="Resend">
  <img src="https://img.shields.io/badge/Render-46E3B7?style=for-the-badge&logo=render&logoColor=black" alt="Render">
</p>

💡 Project Reference: This README documents the architecture, technologies, development workflow, authentication, email integration, deployment and future roadmap of the project.

DevPortfolio.API --- Project Reference

Project Overview

DevPortfolio.API is a full-stack personal portfolio application built
with ASP.NET Core Web API, C#, Entity Framework Core, SQLite, HTML,
CSS and JavaScript.

The application provides:

Public portfolio UI

Visitor contact form

Secure Admin Dashboard

JWT authentication and role-based authorization

SQLite persistence

Resend email integration

Resend webhook integration for visitor replies

Two-way visitor/admin conversation

Project management

Render production deployment

Environment-based production secrets

✨ Key Features

Feature

Description

🔐 Admin Authentication

JWT-based secure administrator login

💬 Visitor Conversations

Complete visitor/admin conversation history

📧 Email Integration

Resend-powered outbound and inbound email

🔄 Webhook Processing

Visitor replies are captured through Resend webhooks

🗃️ Database

Entity Framework Core + SQLite

🎨 Admin UI

Responsive dashboard with conversation modal

🌐 Production

GitHub + Render + custom domain

🛡️ Security

Environment secrets, password hashing and restricted CORS

Technology Stack

Backend

C#

.NET 8 / ASP.NET Core Web API

Entity Framework Core

SQLite

JWT Bearer Authentication

Role-based Authorization

ASP.NET Core PasswordHasher

Swagger / OpenAPI

HttpClient

Dependency Injection

Frontend

HTML5

CSS3

JavaScript

Fetch API

Responsive UI

Admin Dashboard

Conversation Modal

External Services / DevOps

Resend --- email delivery and inbound webhook

GitHub --- source control

Render --- production hosting and deployment

Custom domain --- manishtechnologysolution.com

🛠️ Core Stack

<p>
  <img src="https://img.shields.io/badge/Backend-ASP.NET%20Core-512BD4?style=flat-square&logo=dotnet&logoColor=white">
  <img src="https://img.shields.io/badge/Database-SQLite-003B57?style=flat-square&logo=sqlite&logoColor=white">
  <img src="https://img.shields.io/badge/Auth-JWT-000000?style=flat-square&logo=jsonwebtokens&logoColor=white">
  <img src="https://img.shields.io/badge/Frontend-HTML%20%7C%20CSS%20%7C%20JS-E34F26?style=flat-square&logo=html5&logoColor=white">
  <img src="https://img.shields.io/badge/Email-Resend-000000?style=flat-square">
  <img src="https://img.shields.io/badge/Hosting-Render-46E3B7?style=flat-square&logo=render&logoColor=black">
  <img src="https://img.shields.io/badge/Source-GitHub-181717?style=flat-square&logo=github&logoColor=white">
</p>

🏗️ Architecture

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

📁 Project Structure

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

Folder Responsibilities

Controllers

AuthController.cs

Handles:

Admin login

Credential validation

JWT generation

Authentication-related operations

ContactController.cs

Handles:

Visitor contact messages

Admin message management

Conversation history

Admin replies

Protected contact operations

Important conversation endpoint:

GET /api/Contact/{id}/replies

Admin reply endpoint:

POST /api/Contact/{id}/reply

ProjectsController.cs

Handles portfolio project operations.

WebhookController.cs

Handles:

Resend webhook events

Visitor email replies

Matching replies to existing conversations

Saving inbound replies

Data Layer

ApplicationDbContext.cs

Entity Framework Core database context.

Main entities:

AdminUsers
ContactMessages
EmailReplies
Projects

Migrations are used to maintain the SQLite database schema.

Models

AdminUser.cs

Represents the administrator.

Important properties:

Username
PasswordHash
Role
IsActive

Passwords are stored as hashes, not plaintext.

ContactMessage.cs

Represents a visitor's original contact message.

Typical data:

Id
Name
Email
Subject
Message
CreatedAt

EmailReply.cs

Stores inbound/outbound conversation reply information.

Typical data:

ContactMessageId
FromEmail
ToEmail
Subject
Message
ReceivedAt
ResendEmailId
MessageId

ContactRequest.cs

Request DTO used by the visitor contact form.

LoginRequest.cs

Request DTO used for administrator login.

LoginResponse.cs

Response DTO returned after successful authentication.

Project.cs

Represents portfolio project data.

Services

IEmailService.cs

Defines the email-service contract.

The controller depends on the abstraction rather than directly depending
on the provider.

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

EmailService.cs

Responsible for:

Sending emails

Admin-to-visitor replies

Email configuration

Reply context

Resend API communication

🗄️ Database

Current database:

SQLite

Connection:

Data Source=DevPortfolio.db

Entity Framework Core is used for database operations.

At startup, migrations are applied with:

db.Database.Migrate();

Useful tables:

AdminUsers
ContactMessages
EmailReplies
Projects

Database Relationship

The conversation model is:

ContactMessage
      |
      | 1 : Many
      v
EmailReply

Example:

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

This allows the Admin Dashboard to display the full conversation.

🔐 Authentication and Authorization

The admin system uses:

JWT Bearer Authentication

Flow:

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

Protected APIs use:

[Authorize(Roles = "Admin")]

Admin User Provisioning

The production application seeds the initial admin during startup when
the account does not already exist.

Configuration:

Admin:Username
Admin:Password

Render environment-variable names:

Admin__Username
Admin__Password

Logic:

Admin exists?
   |
   +-- YES --> keep existing admin
   |
   +-- NO ---> create admin
                 |
                 +--> hash password
                 |
                 +--> save AdminUser

The password is hashed using ASP.NET Core's PasswordHasher<AdminUser>.

Public UI

Main page:

wwwroot/index.html

Features:

Portfolio introduction

Projects

Technologies

Contact form

Responsive UI

Contact flow:

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

Admin Dashboard

Main page:

wwwroot/admin.html

Features:

Admin login

Visitor message list

Message statistics

View conversation

Delete message

Refresh messages

Admin reply

Conversation modal

Conversation UI displays:

Visitor email

Admin messages

Visitor messages

Subject

Date/time

Reply textarea

Close button

Send Reply button

Admin → Visitor Email Flow

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

Visitor → Admin Email Flow

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

This creates a two-way communication system.

📧 Resend Integration

Resend is used for outbound email and inbound webhook processing.

Outbound

ASP.NET Core
     |
     v
Resend API
     |
     v
Visitor / recipient

Inbound

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

Configuration and Secrets

Production secrets must not be committed to GitHub.

Examples of sensitive values:

Resend API key
JWT signing key
Admin password
SMTP credentials
Other access tokens

Use environment variables / local secret storage instead.

Render Environment Variables

Nested ASP.NET Core configuration uses double underscores:

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

Do not publish the actual values.

CORS

Production CORS is restricted to the portfolio domains:

https://manishtechnologysolution.com
https://www.manishtechnologysolution.com

Avoid unrestricted production CORS such as:

.AllowAnyOrigin()

Swagger

Swagger/OpenAPI is intended for development and local API testing.

Development -> Swagger enabled
Production  -> Swagger disabled

This keeps local development convenient while reducing unnecessary
production exposure.

💻 Local Development

Prerequisites

.NET SDK

Visual Studio or VS Code

Git

Browser

Optional: DB Browser for SQLite

Step 1 --- Clone

git clone <repository-url>
cd portfolio_4you

Step 2 --- Open

Open the solution in Visual Studio:

DevPortfolio.API.sln

Step 3 --- Restore

dotnet restore

Step 4 --- Build

dotnet build

Expected:

Build succeeded

Step 5 --- Run

Use Visual Studio / IIS Express or:

dotnet run

Step 6 --- Test

Use the local application and Swagger while running in Development.

SQLite Database Viewer

The local database file is:

DevPortfolio.db

A convenient GUI application for inspecting it is:

DB Browser for SQLite

Use it to:

Open the database

Browse tables

Inspect records

Run SQL queries

Check migrations/data

Important:

Local DevPortfolio.db

and the production database are separate environments.

Git Workflow

Check status:

git status

Review changes:

git diff

Stage:

git add .

Commit:

git commit -m "Describe the change"

Push:

git push origin main

☁️ Render Deployment

Production hosting is handled by Render.

Deployment flow:

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

After deployment:

Render
  -> Events / Deployments
  -> Confirm status = Live

Production Domain

Website:

https://manishtechnologysolution.com

Admin:

https://manishtechnologysolution.com/admin.html

🧪 Testing Checklist

Local

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

Production

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

🧠 Skills Demonstrated

Backend

C#

ASP.NET Core Web API

REST API development

Dependency Injection

Entity Framework Core

Database migrations

SQLite

JWT

Authentication

Authorization

Role-based security

Password hashing

Configuration

Environment variables

Webhooks

External API integration

Frontend

HTML5

CSS3

JavaScript

Fetch API

DOM manipulation

Responsive design

Forms

Modals

Dashboard UI

API integration

DevOps / Cloud

Git

GitHub

Render

Production deployment

Environment configuration

Custom domain

Deployment troubleshooting

Production testing

Integrations

Resend API

Email delivery

Email replies

Webhooks

Conversation threading

Recommended Learning Order

If revisiting this project to understand how it was built, follow this
order:

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

🔮 Future Improvements

Possible next improvements:

Migrate SQLite to PostgreSQL.

Add unit tests.

Add integration tests.

Add API rate limiting.

Add structured logging.

Add global exception handling.

Add health checks.

Add webhook signature verification.

Add pagination.

Add unread/read conversation status.

Add email delivery status tracking.

Add database backup strategy.

Add CI validation before deployment.

Add monitoring and alerts.

Add admin password management.

Add audit logging.

Production Notes

SQLite

SQLite is currently used because it is lightweight and simple.

For a larger production application or important long-term visitor data,
consider migrating to a managed database such as PostgreSQL.

SQLite
  |
  v
PostgreSQL

Render Free Instance

On a free/limited Render instance, inactivity can cause the service to
sleep. The first request after inactivity may therefore take longer.

Quick Reference

Technologies

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

Controllers

AuthController
ContactController
ProjectsController
WebhookController

Services

IEmailService
EmailService

Models

AdminUser
ContactMessage
ContactRequest
EmailReply
LoginRequest
LoginResponse
Project

Frontend

index.html
admin.html

Project Status

Current implementation includes:

Public portfolio

Admin authentication

JWT authorization

Visitor contact system

Admin Dashboard

Project management

Conversation modal

Admin-to-visitor replies

Visitor-to-admin replies

Resend email integration

Resend webhook integration

SQLite database

Environment-based secrets

Production CORS

Render deployment

Custom domain

Production testing

👨‍💻 Author

Manish Kumar

.NET / Full Stack Developer

Core technologies:

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

End

This README is intended as a long-term technical reference for
understanding, maintaining, deploying and extending the DevPortfolio.API
project.
