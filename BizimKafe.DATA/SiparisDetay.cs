using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BizimKafe.DATA
{
    public class SiparisDetay
    {
        public string UrunAd { get; set; }

        public decimal BirimFiyat { get; set; }

        public int Adet { get; set; }

        public string TutarTL { get { return $"{Tutar():c2}"; } }


        public decimal Tutar()
        {
            return Adet * BirimFiyat;
        }
    }
}
