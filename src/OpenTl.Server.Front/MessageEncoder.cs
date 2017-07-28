namespace OpenTl.Server.Front
{
    using DotNetty.Buffers;
    using DotNetty.Codecs;
    using DotNetty.Transport.Channels;

    public class MessageEncoder: MessageToByteEncoder<byte[]>
    {
        protected override void Encode(IChannelHandlerContext context, byte[] message, IByteBuffer output)
        {
            var buffer = new SwappedByteBuffer(output);

            buffer.WriteInt(message.Length + 12);
            buffer.WriteInt(0);
            buffer.WriteBytes(message);
            buffer.WriteInt(0);
        }
    }
}