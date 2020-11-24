using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServe.Services
{
    public class GreeterService : Greeter.GreeterBase 
    {
        public override async Task<HelloReply> SayHello(HelloRequest req, ServerCallContext context)
        {
            return await Task.FromResult(new HelloReply() { Message = "Good" });
        }

        public override async Task SayRepeatHello(RepeatHelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            for (var i = 0; i < 5; i++)
            {
                await responseStream.WriteAsync(new HelloReply() { Message = $"StreamingFromServer {i}" });
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }
    }
}
