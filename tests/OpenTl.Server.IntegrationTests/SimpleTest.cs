using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using OpenTl.Schema;
using OpenTl.Schema.Serialization;
using OpenTl.Utils.Crypto;
using Xunit;

namespace OpenTl.Server.IntegrationTests
{
    public class SimpleTest
    {
        private static readonly Random Random = new Random();

        [Fact]
        public async Task RequestReqPqTest()
        {
            var networkStream = await GetServerStream();

            var nonce = new byte[16];
            
            var response = await GetReqPq(networkStream, nonce);

            Assert.Equal(nonce, response.Nonce);
            Assert.Equal(16, response.ServerNonce.Length);
            Assert.NotEmpty(response.Pq);
            Assert.Equal(new List<long> {1507157865616355199}, response.ServerPublicKeyFingerprints.Items);
        }
        
        [Fact]
        public async Task RequestReqDhExchangeTest()
        {
            var networkStream = await GetServerStream();

            var nonce = new byte[16];
            var reqPqResponse = await GetReqPq(networkStream, nonce);

            var pqData = SerializationUtils.GetBytes(reqPqResponse.Pq);

            var pqPair = Factorizator.Factorize(new BigInteger(pqData));

//            var p = SerializationUtils.GetString(pqPair.Min().ToByteArray());
//            var q = SerializationUtils.GetString(pqPair.Max().ToByteArray());
//
//            var newNonce = new byte[32];
//            Random.NextBytes(newNonce);
//            
//            var pqInnerData = new TPQInnerData
//            {
//                Pq = reqPqResponse.Pq,
//                P = p,
//                Q = q,
//                ServerNonce = reqPqResponse.ServerNonce,
//                Nonce = reqPqResponse.Nonce,
//                NewNonce = newNonce    
//            };
//
//            var innerdata = Serializer.SerializeObjectWithoutBuffer(pqInnerData);
//
//            var fingerprint = reqPqResponse.ServerPublicKeyFingerprints[0];
//            var chippertext = Rsa.Encrypt(fingerprint, innerdata, 0, innerdata.Length);
//
//            var request = new RequestReqDHParams
//            {
//                Nonce = reqPqResponse.Nonce,
//                P = p,
//                Q = q,
//                ServerNonce = reqPqResponse.ServerNonce,
//                PublicKeyFingerprint = fingerprint,
//                EncryptedData = SerializationUtils.GetString(chippertext)
//            };
//
//            var requestData = Serializer.SerializeObjectWithBuffer(request);
//            await networkStream.WriteAsync(requestData, 0, requestData.Length);

//            Assert.Equal(nonce, reqPqResponse.Nonce);
//            Assert.Equal(16, reqPqResponse.ServerNonce.Length);
//            Assert.NotEmpty(reqPqResponse.Pq);
//            Assert.Equal(new List<long> {1, 2, 3, 4, 5, 6, 7, 8, 9, 10}, reqPqResponse.ServerPublicKeyFingerprints.Items);
        }

        private static async Task<TResPQ> GetReqPq(Stream networkStream, byte[] nonce)
        {
            Random.NextBytes(nonce);

            var request = new RequestReqPq {Nonce = nonce};

            var binary = Serializer.SerializeObjectWithBuffer(request);
            await networkStream.WriteAsync(binary, 0, binary.Length);

            using (var streamReader = new BinaryReader(networkStream, Encoding.UTF8, true))
            {
                return (TResPQ) Serializer.DeserializeObject(streamReader);
            }
        }

        private static async Task<NetworkStream> GetServerStream()
        {
            var client = new TcpClient();
            await client.ConnectAsync("localhost", 433);

            var networkStream = client.GetStream();
            return networkStream;
        }
    }
}
