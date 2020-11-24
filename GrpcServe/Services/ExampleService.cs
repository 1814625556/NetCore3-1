using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServe.Services
{
    public class ExampleService : Example.ExampleBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ExampleResponse> UnaryCall(ExampleRequest request,
            ServerCallContext context)
        {
            var response = new ExampleResponse() { Result = $"{request.ReqStr} World"};
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ExampleResponse> SayHello(ExampleRequest request, ServerCallContext context)
        {
            var userAgent = context.RequestHeaders.ToList();
            var tempKey = userAgent[0].Key;
            var tempVal_ = userAgent[0].Value;
            return Task.FromResult(new ExampleResponse
            {
                Result = request.ReqStr + "World"
            }) ;
        }

        /// <summary>
        /// 服务端流
        /// </summary>
        /// <param name="request"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task StreamingFromServer(ExampleRequest request,
        IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            for (var i = 0; i < 5; i++)
            {
                await responseStream.WriteAsync(new ExampleResponse() { Result=$"StreamingFromServer {i}"});
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        /// <summary>
        /// 客户端流
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<ExampleResponse> StreamingFromClient(
        IAsyncStreamReader<ExampleRequest> requestStream, ServerCallContext context)
        {
            var msg = new List<ExampleRequest>();
            while (await requestStream.MoveNext())
            {
                msg.Add(requestStream.Current);
                Console.WriteLine($"StreamingFromClient:{DateTime.Now.ToString()}");
            }

            var msgResult = "";
            foreach (var cli in msg)
            {
                msgResult += cli.ReqStr;
            }
            return new ExampleResponse()
            {
               Result = $"serve: {DateTime.Now.ToString()}"
            };
        }

        /// <summary>
        /// 双向流
        /// </summary>
        /// <param name="requestStream"></param>
        /// <param name="responseStream"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task StreamingBothWays(IAsyncStreamReader<ExampleRequest> requestStream,
        IServerStreamWriter<ExampleResponse> responseStream, ServerCallContext context)
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {
                Console.WriteLine($"message from client {message.ReqStr}");
                await responseStream.WriteAsync(new ExampleResponse() {

                    Result = $"serve: {DateTime.Now.ToString()}"
                });
            }
        }

    }
}
