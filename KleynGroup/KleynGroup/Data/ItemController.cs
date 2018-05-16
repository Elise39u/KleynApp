using System;
using System.Collections.Generic;
using KleynGroup.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace KleynGroup.Data
{
    class ItemController
    {
        Item[] items = new Item[]
        {
            new Item { Id = 1, Naam = "Citorèn", Description = "Nice one", Itemnummer = "432123"},
            new Item { Id = 2, Naam = "Merceds", Description = "MRrrrr", Itemnummer = "413414"},
            new Item { Id = 3, Naam = "Fiat", Description = "FAT  FAT", Itemnummer = "123245"}
        };

        public static async Task<string> GetItems()
        {
            var content = "";
            HttpClient client = new HttpClient();
            var RestURL = "http://128.199.54.72/api.php/Vehicles";
            client.BaseAddress = new Uri(RestURL);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(RestURL);
            content = await response.Content.ReadAsStringAsync();
            var Items = JsonConvert.DeserializeObject<Item>(content);
            return Items.ToString();
        }

        public static async void GetAllItems(int id)
        {
            var content = "";
            HttpClient client = new HttpClient();
            var RestURL = "http://128.199.54.72/api.php/Vehicles";
            client.BaseAddress = new Uri(RestURL);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = await client.GetAsync(RestURL);
            content = await response.Content.ReadAsStringAsync();
            var Items = JsonConvert.DeserializeObject<Item>(content);
        }


        public HttpMethod GetItem(int id)
        {
            var item = items.FirstOrDefault((i) => i.Id == id);
            if (item == null)
            {
                return (null);
            }
            return (null);
        }
    }
}
