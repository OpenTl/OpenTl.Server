using System;
using System.Threading.Tasks;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using OpenTl.Server.Back.Contracts;
using Orleans;
using Orleans.Runtime;

namespace OpenTl.Server.Front
{
    public class ClientHandler: ChannelHandlerAdapter
    {
        private readonly IPackageRouterGrain _router = GrainClient.GrainFactory.GetGrain<IPackageRouterGrain>(0);

        private Guid _clientId;

        public override void ChannelActive(IChannelHandlerContext context)
        {
            _clientId = Guid.NewGuid();
            
            base.ChannelActive(context);
        }
        
        public override void ChannelRead(IChannelHandlerContext ctx, object msg)
        {
            base.ChannelRead(ctx, msg);

            var buffer = (IByteBuffer) msg;
            
            var data = new byte[buffer.ReadableBytes];

            buffer.GetBytes(buffer.ReaderIndex, data);
            

            Task.Run(async () =>
            {
                var resultData = await _router.Handle(_clientId, data);

                var resultBuffer = Unpooled.WrappedBuffer(resultData);
                await ctx.WriteAndFlushAsync(resultBuffer);

            });

        }

    }
}