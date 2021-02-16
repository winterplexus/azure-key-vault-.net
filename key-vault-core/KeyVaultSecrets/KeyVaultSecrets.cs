//
//  KeyVaultSecrets.cs
//
//  Wiregrass Code Technology 2020-2021
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace KeyVault
{
    internal class KeyVaultSecrets
    {
        private static readonly string[] commandActions = { "set", "get", "delete", "purge" };
        private static IConfigurationSection configuration;
        private static string command;
        private static string secretName;
        private static string secretValue;

        private static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                DisplayUsage();
                return;
            }

            GetCommandLineArguments(args);
            GetConfiguration("appsettings.json");
            ProcessCommand();
        }

        private static void GetCommandLineArguments(IReadOnlyList<string> args)
        {
            if (args.Count > 0)
            {
                command = args[0];
            }
            if (args.Count > 1)
            {
                secretName = args[1];
            }
            if (args.Count > 2)
            {
                secretValue = args[2];
            }
        }

        private static void ProcessCommand()
        {
            var clientSecretCredential = new ClientSecretCredential(configuration["TenantId"],
                                                                    configuration["ClientId"],
                                                                    configuration["ClientSecret"]);

            var secretClient = new SecretClient(new Uri(configuration["KeyVaultUri"]), clientSecretCredential);

            if (command.ToLowerInvariant() == commandActions[0])
            {
                SetSecret(secretClient);
            }
            else if (command.ToLowerInvariant() == commandActions[1])
            {
                GetSecret(secretClient);
            }
            else if (command.ToLowerInvariant() == commandActions[2])
            {
                DeleteSecret(secretClient);
            }
            else if (command.ToLowerInvariant() == commandActions[3])
            {
                PurgeSecret(secretClient);
            }
            else
            {
                Console.WriteLine("error-> invalid command action: {0}", command.ToLowerInvariant());
            }
        }

        private static void SetSecret(SecretClient secretClient)
        {
            Console.WriteLine("set secret {0}", Environment.NewLine);

            try
            {
                secretClient.SetSecret(secretName, secretValue);

                Console.WriteLine("name: {0} value: {1}", secretName, secretValue);

            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("exception-> {0}", ex);
            }
        }

        private static void GetSecret(SecretClient secretClient)
        {
            Console.WriteLine("get secret {0}", Environment.NewLine);

            try
            {
                KeyVaultSecret secret = secretClient.GetSecret(secretName);

                Console.WriteLine("name: {0} value: {1}", secretName, secretValue);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("exception-> {0}", ex);
            }
        }

        private static void DeleteSecret(SecretClient secretClient)
        {
            Console.WriteLine("delete secret {0}", Environment.NewLine);

            var waitTime = Convert.ToInt32(configuration["WaitTime"], CultureInfo.InvariantCulture);

            try
            {
                secretClient.StartDeleteSecret(secretName);

                System.Threading.Thread.Sleep(waitTime);

                Console.WriteLine("name: {0}", secretName);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("exception-> {0}", ex);
            }
        }

        private static void PurgeSecret(SecretClient secretClient)
        {
            Console.WriteLine("purge secret {0}", Environment.NewLine);

            var waitTime = Convert.ToInt32(configuration["WaitTime"], CultureInfo.InvariantCulture);

            try
            {
                secretClient.PurgeDeletedSecret(secretName);

                System.Threading.Thread.Sleep(waitTime);

                Console.WriteLine("name: {0}", secretName);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("exception-> {0}", ex);
            }
        }

        private static void GetConfiguration(string settingsPath)
        {
            var configurationRoot = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile(settingsPath)
                .Build();

            configuration = configurationRoot.GetSection("AzureKeyVault");
        }

        private static void DisplayUsage()
        {
            var commandActionsOptions = $"[{string.Join("|", commandActions)}]";

            Console.WriteLine("usage: {0}.exe {1} <secret name> <secret value>", Process.GetCurrentProcess().ProcessName, commandActionsOptions);
        }
    }
}
