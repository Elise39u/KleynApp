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
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace KleynGroup.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImagesPage : ContentPage
    {
        public string itemnumber { get; set; }

        public ImagesPage(List<PictureInfo> pictureInfo, string itemnummer)
        {
            //Set the Activity Spinner to not shown
            this.IsBusy = false;
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            Init();
            //Get all the images from the api
            var NewImage = GetImages(pictureInfo, itemnummer);
            Return.Clicked += async (sender, args) =>
            {
                await Navigation.PopAsync(true);
            };
            itemnumber = itemnummer;
        }

        public async Task GetImages(List<PictureInfo> pictureInfo, string itemnummer)
        {
            //Make a new ByteConvertController
            var byteConverter = new ByteConverterController();
            //For each element in pictureInfo retrive the image
            for (int i = 0; i < pictureInfo.Count(); ++i)
            {
                //Show the Activity spinner
                this.IsBusy = true;
                //Get the image source from the api
                var Sources = await byteConverter.DownloadImage(itemnummer, pictureInfo[i].typecode);
                //Make a new image
                var img = new Image
                {
                    //Set the source to what we got from the api
                    Source = ImageSource.FromStream(() => new MemoryStream(Sources)),
                };
                //Make a new label
                var LabelType = new Label
                {
                    Text = pictureInfo[i].label + " - " + pictureInfo[i].typecode,
                    HorizontalOptions = LayoutOptions.StartAndExpand
                };
                //Add all the elements to the image page
                MasterContent.Children.Add(LabelType);
                MasterContent.Children.Add(img);
                //Make a new button the upload/change the image  
                UserDatabaseController UserData = new UserDatabaseController();
                var Userinfo = UserData.GetAllUsers();
                if (Userinfo[0].picturePermisson == 1)
                {
                    var ActionButton = new Button
                    {
                        Text = "Choose an action",
                        BackgroundColor = Constants.KleynGroupTXT,
                        TextColor = Constants.LoginEntryBgColor
                    };
                    MasterContent.Children.Add(ActionButton);
                    //Make a new var with the type code 
                    var TypeCode = pictureInfo[i].typecode;
                    // Check if the user pressed one of the Buttons
                    ActionButton.Clicked += async (sender, args) =>
                    {
                        // If so ask the user what he wants to do with the image
                        var action = await DisplayActionSheet("Select What you want to do with the image", "Cancel", null,
                            "Delete Image", "Edit Image");

                        //Make a new switch 
                        switch (action)
                        {
                            //User pressed Delete
                            case "Delete Image":
                                //Make a new ByteController
                                var Deleted = new ByteConverterController();
                                // Do the delete request
                                var removeImage = Deleted.DeleteImage(itemnummer, TypeCode);
                                //Send the user back to the detail page
                                Navigation.InsertPageBefore(new DetailPage(itemnummer), this);
                                await Navigation.PopAsync(true);
                                break;
                            // User pressed Edit 
                            case "Edit Image":
                                // Ask if the user wants to choose a image from the gallery or take one with the camera
                                var Editanswer = await DisplayActionSheet("Edit Choosed, Do you want to pick a photo from the gallery or one from the camera", "Abort", null, "Camera", "Gallery");
                                // If the user pressed Camera open the camera
                                switch (Editanswer)
                                {
                                    case "Camera":
                                        //Send the user to the camera and go on to the next function
                                        BtnCamera_ClickedAsync(itemnummer, TypeCode, "PUT");
                                        break;
                                    case "Gallery":
                                        //Send the user to his/her Gallery and go on to the next function
                                        BtnPickPhoto_CickedAsync(itemnummer, TypeCode, "PUT");
                                        break;
                                    default:
                                        await DisplayAlert("Action Canceld", "Action has been aborted", "Okay");
                                        break;
                                }
                                break;
                            //if the user pressed canceld or press some where else on the screen cancel the question
                            default:
                                //Show the user he/she canceld it
                                await DisplayAlert("Nothing Choosed", "Nothing has been chosen", "Abort");
                                return;
                        };
                    };
                }
                // Hide the Activity spinner
                this.IsBusy = false;
            }
        }
                

        void Init()
        {
            //header styling
            Header.BackgroundColor = Constants.KleynGroupBG;
            Logo.HeightRequest = 40;


            Return.HorizontalOptions = LayoutOptions.EndAndExpand;

            UserDatabaseController UserData = new UserDatabaseController();
            var Userinfo = UserData.GetAllUsers();
            if (Userinfo[0].picturePermisson == 1)
            {
                //Upload Button Style
                UploadButton.BackgroundColor = Constants.KleynGroupTXT;
                UploadButton.TextColor = Constants.LoginEntryBgColor;
            } else
            {
                //Upload Button Style
                UploadButton.BackgroundColor = Constants.KleynGroupTXT;
                UploadButton.TextColor = Constants.LoginEntryBgColor;
                UploadButton.IsVisible = false;
            }

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
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
            if (status != PermissionStatus.Unknown)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                //Best practice to always check that the key exists
                if (results.ContainsKey(Permission.Storage))
                {
                    status = results[Permission.Storage];
                }

                if (status == PermissionStatus.Granted)
                {

                }
                else if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Storage Denied", "Can not continue, try again.", "OK");
                    return;
                }
            }
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

            //Take action to upload/Change the image 
            await ImageAction(itemnummer, typecode, method, file);
        }

        private async void BtnCamera_ClickedAsync(string itemnummer, string typecode, string method)
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if (status != PermissionStatus.Unknown)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                //Best practice to always check that the key exists
                if (results.ContainsKey(Permission.Camera))
                {
                    status = results[Permission.Camera];
                }

                if (status == PermissionStatus.Granted)
                {
                }
                else if (status != PermissionStatus.Unknown)
                {
                    await DisplayAlert("Camera Denied", "Can not continue, try again.", "OK");
                    return;
                }
            }
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

            //Take action to upload/Change the image
            await ImageAction(itemnummer, typecode, method, file);
        }

        public async Task ImageAction(string itemnummer, string typecode, string method, MediaFile file)
        {
            //Make a new var ByteConverterController
            var byteConverter = new ByteConverterController();
            //Check if method is post or put
            if (method == "POST" || method == "post")
            {
                //If the method is post continue and ask the user if they want to upload the image
                var answer = await DisplayAlert("Change Image?", "Are you sure you want to upload this image", "Yes", "cancel");
                //If the user pressed yes
                if (answer)
                {
                    //Ask the user what kind of picture this is?
                    var typeCodeChoice = await DisplayActionSheet("What kind of picture is this", "Abort", null, "Head picture Left front",
                            "Head picture Right Front", "Head picture left behind", "Head picture right behind", "Interior", "Dashboard",
                            "Specials", "container", "Combination", "Double cabie/cabin", "Radio/Cd player", "Books", "Keys", "Air conditioning",
                            "Under the Hood", "Entry front", "Entry behind", "Cool engine", "Km. Stand", "Chassisno", "Construction sign",
                            "Front of the vehicle", "Back of the vehicle", "Licence plate");
                    //Check what the user has pressed and choose the typecode
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
                            //If the user pressed cancel or some where on the screen cancel the request
                        default:
                            // Let the user know he canceled the request
                            await DisplayAlert("Canceled", "Nothing has been chosen", "Abort");
                            return;
                    }
                    // Do the upload reqeust with the chosen typcode
                    var DoCall = byteConverter.ChangeUploadImage(typeCodeChoice, itemnummer, method, file);
                    // send the user back to the detail page
                    Navigation.InsertPageBefore(new DetailPage(itemnummer), this);
                    await Navigation.PopAsync(true);
                }
                // If the user pressed no or some where on the screen canceld the process
                else
                {
                    //Let the user know the request has been canceld
                    await DisplayAlert("Abort", "Cancel has been chosen", "abort");
                    return;
                }
            }
            else
            {
                // Ask the user if he/she want to edit the image
                var answer = await DisplayAlert("Change Image?", "Are you sure you want to edit this image", "Yes", "cancel");
                //Yes pressed
                if (answer)
                {
                    //Do the edit call
                    var DoCall = byteConverter.ChangeUploadImage(typecode, itemnummer, method, file);
                    Console.WriteLine(DoCall); Console.WriteLine(method); Console.WriteLine(file); Console.WriteLine(itemnummer); Console.WriteLine(typecode);
                    //Send the user back to the detail page
                    Navigation.InsertPageBefore(new DetailPage(itemnummer), this);
                    await Navigation.PopAsync(true);
                }
                // Cancel pressed/ Somewhere else on the screen
                else
                {
                    //Let the user know the process has been canceld
                    await DisplayAlert("Abort", "Cancel has been chosen", "abort");
                    return;
                }

            }
        }

        public async void UploadAsyncButton_Clicked(object sender, EventArgs e)
        {
                // Ask if the user wants to choose a image from the gallery or take one with the camera
                var Uploadanswer = await DisplayActionSheet("Upload Choosed, How do you want to upload", "Abort", null, "Camera", "Gallery");
                // If the user pressed Camera open the camera
                switch (Uploadanswer)
                {
                    case "Camera":
                        //Send the user to the camera and go on to the next function
                        BtnCamera_ClickedAsync(itemnumber, "TSE", "POST");
                        break;
                    case "Gallery":
                        //Send the user to his/her Gallery and go on to the next function
                        BtnPickPhoto_CickedAsync(itemnumber, "TSE", "POST");
                        break;
                    default:
                        await DisplayAlert("Action Canceld", "Action has been aborted", "Okay");
                        break;
                }
        }
    }
}