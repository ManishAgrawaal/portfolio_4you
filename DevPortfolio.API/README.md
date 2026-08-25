<div align="center">

<h1>
  <a href="https://github.com/ManishAgrawaal/portfolio_4you" style="text-decoration: none; color: inherit;">
    🚀 DevPortfolio.API
  </a>
</h1>

**Personal Portfolio & Admin Communication System**

<p align="center">
  DevPortfolio.API is a full-stack personal portfolio application built with <br>
  <b>ASP.NET Core Web API + C# + Entity Framework Core + SQLite + JWT Authentication + Resend.</b>
</p>

[![Deployed on Render](https://img.shields.io/badge/Deployed-Render-46E3B7?style=for-the-badge&logo=render)](#)-
[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)](#)-
[![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite)](#)

</div>

---

It is designed as more than a static portfolio: visitors can submit contact messages, administrators can manage portfolio content and reply to visitors, and visitor replies can be captured back into the application through a Resend webhook.

The application is deployed to Render and connected to a custom domain.

---

## 1. Use Case

### Problem
A personal portfolio normally provides only static information about a developer. This project extends that idea into a small production-style application.

### The Solution
**A visitor can:**
- 🔍 Explore the portfolio
- 💻 View projects
- ✉️ Submit a contact message
- 📥 Receive an email response
- 🔄 Continue the conversation by replying to email

**The administrator can:**
- 🔒 Securely log in
- 📬 View visitor messages
- 📖 Open complete conversations
- ✍️ Reply directly from the Admin Dashboard
- 🛠️ Manage portfolio projects
- 🪝 Receive visitor email replies through a webhook

> **Example flow:** 
> *"A visitor sends a message from my portfolio. I receive the notification, reply from the Admin Dashboard, and if the visitor replies to that email, the reply appears back inside the Admin Dashboard."*

## 2. Why a Backend API?

The portfolio contains data and operations that should not be handled only in browser JavaScript. The backend is responsible for:

- Authentication & Authorization
- Database operations
- Contact message persistence
- Project management
- Email delivery & reply processing
- Webhook handling
- Production configuration
## 3. Architecture
  
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

## 4. Main Application Flow

<details>
<summary><b>👤 Visitor Contact Flow</b></summary>
</details>

<details>
<summary><b>🛡️ Admin Reply Flow</b></summary>
</details>

<details>
<summary><b>🔄 Visitor Reply Flow</b></summary>
</details>

## 5. Technology Stack

### Backend
- **.NET 8 & C#** (ASP.NET Core Web API)
- **Entity Framework Core**
- **SQLite**
- **JWT Bearer Authentication**
- **Dependency Injection** & **REST APIs**
- **Swagger / OpenAPI**

### Frontend
- **HTML5, CSS3, JavaScript**
- **Fetch API** & **DOM manipulation**
- **Responsive UI**
- **Admin Dashboard** (Modal-based conversation UI)

### Email & Cloud
- **Resend API** & **Resend Webhooks**
- **GitHub** (Version Control)
- **Render** (Hosting & Custom Domain)

## 6. Project Structure

## 7. Controllers

| Controller | Responsibility | Flow |
|---|---|---|
| `AuthController` | Admin login, Credential validation, JWT generation. | `User+Pass ➔ Validate ➔ Generate JWT` |
| `ContactController` | Receiving contacts, saving messages, reading convos, sending replies. | `Save message ➔ EmailService ➔ DB` |
| `ProjectsController`| Reading, adding, editing, and deleting portfolio projects. | CRUD Operations |
| `WebhookController` | Processing inbound email events from Resend (Two-way comms). | `Resend ➔ Validate ➔ Save Reply` |

## 8. Data Model

The main application entities are:
- `AdminUser`
- `ContactMessage`
- `EmailReply`
- `Project`

**Contact Relationship:**

*A single visitor conversation can therefore contain multiple replies (Visitor Msg ➔ Admin Reply ➔ Visitor Reply).*

## 9. Database

The application currently uses **SQLite**.
- **Connection:** `Data Source=DevPortfolio.db`
- **ORM:** Entity Framework Core (Database access, entity mapping, migrations, CRUD)

> **Local database inspection:** Use *DB Browser for SQLite* for viewing tables, checking records, inspecting conversations, and debugging. *Note: The local SQLite database and production database should be treated as separate environments.*

## 10. Authentication & Authorization

The Admin Dashboard is protected using **JWT Bearer Authentication**.

1. **Login:** `POST /api/Auth/login` validates credentials and returns a JWT.
2. **Protected Requests:** Frontend sends the token in the header:
3. **Authorization:** Endpoints use rules such as `[Authorize(Roles = "Admin")]`.

*Security principle: Secrets are not intended to live in source code. Production secrets are configured through environment variables.*

## 11. Admin User Seeding

The production application can create the initial admin user when no admin exists, using production configuration.

*The actual production password should never be committed to GitHub.*

## 12. Email Integration with Resend

The project uses Resend for email delivery.

- **Admin Notification:** `Visitor ➔ ContactController ➔ EmailService ➔ Resend ➔ Admin`
- **Admin Reply:** `Admin Dashboard ➔ ContactController ➔ EmailService ➔ Resend ➔ Visitor`
- **Visitor Reply:** `Visitor ➔ Resend ➔ WebhookController ➔ DB ➔ Admin Dashboard`

## 13. Two-Way Conversation System

The most important feature of this project is that email communication is connected to the database. Instead of a dead-end contact form, the application supports a lightweight conversation system:


## 14. CORS

The production API allows requests from the portfolio domains:
- `https://manishtechnologysolution.com`
- `https://www.manishtechnologysolution.com`

*The production configuration avoids unrestricted CORS (e.g., `.AllowAnyOrigin()`) when a specific frontend origin is known.*

## 15. Configuration & Secrets

Local application configuration can contain non-sensitive defaults. Sensitive production values should be supplied through environment variables.

**ASP.NET Core uses `__` to represent nested configuration:**
- `ConnectionStrings__DefaultConnection`
- `EmailSettings__ApiKey`
- `Jwt__Key`
- `Admin__Password`

⚠️ **Never commit:** API keys, JWT signing keys, Passwords, or Access tokens.

## 16. Swagger / OpenAPI

Swagger is available for API development and local testing.
- **Flow:** `Run API ➔ Open Swagger ➔ Authenticate ➔ Test Endpoints ➔ Verify DB/Emails`
- *Swagger should be treated as a development/testing tool and disabled/secured in production.*

## 17. Local Development

**Prerequisites:**
- .NET 8 SDK
- Visual Studio or VS Code
- Git

**Setup:**

## 18. Local Verification

- [ ] **Website:** Homepage, CSS, JS, Images load. Contact form works.
- [ ] **Authentication:** Admin login works. Invalid credentials rejected. JWT generated.
- [ ] **Admin Dashboard:** Messages appear. Conversation modal opens. Replies can be sent. Projects managed.
- [ ] **Email:** Visitor message reaches admin. Admin reply reaches visitor. Webhook catches visitor reply.
- [ ] **Database:** Messages, Replies, and Projects stored. Migrations applied.

## 19. Render Deployment

The application is hosted on Render, which provides auto-deployments, environment variables, logs, and custom domain routing.
- **Production domain:** `https://manishtechnologysolution.com`
- **Admin:** `https://manishtechnologysolution.com/admin.html`

## 20. Render Environment Variables

Configure sensitive values in: `Render → Service → Environment → Environment Variables`.
*After modifying: Save ➔ Rebuild/Deploy ➔ Check Logs ➔ Test.*

## 21. Production Testing Checklist

- [ ] Render service is Live
- [ ] Custom domain opens successfully
- [ ] Admin login works (invalid is rejected)
- [ ] Contact form works & Visitor message is stored
- [ ] Emails successfully sent and received (both directions)
- [ ] Webhook triggers correctly on visitor reply
- [ ] Project CRUD works
- [ ] CORS is restricted to production domains
- [ ] Secrets are stored **only** in Render, not GitHub

## 22. Git Workflow

## 23. Screenshots
<p align="center">
  <b>Portfolio Homepage</b> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <b>Admin Login</b> <br/>
  <img width="48%" height="250" alt="homepage" src="https://github.com/user-attachments/assets/3f5c893d-17d3-4acc-a175-60997b2fce49" />
  <img width="48%" height="250" alt="adminlogin" src="https://github.com/user-attachments/assets/43ae3a97-ead5-4c38-b981-55f8243670cf" />
</p>

<p align="center">
  <b>Admin Dashboard</b> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <b>Conversation</b> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <b>Swagger</b> <br/>
  <img width="32%" height="250" alt="adminview" src="https://github.com/user-attachments/assets/587ce65d-2300-488b-bb61-37c8cde3aebd" />
  <img width="32%" height="250" alt="contactview" src="https://github.com/user-attachments/assets/29d4cc52-7612-4d1d-95b8-13d9c2375b6e" />
  <img width="32%" height="250" alt="swagger" src="https://github.com/user-attachments/assets/8092b451-fa77-4c79-8f4f-f5a20b6c4816" />
</p>


## 24. Skills Demonstrated

- **Backend:** C#, .NET 8, Web API, REST API design, EF Core, SQLite, Migrations, DI, JWT, Password Hashing.
- **Frontend:** HTML5, CSS3, JavaScript, Fetch API, DOM manipulation, Responsive design, Modals.
- **Cloud/DevOps:** Git/GitHub, Render, Environment variables, Custom domains, Production logging.
- **Integrations:** Resend API (Email delivery), Webhooks.

## 25. What This Project Demonstrates

This project demonstrates the complete lifecycle of a modern small web application:


## 26. Future Improvements

- ⭐ Migrate SQLite to PostgreSQL for stronger production persistence.
- 🧪 Add unit and integration tests.
- 🛡️ Add API rate limiting.
- 🔏 Add webhook signature verification.
- 📊 Add structured logging.
- ❤️ Add health-check monitoring.
- 📄 Add pagination for large conversation lists.
- 🔔 Add unread message indicators.
- 📬 Add email delivery status tracking.

## 27. Learning / Reference Order

To understand the codebase, review the files in this order:
1. `wwwroot/index.html`
2. `wwwroot/css` + `wwwroot/js`
3. `Controllers`
4. `Models`
5. `ApplicationDbContext`
6. `services/EmailService.cs`
7. JWT Authentication logic
8. Admin Dashboard
9. Resend Integration
10. `WebhookController`
11. `Program.cs`
12. `appsettings` / Environment Variables
13. GitHub Actions/Commits
14. Render Settings

## 28. Links

- **Source Code:** [GitHub Repository](https://github.com/ManishAgrawaal/portfolio_4you)
- **Live Application:** [manishtechnologysolution.com](https://manishtechnologysolution.com)

## 29. Author

**Manish Kumar**  
*.NET / Full Stack Developer*  
Core technologies: *C#, .NET, ASP.NET Core, EF Core, REST APIs, JWT, SQLite, HTML, CSS, JavaScript, Resend, Git, GitHub, Render*

---

<p align="center">
  🚀 <b>Built with ASP.NET Core, C# and practical production engineering.</b><br>
  <i>Portfolio • API • Authentication • Database • Email • Webhooks • Deployment</i>
</p>

