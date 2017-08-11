namespace OpenTl.Server.IntegrationTests.Helpers
{
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    using OpenTl.Common.Crypto;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;

    public static class NetworkHelper
    {
        public static async Task<NetworkStream> GetServerStream()
        {
            var client = new TcpClient();
            await client.ConnectAsync("localhost", 433);

            return client.GetStream();
        }
        
        public static byte[] SendAndRecive(this NetworkStream networkStream, byte[] package, int seqNumber)
        {
            var reqMessage = NetworkHelper.EncodeMessage(package, seqNumber);

            networkStream.Write(reqMessage, 0, reqMessage.Length);
            
            using (var streamReader = new BinaryReader(networkStream, Encoding.UTF8, true))
            {
                return NetworkHelper.DecodeMessage(networkStream, out var seqNum, out var checksum);
            }
        }
        
        private static byte[] EncodeMessage(byte[] bytes, int seqNumber)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(bytes.Length + 12);
                writer.Write(seqNumber);
                writer.Write(bytes);

                var checksum = Crc32.Compute(stream.ToArray());
                writer.Write(checksum);

                return stream.ToArray();
            }
        }

        private static byte[] DecodeMessage(Stream stream, out int seqNum, out int checksum)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var length = reader.ReadInt32();
                seqNum = reader.ReadInt32();
                var body = reader.ReadBytes(length - 12);
                
                checksum = reader.ReadInt32();

                return body;
            }
        }
    }
}