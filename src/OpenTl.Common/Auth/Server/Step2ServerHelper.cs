namespace OpenTl.Common.Auth.Server
{
    using System;
    using System.IO;
    using System.Linq;

    using BarsGroup.CodeGuard;

    using OpenTl.Common.Crypto;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;
    using OpenTl.Utils.GuardExtentions;

    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Security;

    public static class Step2ServerHelper
    {
        private static readonly Random Random = new Random();

        public static IServerDHParams GetResponse(RequestReqDHParams reqDhParams, string privateKey, out AsymmetricCipherKeyPair serverKeyPair)
        {
            var pqInnerData = DeserializeRequest(reqDhParams, privateKey);

            var generator = new DHParametersGenerator();
            generator.Init(1024, 7, new SecureRandom());

            KeyGenerationParameters kgp = new DHKeyGenerationParameters(new SecureRandom(), generator.GenerateParameters());
            var keyGen = GeneratorUtilities.GetKeyPairGenerator("DH");
            keyGen.Init(kgp);

            serverKeyPair = keyGen.GenerateKeyPair();

            var publicKey = (DHPublicKeyParameters)serverKeyPair.Public;

            var dhInnerData = new TServerDHInnerData
                              {
                                  DhPrime = SerializationUtils.GetStringFromBinary(publicKey.Parameters.P.ToByteArray()),
                                  Nonce = pqInnerData.Nonce,
                                  ServerNonce = pqInnerData.ServerNonce,
                                  G = publicKey.Parameters.G.IntValue,
                                  GA = SerializationUtils.GetStringFromBinary(publicKey.Y.ToByteArray()),
                                  ServerTime = (int)((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds()
                              };

            return SerializeResponse(pqInnerData, dhInnerData);
        }

        private static TPQInnerData DeserializeRequest(RequestReqDHParams reqDhParams, string privateKey)
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

            var innerDataWithHash = RSAHelper.RsaDecryptWithPrivate(encryptedData, privateKey);

            var shaHashsum = innerDataWithHash.Take(20).ToArray();

            var innerData = innerDataWithHash.Skip(20).ToArray();

            var hashsum = SHA1Helper.ComputeHashsum(innerData);
            Guard.That(shaHashsum).IsItemsEquals(hashsum);

            return Serializer.DeserializeObject(innerData).Cast<TPQInnerData>();
        }

        private static TServerDHParamsOk SerializeResponse(TPQInnerData pqInnerData, TServerDHInnerData dhInnerData)
        {
            var answer = Serializer.SerializeObjectWithoutBuffer(dhInnerData);

            var hashsum = SHA1Helper.ComputeHashsum(answer);

            byte[] answerWithHash = null;
            using (var stream = new MemoryStream(hashsum.Length + answer.Length))
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(hashsum);

                writer.Write(answer);

                answerWithHash = stream.ToArray();
            }

            AesHelper.ComputeAESParameters(pqInnerData.NewNonce, pqInnerData.ServerNonce, out var aesKeyData);

            var encryptedAnswer = AES.EncryptAes(aesKeyData, answerWithHash);
            return new TServerDHParamsOk
                   {
                       EncryptedAnswer = SerializationUtils.GetStringFromBinary(encryptedAnswer),
                       Nonce = pqInnerData.Nonce,
                       ServerNonce = pqInnerData.ServerNonce
                   };

        }
    }
}