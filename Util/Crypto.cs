using System;
using System.Security.Cryptography;

namespace SampleApiServer.Util
{
    /// <summary>
    /// PBKDF2によるhash化の機能を提供するクラス
    /// https://docs.microsoft.com/ja-jp/dotnet/api/system.security.cryptography.rfc2898derivebytes?view=netcore-3.1
    /// </summary>
    public static class Crypto
    {
        private const int DEFAULT_SALT_SIZE = 32;
        private const int DEFAULT_HASH_SIZE = 32;
        private const int DEFAULT_STREACH_COUNT = 1000;

        /// <summary>
        /// 文字列をもとにPBKDF2で暗号化したhashを生成する
        /// </summary>
        /// <param name="password">hash化対象の文字列</param>
        /// <param name="saltSize">省略可：saltの大きさ</param>
        /// <param name="hashSize">省略可：hashの大きさ</param>
        /// <param name="streachCount">省略可：ストレッチ回数</param>
        /// <returns>(hash, salt)のタプル</returns>
        public static (string hash, string salt) Generate(
            string password,
            int saltSize = DEFAULT_SALT_SIZE,
            int hashSize = DEFAULT_HASH_SIZE,
            int streachCount = DEFAULT_STREACH_COUNT
        )
        {
            byte[] saltBytes = GenerateSalt(saltSize);
            string hash = GenerateHash(password, saltBytes, hashSize, streachCount);
            string salt = Convert.ToBase64String(saltBytes);

            return (hash, salt);
        }

        /// <summary>
        /// hashの整合性を確認する
        /// </summary>
        /// <param name="password">検証対象の文字列</param>
        /// <param name="inputHash">検証対象のhash</param>
        /// <param name="salt">salt</param>
        /// <param name="hashSize">省略可：hashの大きさ</param>
        /// <param name="streachCount">省略可：ストレッチ回数</param>
        /// <returns>正しいhashか？</returns>
        public static bool Verify(
            string password,
            string inputHash,
            string salt,
            int hashSize = DEFAULT_HASH_SIZE,
            int streachCount = DEFAULT_STREACH_COUNT
        )
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            string trueHash = GenerateHash(password, saltBytes, hashSize, streachCount);
            return inputHash == trueHash;
        }

        // ランダムなsaltを出力する
        private static byte[] GenerateSalt(int saltSize)
        {
            var saltBytes = new byte[saltSize];
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                rngCryptoServiceProvider.GetBytes(saltBytes);
            }

            return saltBytes;
        }

        // saltを元にhashを生成する
        private static string GenerateHash(string password, byte[] saltBytes, int hashSize, int streachCount)
        {
            byte[] hashBytes;

            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, streachCount))
            {
                hashBytes = rfc2898DeriveBytes.GetBytes(hashSize);
            }

            return Convert.ToBase64String(hashBytes);
        }
    }
}
