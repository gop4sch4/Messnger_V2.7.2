using Android.App;
using Android.OS;
using Android.Support.V4.View.Animation;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WoWonder.Activities.Tab;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Utils;
using WoWonderClient.Requests;

namespace WoWonder.Activities.ChatWindow.Fragment
{
    public class ChatColorsFragment : Android.Support.V4.App.Fragment
    {
        private CircleButton Closebutton;
        private string UserId;

        public override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                UserId = Arguments.GetString("userid");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // Use this to return your custom view for this Fragment
                View chatColorsFragmentView = inflater.Inflate(Resource.Layout.Chat_Colors_Fragment, container, false);
                Closebutton = chatColorsFragmentView.FindViewById<CircleButton>(Resource.Id.closebutton);

                Closebutton = chatColorsFragmentView.FindViewById<CircleButton>(Resource.Id.closebutton);
                Closebutton.Click += Closebutton_Click;

                var colorButton1 = chatColorsFragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton1);
                var colorButton2 = chatColorsFragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton2);
                var colorButton3 = chatColorsFragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton3);
                var colorButton4 = chatColorsFragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton4);
                var colorButton5 = chatColorsFragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton5);
                var colorButton6 = chatColorsFragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton6);
                var colorButton7 = chatColorsFragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton7);
                var colorButton8 = chatColorsFragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton8);
                var colorButton9 = chatColorsFragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton9);
                var colorButton10 = chatColorsFragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton10);
                var colorButton11 = chatColorsFragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton11);
                var colorButton12 = chatColorsFragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton12);
                var colorButton13 = chatColorsFragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton13);
                var colorButton14 = chatColorsFragmentView.FindViewById<CircleButton>(Resource.Id.colorbutton14);

                colorButton1.Click += SetColorbutton_Click;
                colorButton2.Click += SetColorbutton_Click;
                colorButton3.Click += SetColorbutton_Click;
                colorButton4.Click += SetColorbutton_Click;
                colorButton5.Click += SetColorbutton_Click;
                colorButton6.Click += SetColorbutton_Click;
                colorButton7.Click += SetColorbutton_Click;
                colorButton8.Click += SetColorbutton_Click;
                colorButton9.Click += SetColorbutton_Click;
                colorButton10.Click += SetColorbutton_Click;
                colorButton11.Click += SetColorbutton_Click;
                colorButton12.Click += SetColorbutton_Click;
                colorButton13.Click += SetColorbutton_Click;
                colorButton14.Click += SetColorbutton_Click;

                return chatColorsFragmentView;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private void SetColorbutton_Click(object sender, EventArgs e)
        {
            try
            {
                CircleButton btn = (CircleButton)sender;
                string colorCssode = (string)btn.Tag;

                var mainActivityview = ((ChatWindowActivity)Activity);
                mainActivityview.ToolBar.SetBackgroundColor(Android.Graphics.Color.ParseColor(colorCssode));
                mainActivityview.ChatSendButton.SetColor(Android.Graphics.Color.ParseColor(colorCssode));

                ChatWindowActivity.MainChatColor = colorCssode;
                ChatWindowActivity.ColorChanged = true;

                SetTheme(Activity, colorCssode);

                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    mainActivityview.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                    mainActivityview.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                    mainActivityview.Window.SetStatusBarColor(Android.Graphics.Color.ParseColor(colorCssode));
                }

                mainActivityview.Recreate();
                var colorFragmentHolder = Activity.FindViewById<FrameLayout>(Resource.Id.ButtomFragmentHolder);
                var interplator = new FastOutSlowInInterpolator();
                colorFragmentHolder.Animate().SetInterpolator(interplator).TranslationY(1200).SetDuration(500);
                Activity.SupportFragmentManager.BeginTransaction().Remove(this).Commit();
                mainActivityview.ChatColorButton.Tag = "Closed";
                mainActivityview.ChatColorButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#888888"));

                if (Methods.CheckConnectivity())
                {
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Message.Change_Chat_Color(UserId, colorCssode) });
                }
                else
                {
                    Toast.MakeText(Context, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //private Color DarkerColor(Android.Graphics.Color color, float correctionfactory = 50f)
        //{
        //    const float hundredpercent = 100f;
        //    return Color.FromArgb((int)((color.R / hundredpercent) * correctionfactory),(int)((color.G / hundredpercent) * correctionfactory), (int)((color.B / hundredpercent) * correctionfactory));
        //}

        //public static Color ChangeColorBrightness(Android.Graphics.Color color, float correctionFactor)
        //{
        //    float red = color.R;
        //    float green = color.G;
        //    float blue = color.B;

        //    if (correctionFactor < 0)
        //    {
        //        correctionFactor = 1 + correctionFactor;
        //        red *= correctionFactor;
        //        green *= correctionFactor;
        //        blue *= correctionFactor;
        //    }
        //    else
        //    {
        //        red = (255 - red) * correctionFactor + red;
        //        green = (255 - green) * correctionFactor + green;
        //        blue = (255 - blue) * correctionFactor + blue;
        //    }

        //    return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        //}

        private void Closebutton_Click(object sender, EventArgs e)
        {
            try
            {
                var colorFragmentHolder = Activity.FindViewById<FrameLayout>(Resource.Id.ButtomFragmentHolder);
                var interplator = new FastOutSlowInInterpolator();
                colorFragmentHolder.Animate().SetInterpolator(interplator).TranslationY(1200).SetDuration(500);
                Activity.SupportFragmentManager.BeginTransaction().Remove(this).Commit();

                var mainActivityview = ((ChatWindowActivity)Activity);

                mainActivityview.ChatColorButton.Tag = "Closed";
                mainActivityview.ChatColorButton.Drawable.SetTint(Android.Graphics.Color.ParseColor("#888888"));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SetTheme(Activity activity, string color)
        {
            try
            {
                if (color.Contains("b582af"))
                {
                    activity.SetTheme(Resource.Style.Chatththemeb582af);
                }
                else if (color.Contains("a84849"))
                {
                    activity.SetTheme(Resource.Style.Chatththemea84849);
                }
                else if (color.Contains("f9c270"))
                {
                    activity.SetTheme(Resource.Style.Chatththemef9c270);
                }
                else if (color.Contains("70a0e0"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme70a0e0);
                }
                else if (color.Contains("56c4c5"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme56c4c5);
                }
                else if (color.Contains("f33d4c"))
                {
                    activity.SetTheme(Resource.Style.Chatththemef33d4c);
                }
                else if (color.Contains("a1ce79"))
                {
                    activity.SetTheme(Resource.Style.Chatththemea1ce79);
                }
                else if (color.Contains("a085e2"))
                {
                    activity.SetTheme(Resource.Style.Chatththemea085e2);
                }
                else if (color.Contains("ed9e6a"))
                {
                    activity.SetTheme(Resource.Style.Chatththemeed9e6a);
                }
                else if (color.Contains("2b87ce"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme2b87ce);
                }
                else if (color.Contains("f2812b"))
                {
                    activity.SetTheme(Resource.Style.Chatththemef2812b);
                }
                else if (color.Contains("0ba05d"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme0ba05d);
                }
                else if (color.Contains("0e71ea"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme0e71ea);
                }
                else if (color.Contains("aa2294"))
                {
                    activity.SetTheme(Resource.Style.Chatththemeaa2294);
                }
                else if (color.Contains("f9a722"))
                {
                    activity.SetTheme(Resource.Style.Chatththemef9a722);
                }
                else if (color.Contains("008484"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme008484);
                }
                else if (color.Contains("5462a5"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme5462a5);
                }
                else if (color.Contains("fc9cde"))
                {
                    activity.SetTheme(Resource.Style.Chatththemefc9cde);
                }
                else if (color.Contains("fc9cde"))
                {
                    activity.SetTheme(Resource.Style.Chatththemefc9cde);
                }
                else if (color.Contains("51bcbc"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme51bcbc);
                }
                else if (color.Contains("c9605e"))
                {
                    activity.SetTheme(Resource.Style.Chatththemec9605e);
                }
                else if (color.Contains("01a5a5"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme01a5a5);
                }
                else if (color.Contains("056bba"))
                {
                    activity.SetTheme(Resource.Style.Chatththeme056bba);
                }
                else
                {
                    //Default Color >> AppSettings.MainColor
                    activity.SetTheme(Resource.Style.Chatththemedefault);
                }

                var dataUser = TabbedMainActivity.GetInstance().LastChatTab.MAdapter.ChatList?.FirstOrDefault(a => a.UserId == UserId);
                if (dataUser != null)
                {
                    dataUser.LastMessage.LastMessageClass.ChatColor = color;
                }

                var mainActivityview = ((ChatWindowActivity)Activity);
                if (mainActivityview.DataUser != null) mainActivityview.DataUser.LastMessage.LastMessageClass.ChatColor = color;
                if (mainActivityview.UserData != null) mainActivityview.UserData.ChatColor = color;

                ChatWindowActivity.MainChatColor = color;

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
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override void OnDestroy()
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
    }
}