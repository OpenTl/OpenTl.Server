namespace OpenTl.Common.Auth.Client
{
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

    public static class Step3ClientHelper
    {
        public static RequestSetClientDHParams GetRequest(TServerDHParamsOk serverDhParams, byte[] newNonce, out AsymmetricCipherKeyPair clientKeyPair, out DHPublicKeyParameters serverPublicKey )
        {
            AesHelper.ComputeAESParameters(newNonce, serverDhParams.ServerNonce, out var aesKeyData);
            
            var dhInnerData = DeserializeResponse(serverDhParams, aesKeyData);
                
            var p = new BigInteger(SerializationUtils.GetBinaryFromString(dhInnerData.DhPrime));  
            var g = BigInteger.ValueOf(dhInnerData.G);
            Guard.That(g).IsValidDhGParameter(p);

            var dhParameters = new DHParameters(p, g);
            KeyGenerationParameters kgp = new DHKeyGenerationParameters(new SecureRandom(), dhParameters);
            var keyGen = GeneratorUtilities.GetKeyPairGenerator("DH");
            keyGen.Init(kgp);

            clientKeyPair = keyGen.GenerateKeyPair();

            var publicKey = ((DHPublicKeyParameters)clientKeyPair.Public);

            var y = new BigInteger(SerializationUtils.GetBinaryFromString(dhInnerData.GA));
            Guard.That(y).IsValidDhPublicKey(dhParameters.P);
            
            serverPublicKey = new DHPublicKeyParameters(y, dhParameters);

            var clientDhInnerData = new TClientDHInnerData
                                    {
                                        RetryId = 0,
                                        Nonce = serverDhParams.Nonce,
                                        ServerNonce = serverDhParams.ServerNonce,
                                        GB = SerializationUtils.GetStringFromBinary(publicKey.Y.ToByteArray())
                                    };

            return SerializeRequest(clientDhInnerData, aesKeyData);
        }

        private static RequestSetClientDHParams SerializeRequest(TClientDHInnerData clientDhInnerData, AesKeyData aesKeyData)
        {
            var innerData = Serializer.SerializeObject(clientDhInnerData);

            var hashsum = SHA1Helper.ComputeHashsum(innerData);

            var answerWithHash = hashsum.Concat(innerData).ToArray();

            var encryptedAnswer = AES.EncryptAes(aesKeyData, answerWithHash);

            return new RequestSetClientDHParams
                   {
                       EncryptedData = SerializationUtils.GetStringFromBinary(encryptedAnswer),
                       Nonce = clientDhInnerData.Nonce,
                       ServerNonce = clientDhInnerData.ServerNonce
                   };
        }

        private static TServerDHInnerData DeserializeResponse(TServerDHParamsOk serverDhParams, AesKeyData aesKeyData)
        {
            var encryptedAnswer = SerializationUtils.GetBinaryFromString(serverDhParams.EncryptedAnswer);
            var answerWithHash = AES.DecryptAes(aesKeyData, encryptedAnswer);

            var serverHashsum = answerWithHash.Take(20).ToArray();
            var answer = answerWithHash.Skip(20).ToArray();

            var serverDhInnerData = (TServerDHInnerData)Serializer.DeserializeObject(answer);

            var clearAnswer = Serializer.SerializeObject(serverDhInnerData);
            var hashsum = SHA1Helper.ComputeHashsum(clearAnswer);
            Guard.That(serverHashsum).IsItemsEquals(hashsum);

            return serverDhInnerData;
        }
    }
}