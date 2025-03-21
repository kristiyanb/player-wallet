using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PlayerWallet.Services;
using PlayerWallet.Services.Models.Configurations;
using PlayerWallet.Services.Contracts;
using PlayerWallet.Services.Models.Results.Base;
using PlayerWallet.Services.Models.Results;

namespace PlayerWallet
{
    public class Program
    {
        public static IServiceProvider serviceProvider;

        public static void Main()
        {
            ConfigureServices();
            Run();
        }

        public static void Run()
        {
            var wallet = serviceProvider.GetRequiredService<IWallet>();

            while (true)
            {
                try
                {
                    Console.WriteLine("Please submit action: ");

                    var input = Console.ReadLine().ToLower().Trim().Split(" ");
                    var action = input[0];

                    if (action == "exit")
                    {
                        break;
                    }

                    var amount = decimal.Parse(input[1]);

                    IResult<WalletActionResult> result = action switch
                    {
                        "deposit" => wallet.Deposit(amount),
                        "withdraw" => wallet.Withdraw(amount),
                        "bet" => wallet.Bet(amount),
                        _ => throw new Exception(),
                    };

                    Console.WriteLine(result.Message);
                    Console.WriteLine();
                }
                catch (Exception)
                {
                    Console.WriteLine();
                    continue;
                }
            }
        }

        public static void ConfigureServices()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();

            services.AddOptions<List<ProbabilityOptions>>().Bind(config.GetSection(nameof(ProbabilityOptions)));
            services.AddOptions<WalletOptions>().Bind(config.GetSection(nameof(WalletOptions)));

            services.AddSingleton<ISlotGame, SlotGame>();
            services.AddSingleton<IWallet, Wallet>();

            serviceProvider = services.BuildServiceProvider();
        }
    }
}
