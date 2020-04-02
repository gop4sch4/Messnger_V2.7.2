using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WoWonder.Activities.DefaultUser;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Utils;
using WoWonderClient.Classes.Global;
using WoWonderClient.Requests;

namespace WoWonder.Activities.DialogUserFragment
{
    public class DialogBlockUser : Android.Support.V4.App.DialogFragment
    {
        public DialogBlockUser(BlockedUsersActivity activity, string userid, UserDataObject item)
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
                var view = inflater.Inflate(Resource.Layout.Dialog_BlockUser_Fragment, container, false);

                // Get values
                TxtUsername = view.FindViewById<TextView>(Resource.Id.Txt_Username);
                TxtName = view.FindViewById<TextView>(Resource.Id.Txt_SecendreName);

                BtnUnBlockUser = view.FindViewById<Button>(Resource.Id.UnBlockUser_Button);

                ImageUserProfile = view.FindViewById<ImageView>(Resource.Id.profileAvatar_image);

                GlideImageLoader.LoadImage(ActivityContext, Item.Avatar, ImageUserProfile, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                TxtUsername.Text = WoWonderTools.GetNameFinal(Item);
                TxtName.Text = "@" + Item.Username;

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
                BtnUnBlockUser.Click += BtnUnBlockUserOnClick;
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

                // Event
                BtnUnBlockUser.Click -= BtnUnBlockUserOnClick;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle); //Sets the title bar to invisible
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; //set the animation
        }

        private void BtnUnBlockUserOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
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

                    Toast.MakeText(ActivityContext, GetString(Resource.String.Lbl_Unblock_successfully), ToastLength.Short).Show();

                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Block_User(Userid, false) });//false >> "un-block"
                }
                else
                {
                    Toast.MakeText(ActivityContext, GetString(Resource.String.Lbl_CheckYourInternetConnection),
                        ToastLength.Short).Show();
                }

                Dismiss();
                var x = Resource.Animation.slide_right;
                Console.WriteLine(x);
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

        public class BlockUserUpEventArgs : EventArgs
        {
            public View View { get; set; }
            public int Position { get; set; }
        }

        #region Variables Basic

        private TextView TxtUsername;
        private TextView TxtName;
        private Button BtnUnBlockUser;
        private ImageView ImageUserProfile;
        public event EventHandler<BlockUserUpEventArgs> OnBlockUserUpComplete;
        private readonly string Userid = "";
        private readonly UserDataObject Item;
        private readonly BlockedUsersActivity ActivityContext;

        #endregion

    }
}