//
//  SecretsResponse.cs
//
//  Copyright (c) Wiregrass Code Technology 2020-2021
//
using Azure;
using Azure.Security.KeyVault.Secrets;

namespace KeyVault.Services
{
    public class SecretsResponse
    {
        private object secret;
        private Pageable<SecretProperties> secrets;
        private string message;

        public object Secret
        {
            get => secret;
            set {  secret = value; HasError = false; }
        }

        public Pageable<SecretProperties> Secrets
        {
            get => secrets;
            set {  secrets = value; HasError = false; }
        }

        public string Message
        {
            get => message;
            set {  message = value; HasError = true; }
        }

        public bool HasError { get; set; }

        public SecretProperties SecretProperties { get; set; }
    }
}