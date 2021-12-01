//
//  IKeyVaultManager.cs
//
//  Wiregrass Code Technology 2020-2022
//
namespace KeyVault.Services
{
    public interface IKeyVaultManager
    {
        IKeysManagement KeysService();
        ISecretsManagement SecretsService();
    }
}