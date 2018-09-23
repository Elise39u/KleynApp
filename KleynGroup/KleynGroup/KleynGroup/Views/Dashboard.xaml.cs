using KleynGroup.Models;
using Plugin.Vibrate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KleynGroup.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace KleynGroup.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Dashboard : ContentPage
    {
        List<string> vehicles = new List<string>
        {
            "11111145445", "111111454576", "1111118676", "1111115647"
        };

        public Dashboard()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Init();

            Backbutton.Clicked += async (sender, args) =>
            {
                var LoginPage = new NavigationPage(new LoginPage());
                NavigationPage.SetHasNavigationBar(LoginPage, false);
                await Navigation.PushAsync(LoginPage);
            };
        }

        void Init()
        {
            Backbutton.HorizontalOptions = LayoutOptions.EndAndExpand;

            Header.BackgroundColor = Constants.KleynGroupBG;
            Logo.HeightRequest = 40;

            Lbl_Or.FontSize = 35;
            Lbl_Or.TextColor = Color.Black;

            Btn_Qr.Image = "QR.png";
            Btn_Qr.HeightRequest = 110;
            Btn_Qr.WidthRequest = 110;

            Footer.BackgroundColor = Constants.KleynGroupBG;
            Lbl_CR.TextColor = Constants.MainTextColor;
            Lbl_CR.HorizontalOptions = LayoutOptions.Center;
            Lbl_CR.VerticalOptions = LayoutOptions.EndAndExpand;
            Lbl_CR.VerticalTextAlignment = TextAlignment.Center;
            Lbl_CR.HeightRequest = 80;

        }

        void Handle_SearchButtonPressed(object sender, System.EventArgs e)
        {
            var keyword = SearchBar.Text;

            double num;
            if (double.TryParse(keyword, out num))
            {
                var ResultPlusText = keyword;
                var DetailPage = new DetailPage(ResultPlusText);
                NavigationPage.SetHasNavigationBar(DetailPage, false);
                DetailPage.BindingContext = ResultPlusText;
                Navigation.PushAsync(DetailPage);
            }
            else
            {
                DisplayAlert("ERROR", "Your search request is not a itemnumber. please use a itemnumber", "OKAY");
            }

        }

        private async void Button_Clicked(object sender, EventArgs e)
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
                // Create our custom overlay
                var customLayout = new StackLayout { };
            var header_qr = new StackLayout
            {
                BackgroundColor = Constants.KleynGroupBG,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start
            };
            var middleLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            var footer_qr = new StackLayout
            {
                BackgroundColor = Constants.KleynGroupBG,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End
            };
            var Logo = new Image
            {
                Source = "LoginLogo.png",
                Margin = new Thickness(5, 5, 0, 10),
                HorizontalOptions = LayoutOptions.Start,
                HeightRequest = 40
            };
            var CopyRightLabel = new Label
            {
                Text = "Copyright © 2018, Kleyn Group",
                Margin = new Thickness(95, 0, 0, 0),
                TextColor = Constants.MainTextColor,
                VerticalOptions = LayoutOptions.EndAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                HeightRequest = 75,
            };

            // Give the header the logo
            header_qr.Children.Add(Logo);
            // Give the footer the Copy Right Label
            footer_qr.Children.Add(CopyRightLabel);
            //Create the layout for the Scan page
            customLayout.Children.Add(header_qr);
            customLayout.Children.Add(middleLayout);
            customLayout.Children.Add(footer_qr);
            /// Make a new scanner page and add the layout
            var scan = new ZXingScannerPage(customOverlay: customLayout);
            NavigationPage.SetHasNavigationBar(scan, false);
            //Push the Scan page as first in the Navigation row
            await Navigation.PushAsync(scan);

            //Handel te qr code result
            scan.OnScanResult += (result) =>
            {
                // Start to sync the result of the qr scanner
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var vibrate = CrossVibrate.Current;
                    vibrate.Vibration(TimeSpan.FromSeconds(0.25));
                    await Navigation.PopAsync();
                    //Check iff the qrcode contains Kleyn
                    if (result.Text.Contains("kleyn"))
                    {

                            //Slice the qrcode on Slahs
                            char[] slashSeparator = new char[] {'/'};
                            var storage = result.Text.Split(slashSeparator, StringSplitOptions.None);
                            //Grab the last pice in the storage array {Itemnumber} Most of the time
                            var InfoDetail = storage.Last();
                            
                            // Check if the itemnumber contains only out of numbers
                            double num;
                            if (double.TryParse(InfoDetail, out num))
                            {
                                // If so send the user to a new detailpage with the itemnumber
                                var ResultPlusText = InfoDetail;
                                var DetailPage = new DetailPage(ResultPlusText);
                                NavigationPage.SetHasNavigationBar(DetailPage, false);
                                DetailPage.BindingContext = ResultPlusText;
                                await Navigation.PushAsync(DetailPage);
                            }
                            //Else send a error back 
                            else
                            {
                                DisplayAlert("ERROR", "The code doesn't have a valid item number", "oops!");
                            }
                    } 
                    //If kleyn is not detected send a error back
                    else
                    {
                        DisplayAlert("ERROR", "You are not scanning a kleyn QR code", "OK");
                    }
                        
                });
            };
        }
    }
}
