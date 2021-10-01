//
//  Program.cs
//
//  Wiregrass Code Technology 2020-2021
//
using Microsoft.Extensions.Configuration;

namespace KeyVault.Services.Console.Application
{
   internal class Program
    {
        private static void Main()
        {
            var configuration = GetConfiguration("appsettings.json");

            var menus = new Menus(configuration);
            menus.MainMenu();
        }

        private static IConfigurationSection GetConfiguration(string settingsPath)
        {
            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile(settingsPath)
                .Build();

            return configurationRoot.GetSection("AzureKeyVault");
        }
    }
}