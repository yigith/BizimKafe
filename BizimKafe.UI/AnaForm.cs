using BizimKafe.DATA;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BizimKafe.UI
{
    public partial class AnaForm : Form
    {
        KafeVeri db = new KafeVeri();

        public AnaForm()
        {
            InitializeComponent();
            VerileriYukle();
            MasalariOlustur();
        }

        private void VerileriYukle()
        {
            try
            {
                string json = File.ReadAllText("veri.json");
                db = JsonSerializer.Deserialize<KafeVeri>(json);
            }
            catch (Exception)
            {
                OrnekVerileriYukle();
            }
        }

        private void OrnekVerileriYukle()
        {
            db.Urunler.Add(new Urun() { UrunAd = "Kola", BirimFiyat = 7.00m });
            db.Urunler.Add(new Urun() { UrunAd = "Ayran", BirimFiyat = 6.50m });
        }

        private void MasalariOlustur()
        {
            for (int i = 1; i <= db.MasaAdet; i++)
            {
                var lvi = new ListViewItem();
                lvi.Text = $"Masa {i}";
                lvi.ImageKey = db.AktifSiparisler.Any(x => x.MasaNo == i) ? "dolu" : "bos";
                lvi.Tag = i;
                lvwMasalar.Items.Add(lvi);
            }
        }

        private void lvwMasalar_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem lvi = lvwMasalar.SelectedItems[0]; 
            int masaNo = (int)lvi.Tag;

            Siparis siparis = db.AktifSiparisler.FirstOrDefault(x => x.MasaNo == masaNo);

            if (siparis == null)
            {
                siparis = new Siparis() { MasaNo = masaNo };
                db.AktifSiparisler.Add(siparis);
                lvi.ImageKey = "dolu";
            }

            var frmSiparis = new SiparisForm(db, siparis);
            frmSiparis.MasaTasindi += FrmSiparis_MasaTasindi;
            DialogResult sonuc = frmSiparis.ShowDialog();

            if (sonuc == DialogResult.OK)
            {
                lvi.ImageKey = "bos";
            }
        }

        private void FrmSiparis_MasaTasindi(int eskiMasaNo, int yeniMasaNo)
        {
            foreach (ListViewItem item in lvwMasalar.Items)
            {
                int masaNo = (int)item.Tag;

                if (masaNo == eskiMasaNo)
                    item.ImageKey = "bos";
                else if (masaNo == yeniMasaNo)
                    item.ImageKey = "dolu";
            }
        }

        private void tsmiGecmisSiparisler_Click(object sender, EventArgs e)
        {
            new GecmisSiparislerForm(db).ShowDialog();
        }

        private void tsmiUrunler_Click(object sender, EventArgs e)
        {
            new UrunlerForm(db).ShowDialog();
        }

        private void AnaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string json = JsonSerializer.Serialize(db);
            File.WriteAllText("veri.json", json);
        }
    }
}
