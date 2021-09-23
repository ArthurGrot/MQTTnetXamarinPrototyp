using MQTTnetXamarinPrototyp.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace MQTTnetXamarinPrototyp.Models
{
    class StorageLocationSynchronization : ModelBase
    {
        public static MQTTClient MQTT => DependencyService.Get<MQTTClient>();


        public StorageLocationSynchronization(string name)
        {
            Name = name;
            isChecked = false;

        }

        public string Name { get; }
        private bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                bool changed = SetProperty(ref isChecked, value);

                if (!changed)
                    return;

                if (value)
                {
                    MQTT.SubscribeToTopicAsync(Name + "/add");
                    MQTT.SubscribeToTopicAsync(Name + "/remove");
                }
                else
                {
                    MQTT.UnsubcribeFromTopicAsync(Name + "/add");
                    MQTT.UnsubcribeFromTopicAsync(Name + "/remove");
                }
            }
        }

        public StorageLocationSynchronization GetStorage => this;
    }
}
