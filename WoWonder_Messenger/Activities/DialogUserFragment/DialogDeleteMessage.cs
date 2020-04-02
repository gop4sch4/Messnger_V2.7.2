using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WoWonder.Activities.Tab;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Message;
using static WoWonderClient.Requests.RequestsAsync;

namespace WoWonder.Activities.DialogUserFragment
{
    public class DialogDeleteMessage : Android.Support.V4.App.DialogFragment
    {
        public class OnDeleteMessageUpEventArgs : EventArgs
        {
            public View View { get; set; }
            public int Position { get; set; }
        }

        #region Variables Basic

        private TextView TxtUsername;
        private TextView TxtName;

        private Button BtnDeleteMessage;

        private ImageView ImageUserprofile;

        public event EventHandler<OnDeleteMessageUpEventArgs> OnDeleteMessageUpComplete;

        private readonly string Userid = "";
        private readonly ChatObject ItemChat;
        private readonly GetUsersListObject.User ItemUser;
        private readonly TabbedMainActivity GlobalContext;

        #endregion

        public DialogDeleteMessage(string userid, ChatObject item)
        {
            try
            {
                Userid = userid;
                ItemChat = item;//ChatObject

                GlobalContext = TabbedMainActivity.GetInstance();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public DialogDeleteMessage(string userid, GetUsersListObject.User item)
        {
            try
            {
                Userid = userid;
                ItemUser = item;//GetUsersListObject.User

                GlobalContext = TabbedMainActivity.GetInstance();
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

                // Set our view from the "Dialog_DeleteMessege_Fragment" layout resource
                var view = inflater.Inflate(Resource.Layout.Dialog_DeleteMessege_Fragment, container, false);

                // Get values
                TxtUsername = view.FindViewById<TextView>(Resource.Id.Txt_Username);
                TxtName = view.FindViewById<TextView>(Resource.Id.Txt_SecendreName);

                BtnDeleteMessage = view.FindViewById<Button>(Resource.Id.DeleteMessage_Button);

                ImageUserprofile = view.FindViewById<ImageView>(Resource.Id.profileAvatar_image);

                if (ItemChat != null)
                {
                    //profile_picture
                    GlideImageLoader.LoadImage(Activity, ItemChat.Avatar, ImageUserprofile, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                    switch (ItemChat.ChatType)
                    {
                        case "user":
                            TxtName.Text = "@" + ItemChat.Username;
                            break;
                        case "page":
                            TxtName.Text = "@" + ItemChat.PageName;
                            break;
                        case "group":
                            TxtName.Text = "@" + ItemChat.GroupName;
                            break;
                    }
                }
                else if (ItemUser != null)
                {
                    //profile_picture
                    GlideImageLoader.LoadImage(Activity, ItemUser.Avatar, ImageUserprofile, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                    TxtName.Text = "@" + ItemUser.Username;
                }

                TxtUsername.Text = GetText(Resource.String.Lbl_Doyouwanttodeletechatwith);

                // Event
                BtnDeleteMessage.Click += BtnDeleteMessageOnClick;

                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            Dialog.Window.RequestFeature(WindowFeatures.NoTitle); //Sets the title bar to invisible
            base.OnActivityCreated(savedInstanceState);
            Dialog.Window.Attributes.WindowAnimations = Resource.Style.dialog_animation; //set the animation
        }

        private void BtnDeleteMessageOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                {
                    var usertoDelete = GlobalContext?.LastChatTab?.MAdapter?.ChatList.FirstOrDefault(a => a.UserId == Userid);
                    if (usertoDelete != null)
                    {
                        var index = GlobalContext.LastChatTab.MAdapter.ChatList.IndexOf(usertoDelete);
                        if (index != -1)
                        {
                            GlobalContext?.LastChatTab.MAdapter.ChatList.Remove(usertoDelete);
                            GlobalContext?.LastChatTab.MAdapter.NotifyItemRemoved(index);
                        }
                    }
                }
                else
                {
                    var userToDelete = GlobalContext?.LastMessagesTab?.MAdapter?.MLastMessagesUser.FirstOrDefault(a => a.UserId == Userid);
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

                var dbDatabase = new SqLiteDatabase();
                dbDatabase.Delete_LastUsersChat(Userid);
                dbDatabase.DeleteAllMessagesUser(UserDetails.UserId, Userid);
                dbDatabase.Dispose();

                if (Methods.CheckConnectivity())
                {
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => Global.Delete_Conversation(Userid) });
                }

                Dismiss();
                int x = Resource.Animation.slide_right;
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
    }
}