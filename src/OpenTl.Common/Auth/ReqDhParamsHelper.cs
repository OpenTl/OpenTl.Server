using System;
using System.IO;
using System.Security.Cryptography;
using OpenTl.Common.Crypto;
using OpenTl.Schema;
using OpenTl.Schema.Serialization;
using Org.BouncyCastle.Math;

namespace OpenTl.Common.Auth
{
    using System.Linq;

    using BarsGroup.CodeGuard;

    using OpenTl.Utils.GuardExtentions;

    public static class ReqDhParamsHelper
    {
        private static readonly Random Random = new Random();
        
        public static RequestReqDHParams Client(TResPQ resPq, string publicKey)
        {
            var pqData = SerializationUtils.GetBinaryFromString(resPq.Pq);

            var pq = new BigInteger(pqData);
            var p = BigIntegerHelper.SmallestPrimeFactor(pq);;
            var q = pq.Divide(p);
            
            var pStr = SerializationUtils.GetString(p.ToByteArray());
            var qStr = SerializationUtils.GetString(q.ToByteArray());

            
            var newNonce = new byte[32];
            Random.NextBytes(newNonce);
            
            var pqInnerData = new TPQInnerData
            {
                Pq = resPq.Pq,
                P = pStr,
                Q = qStr,
                ServerNonce = resPq.ServerNonce,
                Nonce = resPq.Nonce,
                NewNonce = newNonce    
            };
            
            var innerData = Serializer.SerializeObjectWithoutBuffer(pqInnerData);
            
            var fingerprint = resPq.ServerPublicKeyFingerprints[0];

            byte[] ciphertext;
            using (var buffer = new MemoryStream(255))
            using (var writer = new BinaryWriter(buffer))
            {
                using (var sha1 = SHA1.Create())
                {
                    var hashsum = sha1.ComputeHash(innerData, 0, innerData.Length);
                    writer.Write(hashsum);
                }

                buffer.Write(innerData, 0, innerData.Length);

                var innerDataWithHash = buffer.ToArray();

                ciphertext = RSAEncryption.RsaEncryptWithPublic(innerDataWithHash, publicKey);

                if (ciphertext.Length != 256)
                {
                    var paddedCiphertext = new byte[256];
                    var padding = 256 - ciphertext.Length;
                    for (var i = 0; i < padding; i++)
                        paddedCiphertext[i] = 0;
                    ciphertext.CopyTo(paddedCiphertext, padding);
                    ciphertext = paddedCiphertext;
                }
            }
            
            return new RequestReqDHParams
            {
                Nonce = resPq.Nonce,
                P = pStr,
                Q = qStr,
                ServerNonce = resPq.ServerNonce,
                PublicKeyFingerprint = fingerprint,
                EncryptedData = SerializationUtils.GetStringFromBinary(ciphertext)
            };
        }

        public static void Server(RequestReqDHParams reqDhParams, string privateKey)
        {
            var encryptedDataWithPadding = SerializationUtils.GetBinaryFromString(reqDhParams.EncryptedData);
            
            int index;
            for (index = 0; index < encryptedDataWithPadding.Length; index++)
            {
                if (encryptedDataWithPadding[index] != 0)
                {
                    break;
                }
            }
            var dataLength = encryptedDataWithPadding.Length - index;
            var encryptedData = new byte[dataLength];
            encryptedDataWithPadding.CopyTo(encryptedData, index);

            var innerDataWithHash = RSAEncryption.RsaDecryptWithPrivate(encryptedData, privateKey);

            var shaHashsum = innerDataWithHash.Take(20).ToArray();

            var innerData = innerDataWithHash.Skip(20).ToArray();
            
            using (var sha1 = SHA1.Create())
            {
                var hashsum = sha1.ComputeHash(innerData, 0, innerData.Length);
                Guard.That(shaHashsum).IsItemsEquals(hashsum);
            }
            
            var pqInnerData = Serializer.DeserializeObject(innerData).Cast<TPQInnerData>();
//            using (var buffer = new MemoryStream(encryptedDataWithPadding))
//            using (var reader = new BinaryReader(buffer))
//            {
//            }
        }
    }
}