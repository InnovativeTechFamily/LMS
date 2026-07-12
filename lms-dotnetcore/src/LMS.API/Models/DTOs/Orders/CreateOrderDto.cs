namespace LMS.API.Models.DTOs.Orders
{
    public class CreateOrderDto
    {
        public string CourseId { get; set; } = string.Empty;
        public PaymentInfoDto? PaymentInfo { get; set; }
    }

    public class PaymentInfoDto
    {
        public string? Id { get; set; }
        public string? Status { get; set; }
        public string? Type { get; set; }
    }
}
