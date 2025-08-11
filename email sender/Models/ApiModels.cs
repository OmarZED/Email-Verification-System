using System.ComponentModel.DataAnnotations;

namespace email_sender.Models
{
    public class SendCodeRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class VerifyCodeRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(4, MinimumLength = 4)]
        public string Code { get; set; } = string.Empty;
    }

    // Response DTOs
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }

    public class VerificationStatus
    {
        public bool HasPendingVerification { get; set; }
        public bool CanRequestNewCode { get; set; }
    }

    // RabbitMQ Message Model
    public class EmailTask
    {
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
