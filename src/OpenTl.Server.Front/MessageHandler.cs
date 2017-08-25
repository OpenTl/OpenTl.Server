using System;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using OpenTl.Server.Back.Contracts;
using Orleans;

namespace OpenTl.Server.Front
{
    public class MessageHandler: ChannelHandlerAdapter
    {
        private static readonly Random Random = new Random();

        private readonly IPackageRouterGrain _router = GrainClient.GrainFactory.GetGrain<IPackageRouterGrain>(0);

        private ulong _clientId;

        public override void ChannelActive(IChannelHandlerContext context)
        {
            var rand = new byte[8];
            Random.NextBytes(rand);
            _clientId = BitConverter.ToUInt64(rand, 0);

            base.ChannelActive(context);
        }
         
        public override void ChannelRead(IChannelHandlerContext ctx, object msg)
        {
            Task.Run(async () =>
            {
                var resultData = await _router.Handle(_clientId, (byte[]) msg);

                await ctx.WriteAndFlushAsync(resultData);
            });
        }
    }
}