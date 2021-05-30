//
//  ISecretsManagement.cs
//
//  Copyright (c) Wiregrass Code Technology 2020-2021
//
using System.Threading.Tasks;

namespace KeyVault.Services
{
    public interface ISecretsManagement
    {
        public SecretsResponse List();
        Task<SecretsResponse> Create(string secretName, string secretValue);
        Task<SecretsResponse> Retrieve(string secretName);
        Task<SecretsResponse> Update(string secretName, string tagName, string tagValue);
        Task<SecretsResponse> Delete(string secretName);
    }
}