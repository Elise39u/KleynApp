using KleynGroup.Views;
using KleynGroup.Data;
using Xamarin.Forms;

namespace KleynGroup
{
	public partial class App
	{
	    static TokenDatabaseController _tokenDatabase;
	    private static RestServiceLogin _restServiceLogin;

        public App ()
		{
			InitializeComponent();
            MainPage = new NavigationPage(new LoginPage());
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

	    public static TokenDatabaseController TokenDatabase
	    {
	        get
	        {
	            if (_tokenDatabase == null)
	            {
	                _tokenDatabase = new TokenDatabaseController();
	            }

	            return _tokenDatabase;
	        }
	    }

	    public static RestServiceLogin RestserviceLogin
	    {
	        get
	        {
	            if (_restServiceLogin == null)
	            {
	                _restServiceLogin = new RestServiceLogin();
	            }
	            return _restServiceLogin;
            }
	    }

    }
}
