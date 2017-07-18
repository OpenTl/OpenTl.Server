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

    public class SimpleTest
    {
        private const string PublicKey = 
@"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA8k1LTajz2N9RMjDnv1Jq
LUmE+MZWCOTMC0FqjZmjNWm9zhgy7Rv12nUz3I2rLiKEZp/O42ThfTtDgRVrFkkE
ALl0YWrWwt5QQq5k51POngCt9n6bLi2Q82HYMotvIhN6w15B692Urbu6RtDRnfdb
ojRDWGh/DNElRIuy+T1bWCIISCTX47PrXJs4I1WPw/xYDl9zA9xhEMIQx6NwoRlj
XBdWrYayFbllXqV3EPhhIpuQ6cqNsudbLGCFdsaRuNuTJzo43hf549xrumH0I/4K
1FLCbxCWz3gnWLaxoN+RtSJvXvyEfMFRdsWSW/mMr3WFwb8nakabgQ1YbtT5576M
OQIDAQAB
-----END PUBLIC KEY-----";

        [Fact]
        public async Task RequestReqPqTest()
        {
            var networkStream = await GetServerStream();

            var response = await GetReqPq(networkStream);
            var resPq = response.Item1;
            var nonce = response.Item2;
            
            Assert.Equal(nonce, resPq.Nonce);
            Assert.Equal(16, resPq.ServerNonce.Length);
            Assert.NotEmpty(resPq.Pq);
            Assert.Equal(new List<long> {1507157865616355199}, resPq.ServerPublicKeyFingerprints.Items);
        }
        
        [Fact]
        public async Task RequestReqDhExchangeTest()
        {
            var networkStream = await GetServerStream();

            var response = await GetReqPq(networkStream);
            var resPq = response.Item1;
            var nonce = response.Item2;

            var reqDhParams = Step2ClientHelper.GetRequest(resPq, PublicKey, out var newNonce);

            var reqDhParamsData = Serializer.SerializeObject(reqDhParams);
            
            await networkStream.WriteAsync(reqDhParamsData, 0, reqDhParamsData.Length);
        }

        private static async Task<Tuple<TResPQ, byte[]>> GetReqPq(Stream networkStream)
        {
            var resPq = Step1ClientHelper.GetRequest(out var nonce);

            var resPqData = Serializer.SerializeObject(resPq);
            
            await networkStream.WriteAsync(resPqData, 0, resPqData.Length);

            using (var streamReader = new BinaryReader(networkStream, Encoding.UTF8, true))
            {
                return Tuple.Create((TResPQ)Serializer.DeserializeObject(streamReader), nonce);
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
