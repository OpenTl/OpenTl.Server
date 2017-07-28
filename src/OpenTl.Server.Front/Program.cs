using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using OpenTl.Server.Back.Contracts;
using Orleans;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;

namespace OpenTl.Server.Front
{
    using DotNetty.Buffers;
    using DotNetty.Codecs;

    class Program
    {
        private static void Main()
        {
            InitializeOrleans();
            RunDotNettyAsync().Wait();   
        }

        private static async Task RunDotNettyAsync()
        {
            var bossGroup = new MultithreadEventLoopGroup(18);
            var workerGroup = new MultithreadEventLoopGroup();

            try
            {
                var bootstrap = new ServerBootstrap();
                bootstrap
                    .Group(bossGroup, workerGroup)
                    .Channel<TcpServerSocketChannel>()
                    .Option(ChannelOption.SoBacklog, 100)
                    .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        var pipeline = channel.Pipeline;
                        
                        pipeline.AddLast(new LengthFieldBasedFrameDecoder(ByteOrder.LittleEndian, int.MaxValue, 0, 4, -4, 0, true));
                        pipeline.AddLast(new MessageDecoder());
                        pipeline.AddLast(new MessageEncoder());
                        pipeline.AddLast(new MessageHandler());
                    }));

                var boundChannel = await bootstrap.BindAsync(433);

                Console.ReadLine();

                await boundChannel.CloseAsync();
            }
            finally
            {
                await Task.WhenAll(
                    bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
                    workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
            }
        }

        private static int InitializeOrleans()
        {
            var config = ClientConfiguration.LocalhostSilo();
            try
            {
                InitializeOrleansWithRetries(config, initializeAttemptsBeforeFailing: 5);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Orleans client initialization failed failed due to {ex}");

                Console.ReadLine();
                return 1;
            }

            return 0;
        }

        private static void InitializeOrleansWithRetries(ClientConfiguration config, int initializeAttemptsBeforeFailing)
        {
            config.TraceFilePattern = null;
            
            var attempt = 0;
            while (true)
            {
                try
                {
                    GrainClient.Initialize(config);
                    Console.WriteLine("Client successfully connect to silo host");
                    break;
                }
                catch (SiloUnavailableException)
                {
                    attempt++;
                    Console.WriteLine($"Attempt {attempt} of {initializeAttemptsBeforeFailing} failed to initialize the Orleans client.");
                    if (attempt > initializeAttemptsBeforeFailing)
                    {
                        throw;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }
        }
    }
}