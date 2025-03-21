namespace PlayerWallet.Services.Models.Results
{
    public class ValidationResult(bool isValid, string message = "")
    {
        public bool IsValid { get; set; } = isValid;

        public string Message { get; set; } = message;
    }
}
