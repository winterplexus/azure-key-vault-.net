Azure Key Vault Applications for .NET
=====================================

Azure Key Vault services based on .NET 6 platform using Microsoft Identity Client and Azure Security Key Vault Client libraries.

Key Vault services services library can provision and manage keys and secrets using the primary interface:

* IKeyVaultManager interface:

```
    public interface IKeyVaultManager
    {
        IKeysManagement KeysService();
        ISecretsManagement SecretsService();
    }
```
where:

* IKeysManagement interface

```
    public interface IKeysManagement
    {
        KeysResponse List();
        Task<KeysResponse> Create(string keyName, KeyType keyType);
        Task<KeysResponse> Retrieve(string keyName);
        Task<KeysResponse> Update(string keyName, string tagName, string tagValue);
        Task<KeysResponse> Delete(string keyName);
    }
```

* ISecretsManagement interface

```
    public interface ISecretsManagement
    {
        public SecretsResponse List();
        Task<SecretsResponse> Create(string secretName, string secretValue);
        Task<SecretsResponse> Retrieve(string secretName);
        Task<SecretsResponse> Update(string secretName, string tagName, string tagValue);
        Task<SecretsResponse> Delete(string secretName);
    }
```

