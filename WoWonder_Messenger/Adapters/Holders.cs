using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using Com.Luseen.Autolinklibrary;
using Refractored.Controls;
using System;
using WoWonder.Helpers.Fonts;
using WoWonder.Helpers.Utils;
using WoWonder.Library.MusicBar;

namespace WoWonder.Adapters
{
    public class Holders
    {
        public enum TypeClick
        {
            Text, Image, Sound, Contact, Video, Sticker, Gif, File, Product
        }
        public class MesClickEventArgs : EventArgs
        {
            public View View { get; set; }
            public int Position { get; set; }
            public TypeClick Type { get; set; }
        }

        public class TextViewHolder : RecyclerView.ViewHolder
        {
            #region Variables Basic

            public LinearLayout LytParent { get; private set; }
            public TextView Time { get; private set; }
            public View MainView { get; private set; }
            public EventHandler ClickHandler { get; set; }
            public AutoLinkTextView AutoLinkTextView { get; private set; }
            public TextView Seen { get; private set; }
            public TextView UserName { get; private set; }

            #endregion

            public TextViewHolder(View itemView, Action<MesClickEventArgs> clickListener, Action<MesClickEventArgs> longClickListener, bool showName) : base(itemView)
            {
                try
                {
                    MainView = itemView;
                    LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                    AutoLinkTextView = itemView.FindViewById<AutoLinkTextView>(Resource.Id.active);
                    Time = itemView.FindViewById<TextView>(Resource.Id.time);
                    Seen = itemView.FindViewById<TextView>(Resource.Id.seen);

                    UserName = itemView.FindViewById<TextView>(Resource.Id.name);
                    if (UserName != null) UserName.Visibility = showName ? ViewStates.Visible : ViewStates.Gone;

                    itemView.Click += (sender, args) => clickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Text });
                    AutoLinkTextView.LongClick += (sender, args) => longClickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Text });
                    itemView.LongClick += (sender, args) => longClickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Text });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e + "Error");
                }
            }
        }

        public class ImageViewHolder : RecyclerView.ViewHolder
        {
            #region Variables Basic

            public LinearLayout LytParent { get; private set; }
            public View MainView { get; private set; }
            public EventHandler ClickHandler { get; set; }
            public ImageView ImageView { get; private set; }
            public ProgressBar LoadingProgressview { get; private set; }
            public TextView Time { get; private set; }
            public TextView Seen { get; private set; }
            public TextView UserName { get; private set; }

            #endregion

            public ImageViewHolder(View itemView, Action<MesClickEventArgs> clickListener, Action<MesClickEventArgs> longClickListener, bool showName) : base(itemView)
            {
                try
                {
                    MainView = itemView;
                    LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                    ImageView = itemView.FindViewById<ImageView>(Resource.Id.imgDisplay);
                    LoadingProgressview = itemView.FindViewById<ProgressBar>(Resource.Id.loadingProgressview);
                    Time = itemView.FindViewById<TextView>(Resource.Id.time);
                    Seen = itemView.FindViewById<TextView>(Resource.Id.seen);

                    UserName = itemView.FindViewById<TextView>(Resource.Id.name);
                    if (UserName != null) UserName.Visibility = showName ? ViewStates.Visible : ViewStates.Gone;

                    itemView.Click += (sender, args) => clickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Image });
                    itemView.LongClick += (sender, args) => longClickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Image });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public class SoundViewHolder : RecyclerView.ViewHolder
        {
            #region Variables Basic

            public LinearLayout LytParent { get; private set; }
            public View MainView { get; private set; }
            public TextView DurationTextView { get; private set; }
            public TextView MsgTimeTextView { get; private set; }
            public CircleButton PlayButton { get; private set; }
            public ProgressBar LoadingProgressview { get; private set; }
            public TextView UserName { get; private set; }
            public TextView Seen { get; private set; }

            #endregion

            public SoundViewHolder(View itemView, Action<MesClickEventArgs> clickListener, Action<MesClickEventArgs> longClickListener, bool showName) : base(itemView)
            {
                try
                {
                    MainView = itemView;
                    LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                    DurationTextView = itemView.FindViewById<TextView>(Resource.Id.Duration);
                    PlayButton = itemView.FindViewById<CircleButton>(Resource.Id.playButton);
                    MsgTimeTextView = itemView.FindViewById<TextView>(Resource.Id.time);
                    LoadingProgressview = itemView.FindViewById<ProgressBar>(Resource.Id.loadingProgressview);
                    Seen = itemView.FindViewById<TextView>(Resource.Id.seen);

                    UserName = itemView.FindViewById<TextView>(Resource.Id.name);
                    if (UserName != null) UserName.Visibility = showName ? ViewStates.Visible : ViewStates.Gone;

                    itemView.Click += (sender, args) => clickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Sound });
                    itemView.LongClick += (sender, args) => longClickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Sound });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public class MusicBarViewHolder : RecyclerView.ViewHolder
        {
            #region Variables Basic

            public LinearLayout LytParent { get; private set; }
            public View MainView { get; private set; }
            public TextView DurationTextView { get; private set; }
            public TextView MsgTimeTextView { get; private set; }
            public CircleButton PlayButton { get; private set; }
            public ProgressBar LoadingProgressview { get; private set; }
            public FixedMusicBar FixedMusicBar { get; private set; }
            public TextView UserName { get; private set; }
            public TextView Seen { get; private set; }

            #endregion

            public MusicBarViewHolder(View itemView, Action<MesClickEventArgs> clickListener, Action<MesClickEventArgs> longClickListener, bool showName) : base(itemView)
            {
                try
                {
                    MainView = itemView;
                    LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                    DurationTextView = itemView.FindViewById<TextView>(Resource.Id.Duration);
                    PlayButton = itemView.FindViewById<CircleButton>(Resource.Id.playButton);
                    MsgTimeTextView = itemView.FindViewById<TextView>(Resource.Id.time);
                    LoadingProgressview = itemView.FindViewById<ProgressBar>(Resource.Id.loadingProgressview);
                    FixedMusicBar = itemView.FindViewById<FixedMusicBar>(Resource.Id.miniMusicBar);
                    Seen = itemView.FindViewById<TextView>(Resource.Id.seen);

                    UserName = itemView.FindViewById<TextView>(Resource.Id.name);
                    if (UserName != null) UserName.Visibility = showName ? ViewStates.Visible : ViewStates.Gone;

                    itemView.Click += (sender, args) => clickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Sound });
                    itemView.LongClick += (sender, args) => longClickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Sound });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public class ContactViewHolder : RecyclerView.ViewHolder
        {
            #region Variables Basic

            public LinearLayout LytParent { get; private set; }
            public LinearLayout ContactLayout { get; private set; }
            public View MainView { get; private set; }
            public TextView UserContactNameTextView { get; private set; }
            public TextView UserNumberTextView { get; private set; }
            public TextView MsgTimeTextView { get; private set; }
            public CircleImageView ProfileImage { get; private set; }
            public TextView Seen { get; private set; }
            public TextView UserName { get; private set; }

            #endregion

            public ContactViewHolder(View itemView, Action<MesClickEventArgs> clickListener, Action<MesClickEventArgs> longClickListener, bool showName) : base(itemView)
            {
                try
                {
                    MainView = itemView;
                    LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                    ContactLayout = itemView.FindViewById<LinearLayout>(Resource.Id.bubble_layout);
                    UserContactNameTextView = itemView.FindViewById<TextView>(Resource.Id.contactName);
                    UserNumberTextView = itemView.FindViewById<TextView>(Resource.Id.numberText);
                    MsgTimeTextView = itemView.FindViewById<TextView>(Resource.Id.time);
                    ProfileImage = itemView.FindViewById<CircleImageView>(Resource.Id.profile_image);
                    Seen = itemView.FindViewById<TextView>(Resource.Id.seen);

                    UserName = itemView.FindViewById<TextView>(Resource.Id.name);
                    if (UserName != null) UserName.Visibility = showName ? ViewStates.Visible : ViewStates.Gone;

                    itemView.Click += (sender, args) => clickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Contact });
                    itemView.LongClick += (sender, args) => longClickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Contact });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public class VideoViewHolder : RecyclerView.ViewHolder
        {
            #region Variables Basic

            public LinearLayout LytParent { get; private set; }
            public View MainView { get; private set; }
            public EventHandler ClickHandler { get; set; }
            public ImageView ImageView { get; private set; }
            public ProgressBar LoadingProgressview { get; private set; }
            public TextView MsgTimeTextView { get; private set; }
            public TextView IconView { get; private set; }
            public TextView FilenameTextView { get; private set; }
            public CircleButton PlayButton { get; private set; }
            public TextView UserName { get; private set; }
            public TextView Seen { get; private set; }

            #endregion

            public VideoViewHolder(View itemView, Action<MesClickEventArgs> clickListener, Action<MesClickEventArgs> longClickListener, bool showName) : base(itemView)
            {
                try
                {
                    MainView = itemView;
                    LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                    ImageView = itemView.FindViewById<ImageView>(Resource.Id.imgDisplay);
                    LoadingProgressview = itemView.FindViewById<ProgressBar>(Resource.Id.loadingProgressview);
                    MsgTimeTextView = itemView.FindViewById<TextView>(Resource.Id.time);
                    IconView = itemView.FindViewById<TextView>(Resource.Id.icon);
                    FilenameTextView = itemView.FindViewById<TextView>(Resource.Id.fileName);
                    PlayButton = itemView.FindViewById<CircleButton>(Resource.Id.playButton);
                    Seen = itemView.FindViewById<TextView>(Resource.Id.seen);

                    UserName = itemView.FindViewById<TextView>(Resource.Id.name);
                    if (UserName != null) UserName.Visibility = showName ? ViewStates.Visible : ViewStates.Gone;

                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconView, IonIconsFonts.Videocamera);

                    itemView.Click += (sender, args) => clickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Video });
                    itemView.LongClick += (sender, args) => longClickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Video });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public class StickerViewHolder : RecyclerView.ViewHolder
        {
            #region Variables Basic

            public LinearLayout LytParent { get; private set; }
            public View MainView { get; private set; }
            public EventHandler ClickHandler { get; set; }
            public ImageView ImageView { get; private set; }
            public ProgressBar LoadingProgressview { get; private set; }
            public TextView Time { get; private set; }
            public TextView UserName { get; private set; }
            public TextView Seen { get; private set; }

            #endregion

            public StickerViewHolder(View itemView, Action<MesClickEventArgs> clickListener, Action<MesClickEventArgs> longClickListener, bool showName) : base(itemView)
            {
                try
                {
                    MainView = itemView;
                    LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                    ImageView = itemView.FindViewById<ImageView>(Resource.Id.imgDisplay);
                    LoadingProgressview = itemView.FindViewById<ProgressBar>(Resource.Id.loadingProgressview);
                    Time = itemView.FindViewById<TextView>(Resource.Id.time);
                    Seen = itemView.FindViewById<TextView>(Resource.Id.seen);

                    UserName = itemView.FindViewById<TextView>(Resource.Id.name);
                    if (UserName != null) UserName.Visibility = showName ? ViewStates.Visible : ViewStates.Gone;

                    itemView.Click += (sender, args) => clickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Sticker });
                    itemView.LongClick += (sender, args) => longClickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Sticker });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public class GifViewHolder : RecyclerView.ViewHolder
        {
            #region Variables Basic

            public LinearLayout LytParent { get; private set; }
            public View MainView { get; private set; }
            public EventHandler ClickHandler { get; set; }
            public ImageView ImageGifView { get; private set; }
            public ProgressBar LoadingProgressview { get; private set; }
            public TextView Time { get; private set; }
            public TextView UserName { get; private set; }
            public TextView Seen { get; private set; }

            #endregion

            public GifViewHolder(View itemView, Action<MesClickEventArgs> clickListener, Action<MesClickEventArgs> longClickListener, bool showName) : base(itemView)
            {
                try
                {
                    MainView = itemView;

                    LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                    ImageGifView = itemView.FindViewById<ImageView>(Resource.Id.imggifdisplay);
                    LoadingProgressview = itemView.FindViewById<ProgressBar>(Resource.Id.loadingProgressview);
                    Time = itemView.FindViewById<TextView>(Resource.Id.time);
                    Seen = itemView.FindViewById<TextView>(Resource.Id.seen);

                    UserName = itemView.FindViewById<TextView>(Resource.Id.name);
                    if (UserName != null) UserName.Visibility = showName ? ViewStates.Visible : ViewStates.Gone;

                    itemView.Click += (sender, args) => clickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Gif });
                    itemView.LongClick += (sender, args) => longClickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Gif });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public class NotSupportedViewHolder : RecyclerView.ViewHolder
        {
            #region Variables Basic

            public LinearLayout LytParent { get; private set; }
            public View MainView { get; private set; }
            public EventHandler ClickHandler { get; set; }
            public AutoLinkTextView AutoLinkNotsupportedView { get; private set; }

            #endregion

            public NotSupportedViewHolder(View itemView) : base(itemView)
            {
                try
                {
                    MainView = itemView;
                    LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                    AutoLinkNotsupportedView = itemView.FindViewById<AutoLinkTextView>(Resource.Id.active);
                    var time = itemView.FindViewById<TextView>(Resource.Id.time);

                    time.Visibility = ViewStates.Gone;

                    var userName = itemView.FindViewById<TextView>(Resource.Id.name);
                    if (userName != null) userName.Visibility = ViewStates.Gone;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e + "Error");
                }
            }
        }

        public class FileViewHolder : RecyclerView.ViewHolder
        {
            #region Variables Basic

            public LinearLayout LytParent { get; private set; }
            public View MainView { get; private set; }
            public TextView FileNameTextView { get; private set; }
            public TextView SizeFileTextView { get; private set; }
            public TextView MsgTimeTextView { get; private set; }
            public TextView IconTypefile { get; private set; }
            public TextView UserName { get; private set; }
            public TextView Seen { get; private set; }

            #endregion

            public FileViewHolder(View itemView, Action<MesClickEventArgs> clickListener, Action<MesClickEventArgs> longClickListener, bool showName) : base(itemView)
            {
                try
                {
                    MainView = itemView;
                    LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                    FileNameTextView = itemView.FindViewById<TextView>(Resource.Id.fileName);
                    SizeFileTextView = itemView.FindViewById<TextView>(Resource.Id.sizefileText);
                    MsgTimeTextView = itemView.FindViewById<TextView>(Resource.Id.time);
                    IconTypefile = itemView.FindViewById<TextView>(Resource.Id.Icontypefile);
                    Seen = itemView.FindViewById<TextView>(Resource.Id.seen);

                    UserName = itemView.FindViewById<TextView>(Resource.Id.name);
                    if (UserName != null) UserName.Visibility = showName ? ViewStates.Visible : ViewStates.Gone;

                    itemView.Click += (sender, args) => clickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.File });
                    itemView.LongClick += (sender, args) => longClickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.File });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public class ProductViewHolder : RecyclerView.ViewHolder
        {
            #region Variables Basic

            public LinearLayout LytParent { get; private set; }
            public View MainView { get; private set; }
            public EventHandler ClickHandler { get; set; }
            public ImageView ImageView { get; private set; }
            public TextView Time { get; private set; }
            public TextView Seen { get; private set; }
            public TextView Title { get; private set; }
            public TextView Cat { get; private set; }
            public TextView Price { get; private set; }
            public TextView UserName { get; private set; }

            #endregion

            public ProductViewHolder(View itemView, Action<MesClickEventArgs> clickListener, Action<MesClickEventArgs> longClickListener, bool showName) : base(itemView)
            {
                try
                {
                    MainView = itemView;
                    LytParent = itemView.FindViewById<LinearLayout>(Resource.Id.main);
                    ImageView = itemView.FindViewById<ImageView>(Resource.Id.imgDisplay);
                    Time = itemView.FindViewById<TextView>(Resource.Id.time);
                    Seen = itemView.FindViewById<TextView>(Resource.Id.seen);

                    Title = itemView.FindViewById<TextView>(Resource.Id.title);
                    Cat = itemView.FindViewById<TextView>(Resource.Id.cat);
                    Price = itemView.FindViewById<TextView>(Resource.Id.price);

                    UserName = itemView.FindViewById<TextView>(Resource.Id.name);
                    if (UserName != null) UserName.Visibility = showName ? ViewStates.Visible : ViewStates.Gone;

                    itemView.Click += (sender, args) => clickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Product });
                    itemView.LongClick += (sender, args) => longClickListener(new MesClickEventArgs { View = itemView, Position = AdapterPosition, Type = TypeClick.Product });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }


        //################################## LastMessages ###############################
        public class LastMessagesViewHolder : RecyclerView.ViewHolder, SwipeItemTouchHelper.ITouchViewHolder
        {
            #region Variables Basic

            public View MainView { get; private set; }

            public LinearLayout MainSwipe { get; private set; }
            public RelativeLayout RelativeLayoutMain { get; private set; }
            public ImageView IconCheckCountMessages { get; private set; }

            public ImageView LastMessagesIcon { get; private set; }
            public TextView TxtUsername { get; private set; }
            public TextView TxtLastMessages { get; private set; }
            public TextView TxtTimestamp { get; private set; }
            public ImageView ImageAvatar { get; private set; } //ImageView
            public CircleImageView ImageLastseen { get; private set; }
            public CircleButton MoreButton { get; private set; }
            public CircleButton CallButton { get; private set; }
            public CircleButton DeleteButton { get; private set; }

            #endregion

            public LastMessagesViewHolder(View itemView, Action<LastMessagesClickEventArgs> clickListener, Action<LastMessagesClickEventArgs> longClickListener
                        , Action<LastMessagesClickEventArgs> moreClickListener, Action<LastMessagesClickEventArgs> callClickListener, Action<LastMessagesClickEventArgs> deleteClickListener) : base(itemView)
            {
                try
                {
                    MainView = itemView;

                    //Get values
                    MainSwipe = (LinearLayout)MainView.FindViewById(Resource.Id.mainSwipe);
                    MoreButton = (CircleButton)MainView.FindViewById(Resource.Id.moreButton);
                    CallButton = (CircleButton)MainView.FindViewById(Resource.Id.callButton);
                    DeleteButton = (CircleButton)MainView.FindViewById(Resource.Id.deleteButton);

                    RelativeLayoutMain = (RelativeLayout)MainView.FindViewById(Resource.Id.main);
                    ImageAvatar = (ImageView)MainView.FindViewById(Resource.Id.ImageAvatar);
                    ImageLastseen = (CircleImageView)MainView.FindViewById(Resource.Id.ImageLastseen);
                    TxtUsername = (TextView)MainView.FindViewById(Resource.Id.Txt_Username);
                    LastMessagesIcon = (ImageView)MainView.FindViewById(Resource.Id.IconLastMessages);
                    TxtLastMessages = (TextView)MainView.FindViewById(Resource.Id.Txt_LastMessages);
                    TxtTimestamp = (TextView)MainView.FindViewById(Resource.Id.Txt_timestamp);
                    IconCheckCountMessages = (ImageView)MainView.FindViewById(Resource.Id.IconCheckRead);

                    if (!AppSettings.EnableAudioVideoCall)
                        CallButton.Visibility = ViewStates.Gone;

                    //Create an Event
                    MoreButton.Click += (sender, e) => moreClickListener(new LastMessagesClickEventArgs { View = itemView, Position = AdapterPosition });
                    CallButton.Click += (sender, e) => callClickListener(new LastMessagesClickEventArgs { View = itemView, Position = AdapterPosition });
                    DeleteButton.Click += (sender, e) => deleteClickListener(new LastMessagesClickEventArgs { View = itemView, Position = AdapterPosition });

                    itemView.Click += (sender, e) => clickListener(new LastMessagesClickEventArgs { View = itemView, Position = AdapterPosition });
                    itemView.LongClick += (sender, e) => longClickListener(new LastMessagesClickEventArgs { View = itemView, Position = AdapterPosition });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e + "Error");
                }
            }

            public void OnItemSelected()
            {
                try
                {
                    //MainSwipe.SetBackgroundColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#333333") : Color.ParseColor("#efefef")); 
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public void OnItemClear()
            {
                try
                {
                    //MainSwipe.SetBackgroundColor(Color.Transparent);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        public class LastMessagesClickEventArgs : EventArgs
        {
            public View View { get; set; }
            public int Position { get; set; }
        }
    }
}