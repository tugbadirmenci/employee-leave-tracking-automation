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
    public partial class BolumEkle : Form
    {
        public BolumEkle()
        {
            InitializeComponent();
        }
        // string connectionString = "Server=LAPTOP-JK4K9LES\\SQLEXPRESS;Database=veritabani_inot;Integrated Security=True;";
        BaglantiSinifi bgl = new BaglantiSinifi();
        private void button1_Click(object sender, EventArgs e)
        {
            string BOLUMLER = textBox1.Text.Trim();

            if (string.IsNullOrEmpty(BOLUMLER))
            {
                MessageBox.Show("Lütfen bölüm adı giriniz.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // string connectionString = "Server=LAPTOP-JK4K9LES\\SQLEXPRESS;Database=veritabani_inot;Integrated Security=True;";
            

            string query = "INSERT INTO BOLUMLER (BOLUMLER) VALUES (@Bolumler)";

            using (SqlConnection connection = new SqlConnection(bgl.Adres))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Bolumler", BOLUMLER);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Bölüm başarıyla kaydedildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
    }
}
