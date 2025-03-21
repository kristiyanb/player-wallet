using Microsoft.Extensions.Options;
using Moq;

using PlayerWallet.Services;
using PlayerWallet.Services.Contracts;
using PlayerWallet.Services.Models.Configurations;
using PlayerWallet.Services.Models.Results;

namespace PlayerWallet.Tests
{
    public class WalletTests
    {
        private Mock<ISlotGame> slotGameMock = new();
        private IOptions<WalletOptions> walletOptions;

        [SetUp]
        public void Setup()
        {
            walletOptions = Options.Create(new WalletOptions()
            {
                MinBetSize = 1,
                MaxBetSize = 10,
            });
        }

        [Test]
        public void Deposit()
        {
            var wallet = new Wallet(walletOptions, slotGameMock.Object);

            var result = wallet.Deposit(10);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<WalletActionResult>>());
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result?.Data?.Balance, Is.EqualTo(10));
                Assert.That(result?.Message, Is.EqualTo("Your deposit of $10.00 was successful. Your current balance is: $10.00"));
            });
        }

        [Test]
        public void Deposit_NegativeAmount()
        {
            var wallet = new Wallet(walletOptions, slotGameMock.Object);

            var result = wallet.Deposit(-10);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<WalletActionResult>>());
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result?.Message, Is.EqualTo("Amount must be a positive number."));
            });
        }

        [Test]
        public void Withdraw()
        {
            var wallet = new Wallet(walletOptions, slotGameMock.Object);

            wallet.Deposit(10);
            var result = wallet.Withdraw(5);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<WalletActionResult>>());
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result?.Data?.Balance, Is.EqualTo(5));
                Assert.That(result?.Message, Is.EqualTo("Your withdrawal of $5.00 was successful. Your current balance is: $5.00"));
            });
        }

        [Test]
        public void Withdraw_NegativeAmount()
        {
            var wallet = new Wallet(walletOptions, slotGameMock.Object);

            var result = wallet.Withdraw(-10);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<WalletActionResult>>());
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result?.Message, Is.EqualTo("Amount must be a positive number."));
            });
        }

        [Test]
        public void Withdraw_InsufficientBalance()
        {
            var wallet = new Wallet(walletOptions, slotGameMock.Object);

            wallet.Deposit(10);
            var result = wallet.Withdraw(20);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<WalletActionResult>>());
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result?.Message, Is.EqualTo("Insufficient balance."));
            });
        }

        [Test]
        public void Bet_Win()
        {
            slotGameMock.Setup(x => x.Spin(It.IsAny<decimal>())).Returns(25);
            var wallet = new Wallet(walletOptions, slotGameMock.Object);

            wallet.Deposit(10);
            var result = wallet.Bet(5);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<WalletActionResult>>());
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result?.Data?.Balance, Is.EqualTo(30));
                Assert.That(result?.Message, Is.EqualTo("Congrats - you won $25.00! Your current balance is: $30.00"));
            });
        }

        [Test]
        public void Bet_Loss()
        {
            slotGameMock.Setup(x => x.Spin(It.IsAny<decimal>())).Returns(0);
            var wallet = new Wallet(walletOptions, slotGameMock.Object);

            wallet.Deposit(10);
            var result = wallet.Bet(5);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<WalletActionResult>>());
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result?.Data?.Balance, Is.EqualTo(5));
                Assert.That(result?.Message, Is.EqualTo("No luck this time! Your current balance is: $5.00"));
            });
        }

        [Test]
        public void Bet_NegativeAmount()
        {
            var wallet = new Wallet(walletOptions, slotGameMock.Object);

            var result = wallet.Bet(-10); 
            
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<WalletActionResult>>());
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result?.Message, Is.EqualTo("Amount must be a positive number."));
            });
        }

        [Test]
        public void Bet_InsufficientBalance()
        {
            var wallet = new Wallet(walletOptions, slotGameMock.Object);

            wallet.Deposit(5);
            var result = wallet.Bet(10);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<WalletActionResult>>());
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result?.Message, Is.EqualTo("Insufficient balance."));
            });
        }

        [Test]
        public void Bet_MinBetSize()
        {
            var wallet = new Wallet(walletOptions, slotGameMock.Object);

            wallet.Deposit(10);
            var result = wallet.Bet(0.5M);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<WalletActionResult>>());
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result?.Message, Is.EqualTo("Game accepts bets between $1.00 and $10.00"));
            });
        }

        [Test]
        public void Bet_MaxBetSize()
        {
            var wallet = new Wallet(walletOptions, slotGameMock.Object);

            wallet.Deposit(20);
            var result = wallet.Bet(11);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<Result<WalletActionResult>>());
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result?.Message, Is.EqualTo("Game accepts bets between $1.00 and $10.00"));
            });
        }
    }
}