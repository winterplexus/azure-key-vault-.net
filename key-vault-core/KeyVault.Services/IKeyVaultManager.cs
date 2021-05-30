//
//  IKeyVaultManager.cs
//
//  Copyright (c) Wiregrass Code Technology 2020-2021
//
namespace KeyVault.Services
{
    public interface IKeyVaultManager
    {
        IKeysManagement KeysService();
        ISecretsManagement SecretsService();
    }
}