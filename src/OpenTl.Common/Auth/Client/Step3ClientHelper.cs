namespace OpenTl.Common.Auth.Client
{
    using System.Linq;

    using BarsGroup.CodeGuard;

    using OpenTl.Common.Crypto;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;
    using OpenTl.Utils.GuardExtentions;

    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Generators;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Math;
    using Org.BouncyCastle.Security;

    public static class Step3ClientHelper
    {
        public static void GetRequest(TServerDHParamsOk serverDhParams, byte[] newNonce)
        {
            var dhInnerData = DeserializeResponse(serverDhParams, newNonce);

            var p = new BigInteger(SerializationUtils.GetBinaryFromString(dhInnerData.DhPrime));  
            var g = BigInteger.ValueOf(dhInnerData.G);

            KeyGenerationParameters kgp = new DHKeyGenerationParameters(new SecureRandom(), new DHParameters(p, g));
            var keyGen = GeneratorUtilities.GetKeyPairGenerator("DH");
            keyGen.Init(kgp);

            var serverKeyPair = keyGen.GenerateKeyPair();
        }

        private static TServerDHInnerData DeserializeResponse(TServerDHParamsOk serverDhParams, byte[] newNonce)
        {
            AesHelper.ComputeAESParameters(newNonce, serverDhParams.ServerNonce, out var aesKeyData);

            var encryptedAnswer = SerializationUtils.GetBinaryFromString(serverDhParams.EncryptedAnswer);
            var answerWithHash = AES.DecryptAes(aesKeyData, encryptedAnswer);

            var serverHashsum = SHA1Helper.ComputeHashsum(answerWithHash.Take(20).ToArray());
            var answer = answerWithHash.Skip(20).ToArray();

           return (TServerDHInnerData)Serializer.DeserializeObject(answer);

            //            var clearAnswer = Serializer.SerializeObjectWithoutBuffer(serverDhInnerData);
            //            var hashsum = SHA1Helper.ComputeHashsum(clearAnswer);
            //            Guard.That(serverHashsum).IsItemsEquals(hashsum);
        }
    }
}