using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace WeatherLogiciel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string city = "Paris";
        private string baseUri = "https://goweather.herokuapp.com/weather/";
        private string? jsonString;
        private string newsApi = "https://newsapi.org/v2/everything?pageSize=9&apiKey=d37b9a4c96de41219757d1e207360443&language=fr&q=";
        private string[] newsUrl = new string[9];
        public MainWindow()
        {
            InitializeComponent();
            //recupère la date du jour 
            SetDay();
            GetApiResponse();
            LoadNews();
        }

        private async void GetApiResponse()
        {
            try
            {
                using(HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(baseUri + city);
                    jsonString = await response.Content.ReadAsStringAsync();
                    SetUiInfos();
                }

            }catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private void SetUiInfos()
        {
            JObject o = JObject.Parse(jsonString!);
            weatherDesc.Content = o["description"];
            windTxt.Content = o["wind"];
            tempTxt.Content = o["temperature"];
            forecastTemp1.Content = o["forecast"][0]["temperature"];
            forecastTemp2.Content = o["forecast"][1]["temperature"];
            forecastTemp3.Content = o["forecast"][2]["temperature"];
            CheckWeather(o);

        }

        private void CheckWeather(JObject o)
        {
            if (o["description"].ToString().ToLower().Contains("rain"))
                SetImageHeader("rain.jpg");
            else
                SetImageHeader("sunset.jpg");

        }

        private void SetImageHeader(string str)
        {
            imgHeader.Source = new BitmapImage(new Uri($@"/{str}", UriKind.Relative));
        }

        private void SetDay()
        {
            string formatedDate = DateTime.Now.ToString("dddd dd MMMM yyyy");
            string formatedDateUpper = CapitalizeStr(formatedDate);
            dateTxt.Content = formatedDateUpper;

            forecast1.Content = CapitalizeStr(DateTime.Now.AddDays(1).ToString("ddd"));
            forecast2.Content = CapitalizeStr(DateTime.Now.AddDays(2).ToString("ddd"));
            forecast3.Content = CapitalizeStr(DateTime.Now.AddDays(3).ToString("ddd"));
        }
        private string CapitalizeStr(string str)
        {
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (cityTxt.Text != string.Empty )
            {
                city = cityTxt.Text;
                cityTitle.Content = city;
                GetApiResponse();
            }

        }

        private void btnInfos_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Développé par : BENOUN Railas", "Credits", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private async void LoadNews()
        {
            List<Article> list = new List<Article>() { };
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(newsApi + city);
                    jsonString = await response.Content.ReadAsStringAsync();
                }
                JObject o = JObject.Parse(jsonString!);
                for(int i=0; i < o["articles"].Count(); i++)
                {
                    list.Add( new Article() { Title = o["articles"][i]["title"].ToString() });
                    newsUrl[i] = o["articles"][i]["url"].ToString();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex); ;
            }

            newsList.ItemsSource = list;
        }

        private void newsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = newsList.SelectedIndex;
            Process.Start(new ProcessStartInfo(newsUrl[index])
            {
                UseShellExecute = true
            }) ;
        }
    }
    class Article
    {
        public string? Title { get; set; }

        public override string ToString()
        {
            return "- " + Title!;
        }
    }
}
