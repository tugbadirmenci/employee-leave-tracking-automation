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
using System.Net.Http;
using Newtonsoft.Json;

namespace izinTakip
{
    public partial class TatilTablosu : Form
    {
        public TatilTablosu()
        {
            InitializeComponent();
        }
        BaglantiSinifi bgl = new BaglantiSinifi();

        public class Holiday
        {
            public string Name { get; set; }
            public DateTime Start { get; set; } // Google Calendar API'si için doğru alan ismi
        }

        // Form yüklendiğinde verileri göster
        private  async void TatilTablosu_Load(object sender, EventArgs e)
        {
            await FetchAndSaveHolidaysFromAPI();
            kayitlarigetir();
        }

        public void BindHolidaysToDataGridView(DataTable dt)
        {
            dataGridView1.DataSource = dt;
        }

        // API'den tatil bilgilerini al ve SQL Server'a kaydet
        public async Task FetchAndSaveHolidaysFromAPI()
        {
            string apiUrl = "https://www.googleapis.com/calendar/v3/calendars/turkish__tr%40holiday.calendar.google.com/events?key=AIzaSyCpPZ-urRzSDUGaDHV8mNkSM3LosY459z8";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();

                    // Google Calendar API'sinden dönen veriyi çözümle
                    dynamic jsonResponse = JsonConvert.DeserializeObject(jsonData);

                    foreach (var item in jsonResponse.items)
                    {
                        string holidayName = item.summary; // Tatilin adı
                        DateTime holidayDate = item.start.date; // Tatilin tarihi

                        // Veritabanına ekle
                        AddHolidayToDatabase(holidayName, holidayDate);
                    }
                }
                else
                {
                    Console.WriteLine("API'den veri alınamadı.");
                }
            }
        }

        // Veritabanına veri ekleme işlemi (SQL Server)
        public void AddHolidayToDatabase(string holidayName, DateTime holidayDate)
        {
        
            using (SqlConnection conn = new SqlConnection(bgl.Adres))
            {
                conn.Open();
                string query = "INSERT INTO TATILLER (TATIL_ADI, TATIL_TARIHI) VALUES (@name, @date)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", holidayName);
                    cmd.Parameters.AddWithValue("@date", holidayDate);
                    cmd.ExecuteNonQuery();
                }
            }
        } 

        // Tatil verilerini veritabanından çekip DataGridView'e bağlama
        public void kayitlarigetir()
        {
            using (SqlConnection connection = new SqlConnection(bgl.Adres))
            {
                connection.Open();
                SqlDataAdapter ad = new SqlDataAdapter("Select * From TATILLER Order By id", connection);
                DataTable dt = new DataTable();
                ad.Fill(dt);
                dataGridView1.DataSource = dt;

                //BindHolidaysToDataGridView(dt); // Verileri DataGridView'e bağla
                connection.Close();
            }
        }

    }
}
