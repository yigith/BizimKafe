using BizimKafe.DATA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BizimKafe.UI
{
    public partial class SiparisForm : Form
    {
        public event MasaTasindiEventHandler MasaTasindi;

        private readonly KafeVeri _db;
        private readonly Siparis _siparis;

        public SiparisForm(KafeVeri db, Siparis siparis)
        {
            _db = db;
            _siparis = siparis;
            InitializeComponent();
            BilgileriGuncelle();
            UrunleriListele();
        }

        private void UrunleriListele()
        {
            cboUrun.DataSource = _db.Urunler;
        }

        private void BilgileriGuncelle()
        {
            Text = $"Masa {_siparis.MasaNo}";
            lblMasaNo.Text = _siparis.MasaNo.ToString("00");
            lblOdemeTutari.Text = _siparis.ToplamTutarTL;
            dgvSiparisDetaylar.DataSource = _siparis.SiparisDetaylar.ToList();
            MasanolariYukle();
        }

        private void MasanolariYukle()
        {
            cboMasaNo.Items.Clear();
            for (int i = 1; i <= _db.MasaAdet; i++)
            {
                if (!_db.AktifSiparisler.Any(x => x.MasaNo == i))
                {
                    cboMasaNo.Items.Add(i);
                }
            }
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            Urun urun = (Urun)cboUrun.SelectedItem;

            if (urun == null)
                return;

            SiparisDetay sd = _siparis.SiparisDetaylar
                .FirstOrDefault(x => x.UrunAd == urun.UrunAd);

            if (sd != null)
            {
                sd.Adet += (int)nudAdet.Value;
            }
            else
            {
                sd = new SiparisDetay()
                {
                    UrunAd = urun.UrunAd,
                    BirimFiyat = urun.BirimFiyat,
                    Adet = (int)nudAdet.Value,
                };

                _siparis.SiparisDetaylar.Add(sd);
            }

            BilgileriGuncelle();
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOde_Click(object sender, EventArgs e)
        {
            SiparisiKapat(_siparis.ToplamTutar(), SiparisDurum.Odendi);
        }

        private void btnIptal_Click(object sender, EventArgs e)
        {
            SiparisiKapat(0, SiparisDurum.Iptal);
        }

        private void SiparisiKapat(decimal odenentTutar, SiparisDurum yeniDurum)
        {
            _siparis.KapanisZamani = DateTime.Now;
            _siparis.OdenenTutar = odenentTutar;
            _siparis.Durum = yeniDurum;
            _db.AktifSiparisler.Remove(_siparis);
            _db.GecmisSiparisler.Add(_siparis);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnTasi_Click(object sender, EventArgs e)
        {
            if (cboMasaNo.SelectedIndex < 0) return;

            int eskiMasaNo = _siparis.MasaNo;
            int hedefNo = (int)cboMasaNo.SelectedItem;
            _siparis.MasaNo = hedefNo;
            BilgileriGuncelle();

            if (MasaTasindi != null)
                MasaTasindi(eskiMasaNo, hedefNo);
        }
    }
}
