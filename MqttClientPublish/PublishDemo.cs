using MQTTnet.Client;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MqttClientPublish;
public class PublishDemo
{
    public static async Task Publish_Application_Message()
    {
        var muttFactory = new MqttFactory();

        using var mqttClient = muttFactory.CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("127.0.0.1", 1883)
            .Build();

        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        var applicationMessage = new MqttApplicationMessageBuilder()
            .WithTopic("chenchang")
            .WithPayload("Hello ChenChang...")
            .Build();

        await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

        await mqttClient.DisconnectAsync();

        Console.WriteLine("MQTT application message is published.");
    }
}
