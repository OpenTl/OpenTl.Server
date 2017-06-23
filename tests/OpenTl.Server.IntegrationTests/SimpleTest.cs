using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using OpenTl.Schema;
using OpenTl.Schema.Serialization;
using Xunit;

namespace OpenTl.Server.IntegrationTests
{
    public class SimpleTest
    {
        private static readonly Random Random = new Random();

        [Fact]
        public async Task RequestReqPqTest()
        {
            var client = new TcpClient();
            await client.ConnectAsync("localhost", 433);

            var networkStream = client.GetStream();

            var data = new byte[16];
            Random.NextBytes(data);

            var request = new RequestReqPq {Nonce = data};

            var stream = Serializer.SerializeObject(request);
            await stream.CopyToAsync(networkStream);

            using (var streamReader = new BinaryReader(networkStream, Encoding.UTF8, true))
            {
                var response = (TResPQ)Serializer.DeserializeObject(streamReader);
                
                Assert.Equal(data, response.Nonce);
                Assert.Equal(16, response.ServerNonce.Length);
                Assert.NotEmpty(response.Pq);
                Assert.Equal(new List<long>{1, 2, 3, 4, 5, 6, 7, 8, 9, 10}, response.ServerPublicKeyFingerprints.Items);
            }
        }
    }
}
