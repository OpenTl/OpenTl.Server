using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Xunit;

namespace OpenTl.Server.IntegrationTests
{
    public class UnitTest1
    {
        [Fact]
        public async Task Test1()
        {
            TcpClient client = new TcpClient();
            await client.ConnectAsync("localhost", 433);

            var stream = client.GetStream();
            StreamWriter streamWriter = new StreamWriter(stream);
            await streamWriter.WriteLineAsync("kots");
            streamWriter.Flush();
                
            StreamReader streamReader = new StreamReader(stream);
            var line = await streamReader.ReadLineAsync();

            Assert.Equal("kots, Hello!", line);
        }
    }
}
