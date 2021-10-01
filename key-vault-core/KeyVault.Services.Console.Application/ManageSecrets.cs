//
//  ManageSecrets.cs
//
//  Wiregrass Code Technology 2020-2021
//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure;
using Azure.Security.KeyVault.Secrets;

namespace KeyVault.Services.Console.Application
{
    internal class ManageSecrets
    {
        private readonly IKeyVaultManager keyVaultManager;

        internal ManageSecrets(IConfigurationSection configurationSection)
        {
            keyVaultManager = new KeyVaultManager(configurationSection);
        }

        internal void List()
        {
            try
            {
                var secretsResponse = keyVaultManager.SecretsService().List();
                if (secretsResponse.HasError)
                {
                    WriteErrorMessage(secretsResponse.Message);
                }
                else
                {
                    WriteSecretValues(secretsResponse.Secret);
                }

                WriteSecretsList(secretsResponse.Secrets);

                ReadContinue();
            }
            catch (Exception ex)
            {
                WriteException(ex);

                ReadContinue();
            }
        }

        internal void Create()
        {
            try
            {
                var secretName  = GetInputField("secret name", 20);
                var secretValue = GetInputField("secret value", 20);

                var secretsResponse = Task.Run(async () => await keyVaultManager.SecretsService().Create(secretName, secretValue).ConfigureAwait(false)).Result;
                if (secretsResponse.HasError)
                {
                    WriteErrorMessage(secretsResponse.Message);
                }
                else
                {
                    WriteSecretValues(secretsResponse.Secret);
                }

                ReadContinue();
            }
            catch (Exception ex)
            {
                WriteException(ex);

                ReadContinue();
            }
        }

        internal void Retrieval()
        {
            try
            {
                var secretName = GetInputField("secret name", 20);

                var secretsResponse = Task.Run(async () => await keyVaultManager.SecretsService().Retrieve(secretName).ConfigureAwait(false)).Result;
                if (secretsResponse.HasError)
                {
                    WriteErrorMessage(secretsResponse.Message);
                }
                else
                {
                    WriteSecretValues(secretsResponse.Secret);
                }

                ReadContinue();
            }
            catch (Exception ex)
            {
                WriteException(ex);

                ReadContinue();
            }
        }

        internal void Update()
        {
            try
            {
                var secretName = GetInputField("secret name", 20);
                var tagName    = GetInputField("tag name", 20);
                var tagValue   = GetInputField("tag value", 20);

                var secretsResponse = Task.Run(async () => await keyVaultManager.SecretsService().Update(secretName, tagName, tagValue).ConfigureAwait(false)).Result;
                if (secretsResponse.HasError)
                {
                    WriteErrorMessage(secretsResponse.Message);
                }
                else
                {
                    WriteUpdateSecretValues(secretsResponse.Secret);
                }

                ReadContinue();
            }
            catch (Exception ex)
            {
                WriteException(ex);

                ReadContinue();
            }
        }

        internal void Delete()
        {
            try
            {
                var secretName = GetInputField("secret name", 20);

                var secretsResponse = Task.Run(async () => await keyVaultManager.SecretsService().Delete(secretName).ConfigureAwait(false)).Result;
                if (secretsResponse.HasError)
                {
                    WriteErrorMessage(secretsResponse.Message);
                }
                else
                {
                    WriteSecretValues(secretsResponse.Secret);
                }

                ReadContinue();
            }
            catch (Exception ex)
            {
                WriteException(ex);

                ReadContinue();
            }
        }

        private static void ReadContinue()
        {
            System.Console.WriteLine("");
            System.Console.Write("PRESS ENTER TO CONTINUE ->");
            System.Console.ReadKey();
        }

        private static void WriteSecretsList(IEnumerable<SecretProperties> secretsList)
        {
            System.Console.WriteLine("- available secrets    =");

            foreach (var keyProperties in secretsList)
            {
                System.Console.WriteLine($"- secret name          : {keyProperties.Name}");
            }
        }

        private static void WriteSecretValues(object objectSecret)
        {
            var secret = (KeyVaultSecret)objectSecret;
            if (secret != null)
            {
                if (!string.IsNullOrEmpty(secret.Name))
                {
                    System.Console.WriteLine($"- secret name          = {secret.Name}");
                    System.Console.WriteLine($"- secret value         = {secret.Value}");
                    System.Console.WriteLine($"- secret ID            = {secret.Id}");
                }
                if (secret.Properties.Tags.Count > 0)
                {
                    System.Console.WriteLine("- tags                 =");

                    foreach (var tag in secret.Properties.Tags)
                    {
                        System.Console.WriteLine($"- tag name             > {tag.Key}");
                        System.Console.WriteLine($"- tag value            > {tag.Value}");
                    }
                }
            }
        }

        private static void WriteUpdateSecretValues(object objectSecret)
        {
            var secret = (KeyVaultSecret)objectSecret;
            if (secret != null)
            {
                if (!string.IsNullOrEmpty(secret.Name))
                {
                    System.Console.WriteLine($"- secret name          = {secret.Name}");
                    System.Console.WriteLine($"- secret value         = {secret.Value}");
                    System.Console.WriteLine($"- secret ID            = {secret.Id}");
                    System.Console.WriteLine($"- secret version       = {secret.Properties.Version}");
                    System.Console.WriteLine($"- secret updated on    = {secret.Properties.UpdatedOn}");
                }
                if (secret.Properties.Tags.Count > 0)
                {
                    System.Console.WriteLine("- tags                  =");

                    foreach (var tag in secret.Properties.Tags)
                    {
                        System.Console.WriteLine($"- tag name             > {tag.Key}");
                        System.Console.WriteLine($"- tag value            > {tag.Value}");
                    }
                }
            }
        }

        private static void WriteErrorMessage(string message)
        {
            System.Console.WriteLine($"error-> {message}");
        }

        private static void WriteException(Exception ex)
        {
            System.Console.WriteLine($"exception-> {ex.Message}");
            System.Console.WriteLine($"inner exception-> {ex.InnerException?.Message}");
            System.Console.WriteLine($"stack trace-> {Environment.NewLine}{ex.StackTrace}");
        }

        private static string GetInputField(string fieldLabel, int fieldLabelWidth)
        {
            if (string.IsNullOrEmpty(fieldLabel))
            {
                throw new ArgumentNullException(nameof(fieldLabel));
            }

            var width = fieldLabelWidth.ToString(CultureInfo.CurrentCulture);
            var format = "{0,-" + width + "}";

            System.Console.Write("- ");
            System.Console.Write(format, fieldLabel);
            System.Console.Write(" : ");

            return System.Console.ReadLine();
        }
    }
}