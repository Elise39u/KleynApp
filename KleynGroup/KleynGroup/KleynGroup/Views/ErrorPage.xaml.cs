using KleynGroup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KleynGroup.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ErrorPage : ContentPage
	{
		public ErrorPage ()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent ();
            Init();
        }

        void Init()
        {
            Header.BackgroundColor = Constants.KleynGroupBG;
            Logo.HeightRequest = 40;
            Footer.BackgroundColor = Constants.KleynGroupBG;
            Lbl_CR.TextColor = Constants.MainTextColor;
            Lbl_CR.HorizontalOptions = LayoutOptions.Center;
            Lbl_CR.VerticalOptions = LayoutOptions.EndAndExpand;
            Lbl_CR.VerticalTextAlignment = TextAlignment.Center;
            Lbl_CR.HeightRequest = 80;

        }
    }
}