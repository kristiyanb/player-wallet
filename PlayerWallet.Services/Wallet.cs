using Microsoft.Extensions.Options;

using PlayerWallet.Services.Models.Configurations;
using PlayerWallet.Services.Contracts;
using PlayerWallet.Services.Models.Results;
using PlayerWallet.Services.Models.Results.Base;

namespace PlayerWallet.Services
{
    public class Wallet : IWallet
    {
        private decimal balance;
        private readonly int minBetSize;
        private readonly int maxBetSize;
        private readonly ISlotGame slotGame;

        public Wallet(IOptions<WalletOptions> walletOptions, ISlotGame slotGame)
        {
            minBetSize = walletOptions.Value.MinBetSize;
            maxBetSize = walletOptions.Value.MaxBetSize;
            this.slotGame = slotGame;
        }

        public IResult<WalletActionResult> Deposit(decimal amount)
        {
            var amountValidationResult = ValidateAmount(amount);
            if (!amountValidationResult.IsValid)
            {
                return Result<WalletActionResult>.Error(amountValidationResult.Message);
            }

            balance += amount;

            return Result<WalletActionResult>.Success(
                new(balance),
                $"Your deposit of ${amount:F2} was successful. Your current balance is: ${balance:F2}");
        }

        public IResult<WalletActionResult> Bet(decimal amount)
        {
            var amountValidationResult = ValidateAmount(amount);
            if (!amountValidationResult.IsValid)
            {
                return Result<WalletActionResult>.Error(amountValidationResult.Message);
            }

            var balanceValidationResult = ValidateSufficientBalance(amount);
            if (!balanceValidationResult.IsValid)
            {
                return Result<WalletActionResult>.Error(balanceValidationResult.Message);
            }

            var betSizeValidationResult = ValidateBetSize(amount);
            if (!betSizeValidationResult.IsValid)
            {
                return Result<WalletActionResult>.Error(betSizeValidationResult.Message);
            }

            var winnings = slotGame.Spin(amount);
            balance = balance - amount + winnings;

            var resultMessage = winnings == 0
                ? $"No luck this time! Your current balance is: ${balance:F2}"
                : $"Congrats - you won ${winnings:F2}! Your current balance is: ${balance:F2}";

            return Result<WalletActionResult>.Success(new(balance), resultMessage);
        }

        public IResult<WalletActionResult> Withdraw(decimal amount)
        {
            var amountValidationResult = ValidateAmount(amount);
            if (!amountValidationResult.IsValid)
            {
                return Result<WalletActionResult>.Error(amountValidationResult.Message);
            }

            var balanceValidationResult = ValidateSufficientBalance(amount);
            if (!balanceValidationResult.IsValid)
            {
                return Result<WalletActionResult>.Error(balanceValidationResult.Message);
            }

            balance -= amount;

            return Result<WalletActionResult>.Success(
                new(balance),
                $"Your withdrawal of ${amount:F2} was successful. Your current balance is: ${balance:F2}");
        }

        private ValidationResult ValidateAmount(decimal amount)
        {
            if (amount <= 0)
            {
                return new(false, "Amount must be a positive number.");
            }

            return new(true);
        }

        private ValidationResult ValidateSufficientBalance(decimal amount)
        {
            if (amount > balance)
            {
                return new(false, "Insufficient balance.");
            }

            return new(true);
        }

        private ValidationResult ValidateBetSize(decimal amount)
        {
            if (amount < minBetSize || amount > maxBetSize)
            {
                return new(false, $"Game accepts bets between ${minBetSize:F2} and ${maxBetSize:F2}");
            }

            return new(true);
        }
    }
}
