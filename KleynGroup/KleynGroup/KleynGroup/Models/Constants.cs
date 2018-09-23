using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Text;

namespace KleynGroup.Models
{
    //HERE ARE ALL STATIC INFORMATION 
    public class Constants
    { 
        public static string URL_LOGIN = "https://justin555.000webhostapp.com/api/login";
        public static string URL_REGISTER = "http://http://77.162.207.35";

        public static bool IsDev = true;

        public static Color KleynGroupBG = Color.FromRgb(2, 31, 110);
        public static Color KleynGroupTXT = Color.FromRgb(239, 118, 34);
        public static Color MainTextColor = Color.FromRgb(255, 255, 255);
        public static Color LoginEntryBgColor = Color.FromRgb(255, 255, 255);
        public static Color LoginEntryColor = Color.FromRgb(0, 0, 0);
        public static int LoginLogoHeight = 70;

        //Test account
        public static string Username = "kleynadmin";
        public static string Password = "password";

        // Google Urls
        public static string AppName = "Kleyn Park";
        public static string Scope = "https://www.googleapis.com/auth/userinfo.email";
        public static string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
        public static string AccessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
        public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";
    }
}
