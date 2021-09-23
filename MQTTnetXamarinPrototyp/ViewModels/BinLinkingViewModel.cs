using MessagePack;
using MQTTnet;
using MQTTnetXamarinPrototyp.Models;
using MQTTnetXamarinPrototyp.Services;
using MQTTnetXamarinPrototyp.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MQTTnetXamarinPrototyp.ViewModels
{
    class BinLinkingViewModel : BaseViewModel
    {
        private BinLinkingSynchronizationPage binLinkingSynchronizationPage;
        public ICommand NavigateToBinLinkSync { get; }
        public ICommand AddRandomLinking { get; }
        public ICommand RemoveBinLinking { get; }
        public static MQTTClient MQTT => DependencyService.Get<MQTTClient>();

        public BinLinkingViewModel()
        {
            Title = "BinLinking";

            if (binLinkingSynchronizationPage == null)
            {
                binLinkingSynchronizationPage = new BinLinkingSynchronizationPage();
            }

            NavigateToBinLinkSync = new Command(async () =>
                await MainThread.InvokeOnMainThreadAsync(async () =>
                    await Shell.Current.Navigation.PushAsync(binLinkingSynchronizationPage, true).ConfigureAwait(true)).ConfigureAwait(true)
            );

            AddRandomLinking = new Command(async () => await AddRandomLinkedBin());

            RemoveBinLinking = new Command<BinLinkingDTO>(async (binLinkingDTO) => await RemoveLinkedBin(binLinkingDTO));

            MQTT.MessageReceived += MQTT_MessageReceived;
        }

        #region Receive Messages
        private void MQTT_MessageReceived(object sender, EventArgs e)
        {
            if (e.GetType() != typeof(MqttApplicationMessageReceivedEventArgs))
                return;

            MqttApplicationMessageReceivedEventArgs eventArgs = (MqttApplicationMessageReceivedEventArgs)e;

            FilterMessage(eventArgs.ApplicationMessage);
        }

        private void FilterMessage(MqttApplicationMessage applicationMessage)
        {
            if (applicationMessage.Topic.Contains("/add"))
            {
                AddSynchronizedLinkedBin(applicationMessage);
                return;
            }

            if (applicationMessage.Topic.Contains("/remove"))
            {
                RemoveSynchronizedLinkedBin(applicationMessage);
                return;
            }
        }

        private void RemoveSynchronizedLinkedBin(MqttApplicationMessage applicationMessage)
        {
            BinLinkingDTO binLinkingDTO = CreateBinLinkingDTOFromPayload(applicationMessage.Payload);

            if (binLinkingDTO.ClientID == MQTT.ClientID)
                return;

            BinLinkingDTO binLinkingInCollection = SynchronizedBinLinkingDTOs.First(x => x.ClientID == binLinkingDTO.ClientID && x.KLTBarcode == binLinkingDTO.KLTBarcode && x.ProductBarcode == binLinkingDTO.ProductBarcode);
            if (binLinkingInCollection == null)
                return;

            SynchronizedBinLinkingDTOs.Remove(binLinkingInCollection);
        }

        private void AddSynchronizedLinkedBin(MqttApplicationMessage applicationMessage)
        {
            BinLinkingDTO binLinkingDTO = CreateBinLinkingDTOFromPayload(applicationMessage.Payload);

            if (binLinkingDTO.ClientID == MQTT.ClientID)
                return;

            SynchronizedBinLinkingDTOs.Add(binLinkingDTO);
        }

        private BinLinkingDTO CreateBinLinkingDTOFromPayload(byte[] payload)
        {
            string payloadAsString = Encoding.Default.GetString(payload);
            BinLinkingDTO binLinkingDTO = JsonConvert.DeserializeObject<BinLinkingDTO>(payloadAsString);
            return binLinkingDTO;
        }

        #endregion

        #region Add Linkings

        private async Task AddRandomLinkedBin()
        {
            Random random = new Random();
            string kltBarcode = "KLT " + random.Next(100, 999).ToString();
            string productBarcode = "Product " + random.Next(1000, 9999).ToString();
            await AddLinkedBin(kltBarcode, productBarcode);
        }

        private async Task AddLinkedBin(string kltBarcode, string productBarcode)
        {
            BinLinkingDTO binLinkingDTO = new BinLinkingDTO(MQTT.ClientID, kltBarcode, productBarcode);
            BinLinkingDTOs.Add(binLinkingDTO);
            if (BinLinkingSyncViewModel.SelectedTopicToPublish != null)
                await PublishBinLinkingAddAsync(BinLinkingSyncViewModel.SelectedTopicToPublish.Name, binLinkingDTO);
        }

        private async Task RemoveLinkedBin(BinLinkingDTO binLinkingDTO)
        {
            BinLinkingDTOs.Remove(binLinkingDTO);
            binLinkingDTO.ClientID = MQTT.ClientID;
            if (BinLinkingSyncViewModel.SelectedTopicToPublish != null)
                await PublishBinLinkingRemoveAsync(BinLinkingSyncViewModel.SelectedTopicToPublish.Name, binLinkingDTO);
        }

        private async Task PublishBinLinkingAddAsync(string topic, BinLinkingDTO binLinkingDTO)
        {
            string payload = JsonConvert.SerializeObject(binLinkingDTO);
            await MQTT.PublishMessageToTopic(topic + "/add", payload);
        }

        private async Task PublishBinLinkingRemoveAsync(string topic, BinLinkingDTO binLinkingDTO)
        {
            string payload = JsonConvert.SerializeObject(binLinkingDTO);
            await MQTT.PublishMessageToTopic(topic + "/remove", payload);
        }
        #endregion

        #region Properties
        private ObservableCollection<BinLinkingDTO> _SynchronizedBinLinkingDTOs = new ObservableCollection<BinLinkingDTO>();
        public ObservableCollection<BinLinkingDTO> SynchronizedBinLinkingDTOs
        {
            get
            {
                return _SynchronizedBinLinkingDTOs;
            }

            set
            {
                SetProperty(ref _SynchronizedBinLinkingDTOs, value);
            }
        }

        private ObservableCollection<BinLinkingDTO> _BinLinkingDTOs = new ObservableCollection<BinLinkingDTO>();
        public ObservableCollection<BinLinkingDTO> BinLinkingDTOs
        {
            get
            {
                return _BinLinkingDTOs;
            }

            set
            {
                SetProperty(ref _BinLinkingDTOs, value);
            }
        }

        #endregion
    }
}
