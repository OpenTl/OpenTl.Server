namespace OpenTl.Common.Auth.Server
{
    using System.Linq;

    using BarsGroup.CodeGuard;

    using OpenTl.Common.Crypto;
    using OpenTl.Common.GuardExtentions;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;

    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Parameters;
    using Org.BouncyCastle.Math;
    using Org.BouncyCastle.Security;
    
    using MoreLinq;
    
    public static class Step3ServerHelper
    {
        public static ISetClientDHParamsAnswer GetResponse(RequestSetClientDHParams setClientDhParams, byte[] newNonce, AsymmetricCipherKeyPair serverKeyPair, out BigInteger serverAgree)
        {
            AesHelper.ComputeAESParameters(newNonce, setClientDhParams.ServerNonce, out var aesKeyData);
            
            var dhInnerData = DeserializeRequest(setClientDhParams, aesKeyData);

            var y = new BigInteger(SerializationUtils.GetBinaryFromString(dhInnerData.GB));

            var clientPublicKey = new DHPublicKeyParameters(y, ((DHPrivateKeyParameters)serverKeyPair.Private).Parameters);
                
            var serverKeyAgree = AgreementUtilities.GetBasicAgreement("DH");
            serverKeyAgree.Init(serverKeyPair.Private);

            serverAgree = serverKeyAgree.CalculateAgreement(clientPublicKey);

            return SerializeResponse(setClientDhParams, newNonce, serverAgree);
        }

        private static TDhGenOk SerializeResponse(RequestSetClientDHParams setClientDhParams, byte[] newNonce, BigInteger agreement)
        {
            var newNonceHash = SHA1Helper.ComputeHashsum(newNonce).Skip(4).ToArray();
            
            var authKeyAuxHash = SHA1Helper.ComputeHashsum(agreement.ToByteArray()).Take(8).ToArray();

            return new TDhGenOk
                   {
                       Nonce = setClientDhParams.Nonce,
                       ServerNonce = setClientDhParams.ServerNonce,
                       NewNonceHash1 = newNonceHash.Concat((byte)1).Concat(authKeyAuxHash).ToArray()
                   };
        }
        
        private static TClientDHInnerData DeserializeRequest(RequestSetClientDHParams serverDhParams, AesKeyData aesKeyData)
        {
            var encryptedAnswer = SerializationUtils.GetBinaryFromString(serverDhParams.EncryptedData);
            var answerWithHash = AES.DecryptAes(aesKeyData, encryptedAnswer);

            var serverHashsum = answerWithHash.Take(20).ToArray();
            var answer = answerWithHash.Skip(20).ToArray();

            var clientDhInnerData = (TClientDHInnerData)Serializer.DeserializeObject(answer);

            var clearAnswer = Serializer.SerializeObject(clientDhInnerData);
            var hashsum = SHA1Helper.ComputeHashsum(clearAnswer);
            Guard.That(serverHashsum).IsItemsEquals(hashsum);

            return clientDhInnerData;

        }

    }
}