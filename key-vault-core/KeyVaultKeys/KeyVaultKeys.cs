//
//  KeyVaultKeys.cs
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
using Azure.Security.KeyVault.Keys;

namespace KeyVault
{
    internal class KeyVaultKeys
    {
        private static readonly string[] commandActions = { "create", "get", "update", "delete", "purge" };
        private static IConfigurationSection configuration;
        private static string command;
        private static string keyName;
        private static string tagName;
        private static string tagValue;

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
                keyName = args[1];
            }
            if (args.Count > 2)
            {
                tagName = args[2];
            }
            if (args.Count > 3)
            {
                tagValue = args[3];
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

        private static void ProcessCommand()
        {
            var clientSecretCredential = new ClientSecretCredential(configuration["TenantId"],
                                                                    configuration["ClientId"],
                                                                    configuration["ClientSecret"]);

            var keyClient = new KeyClient(vaultUri: new Uri(configuration["KeyVaultUri"]), credential: clientSecretCredential);

            if (command.ToLowerInvariant() == commandActions[0])
            {
                CreateKey(keyClient);
            }
            else if (command.ToLowerInvariant() == commandActions[1])
            {
                GetKey(keyClient);
            }
            else if (command.ToLowerInvariant() == commandActions[2])
            {
                UpdateKey(keyClient);
            }
            else if (command.ToLowerInvariant() == commandActions[3])
            {
                DeleteKey(keyClient);
            }
            else if (command.ToLowerInvariant() == commandActions[4])
            {
                PurgeKey(keyClient);
            }
            else
            {
                Console.WriteLine("error-> invalid command action: {0}", command.ToLowerInvariant());
            }
        }

        private static void CreateKey(KeyClient keyClient)
        {
            Console.WriteLine("create key {0}", Environment.NewLine);

            try
            {
                var createRsaKeyOptions = new CreateRsaKeyOptions(keyName, hardwareProtected: false);
                KeyVaultKey createKey = keyClient.CreateRsaKey(createRsaKeyOptions);

                Console.WriteLine("name: {0} type: {1}", createKey.Name, createKey.KeyType);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("exception-> {0}", ex);
            }
        }

        private static void GetKey(KeyClient keyClient)
        {
            Console.WriteLine("get key {0}", Environment.NewLine);

            try
            {
                KeyVaultKey key = keyClient.GetKey(keyName);

                Console.WriteLine("name: {0} type: {1}", key.Name, key.KeyType);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("exception-> {0}", ex); Console.WriteLine(ex.ToString());
            }
        }

        private static void UpdateKey(KeyClient keyClient)
        {
            Console.WriteLine("update key {0}", Environment.NewLine);

            if (string.IsNullOrEmpty(tagName))
            {
                Console.WriteLine("error-> tag name is missing");
                return;
            }
            if (string.IsNullOrEmpty(tagValue))
            {
                Console.WriteLine("error-> tag value is missing");
                return;
            }

            try
            {
                KeyVaultKey updateKey = keyClient.GetKey(keyName);
                updateKey.Properties.Tags[tagName] = tagValue;

                KeyVaultKey updatedKey = keyClient.UpdateKeyProperties(updateKey.Properties);

                Console.WriteLine("name: {0} version: {1} updated on: {2}", updatedKey.Name, updatedKey.Properties.Version, updatedKey.Properties.UpdatedOn);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("exception-> {0}", ex);
            }
        }

        private static void DeleteKey(KeyClient keyClient)
        {
            Console.WriteLine("delete key {0}", Environment.NewLine);

            try
            {
                var deleteKeyOperation = keyClient.StartDeleteKey(keyName);
                var deleteKey = deleteKeyOperation.Value;

                Console.WriteLine("name: {0} type: {1}", deleteKey.Name, deleteKey.KeyType);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("exception-> {0}", ex);
            }
        }

        private static void PurgeKey(KeyClient keyClient)
        {
            Console.WriteLine("purge key {0}", Environment.NewLine);

            var waitTime = Convert.ToInt32(configuration["WaitTime"], CultureInfo.InvariantCulture);

            try
            {
                var operation = keyClient.StartDeleteKey(keyName);

                while (!operation.HasCompleted)
                {
                    System.Threading.Thread.Sleep(waitTime);
                    operation.UpdateStatus();
                }

                var key = operation.Value;
                keyClient.PurgeDeletedKey(key.Name);

                Console.WriteLine("name: {0}", key.Name);
            }
            catch (RequestFailedException ex)
            {
                Console.WriteLine("exception-> {0}", ex);
            }
        }

        private static void DisplayUsage()
        {
            var commandActionsOptions = $"[{string.Join("|", commandActions)}]";

            Console.WriteLine("usage: {0}.exe {1} <key name> (optional tag name) (optional tag value)", Process.GetCurrentProcess().ProcessName, commandActionsOptions);
        }
    }
}