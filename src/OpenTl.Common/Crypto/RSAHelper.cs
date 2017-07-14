using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.OpenSsl;

namespace OpenTl.Common.Crypto
{
    using Org.BouncyCastle.Crypto.Parameters;

    public static class RSAHelper
    {
        public static byte[] RsaEncryptWithPublic(byte[] bytesToEncrypt, string publicKey)
        {
            var encryptEngine = new Pkcs1Encoding(new RsaEngine());

            using (var txtreader = new StringReader(publicKey))
            {
                var keyParameter = (RsaKeyParameters) new PemReader(txtreader).ReadObject();

                encryptEngine.Init(true, keyParameter);
            }

            return encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length);
        }

        public static byte[] RsaEncryptWithPrivate(byte[] bytesToEncrypt, string privateKey)
        {
            var encryptEngine = new Pkcs1Encoding(new RsaEngine());

            using (var txtreader = new StringReader(privateKey))
            {
                var keyParameter = (RsaPrivateCrtKeyParameters) new PemReader(txtreader).ReadObject();

                encryptEngine.Init(true, keyParameter);
            }

            return encryptEngine.ProcessBlock(bytesToEncrypt, 0, bytesToEncrypt.Length);
        }


        // Decryption:

        public static byte[] RsaDecryptWithPrivate(byte[] bytesToDecrypt, string privateKey)
        {
            var decryptEngine = new Pkcs1Encoding(new RsaEngine());

            using (var txtreader = new StringReader(privateKey))
            {
                var keyParameter = (RsaPrivateCrtKeyParameters) new PemReader(txtreader).ReadObject();

                decryptEngine.Init(false, keyParameter);
            }

            return decryptEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length);
        }

        public static byte[] RsaDecryptWithPublic(byte[] bytesToDecrypt, string publicKey)
        {
            var decryptEngine = new Pkcs1Encoding(new RsaEngine());

            using (var txtreader = new StringReader(publicKey))
            {
                var keyParameter = (RsaKeyParameters) new PemReader(txtreader).ReadObject();

                decryptEngine.Init(false, keyParameter);
            }

            return decryptEngine.ProcessBlock(bytesToDecrypt, 0, bytesToDecrypt.Length);
        }
    }
}