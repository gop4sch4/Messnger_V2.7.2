using Android.App;
using Android.Content;
using Android.OS;
using Java.Lang;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using WoWonder.Frameworks;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using WoWonderClient;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Message;
using WoWonderClient.Requests;
using Exception = System.Exception;

namespace WoWonder.Activities.Tab.Services
{
    [Service]
    public class ScheduledApiService : Service
    {
        private readonly Handler MainHandler = new Handler();
        private ResultReceiver ResultSender;
        private PostUpdaterHelper postUpdater;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnCreate()
        {
            try
            {
                base.OnCreate();
                postUpdater = new PostUpdaterHelper(Application.Context, new Handler(), ResultSender);
                MainHandler.PostDelayed(postUpdater, AppSettings.RefreshChatActivitiesSeconds);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            base.OnStartCommand(intent, flags, startId);
            try
            {
                var rec = intent.GetParcelableExtra("receiverTag");
                ResultSender = (ResultReceiver)rec;
                if (postUpdater != null)
                    postUpdater.ResultSender = ResultSender;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //MainHandler.PostDelayed(new PostUpdaterHelper(Application.Context, new Handler(), ResultSender), AppSettings.RefreshChatActivitiesSeconds);

            return StartCommandResult.Sticky;
        }
    }

    public class PostUpdaterHelper : Java.Lang.Object, IRunnable
    {
        private static Handler MainHandler;
        private static Context Activity;
        public ResultReceiver ResultSender;
        private int SecondPassed;

        public PostUpdaterHelper(Context activity, Handler mainHandler, ResultReceiver resultSender)
        {
            MainHandler = mainHandler;
            Activity = activity;
            ResultSender = resultSender;
        }

        public async void Run()
        {
            if (Methods.AppLifecycleObserver.AppState == 0)
                Methods.AppLifecycleObserver.AppState = Methods.AppLifecycleObserver.AppLifeState.Background;

            if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
            {
                try
                {
                    //Toast.MakeText(Application.Context, "Started", ToastLength.Short).Show(); 
                    if (Methods.AppLifecycleObserver.AppState == Methods.AppLifecycleObserver.AppLifeState.Background)
                    {
                        try
                        {
                            SqLiteDatabase dbDatabase = new SqLiteDatabase();
                            var login = dbDatabase.Get_data_Login_Credentials();
                            Console.WriteLine(login);

                            if (string.IsNullOrEmpty(Current.AccessToken))
                                return;

                            (int apiStatus, var respond) = await RequestsAsync.Global.GetChatAsync("all");
                            dbDatabase.Dispose();
                            if (apiStatus != 200 || !(respond is LastChatObject result))
                            {
                                // Methods.DisplayReportResult(Activity, respond);
                            }
                            else
                            {
                                //Toast.MakeText(Application.Context, "ResultSender 1 \n" + data, ToastLength.Short).Show();

                                #region Call >> Video_Call_User

                                try
                                {
                                    if (AppSettings.EnableAudioVideoCall)
                                    {
                                        if (AppSettings.UseTwilioLibrary)
                                        {
                                            bool twilioVideoCall = result.VideoCall ?? false;
                                            bool twilioAudioCall = result.AudioCall ?? false;

                                            if (AppSettings.EnableVideoCall)
                                            {
                                                #region Twilio Video call

                                                if (twilioVideoCall && SecondPassed >= 5 && !TabbedMainActivity.RunCall)
                                                {
                                                    var callUser = result.VideoCallUser?.CallUserClass;
                                                    if (callUser != null)
                                                    {
                                                        TabbedMainActivity.RunCall = true;

                                                        var userId = callUser.UserId;
                                                        var avatar = callUser.Avatar;
                                                        var name = callUser.Name;

                                                        var videosData = callUser.Data;
                                                        if (videosData != null)
                                                        {
                                                            var id = videosData.Id; //call_id
                                                            var accessToken = videosData.AccessToken;
                                                            var accessToken2 = videosData.AccessToken2;
                                                            var fromId = videosData.FromId;
                                                            var active = videosData.Active;
                                                            var time = videosData.Called;
                                                            var declined = videosData.Declined;
                                                            var roomName = videosData.RoomName;

                                                            Intent intent = new Intent(Activity, typeof(VideoAudioComingCallActivity));
                                                            intent.PutExtra("UserID", userId);
                                                            intent.PutExtra("avatar", avatar);
                                                            intent.PutExtra("name", name);
                                                            intent.PutExtra("access_token", accessToken);
                                                            intent.PutExtra("access_token_2", accessToken2);
                                                            intent.PutExtra("from_id", fromId);
                                                            intent.PutExtra("active", active);
                                                            intent.PutExtra("time", time);
                                                            intent.PutExtra("CallID", id);
                                                            intent.PutExtra("status", declined);
                                                            intent.PutExtra("room_name", roomName);
                                                            intent.PutExtra("declined", declined);
                                                            intent.PutExtra("type", "Twilio_video_call");

                                                            string avatarSplit = avatar.Split('/').Last();
                                                            var getImg = Methods.MultiMedia.GetMediaFrom_Disk(Methods.Path.FolderDiskImage, avatarSplit);
                                                            if (getImg == "File Dont Exists")
                                                                Methods.MultiMedia.DownloadMediaTo_DiskAsync(Methods.Path.FolderDiskImage, avatar);

                                                            if (SecondPassed < 5)
                                                                SecondPassed++;
                                                            else
                                                            {
                                                                TabbedMainActivity.RunCall = false;
                                                                SecondPassed = 0;

                                                                if (!VideoAudioComingCallActivity.IsActive)
                                                                {
                                                                    intent.AddFlags(ActivityFlags.NewTask);
                                                                    Activity.StartActivity(intent);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (twilioVideoCall == false && twilioAudioCall == false)
                                                {
                                                    if (SecondPassed > 5)
                                                    {
                                                        TabbedMainActivity.RunCall = false;

                                                        SecondPassed = 0;

                                                        if (VideoAudioComingCallActivity.IsActive)
                                                            VideoAudioComingCallActivity.CallActivity?.Finish();
                                                    }
                                                }
                                                else
                                                {
                                                    TabbedMainActivity.RunCall = false;
                                                }

                                                #endregion
                                            }

                                            if (AppSettings.EnableAudioCall)
                                            {
                                                #region Twilio Audio call

                                                if (twilioAudioCall && !TabbedMainActivity.RunCall)
                                                {
                                                    var callUser = result.AudioCallUser?.CallUserClass;
                                                    if (callUser != null)
                                                    {
                                                        TabbedMainActivity.RunCall = true;

                                                        var userId = callUser.UserId;
                                                        var avatar = callUser.Avatar;
                                                        var name = callUser.Name;

                                                        var videosData = callUser.Data;
                                                        if (videosData != null)
                                                        {
                                                            var id = videosData.Id; //call_id
                                                            var accessToken = videosData.AccessToken;
                                                            var accessToken2 = videosData.AccessToken2;
                                                            var fromId = videosData.FromId;
                                                            var active = videosData.Active;
                                                            var time = videosData.Called;
                                                            var declined = videosData.Declined;
                                                            var roomName = videosData.RoomName;

                                                            Intent intent = new Intent(Activity, typeof(VideoAudioComingCallActivity));
                                                            intent.PutExtra("UserID", userId);
                                                            intent.PutExtra("avatar", avatar);
                                                            intent.PutExtra("name", name);
                                                            intent.PutExtra("access_token", accessToken);
                                                            intent.PutExtra("access_token_2", accessToken2);
                                                            intent.PutExtra("from_id", fromId);
                                                            intent.PutExtra("active", active);
                                                            intent.PutExtra("time", time);
                                                            intent.PutExtra("CallID", id);
                                                            intent.PutExtra("status", declined);
                                                            intent.PutExtra("room_name", roomName);
                                                            intent.PutExtra("declined", declined);
                                                            intent.PutExtra("type", "Twilio_audio_call");

                                                            string avatarSplit = avatar.Split('/').Last();
                                                            var getImg =
                                                                Methods.MultiMedia.GetMediaFrom_Disk(Methods.Path.FolderDiskImage,
                                                                    avatarSplit);
                                                            if (getImg == "File Dont Exists")
                                                                Methods.MultiMedia.DownloadMediaTo_DiskAsync(
                                                                    Methods.Path.FolderDiskImage, avatar);

                                                            if (SecondPassed < 5)
                                                                SecondPassed++;
                                                            else
                                                            {
                                                                TabbedMainActivity.RunCall = false;
                                                                SecondPassed = 0;

                                                                if (!VideoAudioComingCallActivity.IsActive)
                                                                {
                                                                    intent.AddFlags(ActivityFlags.NewTask);
                                                                    Activity.StartActivity(intent);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (twilioAudioCall == false && twilioVideoCall == false)
                                                {
                                                    if (SecondPassed >= 5)
                                                    {
                                                        TabbedMainActivity.RunCall = false;

                                                        if (VideoAudioComingCallActivity.IsActive)
                                                            VideoAudioComingCallActivity.CallActivity?.Finish();
                                                    }
                                                }
                                                else
                                                {
                                                    TabbedMainActivity.RunCall = false;
                                                }

                                                #endregion
                                            }
                                        }
                                        else if (AppSettings.UseAgoraLibrary)
                                        {
                                            #region Agora Audio/Video call

                                            var agoraCall = result.AgoraCall ?? false;
                                            if (agoraCall && SecondPassed >= 5 && !TabbedMainActivity.RunCall)
                                            {
                                                var callUser = result.AgoraCallData?.CallUserClass;

                                                if (callUser != null)
                                                {
                                                    TabbedMainActivity.RunCall = true;

                                                    var userId = callUser.UserId;
                                                    var avatar = callUser.Avatar;
                                                    var name = callUser.Name;

                                                    var videosData = callUser.Data;
                                                    if (videosData != null)
                                                    {
                                                        var id = videosData.Id; //call_id
                                                                                //var accessToken = videosData.AccessToken;
                                                                                //var accessToken2 = videosData.AccessToken2;
                                                        var fromId = videosData.FromId;
                                                        //var active = videosData.Active;
                                                        var time = videosData.Called;
                                                        //var declined = videosData.Declined;
                                                        var roomName = videosData.RoomName;
                                                        var type = videosData.Type;
                                                        var status = videosData.Status;

                                                        string avatarSplit = avatar.Split('/').Last();
                                                        var getImg = Methods.MultiMedia.GetMediaFrom_Disk(Methods.Path.FolderDiskImage, avatarSplit);
                                                        if (getImg == "File Dont Exists")
                                                            Methods.MultiMedia.DownloadMediaTo_DiskAsync(Methods.Path.FolderDiskImage, avatar);

                                                        if (type == "video")
                                                        {
                                                            if (AppSettings.EnableVideoCall)
                                                            {
                                                                Intent intent = new Intent(Activity, typeof(VideoAudioComingCallActivity));
                                                                intent.PutExtra("UserID", userId);
                                                                intent.PutExtra("avatar", avatar);
                                                                intent.PutExtra("name", name);
                                                                intent.PutExtra("from_id", fromId);
                                                                intent.PutExtra("status", status);
                                                                intent.PutExtra("time", time);
                                                                intent.PutExtra("CallID", id);
                                                                intent.PutExtra("room_name", roomName);
                                                                intent.PutExtra("type", "Agora_video_call_recieve");
                                                                intent.PutExtra("declined", "0");

                                                                if (!VideoAudioComingCallActivity.IsActive)
                                                                {
                                                                    intent.AddFlags(ActivityFlags.NewTask);
                                                                    Activity.StartActivity(intent);
                                                                }
                                                            }
                                                        }
                                                        else if (type == "audio")
                                                        {
                                                            if (AppSettings.EnableAudioCall)
                                                            {
                                                                Intent intent = new Intent(Activity, typeof(VideoAudioComingCallActivity));
                                                                intent.PutExtra("UserID", userId);
                                                                intent.PutExtra("avatar", avatar);
                                                                intent.PutExtra("name", name);
                                                                intent.PutExtra("from_id", fromId);
                                                                intent.PutExtra("status", status);
                                                                intent.PutExtra("time", time);
                                                                intent.PutExtra("CallID", id);
                                                                intent.PutExtra("room_name", roomName);
                                                                intent.PutExtra("type", "Agora_audio_call_recieve");
                                                                intent.PutExtra("declined", "0");

                                                                if (SecondPassed < 5)
                                                                    SecondPassed++;
                                                                else
                                                                {
                                                                    TabbedMainActivity.RunCall = false;

                                                                    SecondPassed = 0;


                                                                    if (!VideoAudioComingCallActivity.IsActive)
                                                                    {
                                                                        intent.AddFlags(ActivityFlags.NewTask);
                                                                        Activity.StartActivity(intent);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else if (agoraCall == false)
                                            {
                                                if (SecondPassed >= 5)
                                                {
                                                    TabbedMainActivity.RunCall = false;

                                                    SecondPassed = 0;

                                                    if (VideoAudioComingCallActivity.IsActive)
                                                        VideoAudioComingCallActivity.CallActivity?.Finish();

                                                }
                                            }

                                            #endregion
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }

                                #endregion

                                if (result.Data.Count > 0)
                                {
                                    ListUtils.UserList = new ObservableCollection<ChatObject>(result.Data);

                                    //Insert All data users to database
                                    dbDatabase.Insert_Or_Update_LastUsersChat(Activity, ListUtils.UserList, true);
                                    dbDatabase.Dispose();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            // Toast.MakeText(Application.Context, "Exception  " + e, ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        (int apiStatus, var respond) = await RequestsAsync.Global.GetChatAsync("all");
                        if (apiStatus != 200 || !(respond is LastChatObject result))
                        {
                            if (respond is ErrorObject errorMessage)
                            {
                                string errorText = errorMessage._errors.ErrorText;

                                if (errorText.Contains("Invalid or expired access_token") || errorText.Contains("No session sent"))
                                    ApiRequest.Logout(TabbedMainActivity.GetInstance());
                            }
                        }
                        else
                        {
                            var b = new Bundle();
                            b.PutString("Json", JsonConvert.SerializeObject(result));
                            ResultSender?.Send(0, b);

                            //Toast.MakeText(Application.Context, "ResultSender 2 \n" + data, ToastLength.Short).Show();

                            Console.WriteLine("Allen Post + started");
                        }
                    }

                    MainHandler.PostDelayed(new PostUpdaterHelper(Activity, MainHandler, ResultSender), AppSettings.RefreshChatActivitiesSeconds);
                }
                catch (Exception e)
                {
                    //Toast.MakeText(Application.Context, "ResultSender failed", ToastLength.Short).Show();
                    MainHandler.PostDelayed(new PostUpdaterHelper(Activity, MainHandler, ResultSender), AppSettings.RefreshChatActivitiesSeconds);
                    Console.WriteLine(e);
                    Console.WriteLine("Allen Post + failed");
                }
            }
            else
            {
                try
                {
                    //Toast.MakeText(Application.Context, "Started \n" + Methods.AppLifecycleObserver.AppState + "\n" + ResultSender, ToastLength.Short).Show(); 
                    if (Methods.AppLifecycleObserver.AppState == Methods.AppLifecycleObserver.AppLifeState.Background)
                    {
                        try
                        {
                            SqLiteDatabase dbDatabase = new SqLiteDatabase();
                            var login = dbDatabase.Get_data_Login_Credentials();
                            Console.WriteLine(login);

                            if (string.IsNullOrEmpty(Current.AccessToken))
                                return;

                            (int apiStatus, var respond) = await RequestsAsync.Message.Get_users_list_Async(UserDetails.UserId, UserDetails.UserId, "35", "0", false);
                            dbDatabase.Dispose();
                            if (apiStatus != 200 || !(respond is GetUsersListObject result))
                            {
                                // Methods.DisplayReportResult(Activity, respond);
                            }
                            else
                            {
                                //Toast.MakeText(Application.Context, "ResultSender 1 \n", ToastLength.Short).Show();

                                #region Call >> Video_Call_User

                                try
                                {
                                    if (AppSettings.EnableAudioVideoCall)
                                    {
                                        if (AppSettings.UseTwilioLibrary)
                                        {
                                            bool twilioVideoCall = result.VideoCall ?? false;
                                            bool twilioAudioCall = result.AudioCall ?? false;

                                            if (AppSettings.EnableVideoCall)
                                            {
                                                #region Twilio Video call

                                                if (twilioVideoCall && SecondPassed >= 5 && !TabbedMainActivity.RunCall)
                                                {
                                                    var callUser = result.VideoCallUser?.CallUserClass;
                                                    if (callUser != null)
                                                    {
                                                        TabbedMainActivity.RunCall = true;

                                                        var userId = callUser.UserId;
                                                        var avatar = callUser.Avatar;
                                                        var name = callUser.Name;

                                                        var videosData = callUser.data;
                                                        if (videosData != null)
                                                        {
                                                            var id = videosData.Id; //call_id
                                                            var accessToken = videosData.AccessToken;
                                                            var accessToken2 = videosData.AccessToken2;
                                                            var fromId = videosData.FromId;
                                                            var active = videosData.Active;
                                                            var time = videosData.Called;
                                                            var declined = videosData.Declined;
                                                            var roomName = videosData.RoomName;

                                                            Intent intent = new Intent(Activity, typeof(VideoAudioComingCallActivity));
                                                            intent.PutExtra("UserID", userId);
                                                            intent.PutExtra("avatar", avatar);
                                                            intent.PutExtra("name", name);
                                                            intent.PutExtra("access_token", accessToken);
                                                            intent.PutExtra("access_token_2", accessToken2);
                                                            intent.PutExtra("from_id", fromId);
                                                            intent.PutExtra("active", active);
                                                            intent.PutExtra("time", time);
                                                            intent.PutExtra("CallID", id);
                                                            intent.PutExtra("status", declined);
                                                            intent.PutExtra("room_name", roomName);
                                                            intent.PutExtra("declined", declined);
                                                            intent.PutExtra("type", "Twilio_video_call");

                                                            string avatarSplit = avatar.Split('/').Last();
                                                            var getImg = Methods.MultiMedia.GetMediaFrom_Disk(Methods.Path.FolderDiskImage, avatarSplit);
                                                            if (getImg == "File Dont Exists")
                                                                Methods.MultiMedia.DownloadMediaTo_DiskAsync(Methods.Path.FolderDiskImage, avatar);

                                                            if (SecondPassed < 5)
                                                                SecondPassed++;
                                                            else
                                                            {
                                                                TabbedMainActivity.RunCall = false;
                                                                SecondPassed = 0;

                                                                if (!VideoAudioComingCallActivity.IsActive)
                                                                {
                                                                    intent.AddFlags(ActivityFlags.NewTask);
                                                                    Activity.StartActivity(intent);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (twilioVideoCall == false && twilioAudioCall == false)
                                                {
                                                    if (SecondPassed > 5)
                                                    {
                                                        TabbedMainActivity.RunCall = false;

                                                        SecondPassed = 0;

                                                        if (VideoAudioComingCallActivity.IsActive)
                                                            VideoAudioComingCallActivity.CallActivity?.Finish();
                                                    }
                                                }
                                                else
                                                {
                                                    TabbedMainActivity.RunCall = false;
                                                }

                                                #endregion
                                            }

                                            if (AppSettings.EnableAudioCall)
                                            {
                                                #region Twilio Audio call

                                                if (twilioAudioCall && !TabbedMainActivity.RunCall)
                                                {
                                                    var callUser = result.AudioCallUser?.CallUserClass;
                                                    if (callUser != null)
                                                    {
                                                        TabbedMainActivity.RunCall = true;

                                                        var userId = callUser.UserId;
                                                        var avatar = callUser.Avatar;
                                                        var name = callUser.Name;

                                                        var videosData = callUser.data;
                                                        if (videosData != null)
                                                        {
                                                            var id = videosData.Id; //call_id
                                                            var accessToken = videosData.AccessToken;
                                                            var accessToken2 = videosData.AccessToken2;
                                                            var fromId = videosData.FromId;
                                                            var active = videosData.Active;
                                                            var time = videosData.Called;
                                                            var declined = videosData.Declined;
                                                            var roomName = videosData.RoomName;

                                                            Intent intent = new Intent(Activity, typeof(VideoAudioComingCallActivity));
                                                            intent.PutExtra("UserID", userId);
                                                            intent.PutExtra("avatar", avatar);
                                                            intent.PutExtra("name", name);
                                                            intent.PutExtra("access_token", accessToken);
                                                            intent.PutExtra("access_token_2", accessToken2);
                                                            intent.PutExtra("from_id", fromId);
                                                            intent.PutExtra("active", active);
                                                            intent.PutExtra("time", time);
                                                            intent.PutExtra("CallID", id);
                                                            intent.PutExtra("status", declined);
                                                            intent.PutExtra("room_name", roomName);
                                                            intent.PutExtra("declined", declined);
                                                            intent.PutExtra("type", "Twilio_audio_call");

                                                            string avatarSplit = avatar.Split('/').Last();
                                                            var getImg =
                                                                Methods.MultiMedia.GetMediaFrom_Disk(Methods.Path.FolderDiskImage,
                                                                    avatarSplit);
                                                            if (getImg == "File Dont Exists")
                                                                Methods.MultiMedia.DownloadMediaTo_DiskAsync(
                                                                    Methods.Path.FolderDiskImage, avatar);

                                                            if (SecondPassed < 5)
                                                                SecondPassed++;
                                                            else
                                                            {
                                                                TabbedMainActivity.RunCall = false;
                                                                SecondPassed = 0;

                                                                if (!VideoAudioComingCallActivity.IsActive)
                                                                {
                                                                    intent.AddFlags(ActivityFlags.NewTask);
                                                                    Activity.StartActivity(intent);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else if (twilioAudioCall == false && twilioVideoCall == false)
                                                {
                                                    if (SecondPassed >= 5)
                                                    {
                                                        TabbedMainActivity.RunCall = false;

                                                        if (VideoAudioComingCallActivity.IsActive)
                                                            VideoAudioComingCallActivity.CallActivity?.Finish();
                                                    }
                                                }
                                                else
                                                {
                                                    TabbedMainActivity.RunCall = false;
                                                }

                                                #endregion
                                            }
                                        }
                                        else if (AppSettings.UseAgoraLibrary)
                                        {
                                            #region Agora Audio/Video call

                                            var agoraCall = result.AgoraCall ?? false;
                                            if (agoraCall && SecondPassed >= 5 && !TabbedMainActivity.RunCall)
                                            {
                                                var callUser = result.AgoraCallData?.CallUserClass;

                                                if (callUser != null)
                                                {
                                                    TabbedMainActivity.RunCall = true;

                                                    var userId = callUser.UserId;
                                                    var avatar = callUser.Avatar;
                                                    var name = callUser.Name;

                                                    var videosData = callUser.data;
                                                    if (videosData != null)
                                                    {
                                                        var id = videosData.Id; //call_id
                                                                                //var accessToken = videosData.AccessToken;
                                                                                //var accessToken2 = videosData.AccessToken2;
                                                        var fromId = videosData.FromId;
                                                        //var active = videosData.Active;
                                                        var time = videosData.Called;
                                                        //var declined = videosData.Declined;
                                                        var roomName = videosData.RoomName;
                                                        var type = videosData.Type;
                                                        var status = videosData.Status;

                                                        string avatarSplit = avatar.Split('/').Last();
                                                        var getImg = Methods.MultiMedia.GetMediaFrom_Disk(Methods.Path.FolderDiskImage, avatarSplit);
                                                        if (getImg == "File Dont Exists")
                                                            Methods.MultiMedia.DownloadMediaTo_DiskAsync(Methods.Path.FolderDiskImage, avatar);

                                                        if (type == "video")
                                                        {
                                                            if (AppSettings.EnableVideoCall)
                                                            {
                                                                Intent intent = new Intent(Activity, typeof(VideoAudioComingCallActivity));
                                                                intent.PutExtra("UserID", userId);
                                                                intent.PutExtra("avatar", avatar);
                                                                intent.PutExtra("name", name);
                                                                intent.PutExtra("from_id", fromId);
                                                                intent.PutExtra("status", status);
                                                                intent.PutExtra("time", time);
                                                                intent.PutExtra("CallID", id);
                                                                intent.PutExtra("room_name", roomName);
                                                                intent.PutExtra("type", "Agora_video_call_recieve");
                                                                intent.PutExtra("declined", "0");

                                                                if (!VideoAudioComingCallActivity.IsActive)
                                                                {
                                                                    intent.AddFlags(ActivityFlags.NewTask);
                                                                    Activity.StartActivity(intent);
                                                                }
                                                            }
                                                        }
                                                        else if (type == "audio")
                                                        {
                                                            if (AppSettings.EnableAudioCall)
                                                            {
                                                                Intent intent = new Intent(Activity, typeof(VideoAudioComingCallActivity));
                                                                intent.PutExtra("UserID", userId);
                                                                intent.PutExtra("avatar", avatar);
                                                                intent.PutExtra("name", name);
                                                                intent.PutExtra("from_id", fromId);
                                                                intent.PutExtra("status", status);
                                                                intent.PutExtra("time", time);
                                                                intent.PutExtra("CallID", id);
                                                                intent.PutExtra("room_name", roomName);
                                                                intent.PutExtra("type", "Agora_audio_call_recieve");
                                                                intent.PutExtra("declined", "0");

                                                                if (SecondPassed < 5)
                                                                    SecondPassed++;
                                                                else
                                                                {
                                                                    TabbedMainActivity.RunCall = false;

                                                                    SecondPassed = 0;


                                                                    if (!VideoAudioComingCallActivity.IsActive)
                                                                    {
                                                                        intent.AddFlags(ActivityFlags.NewTask);
                                                                        Activity.StartActivity(intent);
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else if (agoraCall == false)
                                            {
                                                if (SecondPassed >= 5)
                                                {
                                                    TabbedMainActivity.RunCall = false;

                                                    SecondPassed = 0;

                                                    if (VideoAudioComingCallActivity.IsActive)
                                                        VideoAudioComingCallActivity.CallActivity?.Finish();

                                                }
                                            }

                                            #endregion
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }

                                #endregion

                                if (result.Users.Count > 0)
                                {
                                    ListUtils.UserChatList = new ObservableCollection<GetUsersListObject.User>(result.Users);

                                    //Insert All data users to database
                                    dbDatabase.Insert_Or_Update_LastUsersChat(Activity, ListUtils.UserChatList, true);
                                    dbDatabase.Dispose();

                                    //Toast.MakeText(Application.Context, "ResultSender 3 \n", ToastLength.Short).Show();
                                }
                            }

                            //Toast.MakeText(Application.Context, "ResultSender wael", ToastLength.Short).Show();

                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            //Toast.MakeText(Application.Context, "Exception  " + e, ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        (int apiStatus, var respond) = await RequestsAsync.Message.Get_users_list_Async(UserDetails.UserId, UserDetails.UserId);
                        if (apiStatus != 200 || !(respond is GetUsersListObject result))
                        {
                            if (respond is ErrorObject errorMessage)
                            {
                                string errorText = errorMessage._errors.ErrorText;

                                if (errorText.Contains("Invalid or expired access_token") || errorText.Contains("No session sent"))
                                    ApiRequest.Logout(TabbedMainActivity.GetInstance());
                            }
                        }
                        else
                        {
                            var b = new Bundle();
                            b.PutString("Json", JsonConvert.SerializeObject(result));
                            ResultSender?.Send(0, b);

                            //Toast.MakeText(Application.Context, "ResultSender 2 \n", ToastLength.Short).Show();

                            Console.WriteLine("Allen Post + started");
                        }
                    }

                    MainHandler.PostDelayed(new PostUpdaterHelper(Activity, MainHandler, ResultSender), AppSettings.RefreshChatActivitiesSeconds);
                }
                catch (Exception e)
                {
                    //Toast.MakeText(Application.Context, "ResultSender failed", ToastLength.Short).Show();
                    MainHandler.PostDelayed(new PostUpdaterHelper(Activity, MainHandler, ResultSender), AppSettings.RefreshChatActivitiesSeconds);
                    Console.WriteLine(e);
                    Console.WriteLine("Allen Post + failed");
                }
            }
        }
    }
}