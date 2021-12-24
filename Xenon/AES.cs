using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Xenon.Crypt
{
    /// <summary>
    /// Used for encrypting and decrypting wallet.dat file.
    /// </summary>
    public class Aes
    {
        private const int KeySize = 256;
        private const int DerivationIterations = 1000;

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            byte[] randomBytes = new byte[32];
            using (var rngcsp = new RNGCryptoServiceProvider())
            {
                rngcsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        public string Encrypt(string seed, string password)
        {
            byte[] saltBytes = Generate256BitsOfRandomEntropy();
            byte[] ivBytes = Generate256BitsOfRandomEntropy();
            byte[] seedByte = Encoding.UTF8.GetBytes(seed);

            using (var key = new Rfc2898DeriveBytes(password, saltBytes, DerivationIterations))
            {
                var keyBytes = key.GetBytes(KeySize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(seedByte, 0, seedByte.Length);
                                cryptoStream.FlushFinalBlock();
                                var cipherBytes = saltBytes;
                                cipherBytes = cipherBytes.Concat(ivBytes).ToArray();
                                cipherBytes = cipherBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherBytes);
                            }
                        }
                    }
                }
            }
        }

        public string Decrypt(string cipher, string password)
        {
            


                byte[] cipherSaltIv = Convert.FromBase64String(cipher);
                byte[] saltBytes = cipherSaltIv.Take(KeySize / 8).ToArray();
                byte[] ivBytes = cipherSaltIv.Skip(KeySize / 8).Take(KeySize / 8).ToArray();
                byte[] cipherBytes = cipherSaltIv.Skip((KeySize / 8) * 2).Take(cipherSaltIv.Length - ((KeySize / 8) * 2)).ToArray();

                using (var key = new Rfc2898DeriveBytes(password, saltBytes, DerivationIterations))
                {
                    var keyBytes = key.GetBytes(KeySize / 8);
                    using (var symmetricKey = new RijndaelManaged())
                    {
                        symmetricKey.BlockSize = 256;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivBytes))
                        {
                            using (var memoryStream = new MemoryStream(cipherBytes))
                            {
                                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                {
                                    byte[] seedBytes = new byte[cipherBytes.Length];
                                    var decryptedByteCount = cryptoStream.Read(seedBytes, 0, seedBytes.Length);
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return Encoding.UTF8.GetString(seedBytes, 0, decryptedByteCount);
                                }
                            }
                        }
                    }
                }
            

        }
    }
}
