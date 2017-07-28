namespace OpenTl.Server.Front
{
    using System.Collections.Generic;

    using BarsGroup.CodeGuard;

    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Transport.Channels;

    using OpenTl.Common.Crypto;

    public class MessageDecoder: ByteToMessageDecoder
    {
        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            if (input is EmptyByteBuffer)
            {
                return;
            }
            
            var buffer = new SwappedByteBuffer(input);
            
            var length = buffer.ReadInt();
            var seqNumber = buffer.ReadInt();

            var data = new byte[length - 12]; 
            buffer.ReadBytes(data);

            CheckChecksum(buffer, length);
            
            output.Add(data);
        }

        private static void CheckChecksum(IByteBuffer buffer, int length)
        {
            buffer.ResetReaderIndex();
            var checksumBuffer = buffer.ReadBytes(length - 4);
            
            var checksum = buffer.ReadUnsignedInt();
            var computeChecksum = Crc32.Compute(checksumBuffer.ToArray());
            
            Guard.That(computeChecksum).IsEqual(checksum);
        }
    }
}