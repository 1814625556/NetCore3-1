using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Packets;
using MQTTnet.Protocol;

namespace MqttClient;
public class MqttClientDemo
{
    #region publish
    public static async Task Publish_Application_Message()
    {
        /*
         * This sample pushes a simple application message including a topic and a payload.
         *
         * Always use builders where they exist. Builders (in this project) are designed to be
         * backward compatible. Creating an _MqttApplicationMessage_ via its constructor is also
         * supported but the class might change often in future releases where the builder does not
         * or at least provides backward compatibility where possible.
         */

        var muttFactory = new MqttFactory();

        using var mqttClient = muttFactory.CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("broker.hivemq.com")
            .Build();

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic("samples/temperature/living_room")
            .WithPayload("19.5")
            .Build();

        await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

        await mqttClient.DisconnectAsync();

        Console.WriteLine("MQTT application message is published.");
    }

    public static async Task Publish_Multiple_Application_Messages()
    {
        /*
         * This sample pushes multiple simple application message including a topic and a payload.
         *
         * See sample _Publish_Application_Message_ for more details.
         */

        var mqttFactory = new MqttFactory();

        using (var mqttClient = mqttFactory.CreateMqttClient())
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.hivemq.com")
                .Build();

            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic("samples/temperature/living_room")
                .WithPayload("19.5")
                .Build();

            await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

            applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic("samples/temperature/living_room")
                .WithPayload("20.0")
                .Build();

            await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

            applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic("samples/temperature/living_room")
                .WithPayload("21.0")
                .Build();

            await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

            await mqttClient.DisconnectAsync();

            Console.WriteLine("MQTT application message is published.");
        }
    }
    #endregion

    #region subscripe
    public static async Task Handle_Received_Application_Message()
    {
        /*
         * This sample subscribes to a topic and processes the received message.
         */

        var mqttFactory = new MqttFactory();

        using (var mqttClient = mqttFactory.CreateMqttClient())
        {
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("127.0.0.1", 1883).Build();

            // Setup message handling before connecting so that queued messages
            // are also handled properly. When there is no event handler attached all
            // received messages get lost.
            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                Console.WriteLine($"Received application message:{Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                //e.DumpToConsole();
                Console.WriteLine();
                Console.WriteLine("----------------------------------------------------------");
                Console.WriteLine();
                return Task.CompletedTask;
            };

            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("chenchang");
                    })
                .Build();

            await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

            Console.WriteLine("MQTT client subscribed to topic.");

            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }
    }

    public static async Task Send_Responses()
    {
        /*
         * This sample subscribes to a topic and sends a response to the broker. This requires at least QoS level 1 to work!
         */

        var mqttFactory = new MqttFactory();

        using (var mqttClient = mqttFactory.CreateMqttClient())
        {
            mqttClient.ApplicationMessageReceivedAsync += delegate (MqttApplicationMessageReceivedEventArgs args)
            {
                // Do some work with the message...

                // Now respond to the broker with a reason code other than success.
                args.ReasonCode = MqttApplicationMessageReceivedReasonCode.ImplementationSpecificError;
                args.ResponseReasonString = "That did not work!";

                // User properties require MQTT v5!
                args.ResponseUserProperties.Add(new MqttUserProperty("My", "Data"));

                // Now the broker will resend the message again.
                return Task.CompletedTask;
            };

            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("broker.hivemq.com").Build();

            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("mqttnet/samples/topic/1");
                    })
                .Build();

            var response = await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

            Console.WriteLine("MQTT client subscribed to topic.");

            // The response contains additional data sent by the server after subscribing.
            response.DumpToConsole();
        }
    }

    public static async Task Subscribe_Multiple_Topics()
    {
        /*
         * This sample subscribes to several topics in a single request.
         */

        var mqttFactory = new MqttFactory();

        using (var mqttClient = mqttFactory.CreateMqttClient())
        {
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("broker.hivemq.com").Build();

            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            // Create the subscribe options including several topics with different options.
            // It is also possible to all of these topics using a dedicated call of _SubscribeAsync_ per topic.
            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("mqttnet/samples/topic/1");
                    })
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("mqttnet/samples/topic/2").WithNoLocal();
                    })
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("mqttnet/samples/topic/3").WithRetainHandling(MqttRetainHandling.SendAtSubscribe);
                    })
                .Build();

            var response = await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

            Console.WriteLine("MQTT client subscribed to topics.");

            // The response contains additional data sent by the server after subscribing.
            response.DumpToConsole();
        }
    }

    public static async Task Subscribe_Topic()
    {
        /*
         * This sample subscribes to a topic.
         */

        var mqttFactory = new MqttFactory();

        using (var mqttClient = mqttFactory.CreateMqttClient())
        {
            var mqttClientOptions = new MqttClientOptionsBuilder().WithTcpServer("broker.hivemq.com").Build();

            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
                .WithTopicFilter(
                    f =>
                    {
                        f.WithTopic("mqttnet/samples/topic/1");
                    })
                .Build();

            var response = await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

            Console.WriteLine("MQTT client subscribed to topic.");

            // The response contains additional data sent by the server after subscribing.
            response.DumpToConsole();
        }
    }
    #endregion
}
