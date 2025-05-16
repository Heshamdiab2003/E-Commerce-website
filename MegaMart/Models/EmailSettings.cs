namespace MegaMart.Models
{
    public class EmailSettings
    {
        public string SmtpHost { get; set; }  // بدلاً من SmtpServer
        public int SmtpPort { get; set; }     // بدلاً من Port
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
        public bool EnableSsl { get; set; }
    }
}
