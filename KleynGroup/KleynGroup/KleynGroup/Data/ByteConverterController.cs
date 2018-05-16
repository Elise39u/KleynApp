using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.Net.Http.Headers;

namespace KleynGroup.Data
{
    public class ByteConverterController
    {
        public object Converter(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;
            try
            {
                var byteArray = Convert.ToByte(value);
                Console.WriteLine("  The " + value + " is converted to " + byteArray);
                return byteArray;
            }
            catch (FormatException) {
                Console.WriteLine("The value  is not in the correct format for a base Byte value. ", value);
            }
            catch (OverflowException)
            {
                Console.WriteLine("The value is outside the range of the byte type. ", value);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("The value is invalid. ", value);
            }
            return null;
            /*
            var imgsrc = ImageSource.FromStream(() => {
                var ms = new MemoryStream(bArray);
                ms.Position = 0;
                return ms;
            });

            return imgsrc;
            */
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        /*
        public byte ConvertToImage(byte response)
        {
        }

        public void ConvertToByte()
        {

        }
        */

        public async System.Threading.Tasks.Task<Byte[]> DownloadImage(string itemnummer, string typecode)
        {
            byte[] downloadedImage;
            string imageUrl = "https://web.kleyn.com:1919/locatiesdev/picture/" + itemnummer + "/" + typecode + "";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");
                using (var httpResponse = await client.GetAsync(imageUrl))
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        downloadedImage = await httpResponse.Content.ReadAsByteArrayAsync();
                        return downloadedImage;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async System.Threading.Tasks.Task<string> DeleteImage(string itemnummer, string typecode)
        {
            string imageUrl = "https://web.kleyn.com:1919/locatiesdev/picture/" + itemnummer + "/" + typecode + "";
            try
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");
                using (var httpResponse = await client.DeleteAsync(imageUrl))
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        return "Image Deleted";
                    } else if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
                    {
                        return "Image can`t be deleted";
                    } else
                    {
                        return "Item not found";
                    }
                }
            }
            catch (Exception e)
            {
                return "Exception Caught: " + e.ToString();
            }
        }

        public async System.Threading.Tasks.Task ChangeUploadImage(string typeCode, string itemnumber, string method, MediaFile mediafile)
        {
            byte[] byteArray;
            using (var memoryStream = new MemoryStream())
            {
                mediafile.GetStream().CopyTo(memoryStream);
                mediafile.Dispose();
                byteArray = memoryStream.ToArray();
            }

            var URL = "https://web.kleyn.com:1919/locatiesdev/picture/" + itemnumber + "/" + typeCode;
            try
            {
                var client = new HttpClient();
                ByteArrayContent baContent = new ByteArrayContent(byteArray);

                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");
                if (method == "POST" || method == "post")
                {
                    var result = client.PostAsync(URL, baContent).Result;
                }
                else
                {
                    var result = client.PutAsync(URL, baContent).Result;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Caught: " + e.ToString());
            }
        }
    }
}
