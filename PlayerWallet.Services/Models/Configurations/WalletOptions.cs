namespace PlayerWallet.Services.Models.Configurations
{
    public record WalletOptions
    {
        public int MinBetSize { get; set; }

        public int MaxBetSize { get; set; }
    }
}
