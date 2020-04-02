using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Support.V4.Content;
using Android.Support.V4.View.Animation;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Com.Theartofdev.Edmodo.Cropper;
using Developer.SEmojis.Actions;
using Developer.SEmojis.Helper;
using Java.IO;
using Java.Lang;
using Newtonsoft.Json;
using Refractored.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using WoWonder.Activities.ChatWindow.Adapters;
using WoWonder.Activities.ChatWindow.Fragment;
using WoWonder.Activities.Gif;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Activities.Tab;
using WoWonder.Adapters;
using WoWonder.Frameworks.Agora;
using WoWonder.Frameworks.Twilio;
using WoWonder.Helpers.Ads;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using WoWonderClient;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Message;
using WoWonderClient.Classes.User;
using WoWonderClient.Requests;
using Console = System.Console;
using Exception = System.Exception;
using MessageData = WoWonder.Helpers.Model.MessageDataExtra;
using SupportFragment = Android.Support.V4.App.Fragment;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.ChatWindow
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ResizeableActivity = true, ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden)]
    public class ChatWindowActivity : AppCompatActivity, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback, View.IOnLayoutChangeListener
    {
        #region Variables Basic

        private AppCompatImageView ChatEmojImage;
        private LinearLayout RootView;
        private EmojiconEditText EmojIconEditTextView;
        private CircleImageView UserChatProfile;
        public CircleButton ChatSendButton;
        public ImageView ChatColorButton, ChatStickerButton, ChatMediaButton;
        public RecyclerView MRecycler;
        private RecyclerView RecyclerHiSuggestion;
        private ChatColorsFragment ChatColorBoxFragment;
        private ChatRecordSoundFragment ChatRecordSoundBoxFragment;
        public ChatStickersTabFragment ChatStickersTabBoxFragment;
        private FrameLayout ButtonFragmentHolder;
        public FrameLayout TopFragmentHolder;
        private LinearLayoutManager LayoutManager;
        public MessageAdapter MAdapter;
        private SupportFragment MainFragmentOpened;
        private Methods.AudioRecorderAndPlayer RecorderService;
        private FastOutSlowInInterpolator Interpolation;
        public static string MainChatColor = AppSettings.MainColor;
        private string GifFile, PermissionsType, TypeChat, TaskWork, Notifier, ShowEmpty;
        private string LastSeen;
        public Toolbar ToolBar;
        private Timer Timer;
        private bool IsRecording;
        public static bool ColorChanged;
        public ChatObject DataUser;
        public GetUsersListObject.User DataUserChat;
        public UserDataObject UserData;
        public string Userid; // to_id
        private static ChatWindowActivity Instance;
        private TabbedMainActivity GlobalContext;
        private LinearLayout FirstLiner, FirstBoxOnButton;
        private RelativeLayout SayHiLayout;
        private RecyclerView SayHiSuggestionsRecycler;
        private EmptySuggetionRecylerAdapter SuggestionAdapter;
        //Action Bar Buttons 
        private ImageView BackButton, AudioCallButton, VideoCallButton, MoreButton;
        private TextView ActionBarTitle, ActionBarSubTitle;
        //Say Hi 
        private TextView SayHiToTextView;
        private AdapterModelsClassUser SelectedItemPositions;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                Window.SetSoftInputMode(SoftInput.AdjustResize);

                base.OnCreate(savedInstanceState);
                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);
                Userid = Intent.GetStringExtra("UserID") ?? "";
                TypeChat = Intent.GetStringExtra("TypeChat") ?? "";
                ShowEmpty = Intent.GetStringExtra("ShowEmpty") ?? "";

                Methods.App.FullScreenApp(this);

                //Set ToolBar and data chat
                FirstLoadData_Item();

                Window.SetStatusBarColor(Color.ParseColor(MainChatColor));
                SetTheme(MainChatColor);

                // Set our view from the "ChatWindow" layout resource
                SetContentView(Resource.Layout.ChatWindowLayout);

                Instance = this;
                GlobalContext = TabbedMainActivity.GetInstance();

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                SetRecyclerViewAdapters();

                var dataNotifier = Intent.GetStringExtra("Notifier") ?? "Data not available";
                if (dataNotifier != "Data not available" && !string.IsNullOrEmpty(dataNotifier))
                {
                    Notifier = dataNotifier;
                    if (Notifier == "Notifier")
                    {
                        string dataApp = Intent.GetStringExtra("App");
                        if (dataApp == "Timeline")
                        {
                            string name = Intent.GetStringExtra("Name");
                            //string username = Intent.GetStringExtra("Username");
                            //string about = Intent.GetStringExtra("About");
                            //string address = Intent.GetStringExtra("Address");
                            //string phone = Intent.GetStringExtra("Phone");
                            //string website = Intent.GetStringExtra("Website");
                            //string working = Intent.GetStringExtra("Working");
                            string time = Intent.GetStringExtra("Time");
                            LastSeen = Intent.GetStringExtra("LastSeen") ?? "off";

                            ActionBarTitle.Text = name; // user name
                            SayHiToTextView.Text = name;


                            //Online Or offline
                            if (LastSeen == "on")
                            {
                                ActionBarSubTitle.Text = GetString(Resource.String.Lbl_Online);
                                LastSeen = GetString(Resource.String.Lbl_Online);
                            }
                            else
                            {
                                ActionBarSubTitle.Text = GetString(Resource.String.Lbl_Last_seen) + " " + time;
                                LastSeen = GetString(Resource.String.Lbl_Last_seen) + " " + time;
                            }
                        }

                        Get_UserProfileData_Api();
                    }
                }

                LoadData_ItemUser();

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
                AddOrRemoveEvent(true);

                if (Timer != null)
                {
                    Timer.Enabled = true;
                    Timer.Start();
                }
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
                base.OnPause();
                AddOrRemoveEvent(false);

                if (Timer != null)
                {
                    Timer.Enabled = false;
                    Timer.Stop();
                }
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

        protected override void OnStart()
        {
            try
            {
                ResetMediaPlayer();
                base.OnStart();
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
                if (Timer != null)
                {
                    Timer.Enabled = false;
                    Timer.Stop();
                    Timer.Dispose();
                    Timer = null;
                }

                ResetMediaPlayer();

                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Menu

        private void OnMenuPhoneCallIcon_Click()
        {
            try
            {
                bool granted = ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.RecordAudio) == Permission.Granted;
                if (granted)
                {
                    StartCall();
                }
                else
                {
                    RequestPermissions(new[]
                    {
                        Manifest.Permission.RecordAudio,
                        Manifest.Permission.ModifyAudioSettings
                    }, 1106);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void StartCall()
        {
            try
            {
                string timeNow = DateTime.Now.ToString("hh:mm");
                var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string time = Convert.ToString(unixTimestamp);

                Intent intentVideoCall = new Intent(this, typeof(TwilioVideoCallActivity));
                if (AppSettings.UseAgoraLibrary && AppSettings.UseTwilioLibrary == false)
                {
                    intentVideoCall = new Intent(this, typeof(AgoraAudioCallActivity));
                    intentVideoCall.PutExtra("type", "Agora_audio_calling_start");
                }
                else if (AppSettings.UseAgoraLibrary == false && AppSettings.UseTwilioLibrary)
                {
                    intentVideoCall = new Intent(this, typeof(TwilioAudioCallActivity));
                    intentVideoCall.PutExtra("type", "Twilio_audio_calling_start");
                }

                if (DataUser != null && AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    intentVideoCall.PutExtra("UserID", DataUser.UserId);
                    intentVideoCall.PutExtra("avatar", DataUser.Avatar);
                    intentVideoCall.PutExtra("name", DataUser.Name);
                    intentVideoCall.PutExtra("time", timeNow);
                    intentVideoCall.PutExtra("CallID", time);
                    intentVideoCall.PutExtra("access_token", "YOUR_TOKEN");
                    intentVideoCall.PutExtra("access_token_2", "YOUR_TOKEN");
                    intentVideoCall.PutExtra("from_id", "0");
                    intentVideoCall.PutExtra("active", "0");
                    intentVideoCall.PutExtra("status", "0");
                    intentVideoCall.PutExtra("room_name", "TestRoom");
                }
                else if (DataUserChat != null && AppSettings.LastChatSystem == SystemApiGetLastChat.Old)
                {
                    intentVideoCall.PutExtra("UserID", DataUserChat.UserId);
                    intentVideoCall.PutExtra("avatar", DataUserChat.Avatar);
                    intentVideoCall.PutExtra("name", DataUserChat.Name);
                    intentVideoCall.PutExtra("time", timeNow);
                    intentVideoCall.PutExtra("CallID", time);
                    intentVideoCall.PutExtra("access_token", "YOUR_TOKEN");
                    intentVideoCall.PutExtra("access_token_2", "YOUR_TOKEN");
                    intentVideoCall.PutExtra("from_id", "0");
                    intentVideoCall.PutExtra("active", "0");
                    intentVideoCall.PutExtra("status", "0");
                    intentVideoCall.PutExtra("room_name", "TestRoom");
                }
                else if (UserData != null)
                {
                    intentVideoCall.PutExtra("UserID", UserData.UserId);
                    intentVideoCall.PutExtra("avatar", UserData.Avatar);
                    intentVideoCall.PutExtra("name", UserData.Name);
                    intentVideoCall.PutExtra("time", timeNow);
                    intentVideoCall.PutExtra("CallID", time);
                    intentVideoCall.PutExtra("access_token", "YOUR_TOKEN");
                    intentVideoCall.PutExtra("access_token_2", "YOUR_TOKEN");
                    intentVideoCall.PutExtra("from_id", "0");
                    intentVideoCall.PutExtra("active", "0");
                    intentVideoCall.PutExtra("status", "0");
                    intentVideoCall.PutExtra("room_name", "TestRoom");
                }

                StartActivity(intentVideoCall);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void StartVideoCall()
        {
            try
            {
                string timeNow = DateTime.Now.ToString("hh:mm");
                var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string time = Convert.ToString(unixTimestamp);

                Intent intentVideoCall = new Intent(this, typeof(TwilioVideoCallActivity));
                if (AppSettings.UseAgoraLibrary && AppSettings.UseTwilioLibrary == false)
                {
                    intentVideoCall = new Intent(this, typeof(AgoraVideoCallActivity));
                    intentVideoCall.PutExtra("type", "Agora_video_calling_start");
                }
                else if (AppSettings.UseAgoraLibrary == false && AppSettings.UseTwilioLibrary)
                {
                    intentVideoCall = new Intent(this, typeof(TwilioVideoCallActivity));
                    intentVideoCall.PutExtra("type", "Twilio_video_calling_start");
                }

                if (DataUser != null && AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    intentVideoCall.PutExtra("UserID", DataUser.UserId);
                    intentVideoCall.PutExtra("avatar", DataUser.Avatar);
                    intentVideoCall.PutExtra("name", DataUser.Name);
                    intentVideoCall.PutExtra("time", timeNow);
                    intentVideoCall.PutExtra("CallID", time);
                    intentVideoCall.PutExtra("access_token", "YOUR_TOKEN");
                    intentVideoCall.PutExtra("access_token_2", "YOUR_TOKEN");
                    intentVideoCall.PutExtra("from_id", "0");
                    intentVideoCall.PutExtra("active", "0");
                    intentVideoCall.PutExtra("status", "0");
                    intentVideoCall.PutExtra("room_name", "TestRoom");
                }
                else if (DataUserChat != null && AppSettings.LastChatSystem == SystemApiGetLastChat.Old)
                {
                    intentVideoCall.PutExtra("UserID", DataUserChat.UserId);
                    intentVideoCall.PutExtra("avatar", DataUserChat.Avatar);
                    intentVideoCall.PutExtra("name", DataUserChat.Name);
                    intentVideoCall.PutExtra("time", timeNow);
                    intentVideoCall.PutExtra("CallID", time);
                    intentVideoCall.PutExtra("access_token", "YOUR_TOKEN");
                    intentVideoCall.PutExtra("access_token_2", "YOUR_TOKEN");
                    intentVideoCall.PutExtra("from_id", "0");
                    intentVideoCall.PutExtra("active", "0");
                    intentVideoCall.PutExtra("status", "0");
                    intentVideoCall.PutExtra("room_name", "TestRoom");
                }
                else if (UserData != null)
                {
                    intentVideoCall.PutExtra("UserID", UserData.UserId);
                    intentVideoCall.PutExtra("avatar", UserData.Avatar);
                    intentVideoCall.PutExtra("name", UserData.Name);
                    intentVideoCall.PutExtra("time", timeNow);
                    intentVideoCall.PutExtra("CallID", time);
                    intentVideoCall.PutExtra("access_token", "YOUR_TOKEN");
                    intentVideoCall.PutExtra("access_token_2", "YOUR_TOKEN");
                    intentVideoCall.PutExtra("from_id", "0");
                    intentVideoCall.PutExtra("active", "0");
                    intentVideoCall.PutExtra("status", "0");
                    intentVideoCall.PutExtra("room_name", "TestRoom");
                }

                StartActivity(intentVideoCall);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnMenuVideoCallIcon_Click()
        {
            try
            {
                bool granted = ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) == Permission.Granted && ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.RecordAudio) == Permission.Granted;
                if (granted)
                {
                    StartVideoCall();
                }
                else
                {
                    RequestPermissions(new[] {
                        Manifest.Permission.Camera,
                        Manifest.Permission.RecordAudio,
                        Manifest.Permission.ModifyAudioSettings
                    }, 1107);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //view Profile action!
        private void OnMenuViewProfile_Click()
        {
            try
            {
                if (DataUser != null && AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    WoWonderTools.OpenProfile(this, Userid, DataUser.UserData, "Chat");
                }
                else if (DataUserChat != null && AppSettings.LastChatSystem == SystemApiGetLastChat.Old)
                {
                    WoWonderTools.OpenProfile(this, Userid, DataUserChat, "Chat");
                }
                else if (UserData != null)
                {
                    WoWonderTools.OpenProfile(this, Userid, UserData, "Chat");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void OnMenuBlock_Click()
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    (int apiStatus, var respond) = await RequestsAsync.Global.Block_User(Userid, true); //true >> "block" 
                    if (apiStatus == 200)
                    {
                        Console.WriteLine(respond);

                        var dbDatabase = new SqLiteDatabase();
                        dbDatabase.Delete_UsersContact(Userid);
                        dbDatabase.Dispose();

                        Toast.MakeText(this, GetString(Resource.String.Lbl_Blocked_successfully), ToastLength.Short).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnMenuClearChat_Click()
        {
            try
            {
                var dialog = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);
                dialog.Title(GetText(Resource.String.Lbl_Clear_chat));
                dialog.Content(GetText(Resource.String.Lbl_AreYouSureDeleteMessages));
                dialog.PositiveText(GetText(Resource.String.Lbl_Yes)).OnPositive((materialDialog, action) =>
                {
                    try
                    {
                        MAdapter.DifferList.Clear();
                        MAdapter.NotifyDataSetChanged();

                        if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                        {
                            var userToDelete = GlobalContext?.LastChatTab?.MAdapter?.ChatList?.FirstOrDefault(a => a.UserId == Userid);
                            if (userToDelete != null)
                            {
                                var index = GlobalContext.LastChatTab.MAdapter.ChatList.IndexOf(userToDelete);
                                if (index != -1)
                                {
                                    GlobalContext?.LastChatTab.MAdapter.ChatList.Remove(userToDelete);
                                    GlobalContext?.LastChatTab.MAdapter.NotifyItemRemoved(index);
                                }
                            }
                        }
                        else
                        {
                            var userToDelete = GlobalContext?.LastMessagesTab?.MAdapter?.MLastMessagesUser?.FirstOrDefault(a => a.UserId == Userid);
                            if (userToDelete != null)
                            {
                                var index = GlobalContext.LastMessagesTab.MAdapter.MLastMessagesUser.IndexOf(userToDelete);
                                if (index != -1)
                                {
                                    GlobalContext?.LastMessagesTab.MAdapter.MLastMessagesUser.Remove(userToDelete);
                                    GlobalContext?.LastMessagesTab.MAdapter.NotifyItemRemoved(index);
                                }
                            }
                        }

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();
                        dbDatabase.DeleteAllMessagesUser(UserDetails.UserId, Userid);
                        dbDatabase.Dispose();

                        if (Methods.CheckConnectivity())
                        {
                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Delete_Conversation(Userid) });
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
                dialog.NegativeText(GetText(Resource.String.Lbl_No)).OnNegative(this);
                dialog.AlwaysCallSingleChoiceCallback();
                dialog.Build().Show();
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
                //Audio FrameWork initialize 
                RecorderService = new Methods.AudioRecorderAndPlayer(Userid);

                Interpolation = new FastOutSlowInInterpolator();

                ChatColorBoxFragment = new ChatColorsFragment();
                Bundle args = new Bundle();
                args.PutString("userid", Userid);
                ChatColorBoxFragment.Arguments = args;

                ChatRecordSoundBoxFragment = new ChatRecordSoundFragment();
                ChatStickersTabBoxFragment = new ChatStickersTabFragment();
                //Say Hi 
                SayHiLayout = FindViewById<RelativeLayout>(Resource.Id.SayHiLayout);
                SayHiSuggestionsRecycler = FindViewById<RecyclerView>(Resource.Id.recylerHiSuggetions);
                SayHiToTextView = FindViewById<TextView>(Resource.Id.toUserText);
                //User Info 
                ActionBarTitle = FindViewById<TextView>(Resource.Id.Txt_Username);
                ActionBarSubTitle = FindViewById<TextView>(Resource.Id.Txt_last_time);
                //ActionBarButtons
                BackButton = FindViewById<ImageView>(Resource.Id.BackButton);
                AudioCallButton = FindViewById<ImageView>(Resource.Id.IconCall);
                VideoCallButton = FindViewById<ImageView>(Resource.Id.IconvideoCall);
                MoreButton = FindViewById<ImageView>(Resource.Id.IconMore);
                UserChatProfile = FindViewById<CircleImageView>(Resource.Id.userProfileImage);
                RootView = FindViewById<LinearLayout>(Resource.Id.rootChatWindowView);
                ChatEmojImage = FindViewById<AppCompatImageView>(Resource.Id.emojiicon);
                EmojIconEditTextView = FindViewById<EmojiconEditText>(Resource.Id.EmojiconEditText5);
                ChatSendButton = FindViewById<CircleButton>(Resource.Id.sendButton);
                MRecycler = FindViewById<RecyclerView>(Resource.Id.recyler);
                ChatColorButton = FindViewById<ImageView>(Resource.Id.colorButton);
                ChatStickerButton = FindViewById<ImageView>(Resource.Id.stickerButton);
                ChatMediaButton = FindViewById<ImageView>(Resource.Id.mediaButton);
                ButtonFragmentHolder = FindViewById<FrameLayout>(Resource.Id.ButtomFragmentHolder);
                TopFragmentHolder = FindViewById<FrameLayout>(Resource.Id.TopFragmentHolder);
                FirstLiner = FindViewById<LinearLayout>(Resource.Id.firstLiner);
                FirstBoxOnButton = FindViewById<LinearLayout>(Resource.Id.firstBoxonButtom);

                SupportFragmentManager.BeginTransaction().Add(ButtonFragmentHolder.Id, ChatColorBoxFragment, "ChatColorBoxFragment");
                SupportFragmentManager.BeginTransaction().Add(TopFragmentHolder.Id, ChatRecordSoundBoxFragment, "Chat_Recourd_Sound_Fragment");

                if (ShowEmpty == "no")
                {
                    SayHiLayout.Visibility = ViewStates.Gone;
                    SayHiSuggestionsRecycler.Visibility = ViewStates.Gone;
                }

                if (!AppSettings.EnableAudioCall)
                    AudioCallButton.Visibility = ViewStates.Gone;

                if (!AppSettings.EnableVideoCall)
                    VideoCallButton.Visibility = ViewStates.Gone;

                if (!AppSettings.EnableAudioVideoCall)
                {
                    AudioCallButton.Visibility = ViewStates.Gone;
                    VideoCallButton.Visibility = ViewStates.Gone;
                }

                if (AppSettings.SetTabDarkTheme)
                {
                    TopFragmentHolder.SetBackgroundColor(Color.ParseColor("#282828"));
                    FirstLiner.SetBackgroundColor(Color.ParseColor("#282828"));
                    FirstBoxOnButton.SetBackgroundColor(Color.ParseColor("#282828"));

                }
                else
                {
                    TopFragmentHolder.SetBackgroundColor(Color.White);
                    FirstLiner.SetBackgroundColor(Color.White);
                    FirstBoxOnButton.SetBackgroundColor(Color.White);
                }

                if (AppSettings.ShowButtonRecordSound)
                {
                    ChatSendButton.LongClickable = true;
                    ChatSendButton.Tag = "Free";
                    ChatSendButton.SetImageResource(Resource.Drawable.microphone);
                }
                else
                {
                    ChatSendButton.Tag = "Text";
                    ChatSendButton.SetImageResource(Resource.Drawable.SendLetter);
                }

                ChatSendButton.SetColor(Color.ParseColor(MainChatColor));

                if (AppSettings.ShowButtonStickers)
                {
                    ChatStickerButton.Visibility = ViewStates.Visible;
                    ChatStickerButton.Tag = "Closed";
                }
                else
                {
                    ChatStickerButton.Visibility = ViewStates.Gone;
                }

                //ChatContactButton.Visibility = AppSettings.ShowButtonContact ? ViewStates.Visible : ViewStates.Gone;

                if (AppSettings.ShowButtonColor)
                {
                    ChatColorButton.Visibility = ViewStates.Visible;
                    ChatColorButton.Tag = "Closed";
                }
                else
                {
                    ChatColorButton.Visibility = ViewStates.Gone;
                }

                if (AppSettings.SetTabDarkTheme)
                {
                    EmojIconEditTextView.SetTextColor(Color.White);
                    EmojIconEditTextView.SetHintTextColor(Color.White);
                }
                else
                {
                    EmojIconEditTextView.SetTextColor(Color.ParseColor("#444444"));
                    EmojIconEditTextView.SetHintTextColor(Color.ParseColor("#444444"));
                }

                var emoticon = new EmojIconActions(this, RootView, EmojIconEditTextView, ChatEmojImage);
                emoticon.ShowEmojIcon();
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
                ToolBar = FindViewById<Toolbar>(Resource.Id.toolbar);
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
                MAdapter = new MessageAdapter(this, Userid) { DifferList = new ObservableCollection<AdapterModelsClassUser>() };

                LayoutManager = new LinearLayoutManager(this);
                MRecycler.SetLayoutManager(LayoutManager);
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(10);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                MRecycler.SetAdapter(MAdapter);

                XamarinRecyclerViewOnScrollListener xamarinRecyclerViewOnScrollListener = new XamarinRecyclerViewOnScrollListener(LayoutManager, null);
                xamarinRecyclerViewOnScrollListener.LoadMoreEvent += OnScrollLoadMoreFromTop_Event;
                MRecycler.AddOnScrollListener(xamarinRecyclerViewOnScrollListener);


                RecyclerHiSuggestion = FindViewById<RecyclerView>(Resource.Id.recylerHiSuggetions);
                SuggestionAdapter = new EmptySuggetionRecylerAdapter(this);
                RecyclerHiSuggestion.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Horizontal, false));
                RecyclerHiSuggestion.SetAdapter(SuggestionAdapter);
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
                    ChatSendButton.Touch += ChatSendButtonOnTouch;
                    ChatMediaButton.Click += ChatMediaButtonOnClick;
                    ChatStickerButton.Click += ChatStickerButtonOnClick;
                    ChatSendButton.LongClick += ChatSendButtonOnLongClick;
                    EmojIconEditTextView.TextChanged += EmojIconEditTextViewOnTextChanged;
                    ChatColorButton.Click += ChatColorButtonOnClick;
                    //ActionBar Buttons
                    BackButton.Click += BackButton_Click;
                    AudioCallButton.Click += AudioCallButton_Click;
                    VideoCallButton.Click += VideoCallButton_Click;
                    MoreButton.Click += MoreButton_Click;
                    UserChatProfile.Click += UserChatProfile_Click;
                    ActionBarTitle.Click += UserChatProfile_Click;
                    ActionBarSubTitle.Click += UserChatProfile_Click;
                    SuggestionAdapter.OnItemClick += SuggestionAdapterOnItemClick;
                    MAdapter.ItemClick += MAdapterOnItemClick;
                    MAdapter.ItemLongClick += MAdapterOnItemLongClick;
                }
                else
                {
                    ChatSendButton.Touch -= ChatSendButtonOnTouch;
                    ChatMediaButton.Click -= ChatMediaButtonOnClick;
                    ChatStickerButton.Click -= ChatStickerButtonOnClick;
                    ChatSendButton.LongClick -= ChatSendButtonOnLongClick;

                    EmojIconEditTextView.TextChanged -= EmojIconEditTextViewOnTextChanged;
                    ChatColorButton.Click -= ChatColorButtonOnClick;
                    //ActionBar Buttons
                    BackButton.Click -= BackButton_Click;
                    AudioCallButton.Click -= AudioCallButton_Click;
                    VideoCallButton.Click -= VideoCallButton_Click;
                    MoreButton.Click -= MoreButton_Click;
                    UserChatProfile.Click -= UserChatProfile_Click;
                    ActionBarTitle.Click -= UserChatProfile_Click;
                    ActionBarSubTitle.Click -= UserChatProfile_Click;
                    SuggestionAdapter.OnItemClick -= SuggestionAdapterOnItemClick;
                    MAdapter.ItemClick -= MAdapterOnItemClick;
                    MAdapter.ItemLongClick -= MAdapterOnItemLongClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static ChatWindowActivity GetInstance()
        {
            try
            {
                return Instance;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        #endregion

        #region Events

        private void SuggestionAdapterOnItemClick(object sender, AdapterClickEvents e)
        {
            try
            {
                var position = e.Position;
                var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string time2 = unixTimestamp.ToString(CultureInfo.InvariantCulture);
                MessageData m1 = new MessageData
                {
                    Id = time2,
                    FromId = UserDetails.UserId,
                    ToId = Userid,
                    Text = SuggestionAdapter.GetItem(position).RealMessage,
                    Position = "right",
                    Seen = "0",
                    Time = time2,
                    ModelType = MessageModelType.RightText,
                    TimeText = DateTime.Now.ToShortTimeString(),
                };

                MAdapter.DifferList.Add(new AdapterModelsClassUser()
                {
                    TypeView = MessageModelType.RightText,
                    Id = Long.ParseLong(m1.Id),
                    MesData = m1
                });

                var indexMes = MAdapter.DifferList.IndexOf(MAdapter.DifferList.Last());
                MAdapter.NotifyItemInserted(indexMes);

                //Scroll Down >> 
                MRecycler.ScrollToPosition(MAdapter.ItemCount - 1);

                if (Methods.CheckConnectivity())
                {
                    Task.Factory.StartNew(() =>
                    {
                        MessageController.SendMessageTask(this, Userid, time2, SuggestionAdapter.GetItem(position).RealMessage).ConfigureAwait(false);
                    });
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }

                SayHiLayout.Visibility = ViewStates.Gone;
                SayHiSuggestionsRecycler.Visibility = ViewStates.Gone;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        private void MAdapterOnItemClick(object sender, Holders.MesClickEventArgs e)
        {
            try
            {
                if (e.Position <= -1) return;
                var item = MAdapter.GetItem(e.Position);
                if (item != null)
                {
                    if (e.Type == Holders.TypeClick.Text || e.Type == Holders.TypeClick.Contact)
                    {
                        item.MesData.ShowTimeText = !item.MesData.ShowTimeText;
                        MAdapter.NotifyItemChanged(MAdapter.DifferList.IndexOf(item));
                    }
                    else if (e.Type == Holders.TypeClick.File)
                    {
                        var fileName = item.MesData.Media.Split('/').Last();
                        string imageFile = Methods.MultiMedia.CheckFileIfExits(item.MesData.Media);
                        if (imageFile != "File Dont Exists")
                        {
                            try
                            {
                                var extension = fileName.Split('.').Last();
                                string mimeType = MimeTypeMap.GetMimeType(extension);

                                Intent openFile = new Intent();
                                openFile.SetFlags(ActivityFlags.NewTask);
                                openFile.SetFlags(ActivityFlags.GrantReadUriPermission);
                                openFile.SetAction(Intent.ActionView);
                                openFile.SetDataAndType(Uri.Parse(imageFile), mimeType);
                                StartActivity(openFile);
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception);
                            }
                        }
                        else
                        {
                            var extension = fileName.Split('.').Last();
                            string mimeType = MimeTypeMap.GetMimeType(extension);

                            Intent i = new Intent(Intent.ActionView);
                            i.SetData(Uri.Parse(item.MesData.Media));
                            i.SetType(mimeType);
                            StartActivity(i);
                            // Toast.MakeText(MainActivity, MainActivity.GetText(Resource.String.Lbl_Something_went_wrong), ToastLength.Long).Show();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        private void MAdapterOnItemLongClick(object sender, Holders.MesClickEventArgs e)
        {
            try
            {
                if (e.Position > -1)
                {
                    SelectedItemPositions = MAdapter.GetItem(e.Position);
                    if (SelectedItemPositions != null)
                    {
                        var arrayAdapter = new List<string>();
                        var dialogList = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                        if (e.Type == Holders.TypeClick.Text)
                            arrayAdapter.Add(GetText(Resource.String.Lbl_Copy));

                        if (SelectedItemPositions.MesData.Position == "right")
                            arrayAdapter.Add(GetText(Resource.String.Lbl_MessageInfo));

                        arrayAdapter.Add(GetText(Resource.String.Lbl_Forward));

                        dialogList.Items(arrayAdapter);
                        dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                        dialogList.AlwaysCallSingleChoiceCallback();
                        dialogList.ItemsCallback(this).Build().Show();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        private void UserChatProfile_Click(object sender, EventArgs e)
        {
            OnMenuViewProfile_Click();
        }

        private void MoreButton_Click(object sender, EventArgs e)
        {
            try
            {
                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                arrayAdapter.Add(GetText(Resource.String.Lbl_View_Profile));
                arrayAdapter.Add(GetText(Resource.String.Lbl_Block));
                arrayAdapter.Add(GetText(Resource.String.Lbl_Clear_chat));

                dialogList.Title(GetString(Resource.String.Lbl_Menu_More));
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

        private void VideoCallButton_Click(object sender, EventArgs e)
        {
            try
            {
                OnMenuVideoCallIcon_Click();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void AudioCallButton_Click(object sender, EventArgs e)
        {
            try
            {
                OnMenuPhoneCallIcon_Click();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            try
            {
                Finish();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        private void ChatColorButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (ChatColorButton.Tag.ToString() == "Closed")
                {
                    ResetButtonTags();
                    ChatColorButton.Tag = "Opened";
                    ChatColorButton.Drawable.SetTint(Color.ParseColor(AppSettings.MainColor));
                    ReplaceButtonFragment(ChatColorBoxFragment);
                }
                else
                {
                    ResetButtonTags();
                    ChatColorButton.Drawable.SetTint(Color.ParseColor("#888888"));
                    TopFragmentHolder.Animate().SetInterpolator(Interpolation).TranslationY(1200).SetDuration(300);
                    SupportFragmentManager.BeginTransaction().Remove(ChatColorBoxFragment).Commit();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        private void EmojIconEditTextViewOnTextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (AppSettings.ShowButtonRecordSound)
                {
                    if (!ButtonFragmentHolder.TranslationY.Equals(1200))
                        ButtonFragmentHolder.TranslationY = 1200;

                    if (IsRecording && EmojIconEditTextView.Text == GetString(Resource.String.Lbl_Recording))
                    {
                        ChatSendButton.Tag = "Text";
                        ChatSendButton.SetImageResource(Resource.Drawable.SendLetter);

                        EditTextOpen();
                    }
                    else if (!string.IsNullOrEmpty(EmojIconEditTextView.Text))
                    {
                        ChatSendButton.Tag = "Text";
                        ChatSendButton.SetImageResource(Resource.Drawable.SendLetter);

                        EditTextOpen();


                    }
                    else if (IsRecording)
                    {
                        ChatSendButton.Tag = "Text";
                        ChatSendButton.SetImageResource(Resource.Drawable.SendLetter);


                        EditTextOpen();
                    }
                    else
                    {
                        ChatSendButton.Tag = "Free";
                        ChatSendButton.SetImageResource(Resource.Drawable.microphone);

                        EditTextClose();
                    }
                }
                else
                {
                    ChatSendButton.Tag = "Text";
                    ChatSendButton.SetImageResource(Resource.Drawable.SendLetter);

                    EditTextOpen();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        private void EditTextClose()
        {

            ChatMediaButton.SetImageResource(Resource.Drawable.attach);
            ChatMediaButton.SetColorFilter(Color.ParseColor("#444444"));
            ChatMediaButton.Tag = "attachment";
            ViewGroup.LayoutParams layoutParams = ChatMediaButton.LayoutParameters;
            layoutParams.Width = 52;
            layoutParams.Height = 52;
            ChatMediaButton.LayoutParameters = layoutParams;
            ChatStickerButton.Visibility = ViewStates.Visible;
            ChatColorButton.Visibility = ViewStates.Visible;

        }

        private void EditTextOpen()
        {
            ChatMediaButton.SetImageResource(Resource.Drawable.ic_next);
            ChatMediaButton.SetColorFilter(Color.ParseColor(AppSettings.MainColor));
            ChatMediaButton.Tag = "arrow";
            ChatStickerButton.Visibility = ViewStates.Gone;
            ChatColorButton.Visibility = ViewStates.Gone;
            ViewGroup.LayoutParams layoutParams = ChatMediaButton.LayoutParameters;
            layoutParams.Width = 42;
            layoutParams.Height = 42;
            ChatMediaButton.LayoutParameters = layoutParams;
        }

        //Show Load More Event when scroll to the top Of Recycle
        private async void OnScrollLoadMoreFromTop_Event(object sender, EventArgs e)
        {
            try
            {
                if (RunLoadMore)
                    return;

                //Start Loader Get from Database or API Request >>
                //SwipeRefreshLayout.Refreshing = true;
                //SwipeRefreshLayout.Enabled = true;

                //Code get first Message id where LoadMore >>
                var local = LoadMore_Messages_Database();
                if (local != "1")
                    await LoadMoreMessages_API();

                //SwipeRefreshLayout.Refreshing = false;
                //SwipeRefreshLayout.Enabled = false;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Run Timer
        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                RunOnUiThread(MessageUpdater);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Intent Contact (result is 506 , Permissions is 101 )
        private void ChatContactButtonOnClick()
        {
            try
            {
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    //request code of result is 506
                    new IntentController(this).OpenIntentGetContactNumberPhone();
                }
                else
                {
                    //Check to see if any permission in our group is available, if one, then all are
                    if (CheckSelfPermission(Manifest.Permission.ReadContacts) == Permission.Granted)
                    {
                        //request code of result is 506
                        new IntentController(this).OpenIntentGetContactNumberPhone();
                    }
                    else
                    {
                        //101 >> ReadContacts && ReadPhoneNumbers
                        new PermissionsController(this).RequestPermission(101);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Send Sticker
        private void ChatStickerButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (ChatStickerButton.Tag.ToString() == "Closed")
                {
                    ResetButtonTags();
                    ChatStickerButton.Tag = "Opened";
                    ChatStickerButton.Drawable.SetTint(Color.ParseColor(AppSettings.MainColor));
                    ReplaceButtonFragment(ChatStickersTabBoxFragment);
                }
                else
                {
                    ResetButtonTags();
                    ChatStickerButton.Drawable.SetTint(Color.ParseColor("#888888"));
                    TopFragmentHolder.Animate().SetInterpolator(Interpolation).TranslationY(1200).SetDuration(300);
                    SupportFragmentManager.BeginTransaction().Remove(ChatStickersTabBoxFragment).Commit();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //record voices ( Permissions is 102 )
        private async void ChatSendButtonOnLongClick(object sender, View.LongClickEventArgs e)
        {
            try
            {
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    if (ChatSendButton.Tag.ToString() == "Free")
                    {
                        //Set Record Style
                        IsRecording = true;

                        if (SettingsPrefFragment.SSoundControl)
                            Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("RecourdVoiceButton.mp3");

                        if (EmojIconEditTextView.Text != GetString(Resource.String.Lbl_Recording))
                        {
                            EmojIconEditTextView.Text = GetString(Resource.String.Lbl_Recording);
                            EmojIconEditTextView.SetTextColor(Color.ParseColor("#FA3C4C"));
                        }

                        ChatSendButton.SetColor(Color.ParseColor("#FA3C4C"));
                        ChatSendButton.SetImageResource(Resource.Drawable.ic_stop_white_24dp);

                        RecorderService = new Methods.AudioRecorderAndPlayer(Userid);
                        //Start Audio record
                        await Task.Delay(600);
                        RecorderService.StartRecourding();
                    }
                }
                else
                {
                    //Check to see if any permission in our group is available, if one, then all are
                    if (CheckSelfPermission(Manifest.Permission.RecordAudio) == Permission.Granted)
                    {
                        if (ChatSendButton.Tag.ToString() == "Free")
                        {
                            //Set Record Style
                            IsRecording = true;

                            if (SettingsPrefFragment.SSoundControl)
                                Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("RecourdVoiceButton.mp3");

                            if (EmojIconEditTextView.Text != GetString(Resource.String.Lbl_Recording))
                            {
                                EmojIconEditTextView.Text = GetString(Resource.String.Lbl_Recording);
                                EmojIconEditTextView.SetTextColor(Color.ParseColor("#FA3C4C"));
                            }

                            ChatSendButton.SetColor(Color.ParseColor("#FA3C4C"));
                            ChatSendButton.SetImageResource(Resource.Drawable.ic_stop_white_24dp);

                            RecorderService = new Methods.AudioRecorderAndPlayer(Userid);
                            //Start Audio record
                            await Task.Delay(600);
                            RecorderService.StartRecourding();
                        }
                    }
                    else
                    {
                        new PermissionsController(this).RequestPermission(102);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        // Event sent media (image , Camera , video , file , music )
        private void ChatMediaButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (ChatMediaButton.Tag.ToString() == "attachment")
                {
                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                    if (AppSettings.ShowButtonImage)
                        arrayAdapter.Add(GetText(Resource.String.Btn_Image));
                    if (AppSettings.ShowButtonCamera)
                        arrayAdapter.Add(GetText(Resource.String.Camera));
                    if (AppSettings.ShowButtonVideo)
                        arrayAdapter.Add(GetText(Resource.String.Btn_Video));
                    if (AppSettings.ShowButtonAttachFile && WoWonderTools.CheckAllowedFileSharingInServer())
                        arrayAdapter.Add(GetText(Resource.String.Lbl_File));
                    if (AppSettings.ShowButtonMusic && WoWonderTools.CheckAllowedFileSharingInServer())
                        arrayAdapter.Add(GetText(Resource.String.Lbl_Music));
                    if (AppSettings.ShowButtonGif)
                        arrayAdapter.Add(GetText(Resource.String.Lbl_Gif));
                    if (AppSettings.ShowButtonContact)
                        arrayAdapter.Add(GetText(Resource.String.Lbl_Contact));

                    dialogList.Title(GetString(Resource.String.Lbl_Select_what_you_want));
                    dialogList.Items(arrayAdapter);
                    dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show();
                }
                else if (ChatMediaButton.Tag.ToString() == "arrow")
                {
                    EditTextClose();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void ChatSendButtonOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                var handled = false;

                if (e.Event.Action == MotionEventActions.Down)
                {
                    OnClick_OfSendButton();
                }

                if (e.Event.Action == MotionEventActions.Up)
                {
                    try
                    {
                        if (IsRecording)
                        {
                            RecorderService.StopRecourding();
                            var filePath = RecorderService.GetRecorded_Sound_Path();

                            ChatSendButton.SetColor(Color.ParseColor(MainChatColor));
                            ChatSendButton.SetImageResource(Resource.Drawable.SendLetter);

                            if (EmojIconEditTextView.Text == GetString(Resource.String.Lbl_Recording))
                            {
                                if (!string.IsNullOrEmpty(filePath))
                                {
                                    Bundle bundle = new Bundle();
                                    bundle.PutString("FilePath", filePath);
                                    ChatRecordSoundBoxFragment.Arguments = bundle;
                                    ReplaceTopFragment(ChatRecordSoundBoxFragment);
                                }

                                EmojIconEditTextView.Text = "";
                                EmojIconEditTextView.SetTextColor(Color.ParseColor("#444444"));
                            }

                            IsRecording = false;
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }

                    ChatSendButton.Pressed = false;
                    handled = true;
                }

                e.Handled = handled;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Send Message type => "right_audio" Or "right_text"
        private void OnClick_OfSendButton()
        {
            try
            {
                IsRecording = false;

                var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string time2 = unixTimestamp.ToString(CultureInfo.InvariantCulture);

                if (ChatSendButton.Tag.ToString() == "Audio")
                {
                    var interTortola = new FastOutSlowInInterpolator();
                    TopFragmentHolder.Animate().SetInterpolator(interTortola).TranslationY(1200).SetDuration(300);
                    SupportFragmentManager.BeginTransaction().Remove(ChatRecordSoundBoxFragment).Commit();

                    string filePath = RecorderService.GetRecorded_Sound_Path();
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        MessageData m1 = new MessageData
                        {
                            Id = time2,
                            FromId = UserDetails.UserId,
                            ToId = Userid,
                            Media = filePath,
                            Seen = "0",
                            Time = time2,
                            Position = "right",
                            TimeText = GetText(Resource.String.Lbl_Uploading),
                            MediaDuration = Methods.AudioRecorderAndPlayer.GetTimeString(Methods.AudioRecorderAndPlayer.Get_MediaFileDuration(filePath)),
                            ModelType = MessageModelType.RightAudio
                        };

                        MAdapter.DifferList.Add(new AdapterModelsClassUser()
                        {
                            TypeView = MessageModelType.RightAudio,
                            Id = Long.ParseLong(m1.Id),
                            MesData = m1
                        });

                        var indexMes = MAdapter.DifferList.IndexOf(MAdapter.DifferList.Last());
                        MAdapter.NotifyItemInserted(indexMes);

                        //Scroll Down >> 
                        MRecycler.ScrollToPosition(MAdapter.ItemCount - 1);

                        //Here on This function will send Selected audio file to the user 
                        if (Methods.CheckConnectivity())
                        {
                            Task.Factory.StartNew(() =>
                            {
                                MessageController.SendMessageTask(this, Userid, time2, EmojIconEditTextView.Text, "", filePath).ConfigureAwait(false);
                            });
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                        }
                    }

                    ChatSendButton.Tag = "Free";
                    ChatSendButton.SetImageResource(Resource.Drawable.microphone);

                }
                else if (ChatSendButton.Tag.ToString() == "Text")
                {
                    if (string.IsNullOrEmpty(EmojIconEditTextView.Text))
                    {

                    }
                    else
                    {
                        //Hide SayHi And Suggestion
                        SayHiLayout.Visibility = ViewStates.Gone;
                        SayHiSuggestionsRecycler.Visibility = ViewStates.Gone;
                        //Here on This function will send Text Messages to the user 

                        //remove \n in a string
                        string replacement = Regex.Replace(EmojIconEditTextView.Text, @"\t|\n|\r", "");

                        MessageData m1 = new MessageData
                        {
                            Id = time2,
                            FromId = UserDetails.UserId,
                            ToId = Userid,
                            Text = replacement,
                            Position = "right",
                            Seen = "0",
                            Time = time2,
                            ModelType = MessageModelType.RightText,
                            TimeText = DateTime.Now.ToShortTimeString(),
                        };

                        MAdapter.DifferList.Add(new AdapterModelsClassUser()
                        {
                            TypeView = MessageModelType.RightText,
                            Id = Long.ParseLong(m1.Id),
                            MesData = m1
                        });

                        var indexMes = MAdapter.DifferList.IndexOf(MAdapter.DifferList.Last());
                        MAdapter.NotifyItemInserted(indexMes);

                        //Scroll Down >> 
                        MRecycler.ScrollToPosition(MAdapter.ItemCount - 1);

                        if (Methods.CheckConnectivity())
                        {
                            Task.Factory.StartNew(() =>
                            {
                                MessageController.SendMessageTask(this, Userid, time2, EmojIconEditTextView.Text).ConfigureAwait(false);
                            });
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                        }

                        EmojIconEditTextView.Text = "";
                    }

                    if (AppSettings.ShowButtonRecordSound)
                    {
                        ChatSendButton.SetImageResource(Resource.Drawable.microphone);
                        ChatSendButton.Tag = "Free";
                    }
                    else
                    {
                        ChatSendButton.Tag = "Text";
                        ChatSendButton.SetImageResource(Resource.Drawable.SendLetter);
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
                string timeNow = DateTime.Now.ToShortTimeString();
                var unixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string time2 = Convert.ToString(unixTimestamp);


                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == 506 && resultCode == Result.Ok) // right_contact
                {
                    var contact = Methods.PhoneContactManager.Get_ContactInfoBy_Id(data.Data.LastPathSegment);
                    if (contact != null)
                    {
                        var name = contact.UserDisplayName;
                        var phone = contact.PhoneNumber;

                        MessageData m1 = new MessageData
                        {
                            Id = time2,
                            FromId = UserDetails.UserId,
                            ToId = Userid,
                            ContactName = name,
                            ContactNumber = phone,
                            TimeText = timeNow,
                            Position = "right",
                            Seen = "0",
                            Time = time2,
                            ModelType = MessageModelType.RightContact
                        };
                        MAdapter.DifferList.Add(new AdapterModelsClassUser()
                        {
                            TypeView = MessageModelType.RightContact,
                            Id = Long.ParseLong(m1.Id),
                            MesData = m1
                        });

                        var indexMes = MAdapter.DifferList.IndexOf(MAdapter.DifferList.Last());
                        MAdapter.NotifyItemInserted(indexMes);

                        //Scroll Down >> 
                        MRecycler.ScrollToPosition(MAdapter.ItemCount - 1);

                        var dictionary = new Dictionary<string, string>();

                        if (!dictionary.ContainsKey(name))
                        {
                            dictionary.Add(name, phone);
                        }

                        string dataContact = JsonConvert.SerializeObject(dictionary.ToArray().FirstOrDefault(a => a.Key == name));

                        if (Methods.CheckConnectivity())
                        {
                            //Send contact function
                            Task.Factory.StartNew(() =>
                            {
                                MessageController.SendMessageTask(this, Userid, time2, dataContact, "1").ConfigureAwait(false);
                            });
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                        }
                    }
                }
                else if (requestCode == 500 && resultCode == Result.Ok) // right_image 
                {
                    var filepath = Methods.AttachmentFiles.GetActualPathFromFile(this, data.Data);
                    if (filepath != null)
                    {
                        var check = WoWonderTools.CheckMimeTypesWithServer(filepath);
                        if (!check)
                        {
                            //this file not supported on the server , please select another file 
                            Toast.MakeText(this, GetString(Resource.String.Lbl_ErrorFileNotSupported), ToastLength.Short).Show();
                            return;
                        }

                        var type = Methods.AttachmentFiles.Check_FileExtension(filepath);
                        if (type == "Image")
                        {
                            MessageData m1 = new MessageData
                            {
                                Id = time2,
                                FromId = UserDetails.UserId,
                                ToId = Userid,
                                Media = filepath,
                                Position = "right",
                                Seen = "0",
                                Time = time2,
                                ModelType = MessageModelType.RightImage,
                                TimeText = timeNow
                            };
                            MAdapter.DifferList.Add(new AdapterModelsClassUser()
                            {
                                TypeView = MessageModelType.RightImage,
                                Id = Long.ParseLong(m1.Id),
                                MesData = m1
                            });
                            var indexMes = MAdapter.DifferList.IndexOf(MAdapter.DifferList.Last());
                            MAdapter.NotifyItemInserted(indexMes);

                            //Scroll Down >> 
                            MRecycler.ScrollToPosition(MAdapter.ItemCount - 1);

                            //Send image function
                            if (Methods.CheckConnectivity())
                            {
                                Task.Factory.StartNew(() =>
                                {
                                    MessageController.SendMessageTask(this, Userid, time2, EmojIconEditTextView.Text, "", filepath).ConfigureAwait(false);
                                });
                            }
                            else
                            {
                                Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                            }
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_Please_check_your_details), ToastLength.Long).Show();
                        }
                    }
                }
                else if (requestCode == CropImage.CropImageActivityRequestCode) // right_image 
                {
                    var result = CropImage.GetActivityResult(data);
                    if (resultCode == Result.Ok)
                    {
                        if (result.IsSuccessful)
                        {
                            var resultUri = result.Uri;

                            if (!string.IsNullOrEmpty(resultUri.Path))
                            {
                                MessageData m1 = new MessageData
                                {
                                    Id = time2,
                                    FromId = UserDetails.UserId,
                                    ToId = Userid,
                                    Media = resultUri.Path,
                                    Position = "right",
                                    Seen = "0",
                                    Time = time2,
                                    ModelType = MessageModelType.RightImage,
                                    TimeText = timeNow
                                };
                                MAdapter.DifferList.Add(new AdapterModelsClassUser()
                                {
                                    TypeView = MessageModelType.RightImage,
                                    Id = Long.ParseLong(m1.Id),
                                    MesData = m1
                                });

                                var indexMes = MAdapter.DifferList.IndexOf(MAdapter.DifferList.Last());
                                MAdapter.NotifyItemInserted(indexMes);

                                //Scroll Down >> 
                                MRecycler.ScrollToPosition(MAdapter.ItemCount - 1);

                                //Send image function
                                if (Methods.CheckConnectivity())
                                {
                                    Task.Factory.StartNew(() =>
                                    {
                                        MessageController.SendMessageTask(this, Userid, time2, EmojIconEditTextView.Text, "", resultUri.Path).ConfigureAwait(false);
                                    });
                                }
                                else
                                {
                                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                                }
                            }
                            else
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Something_went_wrong), ToastLength.Long).Show();
                            }
                        }
                    }
                }
                else if (requestCode == 503 && resultCode == Result.Ok) // Add right_image using camera   
                {
                    if (string.IsNullOrEmpty(IntentController.CurrentPhotoPath))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Failed_to_load), ToastLength.Short).Show();
                    }
                    else
                    {
                        //var thumbnail = MediaStore.Images.Media.GetBitmap(ContentResolver, IntentController.ImageCameraUri); 
                        //Bitmap bitmap = BitmapFactory.DecodeFile(IntentController.currentPhotoPath);

                        if (Methods.MultiMedia.CheckFileIfExits(IntentController.CurrentPhotoPath) != "File Dont Exists")
                        {
                            MessageData m1 = new MessageData
                            {
                                Id = time2,
                                FromId = UserDetails.UserId,
                                ToId = Userid,
                                Media = IntentController.CurrentPhotoPath,
                                Position = "right",
                                Seen = "0",
                                Time = time2,
                                ModelType = MessageModelType.RightImage,
                                TimeText = timeNow
                            };
                            MAdapter.DifferList.Add(new AdapterModelsClassUser()
                            {
                                TypeView = MessageModelType.RightImage,
                                Id = Long.ParseLong(m1.Id),
                                MesData = m1
                            });

                            var indexMes = MAdapter.DifferList.IndexOf(MAdapter.DifferList.Last());
                            MAdapter.NotifyItemInserted(indexMes);

                            //Scroll Down >> 
                            MRecycler.ScrollToPosition(MAdapter.ItemCount - 1);

                            //Send image function
                            if (Methods.CheckConnectivity())
                            {
                                Task.Factory.StartNew(() =>
                                {
                                    MessageController.SendMessageTask(this, Userid, time2, EmojIconEditTextView.Text, "", IntentController.CurrentPhotoPath).ConfigureAwait(false);
                                });
                            }
                            else
                            {
                                Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                            }
                        }
                        else
                        {
                            //Toast.MakeText(this, GetText(Resource.String.Lbl_Failed_to_load), ToastLength.Short).Show();
                        }
                    }
                }
                else if (requestCode == 501 && resultCode == Result.Ok) // right_video 
                {
                    var filepath = Methods.AttachmentFiles.GetActualPathFromFile(this, data.Data);
                    if (filepath != null)
                    {
                        var check = WoWonderTools.CheckMimeTypesWithServer(filepath);
                        if (!check)
                        {
                            //this file not supported on the server , please select another file 
                            Toast.MakeText(this, GetString(Resource.String.Lbl_ErrorFileNotSupported), ToastLength.Short).Show();
                            return;
                        }

                        var type = Methods.AttachmentFiles.Check_FileExtension(filepath);
                        if (type == "Video")
                        {
                            var fileName = filepath.Split('/').Last();
                            var fileNameWithoutExtension = fileName.Split('.').First();
                            var pathWithoutFilename = Methods.Path.FolderDcimVideo + Userid;
                            var fullPathFile = new File(Methods.Path.FolderDcimVideo + Userid, fileNameWithoutExtension + ".png");

                            var videoPlaceHolderImage = Methods.MultiMedia.GetMediaFrom_Gallery(pathWithoutFilename, fileNameWithoutExtension + ".png");
                            if (videoPlaceHolderImage == "File Dont Exists")
                            {
                                var bitmapImage = Methods.MultiMedia.Retrieve_VideoFrame_AsBitmap(this, data.Data.ToString());
                                Methods.MultiMedia.Export_Bitmap_As_Image(bitmapImage, fileNameWithoutExtension, pathWithoutFilename);
                            }
                            //wael
                            //var newCopyedFilepath = Methods.MultiMedia.CopyMediaFileTo(filepath, Methods.Path.FolderDcimVideo + Userid, false, true);
                            //if (newCopyedFilepath != "Path File Dont exits")
                            //{
                            MessageData m1 = new MessageData
                            {
                                Id = time2,
                                FromId = UserDetails.UserId,
                                ToId = Userid,
                                Media = filepath,
                                Seen = "0",
                                Time = time2,
                                Position = "right",
                                ModelType = MessageModelType.RightVideo,
                                TimeText = timeNow
                            };
                            MAdapter.DifferList.Add(new AdapterModelsClassUser()
                            {
                                TypeView = MessageModelType.RightVideo,
                                Id = Long.ParseLong(m1.Id),
                                MesData = m1
                            });

                            var indexMes = MAdapter.DifferList.IndexOf(MAdapter.DifferList.Last());
                            MAdapter.NotifyItemInserted(indexMes);

                            //Scroll Down >> 
                            MRecycler.ScrollToPosition(MAdapter.ItemCount - 1);

                            //Send Video function
                            if (Methods.CheckConnectivity())
                            {
                                Task.Factory.StartNew(() =>
                                {
                                    MessageController.SendMessageTask(this, Userid, time2, EmojIconEditTextView.Text, "", filepath).ConfigureAwait(false);
                                });
                            }
                            else
                            {
                                Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                            }
                            //}
                        }
                    }
                }
                else if (requestCode == 504 && resultCode == Result.Ok) // right_file
                {
                    string filepath = Methods.AttachmentFiles.GetActualPathFromFile(this, data.Data);
                    if (filepath != null)
                    {
                        var check = WoWonderTools.CheckMimeTypesWithServer(filepath);
                        if (!check)
                        {
                            //this file not supported on the server , please select another file 
                            Toast.MakeText(this, GetString(Resource.String.Lbl_ErrorFileNotSupported), ToastLength.Short).Show();
                            return;
                        }

                        string totalSize = Methods.FunString.Format_byte_size(filepath);
                        MessageData m1 = new MessageData
                        {
                            Id = time2,
                            FromId = UserDetails.UserId,
                            ToId = Userid,
                            Media = filepath,
                            FileSize = totalSize,
                            TimeText = timeNow,
                            Seen = "0",
                            Time = time2,
                            Position = "right",
                            ModelType = MessageModelType.RightFile
                        };
                        MAdapter.DifferList.Add(new AdapterModelsClassUser()
                        {
                            TypeView = MessageModelType.RightFile,
                            Id = Long.ParseLong(m1.Id),
                            MesData = m1
                        });

                        var indexMes = MAdapter.DifferList.IndexOf(MAdapter.DifferList.Last());
                        MAdapter.NotifyItemInserted(indexMes);

                        //Scroll Down >> 
                        MRecycler.ScrollToPosition(MAdapter.ItemCount - 1);

                        //Send Video function
                        if (Methods.CheckConnectivity())
                        {
                            Task.Factory.StartNew(() =>
                            {
                                MessageController.SendMessageTask(this, Userid, time2, EmojIconEditTextView.Text, "", filepath).ConfigureAwait(false);
                            });
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                        }
                    }
                    else
                    {
                        string src = GetPath(data.Data);
                        if (src != null)
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_FileNotDeviceMemory), ToastLength.Long).Show();
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_Please_check_your_details), ToastLength.Long).Show();
                        }
                    }
                }
                else if (requestCode == 505 && resultCode == Result.Ok) // right_audio
                {
                    var filepath = Methods.AttachmentFiles.GetActualPathFromFile(this, data.Data);
                    if (filepath != null)
                    {
                        var check = WoWonderTools.CheckMimeTypesWithServer(filepath);
                        if (!check)
                        {
                            //this file not supported on the server , please select another file 
                            Toast.MakeText(this, GetString(Resource.String.Lbl_ErrorFileNotSupported), ToastLength.Short).Show();
                            return;
                        }

                        var type = Methods.AttachmentFiles.Check_FileExtension(filepath);
                        if (type == "Audio")
                        {
                            //wael 
                            //var newCopyedFilepath = Methods.MultiMedia.CopyMediaFileTo(filepath, Methods.Path.FolderDcimSound + "/" + Userid, false, true);
                            //if (newCopyedFilepath != "Path File Dont exits")
                            //{
                            string totalSize = Methods.FunString.Format_byte_size(filepath);
                            MessageData m1 = new MessageData
                            {
                                Id = time2,
                                FromId = UserDetails.UserId,
                                ToId = Userid,
                                Media = filepath,
                                FileSize = totalSize,
                                Seen = "0",
                                Time = time2,
                                Position = "right",
                                TimeText = GetText(Resource.String.Lbl_Uploading),
                                MediaDuration = Methods.AudioRecorderAndPlayer.GetTimeString(Methods.AudioRecorderAndPlayer.Get_MediaFileDuration(filepath)),
                                ModelType = MessageModelType.RightAudio
                            };

                            MAdapter.DifferList.Add(new AdapterModelsClassUser()
                            {
                                TypeView = MessageModelType.RightAudio,
                                Id = Long.ParseLong(m1.Id),
                                MesData = m1
                            });

                            var indexMes = MAdapter.DifferList.IndexOf(MAdapter.DifferList.Last());
                            MAdapter.NotifyItemInserted(indexMes);

                            //Scroll Down >> 
                            MRecycler.ScrollToPosition(MAdapter.ItemCount - 1);

                            //Send Video function
                            if (Methods.CheckConnectivity())
                            {
                                Task.Factory.StartNew(() =>
                                {
                                    MessageController.SendMessageTask(this, Userid, time2, "", "", filepath).ConfigureAwait(false);
                                });
                            }
                            else
                            {
                                Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                            }
                            //}
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Failed_to_load), ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Failed_to_load), ToastLength.Short).Show();
                    }
                }
                else if (requestCode == 300 && resultCode == Result.Ok) // right_gif
                {
                    // G_fixed_height_small_url, // UrlGif - view  >>  mediaFileName
                    // G_fixed_height_small_mp4, //MediaGif - sent >>  media

                    var gifLink = data.GetStringExtra("MediaGif") ?? "Data not available";
                    if (gifLink != "Data not available" && !string.IsNullOrEmpty(gifLink))
                    {
                        var gifUrl = data.GetStringExtra("UrlGif") ?? "Data not available";
                        GifFile = gifLink;

                        MessageData m1 = new MessageData
                        {
                            Id = time2,
                            FromId = UserDetails.UserId,
                            ToId = Userid,
                            Media = GifFile,
                            MediaFileName = gifUrl,
                            Seen = "0",
                            Time = time2,
                            Position = "right",
                            ModelType = MessageModelType.RightGif,
                            TimeText = timeNow,
                            Stickers = gifUrl,
                        };

                        MAdapter.DifferList.Add(new AdapterModelsClassUser()
                        {
                            TypeView = MessageModelType.RightGif,
                            Id = Long.ParseLong(m1.Id),
                            MesData = m1
                        });

                        var indexMes = MAdapter.DifferList.IndexOf(MAdapter.DifferList.Last());
                        MAdapter.NotifyItemInserted(indexMes);

                        //Scroll Down >> 
                        MRecycler.ScrollToPosition(MAdapter.ItemCount - 1);

                        //Send image function
                        if (Methods.CheckConnectivity())
                        {
                            Task.Factory.StartNew(() =>
                            {
                                MessageController.SendMessageTask(this, Userid, time2, EmojIconEditTextView.Text, "", "", "", "", gifUrl).ConfigureAwait(false);
                            });
                        }
                        else
                        {
                            Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Please_check_your_details) + " ", ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private string GetPath(Uri uri)
        {
            try
            {
                string path;
                string[] projection = { MediaStore.Files.FileColumns.DateAdded, "text/plain", "text/html", "application/pdf", "audio/mp3", "video/mp4", "text/plain", "application/msword", "image/jpeg", "image/png", "text/html" };
                var cursor = ContentResolver.Query(uri, projection, null, null, null);

                if (cursor == null)
                {
                    path = uri.Path;
                }
                else
                {
                    cursor.MoveToFirst();
                    int columnIndex = cursor.GetColumnIndexOrThrow(projection[0]);
                    path = cursor.GetString(columnIndex);
                    cursor.Close();
                }

                return ((path == null || string.IsNullOrEmpty(path)) ? (uri.Path) : path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }


        //Permissions
        public override async void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 123)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        Methods.Path.Chack_MyFolder(Userid);
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 108)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        switch (PermissionsType)
                        {
                            //requestCode >> 500 => Image Gallery
                            case "Image" when AppSettings.ImageCropping:
                                OpenDialogGallery("Image");
                                break;
                            case "Image": //requestCode >> 500 => Image Gallery
                                new IntentController(this).OpenIntentImageGallery(GetText(Resource.String.Lbl_SelectPictures), false);
                                break;
                            case "Video":
                                //requestCode >> 501 => video Gallery
                                new IntentController(this).OpenIntentVideoGallery();
                                break;
                            case "Camera":
                                //requestCode >> 503 => Camera
                                new IntentController(this).OpenIntentCamera();
                                break;
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 100)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        switch (PermissionsType)
                        {
                            case "File":
                                //requestCode >> 504 => File
                                new IntentController(this).OpenIntentFile(GetText(Resource.String.Lbl_SelectFile));
                                break;
                            case "Music":
                                //requestCode >> 505 => Music
                                new IntentController(this).OpenIntentAudio();
                                break;
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 101)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        //request code of result is 506
                        new IntentController(this).OpenIntentGetContactNumberPhone();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 102)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        if (ChatSendButton.Tag.ToString() == "Free")
                        {
                            //Set Record Style
                            IsRecording = true;

                            if (SettingsPrefFragment.SSoundControl)
                                Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("RecourdVoiceButton.mp3");

                            if (EmojIconEditTextView.Text != GetString(Resource.String.Lbl_Recording))
                            {
                                EmojIconEditTextView.Text = GetString(Resource.String.Lbl_Recording);
                                EmojIconEditTextView.SetTextColor(Color.ParseColor("#FA3C4C"));
                            }

                            ChatSendButton.SetColor(Color.ParseColor("#FA3C4C"));
                            ChatSendButton.SetImageResource(Resource.Drawable.ic_stop_white_24dp);

                            RecorderService = new Methods.AudioRecorderAndPlayer(Userid);
                            //Start Audio record
                            await Task.Delay(600);
                            RecorderService.StartRecourding();
                        }
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 1106) //Audio Call
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        StartCall();
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
                else if (requestCode == 1107) //Video call
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        StartVideoCall();
                    }
                    else
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
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
                if (itemString.ToString() == GetText(Resource.String.Btn_Image)) // image 
                {
                    // Check if we're running on Android 5.0 or higher
                    if ((int)Build.VERSION.SdkInt < 23)
                    {
                        if (AppSettings.ImageCropping)
                            OpenDialogGallery("Image"); //requestCode >> 500 => Image Gallery
                        else
                            new IntentController(this).OpenIntentImageGallery(GetText(Resource.String.Lbl_SelectPictures), false); //requestCode >> 500 => Image Gallery
                    }
                    else
                    {
                        if (CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted
                                                                                                  && CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                        {
                            if (AppSettings.ImageCropping)
                                OpenDialogGallery("Image"); //requestCode >> 500 => Image Gallery
                            else
                                new IntentController(this).OpenIntentImageGallery(GetText(Resource.String.Lbl_SelectPictures), false); //requestCode >> 500 => Image Gallery
                        }
                        else
                        {
                            new PermissionsController(this).RequestPermission(108);
                        }
                    }
                }
                else if (itemString.ToString() == GetText(Resource.String.Camera)) // Camera 
                {
                    PermissionsType = "Camera";

                    // Check if we're running on Android 5.0 or higher
                    if ((int)Build.VERSION.SdkInt < 23)
                    {
                        //requestCode >> 503 => Camera
                        new IntentController(this).OpenIntentCamera();
                    }
                    else
                    {
                        if (CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted
                                                                                                  && CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                        {
                            //requestCode >> 503 => Camera
                            new IntentController(this).OpenIntentCamera();
                        }
                        else
                        {
                            new PermissionsController(this).RequestPermission(108);
                        }
                    }
                }
                else if (itemString.ToString() == GetText(Resource.String.Btn_Video)) // video  
                {
                    PermissionsType = "Video";

                    // Check if we're running on Android 5.0 or higher
                    if ((int)Build.VERSION.SdkInt < 23)
                    {
                        //requestCode >> 501 => video Gallery
                        new IntentController(this).OpenIntentVideoGallery();
                    }
                    else
                    {
                        if (CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted
                                                                                                  && CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                        {
                            //requestCode >> 501 => video Gallery
                            new IntentController(this).OpenIntentVideoGallery();
                        }
                        else
                        {
                            new PermissionsController(this).RequestPermission(108);
                        }
                    }
                }
                else if (itemString.ToString() == GetText(Resource.String.Lbl_File)) // File  
                {
                    PermissionsType = "File";

                    // Check if we're running on Android 5.0 or higher
                    if ((int)Build.VERSION.SdkInt < 23)
                    {
                        //requestCode >> 504 => File
                        new IntentController(this).OpenIntentFile(GetText(Resource.String.Lbl_SelectFile));
                    }
                    else
                    {
                        if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                            CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                        {
                            //requestCode >> 504 => File
                            new IntentController(this).OpenIntentFile(GetText(Resource.String.Lbl_SelectFile));
                        }
                        else
                        {
                            new PermissionsController(this).RequestPermission(100);
                        }
                    }
                }
                else if (itemString.ToString() == GetText(Resource.String.Lbl_Music)) // Music  
                {
                    PermissionsType = "Music";

                    // Check if we're running on Android 5.0 or higher
                    if ((int)Build.VERSION.SdkInt < 23)
                        new IntentController(this).OpenIntentAudio(); //505
                    else
                    {
                        if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted && CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                            new IntentController(this).OpenIntentAudio(); //505
                        else
                            new PermissionsController(this).RequestPermission(100);
                    }
                }
                else if (itemString.ToString() == GetText(Resource.String.Lbl_Gif)) // Gif  
                {
                    StartActivityForResult(new Intent(this, typeof(GifActivity)), 300);
                }
                else if (itemString.ToString() == GetText(Resource.String.Lbl_Contact)) // Contact  
                {
                    ChatContactButtonOnClick();
                }
                else if (itemString.ToString() == GetText(Resource.String.Lbl_View_Profile)) // Menu View profile
                {
                    OnMenuViewProfile_Click();
                }
                else if (itemString.ToString() == GetText(Resource.String.Lbl_Block)) // Menu Block
                {
                    OnMenuBlock_Click();
                }
                else if (itemString.ToString() == GetText(Resource.String.Lbl_Clear_chat)) // Menu Clear Chat
                {
                    OnMenuClearChat_Click();
                }
                else if (itemString.ToString() == GetText(Resource.String.Lbl_Copy))
                {
                    CopyItems();
                }
                else if (itemString.ToString() == GetText(Resource.String.Lbl_MessageInfo))
                {
                    var intent = new Intent(this, typeof(MessageInfoActivity));
                    intent.PutExtra("UserId", Userid);
                    intent.PutExtra("SelectedItem", JsonConvert.SerializeObject(SelectedItemPositions.MesData));
                    StartActivity(intent);
                }
                else if (itemString.ToString() == GetText(Resource.String.Lbl_Forward))
                {
                    ForwardItems();
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

        #region loadData

        private void FirstLoadData_Item()
        {
            try
            {
                if (TypeChat == "LastMessenger")
                {
                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                    {
                        DataUser = JsonConvert.DeserializeObject<ChatObject>(Intent.GetStringExtra("UserItem"));
                        if (DataUser != null)
                        {
                            if (DataUser.LastMessage.LastMessageClass.ChatColor == null)
                                DataUser.LastMessage.LastMessageClass.ChatColor = AppSettings.MainColor;

                            if (!ColorChanged)
                                MainChatColor = !string.IsNullOrEmpty(DataUser.LastMessage.LastMessageClass?.ChatColor) ? DataUser.LastMessage.LastMessageClass.ChatColor.Contains("rgb") ? Methods.FunString.ConvertColorRgBtoHex(DataUser.LastMessage.LastMessageClass?.ChatColor) : DataUser.LastMessage.LastMessageClass?.ChatColor : AppSettings.MainColor ?? AppSettings.MainColor;

                        }
                        else
                        {
                            if (!ColorChanged)
                                MainChatColor = AppSettings.MainColor;
                        }
                    }
                    else
                    {
                        DataUserChat = JsonConvert.DeserializeObject<GetUsersListObject.User>(Intent.GetStringExtra("UserItem"));
                        if (DataUserChat != null)
                        {
                            if (DataUserChat.ChatColor == null)
                                DataUserChat.ChatColor = AppSettings.MainColor;

                            if (!ColorChanged)
                                MainChatColor = !string.IsNullOrEmpty(DataUserChat.ChatColor) ? DataUserChat.ChatColor.Contains("rgb") ? Methods.FunString.ConvertColorRgBtoHex(DataUserChat.ChatColor) : DataUserChat.ChatColor : AppSettings.MainColor ?? AppSettings.MainColor;

                        }
                        else
                        {
                            if (!ColorChanged)
                                MainChatColor = AppSettings.MainColor;
                        }
                    }
                }
                else
                {
                    UserData = JsonConvert.DeserializeObject<UserDataObject>(Intent.GetStringExtra("UserItem"));
                    if (UserData != null)
                    {
                        if (UserData.ChatColor == null)
                            UserData.ChatColor = AppSettings.MainColor;

                        if (!ColorChanged)
                            MainChatColor = UserData.ChatColor.Contains("rgb") ? Methods.FunString.ConvertColorRgBtoHex(UserData.ChatColor) : UserData.ChatColor ?? AppSettings.MainColor;

                    }
                    else
                    {
                        if (!ColorChanged)
                            MainChatColor = AppSettings.MainColor;
                    }
                }

                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt >= 23)
                {
                    if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.AccessMediaLocation) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted
                        && CheckSelfPermission(Manifest.Permission.RecordAudio) == Permission.Granted)
                    {
                        Methods.Path.Chack_MyFolder(Userid);
                    }
                    else
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.Camera,
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.WriteExternalStorage,
                            Manifest.Permission.AccessMediaLocation,
                            Manifest.Permission.RecordAudio,
                            Manifest.Permission.ModifyAudioSettings,
                        }, 123);
                    }
                }
                else
                {
                    Methods.Path.Chack_MyFolder(Userid);
                }

                StartApiService();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadData_ItemUser()
        {
            try
            {
                if (TypeChat == "LastMessenger")
                {
                    if (DataUser != null && AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                    {
                        //Allen To be Edited
                        Glide.With(this).Load(DataUser.Avatar).Apply(new RequestOptions().SetDiskCacheStrategy(DiskCacheStrategy.All).CenterCrop()).Into(UserChatProfile);
                        ActionBarTitle.Text = Methods.FunString.DecodeString(DataUser.Name);
                        SayHiToTextView.Text = Methods.FunString.DecodeString(DataUser.Name);

                        //Online Or offline
                        if (DataUser.Lastseen == "on")
                        {
                            ActionBarSubTitle.Text = GetString(Resource.String.Lbl_Online);
                            LastSeen = GetString(Resource.String.Lbl_Online);
                        }
                        else
                        {
                            ActionBarSubTitle.Text = GetString(Resource.String.Lbl_Last_seen) + " " + Methods.Time.TimeAgo(int.Parse(DataUser.LastseenUnixTime), false);
                            LastSeen = GetString(Resource.String.Lbl_Last_seen) + " " + Methods.Time.TimeAgo(int.Parse(DataUser.LastseenUnixTime), false);
                        }
                    }
                    else if (DataUserChat != null && AppSettings.LastChatSystem == SystemApiGetLastChat.Old)
                    {
                        //Allen To be Edited
                        Glide.With(this).Load(DataUserChat.Avatar).Apply(new RequestOptions().SetDiskCacheStrategy(DiskCacheStrategy.All).CenterCrop()).Into(UserChatProfile);
                        ActionBarTitle.Text = Methods.FunString.DecodeString(DataUserChat.Name);
                        SayHiToTextView.Text = Methods.FunString.DecodeString(DataUserChat.Name);

                        //Online Or offline
                        if (DataUserChat.Lastseen == "on")
                        {
                            ActionBarSubTitle.Text = GetString(Resource.String.Lbl_Online);
                            LastSeen = GetString(Resource.String.Lbl_Online);
                        }
                        else
                        {
                            ActionBarSubTitle.Text = GetString(Resource.String.Lbl_Last_seen) + " " + Methods.Time.TimeAgo(int.Parse(DataUserChat.LastseenUnixTime), false);
                            LastSeen = GetString(Resource.String.Lbl_Last_seen) + " " + Methods.Time.TimeAgo(int.Parse(DataUserChat.LastseenUnixTime), false);
                        }
                    }
                }
                else
                {
                    if (UserData != null)
                    {
                        Glide.With(this).Load(UserData.Avatar)
                            .Apply(new RequestOptions().SetDiskCacheStrategy(DiskCacheStrategy.All).CenterCrop()).Into(UserChatProfile);
                        ActionBarTitle.Text = Methods.FunString.DecodeString(UserData.Name);
                        SayHiToTextView.Text = Methods.FunString.DecodeString(UserData.Name);

                        //Online Or offline
                        if (UserData.Lastseen == "on")
                        {
                            ActionBarSubTitle.Text = GetString(Resource.String.Lbl_Online);
                            LastSeen = GetString(Resource.String.Lbl_Online);
                        }
                        else
                        {
                            ActionBarSubTitle.Text = GetString(Resource.String.Lbl_Last_seen) + " " + Methods.Time.TimeAgo(int.Parse(UserData.LastseenUnixTime), false);
                            LastSeen = GetString(Resource.String.Lbl_Last_seen) + " " + Methods.Time.TimeAgo(int.Parse(UserData.LastseenUnixTime), false);
                        }
                    }
                }

                GetMessages();
            }
            catch (Exception e)
            {
                GetMessages();
                Console.WriteLine(e);
            }
        }

        private async void GetMessages()
        {
            try
            {
                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                var localList = dbDatabase.GetMessages_List(UserDetails.UserId, Userid, "0");
                if (localList == "1") //Database.. Get Messages Local
                {
                    MAdapter.NotifyDataSetChanged();

                    //Scroll Down >> 
                    MRecycler.ScrollToPosition(MAdapter.ItemCount - 1);

                    //SwipeRefreshLayout.Refreshing = false;
                    //SwipeRefreshLayout.Enabled = false; 
                }
                else //Or server.. Get Messages Api
                {
                    //SwipeRefreshLayout.Refreshing = true;
                    //SwipeRefreshLayout.Enabled = true;

                    await GetMessages_Api();
                }

                if (MAdapter.DifferList.Count > 0)
                {
                    SayHiLayout.Visibility = ViewStates.Gone;
                    SayHiSuggestionsRecycler.Visibility = ViewStates.Gone;
                }
                else if (MAdapter.DifferList.Count == 0 && ShowEmpty != "no")
                {
                    SayHiLayout.Visibility = ViewStates.Visible;
                    SayHiSuggestionsRecycler.Visibility = ViewStates.Visible;
                }

                TaskWork = "Working";

                //Run timer
                Timer = new Timer { Interval = AppSettings.MessageRequestSpeed };
                Timer.Elapsed += TimerOnElapsed;
                Timer.Enabled = true;
                Timer.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task GetMessages_Api()
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    RunLoadMore = true;
                    (int apiStatus, var respond) = await ApiRequest.FetchUserChatMessages(Userid);
                    if (apiStatus == 200)
                    {
                        if (respond is UserChatMessagesObject result)
                        {
                            int countList = MAdapter.DifferList.Count;
                            var respondList = result.Messages.Count;
                            if (respondList > 0)
                            {
                                result.Messages.Reverse();

                                foreach (var item in from item in result.Messages let check = MAdapter.DifferList.FirstOrDefault(a => a.MesData.Id == item.Id) where check == null select item)
                                {
                                    var type = MAdapter.GetTypeModel(item);
                                    if (type == MessageModelType.None)
                                        continue;

                                    MAdapter.DifferList.Add(new AdapterModelsClassUser()
                                    {
                                        TypeView = type,
                                        Id = Long.ParseLong(item.Id),
                                        MesData = WoWonderTools.MessageFilter(Userid, item, type)
                                    });
                                }

                                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                // Insert data user in database
                                dbDatabase.Insert_Or_Replace_MessagesTable(MAdapter.DifferList);
                                dbDatabase.Dispose();

                                RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        if (countList > 0)
                                            MAdapter.NotifyItemRangeInserted(countList - 1, MAdapter.DifferList.Count - countList);
                                        else
                                            MAdapter.NotifyDataSetChanged();

                                        //Scroll Down >> 
                                        MRecycler.ScrollToPosition(MAdapter.ItemCount - 1);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                });
                            }
                        }
                    }
                    else Methods.DisplayReportResult(this, respond);

                    //SwipeRefreshLayout.Refreshing = false;
                    //SwipeRefreshLayout.Enabled = false;

                    RunLoadMore = false;
                }
                else Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void MessageUpdater()
        {
            try
            {
                if (TaskWork == "Working")
                {
                    TaskWork = "Stop";

                    if (Methods.CheckConnectivity())
                    {
                        //var data = MAdapter.DifferList.LastOrDefault();
                        //var lastMessageId = data?.MesData?.Id ?? "0";
                        (int apiStatus, var respond) = await ApiRequest.FetchUserChatMessages(Userid, "0", "0");
                        if (apiStatus == 200)
                        {
                            if (respond is UserChatMessagesObject result)
                            {
                                try
                                {
                                    var typing = result.Typing.ToString();
                                    ActionBarSubTitle.Text = typing == "1" ? GetString(Resource.String.Lbl_Typping) : LastSeen ?? LastSeen;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }

                                int countList = MAdapter.DifferList.Count;
                                var respondList = result.Messages.Count;
                                if (respondList > 0)
                                {
                                    foreach (var item in result.Messages)
                                    {
                                        var type = MAdapter.GetTypeModel(item);
                                        if (type == MessageModelType.None)
                                            continue;

                                        var message = WoWonderTools.MessageFilter(Userid, item, type);

                                        var check = MAdapter.DifferList.FirstOrDefault(a => a.MesData.Id == item.Id);
                                        if (check == null)
                                        {
                                            MAdapter.DifferList.Add(new AdapterModelsClassUser()
                                            {
                                                TypeView = type,
                                                Id = Long.ParseLong(item.Id),
                                                MesData = message
                                            });

                                            if (countList > 0)
                                                MAdapter.NotifyItemRangeInserted(countList - 1, MAdapter.DifferList.Count - countList);
                                            else
                                                MAdapter.NotifyDataSetChanged();

                                            //Scroll Down >> 
                                            MRecycler.ScrollToPosition(MAdapter.ItemCount - 1);

                                            if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                                            {
                                                var dataUser = GlobalContext?.LastChatTab?.MAdapter?.ChatList?.FirstOrDefault(a => a.UserId == item.FromId);
                                                if (dataUser != null)
                                                {
                                                    if (item.UserData != null)
                                                    {
                                                        dataUser.UserId = item.UserData.UserId;
                                                        dataUser.Avatar = item.UserData.Avatar;
                                                    }

                                                    dataUser.LastMessage = new LastMessageUnion()
                                                    {
                                                        LastMessageClass = item,
                                                    };
                                                    dataUser.LastMessage.LastMessageClass.ChatColor = MainChatColor;

                                                    var index = GlobalContext?.LastChatTab?.MAdapter?.ChatList?.IndexOf(dataUser);
                                                    if (index > -1 && index != 0)
                                                    {
                                                        GlobalContext?.LastChatTab.MAdapter.ChatList.Move(Convert.ToInt32(index), 0);
                                                        GlobalContext?.LastChatTab.MAdapter.NotifyItemMoved(Convert.ToInt32(index), 0);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var dataUser = GlobalContext?.LastMessagesTab?.MAdapter?.MLastMessagesUser?.FirstOrDefault(a => a.UserId == item.FromId);
                                                if (dataUser != null)
                                                {
                                                    if (item.UserData != null)
                                                    {
                                                        dataUser.UserId = item.UserData.UserId;
                                                        dataUser.Avatar = item.UserData.Avatar;
                                                    }

                                                    dataUser.ChatColor = MainChatColor;

                                                    //last_message
                                                    dataUser.LastMessage = new GetUsersListObject.LastMessage
                                                    {
                                                        Id = item.Id,
                                                        FromId = item.FromId,
                                                        GroupId = item.GroupId,
                                                        ToId = item.ToId,
                                                        Text = item.Text,
                                                        Media = item.Media,
                                                        MediaFileName = item.MediaFileName,
                                                        MediaFileNames = item.MediaFileNames,
                                                        Time = item.Time,
                                                        Seen = "1",
                                                        DeletedOne = item.DeletedOne,
                                                        DeletedTwo = item.DeletedTwo,
                                                        SentPush = item.SentPush,
                                                        NotificationId = item.NotificationId,
                                                        TypeTwo = item.TypeTwo,
                                                        Stickers = item.Stickers
                                                        // DateTime = dateTime,
                                                    };

                                                    var index = GlobalContext?.LastMessagesTab?.MAdapter?.MLastMessagesUser?.IndexOf(dataUser);
                                                    if (index > -1 && index != 0)
                                                    {
                                                        GlobalContext?.LastMessagesTab.MAdapter.MLastMessagesUser.Move(Convert.ToInt32(index), 0);
                                                        GlobalContext?.LastMessagesTab.MAdapter.NotifyItemMoved(Convert.ToInt32(index), 0);
                                                    }
                                                }
                                            }

                                            if (SettingsPrefFragment.SSoundControl)
                                                Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Popup_GetMesseges.mp3");

                                        }
                                        else if (check.MesData.Seen != item.Seen)
                                        {
                                            check.Id = Convert.ToInt32(message.Id);
                                            check.MesData = message;
                                            check.TypeView = type;

                                            MAdapter.NotifyItemChanged(MAdapter.DifferList.IndexOf(check));

                                            SqLiteDatabase sqLite = new SqLiteDatabase();
                                            sqLite.Insert_Or_Update_To_one_MessagesTable(check.MesData);
                                            sqLite.Dispose();
                                        }
                                    }

                                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                    // Insert data user in database
                                    dbDatabase.Insert_Or_Replace_MessagesTable(MAdapter.DifferList);
                                    dbDatabase.Dispose();

                                    if (MAdapter.DifferList.Count > 0)
                                    {
                                        SayHiLayout.Visibility = ViewStates.Gone;
                                        SayHiSuggestionsRecycler.Visibility = ViewStates.Gone;
                                    }
                                    else if (MAdapter.DifferList.Count == 0 && ShowEmpty != "no")
                                    {
                                        SayHiLayout.Visibility = ViewStates.Visible;
                                        SayHiSuggestionsRecycler.Visibility = ViewStates.Visible;
                                    }

                                }
                            }
                        }
                        else Methods.DisplayReportResult(this, respond);
                    }
                    else Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();

                    TaskWork = "Working";
                }
            }
            catch (Exception e)
            {
                TaskWork = "Working";
                Console.WriteLine(e);
            }
        }

        private string LoadMore_Messages_Database()
        {
            try
            {
                if (RunLoadMore)
                    return "1";

                RunLoadMore = true;

                SqLiteDatabase dbDatabase = new SqLiteDatabase();

                var data = MAdapter.DifferList.FirstOrDefault();
                var firstMessageId = data?.MesData?.Id;

                var localList = dbDatabase.GetMessageList(UserDetails.UserId, Userid, firstMessageId);
                if (localList?.Count > 0) //Database.. Get Messages Local
                {
                    foreach (var item in from message in localList
                                         let check = MAdapter.DifferList.FirstOrDefault(a => a.MesData.Id == message.Id)
                                         where check == null
                                         select new MessageData
                                         {
                                             Id = message.Id,
                                             FromId = message.FromId,
                                             GroupId = message.GroupId,
                                             ToId = message.ToId,
                                             Text = message.Text,
                                             Media = message.Media,
                                             MediaFileName = message.MediaFileName,
                                             MediaFileNames = message.MediaFileNames,
                                             Time = message.Time,
                                             Seen = message.Seen,
                                             DeletedOne = message.DeletedOne,
                                             DeletedTwo = message.DeletedTwo,
                                             SentPush = message.SentPush,
                                             NotificationId = message.NotificationId,
                                             TypeTwo = message.TypeTwo,
                                             Stickers = message.Stickers,
                                             TimeText = message.TimeText,
                                             Position = message.Position,
                                             ModelType = message.ModelType,
                                             FileSize = message.FileSize,
                                             MessageUser = JsonConvert.DeserializeObject<UserDataObject>(message.MessageUser),
                                             ContactName = message.ContactName,
                                             ContactNumber = message.ContactNumber,
                                             ChatColor = MainChatColor
                                         })
                    {
                        var type = MAdapter.GetTypeModel(item);
                        if (type == MessageModelType.None)
                            continue;

                        var check = MAdapter.DifferList.FirstOrDefault(a => a.MesData.Id == item.Id);
                        if (check != null) continue;
                        var mes = new AdapterModelsClassUser()
                        {
                            TypeView = type,
                            Id = Long.ParseLong(item.Id),
                            MesData = WoWonderTools.MessageFilter(Userid, item, type)
                        };

                        MAdapter.DifferList.Insert(0, mes);

                        RunOnUiThread(() =>
                        {
                            MAdapter?.NotifyItemInserted(MAdapter.DifferList.IndexOf(mes));

                            var indexMes = MAdapter.DifferList.IndexOf(data);
                            if (indexMes > -1)
                            {
                                //Scroll Down >> 
                                //MRecycler.SmoothScrollToPosition(indexMes);
                            }
                        });
                    }

                    //if (SwipeRefreshLayout.Refreshing)
                    //{
                    //    SwipeRefreshLayout.Refreshing = false;
                    //    SwipeRefreshLayout.Enabled = false;

                    //}

                    dbDatabase.Dispose();
                    RunLoadMore = false;
                    return "1";
                }

                //if (SwipeRefreshLayout.Refreshing)
                //{
                //    SwipeRefreshLayout.Refreshing = false;
                //    SwipeRefreshLayout.Enabled = false;

                //}

                dbDatabase.Dispose();
                RunLoadMore = false;
                return "0";
            }
            catch (Exception e)
            {
                RunLoadMore = false;
                Console.WriteLine(e);
                return "0";
            }
        }

        private bool RunLoadMore;

        private async Task LoadMoreMessages_API()
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (RunLoadMore)
                        return;

                    RunLoadMore = true;

                    var data = MAdapter.DifferList.FirstOrDefault();
                    var firstMessageId = data?.MesData?.Id;

                    (int apiStatus, var respond) = await ApiRequest.FetchUserChatMessages(Userid, firstMessageId);
                    if (apiStatus == 200)
                    {
                        if (respond is UserChatMessagesObject result)
                        {
                            var respondList = result.Messages.Count;
                            if (respondList > 0)
                            {
                                foreach (var item in from item in result.Messages let check = MAdapter.DifferList.FirstOrDefault(a => a.MesData.Id == item.Id) where check == null select item)
                                {
                                    var type = MAdapter.GetTypeModel(item);
                                    if (type == MessageModelType.None)
                                        continue;

                                    var check = MAdapter.DifferList.FirstOrDefault(a => a.MesData.Id == item.Id);
                                    if (check != null) continue;
                                    var mes = new AdapterModelsClassUser()
                                    {
                                        TypeView = type,
                                        Id = Long.ParseLong(item.Id),
                                        MesData = WoWonderTools.MessageFilter(Userid, item, type)
                                    };

                                    MAdapter.DifferList.Insert(0, mes);

                                    RunOnUiThread(() =>
                                    {
                                        MAdapter?.NotifyItemInserted(MAdapter.DifferList.IndexOf(mes));

                                        var indexMes = MAdapter.DifferList.IndexOf(data);
                                        if (indexMes > -1)
                                        {
                                            //Scroll Down >> 
                                            //MRecycler.SmoothScrollToPosition(indexMes);
                                        }
                                    });
                                }

                                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                // Insert data user in database
                                dbDatabase.Insert_Or_Replace_MessagesTable(MAdapter.DifferList);
                                dbDatabase.Dispose();
                            }
                        }
                    }
                    else Methods.DisplayReportResult(this, respond);

                    //if (SwipeRefreshLayout.Refreshing)
                    //{
                    //    SwipeRefreshLayout.Refreshing = false;
                    //    SwipeRefreshLayout.Enabled = false;

                    //}

                    RunLoadMore = false;
                }
                else Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                RunLoadMore = false;
                Console.WriteLine(e);
            }
        }

        //Get Data User API
        private async void Get_UserProfileData_Api()
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
                else
                {
                    var (apiStatus, respond) = await RequestsAsync.Global.Get_User_Data(Userid);
                    if (apiStatus == 200)
                    {
                        if (respond is GetUserDataObject result)
                        {
                            //Add Data User
                            //=======================================
                            if (result.UserData != null)
                            {
                                RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        ActionBarTitle.Text = Methods.FunString.DecodeString(result.UserData.Name);

                                        //Online Or offline
                                        if (result.UserData.LastseenStatus == "on")
                                        {
                                            ActionBarSubTitle.Text = GetString(Resource.String.Lbl_Online);
                                            LastSeen = GetString(Resource.String.Lbl_Online);
                                        }
                                        else
                                        {
                                            ActionBarSubTitle.Text = GetString(Resource.String.Lbl_Last_seen) + " " + Methods.Time.TimeAgo(int.Parse(result.UserData.LastseenUnixTime), false);
                                            LastSeen = GetString(Resource.String.Lbl_Last_seen) + " " + Methods.Time.TimeAgo(int.Parse(result.UserData.LastseenUnixTime), false);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                });
                            }
                        }
                    }
                    else Methods.DisplayReportResult(this, respond);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Get_UserProfileData_Api();
            }
        }

        #endregion

        private void ResetMediaPlayer()
        {
            try
            {
                var list = MAdapter.DifferList.Where(a => a.TypeView == MessageModelType.LeftAudio || a.TypeView == MessageModelType.RightAudio && a.MesData.MediaPlayer != null).ToList();
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        if (item.MesData.MediaPlayer != null)
                        {
                            item.MesData.MediaPlayer.Stop();
                            item.MesData.MediaPlayer.Reset();
                        }
                        item.MesData.MediaPlayer = null;
                        item.MesData.MediaTimer = null;

                        item.MesData.MediaPlayer?.Release();
                        item.MesData.MediaPlayer = null;
                    }
                    MAdapter.NotifyDataSetChanged();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ResetButtonTags()
        {
            try
            {
                ChatStickerButton.Tag = "Closed";
                ChatColorButton.Tag = "Closed";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnBackPressed()
        {
            try
            {
                if (SupportFragmentManager.BackStackEntryCount > 0)
                {
                    SupportFragmentManager.PopBackStack();
                    ResetButtonTags();
                    ChatColorButton.Drawable.SetTint(Color.ParseColor("#888888"));
                    ChatStickerButton.Drawable.SetTint(Color.ParseColor("#888888"));

                    if (SupportFragmentManager.Fragments.Count > 0)
                    {
                        var fragmentManager = SupportFragmentManager.BeginTransaction();
                        foreach (var vrg in SupportFragmentManager.Fragments)
                        {
                            Console.WriteLine(vrg);
                            if (SupportFragmentManager.Fragments.Contains(ChatColorBoxFragment))
                            {
                                fragmentManager.Remove(ChatColorBoxFragment);
                            }
                            else if (SupportFragmentManager.Fragments.Contains(ChatStickersTabBoxFragment))
                            {
                                fragmentManager.Remove(ChatStickersTabBoxFragment);
                            }
                        }

                        fragmentManager.Commit();
                    }
                }
                else
                {
                    MainChatColor = AppSettings.MainColor;
                    ColorChanged = false;
                    base.OnBackPressed();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnLayoutChange(View v, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight, int oldBottom)
        {
            try
            {
                MRecycler.ScrollToPosition(MAdapter.ItemCount - 1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region Fragment

        private void ReplaceTopFragment(SupportFragment fragmentView)
        {
            try
            {
                if (fragmentView.IsVisible)
                    return;

                var trans = SupportFragmentManager.BeginTransaction();
                trans.Replace(TopFragmentHolder.Id, fragmentView);

                if (SupportFragmentManager.BackStackEntryCount == 0)
                {
                    trans.AddToBackStack(null);
                }

                trans.Commit();

                TopFragmentHolder.TranslationY = 1200;
                TopFragmentHolder.Animate().SetInterpolator(new FastOutSlowInInterpolator()).TranslationYBy(-1200)
                    .SetDuration(500);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ReplaceButtonFragment(SupportFragment fragmentView)
        {
            try
            {
                if (fragmentView != MainFragmentOpened)
                {
                    if (MainFragmentOpened == ChatColorBoxFragment)
                    {
                        ChatColorButton.Drawable.SetTint(Color.ParseColor("#888888"));
                    }
                    else if (MainFragmentOpened == ChatStickersTabBoxFragment)
                    {
                        ChatStickerButton.Drawable.SetTint(Color.ParseColor("#888888"));
                    }
                }

                if (fragmentView.IsVisible)
                    return;

                var trans = SupportFragmentManager.BeginTransaction();
                trans.Replace(ButtonFragmentHolder.Id, fragmentView);

                if (SupportFragmentManager.BackStackEntryCount == 0)
                {
                    trans.AddToBackStack(null);
                }

                trans.Commit();

                ButtonFragmentHolder.TranslationY = 1200;
                ButtonFragmentHolder.Animate().SetInterpolator(new FastOutSlowInInterpolator()).TranslationYBy(-1200)
                    .SetDuration(500);
                MainFragmentOpened = fragmentView;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Selected

        //Copy Messages
        private void CopyItems()
        {
            try
            {
                string allText = "";
                if (SelectedItemPositions != null && !string.IsNullOrEmpty(SelectedItemPositions.MesData.Text))
                {
                    allText = SelectedItemPositions.MesData.Text;
                }
                Methods.CopyToClipboard(this, allText);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Forward Messages
        private void ForwardItems()
        {
            try
            {
                if (Timer != null)
                {
                    Timer.Enabled = false;
                    Timer.Stop();
                }

                if (SelectedItemPositions != null)
                {
                    var intent = new Intent(this, typeof(ForwardMessagesActivity));
                    intent.PutExtra("SelectedItem", JsonConvert.SerializeObject(SelectedItemPositions.MesData));
                    StartActivity(intent);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
            var (apiStatus, respond) = await RequestsAsync.Global.Get_User_Data(Userid, "user_data,followers,following");
            if (apiStatus != 200 || !(respond is GetUserDataObject result) || result.UserData == null)
            {
                Methods.DisplayReportResult(this, respond);
            }
            else
            {
                try
                {
                    if (DataUser != null && AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                        DataUser.UserData = result.UserData;
                    else if (DataUserChat != null && AppSettings.LastChatSystem == SystemApiGetLastChat.Old)
                        DataUserChat = CovertProfile(result.UserData);
                    else
                        UserData = result.UserData;

                    Glide.With(this).Load(result.UserData.Avatar).Apply(new RequestOptions().SetDiskCacheStrategy(DiskCacheStrategy.All).CenterCrop()).Into(UserChatProfile);
                    ActionBarTitle.Text = Methods.FunString.DecodeString(result.UserData.Name);
                    SayHiToTextView.Text = Methods.FunString.DecodeString(result.UserData.Name);

                    //Online Or offline
                    if (UserData.Lastseen == "on")
                    {
                        ActionBarSubTitle.Text = GetString(Resource.String.Lbl_Online);
                        LastSeen = GetString(Resource.String.Lbl_Online);
                    }
                    else
                    {
                        ActionBarSubTitle.Text = GetString(Resource.String.Lbl_Last_seen) + " " + Methods.Time.TimeAgo(int.Parse(result.UserData.LastseenUnixTime), false);
                        LastSeen = GetString(Resource.String.Lbl_Last_seen) + " " + Methods.Time.TimeAgo(int.Parse(result.UserData.LastseenUnixTime), false);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        private GetUsersListObject.User CovertProfile(UserDataObject userData)
        {
            try
            {
                DataUserChat.UserId = userData.UserId;
                DataUserChat.Username = userData.Username;
                DataUserChat.Email = userData.Email;
                DataUserChat.FirstName = userData.FirstName;
                DataUserChat.LastName = userData.LastName;
                DataUserChat.Avatar = userData.Avatar;
                DataUserChat.Cover = userData.Cover;
                DataUserChat.BackgroundImage = userData.BackgroundImage;
                DataUserChat.RelationshipId = userData.RelationshipId;
                DataUserChat.Address = userData.Address;
                DataUserChat.Working = userData.Working;
                DataUserChat.Gender = userData.Gender;
                DataUserChat.Facebook = userData.Facebook;
                DataUserChat.Google = userData.Google;
                DataUserChat.Twitter = userData.Twitter;
                DataUserChat.Linkedin = userData.Linkedin;
                DataUserChat.Website = userData.Website;
                DataUserChat.Instagram = userData.Instagram;
                DataUserChat.WebDeviceId = userData.WebDeviceId;
                DataUserChat.Language = userData.Language;
                DataUserChat.IpAddress = userData.IpAddress;
                DataUserChat.PhoneNumber = userData.PhoneNumber;
                DataUserChat.Timezone = userData.Timezone;
                DataUserChat.Lat = userData.Lat;
                DataUserChat.Lng = userData.Lng;
                DataUserChat.About = userData.About;
                DataUserChat.Birthday = userData.Birthday;
                DataUserChat.Registered = userData.Registered;
                DataUserChat.Lastseen = userData.Lastseen;
                DataUserChat.LastLocationUpdate = userData.LastLocationUpdate;
                DataUserChat.Balance = userData.Balance;
                DataUserChat.Verified = userData.Verified;
                DataUserChat.Status = userData.Status;
                DataUserChat.Active = userData.Active;
                DataUserChat.Admin = userData.Admin;
                DataUserChat.IsPro = userData.IsPro;
                DataUserChat.ProType = userData.ProType;
                DataUserChat.School = userData.School;
                DataUserChat.Name = userData.Name;
                DataUserChat.AndroidMDeviceId = userData.AndroidMDeviceId;
                DataUserChat.ECommented = userData.ECommented;
                DataUserChat.AndroidNDeviceId = userData.AndroidMDeviceId;
                DataUserChat.AvatarFull = userData.AvatarFull;
                DataUserChat.BirthPrivacy = userData.BirthPrivacy;
                DataUserChat.CanFollow = userData.CanFollow;
                DataUserChat.ConfirmFollowers = userData.ConfirmFollowers;
                DataUserChat.CountryId = userData.CountryId;
                DataUserChat.EAccepted = userData.EAccepted;
                DataUserChat.EFollowed = userData.EFollowed;
                DataUserChat.EJoinedGroup = userData.EJoinedGroup;
                DataUserChat.ELastNotif = userData.ELastNotif;
                DataUserChat.ELiked = userData.ELiked;
                DataUserChat.ELikedPage = userData.ELikedPage;
                DataUserChat.EMentioned = userData.EMentioned;
                DataUserChat.EProfileWallPost = userData.EProfileWallPost;
                DataUserChat.ESentmeMsg = userData.ESentmeMsg;
                DataUserChat.EShared = userData.EShared;
                DataUserChat.EVisited = userData.EVisited;
                DataUserChat.EWondered = userData.EWondered;
                DataUserChat.EmailNotification = userData.EmailNotification;
                DataUserChat.FollowPrivacy = userData.FollowPrivacy;
                DataUserChat.FriendPrivacy = userData.FriendPrivacy;
                DataUserChat.GenderText = userData.GenderText;
                DataUserChat.InfoFile = userData.InfoFile;
                DataUserChat.IosMDeviceId = userData.IosMDeviceId;
                DataUserChat.IosNDeviceId = userData.IosNDeviceId;
                DataUserChat.IsFollowing = userData.IsFollowing;
                DataUserChat.IsFollowingMe = userData.IsFollowingMe;
                DataUserChat.LastAvatarMod = userData.LastAvatarMod;
                DataUserChat.LastCoverMod = userData.LastCoverMod;
                DataUserChat.LastDataUpdate = userData.LastDataUpdate;
                DataUserChat.LastFollowId = userData.LastFollowId;
                DataUserChat.LastLoginData = userData.LastLoginData;
                DataUserChat.LastseenStatus = userData.LastseenStatus;
                DataUserChat.LastseenTimeText = userData.LastseenTimeText;
                DataUserChat.LastseenUnixTime = userData.LastseenUnixTime;
                DataUserChat.MessagePrivacy = userData.MessagePrivacy;
                DataUserChat.NewEmail = userData.NewEmail;
                DataUserChat.NewPhone = userData.NewPhone;
                DataUserChat.NotificationSettings = userData.NotificationSettings;
                DataUserChat.NotificationsSound = userData.NotificationsSound;
                DataUserChat.OrderPostsBy = userData.OrderPostsBy;
                DataUserChat.PaypalEmail = userData.PaypalEmail;
                DataUserChat.PostPrivacy = userData.PostPrivacy;
                DataUserChat.Referrer = userData.Referrer;
                DataUserChat.ShareMyData = userData.ShareMyData;
                DataUserChat.ShareMyLocation = userData.ShareMyLocation;
                DataUserChat.ShowActivitiesPrivacy = userData.ShowActivitiesPrivacy;
                DataUserChat.TwoFactor = userData.TwoFactor;
                DataUserChat.TwoFactorVerified = userData.TwoFactorVerified;
                DataUserChat.Url = userData.Url;
                DataUserChat.VisitPrivacy = userData.VisitPrivacy;
                DataUserChat.Vk = userData.Vk;
                DataUserChat.Wallet = userData.Wallet;
                DataUserChat.WorkingLink = userData.WorkingLink;
                DataUserChat.Youtube = userData.Youtube;
                DataUserChat.City = userData.City;
                DataUserChat.Points = userData.Points;
                DataUserChat.DailyPoints = userData.DailyPoints;
                DataUserChat.State = userData.State;
                DataUserChat.Zip = userData.Zip;
                DataUserChat.Details = new DetailsUnion() { DetailsClass = new Details() };
                var detailsUnion = DataUserChat.Details;
                detailsUnion.DetailsClass = userData.Details.DetailsClass;
                DataUserChat.IsAdmin = userData.IsAdmin;
                DataUserChat.IsBlocked = userData.IsBlocked;
                DataUserChat.MemberId = userData.MemberId;
                DataUserChat.OldAvatar = userData.Avatar;
                DataUserChat.OldCover = userData.Cover;
                DataUserChat.PointDayExpire = userData.PointDayExpire;
                DataUserChat.Type = userData.Type;
                DataUserChat.UserPlatform = userData.UserPlatform;
                DataUserChat.ChatColor = DataUserChat.ChatColor;
                DataUserChat.ChatTime = DataUserChat.ChatTime;
                DataUserChat.LastMessage = DataUserChat.LastMessage;

                Console.WriteLine(detailsUnion);

                return DataUserChat;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return DataUserChat;
            }
        }

        #endregion

        public void Update_One_Messages(MessageData message)
        {
            try
            {
                var type = MAdapter.GetTypeModel(message);
                if (type == MessageModelType.None)
                    return;

                var checker = MAdapter.DifferList.FirstOrDefault(a => a.MesData.Id == message.Id);
                if (checker != null)
                {
                    checker.Id = Convert.ToInt32(message.Id);
                    checker.MesData = WoWonderTools.MessageFilter(Userid, message, type);
                    checker.TypeView = type;

                    RunOnUiThread(() =>
                    {
                        try
                        {
                            if (MessageController.IsImageExtension(checker.MesData.MediaFileName))
                            {
                                //MAdapter.NotifyItemChanged(MAdapter.DifferList.IndexOf(checker), "WithoutBlobImage");
                            }
                            else if (MessageController.IsVideoExtension(checker.MesData.MediaFileName))
                            {
                                MAdapter.NotifyItemChanged(MAdapter.DifferList.IndexOf(checker), "WithoutBlobVideo");
                            }
                            else if (MessageController.IsAudioExtension(checker.MesData.MediaFileName))
                            {
                                MAdapter.NotifyItemChanged(MAdapter.DifferList.IndexOf(checker), "WithoutBlobAudio");
                            }
                            else if (MessageController.IsFileExtension(checker.MesData.MediaFileName))
                            {
                                MAdapter.NotifyItemChanged(MAdapter.DifferList.IndexOf(checker), "WithoutBlobFile");
                            }
                            else if (checker.MesData.MediaFileName.Contains(".gif") || checker.MesData.MediaFileName.Contains(".GIF"))
                            {
                                MAdapter.NotifyItemChanged(MAdapter.DifferList.IndexOf(checker), "WithoutBlobGIF");
                            }
                            else
                                MAdapter.NotifyItemChanged(MAdapter.DifferList.IndexOf(checker));

                            //Scroll Down >> 
                            MRecycler.ScrollToPosition(MAdapter.ItemCount - 1);

                            if (MAdapter.DifferList.Count > 0)
                            {
                                SayHiLayout.Visibility = ViewStates.Gone;
                                SayHiSuggestionsRecycler.Visibility = ViewStates.Gone;
                            }
                            else if (MAdapter.DifferList.Count == 0 && ShowEmpty != "no")
                            {
                                SayHiLayout.Visibility = ViewStates.Visible;
                                SayHiSuggestionsRecycler.Visibility = ViewStates.Visible;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetTheme(string color)
        {
            try
            {
                if (color.Contains("b582af"))
                {
                    SetTheme(Resource.Style.Chatththemeb582af);
                }
                else if (color.Contains("a84849"))
                {
                    SetTheme(Resource.Style.Chatththemea84849);
                }
                else if (color.Contains("f9c270"))
                {
                    SetTheme(Resource.Style.Chatththemef9c270);
                }
                else if (color.Contains("70a0e0"))
                {
                    SetTheme(Resource.Style.Chatththeme70a0e0);
                }
                else if (color.Contains("56c4c5"))
                {
                    SetTheme(Resource.Style.Chatththeme56c4c5);
                }
                else if (color.Contains("f33d4c"))
                {
                    SetTheme(Resource.Style.Chatththemef33d4c);
                }
                else if (color.Contains("a1ce79"))
                {
                    SetTheme(Resource.Style.Chatththemea1ce79);
                }
                else if (color.Contains("a085e2"))
                {
                    SetTheme(Resource.Style.Chatththemea085e2);
                }
                else if (color.Contains("ed9e6a"))
                {
                    SetTheme(Resource.Style.Chatththemeed9e6a);
                }
                else if (color.Contains("2b87ce"))
                {
                    SetTheme(Resource.Style.Chatththeme2b87ce);
                }
                else if (color.Contains("f2812b"))
                {
                    SetTheme(Resource.Style.Chatththemef2812b);
                }
                else if (color.Contains("0ba05d"))
                {
                    SetTheme(Resource.Style.Chatththeme0ba05d);
                }
                else if (color.Contains("0e71ea"))
                {
                    SetTheme(Resource.Style.Chatththeme0e71ea);
                }
                else if (color.Contains("aa2294"))
                {
                    SetTheme(Resource.Style.Chatththemeaa2294);
                }
                else if (color.Contains("f9a722"))
                {
                    SetTheme(Resource.Style.Chatththemef9a722);
                }
                else if (color.Contains("008484"))
                {
                    SetTheme(Resource.Style.Chatththeme008484);
                }
                else if (color.Contains("5462a5"))
                {
                    SetTheme(Resource.Style.Chatththeme5462a5);
                }
                else if (color.Contains("fc9cde"))
                {
                    SetTheme(Resource.Style.Chatththemefc9cde);
                }
                else if (color.Contains("fc9cde"))
                {
                    SetTheme(Resource.Style.Chatththemefc9cde);
                }
                else if (color.Contains("51bcbc"))
                {
                    SetTheme(Resource.Style.Chatththeme51bcbc);
                }
                else if (color.Contains("c9605e"))
                {
                    SetTheme(Resource.Style.Chatththemec9605e);
                }
                else if (color.Contains("01a5a5"))
                {
                    SetTheme(Resource.Style.Chatththeme01a5a5);
                }
                else if (color.Contains("056bba"))
                {
                    SetTheme(Resource.Style.Chatththeme056bba);
                }
                else
                {
                    //Default Color >> AppSettings.MainColor
                    SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OpenDialogGallery(string typeImage)
        {
            try
            {
                PermissionsType = typeImage;
                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    Methods.Path.Chack_MyFolder(Userid);

                    //Open Image 
                    var myUri = Uri.FromFile(new File(Methods.Path.FolderDiskImage + "/" + Userid, Methods.GetTimestamp(DateTime.Now) + ".jpeg"));
                    CropImage.Builder()
                        .SetInitialCropWindowPaddingRatio(0)
                        .SetAutoZoomEnabled(true)
                        .SetMaxZoom(4)
                        .SetGuidelines(CropImageView.Guidelines.On)
                        .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                        .SetOutputUri(myUri).Start(this);
                }
                else
                {
                    if (!CropImage.IsExplicitCameraPermissionRequired(this) && CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted && CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted)
                    {
                        Methods.Path.Chack_MyFolder(Userid);

                        //Open Image 
                        var myUri = Uri.FromFile(new File(Methods.Path.FolderDiskImage + "/" + Userid, Methods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder()
                            .SetInitialCropWindowPaddingRatio(0)
                            .SetAutoZoomEnabled(true)
                            .SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On)
                            .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
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

        private class XamarinRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
        {
            public delegate void LoadMoreEventHandler(object sender, EventArgs e);

            public event LoadMoreEventHandler LoadMoreEvent;

            private readonly LinearLayoutManager LayoutManager;
            //private readonly SwipeRefreshLayout SwipeRefreshLayout;

            public XamarinRecyclerViewOnScrollListener(LinearLayoutManager layoutManager, SwipeRefreshLayout swipeRefreshLayout)
            {
                LayoutManager = layoutManager;
                Console.WriteLine(swipeRefreshLayout);
                //SwipeRefreshLayout = swipeRefreshLayout;
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                try
                {
                    base.OnScrolled(recyclerView, dx, dy);

                    var visibleItemCount = recyclerView.ChildCount;
                    var totalItemCount = recyclerView.GetAdapter().ItemCount;

                    var pastVisibleItems = LayoutManager.FindFirstVisibleItemPosition();
                    if (pastVisibleItems == 0 && (visibleItemCount != totalItemCount))
                    {
                        //Load More  from API Request
                        LoadMoreEvent?.Invoke(this, null);
                        //Start Load More messages From Database
                    }
                    else
                    {
                        //if (SwipeRefreshLayout.Refreshing)
                        //{
                        //    SwipeRefreshLayout.Refreshing = false;
                        //    SwipeRefreshLayout.Enabled = false;

                        //}
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

        }
    }
}