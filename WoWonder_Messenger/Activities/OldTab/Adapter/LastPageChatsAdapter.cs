using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Java.Util;
using Refractored.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Utils;
using WoWonderClient.Classes.Global;
using Exception = System.Exception;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace WoWonder.Activities.OldTab.Adapter
{
    public class LastPageChatsAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {

        public event EventHandler<LastPageChatsAdapterClickEventArgs> ItemClick;
        public event EventHandler<LastPageChatsAdapterClickEventArgs> ItemLongClick;

        private readonly Activity ActivityContext;
        public ObservableCollection<PageClass> LastPageList = new ObservableCollection<PageClass>();

        public LastPageChatsAdapter(Activity activity)
        {
            try
            {
                HasStableIds = true;
                ActivityContext = activity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override int ItemCount => LastPageList?.Count ?? 0;

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_HContact_view
                var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_HContact_view, parent, false);
                var vh = new LastPageChatsAdapterViewHolder(itemView, Click, LongClick);
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
                if (viewHolder is LastPageChatsAdapterViewHolder holder)
                {
                    var item = LastPageList[position];
                    if (item != null)
                    {
                        GlideImageLoader.LoadImage(ActivityContext, item.Avatar, holder.Image, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                        holder.Name.Text = Methods.FunString.SubStringCutOf(Methods.FunString.DecodeString(item.PageName), 25);

                        if (item.LastMessage.Stickers != null)
                            item.LastMessage.Stickers = item.LastMessage.Stickers.Replace(".mp4", ".gif");

                        //If message contains Media files 
                        if (item.LastMessage.Media.Contains("image"))
                        {
                            holder.About.Text = ActivityContext.GetText(Resource.String.Lbl_SendImageFile);
                        }
                        else if (item.LastMessage.Media.Contains("video"))
                        {
                            holder.About.Text = ActivityContext.GetText(Resource.String.Lbl_SendVideoFile);
                        }
                        else if (item.LastMessage.Media.Contains("sticker"))
                        {
                            holder.About.Text = ActivityContext.GetText(Resource.String.Lbl_SendStickerFile);
                        }
                        else if (item.LastMessage.Media.Contains("sounds"))
                        {
                            holder.About.Text = ActivityContext.GetText(Resource.String.Lbl_SendAudioFile);
                        }
                        else if (item.LastMessage.Media.Contains("file"))
                        {
                            holder.About.Text = ActivityContext.GetText(Resource.String.Lbl_SendFile);
                        }
                        else if (item.LastMessage?.Stickers != null && item.LastMessage.Stickers.Contains(".gif"))
                        {
                            holder.About.Text = ActivityContext.GetText(Resource.String.Lbl_SendGifFile);
                        }
                        else if (!string.IsNullOrEmpty(item.LastMessage.ProductId) && item.LastMessage.ProductId != "0")
                        {
                            holder.About.Text = ActivityContext.GetText(Resource.String.Lbl_SendProductFile);
                        }
                        else
                        {
                            if (item.LastMessage.Text.Contains("http"))
                            {
                                holder.About.Text = Methods.FunString.SubStringCutOf(item.LastMessage.Text, 30);
                            }
                            else if (item.LastMessage.Text.Contains("{&quot;Key&quot;") || item.LastMessage.Text.Contains("{key:^qu") || item.LastMessage.Text.Contains("{^key:^qu") || item.LastMessage.Text.Contains("{key:"))
                            {
                                holder.About.Text = ActivityContext.GetText(Resource.String.Lbl_SendContactnumber);

                            }
                            else
                            {
                                holder.About.Text = Methods.FunString.DecodeString(Methods.FunString.SubStringCutOf(item.LastMessage.Text, 30));
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public PageClass GetItem(int position)
        {
            return LastPageList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return int.Parse(LastPageList[position].UserId);
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

        private void Click(LastPageChatsAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        private void LongClick(LastPageChatsAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = LastPageList[p0];
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

    public class LastPageChatsAdapterViewHolder : RecyclerView.ViewHolder
    {
        public LastPageChatsAdapterViewHolder(View itemView, Action<LastPageChatsAdapterClickEventArgs> clickListener, Action<LastPageChatsAdapterClickEventArgs> longClickListener) : base(itemView)
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
                itemView.Click += (sender, e) => clickListener(new LastPageChatsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new LastPageChatsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });

                Button.Visibility = ViewStates.Gone;
                ImageLastseen.Visibility = ViewStates.Gone;
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

    public class LastPageChatsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }

}