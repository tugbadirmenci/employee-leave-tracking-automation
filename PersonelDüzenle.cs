using Excel = Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static izinTakip.personelEkle;

namespace izinTakip
{
    public partial class PersonelDüzenle : Form
    {
        public PersonelDüzenle()
        {
            InitializeComponent();
        }
        BaglantiSinifi bgl = new BaglantiSinifi();

       // SqlConnection connection = new SqlConnection("Data Source=LAPTOP-JK4K9LES\\SQLEXPRESS;Initial Catalog = veritabani_inot; Integrated Security = True");

        private void PersonelDüzenle_Load(object sender, EventArgs e)
        {
            // Statik değişkenden tema tercihlerini al ve uygula
            tbDarkTheme.Checked = ThemeSettings.DarkTheme;
            ApplyTheme();

            SqlConnection connection = new SqlConnection(bgl.Adres);
            connection.Open();

            SqlCommand com = new SqlCommand("SELECT * FROM BOLUMLER", connection);
            SqlDataReader dr;

            dr = com.ExecuteReader();

            while (dr.Read())
            {
                comboBolum.Items.Add(dr["BOLUMLER"]);
            }

            connection.Close();


            // DateTimePicker başlangıçta boş gibi görünsün.
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = " ";  // Boş göster.

            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = " ";  // Boş göster.

            // Kullanıcının rolü 'user' ise belirli TextBox'ları devre dışı bırak
            if (CurrentUser.Role == "User")
            {
                
                button3.Enabled = false;
               
            }
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

        private void button2_Click(object sender, EventArgs e)
        {
            Anasayfa frm8 = new Anasayfa();
            frm8.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            kayitlarigetir();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Eğer TextBox boşsa işlemi gerçekleştirme
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Lütfen bir isim girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi sonlandır
            }
            SqlConnection connection = new SqlConnection(bgl.Adres);
            connection.Open();
            // Girilen ismin ilk üç harfini alır ve wildcard ile birlikte kullanır
            string kayit = "SELECT * FROM PERSONEL WHERE AD_SOYAD LIKE @ad";
            SqlCommand command = new SqlCommand(kayit, connection);
            command.Parameters.AddWithValue("@ad", textBox5.Text + "%");

            SqlDataAdapter da = new SqlDataAdapter(command);

            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            connection.Close();

        }
        int i = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(bgl.Adres);
            connection.Open();
            // Form üzerindeki tüm kontrolleri kontrol et
            bool allFieldsValid = true;

            // Boş alanları kontrol et
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||   // textBox1 boş mu?
                string.IsNullOrWhiteSpace(textBox2.Text) ||   // textBox2 boş mu?
                string.IsNullOrWhiteSpace(textBox3.Text) ||   // textBox3 boş mu?
                 dateTimePicker1.Text == " " || dateTimePicker2.Text == " ")
            {
                allFieldsValid = false;
            }

            // Eğer alanlar boş ise hata mesajı göster
            if (!allFieldsValid)
            {
                MessageBox.Show("Lütfen tüm gerekli alanları doldurunuz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else {

               // connection.Open();
                string AD_SOYAD = textBox3.Text;
                string SUBE = textBox1.Text;
                string TC_NO = textBox2.Text;
                DateTime DOGUM_TARIHI = DateTime.Parse(dateTimePicker1.Text);
                string BOLUM = comboBolum.Text;
                DateTime ISE_GIRIS_TARIHI = DateTime.Parse(dateTimePicker2.Text);

                DateTime bugun = DateTime.Now;

                // Yıl, Ay, Gün farkını hesaplamak için
                int yilFarki = bugun.Year - ISE_GIRIS_TARIHI.Year;
                int ayFarki = bugun.Month - ISE_GIRIS_TARIHI.Month;
                int gunFarki = bugun.Day - ISE_GIRIS_TARIHI.Day;

                // Eğer ay veya gün farkı negatifse, düzeltmeler yapalım
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


                var kidemYili = bugun.Year - ISE_GIRIS_TARIHI.Year;
                if (bugun < ISE_GIRIS_TARIHI.AddYears(kidemYili))
                {
                    kidemYili--;
                }

                var YAS = bugun.Year - DOGUM_TARIHI.Year;
                if (bugun < DOGUM_TARIHI.AddYears(YAS))
                {
                    YAS--;
                }

                int IZIN_HAKKI;
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
                        // 1 yıl ve 15 yıl dahil 21 gün izin
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
                        // 1-5 yıl arası her yıl 14 gün izin
                        TOPLAM_IZIN = kidemYili * IZIN_HAKKI;
                    }
                    else if (kidemYili <= 15)
                    {
                        IZIN_HAKKI = 21;
                        // 6-15 yıl arası her yıl 21 gün izin
                        TOPLAM_IZIN = (5 * 14) + ((kidemYili - 5) * 21);
                    }
                    else
                    {
                        IZIN_HAKKI = 26;
                        // 15 yıldan fazla her yıl 26 gün izin
                        TOPLAM_IZIN = (5 * 14) + (10 * 21) + ((kidemYili - 15) * 26);
                    }
                }
                DateTime HAKEDIS_TARIHI = ISE_GIRIS_TARIHI.AddYears(1);

                //  string connectionString = "Server=LAPTOP-JK4K9LES\\SQLEXPRESS;Database=veritabani_inot;Integrated Security=True;";
                //  using (SqlConnection connection = new SqlConnection(connectionString))
                // {
                // SqlConnection connection = new SqlConnection(bgl.Adres);
                // connection.Open();
                // string kayitguncelle = ("Update PERSONEL Set AD_SOYAD=@ad, SUBE=@sube,TC_NO=@tcno,BOLUM=@bolum,KIDEM_YILI=@kidem_yili,TOPLAM_IZIN=@toplam_izin,IZIN_HAKKI=@izin_hakki,YAS=@yas,DOGUM_TARIHI=@dogum_tarihi,ISE_GIRIS_TARIHI=@ise_giris_tarihi,HAKEDİS_TARIHI=@hakedis_tarihi where id=@id");
                // using (SqlCommand komut = new SqlCommand(kayitguncelle, connection))
                // {
                SqlCommand komut = new SqlCommand("UPDATE PERSONEL SET AD_SOYAD=@ad, SUBE=@sube, TC_NO=@tcno, BOLUM=@bolum, KIDEM_YILI=@kidem_yili, TOPLAM_IZIN=@toplam_izin, IZIN_HAKKI=@izin_hakki, YAS=@yas, DOGUM_TARIHI=@dogum_tarihi, ISE_GIRIS_TARIHI=@ise_baslama_tarihi, HAKEDİS_TARIHI=@izin_hakedis_tarihi WHERE id=@id", connection);

                komut.Parameters.AddWithValue("@id", dataGridView1.Rows[i].Cells[0].Value);
                        komut.Parameters.AddWithValue("@ad", AD_SOYAD);
                        komut.Parameters.AddWithValue("@sube", SUBE);
                        komut.Parameters.AddWithValue("@tcno", TC_NO);
                        komut.Parameters.AddWithValue("@bolum", BOLUM);
                        komut.Parameters.AddWithValue("@kidem_yili", KIDEM_YILI);
                        komut.Parameters.AddWithValue("@toplam_izin", TOPLAM_IZIN);
                        komut.Parameters.AddWithValue("@izin_hakki", IZIN_HAKKI);
                        komut.Parameters.AddWithValue("@yas", YAS);
                        komut.Parameters.AddWithValue("@dogum_tarihi", DOGUM_TARIHI);
                        komut.Parameters.AddWithValue("@ise_baslama_tarihi", ISE_GIRIS_TARIHI);
                        komut.Parameters.AddWithValue("@izin_hakedis_tarihi", HAKEDIS_TARIHI);

                        connection.Close();
                        connection.Open();
                        komut.ExecuteNonQuery();
                        MessageBox.Show("BAŞARIYLA GÜNCELLENDİ");
                        kayitlarigetir();
                    }
                    textBox5.Clear();
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    dataGridView1.DataSource = null;
                    dataGridView1.Rows.Clear();
                   
                    // DateTimePicker başlangıçta boş gibi görünsün.
                    dateTimePicker1.Format = DateTimePickerFormat.Custom;
                    dateTimePicker1.CustomFormat = " ";  // Boş göster.

                    dateTimePicker2.Format = DateTimePickerFormat.Custom;
                    dateTimePicker2.CustomFormat = " ";  // Boş göster.

                }
       
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            i = e.RowIndex;
            textBox3.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
            textBox1.Text = dataGridView1.Rows[i].Cells[2].Value.ToString();
            textBox2.Text = dataGridView1.Rows[i].Cells[3].Value.ToString();
            comboBolum.Text = dataGridView1.Rows[i].Cells[4].Value.ToString();

            // dateTimePicker1 için tarih değerini ayarlama
            DateTime tarih1;
            if (DateTime.TryParse(dataGridView1.Rows[i].Cells[9].Value.ToString(), out tarih1))
            {
                dateTimePicker1.Value = tarih1;
            }
            else
            {
                MessageBox.Show("Geçersiz tarih formatı: DateTimePicker1");
            }

            // dateTimePicker2 için tarih değerini ayarlama
            DateTime tarih2;
            if (DateTime.TryParse(dataGridView1.Rows[i].Cells[10].Value.ToString(), out tarih2))
            {
                dateTimePicker2.Value = tarih2;
            }
            else
            {
                MessageBox.Show("Geçersiz tarih formatı: DateTimePicker2");
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox5.Clear();
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            // DateTimePicker başlangıçta boş gibi görünsün.
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = " ";  // Boş göster.

            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = " ";  // Boş göster.
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece rakamları ve silme tuşunu (Backspace) izin ver
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; // Geçerli olmayan karakteri işle
            }
        }

        private void textBox2_MouseClick(object sender, MouseEventArgs e)
        {
            textBox2.SelectionStart = 0;
        }

        private void comboBolum_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece harflere ve bazı kontrol tuşlarına (Backspace, Space) izin ver
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Space)
            {
                e.Handled = true; // Geçerli olmayan karakteri işle
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Long;
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.Format = DateTimePickerFormat.Long;
        }
        public static class ThemeSettings
        {
            public static bool DarkTheme { get; set; } = false;
        }
        private void tbDarkTheme_CheckedChanged(object sender, EventArgs e)
        {

            // Statik değişkeni güncelle
            ThemeSettings.DarkTheme = tbDarkTheme.Checked;

            // Temayı uygula
            ApplyTheme();

        }
        private void ApplyTheme()
        {
            if (ThemeSettings.DarkTheme)
            {
                this.BackColor = Color.LightSlateGray;
                label1.ForeColor = Color.White;
            }
            else
            {
                this.BackColor = Color.White;
                label1.ForeColor = Color.LightSlateGray;
            }
        }
    }
}
