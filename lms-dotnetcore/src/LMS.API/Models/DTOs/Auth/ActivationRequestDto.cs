namespace LMS.API.Models.DTOs.Auth
{
    public class ActivationRequestDto
    {
        public string ActivationToken { get; set; } = string.Empty;
        public string ActivationCode { get; set; } = string.Empty;
    }
}
