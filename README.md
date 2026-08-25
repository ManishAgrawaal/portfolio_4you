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

**Architecture Separation:**
