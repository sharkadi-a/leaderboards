using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using AndreyGames.Leaderboards.Service.Abstract;

namespace AndreyGames.Leaderboards.Service.Implementation
{
    static class Extensions
    {
        public static IEnumerable<T> PadRight<T>(this IEnumerable<T> enumerable, int count, T value)
        {
            var counter = 0;
            foreach (var item in enumerable)
            {
                yield return item;
                counter++;
            }

            var diff = count - counter;
            if (diff > 0)
            {
                foreach (var item in Enumerable.Repeat(value, diff))
                {
                    yield return item;
                }
            }
        }
    }
    
    internal class CryptoService : ICryptoService
    {
        private byte[] Encode(byte[] bytes, IEnumerable<byte> key, IEnumerable<byte> vector)
        {
            using var aes = Aes.Create();
            var encryptor = aes.CreateEncryptor(key.ToArray(), vector.ToArray());
            var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.Close();
            return memoryStream.ToArray();
        }

        private byte[] Decode(byte[] bytes, IEnumerable<byte> key, IEnumerable<byte> vector)
        {
            using var aes = Aes.Create();
            var decryptor = aes.CreateDecryptor(key.ToArray(), vector.ToArray());
            var memoryStream = new MemoryStream();
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, bytes.Length);
            cryptoStream.Close();
            return memoryStream.ToArray();
        }

        public string EncryptAsBase64(byte[] bytes, string key, string ivString = default)
        {
            var k = Encoding.Default.GetBytes(key).Take(16).PadRight(16, (byte)0);
            var iv = !string.IsNullOrWhiteSpace(ivString)
                ? Encoding.Default.GetBytes(ivString).Take(16).PadRight(16, (byte)0)
                : default;
            var encrypted = Encode(bytes, k.ToArray(), iv);
            return Convert.ToBase64String(encrypted);
        }

        public byte[] DecryptFromBase64(string base64, string key, string ivString = default)
        {
            var k = Encoding.Default.GetBytes(key).Take(16).PadRight(16, (byte)0);
            var iv = !string.IsNullOrWhiteSpace(ivString)
                ? Encoding.Default.GetBytes(ivString).Take(16).PadRight(16, (byte)0)
                : default;
            var bytes = Convert.FromBase64String(base64);
            var decrypted = Decode(bytes, k.ToArray(), iv);
            return decrypted;
        }

        public byte[] Decrypt(byte[] bytes, string key, string ivString = default)
        {
            var k = Encoding.Default.GetBytes(key).Take(16).PadRight(16, (byte)0);
            var iv = !string.IsNullOrWhiteSpace(ivString)
                ? Encoding.Default.GetBytes(ivString).Take(16).PadRight(16, (byte)0)
                : default;
            var decrypted = Decode(bytes, k.ToArray(), iv);
            return decrypted;
        }
    }
}