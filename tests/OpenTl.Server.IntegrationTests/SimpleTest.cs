using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OpenTl.Server.IntegrationTests
{
    public class SimpleTest
    {
        [Fact]
        public async Task SendHello()
        {
            TcpClient client = new TcpClient();
            await client.ConnectAsync("localhost", 433);

            var networkStream = client.GetStream();

            for (int i = 0; i < 150; i++)
            {
                using (var stream = new MemoryStream())
                {
                    using (var streamWriter = new BinaryWriter(stream, Encoding.UTF8, true))
                    {
                        streamWriter.Write(1);
                        streamWriter.Write("kots");
                    }

                    stream.Seek(0, SeekOrigin.Begin);
                    await stream.CopyToAsync(networkStream);
                }

                using (var streamReader = new BinaryReader(networkStream, Encoding.UTF8, true))
                {
                    Assert.Equal(2, streamReader.ReadInt32());
                    Assert.Equal("kots, Hello!", streamReader.ReadString());
                }
            }
        }

        [Fact]
        public void SendHelloParallel()
        {
            Parallel.For(0, 1000, i =>
            {
                SendHello().Wait();
            });
        }
    }
}
