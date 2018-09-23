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
        public async System.Threading.Tasks.Task<Byte[]> DownloadImage(string itemnummer, string typecode)
        {
            //Make a new byte Array 
            byte[] downloadedImage;

            //Create The download link with the var itemnummer and var typecode
            string imageUrl = "https://web.kleyn.com:1919/locatiesdev/picture/" + itemnummer + "/" + typecode + "";

            //Start The download progress
            try
            {
                //Start a httpclient request
                HttpClient client = new HttpClient();

                // Add the api key as header
                client.DefaultRequestHeaders.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");

                //Start Downloading Images
                using (var httpResponse = await client.GetAsync(imageUrl))
                {
                    //Check if the response is equal to StatusCode 200
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        //If the Response is 200 Try to read the response as a byte Array
                        downloadedImage = await httpResponse.Content.ReadAsByteArrayAsync();
                        Console.WriteLine(downloadedImage);
                        //Return the byte array response
                        return downloadedImage;
                    }
                    //If Response is not 200 return null
                    else
                    {
                        return null;
                    }
                }
            }
            // If a error occured return null
            catch (Exception e)
            {
                return null;
            }
        }

        public async System.Threading.Tasks.Task<string> DeleteImage(string itemnummer, string typecode)
        {
            //Create a url with the parameters itemnummer and typecode
            string imageUrl = "https://web.kleyn.com:1919/locatiesdev/picture/" + itemnummer + "/" + typecode + "";
            try
            {
                //Create a new HttpClient
                HttpClient client = new HttpClient();
                //Add the api key to the request
                client.DefaultRequestHeaders.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");

                // Start A Delete Request
                using (var httpResponse = await client.DeleteAsync(imageUrl))
                {
                    // If Status Code is Ok go on
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        //Return Image Deleted To the user
                        return "Image Deleted";
                    }
                    //Due To the api some images can`t be edited
                    //So if response is Badrequest Return The next string
                    else if (httpResponse.StatusCode == HttpStatusCode.BadRequest)
                    {
                        //Return this string to the user
                        return "Image can`t be deleted or A Bad request to the api";
                    }
                    //Else Return Not Found to the user
                    else
                    {
                        return "Item not found";
                    }
                }
            }
            // If a error occured Thorw a Exception Caught with the message
            catch (Exception e)
            {
                return "Exception Caught: " + e.ToString();
            }
        }

        public async System.Threading.Tasks.Task ChangeUploadImage(string typeCode, string itemnumber, string method, MediaFile mediafile)
        {
            //Make a new ByteArray 
            byte[] byteArray;

            //Make a new memory Stream
            using (var memoryStream = new MemoryStream())
            {
                //Copy the source of the image to the memory Stream
                mediafile.GetStream().CopyTo(memoryStream);
                //Remove the image
                mediafile.Dispose();
                //Convert the memoryStream to the bytearray
                byteArray = memoryStream.ToArray();
            }

            //Set up the api end point
            var URL = "https://web.kleyn.com:1919/locatiesdev/picture/" + itemnumber + "/" + typeCode;
            try
            {
                //Make a new  Http Client
                var client = new HttpClient();
                // Tel the request we sending a byteArray with it
                ByteArrayContent baContent = new ByteArrayContent(byteArray);

                //Set the uri
                client.BaseAddress = new Uri(URL);
                //Add the api key
                client.DefaultRequestHeaders.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");
                //Check what the method var is 
                //If Method is post upload the image
                if (method == "POST" || method == "post")
                {
                    //Return the Result of the upload
                    var result = client.PostAsync(URL, baContent).Result;
                }
                //IF method is put change the image
                else
                {
                    //Return the result of the change upload
                    var result = client.PutAsync(URL, baContent).Result;
                }
            } 
            // If a error occured Thorw a Exception Caught with the message
            catch (Exception e)
            {
                Console.WriteLine("Exception Caught: " + e.ToString());
            }
        }
    }
}
