using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MQTTnetXamarinPrototyp.Models
{
    [MessagePackObject]
    class BinLinkingDTO
    {
        public BinLinkingDTO(string clientID, string kltBarcode, string productBarcode)
        {
            ClientID = clientID;
            KLTBarcode = kltBarcode;
            ProductBarcode = productBarcode;
        }
        [Key(0)]
        public string ClientID { get; set; }
        [Key(1)]
        public string KLTBarcode { get; set; }
        [Key(2)]
        public string ProductBarcode { get; set; }
    }
}
