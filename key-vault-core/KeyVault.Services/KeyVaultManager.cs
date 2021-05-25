﻿//
//  KeyVaultManager.cs
//
//  Fl Department of Revenue 2020-2021
//
using Microsoft.Extensions.Configuration;

namespace KeyVault.Services
{
    public class KeyVaultManager : IKeyVaultManager
    {
        private readonly IConfigurationSection configuration;

        public KeyVaultManager(IConfigurationSection configurationSection)
        {
            configuration = configurationSection;
        }

        public IKeysManagement KeysService()
        {
            return new KeysManagement(configuration);
        }

        public ISecretsManagement SecretsService()
        {
            return new SecretsManagement(configuration);
        }
    }
}