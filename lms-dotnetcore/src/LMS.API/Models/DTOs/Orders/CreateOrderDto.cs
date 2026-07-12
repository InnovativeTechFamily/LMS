namespace LMS.API.Models.DTOs.Orders
{
    public class CreateOrderDto
    {
        public string CourseId { get; set; } = string.Empty;
        public PaymentInfoDto? PaymentInfo { get; set; }
    }

  
}
