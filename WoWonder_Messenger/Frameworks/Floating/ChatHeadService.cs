using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using FloatingView.Interfaces;
using FloatingView.Lib;
using Java.Lang;
using Newtonsoft.Json;
using Q.Rorbin.Badgeview;
using System;
using System.Linq;
using WoWonder.Activities.ChatWindow;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Activities.Tab;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Message;
using WoWonderClient.Classes.Posts;
using Exception = System.Exception;
using String = System.String;

namespace WoWonder.Frameworks.Floating
{
    [Service(Exported = false)]
    public class ChatHeadService : IntentService, IFloatingViewListener, View.IOnClickListener
    {
        public static string ExtraCutoutSafeArea = "cutout_safe_area";
        private const int NotificationId = 9080;

        private FloatingViewManager MFloatingViewManager;
        private FloatingObject DataUser;
        private ImageView Image;
        private LayoutInflater Inflater;

        public static bool RunService;

        public override void OnCreate()
        {
            try
            {
                base.OnCreate();

                CreateNotification(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        protected override void OnHandleIntent(Intent intent)
        {

        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            try
            {
                if (MFloatingViewManager == null)
                {
                    RunService = true;

                    DisplayMetrics metrics = new DisplayMetrics();
                    //DisplayMetrics metrics = Resources.DisplayMetrics;
                    var windowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();
                    windowManager?.DefaultDisplay.GetMetrics(metrics);

                    if (Inflater == null)
                        Inflater = LayoutInflater.From(Application.Context);

                    var iconView = Inflater.Inflate(Resource.Layout.WidgetChatHeadLayout, null, false);

                    Image = iconView.FindViewById<ImageView>(Resource.Id.imageFloating);
                    Image.SetOnClickListener(this);
                    try
                    {
                        DataUser = JsonConvert.DeserializeObject<FloatingObject>(intent.GetStringExtra("UserData"));
                        if (DataUser != null)
                        {
                            Glide.With(Application.Context).Load(DataUser.Avatar).Apply(new RequestOptions().CircleCrop().Placeholder(Resource.Drawable.no_profile_image_circle)).Into(Image);

                            if (!string.IsNullOrEmpty(DataUser.MessageCount) || DataUser.MessageCount != "0")
                                ShowOrHideBadgeView(Convert.ToInt32(DataUser.MessageCount), true);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    MFloatingViewManager = new FloatingViewManager(Application.Context, this);
                    MFloatingViewManager.SetFixedTrashIconImage(Application.Context.GetDrawable(Resource.Drawable.ic_trash_fixed));
                    MFloatingViewManager.SetActionTrashIconImage(Application.Context.GetDrawable(Resource.Drawable.ic_trash_action));
                    MFloatingViewManager.SetSafeInsetRect((Rect)intent.GetParcelableExtra(ExtraCutoutSafeArea));

                    // Setting Options(you can change options at any time)
                    FloatingViewManager.Options options = LoadDynamicOptions(metrics);

                    switch (AppSettings.DisplayModeSettings)
                    {
                        case "Always":
                            MFloatingViewManager.SetDisplayMode(FloatingViewManager.DisplayModeShowAlways);
                            break;
                        case "FullScreen":
                            MFloatingViewManager.SetDisplayMode(FloatingViewManager.DisplayModeHideFullscreen);
                            break;
                        case "Hide":
                            MFloatingViewManager.SetDisplayMode(FloatingViewManager.DisplayModeHideAlways);
                            break;
                    }

                    MFloatingViewManager.AddViewToWindow(iconView, options);
                    return StartCommandResult.Sticky;
                }
                else
                {
                    if (Image != null)
                    {
                        if (!string.IsNullOrEmpty(DataUser.MessageCount) || DataUser.MessageCount != "0")
                        {
                            var x = Convert.ToInt32(DataUser.MessageCount);

                            if (x >= 1 && AppSettings.LastChatSystem == SystemApiGetLastChat.Old)
                                x++;

                            ShowOrHideBadgeView(x, true);
                        }
                    }
                }

                return StartCommandResult.NotSticky;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StartCommandResult.NotSticky;
            }
        }

        public override void OnDestroy()
        {
            try
            {
                Destroy();
                base.OnDestroy();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private QBadgeView Badge;
        private void ShowOrHideBadgeView(int countMessages = 0, bool show = false)
        {
            try
            {
                if (show)
                {
                    if (Image != null && Badge == null)
                    {
                        var gravity = (int)(GravityFlags.Start | GravityFlags.Top);
                        Badge = new QBadgeView(Application.Context);
                        Badge.BindTarget(Image);
                        Badge.SetBadgeNumber(countMessages);
                        Badge.SetBadgeGravity(gravity);
                        Badge.SetBadgeBackgroundColor(Color.ParseColor(AppSettings.MainColor));
                        //Badge.SetBadgeTextSize(13, true);
                    }
                    else
                    {
                        Badge?.SetBadgeNumber(countMessages);
                    }
                }
                else
                {
                    Badge?.BindTarget(Image).Hide(true);
                    Badge = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnClick(View v)
        {
            try
            {
                if (v.Id == Image.Id)
                {
                    IconView_Click();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void IconView_Click()
        {
            try
            {
                if (DataUser == null)
                    return;

                try
                {
                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                    {
                        var item = TabbedMainActivity.GetInstance()?.LastChatTab?.MAdapter?.ChatList?.FirstOrDefault(a => a.UserId == DataUser.UserId);
                        if (item != null && item.LastMessage.LastMessageClass.ToId == UserDetails.UserId && item.LastMessage.LastMessageClass.FromId != UserDetails.UserId)
                        {
                            item.LastMessage.LastMessageClass.Seen = "1";
                            item.LastMessage.LastMessageClass.MessageCount = "0";
                            TabbedMainActivity.GetInstance()?.LastChatTab?.MAdapter?.NotifyDataSetChanged();
                        }
                    }
                    else
                    {
                        var item = TabbedMainActivity.GetInstance()?.LastMessagesTab?.MAdapter?.MLastMessagesUser?.FirstOrDefault(a => a.UserId == DataUser.UserId);
                        if (item != null && item.LastMessage.ToId == UserDetails.UserId && item.LastMessage.FromId != UserDetails.UserId)
                        {
                            item.LastMessage.Seen = "1";
                            TabbedMainActivity.GetInstance()?.LastMessagesTab?.MAdapter?.NotifyDataSetChanged();
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }

                if (DataUser.ChatColor == null)
                    DataUser.ChatColor = AppSettings.MainColor;

                var mainChatColor = DataUser.ChatColor.Contains("rgb") ? Methods.FunString.ConvertColorRgBtoHex(DataUser.ChatColor) : DataUser.ChatColor ?? AppSettings.MainColor;

                if (DataUser.ChatType == "user")
                {
                    Intent intent = new Intent(Application.Context, typeof(ChatWindowActivity));
                    intent.SetFlags(ActivityFlags.NewTask);
                    intent.AddFlags(ActivityFlags.SingleTop);
                    intent.SetAction(Intent.ActionView);
                    intent.PutExtra("UserID", DataUser.UserId);
                    intent.PutExtra("TypeChat", "User");
                    intent.PutExtra("ColorChat", mainChatColor);
                    intent.PutExtra("UserItem", JsonConvert.SerializeObject(new ChatObject()
                    {
                        ChatType = DataUser.ChatType,
                        UserId = DataUser.UserId,
                        GroupId = DataUser.GroupId,
                        PageId = DataUser.PageId,
                        Avatar = DataUser.Avatar,
                        Name = DataUser.Name,
                        Lastseen = DataUser.LastSeen,
                        LastseenUnixTime = DataUser.LastSeenUnixTime,
                        LastMessage = new LastMessageUnion()
                        {
                            LastMessageClass = new MessageData()
                            {
                                Product = new ProductUnion()
                            }
                        },
                    }));
                    Application.Context.StartActivity(intent);
                }

                Destroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void OnFinishFloatingView()
        {
            try
            {
                Destroy();
                StopSelf();
                Console.WriteLine("Finish");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnTouchFinished(bool isFinishing, int x, int y)
        {
            try
            {
                if (!isFinishing)
                {
                    // Save the last position
                    MainSettings.LastPosition.Edit().PutInt(MainSettings.PrefKeyLastPositionX, x).Commit();
                    MainSettings.LastPosition.Edit().PutInt(MainSettings.PrefKeyLastPositionY, y).Commit();
                }
                else
                {
                    Destroy();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Destroy()
        {
            try
            {
                if (MFloatingViewManager != null)
                {
                    MFloatingViewManager.RemoveAllViewToWindow();
                    MFloatingViewManager = null;
                }

                RemoveNotification();

                ShowOrHideBadgeView();

                RunService = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private FloatingViewManager.Options LoadDynamicOptions(DisplayMetrics metrics)
        {
            try
            {
                FloatingViewManager.Options options = new FloatingViewManager.Options
                {
                    OverMargin = (int)(16 * metrics.Density)
                };

                switch (AppSettings.MoveDirectionSettings)
                {
                    case "Default":
                        options.MoveDirection = FloatingViewManager.MoveDirectionDefault;
                        break;
                    case "Left":
                        options.MoveDirection = FloatingViewManager.MoveDirectionLeft;
                        break;
                    case "Right":
                        options.MoveDirection = FloatingViewManager.MoveDirectionRight;
                        break;
                    case "Nearest":
                        options.MoveDirection = FloatingViewManager.MoveDirectionNearest;
                        break;
                    case "Fix":
                        options.MoveDirection = FloatingViewManager.MoveDirectionNone;
                        break;
                    case "Thrown":
                        options.MoveDirection = FloatingViewManager.MoveDirectionThrown;
                        break;
                }

                // Last position 
                if (AppSettings.IsUseLastPosition)
                {
                    var defaultX = options.FloatingViewX;
                    var defaultY = options.FloatingViewY;
                    options.FloatingViewX = MainSettings.LastPosition.GetInt(MainSettings.PrefKeyLastPositionX, defaultX);
                    options.FloatingViewY = MainSettings.LastPosition.GetInt(MainSettings.PrefKeyLastPositionY, defaultY);
                }
                else
                {
                    // Init X/Y
                    String initXSettings = MainSettings.LastPosition.GetString("settings_init_x", "0");
                    String initYSettings = MainSettings.LastPosition.GetString("settings_init_y", "0");
                    if (!TextUtils.IsEmpty(initXSettings) && !TextUtils.IsEmpty(initYSettings))
                    {
                        var offset = (int)(48 + 8 * metrics.Density);
                        options.FloatingViewX = (int)(metrics.WidthPixels * Float.ParseFloat(initXSettings) - offset);
                        options.FloatingViewY = (int)(metrics.HeightPixels * Float.ParseFloat(initYSettings) - offset);
                    }
                }

                switch (AppSettings.ShapeSettings)
                {
                    case "Circle":
                        options.Shape = FloatingViewManager.ShapeCircle;
                        break;
                    case "Rectangle":
                        options.Shape = FloatingViewManager.ShapeRectangle;
                        break;
                }

                return options;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        #region Notification

        private NotificationManager MNotificationManager;
        private NotificationCompat.Builder NotificationBuilder;
        private readonly string NotificationChannelId = "floatingView_ch_10";

        private void CreateNotification(Context context)
        {
            try
            {
                MNotificationManager = (NotificationManager)GetSystemService(NotificationService);

                Intent resultIntent = new Intent(context, typeof(TabbedMainActivity));
                PendingIntent resultPendingIntent = PendingIntent.GetActivity(this, 0, resultIntent, PendingIntentFlags.UpdateCurrent);

                NotificationBuilder = new NotificationCompat.Builder(this, NotificationChannelId)
                .SetSmallIcon(Resource.Mipmap.icon)
                .SetColor(ContextCompat.GetColor(this, Resource.Color.accent))
                .SetContentTitle(context.GetString(Resource.String.Lbl_ChatHead_Title))
                .SetContentText(context.GetString(Resource.String.Lbl_ChatHead_Text))
                .SetTimeoutAfter(700L)
                .SetVisibility(NotificationCompat.VisibilitySecret)
                .SetContentIntent(resultPendingIntent)
                .SetCategory(NotificationCompat.CategoryService)
                .SetPriority(NotificationCompat.PriorityDefault)
                .SetVibrate(new[] { 0L })
                .SetChannelId(NotificationChannelId)
                .SetAutoCancel(true);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    var importance = NotificationImportance.None;
                    NotificationChannel notificationChannel = new NotificationChannel(NotificationChannelId, AppSettings.ApplicationName, importance);
                    notificationChannel.EnableLights(false);
                    notificationChannel.EnableVibration(false);
                    NotificationBuilder.SetChannelId(NotificationChannelId);

                    MNotificationManager?.CreateNotificationChannel(notificationChannel);
                }

                var notification = NotificationBuilder.Build();
                notification.Flags = NotificationFlags.Insistent | NotificationFlags.AutoCancel;
                MNotificationManager?.Notify(NotificationId, notification);

                StartForeground(NotificationId, notification);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void RemoveNotification()
        {
            try
            {
                NotificationManager notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
                notificationManager?.Cancel(NotificationId);

                MNotificationManager?.CancelAll();

                StopForeground(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion


    }
}