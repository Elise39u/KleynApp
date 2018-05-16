using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Xamarin.Forms;
using View = Android.Views.View;

namespace KleynGroup.Droid
{
    [Activity(Label = "KleynGroup", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity, View.IOnSystemUiVisibilityChangeListener
    {
         public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        int _uiOptions;
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            Window.AddFlags(WindowManagerFlags.Fullscreen); // hide the status bar

            _uiOptions = (int)Window.DecorView.SystemUiVisibility;

            _uiOptions |= (int)SystemUiFlags.LowProfile;
            _uiOptions |= (int)SystemUiFlags.Fullscreen;
            _uiOptions |= (int)SystemUiFlags.HideNavigation;
            _uiOptions |= (int)SystemUiFlags.ImmersiveSticky;

            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)_uiOptions;

            Window.DecorView.SetOnSystemUiVisibilityChangeListener(this);

            Forms.Init(this, bundle);
            LoadApplication(new App());
            Xamarin.FormsMaps.Init(this, bundle);
            Xamarin.FormsGoogleMaps.Init(this, bundle);
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity = this;
        }
        public void OnSystemUiVisibilityChange([GeneratedEnum] StatusBarVisibility visibility)
        {
            if (((int)visibility & (int)SystemUiFlags.Fullscreen) == 0)
            {
                Window.DecorView.SystemUiVisibility = (StatusBarVisibility)_uiOptions;
            }

        }
        public override void OnWindowFocusChanged(bool hasFocus)
        {
            base.OnWindowFocusChanged(hasFocus);
            Window.DecorView.SystemUiVisibility = (StatusBarVisibility)((int)SystemUiFlags.Fullscreen
                                                                        | (int)SystemUiFlags.ImmersiveSticky
                                                                        | (int)SystemUiFlags.LayoutHideNavigation
                                                                        | (int)SystemUiFlags.LayoutStable
                                                                        | (int)SystemUiFlags.HideNavigation);
        }
        public override void OnBackPressed()
        {
        }
    }
}

