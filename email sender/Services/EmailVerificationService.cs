using System.Collections.Concurrent;

namespace email_sender.Services
{
    public class EmailVerificationService
    {
        private readonly ConcurrentDictionary<string, VerificationData> _verificationCodes;
        private readonly Random _random;

        public EmailVerificationService()
        {
            _verificationCodes = new ConcurrentDictionary<string, VerificationData>();
            _random = new Random();
        }

        public string GenerateVerificationCode(string email)
        {
            // Generate 4-digit code
            var code = _random.Next(1000, 9999).ToString();

            var verificationData = new VerificationData
            {
                Code = code,
                Email = email,
                CreatedAt = DateTime.Now,
                ExpiresAt = DateTime.Now.AddMinutes(10), // Code expires in 10 minutes
                AttemptCount = 0
            };

            _verificationCodes.AddOrUpdate(email, verificationData, (key, oldValue) => verificationData);

            return code;
        }

        public VerificationResult VerifyCode(string email, string code)
        {
            if (!_verificationCodes.TryGetValue(email, out var verificationData))
            {
                return new VerificationResult { Success = false, Message = "No verification code found for this email." };
            }

            // Check if code expired
            if (DateTime.Now > verificationData.ExpiresAt)
            {
                _verificationCodes.TryRemove(email, out _);
                return new VerificationResult { Success = false, Message = "Verification code has expired." };
            }

            // Increment attempt count
            verificationData.AttemptCount++;

            // Check max attempts (prevent brute force)
            if (verificationData.AttemptCount > 3)
            {
                _verificationCodes.TryRemove(email, out _);
                return new VerificationResult { Success = false, Message = "Too many failed attempts. Please request a new code." };
            }

            // Check if code matches
            if (verificationData.Code != code)
            {
                return new VerificationResult { Success = false, Message = "Invalid verification code." };
            }

            // Success - remove the code so it can't be used again
            _verificationCodes.TryRemove(email, out _);
            return new VerificationResult { Success = true, Message = "Email verified successfully!" };
        }

        public bool HasPendingVerification(string email)
        {
            if (!_verificationCodes.TryGetValue(email, out var verificationData))
                return false;

            return DateTime.Now <= verificationData.ExpiresAt;
        }

        // Anti-spam: Check if user can request new code (1 minute cooldown)
        public bool CanRequestNewCode(string email)
        {
            if (!_verificationCodes.TryGetValue(email, out var verificationData))
                return true;

            return DateTime.Now >= verificationData.CreatedAt.AddMinutes(1);
        }
    }

    public class VerificationData
    {
        public string Code { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int AttemptCount { get; set; }
    }

    public class VerificationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
