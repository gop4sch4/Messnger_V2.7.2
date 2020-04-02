using AFollestad.MaterialDialogs;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Bumptech.Glide.Util;
using Java.Lang;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WoWonder.Activities.Story;
using WoWonder.Activities.Story.Adapter;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonderClient.Classes.Story;
using WoWonderClient.Requests;
using Exception = System.Exception;

namespace WoWonder.Activities.Tab.Fragment
{
    public class LastStoriesFragment : Android.Support.V4.App.Fragment, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        public StoryAdapter MAdapter;
        private SwipeRefreshLayout SwipeRefreshLayout;
        public RecyclerView MRecycler;
        private LinearLayoutManager LayoutManager;
        public ViewStub EmptyStateLayout;
        private View Inflated;
        private ImageView ProfileUserImage;
        private RelativeLayout AddStoryLayout;
        public RelativeLayout AddNewLayout;
        private readonly TabbedMainActivity ContextActivity;

        #endregion

        #region General

        public LastStoriesFragment(TabbedMainActivity context)
        {
            ContextActivity = context;
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.StoryFragmentLayout, container, false);

                InitComponent(view);
                SetRecyclerViewAdapters();

                StartApiService();

                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                //Add Story
                AddNewLayout = view.FindViewById<RelativeLayout>(Resource.Id.addNewLayout);
                AddStoryLayout = view.FindViewById<RelativeLayout>(Resource.Id.rlMain);
                MRecycler = (RecyclerView)view.FindViewById(Resource.Id.recyler);
                EmptyStateLayout = view.FindViewById<ViewStub>(Resource.Id.viewStub);
                ProfileUserImage = view.FindViewById<ImageView>(Resource.Id.ivUser);
                SwipeRefreshLayout = (SwipeRefreshLayout)view.FindViewById(Resource.Id.swipeRefreshLayout);
                SwipeRefreshLayout.SetColorSchemeResources(Android.Resource.Color.HoloBlueLight, Android.Resource.Color.HoloGreenLight, Android.Resource.Color.HoloOrangeLight, Android.Resource.Color.HoloRedLight);
                SwipeRefreshLayout.Refreshing = true;
                SwipeRefreshLayout.Enabled = true;
                SwipeRefreshLayout.SetProgressBackgroundColorSchemeColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#424242") : Color.ParseColor("#f7f7f7"));
                SwipeRefreshLayout.Refresh += SwipeRefreshLayoutOnRefresh;
                //Add story click
                AddStoryLayout.Click += AddStoryLayout_Click;

                //Allen To be removed
                Glide.With(Context).Load(UserDetails.Avatar).Apply(new RequestOptions().SetDiskCacheStrategy(DiskCacheStrategy.All).CenterCrop()).Into(ProfileUserImage);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void AddStoryLayout_Click(object sender, EventArgs e)
        {
            try
            {
                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(Context).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                arrayAdapter.Add(GetText(Resource.String.image));
                arrayAdapter.Add(GetText(Resource.String.video));

                dialogList.Title(GetText(Resource.String.Lbl_Addnewstory));
                dialogList.Items(arrayAdapter);
                dialogList.NegativeText(GetText(Resource.String.Lbl_Close)).OnNegative(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SetRecyclerViewAdapters()
        {
            try
            {
                MAdapter = new StoryAdapter(Activity) { StoryList = new ObservableCollection<GetUserStoriesObject.StoryObject>() };
                MAdapter.ItemClick += MAdapterOnItemClick;
                LayoutManager = new LinearLayoutManager(Activity);
                MRecycler.SetLayoutManager(LayoutManager);
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(10);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<GetUserStoriesObject.StoryObject>(Activity, MAdapter, sizeProvider, 10);
                MRecycler.AddOnScrollListener(preLoader);
                MRecycler.SetAdapter(MAdapter);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Event

        //Refresh
        private void SwipeRefreshLayoutOnRefresh(object sender, EventArgs e)
        {
            try
            {
                MAdapter.StoryList.Clear();
                MAdapter.NotifyDataSetChanged();

                StartApiService();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void MAdapterOnItemClick(object sender, StoryAdapterClickEventArgs e)
        {
            try
            {
                var item = MAdapter.GetItem(e.Position);
                if (item != null)
                {
                    Intent intent = new Intent(Context, typeof(ViewStoryActivity));
                    intent.PutExtra("UserId", item.UserId);
                    intent.PutExtra("DataItem", JsonConvert.SerializeObject(item));
                    Context.StartActivity(intent);

                    item.ProfileIndicator = AppSettings.StoryReadColor;
                    MAdapter.NotifyItemChanged(e.Position);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Get Story

        public void StartApiService()
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(Activity, Context.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { LoadStory });
        }

        private async Task LoadStory()
        {
            if (Methods.CheckConnectivity())
            {
                (int apiStatus, var respond) = await RequestsAsync.Story.Get_UserStories();
                if (apiStatus == 200)
                {
                    if (respond is GetUserStoriesObject result)
                    {
                        foreach (var item in result.Stories)
                        {
                            var check = MAdapter.StoryList.FirstOrDefault(a => a.UserId == item.UserId);
                            if (check != null)
                            {
                                foreach (var item2 in item.Stories)
                                {
                                    if (item.DurationsList == null)
                                        item.DurationsList = new List<long>();

                                    //image and video
                                    var mediaFile = !item2.Thumbnail.Contains("avatar") && item2.Videos.Count == 0 ? item2.Thumbnail : item2.Videos[0].Filename;

                                    var type = Methods.AttachmentFiles.Check_FileExtension(mediaFile);
                                    if (type != "Video")
                                    {
                                        Glide.With(Context).Load(mediaFile).Apply(new RequestOptions().SetDiskCacheStrategy(DiskCacheStrategy.All).CenterCrop()).Preload();
                                        item.DurationsList.Add(AppSettings.StoryDuration);
                                    }
                                    else
                                    {
                                        var fileName = mediaFile.Split('/').Last();
                                        mediaFile = WoWonderTools.GetFile(DateTime.Now.Day.ToString(), Methods.Path.FolderDiskStory, fileName, mediaFile);

                                        var duration = WoWonderTools.GetDuration(mediaFile);
                                        item.DurationsList.Add(Long.ParseLong(duration));
                                    }
                                }

                                check.Stories = item.Stories;
                            }
                            else
                            {
                                foreach (var item1 in item.Stories)
                                {
                                    if (item.DurationsList == null)
                                        item.DurationsList = new List<long>();

                                    //image and video
                                    var mediaFile = !item1.Thumbnail.Contains("avatar") && item1.Videos.Count == 0 ? item1.Thumbnail : item1.Videos[0].Filename;

                                    var type1 = Methods.AttachmentFiles.Check_FileExtension(mediaFile);
                                    if (type1 != "Video")
                                    {
                                        Glide.With(Context).Load(mediaFile).Apply(new RequestOptions().SetDiskCacheStrategy(DiskCacheStrategy.All).CenterCrop()).Preload();
                                        item.DurationsList.Add(AppSettings.StoryDuration);
                                    }
                                    else
                                    {
                                        var fileName = mediaFile.Split('/').Last();
                                        WoWonderTools.GetFile(DateTime.Now.Day.ToString(), Methods.Path.FolderDiskStory, fileName, mediaFile);

                                        var duration = WoWonderTools.GetDuration(mediaFile);
                                        item.DurationsList.Add(Long.ParseLong(duration));
                                    }
                                }

                                MAdapter.StoryList.Add(item);
                            }
                        }

                        Activity.RunOnUiThread(() => { MAdapter.NotifyDataSetChanged(); });
                    }
                }
                else Methods.DisplayReportResult(Activity, respond);

                Activity.RunOnUiThread(ShowEmptyPage);
            }
            else
            {
                Inflated = EmptyStateLayout.Inflate();
                EmptyStateInflater x = new EmptyStateInflater();
                x.InflateLayout(Inflated, EmptyStateInflater.Type.NoConnection);
                if (!x.EmptyStateButton.HasOnClickListeners)
                {
                    x.EmptyStateButton.Click += null;
                    x.EmptyStateButton.Click += EmptyStateButtonOnClick;
                }

                Toast.MakeText(Context, Context.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            }
        }

        public void ShowEmptyPage()
        {
            try
            {
                SwipeRefreshLayout.Refreshing = false;

                if (MAdapter.StoryList.Count > 0)
                {
                    MRecycler.Visibility = ViewStates.Visible;
                    EmptyStateLayout.Visibility = ViewStates.Gone;
                }
                else
                {
                    MRecycler.Visibility = ViewStates.Gone;

                    if (Inflated == null)
                        Inflated = EmptyStateLayout.Inflate();

                    EmptyStateInflater x = new EmptyStateInflater();
                    x.InflateLayout(Inflated, EmptyStateInflater.Type.NoStory);
                    if (!x.EmptyStateButton.HasOnClickListeners)
                    {
                        x.EmptyStateButton.Click += null;
                    }
                    EmptyStateLayout.Visibility = ViewStates.Visible;
                }
            }
            catch (Exception e)
            {
                SwipeRefreshLayout.Refreshing = false;
                Console.WriteLine(e);
            }
        }

        //No Internet Connection 
        private void EmptyStateButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartApiService();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        public void OnSelection(MaterialDialog p0, View p1, int p2, ICharSequence itemString)
        {
            try
            {
                if (itemString.ToString() == GetText(Resource.String.image))
                {
                    ContextActivity.OnImage_Button_Click();
                }
                else if (itemString.ToString() == GetText(Resource.String.video))
                {
                    ContextActivity.OnVideo_Button_Click();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}