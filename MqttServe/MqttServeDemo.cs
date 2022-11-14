using Microsoft.Extensions.Logging;
using MQTTnet.Server;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet.Protocol;

namespace MqttServe;
public static class MqttServeDemo
{
    public static async Task Force_Disconnecting_Client()
    {
        /*
         * This sample will disconnect a client.
         *
         * See _Run_Minimal_Server_ for more information.
         */

        using (var mqttServer = await StartMqttServer())
        {
            // Let the client connect.
            await Task.Delay(TimeSpan.FromSeconds(5));

            // Now disconnect the client (if connected).
            var affectedClient = (await mqttServer.GetClientsAsync()).FirstOrDefault(c => c.Id == "MyClient");
            if (affectedClient != null)
            {
                await affectedClient.DisconnectAsync();
            }
        }
    }

    public static async Task Publish_Message_From_Broker()
    {
        /*
         * This sample will publish a message directly at the broker.
         *
         * See _Run_Minimal_Server_ for more information.
         */

        using (var mqttServer = await StartMqttServer())
        {
            // Create a new message using the builder as usual.
            var message = new MqttApplicationMessageBuilder().WithTopic("HelloWorld").WithPayload("Test").Build();

            // Now inject the new message at the broker.
            await mqttServer.InjectApplicationMessage(
                new InjectedMqttApplicationMessage(message)
                {
                    SenderClientId = "SenderClientId"
                });
        }
    }

    /// <summary>
    /// 默认的ip 和 端口号 是 127.0.0.1，1883
    /// </summary>
    /// <returns></returns>
    public static async Task Run_Minimal_Server()
    {
        var mqttFactory = new MqttFactory();
        var mqttServerOptions = new MqttServerOptionsBuilder().WithDefaultEndpoint().Build();
        using (var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions))
        {
            await mqttServer.StartAsync();

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            // Stop and dispose the MQTT server if it is no longer needed!
            await mqttServer.StopAsync();
        }
    }

    static async Task<MqttServer> StartMqttServer()
    {
        var mqttFactory = new MqttFactory();

        // Due to security reasons the "default" endpoint (which is unencrypted) is not enabled by default!
        var mqttServerOptions = mqttFactory.CreateServerOptionsBuilder().WithDefaultEndpoint().Build();
        var server = mqttFactory.CreateMqttServer(mqttServerOptions);
        await server.StartAsync();
        return server;
    }
}
