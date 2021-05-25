//
//  IKeysManagement.cs
//
//  Fl Department of Revenue 2020-2021
//
using System.Threading.Tasks;
using Azure.Security.KeyVault.Keys;

namespace KeyVault.Services
{
    public interface IKeysManagement
    {
        KeysResponse List();
        Task<KeysResponse> Create(string keyName, KeyType keyType);
        Task<KeysResponse> Retrieve(string keyName);
        Task<KeysResponse> Update(string keyName, string tagName, string tagValue);
        Task<KeysResponse> Delete(string keyName);
    }
}