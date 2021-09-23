using System;
using System.Collections.Generic;
using System.Text;

namespace MQTTnetXamarinPrototyp.Services
{
    public enum MQTTClientConnectionState
    {
        Connected,
        Connecting,
        Disconnected,
        Disconnecting
    }
}
