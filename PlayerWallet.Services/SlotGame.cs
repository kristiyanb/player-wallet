using Microsoft.Extensions.Options;

using PlayerWallet.Services.Models.Configurations;
using PlayerWallet.Services.Contracts;

namespace PlayerWallet.Services
{
    public class SlotGame : ISlotGame
    {
        private readonly List<ProbabilityOptions> probabilities;
        private readonly Random random;
        private readonly int total;

        public SlotGame(IOptions<List<ProbabilityOptions>> probabilityOptions)
        {
            this.probabilities = probabilityOptions.Value;
            random = new();

            total = this.probabilities.Sum(x => x.Weight);
            if (total != 100)
            {
                throw new Exception("Invalid probabilities configuration.");
            }
        }

        public decimal Spin(decimal amount)
        {
            var roll = random.Next(0, total);
            var winnings = 0M;

            foreach (var probability in probabilities)
            {
                if ((roll -= probability.Weight) < 0)
                {
                    winnings = this.CalculateWinnings(amount, probability.MinMultiplierPercentage, probability.MaxMultiplierPercentage);
                    break;
                }
            }

            return winnings;
        }

        private decimal CalculateWinnings(decimal amount, int minMultiplierPercentage, int maxMultiplierPercentage)
        {
            if (maxMultiplierPercentage == 0)
            {
                return 0;
            }

            var multiplierPercentage = random.Next(minMultiplierPercentage, maxMultiplierPercentage);

            return amount * (multiplierPercentage / 100M);
        }
    }
}
