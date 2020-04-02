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
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace WoWonder.Activities.NearBy.Adapters
{
    public class NearByAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {

        public Activity ActivityContext;

        public ObservableCollection<UserDataObject> NearByList = new ObservableCollection<UserDataObject>();

        public NearByAdapter(Activity context)
        {
            try
            {
                // HasStableIds = true;
                ActivityContext = context;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override int ItemCount => NearByList?.Count ?? 0;

        public event EventHandler<NearByAdapterClickEventArgs> ItemClick;

        public event EventHandler<NearByAdapterClickEventArgs> ItemLongClick;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_NearBy_view
                var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_NearBy_view, parent, false);
                var vh = new NearByAdapterViewHolder(itemView, Click, LongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position, IList<Object> payloads)
        {
            try
            {
                if (payloads.Count > 0)
                {
                    if (viewHolder is NearByAdapterViewHolder holder)
                    {
                        var users = NearByList[position];

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
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is NearByAdapterViewHolder holder)
                {
                    var users = NearByList[position];
                    if (users != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext, users.Avatar, holder.Image, ImageStyle.CircleCrop, ImagePlaceholders.Color);

                        //Online Or offline
                        if (users.LastseenStatus == "on")
                        {
                            //Online 
                            GlideImageLoader.LoadImage(ActivityContext, "Green_Online", holder.ImageOnline, ImageStyle.CircleCrop, ImagePlaceholders.Color);
                            holder.LastTimeOnline.Text = ActivityContext.GetString(Resource.String.Lbl_Online);
                        }
                        else
                        {
                            GlideImageLoader.LoadImage(ActivityContext, "Grey_Offline", holder.ImageOnline, ImageStyle.CircleCrop, ImagePlaceholders.Color);
                            holder.LastTimeOnline.Text = Methods.Time.TimeAgo(int.Parse(users.LastseenUnixTime), true);
                        }

                        holder.Name.Text = Methods.FunString.SubStringCutOf(WoWonderTools.GetNameFinal(users), 14);

                        if (users.IsFollowing == "1" || users.IsFollowing == "yes" || users.IsFollowing == "Yes") // My Friend
                        {
                            holder.Button.SetBackgroundResource(Resource.Drawable
                                .follow_button_profile_friends_pressed);
                            holder.Button.SetTextColor(Color.ParseColor("#ffffff"));
                            holder.Button.Text = ActivityContext.GetText(AppSettings.ConnectivitySystem == 1
                                ? Resource.String.Lbl_Following
                                : Resource.String.Lbl_Friends);

                            holder.Button.Tag = "friends";
                        }
                        else //Not Friend
                        {
                            holder.Button.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                            holder.Button.SetTextColor(Color.ParseColor(AppSettings.MainColor));
                            holder.Button.Text = ActivityContext.GetText(AppSettings.ConnectivitySystem == 1
                                ? Resource.String.Lbl_Follow
                                : Resource.String.Lbl_AddFriends);
                            holder.Button.Tag = "false";
                        }

                        if (!holder.Button.HasOnClickListeners)
                            holder.Button.Click += (sender, args) =>
                            {
                                try
                                {
                                    if (!Methods.CheckConnectivity())
                                    {
                                        Toast.MakeText(ActivityContext,
                                            ActivityContext.GetString(Resource.String.Lbl_CheckYourInternetConnection),
                                            ToastLength.Short).Show();
                                    }
                                    else
                                    {
                                        NotifyItemChanged(viewHolder.AdapterPosition,
                                            holder.Button.Tag.ToString() == "false" ? "true" : "false");

                                        if (Methods.CheckConnectivity())
                                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Follow_User(users.UserId) });
                                        else
                                            Toast.MakeText(ActivityContext,
                                                ActivityContext.GetString(Resource.String
                                                    .Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                            };
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public UserDataObject GetItem(int position)
        {
            return NearByList[position];
        }

        public override long GetItemId(int position)
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

        private void Click(NearByAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void LongClick(NearByAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = NearByList[p0];
                if (item == null)
                    return d;
                else
                {
                    if (!string.IsNullOrEmpty(item.Avatar))
                        d.Add(item.Avatar);

                    return d;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Collections.SingletonList(p0);
            }
        }

        public RequestBuilder GetPreloadRequestBuilder(Object p0)
        {
            return GlideImageLoader.GetPreLoadRequestBuilder(ActivityContext, p0.ToString(), ImageStyle.CenterCrop);
        }
    }



    public class NearByAdapterViewHolder : RecyclerView.ViewHolder
    {
        public NearByAdapterViewHolder(View itemView, Action<NearByAdapterClickEventArgs> clickListener, Action<NearByAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                Image = MainView.FindViewById<CircleImageView>(Resource.Id.people_profile_sos);
                ImageOnline = MainView.FindViewById<CircleImageView>(Resource.Id.ImageLastseen);
                Name = MainView.FindViewById<TextView>(Resource.Id.people_profile_name);
                LastTimeOnline = MainView.FindViewById<TextView>(Resource.Id.people_profile_time);
                Button = MainView.FindViewById<Button>(Resource.Id.btn_follow_people);

                //Event
                itemView.Click += (sender, e) => clickListener(new NearByAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new NearByAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #region Variables Basic

        public View MainView { get; }
        public CircleImageView Image { get; set; }
        public CircleImageView ImageOnline { get; set; }
        public TextView Name { get; set; }
        public TextView LastTimeOnline { get; set; }
        public Button Button { get; set; }

        #endregion Variables Basic
    }

    public class NearByAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}