using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BarsGroup.CodeGuard;
using OpenTl.Schema;
using OpenTl.Schema.Serialization;
using OpenTl.Server.Back.Cache;
using OpenTl.Server.Back.Contracts.Auth;
using OpenTl.Utils.Crypto;
using OpenTl.Utils.GuardExtentions;

namespace OpenTl.Server.Back.Auth
{
    public class RequestReqDhParamsHandlerGrain : BaseObjectHandlerGrain<RequestReqDHParams, IServerDHParams>,
        IRequestReqDhParamsHandler
    {
        protected override Task<IServerDHParams> HandleProtected(Guid clientId, RequestReqDHParams obj)
        {
            var cache = AuthCache.GetCache(clientId);

            CheckInputData(cache, obj);

            cache.Nonce = obj.Nonce;
                
            var encryptedDataWithPadding = SerializationUtils.GetBinaryFromString(obj.EncryptedData);

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

            var rsa = RSA.Create();
            rsa;
            new BigInteger(encryptedData)
                
            using (var buffer = new MemoryStream(encryptedDataWithPadding))
            using (var reader = new BinaryReader(buffer))
            {
                

            }
//            {
//                using (var sha1 = SHA1.Create())
//                {
//                    var hashsum = sha1.ComputeHash(data, offset, length);
//                    writer.Write(hashsum);
//                }
//
//                buffer.Write(data, offset, length);
//                if (length < 235)
//                {
//                    var padding = new byte[235 - length];
//                    new Random().NextBytes(padding);
//                    buffer.Write(padding, 0, padding.Length);
//                }
//
//                var innerData = buffer.ToArray();
//                var ciphertext = BigInteger.ModPow(_g, _a, new BigInteger(innerData)).ToByteArray();
//
//                if (ciphertext.Length == 256)
//                {
//                    return ciphertext;
//                }
//                {
//                    var paddedCiphertext = new byte[256];
//                    var padding = 256 - ciphertext.Length;
//                    for (var i = 0; i < padding; i++)
//                        paddedCiphertext[i] = 0;
//                    ciphertext.CopyTo(paddedCiphertext, padding);
//                    return paddedCiphertext;
//                }
//            }

            return null;
        }

        private bool CheckInputData(AuthCache cache, RequestReqDHParams obj)
        {
            try
            {
                Guard.That(obj.ServerNonce).IsItemsEquals(cache.ServerNonce);
                
//                Guard.That(SerializationUtils.GetBinaryFromString(obj.P)).IsItemsEquals(cache.P.ToByteArray());
//                Guard.That(SerializationUtils.GetBinaryFromString(obj.Q)).IsItemsEquals(cache.Q.ToByteArray());
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}