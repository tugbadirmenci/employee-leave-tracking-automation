
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;

namespace izinTakip
{
    public partial class personelEkle : Form
    {
        public personelEkle()
        {
            InitializeComponent();
        }

        BaglantiSinifi bgl = new BaglantiSinifi();

        // SqlConnection con = new SqlConnection("Data Source=LAPTOP-JK4K9LES\\SQLEXPRESS;Initial Catalog = veritabani_inot; Integrated Security = True");

        public static class CurrentUser
        {
            public static string Role { get; set; } // Kullanıcının rolünü saklamak için
        }

        private void personelEkle_Load(object sender, EventArgs e)
        {
            // Statik değişkenden tema tercihlerini al ve uygula
            tbDarkTheme.Checked = ThemeSettings.DarkTheme;
            ApplyTheme();

            // Kullanıcının rolü 'user' ise belirli TextBox'ları devre dışı bırak
            if (CurrentUser.Role == "User")
            {
                txtSube.Enabled = false; // textBox1'in düzenlenmesini engelle
            }
            else if (CurrentUser.Role == "Admin")
            {
                // Admin rolü için tüm TextBox'lar etkin olabilir
                txtAd.Enabled = true;
                txtSube.Enabled = true;
                txtTcNo.Enabled = true;

            }
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
            dtpDogumTarihi.Format = DateTimePickerFormat.Custom;
            dtpDogumTarihi.CustomFormat = " ";  // Boş göster.

            dtpIseGirisTarihi.Format = DateTimePickerFormat.Custom;
            dtpIseGirisTarihi.CustomFormat = " ";  // Boş göster.

        }

        private void button2_Click(object sender, EventArgs e)
        {

            Anasayfa frm7 = new Anasayfa();
            frm7.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
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
        private void dtpDogumTarihi_ValueChanged(object sender, EventArgs e)
        {
            // Kullanıcı bir tarih seçtiğinde normal formatına dön.
            dtpDogumTarihi.Format = DateTimePickerFormat.Long;
   

        }

        private void dtpIseGirisTarihi_ValueChanged(object sender, EventArgs e)
        {
            // Kullanıcı bir tarih seçtiğinde normal formatına dön.
            dtpIseGirisTarihi.Format = DateTimePickerFormat.Long;
            

        }

        private void button1_Click(object sender, EventArgs e)
        {

            // Form üzerindeki tüm kontrolleri kontrol et
            bool allFieldsValid = true;

            // Boş alanları kontrol et
            if (string.IsNullOrWhiteSpace(txtAd.Text) ||
                string.IsNullOrWhiteSpace(txtSube.Text) ||
                string.IsNullOrWhiteSpace(txtTcNo.Text) ||
                string.IsNullOrWhiteSpace(comboBolum.Text) ||
               dtpDogumTarihi.Text == " " || dtpIseGirisTarihi.Text == " ")
            {
                allFieldsValid = false;
            }

            if (!allFieldsValid)
            {
                MessageBox.Show("Lütfen tüm gerekli alanları doldurunuz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                // Kaydetme işlemini burada yapabilirsiniz
                string AD_SOYAD = txtAd.Text;
                string SUBE = txtSube.Text;
                string TC_NO = txtTcNo.Text;
                DateTime DOGUM_TARIHI = DateTime.Parse(dtpDogumTarihi.Text);
                string BOLUM = comboBolum.Text;
                DateTime ISE_GIRIS_TARIHI = DateTime.Parse(dtpIseGirisTarihi.Text);
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
                DateTime HAKEDİS_TARIHI = ISE_GIRIS_TARIHI.AddYears(1);

                // string connectionString = "Server=LAPTOP-JK4K9LES\\SQLEXPRESS;Database=veritabani_inot;Integrated Security=True;";
                // using (SqlConnection connection = new SqlConnection(connectionString))
                // {
                SqlConnection connection = new SqlConnection(bgl.Adres);
                connection.Open();

                //     string query = "INSERT INTO personel (AD_SOYAD,SUBE,TC_NO,BOLUM,KIDEM_YILI,TOPLAM_IZIN,IZIN_HAKKI,YAS,DOGUM_TARIHI,ISE_GIRIS_TARIHI,HAKEDİS_TARIHI) " +
                //              "VALUES (@ad, @sube, @tcno,@bolum,@kidem_yili,@toplam_izin,@izin_hakki,@yas, @dogum_tarihi,  @ise_baslama_tarihi, @izin_hakedis_tarihi)";

                // using (SqlCommand command = new SqlCommand(query, connection))
                //  {

                SqlCommand command = new SqlCommand("insert into PERSONEL (AD_SOYAD,SUBE,TC_NO,BOLUM,KIDEM_YILI,TOPLAM_IZIN,IZIN_HAKKI,YAS,DOGUM_TARIHI,ISE_GIRIS_TARIHI,HAKEDİS_TARIHI) values (@ad, @sube, @tcno,@bolum,@kidem_yili,@toplam_izin,@izin_hakki,@yas, @dogum_tarihi,  @ise_baslama_tarihi, @izin_hakedis_tarihi)", connection);
                        
                        command.Parameters.AddWithValue("@ad", AD_SOYAD);
                        command.Parameters.AddWithValue("@sube", SUBE);
                        command.Parameters.AddWithValue("@tcno", TC_NO);
                        command.Parameters.AddWithValue("@dogum_tarihi", DOGUM_TARIHI);
                        command.Parameters.AddWithValue("@bolum", BOLUM);
                        command.Parameters.AddWithValue("@ise_baslama_tarihi", ISE_GIRIS_TARIHI);
                        command.Parameters.AddWithValue("@kidem_yili", KIDEM_YILI);
                        command.Parameters.AddWithValue("@yas", YAS);
                        command.Parameters.AddWithValue("@izin_hakki", IZIN_HAKKI);
                        command.Parameters.AddWithValue("@toplam_izin", TOPLAM_IZIN);
                        command.Parameters.AddWithValue("@izin_hakedis_tarihi", HAKEDİS_TARIHI);

                        connection.Close(); // open dı 
                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("PERSONEL EKLENDİ");

            }
                    txtTcNo.Clear();
                    txtAd.Clear();
                    // DateTimePicker başlangıçta boş gibi görünsün.
                    dtpDogumTarihi.Format = DateTimePickerFormat.Custom;
                    dtpDogumTarihi.CustomFormat = " ";  // Boş göster. 
                    // DateTimePicker başlangıçta boş gibi görünsün.
                    dtpIseGirisTarihi.Format = DateTimePickerFormat.Custom;
                    dtpIseGirisTarihi.CustomFormat = " ";  // Boş göster.                              
        }
            
       
        private void txtAd_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece harflere ve bazı kontrol tuşlarına (Backspace, Space) izin ver
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Space)
            {
                e.Handled = true; // Geçerli olmayan karakteri işle
            }
        }

        private void txtTcNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece rakamları ve silme tuşunu (Backspace) izin ver
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; // Geçerli olmayan karakteri işle
            }
        }

        private void txtTcNo_MouseClick(object sender, MouseEventArgs e)
        {
            txtTcNo.SelectionStart = 0;
        }

        private void comboBolum_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            txtTcNo.Clear();
            txtAd.Clear();
            // DateTimePicker başlangıçta boş gibi görünsün.
            dtpDogumTarihi.Format = DateTimePickerFormat.Custom;
            dtpDogumTarihi.CustomFormat = " ";  // Boş göster. 
                                                // DateTimePicker başlangıçta boş gibi görünsün.
            dtpIseGirisTarihi.Format = DateTimePickerFormat.Custom;
            dtpIseGirisTarihi.CustomFormat = " ";  // Boş göster.   
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


