using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Refractored.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;

namespace WoWonder.Activities.Tab.Adapter
{
    public class LastCallsAdapter : RecyclerView.Adapter
    {
        public event EventHandler<LastCallsAdapterClickEventArgs> ItemClick;
        public event EventHandler<LastCallsAdapterClickEventArgs> ItemLongClick;
        public event EventHandler<LastCallsAdapterClickEventArgs> CallClick;

        public ObservableCollection<Classes.CallUser> MCallUser = new ObservableCollection<Classes.CallUser>();

        private readonly Activity ActivityContext;

        public LastCallsAdapter(Activity context)
        {
            try
            {
                ActivityContext = context;
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
                //Setup your layout here >> Last_Calls_view
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Last_Calls_view, parent, false);
                var holder = new LastCallsAdapterViewHolder(itemView, OnClick, OnLongClick, CallOnClick);
                return holder;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is LastCallsAdapterViewHolder holder)
                {
                    var item = MCallUser[position];
                    if (item != null)
                    {
                        Initialize(holder, item);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Initialize(LastCallsAdapterViewHolder holder, Classes.CallUser item)
        {
            try
            {
                GlideImageLoader.LoadImage(ActivityContext, item.Avatar, holder.ImageAvatar, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                string name = Methods.FunString.DecodeString(item.Name);
                holder.TxtUsername.Text = name;

                if (item.TypeIcon == "Accept")
                {
                    Glide.With(ActivityContext).Load(Resource.Drawable.social_ic_phone_calls).Into(holder.IconCall);
                }
                else if (item.TypeIcon == "Cancel")
                {
                    Glide.With(ActivityContext).Load(Resource.Drawable.social_ic_phone_call_arrow).Into(holder.IconCall);
                }
                else if (item.TypeIcon == "Declined")
                {
                    Glide.With(ActivityContext).Load(Resource.Drawable.social_ic_forward_call).Into(holder.IconCall);
                }

                if (item.Time == "Declined call")
                {
                    holder.TxtLastTimecall.Text = ActivityContext.GetText(Resource.String.Lbl_Declined_call);
                }
                else if (item.Time == "Missed call")
                {
                    holder.TxtLastTimecall.Text = ActivityContext.GetText(Resource.String.Lbl_Missed_call);
                }
                else if (item.Time == "Answered call")
                {
                    holder.TxtLastTimecall.Text = ActivityContext.GetText(Resource.String.Lbl_Answered_call);
                }
                else
                {
                    holder.TxtLastTimecall.Text = item.Time;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Function Call
        public void Insert(Classes.CallUser call)
        {
            try
            {
                var check = MCallUser.FirstOrDefault(a => a.Id == call.Id);
                if (check == null)
                {
                    MCallUser.Insert(0, call);
                    NotifyItemInserted(0);
                    TabbedMainActivity.GetInstance().LastCallsTab.MRecycler?.ScrollToPosition(0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override int ItemCount => MCallUser?.Count ?? 0;

        public Classes.CallUser GetItem(int position)
        {
            return MCallUser[position];
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

        void OnClick(LastCallsAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(LastCallsAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
        void CallOnClick(LastCallsAdapterClickEventArgs args) => CallClick?.Invoke(this, args);

    }

    public class LastCallsAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public TextView TxtUsername { get; private set; }
        public TextView TxtLastTimecall { get; private set; }

        public ImageView IconCall { get; private set; }
        public ImageView ImageAvatar { get; private set; }
        public CircleImageView ImageLastseen { get; private set; }

        #endregion

        public LastCallsAdapterViewHolder(View itemView, Action<LastCallsAdapterClickEventArgs> clickListener, Action<LastCallsAdapterClickEventArgs> longClickListener, Action<LastCallsAdapterClickEventArgs> callclickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values  
                TxtUsername = (TextView)MainView.FindViewById(Resource.Id.Txt_name);

                TxtLastTimecall = (TextView)MainView.FindViewById(Resource.Id.Txt_Lasttimecalls);
                ImageAvatar = (ImageView)MainView.FindViewById(Resource.Id.Img_Avatar);
                IconCall = (ImageView)MainView.FindViewById(Resource.Id.IconCall);
                ImageLastseen = (CircleImageView)MainView.FindViewById(Resource.Id.ImageLastseen);
                ImageLastseen.Visibility = ViewStates.Gone;

                //Create an Event
                itemView.Click += (sender, e) => clickListener(new LastCallsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new LastCallsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                IconCall.Click += (sender, e) => callclickListener(new LastCallsAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class LastCallsAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}