using KleynGroup.Models;
using KleynGroup.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.IO;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using Plugin.Media;
using Plugin.Media.Abstractions;
using FFImageLoading.Forms;
using FFImageLoading;
using FFImageLoading.Cache;

namespace KleynGroup.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImagesPage : ContentPage
    {

        public ImagesPage(List<PictureInfo> pictureInfo, string itemnummer)
        {
            this.IsBusy = false;
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Init();
            var Test = GetImages(pictureInfo, itemnummer);
        }

        public async Task GetImages(List<PictureInfo> pictureInfo, string itemnummer)
        {
            //await ImageService.Instance.InvalidateCacheAsync(CacheType.All);
            var byteConverter = new ByteConverterController();
            for (int i = 0; i < pictureInfo.Count(); ++i)
            {
                this.IsBusy = true;
                var Sources = await byteConverter.DownloadImage(itemnummer, pictureInfo[i].typecode);
                var img = new CachedImage
                {
                    Source = ImageSource.FromStream(() => new MemoryStream(Sources)),
                    CacheDuration = TimeSpan.FromSeconds(-1),
                };
                //img.Scale = 0.75;
                var LabelName = new Label
                {
                    Text = pictureInfo[i].label,
                    HorizontalOptions = LayoutOptions.Start,
                };
                var LabelType = new Label
                {
                    Text = pictureInfo[i].typecode,
                    HorizontalOptions = LayoutOptions.EndAndExpand
                };
                var ActionButton = new Button
                {
                    Text = "Choose a action",
                    BackgroundColor = Constants.KleynGroupTXT,
                    TextColor = Constants.LoginEntryBgColor
                };
                MasterContent.Children.Add(LabelName);
                MasterContent.Children.Add(LabelType);
                MasterContent.Children.Add(img);
                MasterContent.Children.Add(ActionButton);

                var TypeCode = pictureInfo[i].typecode;
                ActionButton.Clicked += async (sender, args) =>
                {
                    var action = await DisplayActionSheet("Select What you want to do with the image", "Cancel", null, "Upload Image",
                        "Delete Image", "Edit Image");

                    switch (action)
                    {
                        case "Upload Image":
                            var Uploadanswer = await DisplayAlert("Upload Choosed", "Do you want to pick a photo from the gallery or one from the camera", "Camera", "Gallery");
                            if (Uploadanswer)
                            {
                                BtnCamera_ClickedAsync(itemnummer, TypeCode, "POST");
                            }
                            else
                            {
                                BtnPickPhoto_CickedAsync(itemnummer, TypeCode, "POST");
                            }
                            break;
                        case "Delete Image":
                            var Deleted = new ByteConverterController();
                            var removeImage = Deleted.DeleteImage(itemnummer, TypeCode);
                            var DashBoard = new NavigationPage(new DetailPage(itemnummer));
                            NavigationPage.SetHasNavigationBar(DashBoard, false);

                            await Navigation.PushAsync(DashBoard);
                            break;
                        case "Edit Image":
                            var Editanswer = await DisplayAlert("Edit Choosed", "Do you want to pick a photo from the gallery or one from the camera", "Camera", "Gallery");
                            if (Editanswer)
                            {
                                BtnCamera_ClickedAsync(itemnummer, TypeCode, "PUT");
                            }
                            else
                            {
                                BtnPickPhoto_CickedAsync(itemnummer, TypeCode, "PUT");
                            }
                            break;
                        default:
                            await DisplayAlert("Nothing Choosed", "Nothing has been chosen", "Abort");
                            return;
                    };
                };
                this.IsBusy = false;
            }
        }
                

        void Init()
        {
            //header styling
            Header.BackgroundColor = Constants.KleynGroupBG;
            Logo.HeightRequest = 40;

            //footer styling
            Footer.BackgroundColor = Constants.KleynGroupBG;
            Lbl_CR.TextColor = Constants.MainTextColor;
            Lbl_CR.HorizontalOptions = LayoutOptions.Center;
            Lbl_CR.VerticalOptions = LayoutOptions.EndAndExpand;
            Lbl_CR.VerticalTextAlignment = TextAlignment.Center;
            Lbl_CR.HeightRequest = 80;

            //Activty Spinner
            Spinner.Color = Constants.KleynGroupTXT;
        }


        private async void BtnPickPhoto_CickedAsync(string itemnummer, string typecode, string method)
        {
            var byteConverter = new ByteConverterController();
            // Set up the libarys
            await CrossMedia.Current.Initialize();

            //Check if the user can pick a photo 
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                return;
            }

            //Wait to the user picked a photo and put in this var
            var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                //Set the size to medium
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium
            });

            //If file == null than return with nothing
            if (file == null)
                return;

            //var TypePicker = "TSE";
            if (method == "POST" || method == "post")
            {
                var answer = await DisplayAlert("Change Image?", "Are you sure you want to upload this image", "Yes", "cancel");
                if (answer)
                {
                    var typeCodeChoice = await DisplayActionSheet("What kind of picture is this", "Abort", null, "Head picture Left front",
                            "Head picture Right Front", "Head picture left behind", "Head picture right behind", "Interior", "Dashboard",
                            "Specials", "container", "Combination", "Double cabie/cabin", "Radio/Cd player", "Books", "Keys", "Air conditioning",
                            "Under the Hood", "Entry front", "Entry behind", "Cool engine", "Km. Stand", "Chassisno", "Construction sign",
                            "Front of the vehicle", "Back of the vehicle", "Licence plate", "Test image");
                    switch (typeCodeChoice)
                    {
                        case "Head picture Left front":
                            typeCodeChoice = "HLV";
                            break;
                        case "Head picture Right Front":
                            typeCodeChoice = "HRV";
                            break;
                        case "Head picture left behind":
                            typeCodeChoice = "HLA";
                            break;
                        case "Head picture right behind":
                            typeCodeChoice = "HRA";
                            break;
                        case "Interior":
                            typeCodeChoice = "HIN";
                            break;
                        case "Dashboard":
                            typeCodeChoice = "HDA";
                            break;
                        case "Specials":
                            typeCodeChoice = "FSP";
                            break;
                        case "container":
                            typeCodeChoice = "FLB";
                            break;
                        case "Combination":
                            typeCodeChoice = "COM";
                            break;
                        case "Double cabie/cabin":
                            typeCodeChoice = "FDC";
                            break;
                        case "Radio/Cd player":
                            typeCodeChoice = "FRC";
                            break;
                        case "Books":
                            typeCodeChoice = "FBK";
                            break;
                        case "Keys":
                            typeCodeChoice = "FSL";
                            break;
                        case "Air conditioning":
                            typeCodeChoice = "AIR";
                            break;
                        case "Under the Hood":
                            typeCodeChoice = "MOT";
                            break;
                        case "Entry front":
                            typeCodeChoice = "FB1";
                            break;
                        case "Entry behind":
                            typeCodeChoice = "FB2";
                            break;
                        case "Cool engine":
                            typeCodeChoice = "FKL";
                            break;
                        case "Km. Stand":
                            typeCodeChoice = "FKM";
                            break;
                        case "Chassisno":
                            typeCodeChoice = "FCH";
                            break;
                        case "Construction sign":
                            typeCodeChoice = "CPL";
                            break;
                        case "Front of the vehicle":
                            typeCodeChoice = "FVO";
                            break;
                        case "Back of the vehicle":
                            typeCodeChoice = "FAC";
                            break;
                        case "Licence plate":
                            typeCodeChoice = "FAC";
                            break;
                        case "Test image":
                            typeCodeChoice = "TSE";
                            break;
                        default:
                            await DisplayAlert("Canceled", "Nothing has been chosen", "Abort");
                            return;
                    }
                    var DoCall = byteConverter.ChangeUploadImage(typeCodeChoice, itemnummer, method, file);
                    var DashBoard = new NavigationPage(new DetailPage(itemnummer));
                    NavigationPage.SetHasNavigationBar(DashBoard, false);

                    await Navigation.PushAsync(DashBoard);
                }
                else
                {
                    await DisplayAlert("Abort", "Cancel has been chosen", "abort");
                    return;
                }
            }
            else
            {
                var answer = await DisplayAlert("Change Image?", "Are you sure you want to edit this image", "Yes", "cancel");
                if (answer)
                {
                    var DoCall = byteConverter.ChangeUploadImage(typecode, itemnummer, method, file);
                    var DashBoard = new NavigationPage(new DetailPage(itemnummer));
                    NavigationPage.SetHasNavigationBar(DashBoard, false);

                    await Navigation.PushAsync(DashBoard);
                }
                else
                {
                    await DisplayAlert("Abort", "Cancel has been chosen", "abort");
                    return;
                }

            }
        }

        private async void BtnCamera_ClickedAsync(string itemnummer, string typecode, string method)
        {
            var byteConverter = new ByteConverterController();
            // Set up the libarys
            await CrossMedia.Current.Initialize();

            //Check if the device can use the Camera or a photo maker is supported
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                // True send a error message back with no camera avaialble
                await DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                return;
            }

            //False go on with the code and wait for the user to take a picture
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                //Set the size to medium
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                /*Save the photo for now in the app directory VB: (/storage/emulated/0/Android/data/App.packagename -> com.companyname.KleynGroup/
                 files/pictures/sample/filename
                  */
                Directory = "Sample",
                Name = "test.jpg"
            });

            // If the user stops the camera or something goes wrong return nothing
            if (file == null)
                return;

            //var TypePicker = "TSE";
            if (method == "POST" || method == "post")
            {
                var answer = await DisplayAlert("Change Image?", "Are you sure you want to upload this image", "Yes", "cancel");
                if (answer)
                {
                    var typeCodeChoice = await DisplayActionSheet("What kind of picture is this", "Abort", null, "Head picture Left front",
                            "Head picture Right Front", "Head picture left behind", "Head picture right behind", "Interior", "Dashboard",
                            "Specials", "container", "Combination", "Double cabie/cabin", "Radio/Cd player", "Books", "Keys", "Air conditioning",
                            "Under the Hood", "Entry front", "Entry behind", "Cool engine", "Km. Stand", "Chassisno", "Construction sign",
                            "Front of the vehicle", "Back of the vehicle", "Licence plate", "Test image");
                    switch (typeCodeChoice)
                    {
                        case "Head picture Left front":
                            typeCodeChoice = "HLV";
                            break;
                        case "Head picture Right Front":
                            typeCodeChoice = "HRV";
                            break;
                        case "Head picture left behind":
                            typeCodeChoice = "HLA";
                            break;
                        case "Head picture right behind":
                            typeCodeChoice = "HRA";
                            break;
                        case "Interior":
                            typeCodeChoice = "HIN";
                            break;
                        case "Dashboard":
                            typeCodeChoice = "HDA";
                            break;
                        case "Specials":
                            typeCodeChoice = "FSP";
                            break;
                        case "container":
                            typeCodeChoice = "FLB";
                            break;
                        case "Combination":
                            typeCodeChoice = "COM";
                            break;
                        case "Double cabie/cabin":
                            typeCodeChoice = "FDC";
                            break;
                        case "Radio/Cd player":
                            typeCodeChoice = "FRC";
                            break;
                        case "Books":
                            typeCodeChoice = "FBK";
                            break;
                        case "Keys":
                            typeCodeChoice = "FSL";
                            break;
                        case "Air conditioning":
                            typeCodeChoice = "AIR";
                            break;
                        case "Under the Hood":
                            typeCodeChoice = "MOT";
                            break;
                        case "Entry front":
                            typeCodeChoice = "FB1";
                            break;
                        case "Entry behind":
                            typeCodeChoice = "FB2";
                            break;
                        case "Cool engine":
                            typeCodeChoice = "FKL";
                            break;
                        case "Km. Stand":
                            typeCodeChoice = "FKM";
                            break;
                        case "Chassisno":
                            typeCodeChoice = "FCH";
                            break;
                        case "Construction sign":
                            typeCodeChoice = "CPL";
                            break;
                        case "Front of the vehicle":
                            typeCodeChoice = "FVO";
                            break;
                        case "Back of the vehicle":
                            typeCodeChoice = "FAC";
                            break;
                        case "Licence plate":
                            typeCodeChoice = "FAC";
                            break;
                        case "Test image":
                            typeCodeChoice = "TSE";
                            break;
                        default:
                            await DisplayAlert("Canceled", "Nothing has been chosen", "Abort");
                            return;
                    }
                    var DoCall = byteConverter.ChangeUploadImage(typeCodeChoice, itemnummer, method, file);
                    var DashBoard = new NavigationPage(new DetailPage(itemnummer));
                    NavigationPage.SetHasNavigationBar(DashBoard, false);
                    await Navigation.PushAsync(DashBoard);
                }
                else
                {
                    await DisplayAlert("Abort", "Cancel has been chosen", "abort");
                    return;
                }
            } else
            {
                var answer = await DisplayAlert("Change Image?", "Are you sure you want to edit this image", "Yes", "cancel");
                if (answer)
                {
                    var DoCall = byteConverter.ChangeUploadImage(typecode, itemnummer, method, file);
                    var DashBoard = new NavigationPage(new DetailPage(itemnummer));
                    NavigationPage.SetHasNavigationBar(DashBoard, false);

                    await Navigation.PushAsync(DashBoard);
                }
                else
                {
                    await DisplayAlert("Abort", "Cancel has been chosen", "abort");
                    return;
                }

            }
        }
    }
}