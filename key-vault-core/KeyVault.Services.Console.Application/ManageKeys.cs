//
//  ManageKeys.cs
//
//  Copyright (c) Wiregrass Code Technology 2020-2021
//
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure.Security.KeyVault.Keys;

namespace KeyVault.Services.Console.Application
{
    internal class ManageKeys
    {
        private readonly IKeyVaultManager keyVaultManager;

        public ManageKeys(IConfigurationSection configurationSection)
        {
            keyVaultManager = new KeyVaultManager(configurationSection);
        }

        internal void List()
        {
            try
            {
                var keysResponse = keyVaultManager.KeysService().List();
                if (keysResponse.HasError)
                {
                    WriteErrorMessage(keysResponse.Message);
                }
                else
                {
                    WriteKeysList(keysResponse.Keys);
                }

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
                var keyName = GetInputField("key name", 20);

                var keyType = GetKeyType();
                var keysResponse = Task.Run(async () => await keyVaultManager.KeysService().Create(keyName, keyType).ConfigureAwait(false)).Result;
                if (keysResponse.HasError)
                {
                    WriteErrorMessage(keysResponse.Message);
                }
                else
                {
                    WriteKeyValues(keysResponse.Key);
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
                var keyName = GetInputField("key name", 20);

                var keysResponse = Task.Run(async () => await keyVaultManager.KeysService().Retrieve(keyName).ConfigureAwait(false)).Result;
                if (keysResponse.HasError)
                {
                    WriteErrorMessage(keysResponse.Message);
                }
                else
                {
                    WriteKeyValues(keysResponse.Key);
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
                var keyName  = GetInputField("key name", 20);
                var tagName  = GetInputField("tag name", 20);
                var tagValue = GetInputField("tag value", 20);

                var keysResponse = Task.Run(async () => await keyVaultManager.KeysService().Update(keyName, tagName, tagValue).ConfigureAwait(false)).Result;
                if (keysResponse.HasError)
                {
                    WriteErrorMessage(keysResponse.Message);
                }
                else
                {
                    WriteUpdateKeyValues(keysResponse.Key);
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
                var keyName = GetInputField("key name", 20);

                var keysResponse = Task.Run(async () => await keyVaultManager.KeysService().Delete(keyName).ConfigureAwait(false)).Result;
                if (keysResponse.HasError)
                {
                    WriteErrorMessage(keysResponse.Message);
                }
                else
                {
                    WriteKeyValues(keysResponse.Key);
                }

                ReadContinue();
            }
            catch (Exception ex)
            {
                WriteException(ex);

                ReadContinue();
            }
        }

        private static KeyType GetKeyType()
        {
            System.Console.WriteLine("- [ 1 ] ELLIPTIC CURVE");
            System.Console.WriteLine("- [ 2 ] RSA");

            var option = GetInputField("key type", 20);
            return option switch
            {
                "1" => KeyType.Ec,
                "2" => KeyType.Rsa,
                  _ => KeyType.Rsa
            };
        }

        private static void ReadContinue()
        {
            System.Console.WriteLine("");
            System.Console.Write("PRESS ENTER TO CONTINUE ->");
            System.Console.ReadKey();
        }

        private static void WriteKeysList(IEnumerable<KeyProperties> keysList)
        {
            System.Console.WriteLine("- available keys       =");

            foreach (var keyProperties in keysList)
            {
                System.Console.WriteLine($"- key name             = {keyProperties.Name}");
            }
        }

        private static void WriteKeyValues(object objectKey)
        {
            var key = (KeyVaultKey)objectKey;
            if (key != null)
            {
                if (!string.IsNullOrEmpty(key.Name))
                {
                    System.Console.WriteLine($"- key name             = {key.Name}");
                    System.Console.WriteLine($"- key type             = {key.KeyType}");
                    System.Console.WriteLine($"- key ID               = {key.Id}");
                }
            }
        }

        private static void WriteUpdateKeyValues(object objectKey)
        {
            var key = (KeyVaultKey)objectKey;
            if (key != null)
            {
                if (!string.IsNullOrEmpty(key.Name))
                {
                    System.Console.WriteLine($"- key name             = {key.Name}");
                    System.Console.WriteLine($"- key type             = {key.KeyType}");
                    System.Console.WriteLine($"- key ID               = {key.Id}");
                    System.Console.WriteLine($"- key version          = {key.Properties.Version}");
                    System.Console.WriteLine($"- key updated on       = {key.Properties.UpdatedOn}");
                }
                if (key.Properties.Tags.Count > 0)
                {
                    System.Console.WriteLine("- tags                 =");

                    foreach (var tag in key.Properties.Tags)
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