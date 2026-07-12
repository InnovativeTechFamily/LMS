namespace LMS.API.Models.DTOs.Orders
{
    public class OrderResponseDto
    {
        public string? Id { get; set; }
        public string CourseId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public PaymentInfoDto? PaymentInfo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PaymentInfoDto
    {
        public string? Id { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
    }
}
