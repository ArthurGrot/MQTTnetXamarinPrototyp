using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;
using MQTTnet.Client.Unsubscribing;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MQTTnetXamarinPrototyp.Services
{
    public class MQTTClient
    {
        private IManagedMqttClient mqttClient;
        private IMqttClientOptions clientOptions;
        private MqttClientDisconnectOptions mqttClientDisconnectOptions;
        private ManagedMqttClientOptions managedClientOptions;
        private MqttClientOptionsBuilderTlsParameters tlsOptions;

        public event EventHandler MqttClientConnectionsResultChanged;

        public event EventHandler MessageReceived;
        public MQTTClientConnectionState MQTTClientConnectionState { get; set; }

        public string ClientID { get; set; }


        public MQTTClient()
        {
            MQTTClientConnectionState = MQTTClientConnectionState.Disconnected;

            Random random = new Random();
            ClientID = "Client " + random.Next(100).ToString();


            tlsOptions = new MqttClientOptionsBuilderTlsParameters()
            {
                UseTls = true,
                AllowUntrustedCertificates = true,
            };

            clientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer("192.168.178.28")
                    //.WithWebSocketServer("192.168.178.28/mqtt")
                    .WithClientId(ClientID)
                    .WithKeepAlivePeriod(TimeSpan.FromSeconds(5))
                    .WithTls(tlsOptions)
                    .Build();

            managedClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromMilliseconds(5000))
                .WithClientOptions(clientOptions)
                .Build();

            mqttClient = new MqttFactory().CreateManagedMqttClient();

            mqttClient.UseConnectedHandler(this.HandleUseConnected);
            mqttClient.UseDisconnectedHandler(this.HandleUseDisconnected);
            mqttClient.UseApplicationMessageReceivedHandler(this.HandleApplicationMessageReceived);

            mqttClientDisconnectOptions = new MqttClientDisconnectOptions();
        }

        public async Task ConnectAsync()
        {
            try
            {
                if (MQTTClientConnectionState != MQTTClientConnectionState.Connected)
                {
                    MQTTClientConnectionState = MQTTClientConnectionState.Connecting;
                    MqttClientConnectionsResultChanged?.Invoke(this, null);
                    await mqttClient.StartAsync(managedClientOptions);
                }
            }
            catch (Exception ex)
            {

                await Application.Current.MainPage.DisplayAlert("MQTT Connection", "Connection error", "Ok");
            }
        }

        public async Task DisconnectAsync()
        {
            if (MQTTClientConnectionState != MQTTClientConnectionState.Disconnected)
            {
                MQTTClientConnectionState = MQTTClientConnectionState.Disconnecting;
                MqttClientConnectionsResultChanged?.Invoke(this, null);
                await mqttClient.StopAsync();
                mqttClient.Dispose();
            }
        }

        public async Task SubscribeToTopicAsync(string topic)
        {
            try
            {
                var topicFilter = new MqttTopicFilterBuilder().WithTopic(topic).Build();
                await mqttClient.SubscribeAsync(topicFilter);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task UnsubcribeFromTopicAsync(string topic)
        {
            await mqttClient.UnsubscribeAsync(topic);
        }

        public async Task PublishMessageToTopic(string topic, string payload)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithRetainFlag(false)
                .WithExactlyOnceQoS()
                .Build();

            await mqttClient.PublishAsync(message);
        }

        public void HandleUseConnected(MqttClientConnectedEventArgs e)
        {
            if (e.AuthenticateResult.ResultCode == MqttClientConnectResultCode.Success)
            {
                MQTTClientConnectionState = MQTTClientConnectionState.Connected;
                MqttClientConnectionsResultChanged?.Invoke(this, e);
            }
        }

        public void HandleUseDisconnected(MqttClientDisconnectedEventArgs e)
        {
            MQTTClientConnectionState = MQTTClientConnectionState.Disconnected;
            MqttClientConnectionsResultChanged?.Invoke(this, e);
        }

        public void HandleApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }
    }
}
