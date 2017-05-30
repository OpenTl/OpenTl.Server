using System;
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
        }
    }
}
