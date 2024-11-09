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
using Xceed.Words.NET;
using Excel = Microsoft.Office.Interop.Excel;

namespace izinTakip
{
    public partial class İzinOzetTablosu : Form
    {
       // private string connectionString = "Server=LAPTOP-JK4K9LES\\SQLEXPRESS;Database=veritabani_inot;Integrated Security=True;";
        public İzinOzetTablosu()
        {
            InitializeComponent();
            LoadData();         
        }
        BaglantiSinifi bgl = new BaglantiSinifi();

        private void LoadData()
        {
            // Veritabanı bağlantısı oluştur
            using (SqlConnection connection = new SqlConnection(bgl.Adres))
            {
                connection.Open();

                // SQL sorgusu
                string query = "SELECT id, AD_SOYAD, SUBE, BOLUM, ISE_GIRIS_TARIHI, KIDEM_YILI, TOPLAM_IZIN FROM PERSONEL";

                // Verileri almak için SqlDataAdapter oluştur
                SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);

                // Verileri tutmak için DataTable oluştur
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                // DataGridView'ı temizle ve güncel verilerle doldur
                dataGridView1.DataSource = dataTable;
                // İzin özet tablosuna verileri ekle
                // İzin özet tablosuna verileri eklemek için
                foreach (DataRow row in dataTable.Rows)
                {
                    // SQL sorgusu: İzin özet tablosuna veri ekle
                    string insertQuery = @"
                INSERT INTO IZIN_OZET (AD_SOYAD, SUBE, BOLUM, ISE_GIRIS_TARIHI, KIDEM_YILI, TOPLAM_IZIN) 
                VALUES (@AD_SOYAD, @SUBE, @BOLUM, @ISE_GIRIS_TARIHI, @KIDEM_YILI, @TOPLAM_IZIN)";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        // Parametreleri ekle
                        command.Parameters.AddWithValue("@AD_SOYAD", row["AD_SOYAD"]);
                        command.Parameters.AddWithValue("@SUBE", row["SUBE"]);
                        command.Parameters.AddWithValue("@BOLUM", row["BOLUM"]);
                        command.Parameters.AddWithValue("@ISE_GIRIS_TARIHI", row["ISE_GIRIS_TARIHI"]);
                        command.Parameters.AddWithValue("@KIDEM_YILI", row["KIDEM_YILI"]);
                        command.Parameters.AddWithValue("@TOPLAM_IZIN", row["TOPLAM_IZIN"]);

                        // Komutu çalıştır
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Anasayfa frm13 = new Anasayfa();
            frm13.Show();
            this.Hide();
        }
       
        private void İzinOzetTablosu_Load(object sender, EventArgs e)
        {
           
            /*  listView1.View = View.Details;
              listView1.GridLines = true;
              listView1.Columns.Add("ıd", 50);
              listView1.Columns.Add("AD SOYAD", 140);
              listView1.Columns.Add("SUBE", 100);
              listView1.Columns.Add("BOLUM", 100);
              listView1.Columns.Add("ISE GIRIS TARIHI", 100);          
              listView1.Columns.Add("KIDEM YILI", 140);
              listView1.Columns.Add("KALAN IZIN", 120);

              SqlConnection connection = new SqlConnection(bgl.Adres);
              connection.Open();

              SqlCommand cmd = new SqlCommand("Select*From IZIN_OZET", connection);
              SqlDataReader dr;
              dr = cmd.ExecuteReader();

              while (dr.Read())
              {
                  var item1 = listView1.Items.Add(dr[0].ToString());
                  item1.SubItems.Add(dr[1].ToString());
                  item1.SubItems.Add(dr[2].ToString());
                  item1.SubItems.Add(dr[3].ToString());
                  // is giris tarihi
                  var isbasıtarihi = DateTime.Parse(dr[4].ToString());
                  item1.SubItems.Add(isbasıtarihi.ToString("dd/MM/yyyy"));

                  item1.SubItems.Add(dr[5].ToString());
                  item1.SubItems.Add(dr[6].ToString());

              }
              connection.Close(); */
            
            //izinOzetleriGetir();
        }
       

        private string KidemYiliHesapla(DateTime iseGirisTarihi)
        {
            DateTime bugun = DateTime.Now;

            int yilFarki = bugun.Year - iseGirisTarihi.Year;
            int ayFarki = bugun.Month - iseGirisTarihi.Month;
            int gunFarki = bugun.Day - iseGirisTarihi.Day;

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

            return $"{yilFarki} yıl, {ayFarki} ay, {gunFarki} gün";
        }
    
          private void button2_Click(object sender, EventArgs e)
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

                    // DataGridView verilerini yaz
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
