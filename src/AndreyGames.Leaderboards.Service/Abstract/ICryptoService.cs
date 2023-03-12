namespace AndreyGames.Leaderboards.Service.Abstract
{
    /// <summary>
    /// Crypto service
    /// </summary>
    public interface ICryptoService
    {
        /// <summary>
        /// Encrypts an array of bytes and accepts key and IV as strings
        /// </summary>
        string EncryptAsBase64(byte[] bytes, string key, string ivString = default);

        /// <summary>
        /// Decrypts bytes (as Base64 string) and accepts key and IV as strings
        /// </summary>
        byte[] DecryptFromBase64(string base64, string key, string ivString = default);

        /// <summary>
        /// Decrypts bytes (as Base64 string) and accepts key and IV as strings
        /// </summary>
        byte[] Decrypt(byte[] bytes, string key, string ivString = default);
    }
}