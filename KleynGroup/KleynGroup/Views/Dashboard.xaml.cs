using KleynGroup.Models;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;
using Plugin.Vibrate;

namespace KleynGroup.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Dashboard
    {

        public Dashboard()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Init();
        }
       private async void ReturnBack(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Dashboard());
        }
        async void SignOut(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }

        private void Init()
        {
            Header.BackgroundColor = Constants.KleynGroupBg;
            Logo.HeightRequest = 40;

            Backbutton.HeightRequest = 30;
            Backbutton.HorizontalOptions = LayoutOptions.EndAndExpand;
            Backbutton.TextColor = Color.White;

            LblOr.FontSize = 35;
            LblOr.TextColor = Color.Black;

            BtnQr.Image = "QR.png";
            BtnQr.HeightRequest = 110;
            BtnQr.WidthRequest = 110;

            Footer.BackgroundColor = Constants.KleynGroupBg;
            LblCr.TextColor = Constants.MainTextColor;
            LblCr.HorizontalOptions = LayoutOptions.Center;
            LblCr.VerticalOptions = LayoutOptions.EndAndExpand;
            LblCr.VerticalTextAlignment = TextAlignment.Center;
            LblCr.HeightRequest = 80;

        }

        private void Handle_SearchButtonPressed(object sender, EventArgs e)
        {
            var keyword = SearchBar.Text;

            if (double.TryParse(keyword, out _))
            {
                var resultPlusText = keyword;
                var detailPage = new NavigationPage(new DetailPage(resultPlusText));
                NavigationPage.SetHasNavigationBar(detailPage, false);
                detailPage.BindingContext = resultPlusText;
                Navigation.PushAsync(detailPage);
            }
            else
            {
                DisplayAlert("ERROR", "Your search request is not a itemnumber. please use a itemnumber", "OKAY");
            }

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {

            // Create our custom overlay
            var customLayout = new StackLayout();
            var headerQr = new StackLayout
            {
                BackgroundColor = Constants.KleynGroupBg,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Start
        };
            var middleLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };
            var footerQr = new StackLayout
            {
                BackgroundColor = Constants.KleynGroupBg,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.End
            };
            var logo = new Image
            {
                Source = "logo.png",
                Margin = new Thickness(5, 5, 0, 10),
                HorizontalOptions = LayoutOptions.Start,
                HeightRequest = 40
            };
            var back = new Button
            {
                HeightRequest = 40,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                BackgroundColor = Color.Transparent,
                BorderColor = Color.Transparent,
                TextColor = Color.White,
                Text = "back",
                BorderWidth = 0,
                Margin = new Thickness(0,-50,5,0)
            };
            back.Clicked += ReturnBack;
            var copyRightLabel = new Label
            {
                Text = "Copyright © 2018, Kleyn Group",
                Margin = new Thickness(95, 0, 0, 0),
                TextColor = Constants.MainTextColor,
                VerticalOptions = LayoutOptions.EndAndExpand,
                VerticalTextAlignment = TextAlignment.Center,
                HeightRequest = 75,
            };

            // Give the header the logo
            headerQr.Children.Add(logo);
            headerQr.Children.Add(back);
            // Give the footer the Copy Right Label
            footerQr.Children.Add(copyRightLabel);
            //Create the layout for the Scan page
            customLayout.Children.Add(headerQr);
            customLayout.Children.Add(middleLayout);
            customLayout.Children.Add(footerQr);
            //Make a new scanner page and add the layout
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
                        char[] slashSeparator = new char[] { '/' };
                        var storage = result.Text.Split(slashSeparator, StringSplitOptions.None);
                        //Grab the last pice in the storage array {Itemnumber} Most of the time
                        var infoDetail = storage.Last();
                        // Check if the itemnumber contains only out of numbers
                        if (double.TryParse(infoDetail, out _))
                        {
                            // If so send the user to a new detailpage with the itemnumber
                            var resultPlusText = infoDetail;
                            var detailPage = new NavigationPage(new DetailPage(resultPlusText));
                            NavigationPage.SetHasNavigationBar(detailPage, false);
                            detailPage.BindingContext = resultPlusText;
                            await Navigation.PushAsync(detailPage);
                        }
                        //Else send a error back 
                        else
                        {
                            await DisplayAlert("ERROR", "The code doesn't have a valid item number", "oops!");
                        }
                    }
                    //If kleyn is not detected send a error back
                    else
                    {
                        await DisplayAlert("ERROR", "You are not scanning a kleyn QR code", "OK");
                    }

                });
            };
        }
    }
}
