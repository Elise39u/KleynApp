using KleynGroup.Models;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KleynGroup.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ErrorPage
    {

         public ErrorPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            Init();
        }

        void Init()
        {
            Header.BackgroundColor = Constants.KleynGroupBg;
            Logo.HeightRequest = 40;

            OopsTxt.TextColor = Color.Red;
            OopsTxt.FontSize = 20;
            OopsTxt.HorizontalOptions = LayoutOptions.Center;
            OopsTxt.VerticalOptions = LayoutOptions.Center;
            MonkeyTxt.HorizontalOptions = LayoutOptions.CenterAndExpand;
            MonkeyTxt.FontSize = 12;
            StatusCode.HorizontalOptions = LayoutOptions.Center;

            Backbutton.BackgroundColor = Constants.KleynGroupTxt;
            Backbutton.TextColor = Color.White;
            Footer.BackgroundColor = Constants.KleynGroupBg;
            LblCr.TextColor = Constants.MainTextColor;
            LblCr.HorizontalOptions = LayoutOptions.Center;
            LblCr.VerticalOptions = LayoutOptions.EndAndExpand;
            LblCr.VerticalTextAlignment = TextAlignment.Center;
            LblCr.HeightRequest = 80;

        }
        async void ReturnBack(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
            await Navigation.PushAsync(new Dashboard());
        }

        
    }
}