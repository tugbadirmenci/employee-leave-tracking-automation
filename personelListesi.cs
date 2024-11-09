using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Sql;
using Xceed.Words.NET;
using Excel = Microsoft.Office.Interop.Excel;

namespace izinTakip
{
    public partial class personelListesi : Form
    {
        public personelListesi()
        {
            InitializeComponent();
        }
        BaglantiSinifi bgl = new BaglantiSinifi();
        private void personelListesi_Load(object sender, EventArgs e)
        {
            /* listView1.View = View.Details;
             listView1.GridLines = true;
             listView1.Columns.Add("ıd", 50);
             listView1.Columns.Add("AD SOYAD", 100);
             listView1.Columns.Add("SUBE", 120);
             listView1.Columns.Add("TC NO", 100);
             listView1.Columns.Add("BOLUM", 100);           
             listView1.Columns.Add("KIDEM YILI", 100);
             listView1.Columns.Add("TOPLAM IZIN", 100);
             listView1.Columns.Add("IZIN HAKKI", 100);
             listView1.Columns.Add("YAS", 100);
             listView1.Columns.Add("DOGUM TARIHI", 120);
             listView1.Columns.Add("ISE GIRIS TARIHI", 120);
             listView1.Columns.Add("HAKEDİS TARIHI", 120);

             SqlConnection connection = new SqlConnection(bgl.Adres);
             connection.Open();
             SqlCommand cmd = new SqlCommand("Select*From PERSONEL", connection);
             SqlDataReader dr;
             dr = cmd.ExecuteReader();
             while (dr.Read())
             {
                 var item1 = listView1.Items.Add(dr[0].ToString());
                 item1.SubItems.Add(dr[1].ToString());
                 item1.SubItems.Add(dr[2].ToString());
                 item1.SubItems.Add(dr[3].ToString());
                 item1.SubItems.Add(dr[4].ToString());
                 item1.SubItems.Add(dr[5].ToString());
                 item1.SubItems.Add(dr[6].ToString());
                 item1.SubItems.Add(dr[7].ToString());
                 item1.SubItems.Add(dr[8].ToString());            
                 // Doğum tarihi
                 var DOGUM_TARIHI = DateTime.Parse(dr[9].ToString());
                 item1.SubItems.Add(DOGUM_TARIHI.ToString("dd/MM/yyyy"));

                 // İşe giriş tarihi
                 var ISE_GIRIS_TARIHI = DateTime.Parse(dr[10].ToString());
                 item1.SubItems.Add(ISE_GIRIS_TARIHI.ToString("dd/MM/yyyy"));

                 // Hakediş tarihi
                 var HAKEDIS_TARIHI = DateTime.Parse(dr[11].ToString());
                 item1.SubItems.Add(HAKEDIS_TARIHI.ToString("dd/MM/yyyy"));

             }
             connection.Close(); */


            kayitlarigetir();
        }

        public void kayitlarigetir()
        {
            SqlConnection connection = new SqlConnection(bgl.Adres);
            connection.Open();

            // Önce personel verilerini al
            SqlDataAdapter ad = new SqlDataAdapter("SELECT * FROM PERSONEL ORDER BY id", bgl.Adres);
            DataTable dt = new DataTable();
            ad.Fill(dt);

            foreach (DataRow row in dt.Rows)
            {
                // Verileri DataRow'dan al
                DateTime DOGUM_TARIHI = DateTime.Parse(row["DOGUM_TARIHI"].ToString());
                DateTime ISE_GIRIS_TARIHI = DateTime.Parse(row["ISE_GIRIS_TARIHI"].ToString());

                DateTime bugun = DateTime.Now;

                // Kıdem yılı hesaplama
                int yilFarki = bugun.Year - ISE_GIRIS_TARIHI.Year;
                int ayFarki = bugun.Month - ISE_GIRIS_TARIHI.Month;
                int gunFarki = bugun.Day - ISE_GIRIS_TARIHI.Day;

                if (gunFarki < 0)
                {
                    ayFarki--;
                    gunFarki += DateTime.DaysInMonth(bugun.Year, bugun.Month - 1);
                }

                if (ayFarki < 0)
                {
                    yilFarki--;
                    ayFarki += 12;
                }

                string KIDEM_YILI = $"{yilFarki} yıl, {ayFarki} ay, {gunFarki} gün";

                // Yaş hesaplama
                var YAS = bugun.Year - DOGUM_TARIHI.Year;
                if (bugun < DOGUM_TARIHI.AddYears(YAS))
                {
                    YAS--;
                }

                // İzin hakkı ve toplam izin hesaplama
                int IZIN_HAKKI;
                int kidemYili = yilFarki;
                int TOPLAM_IZIN = 0;

                if (YAS > 50)
                {
                    if (kidemYili < 1)
                    {
                        IZIN_HAKKI = 0;
                    }
                    else if (kidemYili <= 15)
                    {
                        IZIN_HAKKI = 21;
                        TOPLAM_IZIN = kidemYili * IZIN_HAKKI;
                    }
                    else
                    {
                        IZIN_HAKKI = 26;
                        TOPLAM_IZIN = kidemYili * IZIN_HAKKI;
                    }
                }
                else
                {
                    if (kidemYili < 1)
                    {
                        IZIN_HAKKI = 0;
                    }
                    else if (kidemYili <= 5)
                    {
                        IZIN_HAKKI = 14;
                        TOPLAM_IZIN = kidemYili * IZIN_HAKKI;
                    }
                    else if (kidemYili <= 15)
                    {
                        IZIN_HAKKI = 21;
                        TOPLAM_IZIN = (5 * 14) + ((kidemYili - 5) * 21);
                    }
                    else
                    {
                        IZIN_HAKKI = 26;
                        TOPLAM_IZIN = (5 * 14) + (10 * 21) + ((kidemYili - 15) * 26);
                    }
                }

                DateTime HAKEDIS_TARIHI = ISE_GIRIS_TARIHI.AddYears(1);

                // Veritabanını güncelle
                SqlCommand komut = new SqlCommand("UPDATE PERSONEL SET KIDEM_YILI=@kidem_yili, TOPLAM_IZIN=@toplam_izin, IZIN_HAKKI=@izin_hakki, YAS=@yas, HAKEDİS_TARIHI=@hakedis_tarihi WHERE id=@id", connection);

                komut.Parameters.AddWithValue("@id", row["id"]);
                komut.Parameters.AddWithValue("@kidem_yili", KIDEM_YILI);
                komut.Parameters.AddWithValue("@toplam_izin", TOPLAM_IZIN);
                komut.Parameters.AddWithValue("@izin_hakki", IZIN_HAKKI);
                komut.Parameters.AddWithValue("@yas", YAS);
                komut.Parameters.AddWithValue("@hakedis_tarihi", HAKEDIS_TARIHI);

                komut.ExecuteNonQuery();
            }

            // Güncellenmiş verileri tekrar çek ve DataGridView'e yükle
            ad = new SqlDataAdapter("SELECT * FROM PERSONEL ORDER BY id", bgl.Adres);
            dt = new DataTable();
            ad.Fill(dt);
            dataGridView1.DataSource = dt;

            connection.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Anasayfa frm10 = new Anasayfa();
            frm10.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult res;
            res = MessageBox.Show("Çıkış yapmak istediğinizden emin misiniz?", "ÇIKIŞ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                Application.Exit();
            }
            else
            {
                this.Show();
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel Files|*.xlsx";
                saveFileDialog.Title = "Save an Excel File";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Excel uygulamasını başlat
                    Excel.Application excelApp = new Excel.Application();
                    Excel.Workbook workbook = excelApp.Workbooks.Add();
                    Excel.Worksheet worksheet = workbook.Sheets[1];

                    // data başlıklarını yaz
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        worksheet.Cells[1, i + 1] = dataGridView1.Columns[i].HeaderText;
                    }

                    // data verilerini yaz
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGridView1.Columns.Count; j++)
                        {
                            worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value?.ToString();
                        }
                    }

                    // Dosyayı kaydet
                    workbook.SaveAs(saveFileDialog.FileName);
                    workbook.Close();
                    excelApp.Quit();

                    // Bellek temizliği
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(worksheet);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                    MessageBox.Show("Veriler Excel dosyasına başarıyla kaydedildi.");
                }

            }
        }
    }
}


