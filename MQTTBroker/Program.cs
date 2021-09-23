using MQTTnet;
using MQTTnet.Server;
using System;
using System.IO;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MQTTBroker
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var certificate = new X509Certificate2(Path.Combine(currentPath, "certificate.pfx"), "dMHIwO/6bkUcYXiKeMx1pQgRDxmfc92hPNvI2ErvFvk=", X509KeyStorageFlags.Exportable);

            var optionsBuilder = new MqttServerOptionsBuilder()
                //.WithoutDefaultEndpoint() // This call disables the default unencrypted endpoint on port 1883
                .WithEncryptedEndpoint()
                .WithEncryptedEndpointPort(8883)
                .WithEncryptionCertificate(certificate.Export(X509ContentType.Pfx))
                .WithEncryptionSslProtocol(SslProtocols.Tls12)
                .WithPersistentSessions()
                .Build();

            var mqttServer = new MqttFactory().CreateMqttServer();
            await mqttServer.StartAsync(optionsBuilder);
            mqttServer.UseApplicationMessageReceivedHandler(HandleApplicationMessageReceived);
            Console.WriteLine("Press any key to exit.");
            Console.ReadLine();
            await mqttServer.StopAsync();
        }

        public static void HandleApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine("Client " + e.ClientId);
            Console.WriteLine("Topic" + e.ApplicationMessage.Topic);
            Console.WriteLine("Payload " + Encoding.Default.GetString(e.ApplicationMessage.Payload));
            Console.WriteLine();
        }
    }
}
