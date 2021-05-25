//
//  KeysManagement.cs
//
//  Fl Department of Revenue 2020-2021
//
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;

namespace KeyVault.Services
{
    public class KeysManagement: IKeysManagement
    {
        private readonly KeyClient keyClient;

        public KeysManagement(IConfigurationSection configurationSection)
        {
            if (configurationSection == null)
            {
                throw new ArgumentNullException(nameof(configurationSection));
            }

            var configuration = configurationSection;

            var clientSecretCredential = new ClientSecretCredential(configuration["TenantId"],
                                                                    configuration["ClientId"],
                                                                    configuration["ClientSecret"]);

            keyClient = new KeyClient(vaultUri: new Uri(configuration["KeyVaultUri"]), credential: clientSecretCredential);
        }

        public KeysResponse List()
        {
            var keysResponse = new KeysResponse();

            try
            {
                keysResponse.Keys = keyClient.GetPropertiesOfKeys();
            }
            catch (RequestFailedException rfe)
            {
                keysResponse.Message = rfe.Message;
            }

            return keysResponse;
        }

        public async Task<KeysResponse> Create(string keyName, KeyType keyType)
        {
            var keysResponse = new KeysResponse();

            if (string.IsNullOrEmpty(keyName))
            {
                return ProcessError(keysResponse, "key name is empty");
            }

            try
            {
                if (keyType == KeyType.Ec)
                {
                    var createKeyOptions = new CreateEcKeyOptions(keyName, hardwareProtected: false);
                    KeyVaultKey createKey = await keyClient.CreateEcKeyAsync(createKeyOptions).ConfigureAwait(false);
                    keysResponse.Key = createKey;
                }
                else if (keyType == KeyType.Rsa)
                {
                    var createKeyOptions = new CreateRsaKeyOptions(keyName, hardwareProtected: false);
                    KeyVaultKey createKey = await keyClient.CreateRsaKeyAsync(createKeyOptions).ConfigureAwait(false);
                    keysResponse.Key = createKey;
                }
                else
                {
                    KeyVaultKey createKey = await keyClient.CreateKeyAsync(keyName, keyType).ConfigureAwait(false);
                    Console.WriteLine("createKey ID" + createKey.Id);
                    keysResponse.Key = createKey;
                }
            }
            catch (RequestFailedException rfe)
            {
                keysResponse.Message = rfe.Message;
            }

            return keysResponse;
        }

        public async Task<KeysResponse> Retrieve(string keyName)
        {
            var keysResponse = new KeysResponse();

            if (string.IsNullOrEmpty(keyName))
            {
                return ProcessError(keysResponse, "key name is empty");
            }

            try
            {
                KeyVaultKey retrieveKey = await keyClient.GetKeyAsync(keyName).ConfigureAwait(false);
                keysResponse.Key = retrieveKey;
            }
            catch (RequestFailedException rfe)
            {
                keysResponse.Message = rfe.Message;
            }

            return keysResponse;
        }

        public async Task<KeysResponse> Update(string keyName, string tagName, string tagValue)
        {
            var keysResponse = new KeysResponse();

            if (string.IsNullOrEmpty(keyName))
            {
                return ProcessError(keysResponse, "key name is empty");
            }
            if (string.IsNullOrEmpty(tagName))
            {
                return ProcessError(keysResponse, "tag name is empty");
            }
            if (string.IsNullOrEmpty(tagValue))
            {
                return ProcessError(keysResponse, "tag value is empty");
            }

            try
            {
                KeyVaultKey updateKey = await keyClient.GetKeyAsync(keyName).ConfigureAwait(false);
                updateKey.Properties.Tags[tagName] = tagValue;

                KeyVaultKey updatedKey = await keyClient.UpdateKeyPropertiesAsync(updateKey.Properties).ConfigureAwait(false);
            }
            catch (RequestFailedException rfe)
            {
                keysResponse.Message = rfe.Message;
            }

            return keysResponse;
        }

        public async Task<KeysResponse> Delete(string keyName)
        {
            var keysResponse = new KeysResponse();

            if (string.IsNullOrEmpty(keyName))
            {
                return ProcessError(keysResponse, "key name is empty");
            }

            try
            {
                var deleteOperation = await keyClient.StartDeleteKeyAsync(keyName).ConfigureAwait(false);

                var deleteKey = deleteOperation.Value;
                keysResponse.Key = deleteKey;
            }
            catch (RequestFailedException rfe)
            {
                keysResponse.Message = rfe.Message;
            }

            return keysResponse;
        }

        private static KeysResponse ProcessError(KeysResponse keysResponse, string message)
        {
            keysResponse.Message = message;
            return keysResponse;
        }
    }
}