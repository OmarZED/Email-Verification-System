using email_sender.Models;
using email_sender.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace email_sender.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly RabbitMqProducer _rabbitMqProducer;
        private readonly EmailVerificationService _emailVerificationService;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(
            RabbitMqProducer rabbitMqProducer,
            EmailVerificationService emailVerificationService,
            ILogger<RegistrationController> logger)
        {
            _rabbitMqProducer = rabbitMqProducer;
            _emailVerificationService = emailVerificationService;
            _logger = logger;
        }

        [HttpPost("send-code")]
        public async Task<IActionResult> SendVerificationCode([FromBody] SendCodeRequest request)
        {
            try
            {
                // Validate email format
                if (!IsValidEmail(request.Email))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid email format."
                    });
                }

                // Anti-spam check
                if (!_emailVerificationService.CanRequestNewCode(request.Email))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Please wait 1 minute before requesting a new code."
                    });
                }

                // Generate verification code
                var verificationCode = _emailVerificationService.GenerateVerificationCode(request.Email);

                // Create email task for RabbitMQ
                var emailTask = new EmailTask
                {
                    Email = request.Email,
                    Code = verificationCode,
                    Timestamp = DateTime.Now
                };

                // Send to RabbitMQ
                await _rabbitMqProducer.SendMessageAsync(emailTask);

                _logger.LogInformation($"Verification code sent to {request.Email}");

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Verification code sent successfully."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending verification code to {Email}", request.Email);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while sending the verification code."
                });
            }
        }

        [HttpPost("verify-code")]
        public IActionResult VerifyCode([FromBody] VerifyCodeRequest request)
        {
            try
            {
                // Validate input
                if (!IsValidEmail(request.Email) || string.IsNullOrWhiteSpace(request.Code))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid email or code format."
                    });
                }

                // Verify the code
                var result = _emailVerificationService.VerifyCode(request.Email, request.Code);

                if (result.Success)
                {
                    _logger.LogInformation($"Email {request.Email} verified successfully");
                    return Ok(new ApiResponse<object>
                    {
                        Success = true,
                        Message = result.Message
                    });
                }
                else
                {
                    _logger.LogWarning($"Failed verification attempt for {request.Email}: {result.Message}");
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = result.Message
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying code for {Email}", request.Email);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while verifying the code."
                });
            }
        }

        private static bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) &&
                   new EmailAddressAttribute().IsValid(email);
        }
    }
}
