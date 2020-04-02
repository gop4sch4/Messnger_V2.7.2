using Android.App;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Java.Util;
using Refractored.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using WoWonderClient.Classes.Global;
using WoWonderClient.Requests;
using Exception = System.Exception;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace WoWonder.Activities.DefaultUser.Adapters
{
    public class ContactsAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public enum TypeTextSecondary
        {
            About,
            LastSeen,
            None
        }

        public event EventHandler<ContactsAdapterClickEventArgs> ItemClick;
        public event EventHandler<ContactsAdapterClickEventArgs> ItemLongClick;

        private readonly Activity ActivityContext;
        public ObservableCollection<UserDataObject> UserList = new ObservableCollection<UserDataObject>();
        private readonly bool ShowButton;
        private readonly TypeTextSecondary Type;
        private readonly List<string> ListOnline = new List<string>();
        public ContactsAdapter(Activity activity, bool showButton, TypeTextSecondary type)
        {
            try
            {
                HasStableIds = true;
                ActivityContext = activity;
                ShowButton = showButton;
                Type = type;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override int ItemCount => UserList?.Count ?? 0;

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position, IList<Object> payloads)
        {
            try
            {
                var users = UserList[position];
                if (payloads.Count > 0)
                {
                    if (viewHolder is ContactsAdapterViewHolder holder)
                    {
                        var data = (string)payloads[0];
                        if (data == "true")
                        {
                            holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                            holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                            if (AppSettings.ConnectivitySystem == 1) // Following
                            {
                                holder.Button.Text = ActivityContext.GetText(Resource.String.Lbl_Following);
                                holder.Button.Tag = "true";
                                users.IsFollowing = "1";
                            }
                            else // Request Friend
                            {
                                holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                                holder.Button.SetTextColor(Color.ParseColor("#444444"));
                                holder.Button.Text = ActivityContext.GetText(Resource.String.Lbl_Request);
                                holder.Button.Tag = "Request";
                                users.IsFollowing = "2";
                            }
                        }
                        else
                        {
                            holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                            holder.Button.SetTextColor(Color.ParseColor(AppSettings.MainColor));
                            holder.Button.Text = ActivityContext.GetText(AppSettings.ConnectivitySystem == 1 ? Resource.String.Lbl_Follow : Resource.String.Lbl_AddFriends);
                            holder.Button.Tag = "false";
                            users.IsFollowing = "0";
                            var dbDatabase = new SqLiteDatabase();
                            dbDatabase.Delete_UsersContact(users.UserId);
                            dbDatabase.Dispose();
                        }
                    }
                }
                else
                {
                    base.OnBindViewHolder(viewHolder, position, payloads);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                base.OnBindViewHolder(viewHolder, position, payloads);
            }
        }


        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_HContact_view
                var itemView = LayoutInflater.From(parent.Context)
                    .Inflate(Resource.Layout.Style_HContact_view, parent, false);
                var vh = new ContactsAdapterViewHolder(itemView, Click, LongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is ContactsAdapterViewHolder holder)
                {
                    var item = UserList[position];
                    if (item != null)
                    {
                        Initialize(holder, item);

                        if (ShowButton)
                        {
                            if (!holder.Button.HasOnClickListeners)
                                holder.Button.Click += (sender, e) => FollowButtonClick(new FollowFollowingClickEventArgs { View = holder.ItemView, UserClass = item, Position = position, ButtonFollow = holder.Button });
                        }
                        else
                        {
                            holder.Button.Visibility = ViewStates.Gone;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Initialize(ContactsAdapterViewHolder holder, UserDataObject users)
        {
            try
            {
                GlideImageLoader.LoadImage(ActivityContext, users.Avatar, holder.Image, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                holder.Name.Text = Methods.FunString.SubStringCutOf(WoWonderTools.GetNameFinal(users), 20);

                if (users.Verified == "1")
                    holder.Name.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.icon_checkmark_small_vector, 0);

                if (Type == TypeTextSecondary.None)
                {
                    holder.About.Visibility = ViewStates.Gone;
                }
                else
                {
                    holder.About.Text = Type == TypeTextSecondary.About ? Methods.FunString.SubStringCutOf(WoWonderTools.GetAboutFinal(users), 25) : ActivityContext.GetString(Resource.String.Lbl_Last_seen) + " " + Methods.Time.TimeAgo(int.Parse(users.LastseenUnixTime), true);
                }

                //Online Or offline
                if (users.LastseenStatus?.ToLower() == "on")
                {
                    holder.ImageLastseen.SetImageResource(Resource.Drawable.Green_Online);
                    if (AppSettings.ShowOnlineOfflineMessage)
                    {
                        var data = ListOnline.Contains(users.Name);
                        if (data == false)
                        {
                            ListOnline.Add(users.Name);

                            Toast toast = Toast.MakeText(ActivityContext, users.Name + " " + ActivityContext.GetString(Resource.String.Lbl_Online), ToastLength.Short);
                            toast.SetGravity(GravityFlags.Center, 0, 0);
                            toast.Show();
                        }
                    }
                }
                else
                {
                    holder.ImageLastseen.SetImageResource(Resource.Drawable.Grey_Offline);
                }

                if (ShowButton)
                    switch (users.IsFollowing)
                    {
                        // My Friend
                        case "1":
                            {
                                holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                                holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                                if (AppSettings.ConnectivitySystem == 1) // Following
                                    holder.Button.Text = ActivityContext.GetText(Resource.String.Lbl_Following);
                                else // Friend
                                    holder.Button.Text = ActivityContext.GetText(Resource.String.Lbl_Friends);
                                holder.Button.Tag = "true";
                                break;
                            }
                        // Request
                        case "2":
                            holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                            holder.Button.SetTextColor(Color.ParseColor("#444444"));
                            holder.Button.Text = ActivityContext.GetText(Resource.String.Lbl_Request);
                            holder.Button.Tag = "Request";
                            break;
                        //Not Friend
                        case "0":
                            {
                                holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                                holder.Button.SetTextColor(Color.ParseColor(AppSettings.MainColor));
                                if (AppSettings.ConnectivitySystem == 1) // Following
                                    holder.Button.Text = ActivityContext.GetText(Resource.String.Lbl_Follow);
                                else // Friend
                                    holder.Button.Text = ActivityContext.GetText(Resource.String.Lbl_AddFriends);
                                holder.Button.Tag = "false";

                                var dbDatabase = new SqLiteDatabase();
                                dbDatabase.Delete_UsersContact(users.UserId);
                                dbDatabase.Dispose();
                                break;
                            }
                        default:
                            {
                                holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                                holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                                if (AppSettings.ConnectivitySystem == 1) // Following
                                    holder.Button.Text = ActivityContext.GetText(Resource.String.Lbl_Following);
                                else // Friend
                                    holder.Button.Text = ActivityContext.GetText(Resource.String.Lbl_Friends);

                                users.IsFollowing = "1";
                                holder.Button.Tag = "true";
                                break;
                            }
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void FollowButtonClick(FollowFollowingClickEventArgs e)
        {
            try
            {
                if (e.UserClass != null)
                {
                    if (e.ButtonFollow.Tag.ToString() == "false")
                        NotifyItemChanged(e.Position, "true");
                    else
                        NotifyItemChanged(e.Position, "false");

                    if (Methods.CheckConnectivity())
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Follow_User(e.UserClass.UserId) });
                    else
                        Toast.MakeText(ActivityContext, ActivityContext.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();

                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public UserDataObject GetItem(int position)
        {
            return UserList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return int.Parse(UserList[position].UserId);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        private void Click(ContactsAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void LongClick(ContactsAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = UserList[p0];
                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.Avatar != "")
                {
                    d.Add(item.Avatar);
                    return d;
                }

                return d;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Collections.SingletonList(p0);
            }
        }

        public RequestBuilder GetPreloadRequestBuilder(Object p0)
        {
            return GlideImageLoader.GetPreLoadRequestBuilder(ActivityContext, p0.ToString(), ImageStyle.CircleCrop);
        }
    }

    public class ContactsAdapterViewHolder : RecyclerView.ViewHolder
    {
        public ContactsAdapterViewHolder(View itemView, Action<ContactsAdapterClickEventArgs> clickListener,
            Action<ContactsAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<ImageView>(Resource.Id.card_pro_pic);
                Name = MainView.FindViewById<TextView>(Resource.Id.card_name);
                About = MainView.FindViewById<TextView>(Resource.Id.card_dist);
                Button = MainView.FindViewById<Button>(Resource.Id.cont);
                ImageLastseen = (CircleImageView)MainView.FindViewById(Resource.Id.ImageLastseen);

                //Event
                itemView.Click += (sender, e) => clickListener(new ContactsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new ContactsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region Variables Basic

        public View MainView { get; }

        public ImageView Image { get; private set; }
        public TextView Name { get; private set; }
        public TextView About { get; private set; }
        public Button Button { get; private set; }
        public CircleImageView ImageLastseen { get; private set; }

        #endregion
    }

    public class ContactsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }

    public class FollowFollowingClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
        public UserDataObject UserClass { get; set; }
        public Button ButtonFollow { get; set; }
    }
}