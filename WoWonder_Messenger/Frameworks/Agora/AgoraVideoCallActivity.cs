using Android.App;
using Android.Content.PM;
using Android.Content.Res;
using Android.Hardware;
using Android.OS;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using DT.Xamarin.Agora;
using DT.Xamarin.Agora.Video;
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
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.ScreenLayout | ConfigChanges.Orientation, LaunchMode = LaunchMode.SingleInstance, ResizeableActivity = true, SupportsPictureInPicture = true)]
    public class AgoraVideoCallActivity : AppCompatActivity, ISensorEventListener
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

        private const int MaxLocalVideoDimension = 150;
        private RtcEngine AgoraEngine;
        private AgoraRtcHandler AgoraHandler;
        private bool IsVideoEnabled = true;
        private SurfaceView LocalVideoView;

        //Controls
        private Button SwitchCamButton;

        private CircleButton EndCallButton;
        private CircleButton MuteVideoButton;
        private CircleButton MuteAudioButton;
        private RelativeLayout UserInfoviewContainer;
        private ImageView UserImageView, PictureInToPictureButton;
        private TextView UserNameTextView;
        private TextView NoteTextView;

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
                SetContentView(Resource.Layout.AgoraVideoCallActivityLayout);

                GlobalContext = TabbedMainActivity.GetInstance();

                SensorManager = (SensorManager)GetSystemService(SensorService);
                Proximity = SensorManager.GetDefaultSensor(SensorType.Proximity);

                UserId = Intent.GetStringExtra("UserID");
                Avatar = Intent.GetStringExtra("avatar");
                Name = Intent.GetStringExtra("name");

                var dataCallId = Intent.GetStringExtra("CallID") ?? "Data not available";
                if (dataCallId != "Data not available" && !String.IsNullOrEmpty(dataCallId))
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

                SwitchCamButton = FindViewById<Button>(Resource.Id.switch_cam_button);
                MuteVideoButton = FindViewById<CircleButton>(Resource.Id.mute_video_button);
                EndCallButton = FindViewById<CircleButton>(Resource.Id.end_call_button);
                MuteAudioButton = FindViewById<CircleButton>(Resource.Id.mute_audio_button);

                UserInfoviewContainer = FindViewById<RelativeLayout>(Resource.Id.userInfoview_container);
                UserImageView = FindViewById<ImageView>(Resource.Id.userImageView);
                UserNameTextView = FindViewById<TextView>(Resource.Id.userNameTextView);
                NoteTextView = FindViewById<TextView>(Resource.Id.noteTextView);
                PictureInToPictureButton = FindViewById<ImageView>(Resource.Id.pictureintopictureButton);

                if (!PackageManager.HasSystemFeature(PackageManager.FeaturePictureInPicture))
                    PictureInToPictureButton.Visibility = ViewStates.Gone;

                SwitchCamButton.Click += Switch_cam_button_Click;
                MuteVideoButton.Click += Mute_video_button_Click;
                EndCallButton.Click += End_call_button_Click;
                MuteAudioButton.Click += Mute_audio_button_Click;
                PictureInToPictureButton.Click += PictureInToPictureButton_Click;

                LoadUserInfo(UserId);

                if (CallType == "Agora_video_calling_start")
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

        private void PictureInToPictureButton_Click(object sender, EventArgs e)
        {
            try
            {
                //var actions = new List<RemoteAction>();
                //.SetActions(new List<RemoteAction>().Add(new RemoteAction().Title = "")
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var param = new PictureInPictureParams.Builder().SetAspectRatio(new Rational(9, 16)).Build();
                    EnterPictureInPictureMode(param);
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }


        }

        public override void OnPictureInPictureModeChanged(bool isInPictureInPictureMode, Configuration newConfig)
        {
            try
            {
                if (isInPictureInPictureMode)
                {
                    EndCallButton.Visibility = ViewStates.Gone;
                    MuteAudioButton.Visibility = ViewStates.Gone;
                    MuteVideoButton.Visibility = ViewStates.Gone;
                    UserNameTextView.Visibility = ViewStates.Gone;
                    NoteTextView.Visibility = ViewStates.Gone;
                    PictureInToPictureButton.Visibility = ViewStates.Gone;
                    FindViewById(Resource.Id.local_video_container).Visibility = ViewStates.Gone;
                }
                else
                {
                    EndCallButton.Visibility = ViewStates.Visible;
                    MuteAudioButton.Visibility = ViewStates.Visible;
                    UserNameTextView.Visibility = ViewStates.Visible;
                    NoteTextView.Visibility = ViewStates.Visible;
                    MuteVideoButton.Visibility = ViewStates.Visible;
                    PictureInToPictureButton.Visibility = ViewStates.Visible;
                    FindViewById(Resource.Id.local_video_container).Visibility = ViewStates.Visible;
                }

                base.OnPictureInPictureModeChanged(isInPictureInPictureMode, newConfig);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }

        }

        protected override void OnUserLeaveHint()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var param = new PictureInPictureParams.Builder().SetAspectRatio(new Rational(9, 16)).Build();
                EnterPictureInPictureMode(param);
            }
            base.OnUserLeaveHint();

        }

        public async void Start_Call_Action(string type)
        {
            try
            {
                if (type == "call")
                {
                    NoteTextView.Text = GetText(Resource.String.Lbl_Calling);
                    var apiStartCall = await ApiRequest.Create_Agora_Call_Event_Async(UserId, "video");
                    if (apiStartCall != null)
                    {
                        RoomName = apiStartCall.RoomName;
                        CallId = apiStartCall.Id;
                        Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("mystic_call.mp3");

                        TimerRequestWaiter = new Timer();
                        TimerRequestWaiter.Interval = 5000;
                        TimerRequestWaiter.Elapsed += TimerCallRequestAnswer_Waiter_Elapsed;
                        TimerRequestWaiter.Start();
                    }
                }
                else
                {
                    RoomName = Intent.GetStringExtra("room_name");
                    CallId = Intent.GetStringExtra("CallID");
                    Name = Intent.GetStringExtra("name");
                    Avatar = Intent.GetStringExtra("avatar");

                    NoteTextView.Text = GetText(Resource.String.Lbl_Waiting_to_connect);

                    var apiStartCall = await ApiRequest.Send_Agora_Call_Action_Async("answer", CallId);
                    if (apiStartCall == "200")
                    {
                        var ckd = GlobalContext?.LastCallsTab?.MAdapter?.MCallUser?.FirstOrDefault(a => a.Id == CallId); // id >> Call_Id
                        if (ckd == null)
                        {
                            Classes.CallUser cv = new Classes.CallUser();
                            cv.Id = CallId;
                            cv.UserId = UserId;
                            cv.Avatar = Avatar;
                            cv.Name = Name;
                            cv.FromId = FromId;
                            cv.Active = Active;
                            cv.Time = "Answered call";
                            cv.Status = Status;
                            cv.RoomName = RoomName;
                            cv.Type = CallType;
                            cv.TypeIcon = "Accept";
                            cv.TypeColor = "#008000";

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
                            Classes.CallUser cv = new Classes.CallUser();
                            cv.Id = CallId;
                            cv.UserId = UserId;
                            cv.Avatar = Avatar;
                            cv.Name = Name;
                            cv.FromId = FromId;
                            cv.Active = Active;
                            cv.Time = "Missed call";
                            cv.Status = Status;
                            cv.RoomName = RoomName;
                            cv.Type = CallType;
                            cv.TypeIcon = "Cancel";
                            cv.TypeColor = "#FF0000";

                            GlobalContext?.LastCallsTab?.MAdapter?.Insert(cv);

                            SqLiteDatabase dbDatabase = new SqLiteDatabase();
                            dbDatabase.Insert_CallUser(cv);
                            dbDatabase.Dispose();
                        }
                        NoteTextView.Text = GetText(Resource.String.Lbl_Faild_to_connect);
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
                Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Error.mp3");
                NoteTextView.Text = GetText(Resource.String.Lbl_No_respond_from_the_user);
                await Task.Delay(3000);
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

        public async void OnCall_Declined_From_User()
        {
            try
            {
                Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Error.mp3");

                NoteTextView.Text = GetText(Resource.String.Lbl_The_user_declinde_your_call);
                await Task.Delay(3000);
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

        private async void TimerCallRequestAnswer_Waiter_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var callResultGeneration = await ApiRequest.Check_Agora_Call_Answer_Async(CallId, "video");

                if (string.IsNullOrEmpty(callResultGeneration))
                    return;

                TimerRequestWaiter.Enabled = false;
                TimerRequestWaiter.Stop();
                TimerRequestWaiter.Close();

                RunOnUiThread(() =>
                {
                    Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                    InitAgoraEngineAndJoinChannel(RoomName);
                });

                if (callResultGeneration == "answered")
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

                    var ckd = GlobalContext?.LastCallsTab?.MAdapter?.MCallUser?.FirstOrDefault(a => a.Id == CallId); // id >> Call_Id
                    if (ckd == null)
                    {
                        Classes.CallUser cv = new Classes.CallUser();
                        cv.Id = CallId;
                        cv.UserId = UserId;
                        cv.Avatar = Avatar;
                        cv.Name = Name;
                        cv.FromId = FromId;
                        cv.Active = Active;
                        cv.Time = "Missed call";
                        cv.Status = Status;
                        cv.RoomName = RoomName;
                        cv.Type = CallType;
                        cv.TypeIcon = "Cancel";
                        cv.TypeColor = "#FF0000";

                        GlobalContext?.LastCallsTab?.MAdapter?.Insert(cv);

                        SqLiteDatabase dbDatabase = new SqLiteDatabase();
                        dbDatabase.Insert_CallUser(cv);
                        dbDatabase.Dispose();
                    }
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
                var visibleMutedLayers = MuteAudioButton.Selected ? ViewStates.Visible : ViewStates.Invisible;
                FindViewById(Resource.Id.local_video_overlay).Visibility = visibleMutedLayers;
                FindViewById(Resource.Id.local_video_muted).Visibility = visibleMutedLayers;
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

        public override void OnBackPressed()
        {
            try
            {
                Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
                AgoraEngine.StopPreview();
                AgoraEngine.SetupLocalVideo(null);
                AgoraEngine.LeaveChannel();
                AgoraEngine.Dispose();
                AgoraEngine = null;
                Finish();

                base.OnBackPressed();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                base.OnBackPressed();
            }
        }

        private void Mute_video_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (MuteVideoButton.Selected)
                {
                    MuteVideoButton.Selected = false;
                    MuteVideoButton.SetImageResource(Resource.Drawable.ic_camera_video_open);
                }
                else
                {
                    MuteVideoButton.Selected = true;
                    MuteVideoButton.SetImageResource(Resource.Drawable.ic_camera_video_mute);
                }

                AgoraEngine.MuteLocalVideoStream(MuteVideoButton.Selected);
                IsVideoEnabled = !MuteVideoButton.Selected;
                FindViewById(Resource.Id.local_video_container).Visibility =
                    IsVideoEnabled ? ViewStates.Visible : ViewStates.Gone;
                LocalVideoView.Visibility = IsVideoEnabled ? ViewStates.Visible : ViewStates.Gone;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Switch_cam_button_Click(object sender, EventArgs e)
        {
            AgoraEngine.SwitchCamera();
        }

        public void OnFirstRemoteVideoDecoded(int uid, int width, int height, int elapsed)
        {
            RunOnUiThread(() =>
            {
                SetupRemoteVideo(uid);
                Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
            });
        }

        protected override void OnDestroy()
        {
            try
            {
                //Close Api Starts here >>
                ApiRequest.Send_Agora_Call_Action_Async("close", CallId).ConfigureAwait(false);

                if (AgoraEngine != null)
                {
                    AgoraEngine.StopPreview();
                    AgoraEngine.SetupLocalVideo(null);
                    AgoraEngine.LeaveChannel();
                    AgoraEngine.Dispose();
                    AgoraEngine = null;
                }
                base.OnDestroy();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                base.OnDestroy();
            }
        }

        public void OnUserOffline(int uid, int reason)
        {
            RunOnUiThread(async () =>
            {

                FrameLayout container = (FrameLayout)FindViewById(Resource.Id.remote_video_view_container);
                Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Error.mp3");
                container.RemoveAllViews();
                UserInfoviewContainer.Visibility = ViewStates.Visible;
                NoteTextView.Text = GetText(Resource.String.Lbl_Lost_his_connection);
                await Task.Delay(3000);
                try
                {
                    AgoraEngine.StopPreview();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                try
                {
                    AgoraEngine.SetupLocalVideo(null);
                    AgoraEngine.LeaveChannel();
                    AgoraEngine.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                AgoraEngine = null;

                Finish();
            });
        }

        public void OnUserJoined(int uid, int reason)
        {
            RunOnUiThread(() =>
            {
                UserInfoviewContainer.Visibility = ViewStates.Gone;
                NoteTextView.Text = "";
                Methods.AudioRecorderAndPlayer.StopAudioFromAsset();
            });
        }

        public void OnUserMuteVideo(int uid, bool muted)
        {
            RunOnUiThread(() =>
            {
                UserInfoviewContainer.Visibility = ViewStates.Visible;
                OnRemoteUserVideoMuted(uid, muted);
            });
        }

        public void OnConnectionLost()
        {
            RunOnUiThread(() =>
            {
                Toast.MakeText(this, GetText(Resource.String.Lbl_Lost_Connection), ToastLength.Short).Show();
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

        public void OnFirstLocalVideoFrame(float height, float width, int p2)
        {
            try
            {
                var ratio = height / width;
                var ratioHeight = ratio * MaxLocalVideoDimension;
                var ratioWidth = MaxLocalVideoDimension / ratio;
                var containerHeight = height > width ? MaxLocalVideoDimension : ratioHeight;
                var containerWidth = height > width ? ratioWidth : MaxLocalVideoDimension;
                RunOnUiThread(() =>
                {
                    var videoContainer = FindViewById<RelativeLayout>(Resource.Id.local_video_container);
                    var parameters = videoContainer.LayoutParameters;
                    parameters.Height = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, containerHeight,
                        Resources.DisplayMetrics);
                    parameters.Width = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, containerWidth,
                        Resources.DisplayMetrics);
                    videoContainer.LayoutParameters = parameters;
                    FindViewById(Resource.Id.local_video_container).Visibility =
                        IsVideoEnabled ? ViewStates.Visible : ViewStates.Invisible;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetupRemoteVideo(int uid)
        {
            try
            {
                FrameLayout container = (FrameLayout)FindViewById(Resource.Id.remote_video_view_container);
                if (container.ChildCount >= 1)
                {
                    return;
                }
                SurfaceView surfaceView = RtcEngine.CreateRendererView(BaseContext);
                container.AddView(surfaceView);

                //AgoraEngine.SetupRemoteVideo(new VideoCanvas(surfaceView, VideoCanvas.RenderModeAdaptive, uid)); >> Old
                AgoraEngine.SetupRemoteVideo(new VideoCanvas(surfaceView, VideoCanvas.RenderModeHidden, uid));
                surfaceView.Tag = uid; // for mark purpose
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitAgoraEngineAndJoinChannel(string roomName)
        {
            try
            {
                InitializeAgoraEngine();
                AgoraEngine.EnableVideo();
                AgoraEngine.SetVideoProfile(AgoraSettings.VideoQuality, false);
                SetupLocalVideo();
                AgoraEngine.JoinChannel(null, roomName, string.Empty, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetupLocalVideo()
        {
            try
            {
                FrameLayout container = (FrameLayout)FindViewById(Resource.Id.local_video_view_container);
                LocalVideoView = RtcEngine.CreateRendererView(BaseContext);
                LocalVideoView.SetZOrderMediaOverlay(true);
                container.AddView(LocalVideoView);
                //AgoraEngine.SetupLocalVideo(new VideoCanvas(LocalVideoView, VideoCanvas.RenderModeAdaptive, 0)); >> Old
                AgoraEngine.SetupLocalVideo(new VideoCanvas(LocalVideoView, VideoCanvas.RenderModeHidden, 0));
                if (!string.IsNullOrEmpty(""))
                {
                    AgoraEngine.SetEncryptionMode("aes-128-xts");
                    AgoraEngine.SetEncryptionSecret("");
                }
                AgoraEngine.StartPreview();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitializeAgoraEngine()
        {
            try
            {
                AgoraHandler = new AgoraRtcHandler(this);
                AgoraEngine = RtcEngine.Create(BaseContext, AgoraSettings.AgoraApi, AgoraHandler);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnRemoteUserVideoMuted(int uid, bool muted)
        {

            FrameLayout container = (FrameLayout)FindViewById(Resource.Id.remote_video_view_container);
            SurfaceView surfaceView = (SurfaceView)container.GetChildAt(0);
            var tag = surfaceView.Tag;
            if (tag != null && (int)tag == uid)
            {
                NoteTextView.Text = GetText(Resource.String.Lbl_Muted_his_video);
                surfaceView.Visibility = muted ? ViewStates.Gone : ViewStates.Visible;
                if (muted)
                {
                    UserInfoviewContainer.Visibility = ViewStates.Visible;
                    NoteTextView.Text = GetText(Resource.String.Lbl_Muted_his_video);
                }
                else
                {
                    UserInfoviewContainer.Visibility = ViewStates.Gone;
                    NoteTextView.Text = "";
                }
            }
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