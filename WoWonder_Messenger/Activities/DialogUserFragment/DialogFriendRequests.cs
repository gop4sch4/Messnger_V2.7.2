using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WoWonder.Activities.FriendRequest;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Utils;
using WoWonderClient.Classes.Global;
using WoWonderClient.Requests;

namespace WoWonder.Activities.DialogUserFragment
{
    public class DialogFriendRequests : Android.Support.V4.App.DialogFragment
    {
        #region Variables Basic

        private TextView TxtUsername, TxtName;
        private CircleButton BtnDelete, BtnAccept;
        private ImageView ImageUserProfile, ImageCover;
        public event EventHandler<FriendRequestsUpEventArgs> OnUserUpComplete;
        private readonly string Userid = "";
        private readonly UserDataObject Item;
        private readonly FriendRequestActivity ActivityContext;

        #endregion

        public DialogFriendRequests(FriendRequestActivity activity, string userid, UserDataObject item)
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

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                base.OnCreateView(inflater, container, savedInstanceState);

                var view = inflater.Inflate(Resource.Layout.Dialog_FriendRequest_Fragment, container, false);

                // Get values
                TxtUsername = view.FindViewById<TextView>(Resource.Id.Txt_Username);
                TxtName = view.FindViewById<TextView>(Resource.Id.Txt_SecendreName);

                BtnDelete = view.FindViewById<CircleButton>(Resource.Id.delete_button);
                BtnDelete.Tag = "false";

                BtnAccept = view.FindViewById<CircleButton>(Resource.Id.Add_button);
                BtnAccept.Tag = "false";
                //BtnAccept.SetColor(Color.ParseColor("#8c8a8a"));

                ImageUserProfile = view.FindViewById<ImageView>(Resource.Id.profileAvatar_image);
                ImageCover = view.FindViewById<ImageView>(Resource.Id.profileCover_image);

                LoadData();

                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();

                //Add Event
                BtnDelete.Click += BtnDelete_OnClick;
                BtnAccept.Click += BtnAccept_OnClick;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnPause()
        {
            try
            {
                base.OnPause();

                //Close Event
                BtnDelete.Click -= BtnDelete_OnClick;
                BtnAccept.Click -= BtnAccept_OnClick;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadData()
        {
            try
            {
                GlideImageLoader.LoadImage(ActivityContext, Item.Avatar, ImageUserProfile, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                Glide.With(ActivityContext).Load(Item.Cover).Apply(new RequestOptions()).Into(ImageCover);

                TxtUsername.Text = WoWonderTools.GetNameFinal(Item);
                TxtName.Text = "@" + Item.Username;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Event button Accept User
        private void BtnAccept_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection),
                        ToastLength.Short).Show();
                }
                else
                {
                    if (BtnAccept.Tag.ToString() == "false")
                    {
                        BtnAccept.SetColor(Color.ParseColor(AppSettings.MainColor));
                        BtnAccept.SetImageResource(Resource.Drawable.ic_tick);
                        BtnAccept.Tag = "true";
                    }
                    else
                    {
                        BtnAccept.SetColor(Color.ParseColor("#8c8a8a"));
                        BtnAccept.SetImageResource(Resource.Drawable.ic_add);
                        BtnAccept.Tag = "false";
                    }

                    BtnDelete.Visibility = ViewStates.Gone;
                    BtnAccept.Enabled = false;

                    var local = ActivityContext.MAdapter?.UserList?.FirstOrDefault(a => a.UserId == Userid);
                    if (local != null)
                    {
                        ActivityContext.MAdapter?.UserList.Remove(local);
                        ActivityContext.MAdapter?.NotifyItemRemoved(ActivityContext.MAdapter.UserList.IndexOf(local));
                    }

                    if (ActivityContext.MAdapter?.UserList?.Count == 0)
                    {
                        ActivityContext.ShowEmptyPage();
                    }

                    //Toast.MakeText(ActivityContext, ActivityContext.GetString(Resource.String.Lbl_Done), ToastLength.Short).Show();

                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Follow_Request_Action(Item.UserId, true) }); // true >> Accept 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Event button Decline User
        private void BtnDelete_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(Context, Context.GetString(Resource.String.Lbl_CheckYourInternetConnection),
                        ToastLength.Short).Show();
                }
                else
                {
                    if (BtnDelete.Tag.ToString() == "false")
                    {
                        BtnDelete.SetColor(Color.ParseColor("#efefef"));
                        BtnDelete.SetImageResource(Resource.Drawable.ic_tick);
                        BtnDelete.Drawable.SetTint(Color.ParseColor("#282828"));
                        BtnDelete.Tag = "true";
                    }
                    else
                    {
                        BtnDelete.SetColor(Color.ParseColor("#282828"));
                        BtnDelete.SetImageResource(Resource.Drawable.ic_close_padded);
                        BtnDelete.Drawable.SetTint(Color.ParseColor("#efefef"));

                        BtnDelete.Tag = "false";
                    }

                    BtnAccept.Visibility = ViewStates.Gone;
                    BtnDelete.Enabled = false;

                    var local = ActivityContext.MAdapter?.UserList?.FirstOrDefault(a => a.UserId == Userid);
                    if (local != null)
                    {
                        ActivityContext.MAdapter?.UserList.Remove(local);
                        ActivityContext.MAdapter?.NotifyItemRemoved(ActivityContext.MAdapter.UserList.IndexOf(local));
                    }

                    if (ActivityContext.MAdapter?.UserList?.Count == 0)
                    {
                        ActivityContext.ShowEmptyPage();
                    }

                    //Toast.MakeText(ActivityContext, ActivityContext.GetString(Resource.String.Lbl_Done), ToastLength.Short).Show();

                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Follow_Request_Action(Item.UserId, false) });// false >> Decline

                    var th = new Thread(ActLikeARequest);
                    th.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ActLikeARequest()
        {
            var x = Resource.Animation.slide_right;
            Console.WriteLine(x);
        }


        //animations
        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            try
            {
                Dialog.Window.RequestFeature(WindowFeatures.NoTitle); //Sets the title bar to invisible
                base.OnActivityCreated(savedInstanceState);
                Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; //set the animation
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

        public class FriendRequestsUpEventArgs : EventArgs
        {
            public View View { get; set; }
            public int Position { get; set; }
        }
    }
}