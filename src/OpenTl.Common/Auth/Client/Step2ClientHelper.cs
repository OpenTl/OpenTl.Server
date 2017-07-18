namespace OpenTl.Common.Auth.Client
{
    using System;
    using System.IO;

    using OpenTl.Common.Crypto;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;

    using Org.BouncyCastle.Math;

    public static class Step2ClientHelper
    {
        private static readonly Random Random = new Random();

        public static RequestReqDHParams GetRequest(TResPQ resPq, string publicKey, out byte[] newNonce)
        {
            var pqData = SerializationUtils.GetBinaryFromString(resPq.Pq);

            var pq = new BigInteger(pqData);
            var p = BigIntegerHelper.SmallestPrimeFactor(pq);

            var q = pq.Divide(p);

            var pStr = SerializationUtils.GetStringFromBinary(p.ToByteArray());
            var qStr = SerializationUtils.GetStringFromBinary(q.ToByteArray());

            newNonce = new byte[32];
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

            var innerData = Serializer.SerializeObject(pqInnerData);

            var fingerprint = resPq.ServerPublicKeyFingerprints[0];

            byte[] ciphertext;
            using (var buffer = new MemoryStream(255))
            using (var writer = new BinaryWriter(buffer))
            {
                var hashsum = SHA1Helper.ComputeHashsum(innerData);
                writer.Write(hashsum);

                buffer.Write(innerData, 0, innerData.Length);

                var innerDataWithHash = buffer.ToArray();

                ciphertext = RSAHelper.RsaEncryptWithPublic(innerDataWithHash, publicKey);

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
    }
}