using Android.Animation;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.Animations;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Bumptech.Glide.Request.Target;
using Bumptech.Glide.Request.Transition;
using Com.Luseen.Autolinklibrary;
using Java.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;
using WoWonder.Activities.DefaultUser;
using WoWonder.Adapters;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Fonts;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.Library.MusicBar;
using WoWonderClient.Classes.Message;
using Console = System.Console;
using MessageData = WoWonder.Helpers.Model.MessageDataExtra;
using Object = Java.Lang.Object;
using Priority = Bumptech.Glide.Priority;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.PageChat.Adapter
{
    public class PageMessagesAdapter : RecyclerView.Adapter, MusicBar.IOnMusicBarAnimationChangeListener, MusicBar.IOnMusicBarProgressChangeListener, ValueAnimator.IAnimatorUpdateListener
    {
        public event EventHandler<Holders.MesClickEventArgs> ItemClick;
        public event EventHandler<Holders.MesClickEventArgs> ItemLongClick;

        public ObservableCollection<AdapterModelsClassPage> DifferList = new ObservableCollection<AdapterModelsClassPage>();
        private readonly Activity MainActivity;
        private readonly RequestOptions Options;
        private readonly string PageId;

        public PageMessagesAdapter(Activity activity, string groupId)
        {
            try
            {
                HasStableIds = true;
                MainActivity = activity;
                PageId = groupId;
                DifferList = new ObservableCollection<AdapterModelsClassPage>();

                Options = new RequestOptions().Apply(RequestOptions.CircleCropTransform()
                    .CenterCrop()
                    .SetPriority(Priority.High).Override(200)
                    .SetUseAnimationPool(false).SetDiskCacheStrategy(DiskCacheStrategy.All)
                    .Error(Resource.Drawable.ImagePlacholder)
                    .Placeholder(Resource.Drawable.ImagePlacholder));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position, IList<Object> payloads)
        {
            try
            {
                if (payloads.Count > 0)
                {
                    var item = DifferList[position];
                    if (item == null)
                        return;

                    switch (payloads[0].ToString())
                    {
                        case "WithoutBlobImage":
                            {
                                if (viewHolder is Holders.ImageViewHolder holder)
                                    LoadImageOfChatItem(holder, item.MesData);
                                break;
                            }
                        case "WithoutBlobVideo":
                            {
                                if (viewHolder is Holders.VideoViewHolder holder)
                                    LoadVideoOfChatItem(holder, item.MesData, position);
                                break;
                            }
                        case "WithoutBlobAudio":
                            {
                                if (AppSettings.ShowMusicBar)
                                {
                                    Holders.MusicBarViewHolder holder = viewHolder as Holders.MusicBarViewHolder;
                                    LoadAudioBarOfChatItem(holder, position, item.MesData);
                                    break;
                                }
                                else
                                {
                                    Holders.SoundViewHolder holder = viewHolder as Holders.SoundViewHolder;
                                    LoadAudioOfChatItem(holder, item.MesData, position);
                                    break;
                                }
                            }
                        case "WithoutBlobFile":
                            {
                                if (viewHolder is Holders.FileViewHolder holder)
                                    LoadFileOfChatItem(holder, item.MesData);
                                break;
                            }
                        case "WithoutBlobGIF":
                            {
                                if (viewHolder is Holders.GifViewHolder holder)
                                    LoadGifOfChatItem(holder, item.MesData);
                                break;
                            }
                        default:
                            base.OnBindViewHolder(viewHolder, position, payloads);
                            break;
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
                switch (viewType)
                {
                    case (int)MessageModelType.RightProduct:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_Products, parent, false);
                            Holders.ProductViewHolder viewHolder = new Holders.ProductViewHolder(row, OnClick, OnLongClick, false);
                            return viewHolder;
                        }
                    case (int)MessageModelType.LeftProduct:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_Products, parent, false);
                            Holders.ProductViewHolder viewHolder = new Holders.ProductViewHolder(row, OnClick, OnLongClick, false);
                            return viewHolder;
                        }
                    case (int)MessageModelType.RightGif:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_gif, parent, false);
                            Holders.GifViewHolder viewHolder = new Holders.GifViewHolder(row, OnClick, OnLongClick, false);
                            return viewHolder;
                        }
                    case (int)MessageModelType.LeftGif:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_gif, parent, false);
                            Holders.GifViewHolder viewHolder = new Holders.GifViewHolder(row, OnClick, OnLongClick, false);
                            return viewHolder;
                        }
                    case (int)MessageModelType.RightText:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_view, parent, false);
                            Holders.TextViewHolder textViewHolder = new Holders.TextViewHolder(row, OnClick, OnLongClick, false);
                            return textViewHolder;
                        }
                    case (int)MessageModelType.LeftText:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_view, parent, false);
                            Holders.TextViewHolder textViewHolder = new Holders.TextViewHolder(row, OnClick, OnLongClick, false);
                            return textViewHolder;
                        }
                    case (int)MessageModelType.RightImage:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_image, parent, false);
                            Holders.ImageViewHolder imageViewHolder = new Holders.ImageViewHolder(row, OnClick, OnLongClick, false);
                            return imageViewHolder;
                        }
                    case (int)MessageModelType.LeftImage:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_image, parent, false);
                            Holders.ImageViewHolder imageViewHolder = new Holders.ImageViewHolder(row, OnClick, OnLongClick, false);
                            return imageViewHolder;
                        }
                    case (int)MessageModelType.RightAudio:
                        {
                            if (AppSettings.ShowMusicBar)
                            {
                                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_AudioBar, parent, false);
                                Holders.MusicBarViewHolder soundViewHolder = new Holders.MusicBarViewHolder(row, OnClick, OnLongClick, false);
                                return soundViewHolder;
                            }
                            else
                            {
                                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_Audio, parent, false);
                                Holders.SoundViewHolder soundViewHolder = new Holders.SoundViewHolder(row, OnClick, OnLongClick, false);
                                return soundViewHolder;
                            }
                        }
                    case (int)MessageModelType.LeftAudio:
                        {
                            if (AppSettings.ShowMusicBar)
                            {
                                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_AudioBar, parent, false);
                                Holders.MusicBarViewHolder soundViewHolder = new Holders.MusicBarViewHolder(row, OnClick, OnLongClick, false);
                                return soundViewHolder;
                            }
                            else
                            {
                                View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_Audio, parent, false);
                                Holders.SoundViewHolder soundViewHolder = new Holders.SoundViewHolder(row, OnClick, OnLongClick, false);
                                return soundViewHolder;
                            }
                        }
                    case (int)MessageModelType.RightContact:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_Contact, parent, false);
                            Holders.ContactViewHolder contactViewHolder = new Holders.ContactViewHolder(row, OnClick, OnLongClick, false);
                            return contactViewHolder;
                        }
                    case (int)MessageModelType.LeftContact:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_Contact, parent, false);
                            Holders.ContactViewHolder contactViewHolder = new Holders.ContactViewHolder(row, OnClick, OnLongClick, false);
                            return contactViewHolder;
                        }
                    case (int)MessageModelType.RightVideo:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_Video, parent, false);
                            Holders.VideoViewHolder videoViewHolder = new Holders.VideoViewHolder(row, OnClick, OnLongClick, false);
                            return videoViewHolder;
                        }
                    case (int)MessageModelType.LeftVideo:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_Video, parent, false);
                            Holders.VideoViewHolder videoViewHolder = new Holders.VideoViewHolder(row, OnClick, OnLongClick, false);
                            return videoViewHolder;
                        }
                    case (int)MessageModelType.RightSticker:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_sticker, parent, false);
                            Holders.StickerViewHolder stickerViewHolder = new Holders.StickerViewHolder(row, OnClick, OnLongClick, false);
                            return stickerViewHolder;
                        }
                    case (int)MessageModelType.LeftSticker:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_sticker, parent, false);
                            Holders.StickerViewHolder stickerViewHolder = new Holders.StickerViewHolder(row, OnClick, OnLongClick, false);
                            return stickerViewHolder;
                        }
                    case (int)MessageModelType.RightFile:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Right_MS_file, parent, false);
                            Holders.FileViewHolder viewHolder = new Holders.FileViewHolder(row, OnClick, OnLongClick, false);
                            return viewHolder;
                        }
                    case (int)MessageModelType.LeftFile:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_file, parent, false);
                            Holders.FileViewHolder viewHolder = new Holders.FileViewHolder(row, OnClick, OnLongClick, false);
                            return viewHolder;
                        }
                    default:
                        {
                            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Left_MS_view, parent, false);
                            Holders.NotSupportedViewHolder viewHolder = new Holders.NotSupportedViewHolder(row);
                            return viewHolder;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder vh, int position)
        {
            try
            {
                var item = DifferList[position];
                if (item == null)
                    return;

                var itemViewType = vh.ItemViewType;
                switch (itemViewType)
                {
                    case (int)MessageModelType.RightProduct:
                    case (int)MessageModelType.LeftProduct:
                        {

                            break;
                        }
                    case (int)MessageModelType.RightGif:
                    case (int)MessageModelType.LeftGif:
                        {
                            Holders.GifViewHolder holder = vh as Holders.GifViewHolder;
                            LoadGifOfChatItem(holder, item.MesData);
                            break;
                        }
                    case (int)MessageModelType.RightText:
                    case (int)MessageModelType.LeftText:
                        {
                            Holders.TextViewHolder holder = vh as Holders.TextViewHolder;
                            LoadTextOfchatItem(holder, item.MesData);
                            break;
                        }
                    case (int)MessageModelType.RightImage:
                    case (int)MessageModelType.LeftImage:
                        {
                            Holders.ImageViewHolder holder = vh as Holders.ImageViewHolder;
                            LoadImageOfChatItem(holder, item.MesData);
                            break;
                        }
                    case (int)MessageModelType.RightAudio:
                    case (int)MessageModelType.LeftAudio:
                        {
                            if (AppSettings.ShowMusicBar)
                            {
                                Holders.MusicBarViewHolder holder = vh as Holders.MusicBarViewHolder;
                                LoadAudioBarOfChatItem(holder, position, item.MesData);
                                break;
                            }
                            else
                            {
                                Holders.SoundViewHolder holder = vh as Holders.SoundViewHolder;
                                LoadAudioOfChatItem(holder, item.MesData, position);
                                break;
                            }
                        }
                    case (int)MessageModelType.RightContact:
                    case (int)MessageModelType.LeftContact:
                        {
                            Holders.ContactViewHolder holder = vh as Holders.ContactViewHolder;
                            LoadContactOfChatItem(holder, item.MesData);
                            break;
                        }
                    case (int)MessageModelType.RightVideo:
                    case (int)MessageModelType.LeftVideo:
                        {
                            Holders.VideoViewHolder holder = vh as Holders.VideoViewHolder;
                            LoadVideoOfChatItem(holder, item.MesData, position);
                            break;
                        }
                    case (int)MessageModelType.RightSticker:
                    case (int)MessageModelType.LeftSticker:
                        {
                            Holders.StickerViewHolder holder = vh as Holders.StickerViewHolder;
                            LoadStickerOfChatItem(holder, item.MesData);
                            break;
                        }
                    case (int)MessageModelType.RightFile:
                    case (int)MessageModelType.LeftFile:
                        {
                            Holders.FileViewHolder holder = vh as Holders.FileViewHolder;
                            LoadFileOfChatItem(holder, item.MesData);
                            break;
                        }
                    default:
                        {
                            if (!string.IsNullOrEmpty(item.MesData.Text))
                            {
                                if (vh is Holders.TextViewHolder holderText)
                                {
                                    LoadTextOfchatItem(holderText, item.MesData);
                                }
                            }
                            else
                            {
                                if (vh is Holders.NotSupportedViewHolder holder)
                                    holder.AutoLinkNotsupportedView.Text = MainActivity.GetText(Resource.String.Lbl_TextChatNotSupported);
                            }

                            break;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //==============================

        #region Function Load Message

        private void LoadTextOfchatItem(Holders.TextViewHolder holder, MessageData message)
        {
            try
            {
                if (holder.UserName != null) holder.UserName.Text = WoWonderTools.GetNameFinal(message.UserData);

                holder.Time.Text = message.TimeText;
                holder.Time.Visibility = message.ShowTimeText ? ViewStates.Visible : ViewStates.Gone;

                if (message.Position == "left")
                {
                    holder.AutoLinkTextView.AddAutoLinkMode(AutoLinkMode.ModePhone, AutoLinkMode.ModeEmail, AutoLinkMode.ModeHashtag, AutoLinkMode.ModeUrl, AutoLinkMode.ModeMention);
                    holder.AutoLinkTextView.SetPhoneModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.left_ModePhone_color));
                    holder.AutoLinkTextView.SetEmailModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.left_ModeEmail_color));
                    holder.AutoLinkTextView.SetHashtagModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.left_ModeHashtag_color));
                    holder.AutoLinkTextView.SetUrlModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.left_ModeUrl_color));
                    holder.AutoLinkTextView.SetMentionModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.left_ModeMention_color));
                    holder.AutoLinkTextView.AutoLinkOnClick += AutoLinkTextViewOnAutoLinkOnClick;
                }
                else
                {
                    holder.AutoLinkTextView.AddAutoLinkMode(AutoLinkMode.ModePhone, AutoLinkMode.ModeEmail, AutoLinkMode.ModeHashtag, AutoLinkMode.ModeUrl, AutoLinkMode.ModeMention);
                    holder.AutoLinkTextView.SetPhoneModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.right_ModePhone_color));
                    holder.AutoLinkTextView.SetEmailModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.right_ModeEmail_color));
                    holder.AutoLinkTextView.SetHashtagModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.right_ModeHashtag_color));
                    holder.AutoLinkTextView.SetUrlModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.right_ModeUrl_color));
                    holder.AutoLinkTextView.SetMentionModeColor(ContextCompat.GetColor(MainActivity, Resource.Color.right_ModeMention_color));
                    holder.AutoLinkTextView.AutoLinkOnClick += AutoLinkTextViewOnAutoLinkOnClick;
                }

                holder.AutoLinkTextView.SetAutoLinkText(message.Text);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void AutoLinkTextViewOnAutoLinkOnClick(object sender1, AutoLinkOnClickEventArgs autoLinkOnClickEventArgs)
        {
            try
            {
                var typetext = Methods.FunString.Check_Regex(autoLinkOnClickEventArgs.P1);
                if (typetext == "Email")
                {
                    Methods.App.SendEmail(MainActivity, autoLinkOnClickEventArgs.P1);
                }
                else if (typetext == "Website")
                {
                    var url = autoLinkOnClickEventArgs.P1;
                    if (!autoLinkOnClickEventArgs.P1.Contains("http"))
                    {
                        url = "http://" + autoLinkOnClickEventArgs.P1;
                    }

                    Methods.App.OpenWebsiteUrl(MainActivity, url);
                }
                else if (typetext == "Hashtag")
                {

                }
                else if (typetext == "Mention")
                {
                    var intent = new Intent(MainActivity, typeof(SearchActivity));
                    intent.PutExtra("Key", autoLinkOnClickEventArgs.P1.Replace("@", ""));
                    MainActivity.StartActivity(intent);
                }
                else if (typetext == "Number")
                {
                    Methods.App.SaveContacts(MainActivity, autoLinkOnClickEventArgs.P1, "", "2");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadImageOfChatItem(Holders.ImageViewHolder holder, MessageData message)
        {
            try
            {
                if (holder.UserName != null) holder.UserName.Text = WoWonderTools.GetNameFinal(message.UserData);

                string imageUrl = message.Media;

                holder.Time.Text = message.TimeText;

                if (imageUrl.Contains("http://") || imageUrl.Contains("https://"))
                {
                    GlideImageLoader.LoadImage(MainActivity, imageUrl, holder.ImageView, ImageStyle.RoundedCrop, ImagePlaceholders.Drawable);
                }
                else
                {
                    var file = Uri.FromFile(new File(imageUrl));
                    Glide.With(MainActivity).Load(file.Path).Apply(GlideImageLoader.GetRequestOptions(ImageStyle.RoundedCrop, ImagePlaceholders.Drawable)).Into(holder.ImageView);
                }

                holder.LoadingProgressview.Indeterminate = false;
                holder.LoadingProgressview.Visibility = ViewStates.Gone;

                if (!holder.ImageView.HasOnClickListeners)
                {
                    holder.ImageView.Click += (sender, args) =>
                    {
                        try
                        {
                            string imageFile = Methods.MultiMedia.CheckFileIfExits(imageUrl);
                            if (imageFile != "File Dont Exists")
                            {
                                File file2 = new File(imageUrl);
                                var photoUri = FileProvider.GetUriForFile(MainActivity, MainActivity.PackageName + ".fileprovider", file2);
                                Intent intent = new Intent();
                                intent.SetAction(Intent.ActionView);
                                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                                intent.SetDataAndType(photoUri, "image/*");
                                MainActivity.StartActivity(intent);
                            }
                            else
                            {
                                Intent intent = new Intent(Intent.ActionView, Uri.Parse(imageUrl));
                                MainActivity.StartActivity(intent);

                                //Toast.MakeText(MainActivity, MainActivity.GetText(Resource.String.Lbl_Something_went_wrong), ToastLength.Long).Show();
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private bool MSeekBarIsTracking;
        private ValueAnimator MValueAnimator;
        private MessageData MusicBarViewHolder;
        private int PositionSound;
        private void LoadAudioBarOfChatItem(Holders.MusicBarViewHolder musicBarViewHolder, int position, MessageData message)
        {
            try
            {
                MusicBarViewHolder = message;
                MusicBarViewHolder.MusicBarViewHolder = musicBarViewHolder;
                message.MusicBarViewHolder = musicBarViewHolder;

                if (message.TimeText == MainActivity.GetText(Resource.String.Lbl_Uploading))
                {
                    musicBarViewHolder.LoadingProgressview.Visibility = ViewStates.Visible;
                    musicBarViewHolder.PlayButton.Visibility = ViewStates.Gone;
                }
                else
                {
                    musicBarViewHolder.LoadingProgressview.Visibility = ViewStates.Gone;
                    musicBarViewHolder.PlayButton.Visibility = ViewStates.Visible;
                }

                musicBarViewHolder.MsgTimeTextView.Text = message.TimeText;

                var fileName = message.Media.Split('/').Last();

                var mediaFile = WoWonderTools.GetFile(PageId, Methods.Path.FolderDcimSound, fileName, message.Media);

                var durationOfSound = Methods.AudioRecorderAndPlayer.Get_MediaFileDuration(mediaFile);
                if (string.IsNullOrEmpty(message.MediaDuration))
                    musicBarViewHolder.DurationTextView.Text = Methods.AudioRecorderAndPlayer.GetTimeString(Methods.AudioRecorderAndPlayer.Get_MediaFileDuration(mediaFile));
                else
                    musicBarViewHolder.DurationTextView.Text = message.MediaDuration;

                musicBarViewHolder.LoadingProgressview.Visibility = ViewStates.Gone;
                musicBarViewHolder.PlayButton.Visibility = ViewStates.Visible;

                if (mediaFile.Contains("http"))
                    mediaFile = WoWonderTools.GetFile(PageId, Methods.Path.FolderDcimSound, fileName, message.Media);

                musicBarViewHolder.FixedMusicBar.LoadFrom(mediaFile, (int)durationOfSound);
                musicBarViewHolder.FixedMusicBar.Show();

                if (!musicBarViewHolder.PlayButton.HasOnClickListeners)
                {
                    musicBarViewHolder.PlayButton.Click += (o, args) =>
                    {
                        try
                        {
                            if (PositionSound != position)
                            {
                                if (message.MediaPlayer != null)
                                {
                                    message.MediaPlayer.Stop();
                                    message.MediaPlayer.Reset();
                                }
                                message.MediaPlayer = null;
                                message.MediaTimer = null;
                            }

                            if (message.MediaPlayer == null)
                            {
                                PositionSound = position;
                                message.MediaPlayer = new MediaPlayer();
                                message.MediaPlayer.SetAudioAttributes(new AudioAttributes.Builder().SetUsage(AudioUsageKind.Media).SetContentType(AudioContentType.Music).Build());

                                message.MediaPlayer.Completion += (sender, e) =>
                                {
                                    try
                                    {
                                        musicBarViewHolder.PlayButton.Tag = "Play";
                                        musicBarViewHolder.PlayButton.SetImageResource(message.ModelType == MessageModelType.LeftAudio ? Resource.Drawable.ic_play_dark_arrow : Resource.Drawable.ic_play_arrow);

                                        message.MediaIsPlaying = false;

                                        message.MediaPlayer.Stop();
                                        message.MediaPlayer.Reset();
                                        message.MediaPlayer = null;

                                        message.MediaTimer.Enabled = false;
                                        message.MediaTimer.Stop();
                                        message.MediaTimer = null;

                                        musicBarViewHolder.FixedMusicBar.StopAutoProgress();
                                    }
                                    catch (Exception exception)
                                    {
                                        Console.WriteLine(exception);
                                    }
                                };

                                message.MediaPlayer.Prepared += (s, ee) =>
                                {
                                    try
                                    {
                                        message.MediaIsPlaying = true;
                                        musicBarViewHolder.PlayButton.Tag = "Pause";
                                        musicBarViewHolder.PlayButton.SetImageResource(message.ModelType == MessageModelType.LeftAudio ? Resource.Drawable.ic_media_pause_dark : Resource.Drawable.ic_media_pause_light);

                                        if (message.MediaTimer == null)
                                            message.MediaTimer = new Timer { Interval = 1000 };

                                        message.MediaPlayer.Start();

                                        var durationOfSound = message.MediaPlayer.Duration;

                                        //change bar width
                                        musicBarViewHolder.FixedMusicBar.SetBarWidth(2);

                                        //change Space Between Bars
                                        musicBarViewHolder.FixedMusicBar.SetSpaceBetweenBar(2);

                                        if (mediaFile.Contains("http"))
                                            mediaFile = WoWonderTools.GetFile(PageId, Methods.Path.FolderDcimSound, fileName, message.Media);

                                        musicBarViewHolder.FixedMusicBar.LoadFrom(mediaFile, durationOfSound);

                                        musicBarViewHolder.FixedMusicBar.SetAnimationChangeListener(this);
                                        musicBarViewHolder.FixedMusicBar.SetProgressChangeListener(this);
                                        InitValueAnimator(1.0f, 0, durationOfSound);
                                        musicBarViewHolder.FixedMusicBar.Show();

                                        message.MediaTimer.Elapsed += (sender, eventArgs) =>
                                        {
                                            MainActivity.RunOnUiThread(() =>
                                            {
                                                try
                                                {
                                                    if (message.MediaTimer.Enabled)
                                                    {
                                                        if (message.MediaPlayer.CurrentPosition <= message.MediaPlayer.Duration)
                                                        {
                                                            musicBarViewHolder.DurationTextView.Text = Methods.AudioRecorderAndPlayer.GetTimeString(message.MediaPlayer.CurrentPosition);
                                                        }
                                                        else
                                                        {
                                                            musicBarViewHolder.DurationTextView.Text = Methods.AudioRecorderAndPlayer.GetTimeString(message.MediaPlayer.Duration);

                                                            musicBarViewHolder.PlayButton.Tag = "Play";
                                                            musicBarViewHolder.PlayButton.SetImageResource(message.ModelType == MessageModelType.LeftAudio ? Resource.Drawable.ic_play_dark_arrow : Resource.Drawable.ic_play_arrow);
                                                        }
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e);
                                                    musicBarViewHolder.PlayButton.Tag = "Play";
                                                }
                                            });
                                        };
                                        message.MediaTimer.Start();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                };

                                if (mediaFile.Contains("http"))
                                {
                                    message.MediaPlayer.SetDataSource(MainActivity, Uri.Parse(mediaFile));
                                    message.MediaPlayer.PrepareAsync();
                                }
                                else
                                {
                                    File file2 = new File(mediaFile);
                                    var photoUri = FileProvider.GetUriForFile(MainActivity, MainActivity.PackageName + ".fileprovider", file2);

                                    message.MediaPlayer.SetDataSource(MainActivity, photoUri);
                                    message.MediaPlayer.Prepare();
                                }

                                MusicBarViewHolder = message;
                                MusicBarViewHolder.MusicBarViewHolder = musicBarViewHolder;
                                message.MusicBarViewHolder = musicBarViewHolder;
                            }
                            else
                            {
                                if (musicBarViewHolder.PlayButton.Tag.ToString() == "Play")
                                {
                                    musicBarViewHolder.PlayButton.Tag = "Pause";
                                    musicBarViewHolder.PlayButton.SetImageResource(message.ModelType == MessageModelType.LeftAudio ? Resource.Drawable.ic_media_pause_dark : Resource.Drawable.ic_media_pause_light);

                                    message.MediaIsPlaying = true;
                                    message.MediaPlayer?.Start();

                                    musicBarViewHolder.FixedMusicBar.Show();

                                    InitValueAnimator(1.0f, musicBarViewHolder.FixedMusicBar.GetPosition(), message.MediaPlayer.Duration);

                                    if (message.MediaTimer != null)
                                    {
                                        message.MediaTimer.Enabled = true;
                                        message.MediaTimer.Start();
                                    }
                                }
                                else if (musicBarViewHolder.PlayButton.Tag.ToString() == "Pause")
                                {
                                    musicBarViewHolder.PlayButton.Tag = "Play";
                                    musicBarViewHolder.PlayButton.SetImageResource(message.ModelType == MessageModelType.LeftAudio ? Resource.Drawable.ic_play_dark_arrow : Resource.Drawable.ic_play_arrow);

                                    message.MediaIsPlaying = false;
                                    message.MediaPlayer?.Pause();

                                    //stop auto progress animation
                                    musicBarViewHolder.FixedMusicBar.StopAutoProgress();

                                    if (message.MediaTimer != null)
                                    {
                                        message.MediaTimer.Enabled = false;
                                        message.MediaTimer.Stop();
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    };
                }

                MusicBarViewHolder = message;
                MusicBarViewHolder.MusicBarViewHolder = musicBarViewHolder;
                message.MusicBarViewHolder = musicBarViewHolder;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadAudioOfChatItem(Holders.SoundViewHolder soundViewHolder, MessageData message, int position)
        {
            try
            {
                if (soundViewHolder.UserName != null) soundViewHolder.UserName.Text = WoWonderTools.GetNameFinal(message.UserData);

                if (message.SoundViewHolder == null)
                    message.SoundViewHolder = soundViewHolder;

                if (message.TimeText == MainActivity.GetText(Resource.String.Lbl_Uploading))
                {
                    soundViewHolder.LoadingProgressview.Visibility = ViewStates.Visible;
                    soundViewHolder.PlayButton.Visibility = ViewStates.Gone;
                }
                else
                {
                    soundViewHolder.LoadingProgressview.Visibility = ViewStates.Gone;
                    soundViewHolder.PlayButton.Visibility = ViewStates.Visible;
                }

                soundViewHolder.MsgTimeTextView.Text = message.TimeText;

                var fileName = message.Media.Split('/').Last();

                var mediaFile = WoWonderTools.GetFile(PageId, Methods.Path.FolderDcimSound, fileName, message.Media);
                if (string.IsNullOrEmpty(message.MediaDuration))
                    soundViewHolder.DurationTextView.Text = Methods.AudioRecorderAndPlayer.GetTimeString(Methods.AudioRecorderAndPlayer.Get_MediaFileDuration(mediaFile));
                else
                    soundViewHolder.DurationTextView.Text = message.MediaDuration;

                soundViewHolder.LoadingProgressview.Visibility = ViewStates.Gone;
                soundViewHolder.PlayButton.Visibility = ViewStates.Visible;

                if (!soundViewHolder.PlayButton.HasOnClickListeners)
                {
                    soundViewHolder.PlayButton.Click += (o, args) =>
                    {
                        try
                        {
                            if (PositionSound != position)
                            {
                                var list = DifferList.Where(a => a.TypeView == MessageModelType.LeftAudio || a.TypeView == MessageModelType.RightAudio && a.MesData.MediaPlayer != null).ToList();
                                if (list.Count > 0)
                                {
                                    foreach (var item in list)
                                    {
                                        if (item.MesData.MediaPlayer != null)
                                        {
                                            item.MesData.MediaPlayer.Stop();
                                            item.MesData.MediaPlayer.Reset();
                                        }
                                        item.MesData.MediaPlayer = null;
                                        item.MesData.MediaTimer = null;

                                        item.MesData.MediaPlayer?.Release();
                                        item.MesData.MediaPlayer = null;
                                    }
                                }
                            }

                            if (mediaFile.Contains("http"))
                                mediaFile = WoWonderTools.GetFile(PageId, Methods.Path.FolderDcimSound, fileName, message.Media);

                            if (message.MediaPlayer == null)
                            {
                                PositionSound = position;
                                message.MediaPlayer = new MediaPlayer();
                                message.MediaPlayer.SetAudioAttributes(new AudioAttributes.Builder().SetUsage(AudioUsageKind.Media).SetContentType(AudioContentType.Music).Build());

                                message.MediaPlayer.Completion += (sender, e) =>
                                {
                                    try
                                    {
                                        soundViewHolder.PlayButton.Tag = "Play";
                                        soundViewHolder.PlayButton.SetImageResource(message.ModelType == MessageModelType.LeftAudio ? Resource.Drawable.ic_play_dark_arrow : Resource.Drawable.ic_play_arrow);

                                        message.MediaIsPlaying = false;

                                        message.MediaPlayer.Stop();
                                        message.MediaPlayer.Reset();
                                        message.MediaPlayer = null;

                                        message.MediaTimer.Enabled = false;
                                        message.MediaTimer.Stop();
                                        message.MediaTimer = null;
                                    }
                                    catch (Exception exception)
                                    {
                                        Console.WriteLine(exception);
                                    }
                                };

                                message.MediaPlayer.Prepared += (s, ee) =>
                                {
                                    try
                                    {
                                        message.MediaIsPlaying = true;
                                        soundViewHolder.PlayButton.Tag = "Pause";
                                        if (message.ModelType == MessageModelType.LeftAudio)
                                            soundViewHolder.PlayButton.SetImageResource(AppSettings.SetTabDarkTheme ? Resource.Drawable.ic_media_pause_light : Resource.Drawable.ic_media_pause_dark);
                                        else
                                            soundViewHolder.PlayButton.SetImageResource(Resource.Drawable.ic_media_pause_light);

                                        if (message.MediaTimer == null)
                                            message.MediaTimer = new Timer { Interval = 1000 };

                                        message.MediaPlayer.Start();

                                        var durationOfSound = message.MediaPlayer.Duration;

                                        message.MediaTimer.Elapsed += (sender, eventArgs) =>
                                        {
                                            MainActivity.RunOnUiThread(() =>
                                            {
                                                try
                                                {
                                                    if (message.MediaTimer.Enabled)
                                                    {
                                                        if (message.MediaPlayer.CurrentPosition <= message.MediaPlayer.Duration)
                                                        {
                                                            soundViewHolder.DurationTextView.Text = Methods.AudioRecorderAndPlayer.GetTimeString(message.MediaPlayer.CurrentPosition);
                                                        }
                                                        else
                                                        {
                                                            soundViewHolder.DurationTextView.Text = Methods.AudioRecorderAndPlayer.GetTimeString(message.MediaPlayer.Duration);

                                                            soundViewHolder.PlayButton.Tag = "Play";
                                                            soundViewHolder.PlayButton.SetImageResource(message.ModelType == MessageModelType.LeftAudio ? Resource.Drawable.ic_play_dark_arrow : Resource.Drawable.ic_play_arrow);
                                                        }
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    Console.WriteLine(e);
                                                    soundViewHolder.PlayButton.Tag = "Play";
                                                }
                                            });
                                        };
                                        message.MediaTimer.Start();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                };

                                if (mediaFile.Contains("http"))
                                {
                                    message.MediaPlayer.SetDataSource(MainActivity, Uri.Parse(mediaFile));
                                    message.MediaPlayer.PrepareAsync();
                                }
                                else
                                {
                                    File file2 = new File(mediaFile);
                                    var photoUri = FileProvider.GetUriForFile(MainActivity, MainActivity.PackageName + ".fileprovider", file2);

                                    message.MediaPlayer.SetDataSource(MainActivity, photoUri);
                                    message.MediaPlayer.Prepare();
                                }

                                message.SoundViewHolder = soundViewHolder;
                            }
                            else
                            {
                                if (soundViewHolder.PlayButton.Tag.ToString() == "Play")
                                {
                                    soundViewHolder.PlayButton.Tag = "Pause";
                                    if (message.ModelType == MessageModelType.LeftAudio)
                                        soundViewHolder.PlayButton.SetImageResource(AppSettings.SetTabDarkTheme ? Resource.Drawable.ic_media_pause_light : Resource.Drawable.ic_media_pause_dark);
                                    else
                                        soundViewHolder.PlayButton.SetImageResource(Resource.Drawable.ic_media_pause_light);

                                    message.MediaIsPlaying = true;
                                    message.MediaPlayer?.Start();

                                    if (message.MediaTimer != null)
                                    {
                                        message.MediaTimer.Enabled = true;
                                        message.MediaTimer.Start();
                                    }
                                }
                                else if (soundViewHolder.PlayButton.Tag.ToString() == "Pause")
                                {
                                    soundViewHolder.PlayButton.Tag = "Play";
                                    soundViewHolder.PlayButton.SetImageResource(message.ModelType == MessageModelType.LeftAudio ? Resource.Drawable.ic_play_dark_arrow : Resource.Drawable.ic_play_arrow);

                                    message.MediaIsPlaying = false;
                                    message.MediaPlayer?.Pause();

                                    if (message.MediaTimer != null)
                                    {
                                        message.MediaTimer.Enabled = false;
                                        message.MediaTimer.Stop();
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    };
                }

                if (message.SoundViewHolder == null)
                    message.SoundViewHolder = soundViewHolder;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadContactOfChatItem(Holders.ContactViewHolder holder, MessageData message)
        {
            try
            {
                if (holder.UserName != null) holder.UserName.Text = WoWonderTools.GetNameFinal(message.UserData);

                holder.MsgTimeTextView.Text = message.TimeText;
                holder.MsgTimeTextView.Visibility = message.ShowTimeText ? ViewStates.Visible : ViewStates.Gone;

                if (!string.IsNullOrEmpty(message.ContactName))
                {
                    holder.UserContactNameTextView.Text = message.ContactName;
                    holder.UserNumberTextView.Text = message.ContactNumber;
                }

                if (!holder.ContactLayout.HasOnClickListeners)
                {
                    holder.ContactLayout.Click += (sender, args) =>
                    {
                        try
                        {
                            Methods.App.SaveContacts(MainActivity, message.ContactNumber, message.ContactName, "2");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadVideoOfChatItem(Holders.VideoViewHolder holder, MessageData message, int position)
        {
            try
            {
                if (holder.UserName != null) holder.UserName.Text = WoWonderTools.GetNameFinal(message.UserData);

                var fileName = message.Media.Split('/').Last();
                var fileNameWithoutExtension = fileName.Split('.').First();
                var mediaFile = WoWonderTools.GetFile(PageId, Methods.Path.FolderDcimVideo, fileName, message.Media);

                holder.MsgTimeTextView.Text = message.TimeText;
                holder.FilenameTextView.Text = Methods.FunString.SubStringCutOf(fileNameWithoutExtension, 12) + ".mp4";

                var videoImage = Methods.MultiMedia.GetMediaFrom_Gallery(Methods.Path.FolderDcimVideo + PageId, fileNameWithoutExtension + ".png");
                if (videoImage == "File Dont Exists")
                {
                    File file2 = new File(mediaFile);
                    Uri photoUri = message.Media.Contains("http") ? Uri.Parse(message.Media) : FileProvider.GetUriForFile(MainActivity, MainActivity.PackageName + ".fileprovider", file2);

                    Glide.With(MainActivity)
                        .AsBitmap()
                        .Apply(GlideImageLoader.GetRequestOptions(ImageStyle.RoundedCrop, ImagePlaceholders.Drawable))
                        .Load(photoUri) // or URI/path
                        .Into(new MySimpleTarget(this, holder, position));  //image view to set thumbnail to 
                }
                else
                {
                    Glide.With(MainActivity).Load(videoImage).Apply(GlideImageLoader.GetRequestOptions(ImageStyle.RoundedCrop, ImagePlaceholders.Drawable)).Into(holder.ImageView);
                }

                holder.LoadingProgressview.Indeterminate = false;
                holder.LoadingProgressview.Visibility = ViewStates.Gone;
                holder.PlayButton.Visibility = ViewStates.Visible;

                if (!holder.PlayButton.HasOnClickListeners)
                {
                    holder.PlayButton.Click += (sender, args) =>
                    {
                        try
                        {
                            string imageFile = Methods.MultiMedia.CheckFileIfExits(mediaFile);
                            if (imageFile != "File Dont Exists")
                            {
                                File file2 = new File(mediaFile);
                                var mediaUri = FileProvider.GetUriForFile(MainActivity, MainActivity.PackageName + ".fileprovider", file2);

                                Intent intent = new Intent();
                                intent.SetAction(Intent.ActionView);
                                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                                intent.SetDataAndType(mediaUri, "video/*");
                                MainActivity.StartActivity(intent);
                            }
                            else
                            {
                                Intent intent = new Intent(Intent.ActionView);
                                intent.SetData(Uri.Parse(mediaFile));
                                intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                                intent.SetType("video/*");
                                MainActivity.StartActivity(intent);

                                //Toast.MakeText(MainActivity,MainActivity.GetText(Resource.String.Lbl_Something_went_wrong),ToastLength.Long).Show();
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        private class MySimpleTarget : CustomTarget
        {
            private readonly PageMessagesAdapter MAdapter;
            private readonly Holders.VideoViewHolder ViewHolder;
            private readonly int Position;
            public MySimpleTarget(PageMessagesAdapter adapter, Holders.VideoViewHolder viewHolder, int position)
            {
                try
                {
                    MAdapter = adapter;
                    ViewHolder = viewHolder;
                    Position = position;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public override void OnResourceReady(Object resource, ITransition transition)
            {
                try
                {
                    if (MAdapter.DifferList?.Count > 0)
                    {
                        var item = MAdapter.DifferList[Position].MesData;
                        if (item != null)
                        {
                            var fileName = item.Media.Split('/').Last();
                            var fileNameWithoutExtension = fileName.Split('.').First();

                            var pathImage = Methods.Path.FolderDcimVideo + MAdapter.PageId + "/" + fileNameWithoutExtension + ".png";

                            var videoImage = Methods.MultiMedia.GetMediaFrom_Gallery(Methods.Path.FolderDcimVideo + MAdapter.PageId, fileNameWithoutExtension + ".png");
                            if (videoImage == "File Dont Exists")
                            {
                                if (resource is Bitmap bitmap)
                                {
                                    Methods.MultiMedia.Export_Bitmap_As_Image(bitmap, fileNameWithoutExtension, Methods.Path.FolderDcimVideo + MAdapter.PageId + "/");

                                    File file2 = new File(pathImage);
                                    var photoUri = FileProvider.GetUriForFile(MAdapter.MainActivity, MAdapter.MainActivity.PackageName + ".fileprovider", file2);

                                    Glide.With(MAdapter.MainActivity).Load(photoUri).Apply(GlideImageLoader.GetRequestOptions(ImageStyle.RoundedCrop, ImagePlaceholders.Drawable)).Into(ViewHolder.ImageView);

                                    item.ImageVideo = photoUri.ToString();
                                }
                            }
                            else
                            {
                                File file2 = new File(pathImage);
                                var photoUri = FileProvider.GetUriForFile(MAdapter.MainActivity, MAdapter.MainActivity.PackageName + ".fileprovider", file2);

                                Glide.With(MAdapter.MainActivity).Load(photoUri).Apply(GlideImageLoader.GetRequestOptions(ImageStyle.RoundedCrop, ImagePlaceholders.Drawable)).Into(ViewHolder.ImageView);

                                item.ImageVideo = photoUri.ToString();
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public override void OnLoadCleared(Drawable p0) { }
        }


        private void LoadStickerOfChatItem(Holders.StickerViewHolder holder, MessageData message)
        {
            try
            {
                if (holder.UserName != null) holder.UserName.Text = WoWonderTools.GetNameFinal(message.UserData);

                string imageUrl = message.Media;

                holder.Time.Text = message.TimeText;
                if (imageUrl.Contains("http://") || imageUrl.Contains("https://"))
                {
                    Glide.With(MainActivity).Load(imageUrl).Apply(new RequestOptions().Placeholder(Resource.Drawable.ImagePlacholder)).Into(holder.ImageView);
                }
                else
                {
                    var file = Uri.FromFile(new File(imageUrl));
                    Glide.With(MainActivity).Load(file.Path).Apply(new RequestOptions().Placeholder(Resource.Drawable.ImagePlacholder)).Into(holder.ImageView);
                }

                holder.LoadingProgressview.Indeterminate = false;
                holder.LoadingProgressview.Visibility = ViewStates.Gone;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadFileOfChatItem(Holders.FileViewHolder holder, MessageData message)
        {
            try
            {
                if (holder.UserName != null) holder.UserName.Text = WoWonderTools.GetNameFinal(message.UserData);

                var fileName = message.Media.Split('/').Last();
                var fileNameWithoutExtension = fileName.Split('.').First();
                var fileNameExtension = fileName.Split('.').Last();

                holder.MsgTimeTextView.Text = message.TimeText;
                holder.FileNameTextView.Text = Methods.FunString.SubStringCutOf(fileNameWithoutExtension, 10) + fileNameExtension;
                holder.SizeFileTextView.Text = message.FileSize;

                if (fileNameExtension.Contains("rar") || fileNameExtension.Contains("RAR") ||
                    fileNameExtension.Contains("zip") || fileNameExtension.Contains("ZIP"))
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, holder.IconTypefile, "\uf1c6"); //ZipBox
                }
                else if (fileNameExtension.Contains("txt") || fileNameExtension.Contains("TXT"))
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, holder.IconTypefile, "\uf15c"); //NoteText
                }
                else if (fileNameExtension.Contains("docx") || fileNameExtension.Contains("DOCX") ||
                         fileNameExtension.Contains("doc") || fileNameExtension.Contains("DOC"))
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, holder.IconTypefile, "\uf1c2"); //FileWord
                }
                else if (fileNameExtension.Contains("pdf") || fileNameExtension.Contains("PDF"))
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, holder.IconTypefile, "\uf1c1"); //FilePdf
                }
                else if (fileNameExtension.Contains("apk") || fileNameExtension.Contains("APK"))
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, holder.IconTypefile, "\uf17b"); //Fileandroid
                }
                else
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, holder.IconTypefile, "\uf15b"); //file
                }

                if (!holder.MainView.HasOnClickListeners)
                {
                    holder.MainView.Click += (sender, args) =>
                    {

                    };
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LoadGifOfChatItem(Holders.GifViewHolder holder, MessageData item)
        {
            try
            {
                if (holder.UserName != null) holder.UserName.Text = WoWonderTools.GetNameFinal(item.UserData);

                // G_fixed_height_small_url, // UrlGif - view  >>  mediaFileName
                // G_fixed_height_small_mp4, //MediaGif - sent >>  media

                string imageUrl = "";
                if (!string.IsNullOrEmpty(item.Stickers))
                    imageUrl = item.Stickers;
                else if (!string.IsNullOrEmpty(item.Media))
                    imageUrl = item.Media;
                else if (!string.IsNullOrEmpty(item.MediaFileName))
                    imageUrl = item.MediaFileName;

                string[] fileName = imageUrl.Split(new[] { "/", "200.gif?cid=", "&rid=200" }, StringSplitOptions.RemoveEmptyEntries);
                var lastFileName = fileName.Last();
                var name = fileName[3] + lastFileName;

                item.Media = WoWonderTools.GetFile(PageId, Methods.Path.FolderDiskGif, name, imageUrl);

                if (item.Media != null && (item.Media.Contains("http")))
                {
                    GlideImageLoader.LoadImage(MainActivity, imageUrl, holder.ImageGifView, ImageStyle.RoundedCrop, ImagePlaceholders.Drawable);
                }
                else
                {
                    var file = Uri.FromFile(new File(item.Media));
                    Glide.With(MainActivity).Load(file.Path).Apply(GlideImageLoader.GetRequestOptions(ImageStyle.RoundedCrop, ImagePlaceholders.Drawable)).Into(holder.ImageGifView);
                }

                holder.LoadingProgressview.Indeterminate = false;
                holder.LoadingProgressview.Visibility = ViewStates.Gone;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        //==============================

        public override int ItemCount => DifferList?.Count ?? 0;

        public AdapterModelsClassPage GetItem(int position)
        {
            var item = DifferList[position];

            return item;
        }

        public override long GetItemId(int position)
        {
            var item = DifferList[position];
            if (item == null)
                return 0;

            return item.Id;
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                var item = DifferList[position];
                if (item == null)
                    return (int)MessageModelType.None;

                switch (item.TypeView)
                {
                    case MessageModelType.RightProduct:
                        {
                            return (int)MessageModelType.RightProduct;
                        }
                    case MessageModelType.LeftProduct:
                        {
                            return (int)MessageModelType.LeftProduct;
                        }
                    case MessageModelType.RightGif:
                        {
                            return (int)MessageModelType.RightGif;
                        }
                    case MessageModelType.LeftGif:
                        {
                            return (int)MessageModelType.LeftGif;
                        }
                    case MessageModelType.RightText:
                        {
                            return (int)MessageModelType.RightText;
                        }
                    case MessageModelType.LeftText:
                        {
                            return (int)MessageModelType.LeftText;
                        }
                    case MessageModelType.RightImage:
                        {
                            return (int)MessageModelType.RightImage;
                        }
                    case MessageModelType.LeftImage:
                        {
                            return (int)MessageModelType.LeftImage;
                        }
                    case MessageModelType.RightAudio:
                        {
                            return (int)MessageModelType.RightAudio;
                        }
                    case MessageModelType.LeftAudio:
                        {
                            return (int)MessageModelType.LeftAudio;
                        }
                    case MessageModelType.RightContact:
                        {
                            return (int)MessageModelType.RightContact;
                        }
                    case MessageModelType.LeftContact:
                        {
                            return (int)MessageModelType.LeftContact;
                        }
                    case MessageModelType.RightVideo:
                        {
                            return (int)MessageModelType.RightVideo;
                        }
                    case MessageModelType.LeftVideo:
                        {
                            return (int)MessageModelType.LeftVideo;
                        }
                    case MessageModelType.RightSticker:
                        {
                            return (int)MessageModelType.RightSticker;
                        }
                    case MessageModelType.LeftSticker:
                        {
                            return (int)MessageModelType.LeftSticker;
                        }
                    case MessageModelType.RightFile:
                        {
                            return (int)MessageModelType.RightFile;
                        }
                    case MessageModelType.LeftFile:
                        {
                            return (int)MessageModelType.LeftFile;
                        }
                    default:
                        {
                            return (int)MessageModelType.None;
                        }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return (int)MessageModelType.None;
            }
        }

        void OnClick(Holders.MesClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(Holders.MesClickEventArgs args) => ItemLongClick?.Invoke(this, args);

        public MessageModelType GetTypeModel(WoWonderClient.Classes.Message.MessageData item)
        {
            try
            {
                MessageModelType modelType;

                if (item.FromId == UserDetails.UserId) // right
                {
                    item.Position = "right";
                }
                else if (item.ToId == UserDetails.UserId) // left
                {
                    item.Position = "left";
                }

                string imageUrl = "";
                if (!string.IsNullOrEmpty(item.Stickers))
                {
                    item.Stickers = item.Stickers.Replace(".mp4", ".gif");
                    imageUrl = item.Stickers;
                }

                if (!string.IsNullOrEmpty(item.Media))
                    imageUrl = item.Media;

                if (!string.IsNullOrEmpty(item.Text))
                    modelType = item.TypeTwo == "contact" ? item.Position == "left" ? MessageModelType.LeftContact : MessageModelType.RightContact : item.Position == "left" ? MessageModelType.LeftText : MessageModelType.RightText;
                else if (item.Product?.ProductClass != null && !string.IsNullOrEmpty(item.ProductId) && item.ProductId != "0")
                    modelType = item.Position == "left" ? MessageModelType.LeftProduct : MessageModelType.RightProduct;
                else if (!string.IsNullOrEmpty(imageUrl))
                {
                    var type = Methods.AttachmentFiles.Check_FileExtension(imageUrl);
                    switch (type)
                    {
                        case "Audio":
                            modelType = item.Position == "left" ? MessageModelType.LeftAudio : MessageModelType.RightAudio;
                            break;
                        case "Video":
                            modelType = item.Position == "left" ? MessageModelType.LeftVideo : MessageModelType.RightVideo;
                            break;
                        case "Image" when !string.IsNullOrEmpty(item.Media) && !item.Media.Contains(".gif"):
                            modelType = item.Media.Contains("sticker") ? item.Position == "left" ? MessageModelType.LeftSticker : MessageModelType.RightSticker : item.Position == "left" ? MessageModelType.LeftImage : MessageModelType.RightImage;
                            break;
                        case "Image" when !string.IsNullOrEmpty(item.Stickers) && item.Stickers.Contains(".gif"):
                        case "Image" when !string.IsNullOrEmpty(item.Media) && item.Media.Contains(".gif"):
                            modelType = item.Position == "left" ? MessageModelType.LeftGif : MessageModelType.RightGif;
                            break;
                        case "File":
                            modelType = item.Position == "left" ? MessageModelType.LeftFile : MessageModelType.RightFile;
                            break;
                        default:
                            modelType = MessageModelType.None;
                            break;
                    }
                }
                else
                {
                    modelType = MessageModelType.None;
                }

                return modelType;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return MessageModelType.None;
            }
        }


        #region MusicBar

        private void InitValueAnimator(float playbackSpeed, int progress, int max)
        {
            int timeToEnd = (int)((max - progress) / playbackSpeed);
            if (timeToEnd > 0)
            {
                if (ValueAnimator.OfInt(progress, max).SetDuration(timeToEnd) is ValueAnimator value)
                {
                    MusicBarViewHolder?.MusicBarViewHolder?.FixedMusicBar?.SetProgressAnimator(value);

                    MValueAnimator = value;
                    MValueAnimator.SetInterpolator(new LinearInterpolator());
                    MValueAnimator.AddUpdateListener(this);
                    MValueAnimator.Start();
                }
            }
        }

        //====================== IOnMusicBarAnimationChangeListener ======================
        public void OnHideAnimationEnd()
        {

        }

        public void OnHideAnimationStart()
        {

        }

        public void OnShowAnimationEnd()
        {

        }

        public void OnShowAnimationStart()
        {

        }

        //====================== IOnMusicBarProgressChangeListener ======================

        public void OnProgressChanged(MusicBar musicBarView, int progress, bool fromUser)
        {
            if (fromUser)
                MSeekBarIsTracking = true;
        }

        public void OnStartTrackingTouch(MusicBar musicBarView)
        {
            MSeekBarIsTracking = true;
        }

        public void OnStopTrackingTouch(MusicBar musicBarView)
        {
            try
            {
                MSeekBarIsTracking = false;
                MusicBarViewHolder?.MusicBarViewHolder?.FixedMusicBar?.InitProgressAnimator(1.0f, musicBarView.GetPosition(), MusicBarViewHolder.MediaPlayer.Duration);
                MusicBarViewHolder?.MediaPlayer?.SeekTo(musicBarView.GetPosition());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnAnimationUpdate(ValueAnimator animation)
        {
            try
            {
                if (MSeekBarIsTracking)
                {
                    animation.Cancel();
                }
                else
                {
                    MusicBarViewHolder?.MusicBarViewHolder?.FixedMusicBar.SetProgress((int)animation.AnimatedValue);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        #endregion

    }
}