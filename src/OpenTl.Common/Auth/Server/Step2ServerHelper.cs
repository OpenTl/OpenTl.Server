namespace OpenTl.Common.Auth.Server
{
    using System;
    using System.Linq;

    using BarsGroup.CodeGuard;

    using OpenTl.Common.Crypto;
    using OpenTl.Common.GuardExtensions;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;

    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Math;
    using Org.BouncyCastle.Security;

    public static class Step2ServerHelper
    {
        private static readonly Random Random = new Random();

        private static void GeneratePandG(out BigInteger p, out int g)
        {
            p = BigInteger.ProbablePrime(2048, Random);

            var mod = new Func<BigInteger, int, int, bool>((a, b, c) => Equals(a.Mod(BigInteger.ValueOf(b)), BigInteger.ValueOf(c)));
            
            if (mod(p,8,7))
            {
                g = 2;
            }
            else if (mod(p, 3, 2))
            {
                g = 3;
            }
            else if (mod(p, 5, 1) || mod(p, 5, 4))
            {
                g = 5;
            }
            else if (mod(p, 24, 19) || mod(p, 24, 23))
            {
                g = 6;
            }
            else if (mod(p, 7, 3) || mod(p, 7, 5) || mod(p, 7, 6))
            {
                g = 7;
            }
            else
            {
                g = 4;
            }
            
            Guard.That(BigInteger.ValueOf(g)).IsValidDhGParameter(p);
        }
        
        public static IServerDHParams GetResponse(RequestReqDHParams reqDhParams, string privateKey, out AsymmetricCipherKeyPair serverKeyPair, out byte[] newNonce)
        {
            var pqInnerData = DeserializeRequest(reqDhParams, privateKey);

            GeneratePandG(out var p, out var g);
                
            KeyGenerationParameters kgp = new DHKeyGenerationParameters(new SecureRandom(), new DHParameters(p, BigInteger.ValueOf(g)));
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

            newNonce = pqInnerData.NewNonce;
            
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
            var answer = Serializer.SerializeObject(dhInnerData);

            var hashsum = SHA1Helper.ComputeHashsum(answer);

            var answerWithHash = hashsum.Concat(answer).ToArray();

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