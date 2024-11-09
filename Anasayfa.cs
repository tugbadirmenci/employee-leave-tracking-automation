using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static izinTakip.personelEkle;

namespace izinTakip
{
    public partial class Anasayfa : Form
    {
        public Anasayfa()
        {
            InitializeComponent();
        }
        private void Anasayfa_Load(object sender, EventArgs e)
        {
            // Önceki logo kaydedildiyse yükle
            string logoYolu = Properties.Settings.Default.LogoYolu;
            if (!string.IsNullOrEmpty(logoYolu) && File.Exists(logoYolu))
            {
                try
                {
                    // Kaydedilen resmi yükle
                    using (Image kaydedilenResim = Image.FromFile(logoYolu))
                    {
                        pictureBox1.Image = new Bitmap(kaydedilenResim);  // Resmi yükle

                        // PictureBox ayarlarını yap
                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;  // Resmi sığdır ve ortala
                        pictureBox1.Width = 700; // PictureBox genişliği
                        pictureBox1.Height = 150; // PictureBox yüksekliği
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Resim yüklenirken hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Kullanıcının rolü 'user' ise belirli TextBox'ları devre dışı bırak
            if (CurrentUser.Role == "User")
            {
                groupBox2.Enabled = false; // textBox1'in düzenlenmesini engelle
                button4.Enabled = false;
                button7.Enabled = false;
                button8.Enabled = false;
                button9.Enabled = false;
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            personelEkle frm2 = new personelEkle();
            frm2.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            PersonelDüzenle frm3 = new PersonelDüzenle();
            frm3.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            izinEkle frm4 = new izinEkle();
            frm4.Show();
            this.Hide();
           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            
            personelListesi frm5 = new personelListesi();
            frm5.Show();
            this.Hide();

        }

        private void button5_Click(object sender, EventArgs e)
        {
           
            cikisKaydi frm6 = new cikisKaydi();
            frm6.Show();
            this.Hide();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            İzinOzetTablosu frm14 = new İzinOzetTablosu();
            frm14.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
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

        private void button9_Click(object sender, EventArgs e)
        {
            İzinKayitTablosu frm15 = new İzinKayitTablosu();
            frm15.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            CikanPersonel frm16 = new CikanPersonel();
            frm16.Show();
            this.Hide();
        }

        /* private void button10_Click(object sender, EventArgs e)
         {
             // OpenFileDialog ile yeni logo seçimi
             OpenFileDialog openFileDialog = new OpenFileDialog();
             openFileDialog.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp";
             openFileDialog.Title = "Yeni Logoyu Seçin";

             if (openFileDialog.ShowDialog() == DialogResult.OK)
             {
                 // Seçilen dosya yolu
                 string yeniLogoYolu = openFileDialog.FileName;

                 // Kullanıcıya soruyu sorma
                 DialogResult result = MessageBox.Show("Yeni logoyu kaydetmek istiyor musunuz?",
                                                       "Logo Değiştirme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                 if (result == DialogResult.Yes)
                 {
                     try
                     {
                         // Uygulama klasörüne yeni logoyu kopyala
                         string uygulamaKlasoru = Application.StartupPath;
                         string hedefYol = Path.Combine(uygulamaKlasoru, "logo.jpg");

                         // Eğer eski logo varsa sil
                         if (File.Exists(hedefYol))
                         {
                             File.Delete(hedefYol);
                         }

                         // Yeni logoyu kaydet
                         File.Copy(yeniLogoYolu, hedefYol);

                         // Seçilen yeni logoyu PictureBox'ta göster
                         pictureBox1.Image = ResimYukle(hedefYol); // Yeni yöntemle resmi yükle

                         // Resim yolunu ayarlara kaydet
                         Properties.Settings.Default.LogoYolu = hedefYol;
                         Properties.Settings.Default.Save();

                         MessageBox.Show("Logo başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                     }
                     catch (Exception ex)
                     {
                         MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                     }
                 }
             }
         }
         /*  private void LogoYukle()
           {
               // Kaydedilen logo yolunu al
               string kayitliLogoYolu = Properties.Settings.Default.LogoYolu;

               // Eğer logo yolu boş değilse ve dosya varsa logoyu yükle
               if (!string.IsNullOrEmpty(kayitliLogoYolu) && File.Exists(kayitliLogoYolu))
               {
                   pictureBox1.Image = Image.FromFile(kayitliLogoYolu);
               }
           } 
         private Image ResimYukle(string dosyaYolu)
         {
             using (var stream = new FileStream(dosyaYolu, FileMode.Open, FileAccess.Read))
             {
                 return Image.FromStream(stream);
             }
         }
         */
        private void button10_Click(object sender, EventArgs e)
        {
            // OpenFileDialog ile yeni logo seçimi
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog.Title = "Yeni Logoyu Seçin";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Seçilen dosya yolu
                string yeniLogoYolu = openFileDialog.FileName;

                try
                {
                    // Seçilen resmin boyutlarını kontrol et
                    using (Image secilenResim = Image.FromFile(yeniLogoYolu))
                    {
                       /* if (secilenResim.Width > 1500 || secilenResim.Height > 350)
                        {
                            MessageBox.Show("Resim boyutları 700x150 pikselden büyük olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        } */
                        if (secilenResim.Width < 400 || secilenResim.Height < 100)
                        {
                            MessageBox.Show("Resim boyutları 300x75 pikselden küçük olamaz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        } 

                        // Resmi bellekten yüklemek için MemoryStream kullan
                        using (MemoryStream ms = new MemoryStream())
                        {
                            secilenResim.Save(ms, secilenResim.RawFormat);
                            pictureBox1.Image = Image.FromStream(ms);
                        }

                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;  // Resmi PictureBox'a sığdır ve ortala
                        pictureBox1.Width = 700;  // PictureBox genişliği
                        pictureBox1.Height = 150; // PictureBox yüksekliği
                    }

                    // Kullanıcıya soruyu sorma
                    DialogResult result = MessageBox.Show("Yeni logoyu kaydetmek istiyor musunuz?",
                                                          "Logo Değiştirme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Uygulama klasörüne yeni logoyu kopyala
                        string uygulamaKlasoru = Application.StartupPath;
                        string hedefYol = Path.Combine(uygulamaKlasoru, "logo.jpg");

                        // Eğer eski logo varsa sil
                        if (File.Exists(hedefYol))
                        {
                            File.Delete(hedefYol);
                        }

                        // Yeni logoyu kaydet
                        File.Copy(yeniLogoYolu, hedefYol);

                        // Resim yolunu ayarlara kaydet
                        Properties.Settings.Default.LogoYolu = hedefYol;
                        Properties.Settings.Default.Save();

                        MessageBox.Show("Logo başarıyla kaydedildi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void button12_Click(object sender, EventArgs e)
        {
            BolumEkle frm = new BolumEkle();
            frm.Show();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            KullanıcıEkle frm = new KullanıcıEkle();
            frm.Show();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            TatilTablosu frm = new TatilTablosu();
            frm.Show();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

       
    }
}
