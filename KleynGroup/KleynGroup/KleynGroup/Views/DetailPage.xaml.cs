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
using Google.Apis.Auth.OAuth2.Web;
using System.Net.Http;
using KleynGroup.Data;
using System.Collections;
using Android.Locations;
using Android.Content;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Android.Content.Res;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Labs;
using Xamarin.Forms.Labs.Services;

namespace KleynGroup.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailPage : ContentPage
    {
        public class AuthenticationState
        {
            public static OAuth2Authenticator Authenticator;
        }

        //Make some public vars to use later
        public static bool error404 { get; set; }
        public string Letter;
        public string ChannelID;
        //public UserDatabaseController UserData;
        public string FoundType { get; set; }
        public TaskStatus cancellationToken { get; set; }
        private CancellationTokenSource _cts;

        public DetailPage(string itemnumber)
        {
            UserDatabaseController UserData = new UserDatabaseController();
            // Set the itemnumber to a new string
            var UserInfo = UserData.GetAllUsers();
            string ResultNumber = itemnumber;
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Init();
            //Load the Vehicle Data from the api
            LoadData(ResultNumber);
            // Get the Typecodes from the api
            var PictureList = CheckPictures(itemnumber).Result;
            // Check if the list contains any typecodes
            if (PictureList.totalrecords > 0)
            {
                // If there are set the data list into a var
                var labelList = PictureList.data;
                //Make a flag to search for the FB1 or HLV typecode
                var FoundHLV = false;

                // for each typecode execute the for loop
                for (int i = 0; i < labelList.Count(); ++i) {
                    // If the current typecode is HLV or FB1 do the if 
                    if (labelList[i].typecode == "HLV" || labelList[i].typecode == "FB1")
                    {
                        // Set FoundType to either HLV or FB1 type code
                        FoundType = labelList[i].typecode;
                        //Set FoundHLV to true and stop the for loop
                        FoundHLV = true;
                        break;
                    }
                }
                // IF FoundHLV == true
                if (FoundHLV == true)
                {
                    //Check if FoundType is HLV to make it hlv
                    if (FoundType == "HLV")
                    {
                        FoundType = "hlv";
                    }
                    //Else make it fb1
                    else
                    {
                        FoundType = "fb1";
                    }
                    //get the imgae from the amazon server
                    Vehicle_Image.Source = "https://s3-eu-west-1.amazonaws.com/kleyn-api/vehicle/" +
                                                  ResultNumber + "/" + FoundType + "/600.jpg";
                }
                //IF either HLV or FB1 has not been found set a dummie image
                else
                {
                    Vehicle_Image.Source = "dummie.jpeg";
                }
                // Show the View more Image Button
                ViewMoreImgButton.IsVisible = true;
            }
            // Else IF there are no typecodes are found
            else
            {
                //Set a dummie Image and Show the take image button
                Vehicle_Image.Source = "dummie.jpeg";
                if (UserInfo[0].picturePermisson == 1)
                {
                    btnCamera.IsVisible = true;
                } else
                {
                    Picturepermission.Text = "No permisson to add images";
                    Picturepermission.TextColor = Color.DimGray;
                }
            }

            //Keep checking if the user pressed the view more image button
            ViewMoreImgButton.Clicked += async (sender, args) =>
            {
                //If pressed send the user to the image page
                var ImagePage = new ImagesPage(PictureList.data, itemnumber);
                NavigationPage.SetHasNavigationBar(ImagePage, false);
                await Navigation.PushAsync(ImagePage);
            };
            // Check if the user pressed take a image button
            btnCamera.Clicked += async (send, obj) =>
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (status != PermissionStatus.Unknown)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Camera))
                    {
                        status = results[Permission.Camera];
                    }

                    if (status == PermissionStatus.Granted)
                    {
                    }
                    else if (status != PermissionStatus.Unknown)
                    {
                        await DisplayAlert("Camera Denied", "Can not continue, try again.", "OK");
                        return;
                    }
                }
                // Ask the user if they want to take one from the camera or the gallery
                var Uploadanswer = await DisplayActionSheet("Do you want to pick a photo from the gallery or one from the camera", "Abort", null, "Camera", "Gallery");
                // If the user pressed Camera open the camera
                switch (Uploadanswer) {
                    case "Camera":
                        //Send the user to the camera and go on to the next function
                        BtnCamera_ClickedAsync(itemnumber, "TSE", "POST");
                        break;
                    case "Gallery":
                        //Send the user to his/her Gallery and go on to the next function
                        BtnPickPhoto_CickedAsync(itemnumber, "TSE", "POST");
                        break;
                    default:
                        await DisplayAlert("Action Canceld", "Action has been aborted", "Okay");
                        break;
                }
            };
            
            Backbutton.Clicked += async (sender, args) =>
            {
                await Navigation.PopToRootAsync();
            };
        }

        /*
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await DisplayAlert("$Test", "Welcome", "okay");
            NotifyPropertyChanged("Sync");
        }
        */

        public async void LoadData(string itenummer)
        {
            // Set the itemnummer to a var
            var ResultNumber = itenummer;
            //Create a webservice URL
            string WEBSERVICE_URL = "https://web.kleyn.com:1919/locatiesdev/item/" + ResultNumber;
            //Set error 404 to false
            error404 = false;
            try
            {
                //Create the request
                var webRequest = WebRequest.Create(WEBSERVICE_URL);
                //Check if the Request isn`t null
                if (webRequest != null)
                {
                    //Set the settinges of the webrequest
                    webRequest.Method = "GET";
                    webRequest.Timeout = 12000;
                    webRequest.ContentType = "application/json";
                    webRequest.Headers.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");
                    //Try to get a response from the website
                    using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                    {
                        //Read the response 
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            // set the response to a var
                            var jsonResponse = sr.ReadToEnd();
                            //Convert ThejsonResponse to a array
                            var Data = JsonConvert.DeserializeObject<API_Items>(jsonResponse);
                            //Fill the page With Vehicle infoStatusBinding.Text = data.Voorraadstatus;
                            Department_Binding.Text = Data.bussinesunit;
                            Vehicle_name_binding.Text = Data.itemnummer;
                            Location_Binding.Text = Data.locatie;
                            Status_Binding.Text = Data.voorraadstatus;
                            //Start Looping in the label list
                            foreach (var o in Data.data)
                            {
                                //Search For the Merk and Type
                                //If label eq Merk start making the Vehicle name
                                if (o.label == "Merk")
                                {
                                    Vehicle_name.Text = o.waarde;

                                }
                                //If label eq type Add the 2 part to the Vehicle name
                                else if (o.label == "Type")
                                {
                                    Vehicle_name.Text = Vehicle_name.Text + "  " + o.waarde;
                                }
                            }
                            //Data list styling
                            MyListView.HasUnevenRows = true;
                            MyListView.ItemsSource = Data.data;
                            MyListView.HeightRequest = -1;
                            //MyListView.HeightRequest = 15 * Data.data.Count;
                        }
                    }
                }
            }
            //If a error occured throw a Exception
            catch (WebException ex)
            {
                // Create a ResponseWeb
                var ResponseWeb = ex.Response as HttpWebResponse;

                //Check If the StatusCode eq 404 throw a error page
                if (ResponseWeb.StatusCode == HttpStatusCode.NotFound)
                {
                    //Make the error page With the statuscode
                    var StatusCode = ResponseWeb.StatusCode;

                    //Send the user to the error page
                    var Errorpage = new ErrorPage();
                    NavigationPage.SetHasNavigationBar(Errorpage, false);
                    Errorpage.BindingContext = StatusCode;

                    await Navigation.PushAsync(Errorpage);
                }
            }
        }
        /*<StackLayout Orientation="Vertical">
                                                <Label Text="{Binding label}" x:Name="datalist" HorizontalOptions="Start" />
                                                <Label Text="{Binding waarde}" x:Name="datalist2"  HorizontalOptions="EndAndExpand"/>
                                            </StackLayout>

            <ListView IsPullToRefreshEnabled="True" IsEnabled="False" x:Name="MyListView" SeparatorVisibility="None" RowHeight="180">
                                <ListView.ItemTemplate>
                                    <DataTemplate>
                                        <ViewCell>
                                            <StackLayout Orientation="Vertical">
                                                <Label Text="{Binding Label}" x:Name="datalist" HeightRequest="25" HorizontalOptions="Start" />
                                                <Label Text="{Binding Waarde}" x:Name="datalist2" HorizontalOptions="End" Margin="0,-30,0,0" />
                                            </StackLayout>
                                        </ViewCell>
                                    </DataTemplate>
                                </ListView.ItemTemplate>
                            </ListView>

              <Grid x:Name="DataGrid">
                                                <Grid.RowDefinitions>
                                                    <RowDefinition Height="*" />
                                                </Grid.RowDefinitions>
                                                <Grid.ColumnDefinitions>
                                                    <ColumnDefinition Width="*"/>
                                                    <ColumnDefinition Width="*"/>
                                                </Grid.ColumnDefinitions>
                                              
                                                <!--Left column - BindingContext set to ModelPair.Item1   -->
                                                <Label  Grid.Row="0" Grid.Column="0"
                                              Text="{Binding label}"  HorizontalOptions="Start" FontAttributes="Bold"/>

                                                <!--Right column - BindingContext set to ModelPair.Item2 Margin="0,-30,0,0 -->
                                                <Label Grid.Row="0" Grid.Column="1"
                                              Text="{Binding waarde}" HorizontalOptions="End"/> 

                                            </Grid>
                                            */
        void Init()
        {
            //Hide take a video button for youtube
            //takeVideo.IsVisible = false;

            //Hide the pictures button first
            ViewMoreImgButton.IsVisible = false;
            btnCamera.IsVisible = false;

            Backbutton.HorizontalOptions = LayoutOptions.EndAndExpand;

            ViewMoreImgButton.BackgroundColor = Constants.KleynGroupTXT;
            ViewMoreImgButton.TextColor = Constants.LoginEntryBgColor;

            //header styling
            Header.BackgroundColor = Constants.KleynGroupBG;
            Logo.HeightRequest = 40;

            //Content styling
            Vehicle_name.FontSize = 20;
            Vehicle_name.TextColor = Color.Black;

            SlaveContentDetails.BackgroundColor = Color.FromRgb(206, 206, 206);
            SlaveContentLocation.BackgroundColor = Color.FromRgb(206, 206, 206);
            h1_Detail.FontSize = 17;
            //h1_Video.FontSize = 17;
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
            //Youtube_Open.BackgroundColor = Constants.KleynGroupTXT;
            //Youtube_Open.TextColor = Constants.LoginEntryBgColor;

            //Activty Spinner
            Spinner.Color = Constants.KleynGroupTXT;
            Spinner2.Color = Constants.KleynGroupTXT;
        }

        //(/◔ ◡ ◔)/ new But still a W.I.P for run function
        private async void BtnVideo_ClickedAsync(object sender, EventArgs e)
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if (status != PermissionStatus.Unknown)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                //Best practice to always check that the key exists
                if (results.ContainsKey(Permission.Camera))
                {
                    status = results[Permission.Camera];
                }

                if (status == PermissionStatus.Granted)
                {
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Camera Denied", "Can not continue, try again.", "OK");
                    return;
                }
            }
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
                await UploadYoutube(file, file.Path);
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
        public async Task UploadYoutube(MediaFile file, string filepath)
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
                    Run(file, filepath, ChannelID).Wait();
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
        private async Task Run(MediaFile file, string file_path, string ChannelID)
        {
            Console.WriteLine("Started Run");
            const string ClientId = "924710780360-74jgdjuemduba5ckh7pcfdfgs4c0jk63.apps.googleusercontent.com";
            const string Scope = "email";
            const string RedirectUrl = "https://developers.google.com/oauthplayground ";

            Console.WriteLine("Passed const strings");
            var authenticator = new OAuth2Authenticator(
                 ClientId,
                 null,
                 Constants.Scope,
                 new Uri(Constants.AuthorizeUrl),
                 new Uri(RedirectUrl),
                 new Uri(Constants.AccessTokenUrl),
                 null,
                 true);

            Console.WriteLine("Passed Authenicator");

            authenticator.Completed += OnAuthCompleted;
            authenticator.Error += OnAuthError;

            Console.WriteLine("Passed the Error/Completed Section");
            AuthenticationState.Authenticator = authenticator;

            Console.WriteLine("Passed the Authenicator class");
            if (Device.RuntimePlatform == Device.Android)
            {
                if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.N)
                {
                    Console.WriteLine("Android version not supported");
                    return;
                }
                else if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.N)
                {
                    var presenter = new Xamarin.Auth.Presenters.OAuthLoginPresenter();
                    Console.WriteLine("Passed the First presenter");
                    presenter.Login(authenticator);
                }
            } else
            {
                Console.WriteLine("Device not supported");
                return;
            }
            Console.WriteLine("Passed  presenter");
        }

        async void OnAuthCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }
        }

        void OnAuthError(object sender, AuthenticatorErrorEventArgs e)
        {
            var authenticator = sender as OAuth2Authenticator;
            if (authenticator != null)
            {
                authenticator.Completed -= OnAuthCompleted;
                authenticator.Error -= OnAuthError;
            }

            Console.WriteLine("Authentication error: " + e.Message);
        }
        /*
        Auth = new GoogleAuthenticator(Configuration.ClientId, Configuration.Scope, Configuration.RedirectUrl, this);

        if (Device.RuntimePlatform == Device.Android)
        {
            var authenticator = Auth.GetAuthenticator();
            var intent = authenticator.GetUI(this);
            StartActivity(intent);
        } else
        {
            await DisplayAlert("Not Supported", "Your platform is not supported for the app", "Abort");
            return;
        }
        */


        private async void BtnCamera_ClickedAsync(string itemnumber, string typeCode, string method)
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if (status != PermissionStatus.Unknown)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                //Best practice to always check that the key exists
                if (results.ContainsKey(Permission.Camera))
                {
                    status = results[Permission.Camera];
                }

                if (status == PermissionStatus.Granted)
                {
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Camera Denied", "Can not continue, try again.", "OK");
                    return;
                }
            }
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

            //Take action to upload/Change the image
            await ImageAction(itemnumber, typeCode, method, file);
        }

        private async void BtnPickPhoto_CickedAsync(string itemnumber, string typeCode, string method)
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
            if (status != PermissionStatus.Unknown)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                //Best practice to always check that the key exists
                if (results.ContainsKey(Permission.Storage))
                {
                    status = results[Permission.Storage];
                }

                if (status == PermissionStatus.Granted)
                {
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Storage Denied", "Can not continue, try again.", "OK");
                    return;
                }
            }
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


            //Take action to upload/Change the image
            await ImageAction(itemnumber, typeCode, method, file);
        }

        public async Task<Locatieitem> getPostion(string itemnummer)
        {
            // Store the Vehicle number
            var ResultNumber = itemnummer;
            // Create the web url endpoint 
            string WEBSERVICE_URL = "https://web.kleyn.com:1919/locatiesdev/location/" + ResultNumber;

            try
            {
                // Create the request 
                var locRequest = WebRequest.Create(WEBSERVICE_URL);
                // Check if the Request isn`t null
                if (locRequest != null)
                {
                    // Set the request settings
                    locRequest.Method = "GET";
                    locRequest.Timeout = 12000;
                    locRequest.ContentType = "application/json";
                    locRequest.Headers.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");
                    //Try to get a response from the website
                    using (System.IO.Stream s = locRequest.GetResponse().GetResponseStream())
                    {
                        //Try to read the response 
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            //Store the JsonResponse in a var
                            var jsonResponse = sr.ReadToEnd();
                            // Deserialize The Response
                            var Data = JsonConvert.DeserializeObject<Locatieitem>(jsonResponse);
                            //Return the response json
                            return Data;
                        }
                    }
                }
            }
            // IF a error occuerd throw a Exception
            catch (WebException ex)
            {
                //Create a Web Response
                var ResponseWeb = ex.Response as HttpWebResponse;

                // if StatusCode eq 404 execute the if statement
                if (ResponseWeb.StatusCode == HttpStatusCode.NotFound)
                {
                    //Create the error request
                    var StatusCode = ResponseWeb.StatusCode;
                    //Send the user error page
                    var Errorpage = new ErrorPage();
                    NavigationPage.SetHasNavigationBar(Errorpage, false);
                    Errorpage.BindingContext = StatusCode;

                    return null;
                }
            }
            return null;
        }


        public async void AddLocation_Clicked(object sender, EventArgs e)
        {
            // Create a Current Location Postion
            var locator = CrossGeolocator.Current;
            //Set the DesiredAccuracy
            locator.DesiredAccuracy = 25;
            // Try to get the user position
            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(25));
            //Ask the use if they want to change the location
            var answer = await DisplayAlert("Add Location", "Do you wan't to add your current location to this Vehilce " 
                + " With Latitude: " + position.Latitude.ToString() + " And Longitude: " + position.Longitude.ToString(), "Yes", "No");
            //Yes pressed
            if(answer)
            {
                //Go to the edit Location page
                await EditLocation(Vehicle_name_binding.Text, position.Latitude, position.Longitude);
            }
            //Canceld/ No pressed/ Somewhere else pressed
            else
            {
                //Return null
                return;
            }
        }

        public async Task EditLocation(string itemnummer, double latitude, double longitude)
        {
            //Create a HttpWebrequest and Set the Method,Contenttype,Api key
            var webRequest = (HttpWebRequest)WebRequest.Create("https://web.kleyn.com:1919/locatiesdev/location/" + itemnummer);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");

            //Try to get a response from the web adress
            using (var streamWritter = new StreamWriter(webRequest.GetRequestStream()))
            {
                var userPosition = new Xamarin.Forms.Labs.Services.Geolocation.Position
                {
                    Latitude = latitude,
                    Longitude = longitude
                };
                var KleynTrucks = new Xamarin.Forms.Labs.Services.Geolocation.Position
                {
                    Latitude = 51.8338083,
                    Longitude = 5.0867637
                };
                var KleynVans = new Xamarin.Forms.Labs.Services.Geolocation.Position
                {
                    Latitude = 51.8399417,
                    Longitude = 5.0877846
                };
                var Xiffix = new Xamarin.Forms.Labs.Services.Geolocation.Position
                {
                    Latitude = 52.3653804,
                    Longitude = 5.177049
                };
                var Dordrecht = new Xamarin.Forms.Labs.Services.Geolocation.Position
                {
                    Latitude = 51.7832742,
                    Longitude = 4.6306253
                };
                var Koornwaard = new Xamarin.Forms.Labs.Services.Geolocation.Position
                {
                    Latitude = 51.872357,
                    Longitude = 5.0575394
                };
                var VolteramTours = new Xamarin.Forms.Labs.Services.Geolocation.Position
                {
                    Latitude = 47.451492,
                    Longitude = 0.727125
                };
                var VredeBest = new Xamarin.Forms.Labs.Services.Geolocation.Position
                {
                    Latitude = 51.8442299,
                    Longitude = 5.0951131
                };

                var distancesTrucks = Xamarin.Forms.Labs.Services.Geolocation.PositionExtensions.DistanceFrom(userPosition, KleynTrucks);
                var distancesVans = Xamarin.Forms.Labs.Services.Geolocation.PositionExtensions.DistanceFrom(userPosition, KleynVans);
                var distancesXiffix = Xamarin.Forms.Labs.Services.Geolocation.PositionExtensions.DistanceFrom(userPosition, Xiffix);
                var distancesDordrecht = Xamarin.Forms.Labs.Services.Geolocation.PositionExtensions.DistanceFrom(userPosition, Dordrecht);
                var distancesKoornwaard = Xamarin.Forms.Labs.Services.Geolocation.PositionExtensions.DistanceFrom(userPosition, Koornwaard);
                var distancesFrance = Xamarin.Forms.Labs.Services.Geolocation.PositionExtensions.DistanceFrom(userPosition, VolteramTours);
                var distancesVredeBest = Xamarin.Forms.Labs.Services.Geolocation.PositionExtensions.DistanceFrom(userPosition, VredeBest);

                SortedList<double, string> Distances = new SortedList<double, string>();
                Distances.Add(distancesTrucks, "Trucks");
                Distances.Add(distancesVans, "Vans");
                Distances.Add(distancesXiffix, "Xiffix");
                Distances.Add(distancesDordrecht, "Dordrecht");
                Distances.Add(distancesKoornwaard, "KoornWaard");
                Distances.Add(distancesFrance, "Volteram France");
                Distances.Add(distancesVredeBest, "Vredebest 1");

                var action = from entry in Distances orderby entry.Key ascending select entry;
                var what = action.OrderBy(key => Distances.Keys);
                switch (what.First().Value)
                {
                    case "Volteram France":
                        Letter = "\"V\"";
                        break;
                    case "Dordrecht":
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
                    // If User canceld/ Pressed somewhere else Cancel the reqeust
                    default:
                        //Let the user know the request is canceld
                        await DisplayAlert("Grrrrr", "No location given, We need to have a location", "Abort");
                        return;
                }
                //Replace the comma in the Longitude and Latitude to avoid Json errors
                string Latitude_Less = latitude.ToString().Replace(',', '.');
                string Longitude_Less = longitude.ToString().Replace(',', '.');
                //Convert Current Time to Json date time set to avoid json errors
                string date = JsonConvert.SerializeObject(DateTime.Now);
                //Make the Json body
                string json = "{\"itemnummer\":" + itemnummer + ","
                    + "\"locatie\":" + Letter + "," +
                    "\"tijdstip\":" + date + ","
                    + "\"lengtegraad\":" + Longitude_Less + ","
                    + "\"breedtegraad\":" + Latitude_Less + ","
                    + "\"gebruiker\":\"Admin\"}";
                
                //Send the request to the api
                streamWritter.Write(json);
                //Clean the Call
                streamWritter.Flush();
                //Remove the Api Call
                streamWritter.Close();
            }
            //Create a Web Response
            var httpRepsone = (HttpWebResponse)webRequest.GetResponse();
            //Make a Stream Reader of the ResponseStream
            using (var streamReader = new StreamReader(httpRepsone.GetResponseStream()))
            {
                // Try to Read the response
                var result = streamReader.ReadToEnd();
                //Send the user Back to the Detailpage to refresh the location
                Navigation.InsertPageBefore(new DetailPage(itemnummer), this);
                await Navigation.PopAsync(true);
            }
        }
        
        public async void Location_Open_ClickedAsync(object sender, EventArgs e)
        {
            var GPSChoice = await DisplayAlert("Gps on?", "Is your GPS turned on?", "Yes", "No");
            if(GPSChoice)
            {

            } else
            {
                await DisplayAlert("Gps is off", "Your GPS is off turned it on", "Okay");
                return;
            }
            try
            {
                //Show the Activity spinner
                this.IsBusy = true;

                //Try to get the postion of the Vehicle
                var Item = await getPostion(Vehicle_name_binding.Text);
                //If there is no postion Create a new button 
                if (Item == null)
                {
                    UserDatabaseController UserData = new UserDatabaseController();
                    var Userinfo = UserData.GetAllUsers();
                    if (Userinfo[0].locationPermisson == 1)
                    {
                        //Set the Old button in a var and create a new one
                        var OldButton = Location_Open;
                        var NewButton = new Button
                        {
                            Text = "Add a location",
                            Margin = new Thickness(10, 5, 10, 5),
                            BackgroundColor = Constants.KleynGroupTXT,
                            TextColor = Constants.LoginEntryBgColor
                        };
                        //Add the addlocation function to the button
                        NewButton.Clicked += new EventHandler(AddLocation_Clicked);
                        //Hide the open location button
                        Location_Open.IsVisible = false;
                        //Add the Add location button
                        SlaveContentLocation.Children.Add(NewButton);
                        //Show the user there is no location
                        await DisplayAlert("Error", "No Location Found =(", "Ok");
                        //Hide the Activity spinner
                        this.IsBusy = false;
                    } else
                    {
                        Label NewLabel = new Label
                        {
                            Text = "No Loaction found and no permission to add a location",
                            TextColor = Color.DimGray
                        };
                        Location_Open.IsVisible = false;
                        SlaveContentLocation.Children.Add(NewLabel);
                        this.IsBusy = false;
                    }
                }
                //Else Open Google Maps
                else
                {
                    TerrainLocations terrainLocations = new TerrainLocations();
                    // Get the users location
                    var locator = CrossGeolocator.Current;
                    //Set the DeiredAccuracy as low as posible
                    locator.DesiredAccuracy = 0.01;
                    //Await the postion of the user
                    var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(0.01));
                    Position UserPosition = new Position(position.Latitude, position.Longitude);
                    //Make a new pin to show the user where the Vehicle is
                    var PinPostion = new Position(Item.breedtegraad, Item.lengtegraad);
                    //Create the pin
                    var pin = new Pin
                    {
                        Type = PinType.Place,
                        Position = PinPostion,
                        Label = Vehicle_name.Text,
                        Address = Vehicle_name_binding.Text,  
                    };
                    var FridgeTraillersPin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.834240, 5.083013),
                        Label = "Fridge Traillers",
                        Address = "Area",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var SlideSheetPin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.834154, 5.083692),
                        Label = "Slide Sails",
                        Address = "Area",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var TippersPin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.834701, 5.083819),
                        Label = "Tippers",
                        Address = "Area",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var TanksPin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.834700, 5.084226),
                        Label = "Tanks",
                        Address = "Area",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var LowLoaderPin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.834174, 5.084135),
                        Label = "Low Loaders",
                        Address = "Area",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var RemainderTrailersPin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.833758, 5.084044),
                        Label = "Remainder Traillers",
                        Address = "Area",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var ContainerTransportPin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.833758, 5.084728),
                        Label = "Container Transport",
                        Address = "Area",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var Combi1Pin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.834168, 5.084896),
                        Label = "Combi Terrain",
                        Address = "Area 1",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var Combi2Pin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.834587, 5.084864),
                        Label = "Combi Terrain",
                        Address = "Area 2",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var EntryVansPin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.833682, 5.085553),
                        Label = "Entry/New Vans",
                        Address = "Area",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var EntryTucksPin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.834045, 5.085703),
                        Label = "Entry/New Trucks",
                        Address = "Area",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var TractorsPin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.834580, 5.087807),
                        Label = "Pull Trucks",
                        Address = "Area",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var TruckPartsPin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.833738, 5.087773),
                        Label = "Truck Parts",
                        Address = "Area",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var DeliverVansPin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.833479, 5.089702),
                        Label = "Deliver Vans",
                        Address = "Area",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var Combi3Pin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.833228, 5.088725),
                        Label = "Combi Terrain",
                        Address = "Area 3",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var BinTruck1Pin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.833111, 5.089607),
                        Label = "Bin Trucks",
                        Address = "Area 1",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var BinTruck2Pin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.832836, 5.088674),
                        Label = "Bin Trucks",
                        Address = "Area 2",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var BinTruck3Pin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.832983, 5.087395),
                        Label = "Bin Trucks",
                        Address = "Area 3",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var Tippers2Pin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.832442, 5.088614),
                        Label = "Tippers",
                        Address = "Area 2",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var Combi4Pin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.832462, 5.086936),
                        Label = "Combi Terrain",
                        Address = "Area 4",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var Tippers3Pin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.832287, 5.088276),
                        Label = "Tippers",
                        Address = "Area 3",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    var MixerTrucksPin = new Pin
                    {
                        Type = PinType.Generic,
                        Position = new Position(51.832239, 5.088967),
                        Label = "Mixer Trucks",
                        Address = "Area",
                        Icon = BitmapDescriptorFactory.DefaultMarker(Color.FromRgb(239, 118, 34))
                    };
                    //Make a new StackLayout
                    var stack = new StackLayout { Spacing = 0 };
                    //Make a new Map
                    var map = new Map();
                    Polygon polygon = new Polygon
                    {
                        FillColor = terrainLocations.KleynOrange,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    polygon.Positions.Add(new Position(terrainLocations.FTp1Latitude, terrainLocations.FTp1Longitude));
                    polygon.Positions.Add(new Position(terrainLocations.FTp2Latitude, terrainLocations.FTp2Longitude));
                    polygon.Positions.Add(new Position(terrainLocations.FTp3Latitude, terrainLocations.FTp3Longitude));
                    polygon.Positions.Add(new Position(terrainLocations.FTp4Latitude, terrainLocations.FTp4Longitude));

                    Polygon slidesheets = new Polygon
                    {
                        FillColor = terrainLocations.orangeRed,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    slidesheets.Positions.Add(new Position(terrainLocations.SZp1Latitude, terrainLocations.SZp1Longitude));
                    slidesheets.Positions.Add(new Position(terrainLocations.SZp2Latitude, terrainLocations.SZp2Longitude));
                    slidesheets.Positions.Add(new Position(terrainLocations.SZp4Latitude, terrainLocations.SZp4Longitude));
                    slidesheets.Positions.Add(new Position(terrainLocations.SZp3Latitude, terrainLocations.SZp3Longitude));

                    Polygon tippers = new Polygon
                    {
                        FillColor = terrainLocations.KleynOrange,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    tippers.Positions.Add(new Position(terrainLocations.TIp1Latitude, terrainLocations.TIp1Longitude));
                    tippers.Positions.Add(new Position(terrainLocations.TIp2Latitude, terrainLocations.TIp2Longitude));
                    tippers.Positions.Add(new Position(terrainLocations.TIp3Latitude, terrainLocations.TIp3Longitude));
                    tippers.Positions.Add(new Position(terrainLocations.TIp4Latitude, terrainLocations.TIp4Longitude));

                    Polygon tanks = new Polygon
                    {
                        FillColor = terrainLocations.orangeRed,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    tanks.Positions.Add(new Position(terrainLocations.TKp1Latitude, terrainLocations.TKp1Longitude));
                    tanks.Positions.Add(new Position(terrainLocations.TKp2Latitude, terrainLocations.TKp2Longitude));
                    tanks.Positions.Add(new Position(terrainLocations.TKp3Latitude, terrainLocations.TKp3Longitude));
                    tanks.Positions.Add(new Position(terrainLocations.TKp4Latitude, terrainLocations.TKp4Longitude));

                    Polygon LowLoaders = new Polygon
                    {
                        FillColor = terrainLocations.KleynOrange,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    LowLoaders.Positions.Add(new Position(terrainLocations.LLp1Latitude, terrainLocations.LLp1Longitude));
                    LowLoaders.Positions.Add(new Position(terrainLocations.LLp2Latitude, terrainLocations.LLp2Longitude));
                    LowLoaders.Positions.Add(new Position(terrainLocations.LLp3Latitude, terrainLocations.LLp3Longitude));
                    LowLoaders.Positions.Add(new Position(terrainLocations.LLp4Latitude, terrainLocations.LLp4Longitude));

                    Polygon RemainderTrailers = new Polygon
                    {
                        FillColor = terrainLocations.orangeRed,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    RemainderTrailers.Positions.Add(new Position(terrainLocations.RTp1Latitude, terrainLocations.RTp1Longitude));
                    RemainderTrailers.Positions.Add(new Position(terrainLocations.RTp2Latitude, terrainLocations.RTp2Longitude));
                    RemainderTrailers.Positions.Add(new Position(terrainLocations.RTp3Latitude, terrainLocations.RTp3Longitude));
                    RemainderTrailers.Positions.Add(new Position(terrainLocations.RTp4Latitude, terrainLocations.RTp4Longitude));

                    Polygon ContainerTransport = new Polygon
                    {
                        FillColor = terrainLocations.KleynOrange,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    ContainerTransport.Positions.Add(new Position(terrainLocations.CTp1Latitude, terrainLocations.CTp1Longitude));
                    ContainerTransport.Positions.Add(new Position(terrainLocations.CTp2Latitude, terrainLocations.CTp2Longitude));
                    ContainerTransport.Positions.Add(new Position(terrainLocations.CTp3Latitude, terrainLocations.CTp3Longitude));
                    ContainerTransport.Positions.Add(new Position(terrainLocations.CTp4Latitude, terrainLocations.CTp4Longitude));

                    Polygon Combi1 = new Polygon
                    {
                        FillColor = terrainLocations.KleynGray,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    Combi1.Positions.Add(new Position(terrainLocations.CB1p1Latitude, terrainLocations.CB1p1Longitude));
                    Combi1.Positions.Add(new Position(terrainLocations.CB1p2Latitude, terrainLocations.CB1p2Longitude));
                    Combi1.Positions.Add(new Position(terrainLocations.CB1p3Latitude, terrainLocations.CB1p3Longitude));
                    Combi1.Positions.Add(new Position(terrainLocations.CB1p4Latitude, terrainLocations.CB1p4Longitude));

                    Polygon Combi2 = new Polygon
                    {
                        FillColor = terrainLocations.DarkerGray,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    Combi2.Positions.Add(new Position(terrainLocations.CB2p1Latitude, terrainLocations.CB2p1Longitude));
                    Combi2.Positions.Add(new Position(terrainLocations.CB2p2Latitude, terrainLocations.CB2p2Longitude));
                    Combi2.Positions.Add(new Position(terrainLocations.CB2p3Latitude, terrainLocations.CB2p3Longitude));
                    Combi2.Positions.Add(new Position(terrainLocations.CB2p4Latitude, terrainLocations.CB2p4Longitude));

                    Polygon EntryVans = new Polygon
                    {
                        FillColor = terrainLocations.KleynGreen,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    EntryVans.Positions.Add(new Position(terrainLocations.EVp1Latitude, terrainLocations.EVp1Longitude));
                    EntryVans.Positions.Add(new Position(terrainLocations.EVp2Latitude, terrainLocations.EVp2Longitude));
                    EntryVans.Positions.Add(new Position(terrainLocations.EVp3Latitude, terrainLocations.EVp3Longitude));
                    EntryVans.Positions.Add(new Position(terrainLocations.EVp4Latitude, terrainLocations.EVp4Longitude));
                    EntryVans.Positions.Add(new Position(terrainLocations.EVp5Latitude, terrainLocations.EVp5Longitude));
                    EntryVans.Positions.Add(new Position(terrainLocations.EVp6Latitude, terrainLocations.EVp6Longitude));

                    Polygon EntryTrucks = new Polygon
                    {
                        FillColor = terrainLocations.KleynBlue,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    EntryTrucks.Positions.Add(new Position(terrainLocations.ETp1Latitude, terrainLocations.ETp1Longitude));
                    EntryTrucks.Positions.Add(new Position(terrainLocations.ETp2Latitude, terrainLocations.ETp2Longitude));
                    EntryTrucks.Positions.Add(new Position(terrainLocations.ETp3Latitude, terrainLocations.ETp3Longitude));
                    EntryTrucks.Positions.Add(new Position(terrainLocations.ETp4Latitude, terrainLocations.ETp4Longitude));

                    Polygon Tractors = new Polygon
                    {
                        FillColor = terrainLocations.SecondBlue,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    Tractors.Positions.Add(new Position(terrainLocations.TRp1Latitude, terrainLocations.TRp1Longitude));
                    Tractors.Positions.Add(new Position(terrainLocations.TRp2Latitude, terrainLocations.TRp2Longitude));
                    Tractors.Positions.Add(new Position(terrainLocations.TRp3Latitude, terrainLocations.TRp3Longitude));
                    Tractors.Positions.Add(new Position(terrainLocations.TRp4Latitude, terrainLocations.TRp4Longitude));
                    Tractors.Positions.Add(new Position(terrainLocations.TRp5Latitude, terrainLocations.TRp5Longitude));
                    Tractors.Positions.Add(new Position(terrainLocations.TRp6Latitude, terrainLocations.TRp6Longitude));
                    Tractors.Positions.Add(new Position(terrainLocations.TRp7Latitude, terrainLocations.TRp7Longitude));
                    Tractors.Positions.Add(new Position(terrainLocations.TRp8Latitude, terrainLocations.TRp8Longitude));

                    Polygon TruckParts = new Polygon
                    {
                        FillColor = terrainLocations.DarkerGray,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    TruckParts.Positions.Add(new Position(terrainLocations.TRPp1Latitude, terrainLocations.TRPp1Longitude));
                    TruckParts.Positions.Add(new Position(terrainLocations.TRPp2Latitude, terrainLocations.TRPp2Longitude));
                    TruckParts.Positions.Add(new Position(terrainLocations.TRPp3Latitude, terrainLocations.TRPp3Longitude));
                    TruckParts.Positions.Add(new Position(terrainLocations.TRPp4Latitude, terrainLocations.TRPp4Longitude));

                    Polygon DeliverVans = new Polygon
                    {
                        FillColor = terrainLocations.SecondGreen,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    DeliverVans.Positions.Add(new Position(terrainLocations.DVp1Latitude, terrainLocations.DVp1Longitude));
                    DeliverVans.Positions.Add(new Position(terrainLocations.DVp2Latitude, terrainLocations.DVp2Longitude));
                    DeliverVans.Positions.Add(new Position(terrainLocations.DVp3Latitude, terrainLocations.DVp3Longitude));
                    DeliverVans.Positions.Add(new Position(terrainLocations.DVp4Latitude, terrainLocations.DVp4Longitude));

                    Polygon Combi3 = new Polygon
                    {
                        FillColor = terrainLocations.KleynGray,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    Combi3.Positions.Add(new Position(terrainLocations.CB3p1Latitude, terrainLocations.CB3p1Longitude));
                    Combi3.Positions.Add(new Position(terrainLocations.CB3p2Latitude, terrainLocations.CB3p2Longitude));
                    Combi3.Positions.Add(new Position(terrainLocations.CB3p3Latitude, terrainLocations.CB3p3Longitude));
                    Combi3.Positions.Add(new Position(terrainLocations.CB3p4Latitude, terrainLocations.CB3p4Longitude));

                    Polygon BinTrucks1 = new Polygon
                    {
                        FillColor = terrainLocations.KleynBlue,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    BinTrucks1.Positions.Add(new Position(terrainLocations.BT1p1Latitude, terrainLocations.BT1p1Longitude));
                    BinTrucks1.Positions.Add(new Position(terrainLocations.BT1p2Latitude, terrainLocations.BT1p2Longitude));
                    BinTrucks1.Positions.Add(new Position(terrainLocations.BT1p3Latitude, terrainLocations.BT1p3Longitude));
                    BinTrucks1.Positions.Add(new Position(terrainLocations.BT1p4Latitude, terrainLocations.BT1p4Longitude));
                    BinTrucks1.Positions.Add(new Position(terrainLocations.BT1p5Latitude, terrainLocations.BT1p5Longitude));
                    BinTrucks1.Positions.Add(new Position(terrainLocations.BT1p6Latitude, terrainLocations.BT1p6Longitude));

                    Polygon BinTrucks2 = new Polygon
                    {
                        FillColor = terrainLocations.SecondBlue,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    BinTrucks2.Positions.Add(new Position(terrainLocations.BT2p1Latitude, terrainLocations.BT2p1Longitude));
                    BinTrucks2.Positions.Add(new Position(terrainLocations.BT2p2Latitude, terrainLocations.BT2p2Longitude));
                    BinTrucks2.Positions.Add(new Position(terrainLocations.BT2p3Latitude, terrainLocations.BT2p3Longitude));
                    BinTrucks2.Positions.Add(new Position(terrainLocations.BT2p4Latitude, terrainLocations.BT2p4Longitude));

                    Polygon BinTrucks3 = new Polygon
                    {
                        FillColor = terrainLocations.KleynBlue,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    BinTrucks3.Positions.Add(new Position(terrainLocations.BT3p1Latitude, terrainLocations.BT3p1Longitude));
                    BinTrucks3.Positions.Add(new Position(terrainLocations.BT3p2Latitude, terrainLocations.BT3p2Longitude));
                    BinTrucks3.Positions.Add(new Position(terrainLocations.BT3p3Latitude, terrainLocations.BT3p3Longitude));
                    BinTrucks3.Positions.Add(new Position(terrainLocations.BT3p4Latitude, terrainLocations.BT3p4Longitude));
                    BinTrucks3.Positions.Add(new Position(terrainLocations.BT3p5Latitude, terrainLocations.BT3p5Longitude));
                    BinTrucks3.Positions.Add(new Position(terrainLocations.BT3p6Latitude, terrainLocations.BT3p6Longitude));

                    Polygon Tippers2 = new Polygon
                    {
                        FillColor = terrainLocations.orangeRed,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    Tippers2.Positions.Add(new Position(terrainLocations.TP2p1Latitude, terrainLocations.TP2p1Longitude));
                    Tippers2.Positions.Add(new Position(terrainLocations.TP2p2Latitude, terrainLocations.TP2p2Longitude));
                    Tippers2.Positions.Add(new Position(terrainLocations.TP2p3Latitude, terrainLocations.TP2p3Longitude));
                    Tippers2.Positions.Add(new Position(terrainLocations.TP2p4Latitude, terrainLocations.TP2p4Longitude));

                    Polygon Combi4 = new Polygon
                    {
                        FillColor = terrainLocations.DarkerGray,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    Combi4.Positions.Add(new Position(terrainLocations.CB4p1Latitude, terrainLocations.CB4p1Longitude));
                    Combi4.Positions.Add(new Position(terrainLocations.CB4p2Latitude, terrainLocations.CB4p2Longitude));
                    Combi4.Positions.Add(new Position(terrainLocations.CB4p3Latitude, terrainLocations.CB4p3Longitude));
                    Combi4.Positions.Add(new Position(terrainLocations.CB4p4Latitude, terrainLocations.CB4p4Longitude));
                    Combi4.Positions.Add(new Position(terrainLocations.CB4p5Latitude, terrainLocations.CB4p5Longitude));
                    Combi4.Positions.Add(new Position(terrainLocations.CB4p6Latitude, terrainLocations.CB4p6Longitude));

                    Polygon Tippers3 = new Polygon
                    {
                        FillColor = terrainLocations.KleynOrange,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    Tippers3.Positions.Add(new Position(terrainLocations.TP3p1Latitude, terrainLocations.TP3p1Longitude));
                    Tippers3.Positions.Add(new Position(terrainLocations.TP3p2Latitude, terrainLocations.TP3p2Longitude));
                    Tippers3.Positions.Add(new Position(terrainLocations.TP3p3Latitude, terrainLocations.TP3p3Longitude));
                    Tippers3.Positions.Add(new Position(terrainLocations.TP3p4Latitude, terrainLocations.TP3p4Longitude));

                    Polygon MixerTrucks = new Polygon
                    {
                        FillColor = terrainLocations.SecondBlue,
                        StrokeColor = Constants.LoginEntryBgColor
                    };
                    MixerTrucks.Positions.Add(new Position(terrainLocations.MTp1Latitude, terrainLocations.MTp1Longitude));
                    MixerTrucks.Positions.Add(new Position(terrainLocations.MTp2Latitude, terrainLocations.MTp2Longitude));
                    MixerTrucks.Positions.Add(new Position(terrainLocations.MTp3Latitude, terrainLocations.MTp3Longitude));
                    MixerTrucks.Positions.Add(new Position(terrainLocations.MTp4Latitude, terrainLocations.MTp4Longitude));
                    
                    //Set the map to the Items postion
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude),
                                              Distance.FromMiles(0.10)));
                    //Add some Extra settings
                    map.MyLocationEnabled = true;
                    map.UiSettings.MyLocationButtonEnabled = true;
                    map.UiSettings.CompassEnabled = true;
                    
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
                    var BackButton = new Button
                    {
                        Text = "Back",
                        TextColor = Color.White,
                        BackgroundColor = Color.Transparent,
                        Margin = new Thickness(0, -50, 25, 0),
                        HorizontalOptions = LayoutOptions.Center
                    };
                    // Give the header the logo
                    header_maps.Children.Add(Logo);
                    header_maps.Children.Add(BackButton);
                    UserDatabaseController UserData = new UserDatabaseController();
                    var Userinfo = UserData.GetAllUsers();
                    if (Userinfo[0].locationPermisson == 1)
                    {
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
                        EditLocation.Clicked += new EventHandler(AddLocation_Clicked);
                        header_maps.Children.Add(EditLocation);
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
                    //If the user pressed the MyLocationButton go to the users location
                    map.MyLocationButtonClicked += async (data, arags) =>
                    {
                        var locator2 = CrossGeolocator.Current;
                        locator.DesiredAccuracy = 0.01;
                        //Await the users postion
                        var position2 = await locator.GetPositionAsync(TimeSpan.FromSeconds(0.01));
                        //Move the Camera and map to the users postion
                        map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position2.Latitude, position2.Longitude),
                                          Distance.FromMiles(0.10)));
                    };

                    BackButton.Clicked += async (sender1, args) =>
                    {
                        await Navigation.PopAsync();
                    };
                    // Give the footer the Copy Right Label
                    footer_maps.Children.Add(CopyRightLabel);

                    //Add The pins and terrain areas to the map
                    map.Pins.Add(pin);
                    map.Pins.Add(FridgeTraillersPin);
                    map.Pins.Add(SlideSheetPin);
                    map.Pins.Add(TippersPin);
                    map.Pins.Add(TanksPin);
                    map.Pins.Add(LowLoaderPin);
                    map.Pins.Add(RemainderTrailersPin);
                    map.Pins.Add(ContainerTransportPin);
                    map.Pins.Add(Combi1Pin);
                    map.Pins.Add(Combi2Pin);
                    map.Pins.Add(EntryVansPin);
                    map.Pins.Add(EntryTucksPin);
                    map.Pins.Add(TractorsPin);
                    map.Pins.Add(TruckPartsPin);
                    map.Pins.Add(DeliverVansPin);
                    map.Pins.Add(Combi3Pin);
                    map.Pins.Add(BinTruck1Pin);
                    map.Pins.Add(BinTruck2Pin);
                    map.Pins.Add(BinTruck3Pin);
                    map.Pins.Add(Tippers2Pin);
                    map.Pins.Add(Combi4Pin);
                    map.Pins.Add(Tippers3Pin);
                    map.Pins.Add(MixerTrucksPin);

                    map.Polygons.Add(polygon);
                    map.Polygons.Add(slidesheets);
                    map.Polygons.Add(tippers);
                    map.Polygons.Add(tanks);
                    map.Polygons.Add(LowLoaders);
                    map.Polygons.Add(RemainderTrailers);
                    map.Polygons.Add(ContainerTransport);
                    map.Polygons.Add(Combi1);
                    map.Polygons.Add(Combi2);
                    map.Polygons.Add(EntryVans);
                    map.Polygons.Add(EntryTrucks);
                    map.Polygons.Add(Tractors);
                    map.Polygons.Add(TruckParts);
                    map.Polygons.Add(DeliverVans);
                    map.Polygons.Add(Combi3);
                    map.Polygons.Add(BinTrucks1);
                    map.Polygons.Add(BinTrucks2);
                    map.Polygons.Add(BinTrucks3);
                    map.Polygons.Add(Tippers2);
                    map.Polygons.Add(Combi4);
                    map.Polygons.Add(Tippers3);
                    map.Polygons.Add(MixerTrucks);

                    //Bind the footer/map/header togther
                    map.SelectedPin = pin;

                    stack.Children.Add(header_maps);
                    stack.Children.Add(map);
                    stack.Children.Add(footer_maps);
                    Content = stack;
                }
            }
                finally
                {
                    //Hide the Activity spinner
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
                    var Errorpage = new ErrorPage();
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
                
                Navigation.InsertPageBefore(new DetailPage(itemnummer), this);
                await Navigation.PopAsync(true);
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
                //Youtube_Open.IsVisible = false;
                //SlaveContentVideo.Children.Add(NewButton);
                await DisplayAlert("Error", "No Youtube accounts found =(", "Ok");
            } else
            {
                Picker picker = new Picker
                {
                    Title = "Youtube channels",
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    Margin = new Thickness(10, 0)
                };

                //Youtube_Open.IsVisible = false;
                //takeVideo.IsVisible = true;
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
                //SlaveContentVideo.Children.Add(picker);
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
            //Store  the itemnumber in a string
            var ResultNumber = itemnummer;
            //Create the EndPoint
            string WEBSERVICE_URL = "https://web.kleyn.com:1919/locatiesdev/picturelist/" + ResultNumber;
            try
            {
                //Create a Web Request
                var PictureListRequest = WebRequest.Create(WEBSERVICE_URL);
                //Check if the Web Request isn`t Empty
                if (PictureListRequest != null)
                {
                    //Set the Setting for the Web Request
                    PictureListRequest.Method = "GET";
                    PictureListRequest.Timeout = 12000;
                    PictureListRequest.ContentType = "application/json";
                    PictureListRequest.Headers.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");
                    //Try to get a respone
                    using (System.IO.Stream s = PictureListRequest.GetResponse().GetResponseStream())
                    {
                        //Try to read the stream response
                        using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                        {
                            //Read  the json Response
                            var jsonResponse = sr.ReadToEnd();
                            //Deserialize The Json Response
                            var ListInfo = JsonConvert.DeserializeObject<Records>(jsonResponse);
                            //Return the Records List
                            return ListInfo;
                        }
                    }
                }
            }
            //If a error occured throw a Exception
            catch (WebException ex)
            {
                //Create a new Web Response
                var ResponseWeb = ex.Response as HttpWebResponse;
                //Check if the WebResponse if eq to 404
                if (ResponseWeb.StatusCode == HttpStatusCode.NotFound)
                {
                    //IF WebRequest is eq to 404 Create a var with a send the user to the Error Page
                    var StatusCode = ResponseWeb.StatusCode;
                    var Errorpage = new ErrorPage();
                    NavigationPage.SetHasNavigationBar(Errorpage, false);
                    Errorpage.BindingContext = StatusCode;

                    return null;
                }
            }
            return null;
        }

        public async Task ImageAction(string itemnummer, string typecode, string method, MediaFile file)
        {
            //Make a new var ByteConverterController
            var byteConverter = new ByteConverterController();
            //Check if method is post or put
            if (method == "POST" || method == "post")
            {
                //If the method is post continue and ask the user if they want to upload the image
                var answer = await DisplayAlert("Change Image?", "Are you sure you want to upload this image", "Yes", "cancel");
                //If the user pressed yes
                if (answer)
                {
                    //Ask the user what kind of picture this is?
                    var typeCodeChoice = await DisplayActionSheet("What kind of picture is this", "Abort", null, "Head picture Left front",
                            "Head picture Right Front", "Head picture left behind", "Head picture right behind", "Interior", "Dashboard",
                            "Specials", "container", "Combination", "Double cabie/cabin", "Radio/Cd player", "Books", "Keys", "Air conditioning",
                            "Under the Hood", "Entry front", "Entry behind", "Cool engine", "Km. Stand", "Chassisno", "Construction sign",
                            "Front of the vehicle", "Back of the vehicle", "Licence plate");
                    //Check what the user has pressed and choose the typecode
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
                        //If the user pressed cancel or some where on the screen cancel the request
                        default:
                            // Let the user know he canceled the request
                            await DisplayAlert("Canceled", "Nothing has been chosen", "Abort");
                            return;
                    }
                    // Do the upload reqeust with the chosen typcode
                    var DoCall = byteConverter.ChangeUploadImage(typeCodeChoice, itemnummer, method, file);
                    // send the user back to the detail page

                    Navigation.InsertPageBefore(new DetailPage(itemnummer), this);
                    await Navigation.PopAsync(true);
                }
                // If the user pressed no or some where on the screen canceld the process
                else
                {
                    //Let the user know the request has been canceld
                    await DisplayAlert("Abort", "Cancel has been chosen", "abort");
                    return;
                }
            }
            else
            {
                // Ask the user if he/she want to edit the image
                var answer = await DisplayAlert("Change Image?", "Are you sure you want to edit this image", "Yes", "cancel");
                //Yes pressed
                if (answer)
                {
                    //Do the edit call
                    var DoCall = byteConverter.ChangeUploadImage(typecode, itemnummer, method, file);
                    //Send the user back to the detail page
                    Navigation.InsertPageBefore(new DetailPage(itemnummer), this);
                    await Navigation.PopAsync(true);
                }
                // Cancel pressed/ Somewhere else on the screen
                else
                {
                    //Let the user know the process has been canceld
                    await DisplayAlert("Abort", "Cancel has been chosen", "abort");
                    return;
                }

            }
        }
    }
}