using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static izinTakip.personelEkle;

namespace izinTakip
{
    public partial class cikisKaydi : Form
    {
        public cikisKaydi()
        {
            InitializeComponent();
        }
        BaglantiSinifi bgl = new BaglantiSinifi();

        // SqlConnection con = new SqlConnection("Data Source=LAPTOP-JK4K9LES\\SQLEXPRESS;Initial Catalog = veritabani_inot; Integrated Security = True");


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

        private void cikisKaydi_Load(object sender, EventArgs e)
        {
            // Statik değişkenden tema tercihlerini al ve uygula
            tbDarkTheme.Checked = ThemeSettings.DarkTheme;
            ApplyTheme();

            dateTimePicker1.Enabled = false;
            dateTimePicker2.Enabled = false;
            textBox4.Enabled = false;
            textBox3.Enabled = false;
            textBox2.Enabled = false;
            textBox1.Enabled = false;

            SqlConnection connection = new SqlConnection(bgl.Adres);

            SqlCommand com = new SqlCommand("SELECT * FROM BOLUMLER", connection);
            SqlDataReader dr;

            connection.Open();
            dr = com.ExecuteReader();

            while (dr.Read())
            {
                textBox4.Items.Add(dr["BOLUMLER"]);
            }

            connection.Close();


            // DateTimePicker başlangıçta boş gibi görünsün.
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = " ";  // Boş göster.

            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = " ";  // Boş göster.

            dateTimePicker3.Format = DateTimePickerFormat.Custom;
            dateTimePicker3.CustomFormat = " ";  // Boş göster.

            // Kullanıcının rolü 'user' ise belirli TextBox'ları devre dışı bırak
            if (CurrentUser.Role == "User")
            {
                textBox2.Enabled = false;        
                button5.Enabled = false;
                textBox3.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Anasayfa frm11 = new Anasayfa();
            frm11.Show();
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
        int i = 0;
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            i = e.RowIndex;
            textBox1.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
            textBox2.Text = dataGridView1.Rows[i].Cells[2].Value.ToString();
            textBox3.Text = dataGridView1.Rows[i].Cells[3].Value.ToString();
            textBox4.Text = dataGridView1.Rows[i].Cells[4].Value.ToString();
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

        private void button1_Click(object sender, EventArgs e)
        {
            // Form üzerindeki tüm kontrolleri kontrol et
            bool allFieldsValid = true;

            // Boş alanları kontrol et
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||   // textBox1 boş mu?
                string.IsNullOrWhiteSpace(textBox2.Text) ||   // textBox2 boş mu?
                string.IsNullOrWhiteSpace(textBox3.Text) ||   // textBox3 boş mu?
                string.IsNullOrWhiteSpace(textBox4.Text) ||   // textBox4 boş mu?
                string.IsNullOrWhiteSpace(textBox5.Text) ||   // textBox4 boş mu?
                 dateTimePicker1.Text == " " || dateTimePicker2.Text == " " || dateTimePicker3.Text == " ")
            {
                allFieldsValid = false;
            }

            // Eğer alanlar boş ise hata mesajı göster
            if (!allFieldsValid)
            {
                MessageBox.Show("Lütfen tüm gerekli alanları doldurunuz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {

                // Kullanıcıya personel çıkışını yapmak istediğinden emin olup olmadığını sormak için onay kutusu göster
                DialogResult result = MessageBox.Show("Personel çıkışı yapmak istediğinize emin misiniz?", "Personel Çıkışı Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {

                    string AD_SOYAD = textBox1.Text;
                    string SUBE = textBox2.Text;
                    string TC_NO = textBox3.Text;
                    DateTime DOGUM_TARIHI = DateTime.Parse(dateTimePicker1.Text);
                    string BOLUM = textBox4.Text;
                    DateTime ISE_GIRIS_TARIHI = DateTime.Parse(dateTimePicker2.Text);
                    DateTime IS_CIKIS_TARIHI = DateTime.Parse(dateTimePicker3.Text);
                    string ACIKLAMA = textBox5.Text;
                    DateTime bugun = DateTime.Now;

                    var KIDEM_YILI = bugun.Year - ISE_GIRIS_TARIHI.Year;
                    if (bugun < ISE_GIRIS_TARIHI.AddYears(KIDEM_YILI))
                    {
                        KIDEM_YILI--;
                    }

                    var YAS = bugun.Year - DOGUM_TARIHI.Year;
                    if (bugun < DOGUM_TARIHI.AddYears(YAS))
                    {
                        YAS--;
                    }

                    int MEVCUT_IZIN_HAKKI;

                    if (YAS > 50)
                    {
                        if (KIDEM_YILI < 1)
                        {
                            MEVCUT_IZIN_HAKKI = 0;
                        }
                        else if (KIDEM_YILI <= 15)
                        {
                            MEVCUT_IZIN_HAKKI = 21;
                        }
                        else
                        {
                            MEVCUT_IZIN_HAKKI = 26;
                        }
                    }
                    else
                    {
                        if (KIDEM_YILI < 1)
                        {
                            MEVCUT_IZIN_HAKKI = 0;
                        }
                        else if (KIDEM_YILI <= 5)
                        {
                            MEVCUT_IZIN_HAKKI = 14;
                        }
                        else if (KIDEM_YILI <= 15)
                        {
                            MEVCUT_IZIN_HAKKI = 21;
                        }
                        else
                        {
                            MEVCUT_IZIN_HAKKI = 26;
                        }
                    }

                    //   string connectionString = "Server=LAPTOP-JK4K9LES\\SQLEXPRESS;Database=veritabani_inot;Integrated Security=True;";
                   // SqlConnection connection = new SqlConnection(bgl.Adres);

                    using (SqlConnection connection = new SqlConnection(bgl.Adres))
                    {
                        connection.Open();

                        string insertquery = "INSERT INTO CIKAN_PERSONEL (AD_SOYAD,SUBE,TC_NO,DOGUM_TARIHI,ISE_GIRIS_TARIHI,BOLUM,MEVCUT_IZIN_HAKKI,IS_CIKIS_TARIHI,ACIKLAMA) " +
                                   "VALUES (@ad, @sube, @tcno,@dogum_tarihi,@ise_baslama_tarihi,@bolum,@mevcut_izin_hakki,@is_cikis_tarihi,@aciklama)";

                        using (SqlCommand insertcommand = new SqlCommand(insertquery, connection))
                        {
                            insertcommand.Parameters.AddWithValue("@ad", AD_SOYAD);
                            insertcommand.Parameters.AddWithValue("@sube", SUBE);
                            insertcommand.Parameters.AddWithValue("@tcno", TC_NO);
                            insertcommand.Parameters.AddWithValue("@dogum_tarihi", DOGUM_TARIHI);
                            insertcommand.Parameters.AddWithValue("@ise_baslama_tarihi", ISE_GIRIS_TARIHI);
                            insertcommand.Parameters.AddWithValue("@bolum", BOLUM);
                            insertcommand.Parameters.AddWithValue("@mevcut_izin_hakki", MEVCUT_IZIN_HAKKI);
                            insertcommand.Parameters.AddWithValue("@is_cikis_tarihi", IS_CIKIS_TARIHI);
                            insertcommand.Parameters.AddWithValue("@aciklama", ACIKLAMA);
                            insertcommand.ExecuteNonQuery();
                        }

                        string deleteQuery = "DELETE FROM PERSONEL WHERE TC_NO = @tcno";
                        using (SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection))
                        {
                            deleteCommand.Parameters.AddWithValue("@tcno", TC_NO);
                            deleteCommand.ExecuteNonQuery();
                        }

                        MessageBox.Show("PERSONEL ÇIKIŞI YAPILDI");

                    }

                    textBox1.Clear();
                    textBox2.Clear();
                    textBox3.Clear();
                    textBox5.Clear();
                    textBox6.Clear();
                    dataGridView1.DataSource = null;
                    dataGridView1.Rows.Clear();
                    // DateTimePicker başlangıçta boş gibi görünsün.
                    dateTimePicker1.Format = DateTimePickerFormat.Custom;
                    dateTimePicker1.CustomFormat = " ";  // Boş göster.

                    dateTimePicker2.Format = DateTimePickerFormat.Custom;
                    dateTimePicker2.CustomFormat = " ";  // Boş göster.

                    dateTimePicker3.Format = DateTimePickerFormat.Custom;
                    dateTimePicker3.CustomFormat = " ";  // Boş göster.
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            // Eğer TextBox boşsa işlemi gerçekleştirme
            if (string.IsNullOrWhiteSpace(textBox6.Text))
            {
                MessageBox.Show("Lütfen bir isim girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi sonlandır
            }

            SqlConnection connection = new SqlConnection(bgl.Adres);
            connection.Open();
            // Girilen ismin ilk üç harfini alır ve wildcard ile birlikte kullanır
            string kayit = "SELECT * FROM PERSONEL WHERE AD_SOYAD LIKE @ad";
            SqlCommand command = new SqlCommand(kayit, connection);
            command.Parameters.AddWithValue("@ad", textBox6.Text + "%");

            SqlDataAdapter da = new SqlDataAdapter(command);

            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            connection.Close(); 
        }

        private void button5_Click(object sender, EventArgs e)
        {
            kayitlarigetir();
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece rakamları ve silme tuşunu (Backspace) izin ver
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)Keys.Back)
            {
                e.Handled = true; // Geçerli olmayan karakteri işle
            }
        }

        private void textBox3_MouseClick(object sender, MouseEventArgs e)
        {
            textBox3.SelectionStart = 0;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece harflere ve bazı kontrol tuşlarına (Backspace, Space) izin ver
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Space)
            {
                e.Handled = true; // Geçerli olmayan karakteri işle
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox3.Clear();
            textBox5.Clear();
            textBox6.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            // DateTimePicker başlangıçta boş gibi görünsün.
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = " ";  // Boş göster.

            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = " ";  // Boş göster.

            dateTimePicker3.Format = DateTimePickerFormat.Custom;
            dateTimePicker3.CustomFormat = " ";  // Boş göster.
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Format = DateTimePickerFormat.Long;
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker2.Format = DateTimePickerFormat.Long;
        }

        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            dateTimePicker3.Format = DateTimePickerFormat.Long;
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
