using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Linq;

namespace CryptoInfoViewer.Models
{
    public class CryptoCurrency
    {



        public string name { get; set; }
        public string symbol { get; set; }

        public int rank { get; set; }
        public decimal supply { get; set; }
        public decimal maxSupply { get; set; }
        public decimal marketCapUsd { get; set; }
        
        public decimal volumeUsd24Hr { get; set; }
        public decimal priceUsd { get; set; }
        public decimal changePercent24Hr { get; set; }
        public decimal vwap24Hr { get; set; }
        public string explorer { get; set; }
    }
}
