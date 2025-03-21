using PlayerWallet.Services.Models.Results;
using PlayerWallet.Services.Models.Results.Base;

namespace PlayerWallet.Services.Contracts
{
    public interface IWallet
    {
        public IResult<WalletActionResult> Deposit(decimal amount);

        public IResult<WalletActionResult> Bet(decimal amount);

        public IResult<WalletActionResult> Withdraw(decimal amount);
    }
}
