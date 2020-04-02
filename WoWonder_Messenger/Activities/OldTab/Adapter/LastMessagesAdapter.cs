using Android.App;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Java.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WoWonder.Adapters;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonderClient.Classes.Message;
using XamarinTextDrawable;
using Console = System.Console;
using Object = Java.Lang.Object;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.OldTab.Adapter
{
    public class LastMessagesAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider, SwipeItemTouchHelper.ISwipeHelperAdapter
    {
        public event EventHandler<Holders.LastMessagesClickEventArgs> MoreItemClick;
        public event EventHandler<Holders.LastMessagesClickEventArgs> CallItemClick;
        public event EventHandler<Holders.LastMessagesClickEventArgs> DeleteItemClick;
        public event EventHandler<Holders.LastMessagesClickEventArgs> ItemClick;
        public event EventHandler<Holders.LastMessagesClickEventArgs> ItemLongClick;

        public ObservableCollection<GetUsersListObject.User> MLastMessagesUser = new ObservableCollection<GetUsersListObject.User>();

        private readonly Activity ActivityContext;
        private readonly List<string> ListOnline = new List<string>();
        private bool OnlineUsers { get; set; }
        private readonly RequestOptions Options;

        public LastMessagesAdapter(Activity context, bool onlineUsers)
        {
            try
            {
                ActivityContext = context;
                OnlineUsers = onlineUsers;
                Options = new RequestOptions().Apply(RequestOptions.CircleCropTransform()
                    .CenterCrop().CircleCrop()
                    .SetPriority(Priority.High).Override(200)
                    .SetUseAnimationPool(false).SetDiskCacheStrategy(DiskCacheStrategy.All)
                    .Error(Resource.Drawable.ImagePlacholder_circle)
                    .Placeholder(Resource.Drawable.ImagePlacholder_circle));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {

                //Setup your layout here >> Last_Message_view
                var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Last_Message_view, parent, false);
                var holder = new Holders.LastMessagesViewHolder(itemView, OnClick, OnLongClick, MoreOnClick, CallOnClick, DeleteOnClick);
                return holder;
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
                if (viewHolder is Holders.LastMessagesViewHolder holder)
                {
                    var item = MLastMessagesUser[position];
                    if (item != null)
                    {
                        Initialize(holder, item);
                        holder.RelativeLayoutMain.Visibility = item.Swiped ? ViewStates.Gone : ViewStates.Visible;
                        holder.MainSwipe.Visibility = item.Swiped ? ViewStates.Visible : ViewStates.Gone;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Initialize(Holders.LastMessagesViewHolder holder, GetUsersListObject.User item)
        {
            try
            {
                var image = !string.IsNullOrEmpty(item.OldAvatar) ? item.OldAvatar : item.Avatar ?? "";
                if (!string.IsNullOrEmpty(image))
                {
                    var avatarSplit = image.Split('/').Last();
                    var getImageAvatar = Methods.MultiMedia.GetMediaFrom_Disk(Methods.Path.FolderDiskImage, avatarSplit);
                    if (getImageAvatar != "File Dont Exists")
                    {
                        var file = Uri.FromFile(new File(getImageAvatar));
                        Glide.With(ActivityContext).Load(file.Path).Apply(Options).Into(holder.ImageAvatar);
                    }
                    else
                    {
                        Methods.MultiMedia.DownloadMediaTo_DiskAsync(Methods.Path.FolderDiskImage, image);
                        GlideImageLoader.LoadImage(ActivityContext, image, holder.ImageAvatar, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                    }
                }

                holder.TxtUsername.Text = WoWonderTools.GetNameFinal(item);

                if (item.Verified == "1")
                    holder.TxtUsername.SetCompoundDrawablesWithIntrinsicBounds(0, 0, Resource.Drawable.icon_checkmark_small_vector, 0);

                if (item.LastMessage.Stickers != null)
                    item.LastMessage.Stickers = item.LastMessage.Stickers.Replace(".mp4", ".gif");

                //If message contains Media files 
                if (item.LastMessage.Media.Contains("image"))
                {

                    holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                    Glide.With(ActivityContext).Load(Resource.Drawable.ic_image_vector).Into(holder.LastMessagesIcon);
                    holder.TxtLastMessages.Text = ActivityContext.GetText(Resource.String.Lbl_SendImageFile);

                }
                else if (item.LastMessage.Media.Contains("video"))
                {
                    holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                    Glide.With(ActivityContext).Load(Resource.Drawable.ic_video_player_vector).Into(holder.LastMessagesIcon);
                    holder.TxtLastMessages.Text = ActivityContext.GetText(Resource.String.Lbl_SendVideoFile);
                }
                else if (item.LastMessage.Media.Contains("sticker"))
                {

                    holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                    Glide.With(ActivityContext).Load(Resource.Drawable.ic_sticker_vector).Into(holder.LastMessagesIcon);
                    holder.TxtLastMessages.Text = ActivityContext.GetText(Resource.String.Lbl_SendStickerFile);

                }
                else if (item.LastMessage.Media.Contains("sounds"))
                {
                    holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                    Glide.With(ActivityContext).Load(Resource.Drawable.ic_radios_vector).Into(holder.LastMessagesIcon);
                    holder.TxtLastMessages.Text = ActivityContext.GetText(Resource.String.Lbl_SendAudioFile);
                }
                else if (item.LastMessage.Media.Contains("file"))
                {
                    holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                    Glide.With(ActivityContext).Load(Resource.Drawable.ic_document_vector).Into(holder.LastMessagesIcon);
                    holder.TxtLastMessages.Text = ActivityContext.GetText(Resource.String.Lbl_SendFile);

                }
                else if (item.LastMessage?.Stickers != null && item.LastMessage.Stickers.Contains(".gif"))
                {
                    holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                    Glide.With(ActivityContext).Load(Resource.Drawable.ic_gif_vector).Into(holder.LastMessagesIcon);
                    holder.TxtLastMessages.Text = ActivityContext.GetText(Resource.String.Lbl_SendGifFile);

                }
                else if (!string.IsNullOrEmpty(item.LastMessage.ProductId) && item.LastMessage.ProductId != "0")
                {
                    holder.TxtLastMessages.Text = ActivityContext.GetText(Resource.String.Lbl_SendProductFile);
                }
                else
                {
                    // holder.LastMessagesIcon.Visibility = ViewStates.Gone;

                    if (item.LastMessage.Text.Contains("http"))
                    {
                        holder.TxtLastMessages.Text = Methods.FunString.SubStringCutOf(item.LastMessage.Text, 30);
                    }
                    else if (item.LastMessage.Text.Contains("{&quot;Key&quot;") || item.LastMessage.Text.Contains("{key:^qu") || item.LastMessage.Text.Contains("{^key:^qu") || item.LastMessage.Text.Contains("{key:"))
                    {
                        holder.TxtLastMessages.Text = ActivityContext.GetText(Resource.String.Lbl_SendContactnumber);
                    }
                    else
                    {
                        holder.TxtLastMessages.Text = Methods.FunString.DecodeString(Methods.FunString.SubStringCutOf(item.LastMessage.Text, 30));
                    }
                }

                //last seen time  
                holder.TxtTimestamp.Text = Methods.Time.TimeAgo(int.Parse(item.LastseenUnixTime), true);

                //Online Or offline
                if (item.Lastseen == "on" && OnlineUsers.Equals(true))
                {
                    holder.TxtTimestamp.Text = ActivityContext.GetText(Resource.String.Lbl_Online);
                    holder.ImageLastseen.SetImageResource(Resource.Drawable.Green_Online);


                    if (AppSettings.ShowOnlineOfflineMessage)
                    {
                        var data = ListOnline.Contains(item.Name);
                        if (data == false)
                        {
                            ListOnline.Add(item.Name);

                            Toast toast = Toast.MakeText(ActivityContext, item.Name + " " + ActivityContext.GetText(Resource.String.Lbl_Online), ToastLength.Short);
                            toast.SetGravity(GravityFlags.Center, 0, 0);
                            toast.Show();
                        }
                    }
                }
                else
                {
                    holder.ImageLastseen.SetImageResource(Resource.Drawable.Grey_Offline);
                }

                //Check read message
                if (item.LastMessage.FromId != null && (item.LastMessage.ToId != null && (item.LastMessage.ToId != UserDetails.UserId && item.LastMessage.FromId == UserDetails.UserId)))
                {
                    if (item.LastMessage.Seen == "0")
                    {
                        holder.IconCheckCountMessages.Visibility = ViewStates.Invisible;
                        holder.TxtUsername.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                        holder.TxtLastMessages.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                    }
                    else
                    {
                        holder.IconCheckCountMessages.Visibility = ViewStates.Visible;
                        holder.TxtUsername.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                        holder.TxtLastMessages.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                        Glide.With(ActivityContext).Load(Resource.Drawable.ic_tick_vector).Into(holder.IconCheckCountMessages);
                    }
                }
                else if (item.LastMessage.FromId != null && (item.LastMessage.ToId != null && (item.LastMessage.ToId == UserDetails.UserId && item.LastMessage.FromId != UserDetails.UserId)))
                {
                    if (item.LastMessage.Seen == "0")
                    {
                        holder.TxtUsername.SetTypeface(Typeface.Default, TypefaceStyle.Bold);
                        holder.TxtLastMessages.SetTypeface(Typeface.Default, TypefaceStyle.Bold);

                        holder.IconCheckCountMessages.Visibility = ViewStates.Visible;
                        var drawable = new TextDrawable.Builder().BeginConfig().FontSize(25).EndConfig().BuildRound("N", Color.ParseColor(AppSettings.MainColor));
                        holder.IconCheckCountMessages.SetImageDrawable(drawable);
                        holder.IconCheckCountMessages.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        holder.IconCheckCountMessages.Visibility = ViewStates.Invisible;
                        holder.TxtUsername.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                        holder.TxtLastMessages.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override int ItemCount => MLastMessagesUser?.Count ?? 0;

        public GetUsersListObject.User GetItem(int position)
        {
            return MLastMessagesUser[position];
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int GetItemViewType(int position)
        {
            return position;
        }

        public override void OnAttachedToRecyclerView(RecyclerView recyclerView)
        {

            try
            {
                recyclerView.AddOnScrollListener(new MySwipeItemScroll(this));
                base.OnAttachedToRecyclerView(recyclerView);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        void MoreOnClick(Holders.LastMessagesClickEventArgs args) => MoreItemClick?.Invoke(this, args);
        void CallOnClick(Holders.LastMessagesClickEventArgs args) => CallItemClick?.Invoke(this, args);
        void DeleteOnClick(Holders.LastMessagesClickEventArgs args) => DeleteItemClick?.Invoke(this, args);
        void OnClick(Holders.LastMessagesClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(Holders.LastMessagesClickEventArgs args) => ItemLongClick?.Invoke(this, args);

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = MLastMessagesUser[p0];
                if (item == null)
                    return d;
                else
                {
                    var image = !string.IsNullOrEmpty(item.OldAvatar) ? item.OldAvatar : item.Avatar ?? item.Cover;
                    if (!string.IsNullOrEmpty(image))
                        d.Add(image);
                    return d;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var d = new List<string>();
                return d;
            }
        }

        public RequestBuilder GetPreloadRequestBuilder(Object p0)
        {
            return GlideImageLoader.GetPreLoadRequestBuilder(ActivityContext, p0.ToString(), ImageStyle.CircleCrop);
        }

        #region SwipeItem

        public GetUsersListObject.User ItemsSwiped;

        private class MySwipeItemScroll : RecyclerView.OnScrollListener
        {
            private readonly LastMessagesAdapter MAdapter;
            public MySwipeItemScroll(LastMessagesAdapter messagesAdapter)
            {
                try
                {
                    MAdapter = messagesAdapter;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
            {
                try
                {
                    if (MAdapter.ItemsSwiped != null)
                    {
                        var list = MAdapter.MLastMessagesUser.Where(w => w.Swiped).ToList();
                        if (list.Count > 0)
                        {
                            MAdapter.ItemsSwiped.Swiped = false;
                            MAdapter.ItemsSwiped = null;

                            foreach (var user in list)
                            {
                                int indexSwiped = MAdapter.MLastMessagesUser.IndexOf(user);
                                if (indexSwiped > -1)
                                {
                                    user.Swiped = false;
                                    MAdapter.NotifyItemChanged(indexSwiped);
                                }
                            }
                        }
                    }

                    base.OnScrollStateChanged(recyclerView, newState);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public void OnItemDismiss(int position)
        {
            try
            {
                var item = MLastMessagesUser[position];
                // handle when double swipe
                if (item != null && item.Swiped)
                {
                    ItemsSwiped = null;
                    item.Swiped = false;
                    NotifyItemChanged(position);
                    return;
                }

                if (item == null) return;

                foreach (var user in MLastMessagesUser)
                {
                    int indexSwiped = MLastMessagesUser.IndexOf(user);
                    if (indexSwiped <= -1) continue;
                    user.Swiped = false;
                    NotifyItemChanged(indexSwiped);
                }

                item.Swiped = true;
                ItemsSwiped = item;
                NotifyItemChanged(position);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

    }


}