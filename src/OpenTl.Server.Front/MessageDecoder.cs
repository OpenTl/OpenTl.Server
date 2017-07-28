namespace OpenTl.Server.Front
{
    using System.Collections.Generic;

    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Transport.Channels;

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

            var chechsum = buffer.ReadInt();

            output.Add(data);
        }
    }
}