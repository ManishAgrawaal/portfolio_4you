using Microsoft.EntityFrameworkCore;
using DevPortfolio.API.Models;

namespace DevPortfolio.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<EmailReply> EmailReplies { get; set; }
    }
}