namespace PlayerWallet.Services.Models.Results
{
    public class WalletActionResult(decimal balance)
    {
        public decimal Balance { get; set; } = balance;
    }
}
