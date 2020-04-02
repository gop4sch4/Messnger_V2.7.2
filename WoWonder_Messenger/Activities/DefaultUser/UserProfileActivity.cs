using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Util;
using Com.Luseen.Autolinklibrary;
using Java.IO;
using Newtonsoft.Json;
using Plugin.Share;
using Plugin.Share.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WoWonder.Activities.ChatWindow;
using WoWonder.Activities.SharedFiles;
using WoWonder.Activities.SharedFiles.Adapter;
using WoWonder.Helpers.Ads;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Fonts;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using WoWonderClient;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Message;
using WoWonderClient.Classes.User;
using WoWonderClient.Requests;
using Console = System.Console;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.DefaultUser
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class UserProfileActivity : AppCompatActivity
    {
        #region Variables Basic

        private TextView TxtUserName, TxtFullname, TxtFollowing, TxtFollowingCount, TxtFollowers, TxtFollowersCount;
        private TextView TxtGenderText, TxtLocationText, TxtMobileText, TxtWebsiteText, TxtWorkText;
        private AutoLinkTextView TxtAbout;
        private TextView TxtGenderIcon, TxtLocationIcon, TxtMobileIcon, TxtWebsiteIcon, TxtWorkIcon;
        private ImageView ImageUserProfile, ImageUserCover;
        private CircleButton AddFriendOrFollowButton, BtnFacebook, BtnInstegram, BtnTwitter, BtnGoogle, BtnVk, BtnYoutube;
        private LinearLayout GenderLiner, LocationLiner, MobileLiner, WebsiteLiner, WorkLiner, MediaLiner;
        private AdView MAdView;
        private string SUserId = "", NamePage;
        private UserDataObject UserData;
        private HSharedFilesAdapter MAdapter;
        private LinearLayout MainLinear;
        private TextView IconTitle;
        private RecyclerView MRecycler;
        private LinearLayoutManager LayoutManager;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                View mContentView = Window.DecorView;
                var uiOptions = (int)mContentView.SystemUiVisibility;
                var newUiOptions = uiOptions;

                newUiOptions |= (int)SystemUiFlags.LayoutStable;
                newUiOptions |= (int)SystemUiFlags.LayoutFullscreen;
                mContentView.SystemUiVisibility = (StatusBarVisibility)newUiOptions;

                Window.AddFlags(WindowManagerFlags.Fullscreen);

                // Create your application here
                SetContentView(Resource.Layout.UserProfile_Layout);

                NamePage = Intent.GetStringExtra("NamePage") ?? string.Empty;

                SUserId = Intent.GetStringExtra("UserId") ?? string.Empty;

                var userObject = Intent.GetStringExtra("UserObject");
                if (!string.IsNullOrEmpty(userObject))
                {
                    try
                    {
                        if (NamePage == "Chat")
                        {
                            if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                                UserData = JsonConvert.DeserializeObject<UserDataObject>(userObject);
                            else if (AppSettings.LastChatSystem == SystemApiGetLastChat.Old)
                                UserData = JsonConvert.DeserializeObject<GetUsersListObject.User>(userObject);
                            else
                                UserData = JsonConvert.DeserializeObject<UserDataObject>(userObject);
                        }
                        else
                        {
                            UserData = JsonConvert.DeserializeObject<UserDataObject>(userObject);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                SetRecyclerViewAdapters();

                MAdView = FindViewById<AdView>(Resource.Id.adView);
                AdsGoogle.InitAdView(MAdView, null);

                //Get Data User  
                if (UserData != null)
                    LoadDataUser(UserData);

                GetFiles();

                StartApiService();

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
                MAdView?.Resume();
                base.OnResume();
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

            var item = menu.FindItem(Resource.Id.menue_SaveData);

            if (NamePage == "Chat")
            {
                item.SetVisible(false);
            }
            else
            {
                item?.SetIcon(Resource.Drawable.SendLetter);
                item?.SetTitle(GetString(Resource.String.Lbl_SendMessage));
            }

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
                    OpenChatButtonOnClick();
                    break;

                case Resource.Id.menue_block:
                    BlockUserButtonClick();
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

        private void OpenChatButtonOnClick()
        {
            try
            {
                if (UserData.ChatColor == null)
                    UserData.ChatColor = AppSettings.MainColor;

                var mainChatColor = UserData.ChatColor.Contains("rgb") ? Methods.FunString.ConvertColorRgBtoHex(UserData.ChatColor) : UserData.ChatColor ?? AppSettings.MainColor;

                Intent intent = new Intent(this, typeof(ChatWindowActivity));
                intent.PutExtra("UserID", UserData.UserId);
                intent.PutExtra("TypeChat", "User");
                intent.PutExtra("ColorChat", mainChatColor);
                intent.PutExtra("UserItem", JsonConvert.SerializeObject(UserData));
                StartActivity(intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Event Block User
        private async void BlockUserButtonClick()
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
                    var (apiStatus, respond) = await RequestsAsync.Global.Block_User(SUserId, true).ConfigureAwait(true); //true >> "block"
                    if (apiStatus == 200)
                    {
                        var dbDatabase = new SqLiteDatabase();
                        dbDatabase.Delete_UsersContact(SUserId);
                        dbDatabase.Dispose();

                        Toast.MakeText(this, GetString(Resource.String.Lbl_Blocked_successfully), ToastLength.Short).Show();
                        Finish();
                    }
                    else Methods.DisplayReportResult(this, respond);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Event Menu >> Cope Link To Profile
        private void OnCopeLinkToProfile_Button_Click()
        {
            try
            {
                var clipboardManager = (ClipboardManager)GetSystemService(ClipboardService);

                var clipData = ClipData.NewPlainText("text", UserData.Url);
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

                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = WoWonderTools.GetNameFinal(UserData),
                    Text = "",
                    Url = UserData.Url
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
                MediaLiner = (LinearLayout)FindViewById(Resource.Id.mediaLiner);
                MediaLiner.Visibility = ViewStates.Gone;

                MainLinear = (LinearLayout)FindViewById(Resource.Id.mainLinear);
                IconTitle = (TextView)FindViewById(Resource.Id.iconTitle);
                MRecycler = (RecyclerView)FindViewById(Resource.Id.recyler);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconTitle, IonIconsFonts.Document);

                TxtFullname = FindViewById<TextView>(Resource.Id.Txt_fullname);
                TxtUserName = FindViewById<TextView>(Resource.Id.username);

                TxtFollowers = FindViewById<TextView>(Resource.Id.Txt_flowersView);
                TxtFollowersCount = FindViewById<TextView>(Resource.Id.Txt_flowers_count);

                TxtFollowing = FindViewById<TextView>(Resource.Id.flowinglabelView);
                TxtFollowingCount = FindViewById<TextView>(Resource.Id.Txt_flowing_countView);

                TxtAbout = FindViewById<AutoLinkTextView>(Resource.Id.Txt_AboutUser);

                ImageUserProfile = FindViewById<ImageView>(Resource.Id.profile_image);
                ImageUserCover = FindViewById<ImageView>(Resource.Id.coverImageView);

                GenderLiner = FindViewById<LinearLayout>(Resource.Id.genderLiner);
                TxtGenderIcon = FindViewById<TextView>(Resource.Id.gender_icon);
                TxtGenderText = FindViewById<TextView>(Resource.Id.gender_text);

                LocationLiner = FindViewById<LinearLayout>(Resource.Id.locationLiner);
                TxtLocationIcon = FindViewById<TextView>(Resource.Id.location_icon);
                TxtLocationText = FindViewById<TextView>(Resource.Id.location_text);

                MobileLiner = FindViewById<LinearLayout>(Resource.Id.mobileLiner);
                TxtMobileIcon = FindViewById<TextView>(Resource.Id.mobile_icon);
                TxtMobileText = FindViewById<TextView>(Resource.Id.mobile_text);

                WebsiteLiner = FindViewById<LinearLayout>(Resource.Id.websiteLiner);
                TxtWebsiteIcon = FindViewById<TextView>(Resource.Id.website_icon);
                TxtWebsiteText = FindViewById<TextView>(Resource.Id.website_text);

                WorkLiner = FindViewById<LinearLayout>(Resource.Id.workLiner);
                TxtWorkIcon = FindViewById<TextView>(Resource.Id.work_icon);
                TxtWorkText = FindViewById<TextView>(Resource.Id.work_text);

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

                AddFriendOrFollowButton = FindViewById<CircleButton>(Resource.Id.follow_button);
                AddFriendOrFollowButton.Tag = "Add";
                AddFriendOrFollowButton.SetColor(Color.ParseColor("#8c8a8a"));

                BtnFacebook = FindViewById<CircleButton>(Resource.Id.facebook_button);
                BtnInstegram = FindViewById<CircleButton>(Resource.Id.instegram_button);
                BtnTwitter = FindViewById<CircleButton>(Resource.Id.twitter_button);
                BtnGoogle = FindViewById<CircleButton>(Resource.Id.google_button);
                BtnVk = FindViewById<CircleButton>(Resource.Id.vk_button);
                BtnYoutube = FindViewById<CircleButton>(Resource.Id.youtube_button);


                GenderLiner.Visibility = ViewStates.Gone;
                LocationLiner.Visibility = ViewStates.Gone;
                MobileLiner.Visibility = ViewStates.Gone;
                WebsiteLiner.Visibility = ViewStates.Gone;
                WorkLiner.Visibility = ViewStates.Gone;
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

        private void SetRecyclerViewAdapters()
        {
            try
            {
                MAdapter = new HSharedFilesAdapter(this, SUserId, "Horizontal") { SharedFilesList = new ObservableCollection<Classes.SharedFile>() };
                LayoutManager = new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false);
                MRecycler.SetLayoutManager(LayoutManager);
                MRecycler.NestedScrollingEnabled = false;
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(10);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<Classes.SharedFile>(this, MAdapter, sizeProvider, 8);
                MRecycler.AddOnScrollListener(preLoader);
                MRecycler.SetAdapter(MAdapter);
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
                    AddFriendOrFollowButton.Click += AddFriendOrFollowButtonClick;
                    BtnFacebook.Click += SocialFacebookOnClick;
                    BtnInstegram.Click += SocialInstagramOnClick;
                    BtnTwitter.Click += SocialTwitterOnClick;
                    BtnGoogle.Click += SocialGoogleOnClick;
                    BtnVk.Click += BtnVkontakteOnClick;
                    BtnYoutube.Click += BtnYoutubeOnClick;
                    MainLinear.Click += MediaLinerOnClick;
                    MAdapter.ItemClick += MAdapterOnItemClick;
                }
                else
                {
                    AddFriendOrFollowButton.Click -= AddFriendOrFollowButtonClick;
                    BtnFacebook.Click -= SocialFacebookOnClick;
                    BtnInstegram.Click -= SocialInstagramOnClick;
                    BtnTwitter.Click -= SocialTwitterOnClick;
                    BtnGoogle.Click -= SocialGoogleOnClick;
                    BtnVk.Click -= BtnVkontakteOnClick;
                    BtnYoutube.Click -= BtnYoutubeOnClick;
                    MainLinear.Click -= MediaLinerOnClick;
                    MAdapter.ItemClick -= MAdapterOnItemClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        private void MAdapterOnItemClick(object sender, HSharedFilesAdapterViewHolderClickEventArgs e)
        {
            try
            {
                var item = MAdapter.GetItem(e.Position);
                if (item != null)
                {
                    File file2 = new File(item.FilePath);
                    var mediaUri = FileProvider.GetUriForFile(this, PackageName + ".fileprovider", file2);

                    var extension = item.FilePath.Split('.').Last();
                    string mimeType = MimeTypeMap.GetMimeType(extension);

                    Intent intent = new Intent(Intent.ActionView);
                    intent.SetFlags(ActivityFlags.NewTask);
                    intent.SetFlags(ActivityFlags.GrantReadUriPermission);
                    intent.SetAction(Intent.ActionView);
                    intent.SetDataAndType(mediaUri, mimeType);
                    StartActivity(intent);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        // Open All Shared Files
        private void MediaLinerOnClick(object sender, EventArgs e)
        {
            try
            {
                var intent = new Intent(this, typeof(SharedFilesActivity));
                intent.PutExtra("UserId", SUserId);
                StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Instagram
        private void SocialInstagramOnClick(object sender, EventArgs e)
        {
            try
            {
                new IntentController(this).OpenInstagramIntent(UserData.Instagram);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Twitter
        private void SocialTwitterOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    //IMethods.IApp.OpenbrowserUrl(this, "https://twitter.com/"+ Twitter);

                    new IntentController(this).OpenTwitterIntent(UserData.Twitter);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Facebook
        private void SocialFacebookOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    //IMethods.IApp.OpenbrowserUrl(this, "https://www.facebook.com/"+ Facebook);

                    new IntentController(this).OpenFacebookIntent(this, UserData.Facebook);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Google
        private void SocialGoogleOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    Methods.App.OpenbrowserUrl(this, "https://plus.google.com/u/0/" + UserData.Google);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void BtnYoutubeOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    new IntentController(this).OpenYoutubeIntent(UserData.Youtube);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void BtnVkontakteOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    new IntentController(this).OpenVkontakteIntent(UserData.Vk);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void AddFriendOrFollowButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (AddFriendOrFollowButton.Tag.ToString() == "Add") //(is_following == "0") >> Not Friend
                    {
                        AddFriendOrFollowButton.SetColor(Color.ParseColor(AppSettings.MainColor));
                        AddFriendOrFollowButton.SetImageResource(Resource.Drawable.ic_tick);
                        AddFriendOrFollowButton.Tag = "friends";
                    }
                    else if (AddFriendOrFollowButton.Tag.ToString() == "request") //(is_following == "2") >> Request
                    {
                        AddFriendOrFollowButton.SetColor(Color.ParseColor(AppSettings.MainColor));
                        AddFriendOrFollowButton.SetImageResource(Resource.Drawable.ic_tick);
                        AddFriendOrFollowButton.Tag = "Add";
                    }
                    else //(is_following == "1") >> Friend
                    {
                        AddFriendOrFollowButton.SetColor(Color.ParseColor("#8c8a8a"));
                        AddFriendOrFollowButton.SetImageResource(Resource.Drawable.ic_add);

                        AddFriendOrFollowButton.Tag = "Add";
                    }

                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Follow_User(SUserId) });
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        #endregion

        #region Get Data User Api

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
            var (apiStatus, respond) = await RequestsAsync.Global.Get_User_Data(SUserId, "user_data,followers,following");

            if (apiStatus != 200 || !(respond is GetUserDataObject result) || result.UserData == null)
            {
                Methods.DisplayReportResult(this, respond);
            }
            else
            {
                UserData = result.UserData;
                LoadDataUser(result.UserData);
            }
        }

        #endregion

        #region Get Data User

        private void LoadDataUser(UserDataObject data)
        {
            try
            {
                //Cover
                GlideImageLoader.LoadImage(this, data.Cover, ImageUserCover, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                //profile_picture
                GlideImageLoader.LoadImage(this, data.Avatar, ImageUserProfile, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                TextSanitizer sanitizer = new TextSanitizer(TxtAbout, this);
                sanitizer.Load(WoWonderTools.GetAboutFinal(data));

                TxtFullname.Text = WoWonderTools.GetNameFinal(data);
                TxtUserName.Text = "@" + data.Username;

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

                switch (data.IsFollowing)
                {
                    // My Friend
                    case "1":
                        AddFriendOrFollowButton.Visibility = ViewStates.Visible;
                        AddFriendOrFollowButton.SetColor(Color.ParseColor(AppSettings.MainColor));
                        AddFriendOrFollowButton.SetImageResource(Resource.Drawable.ic_tick);
                        AddFriendOrFollowButton.Tag = "friends";
                        break;
                    // Request
                    case "2":
                        AddFriendOrFollowButton.Visibility = ViewStates.Visible;
                        AddFriendOrFollowButton.SetColor(Color.ParseColor(AppSettings.MainColor));
                        AddFriendOrFollowButton.SetImageResource(Resource.Drawable.ic_requestAdd);
                        AddFriendOrFollowButton.Tag = "request";
                        break;
                    //Not Friend
                    case "0":
                        AddFriendOrFollowButton.Visibility = ViewStates.Visible;
                        AddFriendOrFollowButton.SetColor(Color.ParseColor("#8c8a8a"));
                        AddFriendOrFollowButton.SetImageResource(Resource.Drawable.ic_add);
                        AddFriendOrFollowButton.Tag = "Add";
                        break;
                }

                if (Methods.FunString.StringNullRemover(data.Gender) != "Empty")
                {
                    TxtGenderText.Text = data.Gender.ToLower() switch
                    {
                        "male" => GetText(Resource.String.Radio_Male),
                        "female" => GetText(Resource.String.Radio_Female),
                        _ => data.Gender
                    };

                    GenderLiner.Visibility = ViewStates.Visible;
                }
                else
                {
                    GenderLiner.Visibility = ViewStates.Gone;
                }

                if (Methods.FunString.StringNullRemover(data.Address) != "Empty")
                {
                    LocationLiner.Visibility = ViewStates.Visible;
                    TxtLocationText.Text = data.Address;
                }
                else
                {
                    LocationLiner.Visibility = ViewStates.Gone;
                }

                if (AppSettings.EnableShowPhoneNumber)
                {
                    if (Methods.FunString.StringNullRemover(data.PhoneNumber) != "Empty")
                    {
                        MobileLiner.Visibility = ViewStates.Visible;
                        TxtMobileText.Text = data.PhoneNumber;
                    }
                    else
                    {
                        MobileLiner.Visibility = ViewStates.Gone;
                    }
                }
                else
                {
                    MobileLiner.Visibility = ViewStates.Gone;
                }

                if (Methods.FunString.StringNullRemover(data.Website) != "Empty")
                {
                    WebsiteLiner.Visibility = ViewStates.Visible;
                    TxtWebsiteText.Text = data.Website;
                }
                else
                {
                    WebsiteLiner.Visibility = ViewStates.Gone;
                }

                if (Methods.FunString.StringNullRemover(data.Working) != "Empty")
                {
                    WorkLiner.Visibility = ViewStates.Visible;
                    TxtWorkText.Text = data.Working;
                }
                else
                {
                    WorkLiner.Visibility = ViewStates.Gone;
                }

                if (Methods.FunString.StringNullRemover(data.Facebook) == "Empty")
                {
                    BtnFacebook.Enabled = false;
                    BtnFacebook.SetColor(Color.ParseColor("#8c8a8a"));
                }
                else
                {
                    BtnFacebook.Enabled = true;
                }

                if (Methods.FunString.StringNullRemover(data.Google) == "Empty")
                {
                    BtnGoogle.Enabled = false;
                    BtnGoogle.SetColor(Color.ParseColor("#8c8a8a"));
                }
                else
                {
                    BtnGoogle.Enabled = true;
                }

                if (Methods.FunString.StringNullRemover(data.Twitter) == "Empty")
                {
                    BtnTwitter.Enabled = false;
                    BtnTwitter.SetColor(Color.ParseColor("#8c8a8a"));
                }
                else
                {
                    BtnTwitter.Enabled = true;
                }

                if (Methods.FunString.StringNullRemover(data.Youtube) == "Empty")
                {
                    BtnYoutube.Enabled = false;
                    BtnYoutube.SetColor(Color.ParseColor("#8c8a8a"));
                }
                else
                {
                    BtnYoutube.Enabled = true;
                }

                if (Methods.FunString.StringNullRemover(data.Vk) == "Empty")
                {
                    BtnVk.Enabled = false;
                    BtnVk.SetColor(Color.ParseColor("#8c8a8a"));
                }
                else
                {
                    BtnVk.Enabled = true;
                }

                if (Methods.FunString.StringNullRemover(data.Instagram) == "Empty")
                {
                    BtnInstegram.Enabled = false;
                    BtnInstegram.SetColor(Color.ParseColor("#8c8a8a"));
                }
                else
                {
                    BtnInstegram.Enabled = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        private async void GetFiles()
        {
            try
            {
                if (ListUtils.ListSharedFiles.Count > 0)
                {
                    ListUtils.ListSharedFiles.Clear();
                    ListUtils.LastSharedFiles.Clear();
                }

                await WoWonderTools.GetSharedFiles(SUserId);

                if (ListUtils.LastSharedFiles.Count > 0)
                {
                    MRecycler.Visibility = ViewStates.Visible;
                    MediaLiner.Visibility = ViewStates.Visible;

                    MAdapter.SharedFilesList = ListUtils.LastSharedFiles;
                    MAdapter.NotifyDataSetChanged();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}