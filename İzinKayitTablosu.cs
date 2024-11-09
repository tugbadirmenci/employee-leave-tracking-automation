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
    public partial class İzinKayitTablosu : Form
    {
        public İzinKayitTablosu()
        {
            InitializeComponent();
        }
        BaglantiSinifi bgl = new BaglantiSinifi();


        private void button1_Click(object sender, EventArgs e)
        {
            Anasayfa frm15 = new Anasayfa();
            frm15.Show();
            this.Hide();
        }
        public void kayitlarigetir()
        {
            SqlConnection connection = new SqlConnection(bgl.Adres);
            connection.Open();

            string getir = "Select * From IZIN_KAYIT Order By id";
            SqlCommand komut = new SqlCommand(getir, connection);
            SqlDataAdapter ad = new SqlDataAdapter(komut);
            DataTable dt = new DataTable();
            ad.Fill(dt);
            dataGridView1.DataSource = dt;
            connection.Close();

        }
        private void İzinKayitTablosu_Load(object sender, EventArgs e)
        {
            kayitlarigetir();
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

                    // DataGridView başlıklarını yaz
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

        private void button7_Click(object sender, EventArgs e)
        {

            // DataGridView'den seçili satırdaki iznin bilgilerini alıyoruz.
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string TC_NO = dataGridView1.SelectedRows[0].Cells["TC_NO"].Value.ToString();
                int kullanilanIzin = int.Parse(dataGridView1.SelectedRows[0].Cells["KULLANILAN_IZIN"].Value.ToString());
                DateTime izinCikisTarihi = DateTime.Parse(dataGridView1.SelectedRows[0].Cells["IZIN_CIKIS_TARIHI"].Value.ToString());

                // Kullanıcıya emin olup olmadığını sormak için onay kutusu göster
                DialogResult result = MessageBox.Show("Bu izni silmek istediğinize emin misiniz?", "İzin Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    SqlConnection connection = new SqlConnection(bgl.Adres);
                   

                    // string connectionString = "Server=LAPTOP-JK4K9LES\\SQLEXPRESS;Database=veritabani_inot;Integrated Security=True;";

                    // using (SqlConnection connection = new SqlConnection(connectionString))
                    // {
                    connection.Open();

                        // Seçili izin kaydını silmek için SQL sorgusu
                        string deleteQuery = "DELETE FROM IZIN_KAYIT WHERE TC_NO = @tcno AND IZIN_CIKIS_TARIHI = @izinCikisTarihi";

                        using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@tcno", TC_NO);
                            deleteCommand.Parameters.AddWithValue("@izinCikisTarihi", izinCikisTarihi);
                            deleteCommand.ExecuteNonQuery();
                        }

                        // Silinen iznin gününü personelin toplam iznine eklemek için SQL sorgusu
                        string updateQuery = "UPDATE PERSONEL SET TOPLAM_IZIN = TOPLAM_IZIN + @kullanilanIzin WHERE TC_NO = @tcno";

                        using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@kullanilanIzin", kullanilanIzin);
                            updateCommand.Parameters.AddWithValue("@tcno", TC_NO);
                            updateCommand.ExecuteNonQuery();
                        }

                        MessageBox.Show("İzin başarıyla silindi ve personelin toplam izni güncellendi.");

                        // DataGridView'i güncelle
                        string selectQuery = "SELECT * FROM IZIN_KAYIT"; // Burada güncellenmiş verileri getiren sorgu
                        SqlDataAdapter dataAdapter = new SqlDataAdapter(selectQuery, connection);
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);
                        dataGridView1.DataSource = dataTable;
                    }
                }
        
            else
            {
                MessageBox.Show("Lütfen silmek istediğiniz izni seçiniz.");
            }

        }
    }
    }

