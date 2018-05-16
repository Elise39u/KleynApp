using KleynGroup.Models;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net;
using System.Threading.Tasks;
using KleynGroup.Data;
using Newtonsoft.Json;
using Xamarin.Forms.GoogleMaps;
using Plugin.Geolocator;




namespace KleynGroup.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailPage
    {
        public string Letter;

        public DetailPage(string itemnumber)
        {
            string resultNumber = itemnumber;
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Init();
            LoadData(resultNumber);
            var userDatabase = new UserDatabase();
            var userdata = userDatabase.GetAllUsers();
            var getael = userdata.First().Ael;
            var getadmin = userdata.First().IsAdmin;


            var location = GetPostion(itemnumber).GetResult();

            if (location == null)
            {
                if (getael == "1" || getadmin == "1")
                {
                    var newButton = new Button
                    {
                        Text = "Add a location",
                        Margin = new Thickness(10, 5, 10, 5),
                        BackgroundColor = Constants.KleynGroupTxt,
                        TextColor = Constants.LoginEntryBgColor
                    };
                    SlaveContentLocation.Children.Add(newButton);
                    newButton.Clicked += AddLocation_Clicked;
                }
                else
                {
                    var errorlabel = new Label
                    {
                        Text = "There is no location set!",
                        TextColor = Constants.KleynGroupTxt,
                        FontSize = 20
                    };
                    SlaveContentLocation.Children.Add(errorlabel);
                }
            }
            else
            {
                var newButton = new Button
                {
                    Text = "Show vehicle location",
                    Margin = new Thickness(10, 5, 10, 5),
                    BackgroundColor = Constants.KleynGroupTxt,
                    TextColor = Constants.LoginEntryBgColor
                };
                SlaveContentLocation.Children.Add(newButton);
                newButton.Clicked += Location_Open_ClickedAsync;
            }
        }

        async void ReturnBackDetailPage(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DetailPage(VehicleNameBinding.Text));
        }

        async void ReturnBack(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Dashboard());
        }

        async void EditVehicle(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Dashboard());
        }


        public async void LoadData(string itenummer)
        {
            if (CheckNetwork.IsInternet())
            {
                var resultNumber = itenummer;
                string webserviceUrl = "https://web.kleyn.com:1919/locatiesdev/item/" + resultNumber;


                try
                {
                    var webRequest = WebRequest.Create(webserviceUrl);
                    webRequest.Method = "GET";
                    webRequest.Timeout = 12000;
                    webRequest.ContentType = "application/json";
                    webRequest.Headers.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");

                    using (var s = webRequest.GetResponse().GetResponseStream())
                    {
                        using (var sr =
                            new StreamReader(s ?? throw new InvalidOperationException()))
                        {
                            var jsonResponse = sr.ReadToEnd();
                            var data = JsonConvert.DeserializeObject<ApiItems>(jsonResponse);
                            VehicleNameBinding.Text = data.Itemnummer;
                            if (data.Voorraadstatus == "Verkocht" || data.Voorraadstatus == "Afgeleverd")
                            {
                                StatusBinding.TextColor = Color.Red;
                            }
                            else if (data.Voorraadstatus == "Op voorraad")
                            {
                                StatusBinding.TextColor = Color.LimeGreen;
                            }
                            else
                            {
                                StatusBinding.TextColor = Color.Black;
                            }

                            StatusBinding.Text = data.Voorraadstatus;
                            LocationBinding.Text = data.Locatie;
                            DepartmentBinding.Text = data.Bussinesunit;
                            VehicleImage.Source = "https://s3-eu-west-1.amazonaws.com/kleyn-api/vehicle/" +
                                                  data.Itemnummer + "/hlv/600.jpg";

                            foreach (var o in data.Data)
                            {
                                if (o.Label == "Merk")
                                {
                                    VehicleName.Text = o.Waarde;

                                }
                                else if (o.Label == "Type")
                                {
                                    VehicleName.Text = VehicleName.Text + "  " + o.Waarde;
                                }

                            }

                            //Data list styling
                            VehicleDetailList.HasUnevenRows = true;
                            VehicleDetailList.ItemsSource = data.Data;
                            VehicleDetailList.HeightRequest = 40 * data.Data.Count;
                        }
                    }
                }
                catch (WebException ex)
                {
                    if (ex.Response is HttpWebResponse responseWeb)
                    {
                        Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                        Console.WriteLine(responseWeb.StatusCode);
                        Console.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

                        if (responseWeb.StatusCode == HttpStatusCode.NotFound)
                        {
                            var StatusCode = "ERROR 404: Vehicle not found";
                            var joke = "Our monkeys are not able to find the result of your request.";
                            var errorpage = new NavigationPage(new ErrorPage());
                            NavigationPage.SetHasNavigationBar(errorpage, false);
                            errorpage.BindingContext = new {Error = StatusCode, Joke = joke};
                            await Navigation.PushAsync(errorpage);
                        }
                        else if (responseWeb.StatusCode == HttpStatusCode.BadRequest)
                        {
                            var StatusCode = "ERROR 400: Bad request";
                            var joke = "Our monkeys didn't understand your request!";
                            var errorpage = new NavigationPage(new ErrorPage());
                            NavigationPage.SetHasNavigationBar(errorpage, false);
                            errorpage.BindingContext = new {Error = StatusCode, Joke = joke};
                            await Navigation.PushAsync(errorpage);
                        }
                        else if (responseWeb.StatusCode == HttpStatusCode.Forbidden)
                        {
                            var StatusCode = "ERROR 403: Forbidden";
                            var joke = "Our monkeys don't allow your request!";
                            var errorpage = new NavigationPage(new ErrorPage());
                            NavigationPage.SetHasNavigationBar(errorpage, false);
                            errorpage.BindingContext = new {Error = StatusCode, Joke = joke};
                            await Navigation.PushAsync(errorpage);
                        }
                        else if (responseWeb.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            var StatusCode = "ERROR 401: Unauthorized";
                            var joke = "There is no key to the monkey cage :(";
                            var errorpage = new NavigationPage(new ErrorPage());
                            NavigationPage.SetHasNavigationBar(errorpage, false);
                            errorpage.BindingContext = new {Error = StatusCode, Joke = joke};
                            await Navigation.PushAsync(errorpage);
                        }
                        else if (responseWeb.StatusCode == HttpStatusCode.InternalServerError)
                        {
                            var StatusCode = "ERROR 500: Internal server error";
                            var joke = "Our monkeys are sick. They are not able to work propperly.";
                            var errorpage = new NavigationPage(new ErrorPage());
                            NavigationPage.SetHasNavigationBar(errorpage, false);
                            errorpage.BindingContext = new {Error = StatusCode, Joke = joke};
                            await Navigation.PushAsync(errorpage);
                        }
                        else if (responseWeb.StatusCode == HttpStatusCode.RequestTimeout)
                        {
                            var StatusCode = "ERROR 408: Request timed out";
                            var joke = "Our monkeys are sick. They are not able to work propperly.";
                            var errorpage = new NavigationPage(new ErrorPage());
                            NavigationPage.SetHasNavigationBar(errorpage, false);
                            errorpage.BindingContext = new {Error = StatusCode, Joke = joke};
                            await Navigation.PushAsync(errorpage);
                        }
                    }
                }
            }
            else
            {
                var StatusCode = "ERROR: No internet connection";
                var joke = "Our monkeys are internet addicted. NoW they don't have it.";
                var errorpage = new NavigationPage(new ErrorPage());
                NavigationPage.SetHasNavigationBar(errorpage, false);
                errorpage.BindingContext = new {Error = StatusCode, Joke = joke};
                await Navigation.PushAsync(errorpage);
            }
        }

        void Init()
        {
            //header styling
            Header.BackgroundColor = Constants.KleynGroupBg;
            Logo.HeightRequest = 40;

            Backbutton.HeightRequest = 30;
            Backbutton.HorizontalOptions = LayoutOptions.EndAndExpand;
            Backbutton.TextColor = Color.White;
            EditButton.HorizontalOptions = LayoutOptions.EndAndExpand;
            EditButton.TextColor = Color.White;

            //Content styling
            VehicleName.FontSize = 20;
            VehicleName.TextColor = Color.Black;
            VehicleImage.HorizontalOptions = LayoutOptions.FillAndExpand;
            VehicleImage.Aspect = Aspect.AspectFill;
            ViewMoreImgButton.BackgroundColor = Constants.KleynGroupTxt;
            ViewMoreImgButton.TextColor = Color.White;
            SlaveContentDetails.BackgroundColor = Color.FromRgb(206, 206, 206);

            StatusText.TextColor = Color.Black;

            StatusBinding.FontAttributes = FontAttributes.Bold;

            SlaveContentLocation.BackgroundColor = Color.FromRgb(206, 206, 206);
            H1Detail.FontSize = 17;
            H1Video.FontSize = 17;
            H1Location.FontSize = 17;

            //footer styling
            Footer.BackgroundColor = Constants.KleynGroupBg;
            LblCr.TextColor = Constants.MainTextColor;
            LblCr.HorizontalOptions = LayoutOptions.Center;
            LblCr.VerticalOptions = LayoutOptions.EndAndExpand;
            LblCr.VerticalTextAlignment = TextAlignment.Center;
            LblCr.HeightRequest = 80;

            // Vechile name styling
            VehicleName.FontSize = 16;
            Spinner.Color = Constants.KleynGroupTxt;

        }

        public Locatieitem GetPostion(string itemnummer)
        {
            var resultNumber = itemnummer;
            string webserviceUrl = "https://web.kleyn.com:1919/locatiesdev/location/" + resultNumber;
            try
            {
                var locRequest = WebRequest.Create(webserviceUrl);
                locRequest.Method = "GET";
                locRequest.Timeout = 12000;
                locRequest.ContentType = "application/json";
                locRequest.Headers.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");
                using (var s = locRequest.GetResponse().GetResponseStream())
                {
                    using (var sr = new StreamReader(s ?? throw new InvalidOperationException()))
                    {
                        var jsonResponse = sr.ReadToEnd();
                        var data = JsonConvert.DeserializeObject<Locatieitem>(jsonResponse);
                        return data;
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse responseWeb && responseWeb.StatusCode == HttpStatusCode.NotFound)
                {
                    var statusCode = responseWeb.StatusCode;
                    var errorpage = new NavigationPage(new ErrorPage());
                    NavigationPage.SetHasNavigationBar(errorpage, false);
                    errorpage.BindingContext = statusCode;

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
                                                            + " With Latitude: " + position.Latitude.ToString(CultureInfo.InvariantCulture) +
                                                            " And Longitude: " + position.Longitude.ToString(CultureInfo.InvariantCulture), "Yes",
                "No");
            if (answer)
            {
                await EditLocation(VehicleNameBinding.Text, position.Latitude.ToString(CultureInfo.InvariantCulture),
                    position.Longitude.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
            }
        }

        public async Task EditLocation(string itemnummer, string latitude, string longitude)
        {
            var webRequest =
                (HttpWebRequest) WebRequest.Create("https://web.kleyn.com:1919/locatiesdev/location/" + itemnummer);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add("Kleyn-Apikey", "{CA78D608-E9C7-4880-AB6A-B2DF6E0EE093}");

            using (var streamWritter = new StreamWriter(webRequest.GetRequestStream()))
            {
                var action = await DisplayActionSheet("Select Your current location terrain", "Cancel", null,
                    "Volteram France", "Dorderecht", "Trucks", "Vans",
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

                string latitudeLess = latitude.Replace(',', '.');
                string longitudeLess = longitude.Replace(',', '.');
                string date = JsonConvert.SerializeObject(DateTime.Now);
                string json = "{\"itemnummer\":" + itemnummer + ","
                              + "\"locatie\":" + Letter + "," +
                              "\"tijdstip\":" + date + ","
                              + "\"lengtegraad\":" + longitudeLess + ","
                              + "\"breedtegraad\":" + latitudeLess + ","
                              + "\"gebruiker\":\"Admin\"}";
                streamWritter.Write(json);
                streamWritter.Flush();
                streamWritter.Close();
            }

            var httpRepsone = (HttpWebResponse) webRequest.GetResponse();
            using (var streamReader = new StreamReader(httpRepsone.GetResponseStream() ?? throw new InvalidOperationException()))
            {
                streamReader.ReadToEnd();
                await Navigation.PushAsync(new DetailPage(VehicleNameBinding.Text));
            }
        }

        //(/◔ ◡ ◔)/
        private async void Location_Open_ClickedAsync(object sender, EventArgs e)
        {
            var item = GetPostion(VehicleNameBinding.Text);
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 0.01;
            var position = await locator.GetPositionAsync(TimeSpan.FromSeconds(0.01));
            var pinPostion = new Position(item.Breedtegraad, item.Lengtegraad);
            var pin = new Pin
            {
                Type = PinType.Place,
                Position = pinPostion,
                Label = VehicleName.Text,
                Address = VehicleNameBinding.Text
            };
            var stack = new StackLayout { Spacing = 0 };
            var map = new Map();
            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude),
                Distance.FromMiles(0.28)));
            map.MyLocationEnabled = true;
            map.UiSettings.MyLocationButtonEnabled = true;
            map.UiSettings.CompassEnabled = true;
            map.MyLocationButtonClicked += (data, arags) =>
            {
#pragma warning disable 618
                map.CameraChanged += async (info, args) =>
#pragma warning restore 618
                {
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
            var headerMaps = new StackLayout
            {
                BackgroundColor = Constants.KleynGroupBg,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start
            };
            //Footer
            var footerMaps = new StackLayout
            {
                BackgroundColor = Constants.KleynGroupBg,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End
            };
            //Make the Logo
            var logo = new Image
            {
                Source = "logo.png",
                Margin = new Thickness(5, 5, 0, 10),
                HorizontalOptions = LayoutOptions.Start,
                HeightRequest = 40
            };
            var editLocation = new Button
            {
                Text = "Move",
                Margin = new Thickness(0, -50, -40, 5),
                BackgroundColor = Color.Transparent,
                TextColor = Color.White,
                HeightRequest = 40,

            };
            var backButton = new Button
            {
                Text = "Back",
                Margin = new Thickness(0, -50, 0, 5),
                BackgroundColor = Color.Transparent,
                TextColor = Color.White,
                HeightRequest = 40,
                HorizontalOptions = LayoutOptions.End
            };
            backButton.Clicked += ReturnBackDetailPage;
            //Make the copyright label
            var copyRightLabel = new Label
            {
                Text = "Copyright © 2018, Kleyn Group",
                Margin = new Thickness(95, 0, 0, 0),
                TextColor = Constants.MainTextColor,
                VerticalOptions = LayoutOptions.EndAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                HeightRequest = 75,
            };
            editLocation.Clicked += AddLocation_Clicked;

            // Give the header the logo
            headerMaps.Children.Add(logo);
            var userDatabase = new UserDatabase();
            var userdata = userDatabase.GetAllUsers();
            var getael = userdata.First().Ael;
            var getadmin = userdata.First().IsAdmin;

            if (getael == "1" || getadmin == "1")
            {
                headerMaps.Children.Add(editLocation);
            }

            headerMaps.Children.Add(backButton);
            // Give the footer the Copy Right Label
            footerMaps.Children.Add(copyRightLabel);

            //Add the location pin to the map
            map.Pins.Add(pin);

            //Bind the footer/map/header togther 
            stack.Children.Add(headerMaps);
            stack.Children.Add(map);
            stack.Children.Add(footerMaps);
            Content = stack;

        }

    }

}

