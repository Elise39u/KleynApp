using KleynGroup.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KleynGroup.Data
{
    public class RestServiceLogin
    {
        readonly HttpClient _client;
        private string grant_type = "password";

        public RestServiceLogin()
        {
            _client = new HttpClient { MaxResponseContentBufferSize = 256000 };
            _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        }

        public async Task<Token> Login(User user)
        {
            var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", grant_type),
                new KeyValuePair<string, string>("username", user.Username),
                new KeyValuePair<string, string>("password", user.Password)
            };
            var content = new FormUrlEncodedContent(postData);
            var response = await PostResponseLogin<Token>(Constants.URL_LOGIN, content);
            var dt = DateTime.Today;
            Console.WriteLine("Passed Time");
            Console.WriteLine(response);
            response.ExpireDate = dt.AddSeconds(response.ExpireIn);
            Console.WriteLine("Passed Addning time");
            return response;
        }

        public async Task<T> PostResponseLogin<T>(string weburl, FormUrlEncodedContent content) where T : class
        {
            Console.WriteLine("Post time");
            var response = await _client.PostAsync(Constants.URL_LOGIN, content);
            Console.WriteLine("Posted Client");
            var jsonResult = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(jsonResult);
            var responseObject = JsonConvert.DeserializeObject<T>(jsonResult);
            Console.WriteLine("Passed Tesst");
            return responseObject;
        }

        public async Task<T> PostResponse<T>(string weburl, string jsonstring) where T : class
        {
            var token = App.TokenDatabase.GetToken();
            string contentType = "application/json";
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.remember_token);
            try
            {
                var result = await _client.PostAsync(Constants.URL_LOGIN, new StringContent(jsonstring, Encoding.UTF8, contentType));
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonresult = result.Content.ReadAsStringAsync().Result;
                    try
                    {
                        var contentResp = JsonConvert.DeserializeObject<T>(jsonresult);
                        return contentResp;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        public async Task<T> GetResponse<T>(string weburl) where T : class
        {
            var token = App.TokenDatabase.GetToken();
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.remember_token);
            try
            {
                var response = await _client.GetAsync(weburl);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var jsonresult = response.Content.ReadAsStringAsync().Result;
                    Debug.WriteLine("JsonResult:" + jsonresult);
                    try
                    {
                        var contentResp = JsonConvert.DeserializeObject<T>(jsonresult);
                        return contentResp;
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

    }
}
