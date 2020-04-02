using Android.App;
using Android.Content.PM;
using Android.Hardware;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using AT.Markushi.UI;
using DT.Xamarin.Agora;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using WoWonder.Activities.Tab;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;

namespace WoWonder.Frameworks.Agora
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation, ResizeableActivity = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class AgoraAudioCallActivity : AppCompatActivity, ISensorEventListener
    {
        #region Variables Basic

        private string RoomName = "TestRoom";
        private string CallId = "0";
        private string CallType = "0";
        private string UserId = "";
        private string Avatar = "0";
        private string Name = "0";
        private string FromId = "0";
        private string Active = "0";
        private string Status = "0";

        private RtcEngine AgoraEngine;
        private AgoraRtcAudioCallHandler AgoraHandler;

        private CircleButton EndCallButton;
        private CircleButton SpeakerAudioButton;
        private CircleButton MuteAudioButton;
        private ImageView UserImageView;
        private TextView UserNameTextView;
        private TextView DurationTextView;
        private Timer TimerSound;

        private int CountSecoundsOfOutgoingCall;
        private Timer TimerRequestWaiter = new Timer();
        private TabbedMainActivity GlobalContext;

        private SensorManager SensorManager;
        private Sensor Proximity;
        private readonly int SensorSensitivity = 4;


        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.AgoraAudioCallActivityLayout);

                GlobalContext = TabbedMainActivity.GetInstance();

                SensorManager = (SensorManager)GetSystemService(SensorService);
                Proximity = SensorManager.GetDefaultSensor(SensorType.Proximity);

                UserId = Intent.GetStringExtra("UserID");
                Avatar = Intent.GetStringExtra("avatar");
                Name = Intent.GetStringExtra("name");

                var dataCallId = Intent.GetStringExtra("CallID") ?? "Data not available";
                if (dataCallId != "Data not available" && !string.IsNullOrEmpty(dataCallId))
                {
                    CallId = dataCallId;

                    FromId = Intent.GetStringExtra("from_id");
                    Active = Intent.GetStringExtra("active");
                    var time = Intent.GetStringExtra("time");
                    Status = Intent.GetStringExtra("status");
                    RoomName = Intent.GetStringExtra("room_name");
                    CallType = Intent.GetStringExtra("type");
                    Console.WriteLine(time);
                }

                SpeakerAudioButton = FindViewById<CircleButton>(Resource.Id.speaker_audio_button);
                EndCallButton = FindViewById<CircleButton>(Resource.Id.end_audio_call_button);
                MuteAudioButton = FindViewById<CircleButton>(Resource.Id.mute_audio_call_button);

                UserImageView = FindViewById<ImageView>(Resource.Id.audiouserImageView);
                UserNameTextView = FindViewById<TextView>(Resource.Id.audiouserNameTextView);
                DurationTextView = FindViewById<TextView>(Resource.Id.audiodurationTextView);

                SpeakerAudioButton.Click += Speaker_audio_button_Click;
                EndCallButton.Click += End_call_button_Click;
                MuteAudioButton.Click += Mute_audio_button_Click;


                SpeakerAudioButton.SetImageResource(Resource.Drawable.ic_speaker_close);


                LoadUserInfo(UserId);

                if (CallType == "Agora_audio_calling_start")
                {
                    Start_Call_Action("call");
                }
                else
                {
                    Start_Call_Action("recieve_call");
                }


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
                SensorManager.UnregisterListener(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public async void Start_Call_Action(string type)
        {
            try
            {
                if (type == "call")
                {
                    DurationTextView.Text = GetText(Resource.String.Lbl_Calling);
                    var apiStartCall = await ApiRequest.Create_Agora_Call_Event_Async(UserId, "audio");
                    if (apiStartCall != null && !string.IsNullOrEmpty(apiStartCall.RoomName) && !string.IsNullOrEmpty(apiStartCall.Id))
                    {
                        if (!string.IsNullOrEmpty(apiStartCall.RoomName))
                            RoomName = apiStartCall.RoomName;
                        if (!string.IsNullOrEmpty(apiStartCall.Id))
                            CallId = apiStartCall.Id;
                        Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("mystic_call.mp3");

                        TimerRequestWaiter = new Timer();
                        TimerRequestWaiter.Interval = 5000;
                        TimerRequestWaiter.Elapsed += TimerCallRequestAnswer_Waiter_Elapsed;
                        TimerRequestWaiter.Start();
                        InitAgoraEngineAndJoinChannel(RoomName); //the caller Is Joining agora Server
                    }
                }
                else
                {
                    RoomName = Intent.GetStringExtra("room_name");
                    CallId = Intent.GetStringExtra("CallID");
                    DurationTextView.Text = GetText(Resource.String.Lbl_Waiting_to_connect);

                    var apiStartCall = await ApiRequest.Send_Agora_Call_Action_Async("answer", CallId);
                    if (apiStartCall == "200")
                    {
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
                        InitAgoraEngineAndJoinChannel(RoomName); //the caller Is Joining agora Server
                    }
                    else
                    {
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

                        DurationTextView.Text = GetText(Resource.String.Lbl_Faild_to_connect);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void LoadUserInfo(string userid)
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

        public async void OnCallTime_Running_Out()
        {
            try
            {
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
                Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Error.mp3");
                DurationTextView.Text = GetText(Resource.String.Lbl_No_respond_from_the_user);
                await Task.Delay(3000);
                AgoraEngine.LeaveChannel();
                AgoraEngine.Dispose();
                AgoraEngine = null;
                Finish();
            }
            catch (Exception e)
            {
                Finish();
                Console.WriteLine(e);
            }
        }

        public async void OnCall_Declined_From_User()
        {
            try
            {
                Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Error.mp3");
                DurationTextView.Text = GetText(Resource.String.Lbl_The_user_declinde_your_call);
                await Task.Delay(3000);
                AgoraEngine.LeaveChannel();
                AgoraEngine.Dispose();
                AgoraEngine = null;
                Finish();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void TimerCallRequestAnswer_Waiter_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var callResultGeneration = await ApiRequest.Check_Agora_Call_Answer_Async(CallId, "audio");

                if (string.IsNullOrEmpty(callResultGeneration))
                    return;

                if (callResultGeneration == "answered")
                {

                    TimerRequestWaiter.Enabled = false;
                    TimerRequestWaiter.Stop();
                    TimerRequestWaiter.Close();

                    RunOnUiThread(() =>
                    {
                        Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                        InitAgoraEngineAndJoinChannel(RoomName);
                    });

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
                else if (callResultGeneration == "calling")
                {
                    if (CountSecoundsOfOutgoingCall < 80)
                    {
                        CountSecoundsOfOutgoingCall += 10;
                    }
                    else
                    {
                        //Call Is inactive 
                        TimerRequestWaiter.Enabled = false;
                        TimerRequestWaiter.Stop();
                        TimerRequestWaiter.Close();

                        RunOnUiThread(OnCallTime_Running_Out);
                    }


                }
                else if (callResultGeneration == "declined")
                {
                    TimerRequestWaiter.Enabled = false;
                    TimerRequestWaiter.Stop();
                    TimerRequestWaiter.Close();

                    RunOnUiThread(OnCall_Declined_From_User);

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
                }
                else if (callResultGeneration == "no_answer")
                {
                    //Call Is inactive 
                    TimerRequestWaiter.Enabled = false;
                    TimerRequestWaiter.Stop();
                    TimerRequestWaiter.Close();

                    RunOnUiThread(OnCallTime_Running_Out);

                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        private void Mute_audio_button_Click(object sender, EventArgs e)
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

                AgoraEngine.MuteLocalAudioStream(MuteAudioButton.Selected);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void End_call_button_Click(object sender, EventArgs e)
        {
            try
            {
                Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                if (AgoraEngine != null)
                {
                    try
                    {
                        AgoraEngine.StopPreview();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);

                    }
                    try
                    {
                        AgoraEngine.SetupLocalVideo(null);
                        AgoraEngine.LeaveChannel();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }

                    AgoraEngine.Dispose();
                    AgoraEngine = null;
                }
                Finish();
            }
            catch (Exception exception)
            {
                Finish();
                Console.WriteLine(exception);
            }
        }

        private void Speaker_audio_button_Click(object sender, EventArgs e)
        {
            try
            {
                //Speaker
                if (SpeakerAudioButton.Selected)
                {
                    SpeakerAudioButton.Selected = false;
                    SpeakerAudioButton.SetImageResource(Resource.Drawable.ic_speaker_close);
                    AgoraEngine.SetEnableSpeakerphone(false);
                }
                else
                {
                    SpeakerAudioButton.Selected = true;

                    SpeakerAudioButton.SetImageResource(Resource.Drawable.ic_speaker_up);
                    AgoraEngine.SetEnableSpeakerphone(true);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override void OnBackPressed()
        {
            //Close Api Starts here >>
            try
            {
                Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                if (AgoraEngine != null)
                {
                    try
                    {
                        AgoraEngine.StopPreview();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                    try
                    {
                        AgoraEngine.SetupLocalVideo(null);
                        AgoraEngine.LeaveChannel();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }

                    AgoraEngine.Dispose();
                    AgoraEngine = null;
                }
            }
            catch (Exception exception)
            {
                base.OnBackPressed();
                Console.WriteLine(exception);
            }
            base.OnBackPressed();
        }

        public void InitAgoraEngineAndJoinChannel(string roomName)
        {
            try
            {
                AgoraHandler = new AgoraRtcAudioCallHandler(this);
                AgoraEngine = RtcEngine.Create(BaseContext, AgoraSettings.AgoraApi, AgoraHandler);

                AgoraEngine.DisableVideo();
                AgoraEngine.EnableAudio();
                AgoraEngine.SetEncryptionMode("aes-128-xts");
                AgoraEngine.SetEncryptionSecret("");
                AgoraEngine.JoinChannel(null, roomName, string.Empty, 0);
                AgoraEngine.SetEnableSpeakerphone(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnUserOffline(int uid, int reason)
        {
            RunOnUiThread(async () =>
            {
                try
                {
                    Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                    Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Error.mp3");
                    DurationTextView.Text = GetText(Resource.String.Lbl_Lost_his_connection);
                    await Task.Delay(3000);
                    AgoraEngine.LeaveChannel();
                    AgoraEngine.Dispose();
                    AgoraEngine = null;
                    Finish();
                }
                catch (Exception e)
                {
                    Finish();
                    Console.WriteLine(e);
                }
            });
        }

        public void OnUserJoined(int uid, int reason)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    DurationTextView.Text = GetText(Resource.String.Lbl_Please_wait);
                    Methods.AudioRecorderAndPlayer.StopAudioFromAsset();

                    TimerSound = new Timer();
                    TimerSound.Interval = 1000;
                    TimerSound.Elapsed += TimerSound_Elapsed;
                    TimerSound.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            try
            {
                //Close Api Starts here >>
                ApiRequest.Send_Agora_Call_Action_Async("close", CallId).ConfigureAwait(false);

                if (AgoraEngine != null)
                {
                    AgoraEngine.LeaveChannel();
                    AgoraEngine.Dispose();
                    AgoraEngine = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void TimerSound_Elapsed(object sender, ElapsedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                //Write your own duration function here 
                string s = TimeSpan.FromSeconds(e.SignalTime.Second).ToString(@"mm\:ss");
                DurationTextView.Text = s;
            });
        }

        public void OnConnectionLost()
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(this, GetText(Resource.String.Lbl_Lost_Connection), ToastLength.Short).Show();

                Finish();
            });
        }

        public void OnNetworkQuality(int p0, int p1, int p2)
        {
            RunOnUiThread(() =>
            {
                if (p1 == 3 || p2 == 3)
                {
                    //QUALITY_POOR(3)
                }
                else if (p1 == 4 || p2 == 4)
                {
                    //QUALITY_VBAD(5)
                }
                else if (p1 == 5 || p2 == 5)
                {
                    //QUALITY_DOWN(6)
                }
                else if (p1 == 6 || p2 == 6)
                {
                    //QUALITY_DOWN(6)
                }
            });
        }

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
                    if (e.Values[0] >= -SensorSensitivity && e.Values[0] <= SensorSensitivity)
                    {
                        //near 
                        TabbedMainActivity.GetInstance()?.SetOffWakeLock();
                    }
                    else
                    {
                        //far 
                        TabbedMainActivity.GetInstance()?.SetOnWakeLock();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

    }
}