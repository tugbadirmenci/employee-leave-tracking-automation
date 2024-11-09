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
using Newtonsoft.Json.Linq; // JSON parsing için Newtonsoft.Json kütüphanesi
using System.Net.Http;
using static izinTakip.personelEkle;
using static izinTakip.TatilTablosu;

namespace izinTakip
{
    public partial class izinEkle : Form
    {
        public izinEkle()
        {
            InitializeComponent();
        }
        //   SqlConnection connection = new SqlConnection("Data Source=LAPTOP-JK4K9LES\\SQLEXPRESS;Initial Catalog = veritabani_inot; Integrated Security = True");
        BaglantiSinifi bgl = new BaglantiSinifi();

        private void button4_Click(object sender, EventArgs e)
        /*{
            
                // Başlangıç tarihi ve izin gününü al
                DateTime baslangicTarihi = dateTimePicker1.Value.Date;
                int izinGunu = int.Parse(textBox6.Text);

                // İzin bitiş tarihini hesaplamak için geçici bir değişken
                DateTime bitisTarihi = baslangicTarihi;

                // Resmi tatiller ve pazar günlerini sorgula
                List<DateTime> tatiller = GetTatilList();

                // İzin günlerini hesaplarken hafta sonu ve tatil günlerini hesaba kat
                int izinSayaci = 0;
                while (izinSayaci < izinGunu)
                {
                    bitisTarihi = bitisTarihi.AddDays(1);

                    // Eğer gün pazar değilse ve resmi tatil değilse izin sayacını arttır
                    if (!IsWeekendOrHoliday(bitisTarihi, tatiller))
                    {
                        izinSayaci++;
                    }
                }
                // İzin bitiş tarihi bir gün geri alınmalı çünkü son gün iş başı günü olacak
                bitisTarihi = bitisTarihi.AddDays(-1);

            // İş başı tarihini izin bitiş tarihinden bir gün sonrasına ayarla
            DateTime isBasiTarihi = bitisTarihi.AddDays(1);

                // İş başı tarihini kontrol et ve hafta sonu veya resmi tatil ise sonraki günü bul
                isBasiTarihi = CheckForWeekendOrHoliday(isBasiTarihi, tatiller);

                // Sonuçları datetimepicker'lara yaz
                dateTimePicker2.Value = bitisTarihi;  // İzin bitiş tarihi
                dateTimePicker3.Value = isBasiTarihi; // İş başı tarihi
            }

            private List<DateTime> GetTatilList()
            {
                List<DateTime> tatiller = new List<DateTime>();

            // using (SqlConnection connection = new SqlConnection("Server=LAPTOP-JK4K9LES\\SQLEXPRESS;Database=veritabani_inot;Integrated Security=True;"))
            //  {
            SqlConnection connection = new SqlConnection(bgl.Adres);
            

            connection.Open();
                    SqlCommand command = new SqlCommand("SELECT TATIL_TARIHI FROM TATILLER", connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        tatiller.Add(reader.GetDateTime(0));
                    }
             
                return tatiller; 

        }
        private bool IsWeekendOrHoliday(DateTime tarih, List<DateTime> tatiller)
        {
            // Pazar gününü kontrol et
            if (tarih.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }

            // Resmi tatiller arasında kontrol et
            if (tatiller.Contains(tarih.Date))
            {
                return true;
            }

            return false;


        }
        private DateTime CheckForWeekendOrHoliday(DateTime tarih, List<DateTime> TATILLER)
        {
            while (IsWeekendOrHoliday(tarih, TATILLER))
            {
                tarih = tarih.AddDays(1);
            }
            return tarih; 


        } */

        {
            // Başlangıç tarihi ve izin gününü al
            DateTime baslangicTarihi = dateTimePicker1.Value.Date;
            int izinGunu = int.Parse(textBox6.Text);

            // İzin bitiş tarihini hesaplamak için geçici bir değişken
            DateTime bitisTarihi = baslangicTarihi;

            // Resmi tatiller ve pazar günlerini sorgula
            List<DateTime> tatiller = GetTatilList();

            // Cumartesi günlerinin de izin gününden sayılmasını kontrol et
            bool cumartesiIzinGunu = checkBox1.Checked;

          

            // İzin günlerini hesaplarken hafta sonu ve tatil günlerini hesaba kat
            int izinSayaci = 0;
            while (izinSayaci < izinGunu)
            {
                bitisTarihi = bitisTarihi.AddDays(1);

                // Cumartesi günlerini de kontrol et
                if (!IsWeekendOrHoliday(bitisTarihi, tatiller, cumartesiIzinGunu))
                {
                    izinSayaci++;
                }
            }
            // İzin bitiş tarihi bir gün geri alınmalı çünkü son gün iş başı günü olacak
            bitisTarihi = bitisTarihi.AddDays(-1);

            // İş başı tarihini izin bitiş tarihinden bir gün sonrasına ayarla
            DateTime isBasiTarihi = bitisTarihi.AddDays(1);

            // İş başı tarihini kontrol et ve hafta sonu veya resmi tatil ise sonraki günü bul
            isBasiTarihi = CheckForWeekendOrHoliday(isBasiTarihi, tatiller);

            // Sonuçları datetimepicker'lara yaz
            dateTimePicker2.Value = bitisTarihi;  // İzin bitiş tarihi
            dateTimePicker3.Value = isBasiTarihi; // İş başı tarihi
        }

        private List<DateTime> GetTatilList()
        {
            List<DateTime> tatiller = new List<DateTime>();

            SqlConnection connection = new SqlConnection(bgl.Adres);

            connection.Open();
            SqlCommand command = new SqlCommand("SELECT TATIL_TARIHI FROM TATILLER", connection);
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                tatiller.Add(reader.GetDateTime(0));
            }
            return tatiller;
        }

        private bool IsWeekendOrHoliday(DateTime tarih, List<DateTime> tatiller, bool cumartesiIzinGunu)
        {
            // Pazar gününü kontrol et
            if (tarih.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }

            // Cumartesi günlerini kontrol et
            if (cumartesiIzinGunu && tarih.DayOfWeek == DayOfWeek.Saturday)
            {
                return true;
            }

            // Resmi tatiller arasında kontrol et
            if (tatiller.Contains(tarih.Date))
            {
                return true;
            }

            return false;
        }

        private DateTime CheckForWeekendOrHoliday(DateTime tarih, List<DateTime> tatiller)
        {
            while (IsWeekendOrHoliday(tarih, tatiller, false)) // Cumartesi günlerini kontrol etme
            {
                tarih = tarih.AddDays(1);
            }
            return tarih;
        }


        private void izinEkle_Load(object sender, EventArgs e)
        {
            // Statik değişkenden tema tercihlerini al ve uygula
            tbDarkTheme.Checked = ThemeSettings.DarkTheme;
            ApplyTheme();

            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
            textBox4.Enabled = false;


            // Ayarları yükle
            checkBox1.Checked = Properties.Settings.Default.CheckBox1IsChecked;

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
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                checkBox1.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            DialogResult res;
            res = MessageBox.Show("Anasayfaya dönmek istediğinizden emin misiniz?", "Anasayfaya Git", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                Anasayfa frm9 = new Anasayfa();
                frm9.Show();
                this.Hide();
            }
            else
            {
                this.Show();
            }

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

        private void button6_Click(object sender, EventArgs e)
        {

            // Eğer TextBox boşsa işlemi gerçekleştirme
            if (string.IsNullOrWhiteSpace(textBox7.Text))
            {
                MessageBox.Show("Lütfen bir isim girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // İşlemi sonlandır
            }

            SqlConnection connection = new SqlConnection(bgl.Adres);
            connection.Open();
            // Girilen ismin ilk üç harfini alır ve wildcard ile birlikte kullanır
            string kayit = "SELECT * FROM PERSONEL WHERE AD_SOYAD LIKE @ad";
            SqlCommand command = new SqlCommand(kayit, connection);
            command.Parameters.AddWithValue("@ad", textBox7.Text + "%");

            SqlDataAdapter da = new SqlDataAdapter(command);

            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;
            connection.Close();
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
        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            textBox7.Clear();
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

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece harflere ve bazı kontrol tuşlarına (Backspace, Space) izin ver
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Space)
            {
                e.Handled = true; // Geçerli olmayan karakteri işle
            }
        }
        int i = 0;

        private void button1_Click(object sender, EventArgs e)
        {

            // Form üzerindeki tüm kontrolleri kontrol et
            bool allFieldsValid = true;

            // Boş alanları kontrol et
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                 string.IsNullOrWhiteSpace(textBox5.Text) ||
                 string.IsNullOrWhiteSpace(textBox6.Text) ||

                 dateTimePicker1.Text == " " || dateTimePicker2.Text == " " || dateTimePicker3.Text == " ")
            {
                allFieldsValid = false;
            }

            if (!allFieldsValid)
            {
                MessageBox.Show("Lütfen tüm gerekli alanları doldurunuz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {

                string AD_SOYAD = textBox1.Text;
                string SUBE = textBox3.Text;
                string TC_NO = textBox2.Text;
                DateTime IZIN_CIKIS_TARIHI = DateTime.Parse(dateTimePicker1.Text);
                DateTime IZIN_BITIS_TARIHI = DateTime.Parse(dateTimePicker2.Text);
                DateTime ISBASI_TARIHI = DateTime.Parse(dateTimePicker3.Text);
                int KULLANILAN_IZIN = int.Parse(textBox6.Text);
                string ACIKLAMA = textBox5.Text;

                DateTime bugun = DateTime.Now;

               // string connectionString = "Server=LAPTOP-JK4K9LES\\SQLEXPRESS;Database=veritabani_inot;Integrated Security=True;";
                using (SqlConnection connection = new SqlConnection(bgl.Adres))
                {
                   
                    connection.Open();
                    // Öncelikle, personelin en son işbaşı tarihini alıyoruz
                    string getLastIsBasiTarihiQuery = "SELECT MAX(ISBASI_TARIHI) FROM IZIN_KAYIT WHERE TC_NO = @tcno";

                    DateTime? lastIsbasiTarihi = null;
                    using (SqlCommand getLastIsBasiTarihiCommand = new SqlCommand(getLastIsBasiTarihiQuery, connection))
                    {
                        getLastIsBasiTarihiCommand.Parameters.AddWithValue("@tcno", TC_NO);
                        object result = getLastIsBasiTarihiCommand.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            lastIsbasiTarihi = (DateTime)result;
                        }
                    }

                    // Eğer bir önceki işbaşı tarihi varsa ve yeni izin başlangıç tarihi bu tarihten önceyse izin eklenmesin
                    if (lastIsbasiTarihi.HasValue && IZIN_CIKIS_TARIHI <= lastIsbasiTarihi.Value)
                    {
                        MessageBox.Show("Bu personel için işbaşı tarihinden önce yeni bir izin eklenemez. Lütfen geçerli bir tarih seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // İzin kaydı yapılmadan çık
                    }
                    // Öncelikle tarihler arasında çakışma olup olmadığını kontrol et
                    string checkOverlapQuery = "SELECT COUNT(*) FROM IZIN_KAYIT WHERE TC_NO = @tcno " +
                                               "AND (IZIN_CIKIS_TARIHI BETWEEN @izin_cikis_tarihi AND @isbasi_tarihi " +
                                               "OR ISBASI_TARIHI BETWEEN @izin_cikis_tarihi AND @isbasi_tarihi " +
                                               "OR @izin_cikis_tarihi BETWEEN IZIN_CIKIS_TARIHI AND ISBASI_TARIHI " +
                                               "OR @isbasi_tarihi BETWEEN IZIN_CIKIS_TARIHI AND ISBASI_TARIHI)";

                    using (SqlCommand checkCommand = new SqlCommand(checkOverlapQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@tcno", TC_NO);
                        checkCommand.Parameters.AddWithValue("@izin_cikis_tarihi", IZIN_CIKIS_TARIHI);
                        checkCommand.Parameters.AddWithValue("@isbasi_tarihi", ISBASI_TARIHI);

                        int overlappingCount = (int)checkCommand.ExecuteScalar();

                        // Eğer çakışma varsa izin kaydedilmesin
                        if (overlappingCount > 0)
                        {
                            MessageBox.Show("Belirtilen tarihlerde zaten bir izin kaydı var. Lütfen başka bir tarih aralığı seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return; // İzin kaydı yapılmadan çık
                        }
                    }

                    string insertquery = "INSERT INTO IZIN_KAYIT (AD_SOYAD,SUBE,TC_NO,IZIN_CIKIS_TARIHI,ISBASI_TARIHI,KULLANILAN_IZIN,ACIKLAMA) " +
                               "VALUES (@ad, @sube, @tcno,@izin_cikis_tarihi,@isbasi_tarihi,@kullanilan_izin,@aciklama)";

                    using (SqlCommand insertcommand = new SqlCommand(insertquery, connection))
                    {

                        insertcommand.Parameters.AddWithValue("@ad", AD_SOYAD);
                        insertcommand.Parameters.AddWithValue("@sube", SUBE);
                        insertcommand.Parameters.AddWithValue("@tcno", TC_NO);
                        insertcommand.Parameters.AddWithValue("@izin_cikis_tarihi", IZIN_CIKIS_TARIHI);
                        insertcommand.Parameters.AddWithValue("@isbasi_tarihi", ISBASI_TARIHI);
                        insertcommand.Parameters.AddWithValue("@kullanilan_izin", KULLANILAN_IZIN);
                        insertcommand.Parameters.AddWithValue("@aciklama", ACIKLAMA);

                        insertcommand.ExecuteNonQuery();

                    }
                    string updateQuery = "UPDATE PERSONEL SET TOPLAM_IZIN = TOPLAM_IZIN - @kullanilan_izin WHERE TC_NO = @tcno";

                    using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@kullanilan_izin", KULLANILAN_IZIN);
                        updateCommand.Parameters.AddWithValue("@tcno", TC_NO);

                        updateCommand.ExecuteNonQuery();
                    }

                    MessageBox.Show("İZİN KAYIT LİSTESİNE KAYDEDİLDİ.");

                }
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                textBox6.Clear();
                textBox7.Clear();
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


        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            i = e.RowIndex;
            textBox1.Text = dataGridView1.Rows[i].Cells[1].Value.ToString();
            textBox2.Text = dataGridView1.Rows[i].Cells[3].Value.ToString();
            textBox3.Text = dataGridView1.Rows[i].Cells[2].Value.ToString();
            textBox4.Text = dataGridView1.Rows[i].Cells[6].Value.ToString();

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

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Checkbox'ın durumunu ayarlarına kaydet
            Properties.Settings.Default.CheckBox1IsChecked = checkBox1.Checked;
            Properties.Settings.Default.Save(); // Ayarları kaydet
        }

        private void izinEkle_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.CheckBox1IsChecked = checkBox1.Checked;
            Properties.Settings.Default.Save(); // Ayarları kaydet
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

        

    

  
    

