using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnetXamarinPrototyp.Models;
using MQTTnetXamarinPrototyp.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace MQTTnetXamarinPrototyp.ViewModels
{
    class BinLinkingSyncViewModel : BaseViewModel
    {

        public static MQTTClient MQTT => DependencyService.Get<MQTTClient>();

        public ICommand ConnectBrokerCommand { get; }
        public ICommand DisconnectBrokerCommand { get; }
        public ICommand SelectTopicToPublish { get; }

        public BinLinkingSyncViewModel()
        {
            Title = "BinLinking";

            ConnectBrokerCommand = new Command(async () => await ConnectBrokerAsync());
            DisconnectBrokerCommand = new Command(async () => await DisconnectBrokerAsync());
            SelectTopicToPublish = new Command<StorageLocationSynchronization>(SelectTopic);

            MQTT.MqttClientConnectionsResultChanged += MQTT_MqttClientConnectionsResultChanged;
        }

        private void MQTT_MqttClientConnectionsResultChanged(object sender, EventArgs e)
        {
            ConnectionStatus = MQTT.MQTTClientConnectionState.ToString();
        }

        private async Task ConnectBrokerAsync()
        {
            await MQTT.ConnectAsync();
        }

        private async Task DisconnectBrokerAsync()
        {
            IsCheckedAll = false;
            UnsubscribeFromAllLocations();
            await MQTT.DisconnectAsync();
        }

        private void SubscribeToAllLocations()
        {
            foreach (StorageLocationSynchronization storageLocationSynchronization in StorageLocations)
            {
                if (storageLocationSynchronization.IsChecked)
                    continue;

                storageLocationSynchronization.IsChecked = true;
            }
        }

        private void UnsubscribeFromAllLocations()
        {
            foreach (StorageLocationSynchronization storageLocationSynchronization in StorageLocations)
            {
                if (!storageLocationSynchronization.IsChecked)
                    continue;

                storageLocationSynchronization.IsChecked = false;
            }
        }

        private void SelectTopic(StorageLocationSynchronization obj)
        {
            SelectedTopicToPublish = obj;
        }

        #region Properties

        private string _ConnectionStatus;
        public string ConnectionStatus
        {
            get { return _ConnectionStatus; }

            set
            {
                SetProperty(ref _ConnectionStatus, value);
            }
        }

        private bool _IsCheckedAll;
        public bool IsCheckedAll
        {
            get
            {
                return _IsCheckedAll;
            }
            set
            {
                bool changed = SetProperty(ref _IsCheckedAll, value);

                if (!changed)
                    return;

                if (value)
                {
                    SubscribeToAllLocations();
                }
                else
                {
                    UnsubscribeFromAllLocations();
                }
            }
        }

        private ObservableCollection<StorageLocationSynchronization> _StorageLocations = new ObservableCollection<StorageLocationSynchronization>()
        {
            new StorageLocationSynchronization("WIS/SystembetreuerApp/Behälterverheiratung/Kunde1/Lagerort1"),
            new StorageLocationSynchronization("WIS/SystembetreuerApp/Behälterverheiratung/Kunde1/Lagerort2"),
            new StorageLocationSynchronization("WIS/SystembetreuerApp/Behälterverheiratung/Kunde1/Lagerort3")
        };
        public ObservableCollection<StorageLocationSynchronization> StorageLocations
        {
            get
            {
                return _StorageLocations;
            }
            set
            {
                SetProperty(ref _StorageLocations, value);
            }
        }


        private static StorageLocationSynchronization _SelectedTopicToPublish;
        public static StorageLocationSynchronization SelectedTopicToPublish
        {
            get
            {
                return _SelectedTopicToPublish;
            }
            set
            {
                if (value == null)
                    return;

                if (_SelectedTopicToPublish == value)
                    return;

                _SelectedTopicToPublish = value;
            }

        }

        #endregion
    }
}
