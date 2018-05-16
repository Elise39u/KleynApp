using KleynGroup.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using KleynGroup.Data;


namespace KleynGroup.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage
    {

        public LoginPage()
        {

            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Init();
        }

        void Init()
        {
            BackgroundColor = Constants.KleynGroupBg;
                       LblLogingin.FontAttributes = FontAttributes.Bold;

            LoginLogo.HeightRequest = Constants.LoginLogoHeight;

            ActivitySpinner.IsVisible = false;

            EntryUsername.BackgroundColor = Constants.LoginEntryBgColor;
            EntryUsername.TextColor = Constants.LoginEntryColor;

            EntryPassword.BackgroundColor = Constants.LoginEntryBgColor;
            EntryPassword.TextColor = Constants.LoginEntryColor;
            EntryPassword.IsPassword = true;
            LblLogingin.FontAttributes = FontAttributes.Bold;
            BtnSignin.BackgroundColor = Constants.KleynGroupTxt;
            BtnSignin.TextColor = Constants.MainTextColor;
            BtnForgot.BackgroundColor = Constants.KleynGroupTxt;
            BtnForgot.TextColor = Constants.MainTextColor;
            LblEdition.TextColor = Constants.MainTextColor;

            //Btn_Register.BackgroundColor = Constants.KleynGroupTXT;
            //Btn_Register.TextColor = Constants.MainTextColor;

            LblCr.TextColor = Constants.MainTextColor;
            LblCr.HorizontalOptions = LayoutOptions.Center;
            LblCr.VerticalOptions = LayoutOptions.EndAndExpand;


        }

        async void LoginRequest(object sender, EventArgs e)
        {

            if (CheckNetwork.IsInternet())
            {
                var user = new User
                {
                    Username = EntryUsername.Text,
                    Password = EntryPassword.Text
                };
 
                if (user.Username == "" || user.Password == "")
                {
                    LblLogingin.TextColor = Color.Red;
                    LblLogingin.Text = "Please fill in the fields before logging in!";
                }
                else
                {
                    LblLogingin.TextColor = Color.SpringGreen;
                    LblLogingin.Text = "Logging in! please wait.....";
                    var result = await App.RestserviceLogin.Login(user);
                    var dbclear = new UserDatabase();
                    dbclear.Droptable();
                    if (result.AccessToken != null)
                    {
                        if (result.IsFrozen == "0")
                        {
                            var userDatabase = new UserDatabase();
                            userDatabase.AddUser(result);
                            await Navigation.PushAsync(new Dashboard());
                            LblLogingin.Text = "";
                            EntryPassword.Text = "";
                        }
                        else
                        {
                            LblLogingin.TextColor = Color.Red;

                            LblLogingin.Text = "Your account has been deactivated. Please contact a administrator";
                        }
                        
                    }
                    else
                    {
                        LblLogingin.TextColor = Color.Red;

                        LblLogingin.Text = "Invalid login information... please try again!";

                    }
                }
            }
            else
            {
                LblLogingin.TextColor = Color.FromRgb(255, 0, 0);
                LblLogingin.HorizontalTextAlignment = TextAlignment.Center;
                LblLogingin.Text =
                    "OOPS! Please connect to a network first.";
                await Task.Delay(250);
                LblLogingin.TextColor = Color.FromRgb(170, 0, 0);
                await Task.Delay(250);
                LblLogingin.TextColor = Color.FromRgb(255, 0, 0);
                await Task.Delay(250);
                LblLogingin.TextColor = Color.FromRgb(170, 0, 0);
                await Task.Delay(250);
                LblLogingin.TextColor = Color.FromRgb(255, 0, 0);
                await Task.Delay(250);
                LblLogingin.TextColor = Color.FromRgb(170, 0, 0);
                await Task.Delay(250);
                LblLogingin.TextColor = Color.FromRgb(255, 0, 0);
                await Task.Delay(250);
                LblLogingin.TextColor = Color.FromRgb(170, 0, 0);
                await Task.Delay(250);
                LblLogingin.TextColor = Color.FromRgb(255, 0, 0);
                await Task.Delay(250);
                LblLogingin.TextColor = Color.FromRgb(170, 0, 0);
                await Task.Delay(250);
                LblLogingin.TextColor = Color.FromRgb(255, 0, 0);
                await Task.Delay(250);
                LblLogingin.TextColor = Color.FromRgb(170, 0, 0);
                await Task.Delay(250);
                LblLogingin.TextColor = Color.FromRgb(255, 0, 0);
                await Task.Delay(250);
                LblLogingin.TextColor = Color.FromRgb(170, 0, 0);
                await Task.Delay(250);
                LblLogingin.TextColor = Color.FromRgb(255, 0, 0);
                await Task.Delay(250);
                LblLogingin.TextColor = Color.FromRgb(170, 0, 0);
                await Task.Delay(250);
                LblLogingin.TextColor = Color.FromRgb(255, 0, 0);
                LblLogingin.Text = "";
            }
        }
        private void ForgotPassword(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("http://kleynpark.nl/resetpw"));
        }
    }   
}

