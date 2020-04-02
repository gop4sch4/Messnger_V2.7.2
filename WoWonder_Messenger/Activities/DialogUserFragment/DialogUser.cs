using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WoWonder.Activities.ChatWindow;
using WoWonder.Activities.DefaultUser;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using WoWonderClient.Classes.Global;
using WoWonderClient.Requests;


namespace WoWonder.Activities.DialogUserFragment
{
    public class DialogUser : Android.Support.V4.App.DialogFragment
    {
        public class OnUserUpEventArgs : EventArgs
        {
            public View View { get; set; }
            public int Position { get; set; }
        }

        #region Variables Basic

        private TextView TxtUsername;
        private TextView TxtName;

        private CircleButton BtnSendMesseges;
        private CircleButton BtnAdd;

        private ImageView ImageUserprofile;

        public event EventHandler<OnUserUpEventArgs> OnUserUpComplete;

        public string Userid = "";
        public UserDataObject Item;
        private readonly SearchActivity ActivityContext;
        #endregion

        public DialogUser(SearchActivity activity, string userid, UserDataObject item)
        {
            try
            {
                ActivityContext = activity;
                Userid = userid;
                Item = item;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //open Layout as a message
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                base.OnCreateView(inflater, container, savedInstanceState);

                var view = inflater.Inflate(Resource.Layout.Dialog_User_Fragment, container, false);

                // Get values
                TxtUsername = view.FindViewById<TextView>(Resource.Id.Txt_Username);
                TxtName = view.FindViewById<TextView>(Resource.Id.Txt_SecendreName);

                BtnSendMesseges = view.FindViewById<CircleButton>(Resource.Id.SendMesseges_button);

                BtnAdd = view.FindViewById<CircleButton>(Resource.Id.Add_button);
                BtnAdd.Tag = "Add";

                ImageUserprofile = view.FindViewById<ImageView>(Resource.Id.profileAvatar_image);

                //profile_picture
                GlideImageLoader.LoadImage(Activity, Item.Avatar, ImageUserprofile, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                TxtUsername.Text = Item.Name;
                TxtName.Text = "@" + Item.Username;

                if (Item.IsFollowing == "1") // My Friend
                {
                    BtnAdd.SetColor(Color.ParseColor(AppSettings.MainColor));
                    BtnAdd.SetImageResource(Resource.Drawable.ic_tick);
                    BtnAdd.Tag = "friends";
                }
                else if (Item.IsFollowing == "2") // Request
                {
                    BtnAdd.SetColor(Color.ParseColor(AppSettings.MainColor));
                    BtnAdd.SetImageResource(Resource.Drawable.ic_tick);
                    BtnAdd.Tag = "Request";
                }
                else if (Item.IsFollowing == "0") //Not Friend
                {
                    BtnAdd.Visibility = ViewStates.Visible;
                    BtnAdd.SetColor(Color.ParseColor("#8c8a8a"));

                    BtnAdd.SetImageResource(Resource.Drawable.ic_add);
                    BtnAdd.Tag = "Add";
                }

                // Event
                BtnSendMesseges.Click += BtnSendMessegesOnClick;
                BtnAdd.Click += BtnAddOnClick;

                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        //animations
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle); //Sets the title bar to invisible
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; //set the animation
        }

        private void BtnSendMessegesOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                Dismiss();
                int x = Resource.Animation.slide_right;
                Console.WriteLine(x);

                if (Item.ChatColor == null)
                    Item.ChatColor = AppSettings.MainColor;

                var mainChatColor = Item.ChatColor.Contains("rgb") ? Methods.FunString.ConvertColorRgBtoHex(Item.ChatColor) : Item.ChatColor ?? AppSettings.MainColor;

                Intent intent = new Intent(Context, typeof(ChatWindowActivity));
                intent.PutExtra("UserID", Userid);
                intent.PutExtra("TypeChat", "Search");
                intent.PutExtra("ColorChat", mainChatColor);
                intent.PutExtra("UserItem", JsonConvert.SerializeObject(Item));
                StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void BtnAddOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (BtnAdd.Tag.ToString() == "Add") //(is_following == "0") >> Not Friend
                    {
                        BtnAdd.SetColor(Color.ParseColor(AppSettings.MainColor));
                        BtnAdd.SetImageResource(Resource.Drawable.ic_tick);
                        BtnAdd.Tag = "friends";

                        Item.IsFollowing = "1";
                    }
                    else if (BtnAdd.Tag.ToString() == "request") //(is_following == "2") >> Request
                    {
                        BtnAdd.SetColor(Color.ParseColor(AppSettings.MainColor));
                        BtnAdd.SetImageResource(Resource.Drawable.ic_tick);
                        BtnAdd.Tag = "Add";

                        Item.IsFollowing = "2";
                    }
                    else //(is_following == "1") >> Friend
                    {
                        BtnAdd.SetColor(Color.ParseColor("#8c8a8a"));
                        BtnAdd.SetImageResource(Resource.Drawable.ic_add);

                        BtnAdd.Tag = "Add";

                        var dbDatabase = new SqLiteDatabase();
                        dbDatabase.Delete_UsersContact(Userid);
                        dbDatabase.Dispose();

                        Item.IsFollowing = "0";
                    }

                    var local = ActivityContext.MAdapter?.UserList?.FirstOrDefault(a => a.UserId == Userid);
                    if (local != null)
                    {
                        local.IsFollowing = Item.IsFollowing;
                        ActivityContext.MAdapter?.NotifyItemChanged(ActivityContext.MAdapter.UserList.IndexOf(local));
                    }

                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Follow_User(Userid) });

                    if (Item.IsFollowing == "1")
                    {
                        if (AppSettings.ConnectivitySystem == 1)
                        {
                            Toast.MakeText(ActivityContext, GetText(Resource.String.Lbl_Sent_successfully_followed), ToastLength.Short).Show();
                        }
                        else
                        {
                            Toast.MakeText(ActivityContext, GetText(Resource.String.Lbl_Sent_successfully_FriendRequest), ToastLength.Short).Show();
                        }
                    }
                    else
                    {
                        if (AppSettings.ConnectivitySystem == 1)
                        {
                            Toast.MakeText(ActivityContext, GetText(Resource.String.Lbl_Sent_successfully_Unfollowed), ToastLength.Short).Show();
                        }
                        else
                        {
                            Toast.MakeText(ActivityContext, GetText(Resource.String.Lbl_Sent_successfully_FriendRequestCancelled), ToastLength.Short).Show();
                        }
                    }
                }
                else
                {
                    Toast.MakeText(ActivityContext, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
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
    }
}