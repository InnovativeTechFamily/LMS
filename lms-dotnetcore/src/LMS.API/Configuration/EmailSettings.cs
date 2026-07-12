namespace LMS.API.Configuration
{
    public class EmailSettings
    {
        public string SendGridKey { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
    }
}
