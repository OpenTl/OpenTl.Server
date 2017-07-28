using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using OpenTl.Schema;
using OpenTl.Schema.Serialization;

using Xunit;

namespace OpenTl.Server.IntegrationTests
{
    using OpenTl.Common.Auth.Client;
    using OpenTl.Common.Crypto;

    using Org.BouncyCastle.Crypto;
    using Org.BouncyCastle.Crypto.Parameters;

    public class SimpleTest
    {
        private const string PublicKey = 
@"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA7Kh5FpK5KxFNFUSQ8yWK
GzBW4OJX+ju3S2zX179uJFqisgaC256UgI4UsUfZhR38zi6g4AZqlOUOZcLNwp3r
6zK0ujxmZNu1M3LQLcS1D4aoWHZRHnEz0wuZqhOnIOXA31ATB8kCqiIu0FbKrZ/5
HnPfdDFXt6bfv3+vgyyJwI/G8umiLHo0DSBuXnxo4v6/9hEQcj8acZ26sNj3u9N8
M1WncvMct9os0FJWKaeJ0BUatxHrZC/xsaW5nS9f6Pjw9TfMwuU9qnEZye4Gmgu8
6lv1fbUcg0zl0S4FQpDnf0aKOaU2+DiNxM6DqCBJs7Fz9OZ+LB7bw+5006/defll
cQIDAQAB
-----END PUBLIC KEY-----";

        [Fact]
        public async Task Step1Test()
        {
            var networkStream = await GetServerStream();

            var requestReqPq = Step1ClientHelper.GetRequest(out var nonce);

            var resPq = GetStep1Response(networkStream, requestReqPq);
            
            Assert.Equal(nonce, resPq.Nonce);
            Assert.Equal(16, resPq.ServerNonce.Length);
            Assert.NotEmpty(resPq.Pq);
            Assert.Equal(new List<long> {RSAHelper.GetFingerprint(PublicKey)}, resPq.ServerPublicKeyFingerprints.Items);
        }
        
        [Fact]
        public async Task Step2Test()
        {
            var networkStream = await GetServerStream();

            var requestReqPq = Step1ClientHelper.GetRequest(out var nonce);
            
            var resPq = GetStep1Response(networkStream, requestReqPq);

            var response =  GetStep2Response(resPq, networkStream, out var newNonce);

            Step3ClientHelper.GetRequest(response, newNonce, out var clientKeyPair, out var serverPublicKey);
            
        }
        
        [Fact]
        public async Task Step3Test()
        {
            var networkStream = await GetServerStream();

            var requestReqPq = Step1ClientHelper.GetRequest(out var nonce);
            
            var resPq = GetStep1Response(networkStream, requestReqPq);

            var serverDhParams =  GetStep2Response(resPq, networkStream, out var newNonce);

            var response = GetStep3Response(networkStream, serverDhParams, newNonce, out var clientKeyPair, out var serverPublicKey);
        }

        private static TDhGenOk GetStep3Response(Stream networkStream, TServerDHParamsOk serverDhParams, byte[] newNonce, out byte[] clientAgree, out int serverTime)
        {
            var reqDhParams = Step3ClientHelper.GetRequest(serverDhParams, newNonce, out var agree, out var time);
            clientAgree = agree;
            serverTime = time;

            var reqDhParamsData = Serializer.SerializeObject(reqDhParams);

            networkStream.Write(reqDhParamsData, 0, reqDhParamsData.Length);
            
            using (var streamReader = new BinaryReader(networkStream, Encoding.UTF8, true))
            {
                return (TDhGenOk) Serializer.DeserializeObject(streamReader);
            }
        }
         
        private static TServerDHParamsOk  GetStep2Response(TResPQ resPq, Stream networkStream, out byte[] clientNonce)
        {
            var reqDhParams = Step2ClientHelper.GetRequest(resPq, PublicKey, out var newNonce);
            clientNonce = newNonce;

            var reqDhParamsData = Serializer.SerializeObject(reqDhParams);

            networkStream.Write(reqDhParamsData, 0, reqDhParamsData.Length);
            
            using (var streamReader = new BinaryReader(networkStream, Encoding.UTF8, true))
            {
                return (TServerDHParamsOk) Serializer.DeserializeObject(streamReader);
            }
        }
        
        private static TResPQ GetStep1Response(Stream networkStream, RequestReqPq resPq)
        {
            var resPqData = Serializer.SerializeObject(resPq);
            
            networkStream.Write(resPqData, 0, resPqData.Length);

            using (var streamReader = new BinaryReader(networkStream, Encoding.UTF8, true))
            {
                return (TResPQ)Serializer.DeserializeObject(streamReader);
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
