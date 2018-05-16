using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Text;

namespace KleynGroup.Models
{
    //HERE ARE ALL STATIC INFORMATION 
    public class Constants
    { 
        public static string URL_LOGIN = "http://192.168.2.12:45455/token";
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
    }
}
