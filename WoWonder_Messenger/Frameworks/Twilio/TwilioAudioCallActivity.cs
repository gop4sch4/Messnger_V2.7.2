using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using TwilioVideo;
using WoWonder.Activities.Tab;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using WoWonderClient.Classes.Call;
using WoWonderClient.Requests;

namespace WoWonder.Frameworks.Twilio
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation, ResizeableActivity = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class TwilioAudioCallActivity : AppCompatActivity, ISensorEventListener, TwilioVideoHelper.IListener
    {
        #region Variables Basic

        private TwilioVideoHelper TwilioVideo { get; set; }
        private string TwilioAccessToken = "YOUR_TOKEN", TwilioAccessTokenUser2 = "YOUR_TOKEN", RoomName = "TestRoom";
        private string CallId = "0", CallType = "0", UserId = "", Avatar = "0", Name = "0", FromId = "0", Active = "0", Time = "0", Status = "0";
        private CircleButton EndCallButton, SpeakerAudioButton, MuteAudioButton;
        private ImageView UserImageView;
        private TextView UserNameTextView, DurationTextView;
        private Timer TimerRequestWaiter = new Timer();
        private LocalVideoTrack LocalVideoTrack;
        private VideoTrack UserVideoTrack;
        private bool DataUpdated;
        private int CountSecondsOfOutGoingCall;
        private string LocalVideoTrackId, RemoteVideoTrackId;
        private TabbedMainActivity GlobalContext;

        private SensorManager SensorManager;
        private Sensor Proximity;
        private readonly int SENSOR_SENSITIVITY = 4;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                Window.AddFlags(WindowManagerFlags.KeepScreenOn);

                // Create your application here
                SetContentView(Resource.Layout.TwilioAudioCallActivityLayout);

                SensorManager = (SensorManager)GetSystemService(SensorService);
                Proximity = SensorManager.GetDefaultSensor(SensorType.Proximity);

                GlobalContext = TabbedMainActivity.GetInstance();

                //Get Value And Set Toolbar
                InitComponent();
                InitTwilioCall();
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
                SensorManager.RegisterListener(this, Proximity, SensorDelay.Normal);
                AddOrRemoveEvent(true);
                UpdateState();
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
                base.OnStart();
                UpdateState();
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
                DataUpdated = false;
                base.OnPause();
                AddOrRemoveEvent(false);
                SensorManager.UnregisterListener(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnRestart()
        {
            try
            {
                base.OnRestart();
                TwilioVideo = TwilioVideoHelper.GetOrCreate(this);
                UpdateState();
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
                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                SpeakerAudioButton = FindViewById<CircleButton>(Resource.Id.speaker_audio_button);
                EndCallButton = FindViewById<CircleButton>(Resource.Id.end_audio_call_button);
                MuteAudioButton = FindViewById<CircleButton>(Resource.Id.mute_audio_call_button);

                UserImageView = FindViewById<ImageView>(Resource.Id.audiouserImageView);
                UserNameTextView = FindViewById<TextView>(Resource.Id.audiouserNameTextView);
                DurationTextView = FindViewById<TextView>(Resource.Id.audiodurationTextView);

                SpeakerAudioButton.Selected = false;
                SpeakerAudioButton.SetImageResource(Resource.Drawable.ic_speaker_close);
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
                    SpeakerAudioButton.Click += SpeakerAudioButtonOnClick;
                    EndCallButton.Click += EndCallButtonOnClick;
                    MuteAudioButton.Click += MuteAudioButtonOnClick;
                }
                else
                {
                    SpeakerAudioButton.Click -= SpeakerAudioButtonOnClick;
                    EndCallButton.Click -= EndCallButtonOnClick;
                    MuteAudioButton.Click -= MuteAudioButtonOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        private void MuteAudioButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (MuteAudioButton.Selected)
                {
                    MuteAudioButton.Selected = false;
                    MuteAudioButton.SetImageResource(Resource.Drawable.ic_camera_mic_open);
                }
                else
                {
                    MuteAudioButton.Selected = true;
                    MuteAudioButton.SetImageResource(Resource.Drawable.ic_camera_mic_mute);
                }

                TwilioVideo.Mute(MuteAudioButton.Selected);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void EndCallButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                RequestsAsync.Call.DeclineCallAsync(UserDetails.UserId, CallId, TypeCall.Audio).ConfigureAwait(false);
                FinishCall();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SpeakerAudioButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                //Speaker
                if (SpeakerAudioButton.Selected)
                {
                    SpeakerAudioButton.Selected = false;
                    SpeakerAudioButton.SetImageResource(Resource.Drawable.ic_speaker_close);

                }
                else
                {
                    SpeakerAudioButton.Selected = true;
                    SpeakerAudioButton.SetImageResource(Resource.Drawable.ic_speaker_up);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Sensor System

        public void OnAccuracyChanged(Sensor sensor, SensorStatus accuracy)
        {
            try
            {
                // Do something here if sensor accuracy changes.
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void OnSensorChanged(SensorEvent e)
        {
            try
            {
                if (e.Sensor.Type == SensorType.Proximity)
                {
                    if (e.Values[0] >= -SENSOR_SENSITIVITY && e.Values[0] <= SENSOR_SENSITIVITY)
                    {
                        //near 
                        GlobalContext?.SetOffWakeLock();
                    }
                    else
                    {
                        //far 
                        GlobalContext?.SetOnWakeLock();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Twilio  

        private async void InitTwilioCall()
        {
            try
            {
                bool granted =
                    ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) ==
                    Permission.Granted &&
                    ContextCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.RecordAudio) ==
                    Permission.Granted;

                CheckVideoCallPermissions(granted);

                UserId = Intent.GetStringExtra("UserID");
                Avatar = Intent.GetStringExtra("avatar");
                Name = Intent.GetStringExtra("name");

                var dataCallId = Intent.GetStringExtra("CallID") ?? "Data not available";
                if (dataCallId != "Data not available" && !string.IsNullOrEmpty(dataCallId))
                {
                    CallId = dataCallId;

                    TwilioAccessToken = Intent.GetStringExtra("access_token");
                    TwilioAccessTokenUser2 = Intent.GetStringExtra("access_token_2");
                    FromId = Intent.GetStringExtra("from_id");
                    Active = Intent.GetStringExtra("active");
                    var time = Intent.GetStringExtra("time");
                    Status = Intent.GetStringExtra("status");
                    RoomName = Intent.GetStringExtra("room_name");
                    CallType = Intent.GetStringExtra("type");

                    Console.WriteLine(time);
                }

                if (CallType == "Twilio_audio_call")
                {
                    if (!string.IsNullOrEmpty(TwilioAccessToken))
                    {
                        if (!string.IsNullOrEmpty(UserId))
                            Load_userWhenCall();

                        TwilioVideo = TwilioVideoHelper.GetOrCreate(this);
                        UpdateState();
                        DurationTextView.Text = GetText(Resource.String.Lbl_Waiting_for_answer);

                        var (apiStatus, respond) = await RequestsAsync.Call.AnswerCallAsync(UserDetails.UserId, CallId, TypeCall.Audio);
                        if (apiStatus == 200)
                        {
                            ConnectToRoom();

                            var ckd = GlobalContext?.LastCallsTab?.MAdapter?.MCallUser?.FirstOrDefault(a => a.Id == CallId); // id >> Call_Id
                            if (ckd == null)
                            {
                                Classes.CallUser cv = new Classes.CallUser
                                {
                                    Id = CallId,
                                    UserId = UserId,
                                    Avatar = Avatar,
                                    Name = Name,
                                    FromId = FromId,
                                    Active = Active,
                                    Time = "Answered call",
                                    Status = Status,
                                    RoomName = RoomName,
                                    Type = CallType,
                                    TypeIcon = "Accept",
                                    TypeColor = "#008000"
                                };

                                GlobalContext?.LastCallsTab?.MAdapter?.Insert(cv);

                                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                dbDatabase.Insert_CallUser(cv);
                                dbDatabase.Dispose();
                            }
                        }
                        //else Methods.DisplayReportResult(this, respond);
                    }
                }
                else if (CallType == "Twilio_audio_calling_start")
                {
                    DurationTextView.Text = GetText(Resource.String.Lbl_Calling);
                    TwilioVideo = TwilioVideoHelper.GetOrCreate(this);

                    Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("mystic_call.mp3");

                    StartApiService();



                    UpdateState();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Load_userWhenCall()
        {
            try
            {
                UserNameTextView.Text = Name;

                //profile_picture
                GlideImageLoader.LoadImage(this, Avatar, UserImageView, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void StartApiService()
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { LoadProfileFromUserId });
        }

        private async Task LoadProfileFromUserId()
        {
            Load_userWhenCall();
            var (apiStatus, respond) = await RequestsAsync.Call.CreateNewCallAsync(UserDetails.UserId, UserId, TypeCall.Audio);
            if (apiStatus == 200)
            {
                if (respond is CreateNewCallObject result)
                {
                    CallId = result.Id.ToString();
                    TwilioAccessToken = result.AccessToken;
                    TwilioAccessTokenUser2 = result.AccessToken2;
                    RoomName = result.RoomName;

                    TimerRequestWaiter = new Timer { Interval = 5000 };
                    TimerRequestWaiter.Elapsed += TimerCallRequestAnswer_Waiter_Elapsed;
                    TimerRequestWaiter.Start();
                }
            }
            else
            {
                FinishCall();
                //Methods.DisplayReportResult(this, respond);
            }
        }

        private async void TimerCallRequestAnswer_Waiter_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var (apiStatus, respond) = await RequestsAsync.Call.CheckForAnswerAsync(UserDetails.UserId, CallId, TypeCall.Audio);
                if (apiStatus == 200)
                {
                    Methods.AudioRecorderAndPlayer.StopAudioFromAsset();

                    if (!string.IsNullOrEmpty(TwilioAccessToken))
                    {
                        TimerRequestWaiter.Enabled = false;
                        TimerRequestWaiter.Stop();
                        TimerRequestWaiter.Close();

                        RunOnUiThread(async () =>
                        {
                            await Task.Delay(1000);

                            TwilioVideo?.UpdateToken(TwilioAccessTokenUser2);
                            TwilioVideo?.JoinRoom(this, RoomName);

                            var ckd = GlobalContext?.LastCallsTab?.MAdapter?.MCallUser?.FirstOrDefault(a => a.Id == CallId); // id >> Call_Id
                            if (ckd == null)
                            {
                                Classes.CallUser cv = new Classes.CallUser
                                {
                                    Id = CallId,
                                    UserId = UserId,
                                    Avatar = Avatar,
                                    Name = Name,
                                    FromId = FromId,
                                    Active = Active,
                                    Time = "Answered call",
                                    Status = Status,
                                    RoomName = RoomName,
                                    Type = CallType,
                                    TypeIcon = "Accept",
                                    TypeColor = "#008000"
                                };

                                GlobalContext?.LastCallsTab?.MAdapter?.Insert(cv);

                                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                dbDatabase.Insert_CallUser(cv);
                                dbDatabase.Dispose();
                            }
                        });
                    }

                }
                else if (apiStatus == 300)
                {
                    if (CountSecondsOfOutGoingCall < 70)
                    {
                        CountSecondsOfOutGoingCall += 10;
                    }
                    else
                    {
                        RunOnUiThread(() =>
                        {
                            try
                            {
                                //Call Is inactive 
                                TimerRequestWaiter.Enabled = false;
                                TimerRequestWaiter.Stop();
                                TimerRequestWaiter.Close();

                                var ckd = GlobalContext?.LastCallsTab?.MAdapter?.MCallUser?.FirstOrDefault(a => a.Id == CallId); // id >> Call_Id
                                if (ckd == null)
                                {
                                    Classes.CallUser cv = new Classes.CallUser
                                    {
                                        Id = CallId,
                                        UserId = UserId,
                                        Avatar = Avatar,
                                        Name = Name,
                                        FromId = FromId,
                                        Active = Active,
                                        Time = "Missed call",
                                        Status = Status,
                                        RoomName = RoomName,
                                        Type = CallType,
                                        TypeIcon = "Cancel",
                                        TypeColor = "#FF0000"
                                    };

                                    GlobalContext?.LastCallsTab?.MAdapter?.Insert(cv);

                                    SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                    dbDatabase.Insert_CallUser(cv);
                                    dbDatabase.Dispose();
                                }

                                FinishCall();
                            }
                            catch (Exception exception)
                            {
                                Console.WriteLine(exception);
                            }
                        });

                    }
                }
                else
                {
                    RunOnUiThread(() =>
                    {
                        try
                        {
                            //Call Is inactive 
                            TimerRequestWaiter.Enabled = false;
                            TimerRequestWaiter.Stop();
                            TimerRequestWaiter.Close();

                            var ckd = GlobalContext?.LastCallsTab?.MAdapter?.MCallUser?.FirstOrDefault(a => a.Id == CallId); // id >> Call_Id
                            if (ckd == null)
                            {
                                Classes.CallUser cv = new Classes.CallUser
                                {
                                    Id = CallId,
                                    UserId = UserId,
                                    Avatar = Avatar,
                                    Name = Name,
                                    FromId = FromId,
                                    Active = Active,
                                    Time = "Declined call",
                                    Status = Status,
                                    RoomName = RoomName,
                                    Type = CallType,
                                    TypeIcon = "Declined",
                                    TypeColor = "#FF8000"
                                };

                                GlobalContext?.LastCallsTab?.MAdapter?.Insert(cv);

                                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                                dbDatabase.Insert_CallUser(cv);
                                dbDatabase.Dispose();
                            }

                            FinishCall();
                            //Methods.DisplayReportResult(this, respond);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                    });
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Permissions

        private void RequestCameraAndMicrophonePermissions()
        {
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.Camera) ||
                ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.RecordAudio))
                Toast.MakeText(this, GetText(Resource.String.Lbl_Need_Camera), ToastLength.Long).Show();
            else
                ActivityCompat.RequestPermissions(this,
                    new[] { Manifest.Permission.Camera, Manifest.Permission.RecordAudio, Manifest.Permission.ModifyAudioSettings }, 1);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == 1)
                CheckVideoCallPermissions(grantResults.Any(p => p == Permission.Denied));
        }

        private void CheckVideoCallPermissions(bool granted)
        {
            if (!granted)
                RequestCameraAndMicrophonePermissions();
        }


        #endregion

        #region TwilioVideoHelper.IListener

        public void SetLocalVideoTrack(LocalVideoTrack track)
        {
            try
            {
                if (LocalVideoTrack == null)
                {
                    LocalVideoTrack = track;
                    var trackId = track?.Name;
                    if (LocalVideoTrackId == trackId)
                    {
                        LocalVideoTrack.Enable(false);
                    }
                    else
                    {
                        LocalVideoTrackId = trackId;
                        LocalVideoTrack.Enable(false);
                    }
                }
                else
                {
                    if (LocalVideoTrack.IsEnabled)
                        LocalVideoTrack.Enable(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetRemoteVideoTrack(VideoTrack track)
        {
            try
            {
                var trackId = track?.Name;

                if (RemoteVideoTrackId == trackId)
                    return;

                RemoteVideoTrackId = trackId;
                if (UserVideoTrack == null)
                {
                    UserVideoTrack = track;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void RemoveLocalVideoTrack(LocalVideoTrack track)
        {
            try
            {
                SetLocalVideoTrack(null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void RemoveRemoteVideoTrack(VideoTrack track)
        {

        }

        public void OnRoomConnected(string roomId)
        {

        }

        public void OnRoomDisconnected(TwilioVideoHelper.StopReason reason)
        {
            try
            {
                Toast.MakeText(this, GetText(Resource.String.Lbl_Room_Disconnected), ToastLength.Short).Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnParticipantConnected(string participantId)
        {
            try
            {
                DurationTextView.Text = GetText(Resource.String.Lbl_Connected);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnParticipantDisconnected(string participantId)
        {
            RunOnUiThread(async () =>
            {
                try
                {
                    DurationTextView.Text = GetText(Resource.String.Lbl_User_Lost_Connection);
                    await Task.Delay(2000);
                    FinishCall();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        public void SetCallTime(int seconds)
        {
            try
            {
                DurationTextView.Text = seconds.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        private void ConnectToRoom()
        {
            TwilioVideo?.UpdateToken(TwilioAccessToken);
            TwilioVideo?.JoinRoom(ApplicationContext, RoomName);
        }

        public override bool OnSupportNavigateUp()
        {
            TryCancelCall();
            return true;
        }

        public override void OnBackPressed()
        {
            FinishCall();
        }

        void UpdateState()
        {
            try
            {
                if (DataUpdated)
                    return;

                DataUpdated = true;

                TwilioVideo?.Bind(this);

                UpdatingState();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void UpdatingState()
        {
        }

        private void TryCancelCall()
        {
            CloseScreen();
        }

        private void CloseScreen()
        {
            Finish();
        }

        private void FinishCall()
        {
            try
            {
                if (TwilioVideo.ClientIsReady)
                {
                    TwilioVideo.Unbind(this);
                    TwilioVideo.FinishCall();
                }

                Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                Finish();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}