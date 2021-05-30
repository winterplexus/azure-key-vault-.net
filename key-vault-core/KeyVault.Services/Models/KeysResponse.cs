//
//  KeysResponse.cs
//
//  Copyright (c) Wiregrass Code Technology 2020-2021
//
using Azure;
using Azure.Security.KeyVault.Keys;

namespace KeyVault.Services
{
    public class KeysResponse
    {
        private object key;
        private Pageable<KeyProperties> keys;
        private string message;

        public object Key
        {
            get => key;
            set {  key = value; HasError = false; }
        } 

        public Pageable<KeyProperties> Keys
        {
            get => keys;
            set {  keys = value; HasError = false; }
        }

        public string Message
        {
            get => message;
            set {  message = value; HasError = true; }
        }

        public bool HasError { get; set; }
    }
}