//
//  IKeyVaultManager.cs
//
//  Fl Department of Revenue 2020-2021
//
namespace KeyVault.Services
{
    public interface IKeyVaultManager
    {
        IKeysManagement KeysService();
        ISecretsManagement SecretsService();
    }
}