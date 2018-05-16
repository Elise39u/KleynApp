using KleynGroup.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KleynGroup.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TestPage2 : ContentPage
	{
        public List<API_Items> Items;
        public TestPage2 ()
		{
			InitializeComponent();
            LoadData();
		}

        public void LoadData()
        {
            const string WEBSERVICE_URL = "https://web.kleyn.com:1919/locatiesdev/item/252488";
            try
            {
                var webRequest = WebRequest.Create(WEBSERVICE_URL);
                if (webRequest != null)
                {
                    webRequest.Method = "GET";
                    webRequest.Timeout = 12000;
                    webRequest.ContentType = "application/json";
                    webRequest.Headers.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");

                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            var jsonResponse = sr.ReadToEnd();
                            var Data = JsonConvert.DeserializeObject<API_Items>(jsonResponse);

                            MyListView.ItemsSource = new string[] {
                                 Data.itemnummer,
                                 Data.bussinesunit,
                                 Data.locatie,
                                 Data.voorraadstatus
                            };
                            Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                            Console.WriteLine(jsonResponse);
                            Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            }
        }

        public void Init()
        {
           
        }
    }
}