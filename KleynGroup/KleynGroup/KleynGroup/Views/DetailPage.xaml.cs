using KleynGroup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.IO;
using System.Net;
using Newtonsoft.Json;
//using Xamarin.Forms.Maps;
using Xamarin.Forms.GoogleMaps;
using Plugin.Geolocator;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Reflection;
using System.Threading;
using Xamarin.Auth;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using System.Net.Http;
using KleynGroup.Data;

namespace KleynGroup.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailPage : ContentPage
    {
        public static bool error404 { get; set; }
        public string Letter;
        public string ChannelID;
        UserCredential credential;

        public DetailPage(string itemnumber)
        {
            this.IsBusy = false;
            string ResultNumber = itemnumber;
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Init();
            LoadData(ResultNumber);
            var PictureList = CheckPictures(itemnumber).Result;
            if (PictureList.totalrecords > 0)
            {
                var labelList = PictureList.data;
                var FoundHLV = false;

                for (int i = 0; i < labelList.Count(); ++i) {
                    if (labelList[i].typecode == "HLV")
                    {
                        FoundHLV = true;
                        break;
                    }
                }

                if (FoundHLV == true)
                {
                    var Sources = getHLVImage(itemnumber, "HLV");
                }
                else
                {
                    Vehicle_Image.Source = "dummie.jpeg";
                }
                //HLV --> Orignal image
                ViewMoreImgButton.IsVisible = true;
            }
            else
            {
                Vehicle_Image.Source = "dummie.jpeg";
                btnCamera.IsVisible = true;
            }

            ViewMoreImgButton.Clicked += async (sender, args) =>
            {
                var ImagePage = new NavigationPage(new ImagesPage(PictureList.data, itemnumber));
                NavigationPage.SetHasNavigationBar(ImagePage, false);

                await Navigation.PushAsync(ImagePage);
            };

            btnCamera.Clicked += async (send, obj) =>
            {
                 var Uploadanswer = await DisplayAlert("Upload Choosed", "Do you want to pick a photo from the gallery or one from the camera", "Camera", "Gallery");
                 if (Uploadanswer)
                 {
                    BtnCamera_ClickedAsync(itemnumber, "TSE", "POST");
                 }
                 else
                 {
                      BtnPickPhoto_CickedAsync(itemnumber, "TSE", "POST");
                 }
            };
        }

        public async Task getHLVImage(string itemnumber, string Typecode)
        {
            ByteConverterController byteConverter = new ByteConverterController();
            var Source = await byteConverter.DownloadImage(itemnumber, "HLV");
            Vehicle_Image.Source = ImageSource.FromStream(() => new MemoryStream(Source));
        }

        public async void LoadData(string itenummer)
        {
            var ResultNumber = itenummer;
            string WEBSERVICE_URL = "https://web.kleyn.com:1919/locatiesdev/item/" + ResultNumber;
            error404 = false;
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
                            Vehicle_name_binding.Text = Data.itemnummer;
                            Status_Binding.Text = Data.voorraadstatus;
                            Location_Binding.Text = Data.locatie;
                            Department_Binding.Text = Data.bussinesunit;
                            foreach (var o in Data.data)
                            {
                                if (o.label == "Merk")
                                {
                                    Vehicle_name.Text = o.waarde;

                                }
                                else if(o.label == "Type")
                                {
                                    Vehicle_name.Text = Vehicle_name.Text + "  " +  o.waarde;
                                }
                            }
                            //Data list styling
                            MyListView.HasUnevenRows = true;
                            MyListView.ItemsSource = Data.data;
                            MyListView.HeightRequest = 22 * Data.data.Count;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                var ResponseWeb = ex.Response as HttpWebResponse;

                if (ResponseWeb.StatusCode == HttpStatusCode.NotFound)
                {
                    var StatusCode = ResponseWeb.StatusCode;
                    var Errorpage = new NavigationPage(new ErrorPage());
                    NavigationPage.SetHasNavigationBar(Errorpage, false);
                    Errorpage.BindingContext = StatusCode;

                    await Navigation.PushAsync(Errorpage);
                }
            }
        }

        void Init()
        {
            //Hide take a video button for youtube
            takeVideo.IsVisible = false;

            //Hide the pictures button first
            ViewMoreImgButton.IsVisible = false;
            btnCamera.IsVisible = false;

            ViewMoreImgButton.BackgroundColor = Constants.KleynGroupTXT;
            ViewMoreImgButton.TextColor = Constants.LoginEntryBgColor;

            //header styling
            Header.BackgroundColor = Constants.KleynGroupBG;
            Logo.HeightRequest = 40;
            Image_Edit.HeightRequest = 30;
            Image_Edit.HorizontalOptions = LayoutOptions.EndAndExpand;

            //Content styling
            Vehicle_name.FontSize = 20;
            Vehicle_name.TextColor = Color.Black;

            SlaveContentDetails.BackgroundColor = Color.FromRgb(206, 206, 206);


            Seller_Text.TextColor = Color.Black;
            Seller_Binding.TextColor = Color.Black;

            Price_Text.TextColor = Color.Black;
            Price_Binding.TextColor = Color.Black;

            Status_Text.TextColor = Color.Black;
            Status_Binding.TextColor = Color.Red;
            Status_Binding.FontAttributes = FontAttributes.Bold;

            Mileage_Text.TextColor = Color.Black;
            Mileage_Binding.TextColor = Color.Black;

            BuiltDate_Text.TextColor = Color.Black;
            BuiltDate_Binding.TextColor = Color.Black;

            SlaveContentLocation.BackgroundColor = Color.FromRgb(206, 206, 206);
            h1_Detail.FontSize = 17;
            h1_Video.FontSize = 17;
            h1_Location.FontSize = 17;

            //footer styling
            Footer.BackgroundColor = Constants.KleynGroupBG;
            Lbl_CR.TextColor = Constants.MainTextColor;
            Lbl_CR.HorizontalOptions = LayoutOptions.Center;
            Lbl_CR.VerticalOptions = LayoutOptions.EndAndExpand;
            Lbl_CR.VerticalTextAlignment = TextAlignment.Center;
            Lbl_CR.HeightRequest = 80;

            // Vechile name styling
            Vehicle_name.FontSize = 16;

            //Location button
            Location_Open.BackgroundColor = Constants.KleynGroupTXT;
            Location_Open.TextColor = Constants.LoginEntryBgColor;

            //Youtube button
            Youtube_Open.BackgroundColor = Constants.KleynGroupTXT;
            Youtube_Open.TextColor = Constants.LoginEntryBgColor;

            //Activty Spinner
            Spinner.Color = Constants.KleynGroupTXT;
        }

        //(/◔ ◡ ◔)/ new But still a W.I.P for run function
        private async void BtnVideo_ClickedAsync(object sender, EventArgs e)
        {
            // Set up the libarys
            await CrossMedia.Current.Initialize();

            //Check if the device can use the Camera or a video maker is supported
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
            {
                // True send a error message back with no camera avaialble
                await DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                return;
            }

            //False go on with the code and wait for the user to take a picture
            var file = await CrossMedia.Current.TakeVideoAsync(new Plugin.Media.Abstractions.StoreVideoOptions
            {
                /*Save the video for now in the app directory VB: (/storage/emulated/0/Android/data/App.packagename -> com.companyname.KleynGroup/
                 files/Movies/DeafultVideos/filename
                  */
                Name = "video.mp4",
                Directory = "DefaultVideos",
            });

            // If user stops the camera or something went wrong return the user back to the detailpage
            if (file == null)
                return;

            //Ask the user to upload it to youtube
            var answer = await DisplayAlert("Youtube upload", "Do you want to upload the video to youtube?", "Yes", "No");
            if (answer)
            {
                await UploadYoutube(file.Path);
            }
            else
            {
                //Else await a message with the video location
                await DisplayAlert("Video Recorded", "Location: " + file.Path, "OK");
                return;
            }

            // Dispoe the video to the save location
            file.Dispose();
        }

        //(/◔ ◡ ◔)/
        public async Task UploadYoutube(string filepath)
        {
            //Get youtube channels 
            var YTchannels = getYoutubeID(Vehicle_name_binding.Text).Result;
            const string YoutubeAPIKey = "AIzaSyAqWV2Y1lTYOctfuFwTpLfMbH-Et_1K9lA";
            if (YTchannels == null)
            {
                await DisplayAlert("No channels", "This item has no youtube channels", "Okay");
            } else
            {
                var length = YTchannels.Data.Count();
                if(length <= 1)
                {
                    var ChannelID = YTchannels.Data[0].Id;
                } else
                { 
                    for(int i = 0; i < length; i ++)
                    {
                        var ChannelID = await DisplayActionSheet("Select Channel", YTchannels.Data[i].Id, "okay");
                    }
                }
                Console.WriteLine("YouTube Data API: Upload Video");
                Console.WriteLine("==============================");

                try
                {
                    Run(filepath, ChannelID).Wait();
                }
                catch (AggregateException ex)
                {
                    foreach (var e in ex.InnerExceptions)
                    {
                        Console.WriteLine("Error: " + e.Message);
                    }
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();

            }
        }

        // Is new but a W.I.P
        private async Task Run(string file_path, string ChannelID)
        {
            /*
            //This is not compatible with the google oauth2
            var credentialXam = new OAuth2Authenticator(
               clientId: "100841282957-o3skjv647hgm19rm5d0s8pmid2983dpg.apps.googleusercontent.com",
               clientSecret: "",
               scope: "openid",
               authorizeUrl: new Uri("https://accounts.google.com/o/oauth2/auth"),
               redirectUrl: new Uri("myredirect:oob"),
               accessTokenUrl: new Uri("https://accounts.google.com/o/oauth2/token"),
               getUsernameAsync: null
           );
            Console.WriteLine("######################################### Before");
            Console.WriteLine(credentialXam.AuthorizeUrl.PathAndQuery); // --> /o/oauth2/auth
            Console.WriteLine(credentialXam.IsAuthenticated); // -> System.Func`1[System.Boolean]
            Console.WriteLine(credentialXam.AccessTokenUrl.Host); // -> accounts.google.com
            Console.WriteLine(credentialXam.AccessTokenUrl.HostNameType); // -> Dns
            Console.WriteLine(credentialXam.AccessTokenUrl.PathAndQuery); // -> /o/oauth2/token
            Console.WriteLine(credentialXam.AccessTokenName); // -> access_token
            Console.WriteLine("#########################################");
            */
            /*
            var stream = new FileStream(@"client_secret.json", FileMode.Open, FileAccess.Read);
            GoogleClientSecrets cs = GoogleClientSecrets.Load(stream);
            */

            ClientSecrets secrets = new ClientSecrets
            {
                ClientId = "100841282957-328kfjqs5v119subdcbtk22g3seejg5p.apps.googleusercontent.com", // Putting someting random causes 403 unauthorized / invalid client
                ClientSecret = "lwkOpzX1vIg5mJpc2FZKrDpX", // Depends on client account of there is a Client Secret needed
            };

            GoogleAuthorizationCodeFlow.Initializer initializer = new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = secrets,
                Scopes = new[] { YouTubeService.Scope.Youtube,  // view and manage your YouTube account
                                             YouTubeService.Scope.YoutubeForceSsl,
                                             YouTubeService.Scope.Youtubepartner,
                                             YouTubeService.Scope.YoutubepartnerChannelAudit,
                                             YouTubeService.Scope.YoutubeReadonly,
                                             YouTubeService.Scope.YoutubeUpload}
            };
            //initializer.DataStore = new FileDataStore("Youtube.Auth.Store");

            var flow = new GoogleAuthorizationCodeFlow(initializer);

            Console.WriteLine("##################################");
            Console.WriteLine(initializer.AuthorizationServerUrl); // --> https://accounts.google.com/o/oauth2/auth
            Console.WriteLine(initializer.TokenServerUrl); // --> http://www.googleapis.com/oauth2/v4/token
            Console.WriteLine(initializer.ClientSecretsStream); // -> Null or empty
            Console.WriteLine(initializer.Clock); //-> Google.Apis.Util.SystemClock
            Console.WriteLine(initializer.Clock.UtcNow); // UTC+1 - 2 hours Example: utc  = 12:55:34  system = 10:55:34
            Console.WriteLine(initializer.AccessMethod);// --> Google.Apis.Auth.OAuth2.BearerToken+AuthorizationHeaderAccessMethod
            Console.WriteLine(initializer.DataStore); // --> Google.Apis.Util.Store.FileDataStore
            Console.WriteLine("##################################");

            // RefreshToken = "https://accounts.google.com/o/oauth2/token" --> Refresh token is invaild, incorrect, incompleet
            var token = new TokenResponse
            {
                RefreshToken = initializer.TokenServerUrl,
                TokenType = "Bearer",
                ExpiresInSeconds = 18000,
                IssuedUtc = DateTime.UtcNow.ToLocalTime(),
            };

            // Outcomment this and you get a missing login header error: 401 with UserCredential credentail as global var
            credential = new UserCredential(flow, "Kleynpark", token); // Is correct authorized?

            Console.WriteLine("##################################");
            Console.WriteLine(token.RefreshToken); // --> Empty or null
            Console.WriteLine(flow.DataStore); // --> http://www.googleapis.com/oauth2/v4/token
            Console.WriteLine("##################################");

            //Causes 401 missing Login Authorized headers SOLVED
            /*
            var credential = new UserCredential(new GoogleAuthorizationCodeFlow(
                 new GoogleAuthorizationCodeFlow.Initializer
                 {
                     ClientSecrets = secrets,
                     Scopes = new[] { YouTubeService.Scope.YoutubeUpload },
                 }), "Kleyn Park", token);
                 */
            /* WebAuthorization wont work with Xamarin
             * credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                secrets,
                new string[]
                {
                        YouTubeService.Scope.YoutubeUpload
                },
                "user",
                CancellationToken.None);
                */
            /*
             * FileStream cant find anything even function
            using (var stream = (new FileStream(secrets.ToString(), mode: FileMode.Open, access: FileAccess.Read)))
            {
              GoogleWebAuthorizationBroker.Folder = "Tasks.Auth.Store";
              credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
              GoogleClientSecrets.Load(stream).Secrets,
              new[] { YouTubeService.Scope.Youtube, YouTubeService.Scope.YoutubeUpload },
              "user",
              CancellationToken.None,
              new FileDataStore("YouTube.Auth.Store")).Result;
            } 
            */

            // Put this above credential and you get a 401 missing login heaader
            YouTubeService youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApiKey = "AIzaSyAqWV2Y1lTYOctfuFwTpLfMbH-Et_1K9lA",
                ApplicationName = Assembly.GetExecutingAssembly().GetName().Name
            });

            var video = new Video();
            video.Snippet = new VideoSnippet();
            video.Snippet.Title = "Default Video Title";
            video.Snippet.Description = "Default Video Description";
            video.Snippet.Tags = new string[] { "truck", "van" };
            video.Snippet.CategoryId = "1"; // See https://developers.google.com/youtube/v3/docs/videoCategories/list
            video.Status = new VideoStatus();
            video.Status.PrivacyStatus = "public"; // "unlisted" or "private" or "public"
            var filePath = file_path; // Replace with path to actual movie file.

            using (var fileStream = new FileStream(filePath, FileMode.Open))
            {
                var videosInsertRequest = youtubeService.Videos.Insert(video, "snippet,status", fileStream, "video/*");
                videosInsertRequest.ProgressChanged += videosInsertRequest_ProgressChanged;
                videosInsertRequest.ResponseReceived += videosInsertRequest_ResponseReceived;

                await videosInsertRequest.UploadAsync();
            }
        }

        void videosInsertRequest_ProgressChanged(Google.Apis.Upload.IUploadProgress progress)
        {
            switch (progress.Status)
            {
                case UploadStatus.Uploading:
                    Console.WriteLine("{0} bytes sent.", progress.BytesSent);
                    break;

                case UploadStatus.Failed:
                    Console.WriteLine("An error prevented the upload from completing.\n{0}", progress.Exception);
                    break;
            }
        }

        void videosInsertRequest_ResponseReceived(Video video)
        {
            Console.WriteLine("Video id '{0}' was successfully uploaded.", video.Id);
        }


        private async void BtnCamera_ClickedAsync(string itemnumber, string typeCode, string method)
        {
            var byteConverter = new ByteConverterController();
            // Set up the libarys
            await CrossMedia.Current.Initialize();

            //Check if the device can use the Camera or a photo maker is supported
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                // True send a error message back with no camera avaialble
                await DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                return;
            }

            //False go on with the code and wait for the user to take a picture
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                //Set the size to medium
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                /*Save the photo for now in the app directory VB: (/storage/emulated/0/Android/data/App.packagename -> com.companyname.KleynGroup/
                 files/pictures/sample/filename
                  */
                Directory = "Sample",
                Name = "test.jpg"
            });

            // If the user stops the camera or something goes wrong return nothing
            if (file == null)
                return;

            //Else await a message with the image location
            await DisplayAlert("File Location", file.Path, "OK");

            var answer = await DisplayAlert("Change Image?", "Are you sure you want to upload this image", "Yes", "cancel");
            if (answer)
            {
                var typeCodeChoice = await DisplayActionSheet("What kind of picture is this", "Abort", null, "Head picture Left front",
                        "Head picture Right Front", "Head picture left behind", "Head picture right behind", "Interior", "Dashboard",
                        "Specials", "container", "Combination", "Double cabie/cabin", "Radio/Cd player", "Books", "Keys", "Air conditioning",
                        "Under the Hood", "Entry front", "Entry behind", "Cool engine", "Km. Stand", "Chassisno", "Construction sign",
                        "Front of the vehicle", "Back of the vehicle", "Licence plate", "Test image");
                switch (typeCodeChoice)
                {
                    case "Head picture Left front":
                        typeCodeChoice = "HLV";
                        break;
                    case "Head picture Right Front":
                        typeCodeChoice = "HRV";
                        break;
                    case "Head picture left behind":
                        typeCodeChoice = "HLA";
                        break;
                    case "Head picture right behind":
                        typeCodeChoice = "HRA";
                        break;
                    case "Interior":
                        typeCodeChoice = "HIN";
                        break;
                    case "Dashboard":
                        typeCodeChoice = "HDA";
                        break;
                    case "Specials":
                        typeCodeChoice = "FSP";
                        break;
                    case "container":
                        typeCodeChoice = "FLB";
                        break;
                    case "Combination":
                        typeCodeChoice = "COM";
                        break;
                    case "Double cabie/cabin":
                        typeCodeChoice = "FDC";
                        break;
                    case "Radio/Cd player":
                        typeCodeChoice = "FRC";
                        break;
                    case "Books":
                        typeCodeChoice = "FBK";
                        break;
                    case "Keys":
                        typeCodeChoice = "FSL";
                        break;
                    case "Air conditioning":
                        typeCodeChoice = "AIR";
                        break;
                    case "Under the Hood":
                        typeCodeChoice = "MOT";
                        break;
                    case "Entry front":
                        typeCodeChoice = "FB1";
                        break;
                    case "Entry behind":
                        typeCodeChoice = "FB2";
                        break;
                    case "Cool engine":
                        typeCodeChoice = "FKL";
                        break;
                    case "Km. Stand":
                        typeCodeChoice = "FKM";
                        break;
                    case "Chassisno":
                        typeCodeChoice = "FCH";
                        break;
                    case "Construction sign":
                        typeCodeChoice = "CPL";
                        break;
                    case "Front of the vehicle":
                        typeCodeChoice = "FVO";
                        break;
                    case "Back of the vehicle":
                        typeCodeChoice = "FAC";
                        break;
                    case "Licence plate":
                        typeCodeChoice = "FAC";
                        break;
                    case "Test image":
                        typeCodeChoice = "TSE";
                        break;
                    default:
                        await DisplayAlert("Canceled", "Nothing has been chosen", "Abort");
                        return;
                }
                var DoCall = byteConverter.ChangeUploadImage(typeCodeChoice, itemnumber, method, file);
                var DashBoard = new NavigationPage(new DetailPage(itemnumber));
                NavigationPage.SetHasNavigationBar(DashBoard, false);

                await Navigation.PushAsync(DashBoard);
            }
            else
            {
                await DisplayAlert("Abort", "Cancel has been chosen", "abort");
                return;
            }
        }

        private async void BtnPickPhoto_CickedAsync(string itemnumber, string typeCode, string method)
        {
            var byteConverter = new ByteConverterController();
            // Set up the libarys
            await CrossMedia.Current.Initialize();

            //Check if the user can pick a photo 
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
               await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
               return;
            }

            //Wait to the user picked a photo and put in this var
            var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
               //Set the size to medium
               PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
            });

             //If file == null than return with nothing
             if (file == null)
                return;

            var answer = await DisplayAlert("Change Image?", "Are you sure you want to upload this image", "Yes", "cancel");
            if (answer)
            {
                var typeCodeChoice = await DisplayActionSheet("What kind of picture is this", "Abort", null, "Head picture Left front",
                        "Head picture Right Front", "Head picture left behind", "Head picture right behind", "Interior", "Dashboard",
                        "Specials", "container", "Combination", "Double cabie/cabin", "Radio/Cd player", "Books", "Keys", "Air conditioning",
                        "Under the Hood", "Entry front", "Entry behind", "Cool engine", "Km. Stand", "Chassisno", "Construction sign",
                        "Front of the vehicle", "Back of the vehicle", "Licence plate", "Test image");
                switch (typeCodeChoice)
                {
                    case "Head picture Left front":
                        typeCodeChoice = "HLV";
                        break;
                    case "Head picture Right Front":
                        typeCodeChoice = "HRV";
                        break;
                    case "Head picture left behind":
                        typeCodeChoice = "HLA";
                        break;
                    case "Head picture right behind":
                        typeCodeChoice = "HRA";
                        break;
                    case "Interior":
                        typeCodeChoice = "HIN";
                        break;
                    case "Dashboard":
                        typeCodeChoice = "HDA";
                        break;
                    case "Specials":
                        typeCodeChoice = "FSP";
                        break;
                    case "container":
                        typeCodeChoice = "FLB";
                        break;
                    case "Combination":
                        typeCodeChoice = "COM";
                        break;
                    case "Double cabie/cabin":
                        typeCodeChoice = "FDC";
                        break;
                    case "Radio/Cd player":
                        typeCodeChoice = "FRC";
                        break;
                    case "Books":
                        typeCodeChoice = "FBK";
                        break;
                    case "Keys":
                        typeCodeChoice = "FSL";
                        break;
                    case "Air conditioning":
                        typeCodeChoice = "AIR";
                        break;
                    case "Under the Hood":
                        typeCodeChoice = "MOT";
                        break;
                    case "Entry front":
                        typeCodeChoice = "FB1";
                        break;
                    case "Entry behind":
                        typeCodeChoice = "FB2";
                        break;
                    case "Cool engine":
                        typeCodeChoice = "FKL";
                        break;
                    case "Km. Stand":
                        typeCodeChoice = "FKM";
                        break;
                    case "Chassisno":
                        typeCodeChoice = "FCH";
                        break;
                    case "Construction sign":
                        typeCodeChoice = "CPL";
                        break;
                    case "Front of the vehicle":
                        typeCodeChoice = "FVO";
                        break;
                    case "Back of the vehicle":
                        typeCodeChoice = "FAC";
                        break;
                    case "Licence plate":
                        typeCodeChoice = "FAC";
                        break;
                    case "Test image":
                        typeCodeChoice = "TSE";
                        break;
                    default:
                        await DisplayAlert("Canceled", "Nothing has been chosen", "Abort");
                        return;
                }
                var DoCall = byteConverter.ChangeUploadImage(typeCodeChoice, itemnumber, method, file);
                var DashBoard = new NavigationPage(new DetailPage(itemnumber));
                NavigationPage.SetHasNavigationBar(DashBoard, false);

                await Navigation.PushAsync(DashBoard);
            }
            else
            {
                await DisplayAlert("Abort", "Cancel has been chosen", "abort");
                return;
            }
        }

        public async Task<Locatieitem> getPostion(string itemnummer)
        {
            var ResultNumber = itemnummer;
            string WEBSERVICE_URL = "https://web.kleyn.com:1919/locatiesdev/location/" + ResultNumber;
            try
            {
                var locRequest = WebRequest.Create(WEBSERVICE_URL);
                if (locRequest != null)
                {
                    locRequest.Method = "GET";
                    locRequest.Timeout = 12000;
                    locRequest.ContentType = "application/json";
                    locRequest.Headers.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");
                    using (System.IO.Stream s = locRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            var jsonResponse = sr.ReadToEnd();
                            var Data = JsonConvert.DeserializeObject<Locatieitem>(jsonResponse);
                            return Data;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                var ResponseWeb = ex.Response as HttpWebResponse;
                if (ResponseWeb.StatusCode == HttpStatusCode.NotFound)
                {
                    var StatusCode = ResponseWeb.StatusCode;
                    var Errorpage = new NavigationPage(new ErrorPage());
                    NavigationPage.SetHasNavigationBar(Errorpage, false);
                    Errorpage.BindingContext = StatusCode;

                    return null;
                }
            }
            return null;
        }


        public async void AddLocation_Clicked(object sender, EventArgs e)
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(100));
            var answer = await DisplayAlert("Add Location", "Do you wan't to add your current location to this vechile " 
                + " With Latitude: " + position.Latitude.ToString() + " And Longitude: " + position.Longitude.ToString(), "Yes", "No");
            if(answer)
            {
                await EditLocation(Vehicle_name_binding.Text, position.Latitude.ToString(), position.Longitude.ToString());
            } else
            {
                return;
            }
        }

        public async Task EditLocation(string itemnummer, string latitude, string longitude)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create("https://web.kleyn.com:1919/locatiesdev/location/" + itemnummer );
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");

            using (var streamWritter = new StreamWriter(webRequest.GetRequestStream()))
            {
                var action = await DisplayActionSheet("Select Your current location terrain", "Cancel", null, "Volteram France", "Dorderecht", "Trucks", "Vans",
                    "Xiffix", "Vredebest 1", "Vredebest 2");

                switch (action)
                {
                    case "Volteram France":
                        Letter = "\"V\"";
                        break;
                    case "Dorderecht":
                        Letter = "\"D\"";
                        break;
                    case "Trucks":
                        Letter = "\"Z\"";
                        break;
                    case "Vans":
                        Letter = "\"N\"";
                        break;
                    case "Xiffix":
                        Letter = "\"X\"";
                        break;
                    case "Vredebest 1":
                        Letter = "\"1\"";
                        break;
                    case "Vredebest 2":
                        Letter = "\"2\"";
                        break;
                    default:
                        await DisplayAlert("Grrrrr", "No location given, We need to have a location", "Abort");
                        return;
                }
                string Latitude_Less = latitude.Replace(',', '.');
                string Longitude_Less = longitude.Replace(',', '.');
                string date = JsonConvert.SerializeObject(DateTime.Now);
                string json = "{\"itemnummer\":" + itemnummer + ","
                    + "\"locatie\":" + Letter + "," +
                    "\"tijdstip\":" + date + ","
                    +"\"lengtegraad\":" + Longitude_Less + ","
                    + "\"breedtegraad\":" + Latitude_Less + ","
                    + "\"gebruiker\":\"Admin\"}";
                streamWritter.Write(json);
                streamWritter.Flush();
                streamWritter.Close();
            }
            
            var httpRepsone = (HttpWebResponse)webRequest.GetResponse();
            using (var streamReader = new StreamReader(httpRepsone.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                await Navigation.PushAsync(new DetailPage(Vehicle_name_binding.Text));
            }
        }
        
        private async void Location_Open_ClickedAsync(object sender, EventArgs e)
        {
            try
            {
                this.IsBusy = true;

                var Item = await getPostion(Vehicle_name_binding.Text);
                if (Item == null)
                {
                    var OldButton = Location_Open;
                    var NewButton = new Button
                    {
                        Text = "Add a location",
                        Margin = new Thickness(10, 5, 10, 5),
                        BackgroundColor = Constants.KleynGroupTXT,
                        TextColor = Constants.LoginEntryBgColor
                    };
                    NewButton.Clicked += new EventHandler(AddLocation_Clicked);
                    Location_Open.IsVisible = false;
                    SlaveContentLocation.Children.Add(NewButton);
                    await DisplayAlert("Error", "No Location Found =(", "Ok");
                    this.IsBusy = false;
                }
                else
                {
                    var locator = CrossGeolocator.Current;
                    locator.DesiredAccuracy = 0.01;
                    var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(0.01));
                    var PinPostion = new Position(Item.breedtegraad, Item.lengtegraad);
                    var pin = new Pin
                    {
                        Type = PinType.Place,
                        Position = PinPostion,
                        Label = Vehicle_name.Text,
                        Address = Vehicle_name_binding.Text
                    };
                    var stack = new StackLayout { Spacing = 0 };
                    var map = new Map();
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude),
                                              Distance.FromMiles(0.28)));
                    map.MyLocationEnabled = true;
                    map.UiSettings.MyLocationButtonEnabled = true;
                    map.UiSettings.CompassEnabled = true;
                    map.MyLocationButtonClicked += async (data, arags) =>
                    {
                            map.CameraChanged += async (info, args) =>
                            {
                                var locator2 = CrossGeolocator.Current;
                                locator.DesiredAccuracy = 0.01;
                                var position2 = await locator.GetPositionAsync(TimeSpan.FromSeconds(0.01));
                                map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position2.Latitude, position2.Longitude),
                                                  Distance.FromMiles(0.28)));
                            };
                    };
                    /*
                     * Create a custom layout for google maps
                     * Make the header
                    */
                    var header_maps = new StackLayout
                    {
                        BackgroundColor = Constants.KleynGroupBG,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Start
                    };
                    //Footer
                    var footer_maps = new StackLayout
                    {
                        BackgroundColor = Constants.KleynGroupBG,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.End
                    };
                    //Make the Logo
                    var Logo = new Image
                    {
                        Source = "LoginLogo.png",
                        Margin = new Thickness(5, 5, 0, 10),
                        HorizontalOptions = LayoutOptions.Start,
                        HeightRequest = 40
                    };
                    //Edit button
                    var EditLocation = new Button
                    {
                        Text = "Change Location",
                        Margin = new Thickness(0, -50, 5, 0),
                        BackgroundColor = Constants.KleynGroupTXT,
                        TextColor = Constants.LoginEntryBgColor,
                        HeightRequest = 40,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                    };
                    //Make the copyright label
                    var CopyRightLabel = new Label
                    {
                        Text = "Copyright © 2018, Kleyn Group",
                        Margin = new Thickness(95, 0, 0, 0),
                        TextColor = Constants.MainTextColor,
                        VerticalOptions = LayoutOptions.EndAndExpand,
                        VerticalTextAlignment = TextAlignment.Center,
                        HeightRequest = 75,
                    };
                    EditLocation.Clicked += new EventHandler(AddLocation_Clicked);

                    // Give the header the logo
                    header_maps.Children.Add(Logo);
                    header_maps.Children.Add(EditLocation);
                    // Give the footer the Copy Right Label
                    footer_maps.Children.Add(CopyRightLabel);

                    //Add the location pin to the map
                    map.Pins.Add(pin);

                    //Bind the footer/map/header togther 
                    stack.Children.Add(header_maps);
                    stack.Children.Add(map);
                    stack.Children.Add(footer_maps);
                    Content = stack;
                }
            }
                finally
                {
                    this.IsBusy = false;
                }
            }

        //(/◔ ◡ ◔)/
        public async Task<Youtube> getYoutubeID(string itemnummer)
        {
            var ResultNumber = itemnummer;
            string WEBSERVICE_URL = "https://web.kleyn.com:1919/locatiesdev/youtubeid/" + ResultNumber;
            try
            {
                var YTRequest = WebRequest.Create(WEBSERVICE_URL);
                if (YTRequest != null)
                {
                    YTRequest.Method = "GET";
                    YTRequest.Timeout = 12000;
                    YTRequest.ContentType = "application/json";
                    YTRequest.Headers.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");
                    using (System.IO.Stream s = YTRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            var jsonResponse = sr.ReadToEnd();
                            var Data = JsonConvert.DeserializeObject<Youtube>(jsonResponse);
                            var JsArray = Data.Data;
                            var length = JsArray.Count();
                            return Data;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                var ResponseWeb = ex.Response as HttpWebResponse;
                if (ResponseWeb.StatusCode == HttpStatusCode.NotFound)
                {
                    var StatusCode = ResponseWeb.StatusCode;
                    var Errorpage = new NavigationPage(new ErrorPage());
                    NavigationPage.SetHasNavigationBar(Errorpage, false);
                    Errorpage.BindingContext = StatusCode;

                    return null;
                }
            }
            return null;
        }

        //(/◔ ◡ ◔)/
        public async void AddYoutubeID(object sender, EventArgs e)
        {
            string itemnummer = Vehicle_name_binding.Text;
            var webRequest = (HttpWebRequest)WebRequest.Create("https://web.kleyn.com:1919/locatiesdev/youtubeid/" + itemnummer);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");

            using (var streamWritter = new StreamWriter(webRequest.GetRequestStream()))
            {
                string json = "{\"id\":\"UCdrcANsC3BAzbc7cvBzNMjQ\"}";
                streamWritter.Write(json);
                streamWritter.Flush();
                streamWritter.Close();
            }

            var httpRepsone = (HttpWebResponse)webRequest.GetResponse();
            using (var streamReader = new StreamReader(httpRepsone.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                await Navigation.PushAsync(new DetailPage(Vehicle_name_binding.Text));
            }
        }

        //(/◔ ◡ ◔)/
        private async void Check_Youtube_ChannelsAsync(object sender, EventArgs e)
        {
            var YTId = await getYoutubeID(Vehicle_name_binding.Text);
            if(YTId == null)
            {
                var NewButton = new Button
                {
                    Text = "Add a Youtube channel",
                    Margin = new Thickness(10, 5, 10, 5),
                    BackgroundColor = Constants.KleynGroupTXT,
                    TextColor = Constants.LoginEntryBgColor
                };
                NewButton.Clicked += new EventHandler(AddYoutubeID);
                Youtube_Open.IsVisible = false;
                SlaveContentVideo.Children.Add(NewButton);
                await DisplayAlert("Error", "No Youtube accounts found =(", "Ok");
            } else
            {
                Picker picker = new Picker
                {
                    Title = "Youtube channels",
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    Margin = new Thickness(10, 0)
                };

                Youtube_Open.IsVisible = false;
                takeVideo.IsVisible = true;
                var length = YTId.Data.Count();
                if (length <= 1)
                {
                    picker.Items.Add(YTId.Data[0].Id);
                }
                else
                { 
                    for(int i = 0; i < length; i++)
                    {
                        picker.Items.Add(YTId.Data[i].Id);
                    }
                }
                SlaveContentVideo.Children.Add(picker);
                int selectedIndex = picker.SelectedIndex;
                picker.SelectedIndexChanged += async (object agras, EventArgs ev) =>
                {
                    if (selectedIndex != -1)
                    {
                        await DisplayAlert("Well", "Nothing selected", "Okay");
                    }
                    else
                    {
                        await DisplayAlert("okay", picker.Items[picker.SelectedIndex], "Okay");
                    }
                };
            }
        }

        //(/◔ ◡ ◔)/
        public async Task<Records> CheckPictures(string itemnummer)
        {
            var ResultNumber = itemnummer;
            string WEBSERVICE_URL = "https://web.kleyn.com:1919/locatiesdev/picturelist/" + ResultNumber;
            try
            {
                var PictureListRequest = WebRequest.Create(WEBSERVICE_URL);
                if (PictureListRequest != null)
                {
                    PictureListRequest.Method = "GET";
                    PictureListRequest.Timeout = 12000;
                    PictureListRequest.ContentType = "application/json";
                    PictureListRequest.Headers.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");
                    using (System.IO.Stream s = PictureListRequest.GetResponse().GetResponseStream())
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            var jsonResponse = sr.ReadToEnd();
                            var ListInfo = JsonConvert.DeserializeObject<Records>(jsonResponse);
                            return ListInfo;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                var ResponseWeb = ex.Response as HttpWebResponse;
                if (ResponseWeb.StatusCode == HttpStatusCode.NotFound)
                {
                    var StatusCode = ResponseWeb.StatusCode;
                    var Errorpage = new NavigationPage(new ErrorPage());
                    NavigationPage.SetHasNavigationBar(Errorpage, false);
                    Errorpage.BindingContext = StatusCode;

                    return null;
                }
            }
            return null;
        }
    }
}