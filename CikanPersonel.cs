using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using Xceed.Words.NET;
using Excel = Microsoft.Office.Interop.Excel;

namespace izinTakip
{
    public partial class CikanPersonel : Form
    {
        public CikanPersonel()
        {
            InitializeComponent();
        }
        BaglantiSinifi bgl = new BaglantiSinifi();
        private void button1_Click(object sender, EventArgs e)
        {
            Anasayfa frm12 = new Anasayfa();
            frm12.Show();
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

        private void CikanPersonel_Load(object sender, EventArgs e)
        {
            /*  listView1.View = View.Details;
              listView1.GridLines = true;
              listView1.Columns.Add("ıd", 50);
              listView1.Columns.Add("AD SOYAD", 100);
              listView1.Columns.Add("SUBE", 120);
              listView1.Columns.Add("TC", 100);
              listView1.Columns.Add("DOGUM TARIHI", 100);         
              listView1.Columns.Add("ISE GIRIS TARIHI", 100);           
              listView1.Columns.Add("BOLUM", 120);
              listView1.Columns.Add("MEVCUT IZIN HAKKI", 120);
              listView1.Columns.Add("IS CIKIS TARIHI", 120);
              listView1.Columns.Add("ACIKLAMA", 140);

              //  SqlConnection con = new SqlConnection("Data Source=LAPTOP-JK4K9LES\\SQLEXPRESS;Initial Catalog = veritabani_inot; Integrated Security = True");
              SqlConnection connection = new SqlConnection(bgl.Adres);

              connection.Open();
              SqlCommand cmd = new SqlCommand("Select*From CIKAN_PERSONEL", connection);
              SqlDataReader dr;
              dr = cmd.ExecuteReader();
              while (dr.Read())
              {
                  var item1 = listView1.Items.Add(dr[0].ToString());
                  item1.SubItems.Add(dr[1].ToString());
                  item1.SubItems.Add(dr[2].ToString());
                  item1.SubItems.Add(dr[3].ToString());
                  // Doğum tarihi
                  var dogumTarihi = DateTime.Parse(dr[4].ToString());
                  item1.SubItems.Add(dogumTarihi.ToString("dd/MM/yyyy"));
                  // İşe giriş tarihi
                  var iseGirisTarihi = DateTime.Parse(dr[5].ToString());
                  item1.SubItems.Add(iseGirisTarihi.ToString("dd/MM/yyyy"));

                  item1.SubItems.Add(dr[6].ToString());
                  item1.SubItems.Add(dr[7].ToString());

                  // is cıkıs tarihi
                  var isCıkısTarihi = DateTime.Parse(dr[8].ToString());
                  item1.SubItems.Add(isCıkısTarihi.ToString("dd/MM/yyyy"));
                  item1.SubItems.Add(dr[9].ToString());


                  /* var datef = DateTime.Parse(dr[10].ToString());
                   item1.SubItems.Add(datef.ToString("MM/dd/yyyy"));
                   item1.SubItems.Add(datef.ToString("MM/dd/yyyy"));
                   item1.SubItems.Add(datef.ToString("MM/dd/yyyy")); 
              }
              connection.Close(); */
            cikanPersonelleriGetir();
        }
        private void cikanPersonelleriGetir()
        {
            using (SqlConnection connection = new SqlConnection(bgl.Adres))
            {
                connection.Open();
                SqlDataAdapter ad = new SqlDataAdapter("SELECT AD_SOYAD, SUBE, TC_NO, DOGUM_TARIHI, ISE_GIRIS_TARIHI, BOLUM, MEVCUT_IZIN_HAKKI, IS_CIKIS_TARIHI, ACIKLAMA FROM CIKAN_PERSONEL ORDER BY IS_CIKIS_TARIHI DESC", connection);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                dataGridView1.DataSource = dt;
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
