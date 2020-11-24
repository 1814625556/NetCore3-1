using Grpc.Core;
using Grpc.Net.Client;
using GrpcServe;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Threading.Tasks;

namespace GrpcClientConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:16666");
            var client = new Example.ExampleClient(channel);
            var message = client.SayHello(new ExampleRequest());
            Console.WriteLine(message.Result);
            
            //ClientStreamCall(channel);
            //ServeStreamCall(channel);
            //await TwoWayStream(channel);
            Console.ReadKey();
        }

        /// <summary>
        /// 服务端流调用
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        static async Task ServeStreamCall(GrpcChannel channel)
        {
            var client = new Example.ExampleClient(channel);
            using var call = client.StreamingFromServer(new ExampleRequest 
            { ReqStr = "Hello " });
            while (await call.ResponseStream.MoveNext())
            {
                var response = call.ResponseStream.Current;
                Console.WriteLine($"res:{response.Result}");
            }
        }

       /// <summary>
       /// 客户端流式处理调用
       /// </summary>
       /// <param name="channel"></param>
       /// <returns></returns>
        static async Task ClientStreamCall(GrpcChannel channel)
        {
            var client = new Example.ExampleClient(channel);
            using var call = client.StreamingFromClient();

            for (var i = 0; i < 3; i++)
            {
                await call.RequestStream.WriteAsync(new  ExampleRequest { ReqStr = "Hello" });
                Console.WriteLine($"ClientStreamCall{DateTime.Now.ToString()}");
                await Task.Delay(2000);
            }
            //这里就是告诉服务端 我已经发送完毕了
            await call.RequestStream.CompleteAsync();

            var response = await call;
            Console.WriteLine($"res:{response.Result}");
        }

        /// <summary>
        /// 一元调用
        /// </summary>
        /// <returns></returns>
        static async Task UnaryCall(GrpcChannel channel)
        {
            var client = new Example.ExampleClient(channel);
            var response = await client.UnaryCallAsync(new ExampleRequest() { ReqStr = "Hello " });
            Console.WriteLine("Greeting: " + response.Result);
        }

        /// <summary>
        /// 全双流模式
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        static async Task TwoWayStream(GrpcChannel channel)
        {
            var client = new Example.ExampleClient(channel);
            using var call = client.StreamingBothWays();

            Console.WriteLine("Starting background task to receive messages");
            var readTask = Task.Run(async () =>
            {
                await foreach (var response in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine($"res:{response.Result}");
                }
            });

            Console.WriteLine("Starting to send messages");
            Console.WriteLine("Type a message to echo then press enter.");

            while (true)
            {
                var result = Console.ReadLine();
                if (string.IsNullOrEmpty(result))
                {
                    break;
                }

                await call.RequestStream.WriteAsync(new ExampleRequest { ReqStr = "Hello" });
            }
            Console.WriteLine("Disconnecting");
            await call.RequestStream.CompleteAsync();
            await readTask;
        }

    }
}
