using System.Text;
using System.Threading;
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

        private static int _clientNumber;

        private int _clientId;
        private int _packageId;

        public override void ChannelActive(IChannelHandlerContext context)
        {
            Interlocked.Increment(ref _clientNumber);
            _clientId = _clientNumber;

            base.ChannelActive(context);
        }

        public override void ChannelRead(IChannelHandlerContext ctx, object msg)
        {
            var buffer = (IByteBuffer) msg;
            var data = new byte[buffer.ReadableBytes];

            buffer.GetBytes(buffer.ReaderIndex, data);

            var packageId = ++_packageId;

            Task.Run(async () =>
            {
                GrainClient.Logger.Info($"client id = {_clientId}, packageId = {packageId} - IN");

                var resultData = await _router.Handle(data);

                var resultBuffer = Unpooled.WrappedBuffer(resultData);
                await ctx.WriteAndFlushAsync(resultBuffer);

                GrainClient.Logger.Info($"client id = {_clientId}, packageId = {packageId} - OUT");
            });

            base.ChannelRead(ctx, msg);
        }

    }
}