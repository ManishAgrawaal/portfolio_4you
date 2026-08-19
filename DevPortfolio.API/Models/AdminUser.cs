namespace DevPortfolio.API.Models
{
    public class AdminUser
    {
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Admin";

        public bool IsActive { get; set; } = true;
    }
}