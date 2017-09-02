namespace OpenTl.Server.IntegrationTests.Helpers
{
    using System.IO;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    using OpenTl.Common.Crypto;
    using OpenTl.Common.MtProto;
    using OpenTl.Schema;
    using OpenTl.Schema.Serialization;
    using OpenTl.Server.IntegrationTests.Entities;

    public static class NetworkHelper
    {
        public static async Task<NetworkStream> GetServerStream()
        {
            var client = new TcpClient();
            await client.ConnectAsync("localhost", 433);

            return client.GetStream();
        }

        public static IObject SendAndRecieve(this NetworkStream networkStream, IObject request, TestSession testSession)
        {
            var package = Serializer.SerializeObject(request);
            var data = SendAndRecieve(networkStream, package, testSession);
            return Serializer.DeserializeObject(data);
        }

        public static byte[] SendAndRecieve(this NetworkStream networkStream, byte[] package, TestSession testSession)
        {
            var reqMessage = EncodeMessage(package, testSession);

            networkStream.Write(reqMessage, 0, reqMessage.Length);

            var data = DecodeMessage(networkStream, out var seqNum, out var checksum);
            return data;
        }

        public static IObject EncryptionSendAndRecieve(this NetworkStream networkStream, IObject request, TestSession testSession)
        {
            var requestData = Serializer.SerializeObject(request);
            var encryptedRequestData = MtProtoHelper.FromClientEncrypt(requestData, testSession, testSession.SeqNumber);
            testSession.SeqNumber++;

            var encryptedResponseData = networkStream.SendAndRecieve(encryptedRequestData, testSession);
            var responseData = MtProtoHelper.FromServerDecrypt(encryptedResponseData, testSession, out var authKeyId, out var serverSalt, out var sessionId, out var messageId, out var sNumber);

            return Serializer.DeserializeObject(responseData);
        }

        private static byte[] EncodeMessage(byte[] bytes, TestSession session)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(bytes.Length + 12);
                writer.Write(session.SeqNumber);
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