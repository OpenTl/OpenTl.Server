using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using OpenTl.Server.Back.Contracts;
using Orleans;

namespace OpenTl.Server.Front
{
    public class ClientHandler: ChannelHandlerAdapter
    {
        public override void ChannelRead(IChannelHandlerContext ctx, object msg)
        {
            var buffer = (IByteBuffer) msg;
            var data = new byte[buffer.ReadableBytes];

            buffer.GetBytes(buffer.ReaderIndex, data);

            var name = Encoding.UTF8.GetString(data);

            name = name.TrimEnd('\n', '\r');

            Task.Run(async () =>
            {
                var friend = GrainClient.GrainFactory.GetGrain<IHello>(0);
                var result = await friend.SayHello(name) + "\n\r";

                var resultData = Encoding.UTF8.GetBytes(result);

                IByteBuffer resultBuffer = Unpooled.WrappedBuffer(resultData);
                await ctx.WriteAndFlushAsync(resultBuffer);
            });

            base.ChannelRead(ctx, msg);
        }

    }
}