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

namespace izinTakip
{
    public partial class KullanıcıEkle : Form
    {
        public KullanıcıEkle()
        {
            InitializeComponent();
            LoadData();
        }
        BaglantiSinifi bgl = new BaglantiSinifi();

        private void LoadData()
        {
            SqlConnection connection = new SqlConnection(bgl.Adres);

            // KULLANICIGIRIS tablosunun verilerini yükle
            string query1 = "SELECT * FROM KULLANİCİGİRİS";
            SqlDataAdapter adapter1 = new SqlDataAdapter(query1, connection); // SQL Server için
                                                                             
            DataTable dt1 = new DataTable();
            adapter1.Fill(dt1);
            dataGridView1.DataSource = dt1;

            // USERGIRIS tablosunun verilerini yükle
            string query2 = "SELECT * FROM USERGIRIS";
            SqlDataAdapter adapter2 = new SqlDataAdapter(query2, connection); // SQL Server için
                                                                            
            DataTable dt2 = new DataTable();
            adapter2.Fill(dt2);
            dataGridView2.DataSource = dt2;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            /* 
             // TextBox'ların boş olup olmadığını kontrol et
             if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
             {
                 MessageBox.Show("Kullanıcı adı ve şifre boş olamaz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 return;
             }

             // SQL bağlantısı
             SqlConnection connection = new SqlConnection(bgl.Adres);
             connection.Open();

             try
                 {
                   //  connection.Open();
                     string query = "";

                     // Yetkili mi kullanıcı mı seçildiğini kontrol et
                     if (checkBox1.Checked && !checkBox2.Checked)
                     {
                         // Yetkili seçiliyse KULLANİCİGİRİS tablosuna ekle
                         query = "INSERT INTO KULLANİCİGİRİS (kullanici_adi, sifre) VALUES (@kullaniciAdi, @sifre)";
                     }
                     else if (!checkBox1.Checked && checkBox2.Checked)
                     {
                         // Kullanıcı seçiliyse USERGIRIS tablosuna ekle
                         query = "INSERT INTO USERGIRIS (KullaniciAdi, Sifre) VALUES (@kullaniciAdi, @sifre)";
                     }
                     else
                     {
                         // Hiçbir seçim yapılmadıysa uyarı göster
                         MessageBox.Show("Lütfen yetkili veya kullanıcı seçimini yapın.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                         return;
                     }

                     // SQL komutunu hazırla
                     using (SqlCommand cmd = new SqlCommand(query, connection))
                     {
                         cmd.Parameters.AddWithValue("@kullaniciAdi", textBox1.Text);
                         cmd.Parameters.AddWithValue("@sifre", textBox2.Text);

                         // Komutu çalıştır
                         cmd.ExecuteNonQuery();

                         // Başarı mesajı
                         MessageBox.Show("Kayıt başarıyla gerçekleştirildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                         textBox1.Clear();
                         textBox2.Clear();
                     }

                 }
                 catch (Exception ex)
                 {
                     // Hata durumunda hata mesajı göster
                     MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                 } */
            
                // TextBox'ların boş olup olmadığını kontrol et
                if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
                {
                    MessageBox.Show("Kullanıcı adı ve şifre boş olamaz!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // SQL bağlantısı
                SqlConnection connection = new SqlConnection(bgl.Adres);
                connection.Open();

                try
                {
                    string query = "";
                    string checkQuery = "";
                    string tableName = "";

                    // Yetkili mi kullanıcı mı seçildiğini kontrol et
                    if (checkBox1.Checked && !checkBox2.Checked)
                    {
                        // Yetkili seçiliyse KULLANİCİGİRİS tablosuna ekle
                        tableName = "KULLANİCİGİRİS";
                        query = $"INSERT INTO {tableName} (kullanici_adi, sifre) VALUES (@kullaniciAdi, @sifre)";
                        checkQuery = $"SELECT COUNT(*) FROM {tableName} WHERE kullanici_adi = @kullaniciAdi";
                    }
                    else if (!checkBox1.Checked && checkBox2.Checked)
                    {
                        // Kullanıcı seçiliyse USERGIRIS tablosuna ekle
                        tableName = "USERGIRIS";
                        query = $"INSERT INTO {tableName} (KullaniciAdi, Sifre) VALUES (@kullaniciAdi, @sifre)";
                        checkQuery = $"SELECT COUNT(*) FROM {tableName} WHERE KullaniciAdi = @kullaniciAdi";
                    }
                    else
                    {
                        // Hiçbir seçim yapılmadıysa uyarı göster
                        MessageBox.Show("Lütfen yetkili veya kullanıcı seçimini yapın.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Kullanıcı adı daha önce eklenmiş mi kontrol et
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@kullaniciAdi", textBox1.Text);
                        int userCount = (int)checkCmd.ExecuteScalar();

                        if (userCount > 0)
                        {
                            MessageBox.Show("Bu kullanıcı adı zaten mevcut. Yeni kayıt oluşturulamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    // SQL komutunu hazırla ve yeni kayıt ekle
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@kullaniciAdi", textBox1.Text);
                        cmd.Parameters.AddWithValue("@sifre", textBox2.Text);

                        // Komutu çalıştır
                        cmd.ExecuteNonQuery();

                        // Başarı mesajı
                        MessageBox.Show("Kayıt başarıyla gerçekleştirildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBox1.Clear();
                        textBox2.Clear();
                    }

                }
                catch (Exception ex)
                {
                    // Hata durumunda hata mesajı göster
                    MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connection.Close();
                }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Eğer Yetkili checkbox işaretlendiyse Kullanıcı checkbox'ını devre dışı bırak
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            // Eğer Yetkili checkbox işaretlendiyse Kullanıcı checkbox'ını devre dışı bırak
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
            }
        }
      
        private void label3_MouseClick(object sender, MouseEventArgs e)
        {
            DialogResult res;
            res = MessageBox.Show("Çıkış yapmak istediğinizden emin misiniz?", "ÇIKIŞ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                Anasayfa frm = new Anasayfa();
                frm.Show();
                this.Hide();
            }
            else
            {
                this.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Yetkili DataGridView'dan seçili satırın silinmesi
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Seçili yetkili kaydını silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["id"].Value); // "ID" sütun adını kendi tablonuza göre düzenleyin
                    Sil("KULLANİCİGİRİS", id);
                    TabloyuGuncelleYetkili();
                }
            }
            // Kullanıcı DataGridView'dan seçili satırın silinmesi
            else if (dataGridView2.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Seçili kullanıcı kaydını silmek istediğinize emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(dataGridView2.SelectedRows[0].Cells["id"].Value); // "ID" sütun adını kendi tablonuza göre düzenleyin
                    Sil("USERGIRIS", id);
                    TabloyuGuncelleKullanici();
                }
            }
            else
            {
                MessageBox.Show("Silinecek bir satır seçilmedi.");
            }
        }
        private void Sil(string tableName, int id)
        {
            // Veritabanı bağlantı dizesi (güncel veritabanı bilgilerinizi kullanın)
            string connectionString = bgl.Adres; // Bağlantı dizesi
            string query = $"DELETE FROM {tableName} WHERE id = @id"; // ID'yi kendi tablonuza göre değiştirin

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        private void TabloyuGuncelleYetkili()
        {
            // Yetkili DataGridView'ı güncelle
            string connectionString = bgl.Adres;
            string query = "SELECT * FROM KULLANİCİGİRİS"; // Tablo adını kendi tablonuza göre değiştirin

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView1.DataSource = dataTable;
                }
            }
        }

        private void TabloyuGuncelleKullanici()
        {
            // Kullanıcı DataGridView'ı güncelle
            string connectionString = bgl.Adres;
            string query = "SELECT * FROM USERGIRIS"; // Tablo adını kendi tablonuza göre değiştirin

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dataGridView2.DataSource = dataTable;
                }
            }
        }
    }
}
