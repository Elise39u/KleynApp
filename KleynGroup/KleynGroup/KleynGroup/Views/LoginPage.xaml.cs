using KleynGroup.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Runtime.Serialization;
using System.Diagnostics;
using KleynGroup.Data;
using System.Timers;

namespace KleynGroup.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {

        public LoginPage()
        {

            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Init();
        }

        void Init()
        {
            BackgroundColor = Constants.KleynGroupBG;
                       Lbl_Logingin.FontAttributes = FontAttributes.Bold;

            LoginLogo.HeightRequest = Constants.LoginLogoHeight;

            ActivitySpinner.IsVisible = false;

            Entry_Username.BackgroundColor = Constants.LoginEntryBgColor;
            Entry_Username.TextColor = Constants.LoginEntryColor;

            Entry_Password.BackgroundColor = Constants.LoginEntryBgColor;
            Entry_Password.TextColor = Constants.LoginEntryColor;
            Entry_Password.IsPassword = true;
            Lbl_Logingin.FontAttributes = FontAttributes.Bold;
            Btn_Signin.BackgroundColor = Constants.KleynGroupTXT;
            Btn_Signin.TextColor = Constants.MainTextColor;


            //Btn_Register.BackgroundColor = Constants.KleynGroupTXT;
            //Btn_Register.TextColor = Constants.MainTextColor;

            Lbl_CR.TextColor = Constants.MainTextColor;
            Lbl_CR.HorizontalOptions = LayoutOptions.Center;
            Lbl_CR.VerticalOptions = LayoutOptions.EndAndExpand;


        }

        async void LoginRequest(object sender, EventArgs e)
        {


            var user = new User
            {
                Username = Entry_Username.Text,
                Password = Entry_Password.Text
            };
            if (CheckNetwork.IsInternet() == true)
            {
                if (user.Username == "" && user.Password == "")
                {
                    Lbl_Logingin.TextColor = Color.Red;
                    Lbl_Logingin.Text = "Please fill in the fields before logging in!";
                }
                else
                {
                    Lbl_Logingin.TextColor = Color.SpringGreen;
                    Lbl_Logingin.Text = "Logging in! please wait.....";
                    var result = await App.RestserviceLogin.Login(user);
                    if (result.access_token != null)
                    {

                        App.UserDatabase.SaveUser(user);
                        App.TokenDatabase.SaveToken(result);
                        await Navigation.PushAsync(new Dashboard());
                        Lbl_Logingin.Text = "";
                        Entry_Password.Text = "";
                    }
                    else
                    {
                        Lbl_Logingin.TextColor = Color.Red;

                        Lbl_Logingin.Text = "Invalid login information... please try again!";

                    }
                }
            }
            else
            {
                Lbl_Logingin.TextColor = Color.FromRgb(255, 0, 0);
                Lbl_Logingin.HorizontalTextAlignment = TextAlignment.Center;
                Lbl_Logingin.Text =
                    "OOPS! Please connect to a network first.";
                await Task.Delay(250);
                Lbl_Logingin.TextColor = Color.FromRgb(170, 0, 0);
                await Task.Delay(250);
                Lbl_Logingin.TextColor = Color.FromRgb(255, 0, 0);
                await Task.Delay(250);
                Lbl_Logingin.TextColor = Color.FromRgb(170, 0, 0);
                await Task.Delay(250);
                Lbl_Logingin.TextColor = Color.FromRgb(255, 0, 0);
                await Task.Delay(250);
                Lbl_Logingin.TextColor = Color.FromRgb(170, 0, 0);
                await Task.Delay(250);
                Lbl_Logingin.TextColor = Color.FromRgb(255, 0, 0);
                await Task.Delay(250);
                Lbl_Logingin.TextColor = Color.FromRgb(170, 0, 0);
                await Task.Delay(250);
                Lbl_Logingin.TextColor = Color.FromRgb(255, 0, 0);
                await Task.Delay(250);
                Lbl_Logingin.TextColor = Color.FromRgb(170, 0, 0);
                await Task.Delay(250);
                Lbl_Logingin.TextColor = Color.FromRgb(255, 0, 0);
                await Task.Delay(250);
                Lbl_Logingin.TextColor = Color.FromRgb(170, 0, 0);
                await Task.Delay(250);
                Lbl_Logingin.TextColor = Color.FromRgb(255, 0, 0);
                await Task.Delay(250);
                Lbl_Logingin.TextColor = Color.FromRgb(170, 0, 0);
                await Task.Delay(250);
                Lbl_Logingin.TextColor = Color.FromRgb(255, 0, 0);
                await Task.Delay(250);
                Lbl_Logingin.TextColor = Color.FromRgb(170, 0, 0);
                await Task.Delay(250);
                Lbl_Logingin.TextColor = Color.FromRgb(255, 0, 0);
                Lbl_Logingin.Text = "";
            }
        }
    }
}

