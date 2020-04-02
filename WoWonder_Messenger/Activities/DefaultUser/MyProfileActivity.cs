using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AT.Markushi.UI;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Com.Theartofdev.Edmodo.Cropper;
using Java.IO;
using Java.Lang;
using Plugin.Share;
using Plugin.Share.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WoWonder.Helpers.Ads;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Fonts;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.User;
using WoWonderClient.Requests;
using Console = System.Console;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.DefaultUser
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MyProfileActivity : AppCompatActivity, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private TextView TxtUserName, TxtFullname, TxtFollowing, TxtFollowingCount, TxtFollowers, TxtFollowersCount;
        private EditText TxtFirstName, TxtLastName, TxtGenderText, TxtLocationText, TxtMobileText, TxtWebsiteText, TxtWorkText;
        private TextView TxtNameIcon, TxtGenderIcon, TxtLocationIcon, TxtMobileIcon, TxtWebsiteIcon, TxtWorkIcon;
        private ImageView UserProfileImage, CoverImage;
        private CircleButton EditProfileButton;
        private TextView TxtFacebookIcon, TxtGoogleIcon, TxtTwitterIcon, TxtVkIcon, TxtInstagramIcon, TxtYoutubeIcon;
        private EditText TxtFacebookText, TxtGoogleText, TxtTwitterText, TxtVkText, TxtInstagramText, TxtYoutubeText;
        private string ImageType = "", GenderStatus = "";
        private AdView MAdView;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);
                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.MyProfile_Layout);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();

                MAdView = FindViewById<AdView>(Resource.Id.adView);
                AdsGoogle.InitAdView(MAdView, null);

                GetMyInfoData();

                AdsGoogle.Ad_Interstitial(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                MAdView?.Resume();
                AddOrRemoveEvent(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                MAdView?.Pause();
                base.OnPause();
                AddOrRemoveEvent(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                MAdView?.Destroy();

                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Menu

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.Profile_Menu, menu);

            var item = menu.FindItem(Resource.Id.menue_block);
            item.SetVisible(false);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                case Resource.Id.menue_SaveData:
                    EditProfileButtonOnClick();
                    break;

                case Resource.Id.menue_block:

                    break;

                case Resource.Id.menue_Copy:
                    OnCopeLinkToProfile_Button_Click();
                    break;

                case Resource.Id.menue_Share:
                    OnShare_Button_Click();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }


        //Event Menu >> Cope Link To Profile
        private void OnCopeLinkToProfile_Button_Click()
        {
            try
            {
                var clipboardManager = (ClipboardManager)GetSystemService(ClipboardService);

                var clipData = ClipData.NewPlainText("text", ListUtils.MyProfileList.FirstOrDefault()?.Url);
                clipboardManager.PrimaryClip = clipData;


                Toast.MakeText(this, GetText(Resource.String.Lbl_Copied), ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Event Menu >> Share
        private async void OnShare_Button_Click()
        {
            try
            {
                //Share Plugin same as video
                if (!CrossShare.IsSupported) return;

                var data = ListUtils.MyProfileList.FirstOrDefault();
                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = WoWonderTools.GetNameFinal(data),
                    Text = "",
                    Url = data?.Url
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                TxtNameIcon = FindViewById<TextView>(Resource.Id.name_icon);
                TxtFullname = FindViewById<TextView>(Resource.Id.Txt_fullname);
                TxtUserName = FindViewById<TextView>(Resource.Id.username);

                TxtFollowers = FindViewById<TextView>(Resource.Id.Txt_flowersView);
                TxtFollowersCount = FindViewById<TextView>(Resource.Id.Txt_flowers_count);

                TxtFollowing = FindViewById<TextView>(Resource.Id.flowinglabelView);
                TxtFollowingCount = FindViewById<TextView>(Resource.Id.Txt_flowing_countView);

                TxtFirstName = FindViewById<EditText>(Resource.Id.FirstName_text);
                TxtLastName = FindViewById<EditText>(Resource.Id.LastName_text);

                UserProfileImage = FindViewById<ImageView>(Resource.Id.profile_image);
                CoverImage = FindViewById<ImageView>(Resource.Id.coverImageView);

                TxtGenderIcon = FindViewById<TextView>(Resource.Id.gender_icon);
                TxtGenderText = FindViewById<EditText>(Resource.Id.gender_text);

                TxtLocationIcon = FindViewById<TextView>(Resource.Id.location_icon);
                TxtLocationText = FindViewById<EditText>(Resource.Id.location_text);

                TxtMobileIcon = FindViewById<TextView>(Resource.Id.mobile_icon);
                TxtMobileText = FindViewById<EditText>(Resource.Id.mobile_text);

                TxtWebsiteIcon = FindViewById<TextView>(Resource.Id.website_icon);
                TxtWebsiteText = FindViewById<EditText>(Resource.Id.website_text);

                TxtWorkIcon = FindViewById<TextView>(Resource.Id.work_icon);
                TxtWorkText = FindViewById<EditText>(Resource.Id.work_text);

                TxtFacebookIcon = FindViewById<TextView>(Resource.Id.facebook_icon);
                TxtFacebookText = FindViewById<EditText>(Resource.Id.facebook_text);
                TxtGoogleIcon = FindViewById<TextView>(Resource.Id.Google_icon);
                TxtGoogleText = FindViewById<EditText>(Resource.Id.Google_text);
                TxtTwitterIcon = FindViewById<TextView>(Resource.Id.Twitter_icon);
                TxtTwitterText = FindViewById<EditText>(Resource.Id.Twitter_text);
                TxtVkIcon = FindViewById<TextView>(Resource.Id.VK_icon);
                TxtVkText = FindViewById<EditText>(Resource.Id.VK_text);
                TxtInstagramIcon = FindViewById<TextView>(Resource.Id.Instagram_icon);
                TxtInstagramText = FindViewById<EditText>(Resource.Id.Instagram_text);
                TxtYoutubeIcon = FindViewById<TextView>(Resource.Id.Youtube_icon);
                TxtYoutubeText = FindViewById<EditText>(Resource.Id.Youtube_text);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, TxtGenderIcon, IonIconsFonts.Male);
                TxtGenderIcon.SetTextColor(Color.ParseColor("#4693d8"));

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, TxtLocationIcon, IonIconsFonts.Location);
                TxtLocationIcon.SetTextColor(Color.ParseColor(AppSettings.MainColor));

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, TxtMobileIcon, IonIconsFonts.AndroidCall);
                TxtMobileIcon.SetTextColor(Color.ParseColor("#fa6670"));

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, TxtWebsiteIcon, IonIconsFonts.AndroidGlobe);
                TxtWebsiteIcon.SetTextColor(Color.ParseColor("#6b38d1"));

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, TxtWorkIcon, IonIconsFonts.Briefcase);
                TxtWorkIcon.SetTextColor(Color.ParseColor("#eca72c"));

                EditProfileButton = FindViewById<CircleButton>(Resource.Id.Edit_button);
                //EditProfile_button.Click += EditProfileButtonOnClick;
                EditProfileButton.Visibility = ViewStates.Invisible;
                EditProfileButton.SetColor(Color.ParseColor("#282828"));

                if (AppSettings.SetTabDarkTheme)
                {
                    TxtFirstName.SetTextColor(Color.White);
                    TxtFirstName.SetHintTextColor(Color.White);

                    TxtLastName.SetTextColor(Color.White);
                    TxtLastName.SetHintTextColor(Color.White);

                    TxtGenderText.SetTextColor(Color.White);
                    TxtGenderText.SetHintTextColor(Color.White);

                    TxtLocationText.SetTextColor(Color.White);
                    TxtLocationText.SetHintTextColor(Color.White);

                    TxtMobileText.SetTextColor(Color.White);
                    TxtMobileText.SetHintTextColor(Color.White);

                    TxtWebsiteText.SetTextColor(Color.White);
                    TxtWebsiteText.SetHintTextColor(Color.White);

                    TxtWorkText.SetTextColor(Color.White);
                    TxtWorkText.SetHintTextColor(Color.White);

                    TxtFacebookText.SetTextColor(Color.White);
                    TxtFacebookText.SetHintTextColor(Color.White);

                    TxtGoogleText.SetTextColor(Color.White);
                    TxtGoogleText.SetHintTextColor(Color.White);

                    TxtTwitterText.SetTextColor(Color.White);
                    TxtTwitterText.SetHintTextColor(Color.White);

                    TxtVkText.SetTextColor(Color.White);
                    TxtVkText.SetHintTextColor(Color.White);

                    TxtInstagramText.SetTextColor(Color.White);
                    TxtInstagramText.SetHintTextColor(Color.White);

                    TxtYoutubeText.SetTextColor(Color.White);
                    TxtYoutubeText.SetHintTextColor(Color.White);
                }
                else
                {
                    TxtFirstName.SetTextColor(Color.Black);
                    TxtFirstName.SetHintTextColor(Color.Black);

                    TxtLastName.SetTextColor(Color.Black);
                    TxtLastName.SetHintTextColor(Color.Black);

                    TxtGenderText.SetTextColor(Color.Black);
                    TxtGenderText.SetHintTextColor(Color.Black);

                    TxtLocationText.SetTextColor(Color.Black);
                    TxtLocationText.SetHintTextColor(Color.Black);

                    TxtMobileText.SetTextColor(Color.Black);
                    TxtMobileText.SetHintTextColor(Color.Black);

                    TxtWebsiteText.SetTextColor(Color.Black);
                    TxtWebsiteText.SetHintTextColor(Color.Black);

                    TxtWorkText.SetTextColor(Color.Black);
                    TxtWorkText.SetHintTextColor(Color.Black);

                    TxtFacebookText.SetTextColor(Color.Black);
                    TxtFacebookText.SetHintTextColor(Color.Black);

                    TxtGoogleText.SetTextColor(Color.Black);
                    TxtGoogleText.SetHintTextColor(Color.Black);

                    TxtTwitterText.SetTextColor(Color.Black);
                    TxtTwitterText.SetHintTextColor(Color.Black);

                    TxtVkText.SetTextColor(Color.Black);
                    TxtVkText.SetHintTextColor(Color.Black);

                    TxtInstagramText.SetTextColor(Color.Black);
                    TxtInstagramText.SetHintTextColor(Color.Black);

                    TxtYoutubeText.SetTextColor(Color.Black);
                    TxtYoutubeText.SetHintTextColor(Color.Black);
                }


                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, TxtNameIcon, IonIconsFonts.Person);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, TxtFacebookIcon, IonIconsFonts.SocialFacebook);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, TxtGoogleIcon, IonIconsFonts.SocialGoogle);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, TxtTwitterIcon, IonIconsFonts.SocialTwitter);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeBrands, TxtVkIcon, FontAwesomeIcon.Vk);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, TxtInstagramIcon, IonIconsFonts.SocialInstagram);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, TxtYoutubeIcon, IonIconsFonts.SocialYoutube);

                TxtGenderText.SetFocusable(ViewFocusability.NotFocusable);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = " ";
                    toolbar.SetTitleTextColor(Color.White);
                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);

                    if (AppSettings.FlowDirectionRightToLeft)
                        toolbar.LayoutDirection = LayoutDirection.Rtl;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    UserProfileImage.Click += UserProfileImageOnClick;
                    CoverImage.Click += ImageUserCoverOnClick;
                    TxtGenderText.Click += TxtGenderTextOnClick;

                }
                else
                {
                    UserProfileImage.Click -= UserProfileImageOnClick;
                    CoverImage.Click -= ImageUserCoverOnClick;
                    TxtGenderText.Click -= TxtGenderTextOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }



        #endregion

        #region Events

        private void ImageUserCoverOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                OpenDialogGallery("Cover");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void UserProfileImageOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                OpenDialogGallery("Avatar");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void TxtGenderTextOnClick(object sender, EventArgs e)
        {
            try
            {
                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                if (ListUtils.SettingsSiteList?.Genders.Count > 0)
                {
                    arrayAdapter.AddRange(from item in ListUtils.SettingsSiteList?.Genders select item.Value);
                }
                else
                {
                    arrayAdapter.Add(GetText(Resource.String.Radio_Male));
                    arrayAdapter.Add(GetText(Resource.String.Radio_Female));
                }

                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        #endregion

        #region Update Image Avatar && Cover

        private void OpenDialogGallery(string typeImage)
        {
            try
            {
                ImageType = typeImage;
                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    Methods.Path.Chack_MyFolder();

                    //Open Image 
                    var myUri = Uri.FromFile(new File(Methods.Path.FolderDcimImage, Methods.GetTimestamp(DateTime.Now) + ".jpeg"));
                    CropImage.Builder()
                        .SetInitialCropWindowPaddingRatio(0)
                        .SetAutoZoomEnabled(true)
                        .SetMaxZoom(4)
                        .SetGuidelines(CropImageView.Guidelines.On)
                        .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Crop))
                        .SetOutputUri(myUri).Start(this);
                }
                else
                {
                    if (!CropImage.IsExplicitCameraPermissionRequired(this) && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted && CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted)
                    {
                        Methods.Path.Chack_MyFolder();

                        //Open Image 
                        var myUri = Uri.FromFile(new File(Methods.Path.FolderDcimImage, Methods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder()
                            .SetInitialCropWindowPaddingRatio(0)
                            .SetAutoZoomEnabled(true)
                            .SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On)
                            .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Crop))
                            .SetOutputUri(myUri).Start(this);
                    }
                    else
                    {
                        new PermissionsController(this).RequestPermission(108);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void Update_Image_Api(string type, string path)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)
                        .Show();
                }
                else
                {
                    if (type == "Avatar")
                    {
                        var (apiStatus, respond) = await RequestsAsync.Global.Update_User_Avatar(path);
                        if (apiStatus == 200)
                        {
                            if (respond is MessageObject result)
                            {
                                Console.WriteLine(result.Message);
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Image_changed_successfully), ToastLength.Short).Show();

                                //Set image 
                                File file2 = new File(path);
                                var photoUri = FileProvider.GetUriForFile(this, PackageName + ".fileprovider", file2);
                                Glide.With(this).Load(photoUri).Apply(new RequestOptions().CircleCrop()).Into(UserProfileImage);

                                //GlideImageLoader.LoadImage(this, path, UserProfileImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                                var local = ListUtils.MyProfileList.FirstOrDefault();
                                if (local != null)
                                {
                                    local.Avatar = path;

                                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                    dbDatabase.Insert_Or_Update_To_MyProfileTable(local);
                                    dbDatabase.Dispose();
                                }
                            }
                        }
                        else Methods.DisplayReportResult(this, respond);
                    }
                    else if (type == "Cover")
                    {
                        var (apiStatus, respond) = await RequestsAsync.Global.Update_User_Cover(path);
                        if (apiStatus == 200)
                        {
                            if (respond is MessageObject result)
                            {
                                Console.WriteLine(result.Message);
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Image_changed_successfully), ToastLength.Short).Show();

                                //Set image 
                                //var file = Uri.FromFile(new File(path));
                                File file2 = new File(path);
                                var photoUri = FileProvider.GetUriForFile(this, PackageName + ".fileprovider", file2);
                                Glide.With(this).Load(photoUri).Apply(new RequestOptions()).Into(CoverImage);

                                //GlideImageLoader.LoadImage(this, path, CoverImage, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                                var local = ListUtils.MyProfileList.FirstOrDefault();
                                if (local != null)
                                {
                                    local.Cover = path;

                                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                    dbDatabase.Insert_Or_Update_To_MyProfileTable(local);
                                    dbDatabase.Dispose();
                                }

                            }
                        }
                        else Methods.DisplayReportResult(this, respond);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Permissions && Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                //If its from Camera or Gallery
                if (requestCode == CropImage.CropImageActivityRequestCode)
                {
                    var result = CropImage.GetActivityResult(data);

                    if (resultCode == Result.Ok)
                    {
                        if (result.IsSuccessful)
                        {
                            var resultUri = result.Uri;

                            if (!string.IsNullOrEmpty(resultUri.Path))
                            {
                                string pathimg;
                                if (ImageType == "Cover")
                                {
                                    pathimg = resultUri.Path;
                                    Update_Image_Api(ImageType, pathimg);
                                }
                                else if (ImageType == "Avatar")
                                {
                                    pathimg = resultUri.Path;
                                    Update_Image_Api(ImageType, pathimg);
                                }
                            }
                            else
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Something_went_wrong), ToastLength.Long).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Something_went_wrong), ToastLength.Long)
                                .Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Something_went_wrong), ToastLength.Long)
                            .Show();
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 108)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        OpenDialogGallery(ImageType);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Get Data User

        //Get Data User From Database 
        private void GetMyInfoData()
        {
            try
            {
                var dataUser = ListUtils.MyProfileList.FirstOrDefault();
                LoadDataUser(dataUser);

                StartApiService();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Get Data My Profile API
        private void StartApiService()
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { GetProfileApi });
        }

        private async Task GetProfileApi()
        {
            var (apiStatus, respond) = await RequestsAsync.Global.Get_User_Data(UserDetails.UserId, "user_data,followers,following");

            if (apiStatus != 200 || !(respond is GetUserDataObject result) || result.UserData == null)
            {
                Methods.DisplayReportResult(this, respond);
            }
            else
            {
                LoadDataUser(result.UserData);
            }
        }


        private void LoadDataUser(UserDataObject data)
        {
            try
            {
                //Cover
                GlideImageLoader.LoadImage(this, data.Cover, CoverImage, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                //profile_picture
                GlideImageLoader.LoadImage(this, data.Avatar, UserProfileImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                UserDetails.FullName = data.Name;

                TxtFullname.Text = WoWonderTools.GetNameFinal(data);
                TxtUserName.Text = "@" + data.Username;
                TxtFirstName.Text = Methods.FunString.DecodeString(data.FirstName);
                TxtLastName.Text = Methods.FunString.DecodeString(data.LastName);

                if (data.Details.DetailsClass != null)
                {
                    var following = Methods.FunString.FormatPriceValue(Convert.ToInt32(data.Details.DetailsClass?.FollowingCount));
                    var followers = Methods.FunString.FormatPriceValue(Convert.ToInt32(data.Details.DetailsClass?.FollowersCount));

                    if (AppSettings.ConnectivitySystem == 1)
                    {
                        TxtFollowing.Visibility = ViewStates.Visible;
                        TxtFollowingCount.Visibility = ViewStates.Visible;

                        TxtFollowers.Visibility = ViewStates.Visible;
                        TxtFollowersCount.Visibility = ViewStates.Visible;

                        TxtFollowing.Text = GetText(Resource.String.Lbl_Following);
                        TxtFollowingCount.Text = following;

                        TxtFollowers.Text = GetText(Resource.String.Lbl_Followers);
                        TxtFollowersCount.Text = followers;
                    }
                    else
                    {
                        TxtFollowing.Visibility = ViewStates.Visible;
                        TxtFollowingCount.Visibility = ViewStates.Visible;

                        TxtFollowers.Visibility = ViewStates.Gone;
                        TxtFollowersCount.Visibility = ViewStates.Gone;

                        TxtFollowing.Text = GetText(Resource.String.Lbl_Friends);
                        TxtFollowingCount.Text = following;
                    }
                }

                switch (data.Gender.ToLower())
                {
                    case "male":
                        TxtGenderText.Text = GetText(Resource.String.Radio_Male);
                        break;
                    case "female":
                        TxtGenderText.Text = GetText(Resource.String.Radio_Female);
                        break;
                    default:
                        TxtGenderText.Text = data.Gender;
                        break;
                }

                TxtLocationText.Text = data.Address;
                TxtMobileText.Text = data.PhoneNumber;
                TxtWebsiteText.Text = data.Website;
                TxtWorkText.Text = data.Working;

                TxtFacebookText.Text = data.Facebook;
                TxtGoogleText.Text = data.Google;
                TxtTwitterText.Text = data.Twitter;
                TxtVkText.Text = data.Vk;
                TxtInstagramText.Text = data.Instagram;
                TxtYoutubeText.Text = data.Youtube;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                if (ListUtils.SettingsSiteList?.Genders.Count > 0)
                {
                    TxtGenderText.Text = itemString.ToString();
                    var key = ListUtils.SettingsSiteList?.Genders?.FirstOrDefault(a => a.Value == itemString.ToString()).Key;
                    GenderStatus = key ?? "male";
                }
                else
                {
                    if (itemString.ToString() == GetText(Resource.String.Radio_Male))
                    {
                        TxtGenderText.Text = GetText(Resource.String.Radio_Male);
                        GenderStatus = "male";
                    }
                    else if (itemString.ToString() == GetText(Resource.String.Radio_Female))
                    {
                        TxtGenderText.Text = GetText(Resource.String.Radio_Female);
                        GenderStatus = "female";
                    }
                    else
                    {
                        GenderStatus = "male";
                    }
                }
                switch (itemId)
                {
                    // Radio_Male 
                    case 0:
                        TxtGenderText.Text = GetText(Resource.String.Radio_Male);
                        break;
                    // Radio_Female 
                    case 1:
                        TxtGenderText.Text = GetText(Resource.String.Radio_Female);
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        private async void EditProfileButtonOnClick()
        {
            try
            {
                //Show a progress
                AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                var local = ListUtils.MyProfileList.FirstOrDefault();
                if (local != null)
                {
                    local.FirstName = TxtFirstName.Text;
                    local.LastName = TxtLastName.Text;
                    local.Address = TxtLocationText.Text;
                    local.Working = TxtWorkText.Text;
                    local.GenderText = TxtGenderText.Text;
                    local.Gender = GenderStatus;
                    local.Website = TxtWebsiteText.Text;
                    local.Facebook = TxtFacebookText.Text;
                    local.Google = TxtGoogleText.Text;
                    local.Twitter = TxtTwitterText.Text;
                    local.Youtube = TxtYoutubeText.Text;
                    local.Vk = TxtVkText.Text;
                    local.Instagram = TxtInstagramText.Text;
                    local.PhoneNumber = TxtMobileText.Text;

                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                    dbDatabase.Insert_Or_Update_To_MyProfileTable(local);
                    dbDatabase.Dispose();
                }

                if (Methods.CheckConnectivity())
                {
                    var dictionary = new Dictionary<string, string>();

                    if (!string.IsNullOrEmpty(TxtFirstName.Text))
                        dictionary.Add("first_name", TxtFirstName.Text);

                    if (!string.IsNullOrEmpty(TxtLastName.Text))
                        dictionary.Add("last_name", TxtLastName.Text);

                    if (!string.IsNullOrEmpty(TxtFacebookText.Text))
                        dictionary.Add("facebook", TxtFacebookText.Text);

                    if (!string.IsNullOrEmpty(TxtGoogleText.Text))
                        dictionary.Add("google", TxtGoogleText.Text);

                    if (!string.IsNullOrEmpty(TxtTwitterText.Text))
                        dictionary.Add("twitter", TxtTwitterText.Text);

                    if (!string.IsNullOrEmpty(TxtYoutubeText.Text))
                        dictionary.Add("youtube", TxtYoutubeText.Text);

                    if (!string.IsNullOrEmpty(TxtInstagramText.Text))
                        dictionary.Add("instagram", TxtInstagramText.Text);

                    if (!string.IsNullOrEmpty(TxtVkText.Text))
                        dictionary.Add("vk", TxtVkText.Text);

                    if (!string.IsNullOrEmpty(TxtWebsiteText.Text))
                        dictionary.Add("website", TxtWebsiteText.Text);

                    if (!string.IsNullOrEmpty(TxtLocationText.Text))
                        dictionary.Add("address", TxtLocationText.Text);

                    if (!string.IsNullOrEmpty(TxtGenderText.Text))
                        dictionary.Add("gender", GenderStatus);

                    if (!string.IsNullOrEmpty(TxtMobileText.Text))
                        dictionary.Add("phone_number", TxtMobileText.Text);

                    var (apiStatus, respond) = await RequestsAsync.Global.Update_User_Data(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is MessageObject result)
                        {
                            if (result.Message.Contains("updated"))
                            {
                                AndHUD.Shared.Dismiss(this);
                                Toast.MakeText(this, result.Message, ToastLength.Short).Show();
                            }
                        }
                    }
                    else Methods.DisplayReportResult(this, respond);

                    AndHUD.Shared.Dismiss(this);
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception e)
            {
                AndHUD.Shared.Dismiss(this);
                Console.WriteLine(e);
            }
        }
    }
}