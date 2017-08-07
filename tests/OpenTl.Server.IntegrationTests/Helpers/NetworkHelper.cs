namespace OpenTl.Server.IntegrationTests.Helpers
{
    using System.IO;
    using System.Text;

    using OpenTl.Common.Crypto;

    public static class NetworkHelper
    {
        public static byte[] EncodeMessage(byte[] bytes, int seqNumber)
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

        public static byte[] DecodeMessage(Stream stream, out int seqNum, out int checksum)
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