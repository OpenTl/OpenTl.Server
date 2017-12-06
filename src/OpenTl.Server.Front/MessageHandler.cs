using System;
using System.Threading.Tasks;
using DotNetty.Transport.Channels;
using OpenTl.Server.Back.Contracts;
using Orleans;

namespace OpenTl.Server.Front
{
    using System.Collections.Concurrent;

    public class MessageHandler: ChannelHandlerAdapter
    {
        private readonly IPackageRouterGrain _router = GrainClient.GrainFactory.GetGrain<IPackageRouterGrain>(0);

        private Guid _clientId;
        
        private static readonly ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, IChannelHandlerContext>> Sessions = new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, IChannelHandlerContext>>();
        private static readonly ConcurrentDictionary<Guid, Guid> Connections = new ConcurrentDictionary<Guid, Guid>();

        public override void ChannelActive(IChannelHandlerContext context)
        {
            _clientId = Guid.NewGuid();

            base.ChannelActive(context);
        }
         
        public override void ChannelRead(IChannelHandlerContext ctx, object msg)
        {
            Task.Run(async () =>
            {
                var resultData = await _router.Handle(_clientId, (byte[]) msg);

                if (resultData.Item1.HasValue)
                {
                    var sessionId = resultData.Item1.Value;
                    
                    var currentSession = Sessions.GetOrAdd(sessionId, guid => new ConcurrentDictionary<Guid, IChannelHandlerContext>());
                    currentSession.GetOrAdd(_clientId, ctx);
                    
                    Connections.GetOrAdd(_clientId, sessionId);
                } 
                
                await ctx.WriteAndFlushAsync(resultData.Item2);
            });
        }

        public override void ChannelInactive(IChannelHandlerContext context)
        {
            if (Connections.TryRemove(_clientId, out var sessionId))
            {
                var connection = Sessions[sessionId];
                
                if (connection.TryRemove(_clientId, out var _) && connection.IsEmpty)
                {
                    Sessions.TryRemove(sessionId, out var _);
                }
            }
            
            base.ChannelInactive(context);
        }
    }
}