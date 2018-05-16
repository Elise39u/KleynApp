using KleynGroup.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using KleynGroup.Data;
using Xamarin.Forms;

namespace KleynGroup
{
	public partial class App : Application
	{
	    static TokenDatabaseController tokenDatabase;
	    static UserDatabaseController userDatabase;
	    static RestServiceLogin RestServiceLogin;
        public static bool IsUserLoggedIn { get; set; }
        public App ()
		{
			InitializeComponent();

            if (!IsUserLoggedIn)
            {
                MainPage = new NavigationPage(new Dashboard());
            }
            else
            {
                MainPage = new NavigationPage(new Dashboard());
            }
        }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}

	    public static UserDatabaseController UserDatabase
	    {
	        get
	        {
	            if (userDatabase == null)
	            {
	                userDatabase = new UserDatabaseController();
	            }

	            return userDatabase;
	        }
	    }

	    public static TokenDatabaseController TokenDatabase
	    {
	        get
	        {
	            if (tokenDatabase == null)
	            {
	                tokenDatabase = new TokenDatabaseController();
	            }

	            return tokenDatabase;
	        }
	    }

	    public static RestServiceLogin RestserviceLogin
	    {
	        get
	        {
	            if (RestServiceLogin == null)
	            {
	                RestServiceLogin = new RestServiceLogin();
	            }
	            return RestServiceLogin;
            }
	    }

        public static int RecievedBytes { get; internal set; }
    }
}
