using Android.App;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;

namespace WoWonder.Activities.ChatWindow.Adapters
{
    public class EmptySuggetionRecylerAdapter : RecyclerView.Adapter
    {
        public class SuggetionsMessages
        {
            public string Message { get; set; }

            public string RealMessage { get; set; }
            public int Id { get; set; }

        }
        public event EventHandler<AdapterClickEvents> OnItemClick;
        public static RecyclerView Recylercontrol;
        private readonly JavaList<SuggetionsMessages> SuggetionsMessagesList;
        private readonly Activity ActivityContext;

        public EmptySuggetionRecylerAdapter(Activity context)
        {
            ActivityContext = context;
            SuggetionsMessagesList = new JavaList<SuggetionsMessages>();

            SuggetionsMessages a1 = new SuggetionsMessages();
            a1.Id = 1;
            a1.Message = "Say Hi 🖐️";
            a1.RealMessage = "Hi 🖐️";

            SuggetionsMessages a2 = new SuggetionsMessages();
            a2.Id = 2;
            a2.Message = "How are you?";
            a2.RealMessage = "How are you?";

            SuggetionsMessages a3 = new SuggetionsMessages();
            a3.Id = 3;
            a3.Message = "Can we speak?";
            a3.RealMessage = "Hi";

            SuggetionsMessages a4 = new SuggetionsMessages();
            a4.Id = 4;
            a4.Message = "I like your picture ❤️";
            a4.RealMessage = "I like your picture ❤️";

            SuggetionsMessagesList.Add(a1);
            SuggetionsMessagesList.Add(a2);
            SuggetionsMessagesList.Add(a3);
            SuggetionsMessagesList.Add(a4);

        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here //  First RUN

            View row = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Recyler_Suggetion_Msg_Layout, parent, false);

            var vh = new EmptySuggetionRecylerViewHolder(row, OnClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                // Replace the contents of the view with that element
                if (viewHolder is EmptySuggetionRecylerViewHolder holder)
                {
                    var item = SuggetionsMessagesList[position];
                    if (item != null)
                    {
                        holder.NormaText.Text = item.Message;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OnClick(AdapterClickEvents args)
        {
            OnItemClick?.Invoke(this, args);
        }

        public override int ItemCount => SuggetionsMessagesList?.Count ?? 0;

        public SuggetionsMessages GetItem(int position)
        {
            return SuggetionsMessagesList[position];
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

    }

    public class EmptySuggetionRecylerViewHolder : RecyclerView.ViewHolder
    {

        public View MainView { get; private set; }
        public TextView NormaText { get; private set; }

        public EmptySuggetionRecylerViewHolder(View itemView, Action<AdapterClickEvents> listener) : base(itemView)
        {
            try
            {
                MainView = itemView;
                NormaText = itemView.FindViewById<TextView>(Resource.Id.normalText);

                itemView.Click += (sender, e) => listener(new AdapterClickEvents
                {
                    View = itemView,
                    Position = AdapterPosition
                });

            }
            catch (Exception e)
            {
                Console.WriteLine(e + "Error Allen");

            }
        }
    }

    public class AdapterClickEvents : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }



}