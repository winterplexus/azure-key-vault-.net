//
//  SecretsManagement.cs
//
//  Wiregrass Code Technology 2020-2022
//
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace KeyVault.Services
{
    public class SecretsManagement: ISecretsManagement
    {
        private readonly SecretClient secretClient;

        public SecretsManagement(IConfigurationSection configurationSection)
        {
            if (configurationSection == null)
            {
                throw new ArgumentNullException(nameof(configurationSection));
            }

            var configuration = configurationSection;

            var clientSecretCredential = new ClientSecretCredential(configuration["Tenant"],
                                                                    configuration["ClientId"],
                                                                    configuration["ClientSecret"]);

            secretClient = new SecretClient(vaultUri: new Uri(configuration["KeyVaultUri"]), credential: clientSecretCredential);
        }

        public SecretsResponse List()
        {
            var secretsResponse = new SecretsResponse();

            try
            {
                secretsResponse.Secrets = secretClient.GetPropertiesOfSecrets();
            }
            catch (RequestFailedException rfe)
            {
                secretsResponse.Message = rfe.Message;
            }

            return secretsResponse;
        }

        public async Task<SecretsResponse> Create(string secretName, string secretValue)
        {
            var secretsResponse = new SecretsResponse();

            if (string.IsNullOrEmpty(secretName))
            {
                return ProcessError(secretsResponse, "secret name is empty");
            }
            if (string.IsNullOrEmpty(secretValue))
            {
                return ProcessError(secretsResponse, "secret value is empty");
            }

            try
            {
                KeyVaultSecret createSecret = await secretClient.SetSecretAsync(secretName, secretValue).ConfigureAwait(false);
                secretsResponse.Secret = createSecret;
            }
            catch (RequestFailedException rfe)
            {
                secretsResponse.Message = rfe.Message;
            }

            return secretsResponse;
        }

        public async Task<SecretsResponse> Retrieve(string secretName)
        {
            var secretsResponse = new SecretsResponse();

            if (string.IsNullOrEmpty(secretName))
            {
                return ProcessError(secretsResponse, "secret name is empty");
            }

            try
            {
                KeyVaultSecret retrieveSecret = await secretClient.GetSecretAsync(secretName).ConfigureAwait(false);
                secretsResponse.Secret = retrieveSecret;
            }
            catch (RequestFailedException rfe)
            {
                secretsResponse.Message = rfe.Message;
            }

            return secretsResponse;
        }

        public async Task<SecretsResponse> Update(string secretName, string tagName, string tagValue)
        {
            var secretsResponse = new SecretsResponse();

            if (string.IsNullOrEmpty(secretName))
            {
                return ProcessError(secretsResponse, "secret name is empty");
            }
            if (string.IsNullOrEmpty(tagName))
            {
                return ProcessError(secretsResponse, "tag name is empty");
            }
            if (string.IsNullOrEmpty(tagValue))
            {
                return ProcessError(secretsResponse, "tag value is empty");
            }

            try
            {
                KeyVaultSecret updateSecret = await secretClient.GetSecretAsync(secretName).ConfigureAwait(false);
                updateSecret.Properties.Tags[tagName] = tagValue;

                SecretProperties updatedSecretProperties = await secretClient.UpdateSecretPropertiesAsync(updateSecret.Properties).ConfigureAwait(false);
                secretsResponse.SecretProperties = updatedSecretProperties;
            }
            catch (RequestFailedException rfe)
            {
                secretsResponse.Message = rfe.Message;
            }

            return secretsResponse;
        }

        public async Task<SecretsResponse> Delete(string secretName)
        {
            var secretsResponse = new SecretsResponse();

            if (string.IsNullOrEmpty(secretName))
            {
                return ProcessError(secretsResponse, "secret name is empty");
            }

            try
            {
                var deleteOperation = await secretClient.StartDeleteSecretAsync(secretName).ConfigureAwait(false);

                var deleteSecret = deleteOperation.Value;
                secretsResponse.Secret = deleteSecret;
            }
            catch (RequestFailedException rfe)
            {
                secretsResponse.Message = rfe.Message;
            }

            return secretsResponse;
        }

        private static SecretsResponse ProcessError(SecretsResponse secretsResponse, string message)
        {
            secretsResponse.Message = message;
            return secretsResponse;
        }
    }
}