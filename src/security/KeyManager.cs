using System;
using System.Security.Cryptography;
using Microsoft.Win32;

namespace Steganography.security
{
    public static class KeyManager
    {
        private const string KeyPath = "Software\\Steganography";
        private const string AesKeyName = "AESKey";
        private const string AesIvName = "AESIV";

        public static (byte[] key, byte[] iv) GetOrCreateKeys()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(KeyPath, false);
                if (key == null)
                {
                    return GenerateAndStoreNewKeys();
                }

                var storedKey = key.GetValue(AesKeyName) as byte[];
                var storedIv = key.GetValue(AesIvName) as byte[];

                if (storedKey == null || storedIv == null || storedKey.Length != 32 || storedIv.Length != 16)
                {
                    return GenerateAndStoreNewKeys();
                }

                return (storedKey, storedIv);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to access or create encryption keys.", ex);
            }
        }

        private static (byte[] key, byte[] iv) GenerateAndStoreNewKeys()
        {
            byte[] key = new byte[32];
            byte[] iv = new byte[16];

            RandomNumberGenerator.Fill(key);
            RandomNumberGenerator.Fill(iv);

            try
            {
                using var regKey = Registry.CurrentUser.CreateSubKey(KeyPath, true);
                if (regKey == null)
                {
                    throw new InvalidOperationException("Failed to create registry key for storing encryption keys.");
                }

                regKey.SetValue(AesKeyName, key, RegistryValueKind.Binary);
                regKey.SetValue(AesIvName, iv, RegistryValueKind.Binary);

                return (key, iv);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to store encryption keys in the registry.", ex);
            }
        }

        public static void RegenerateKeys()
        {
            GenerateAndStoreNewKeys();
        }
    }
} 