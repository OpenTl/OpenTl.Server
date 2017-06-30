using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using OpenTl.Server.Back.Crypto;

namespace OpenTl.Utils.Crypto
{
    internal class RsaServerKey
    {
        private readonly BigInteger _e;

        private readonly BigInteger _m;

        public RsaServerKey(BigInteger m, BigInteger e)
        {
            _m = m;
            _e = e;
        }

        public byte[] Encrypt(byte[] data, int offset, int length)
        {
            using (var buffer = new MemoryStream(255))
            using (var writer = new BinaryWriter(buffer))
            {
                using (var sha1 = SHA1.Create())
                {
                    var hashsum = sha1.ComputeHash(data, offset, length);
                    writer.Write(hashsum);
                }

                buffer.Write(data, offset, length);
                if (length < 235)
                {
                    var padding = new byte[235 - length];
                    new Random().NextBytes(padding);
                    buffer.Write(padding, 0, padding.Length);
                }

                var ciphertext = new BigInteger(1, buffer.ToArray()).ModPow(_e, _m).ToByteArrayUnsigned();

                if (ciphertext.Length == 256)
                {
                    return ciphertext;
                }
                {
                    var paddedCiphertext = new byte[256];
                    var padding = 256 - ciphertext.Length;
                    for (var i = 0; i < padding; i++)
                        paddedCiphertext[i] = 0;
                    ciphertext.CopyTo(paddedCiphertext, padding);
                    return paddedCiphertext;
                }
            }
        }
    }

    public class Rsa
    {
        private static readonly Dictionary<long, RsaServerKey> ServerKeys = new Dictionary<long, RsaServerKey>
        {
            {
                0x14EA8017F07FAF7F,
                new RsaServerKey(new BigInteger(
                        "30820122300D06092A864886F70D01010105000382010F003082010A0282010100831455B68A6AE9ED34EA5A78CDE399CD119E8DCAF335382A0BA7893E75DF7729BE32299E4D86841635423EE8A0E8949E55CEBB19C4884332C83DE5A6036B10056B5892718FA1B9E90ADBFCCCC02FD12A02D0A9FF71A3632C2745A64DC1E14E6B5D0D7FD685F2BB27589BC0D4F8E7BECDE3EAA17A57D34AEDA067F5680CFCCE0364A3860BCFC9A5F4A23348FC8077F211F738234CC3C606DC95FB77B893E0BB5E39C788DD59B8A946ABF584117D95DB85260C2FF45B7741D558B6A5BF3324CAC94BE2EC2AAB7200CD75A7284690178381BE4198F963EB82CFC5073AA939674A8E5277519FBBB23483B4C75F44DC9FA34EC8704120C5D040769471DB4C35CCE44D0203010001",
                        16), new BigInteger("010001", 16))
            }
        };

        public static byte[] Encrypt(long fingerprint, byte[] data, int offset, int length)
        {
            return ServerKeys.TryGetValue(fingerprint, out var key) ? key.Encrypt(data, offset, length) : null;
        }
    }
}