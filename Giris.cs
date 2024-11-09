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
using static izinTakip.personelEkle;

namespace izinTakip
{
    public partial class Giris : Form
    {
        
        public Giris()
        {
            InitializeComponent();
        }
        BaglantiSinifi bgl = new BaglantiSinifi();

        SqlConnection con;
        SqlDataReader dr;
        SqlCommand com;

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
         
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
          
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
         
        }
    
        private void textBox2_Leave(object sender, EventArgs e)
        {
          
        }



        private void button1_Click(object sender, EventArgs e)
        {
            
            // Eğer hiçbir checkbox seçilmediyse uyarı göster
            if (!checkBox1.Checked && !checkBox2.Checked)
            {
                MessageBox.Show("Lütfen bir rol seçiniz: Yetkili veya Kullanıcı.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Giriş işlemini iptal et
            }
           
            // Eğer Yetkili checkbox'ı seçiliyse
            if (checkBox1.Checked)
            {
                CurrentUser.Role = "Admin";  // Rolü 'Admin' olarak belirle
            }
            else if (checkBox2.Checked)
            {
                CurrentUser.Role = "User";   // Rolü 'User' olarak belirle
            }
            SqlConnection connection = new SqlConnection(bgl.Adres);
           

            string user = textBox1.Text;
            string password = textBox2.Text;
            string query = "";
          //  string connectionString = "Data Source=LAPTOP-JK4K9LES\\SQLEXPRESS;Initial Catalog=veritabani_inot;Integrated Security=True";

            con = new SqlConnection(bgl.Adres);
            com = new SqlCommand();
            con.Open();
            com.Connection = con;
           
            // Yetkili checkbox'ı seçildiyse KULLANİCİGİRİS tablosunu kontrol et
            if (checkBox1.Checked)
            {
                query = "SELECT * FROM KULLANİCİGİRİS WHERE kullanici_adi COLLATE SQL_Latin1_General_CP1_CS_AS = @user AND sifre COLLATE SQL_Latin1_General_CP1_CS_AS = @password";
            }
            // Kullanıcı checkbox'ı seçildiyse USERGIRIS tablosunu kontrol et
            else if (checkBox2.Checked)
            {
                query = "SELECT * FROM USERGIRIS WHERE KullaniciAdi COLLATE SQL_Latin1_General_CP1_CS_AS = @user AND Sifre COLLATE SQL_Latin1_General_CP1_CS_AS = @password";
            }


            // SQL sorgusunu parametreli şekilde çalıştırmak için
            com.CommandText = query;
            com.Parameters.AddWithValue("@user", user);
            com.Parameters.AddWithValue("@password", password);

            dr = com.ExecuteReader();

            // Giriş başarılıysa
            if (dr.Read())
            {
                Anasayfa frm = new Anasayfa();
                frm.Show();
                this.Hide();
           
            }
            // Giriş başarısızsa hata mesajı göster
            else
            {
                MessageBox.Show("Hatalı kullanıcı adı veya şifre.");
                textBox1.Clear();
                textBox2.Clear();
                textBox1.Focus();
            }

            con.Close();
        }

        private void label1_MouseClick(object sender, MouseEventArgs e)
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Yetkili seçildiyse kullanıcı checkbox'ını pasif yap
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;
                checkBox2.Enabled = false;
            }
            else
            {
                checkBox2.Enabled = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            // Kullanıcı seçildiyse yetkili checkbox'ını pasif yap
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;
                checkBox1.Enabled = false;
            }
            else
            {
                checkBox1.Enabled = true;
            }
        }
    }
}

