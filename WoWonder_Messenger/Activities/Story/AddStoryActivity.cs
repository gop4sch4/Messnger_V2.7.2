using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AT.Markushi.UI;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Developer.SEmojis.Actions;
using Developer.SEmojis.Helper;
using Java.Lang;
using JP.ShTs.StoriesProgressView;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WoWonder.Activities.Story.Service;
using WoWonder.Activities.Tab;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using Exception = System.Exception;
using File = Java.IO.File;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.Story
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class AddStoryActivity : AppCompatActivity
    {
        #region Variables Basic

        private ImageView StoryImageView;
        private VideoView StoryVideoView;
        private AppCompatImageView EmojisView;
        private CircleButton PlayIconVideo, AddStoryButton;
        private EmojiconEditText EmojisIconEditText;
        private RelativeLayout RootView;
        private string PathStory = "", Type = "", Thumbnail = UserDetails.Avatar;
        private StoriesProgressView StoriesProgress;
        private long Duration;
        private TabbedMainActivity GlobalContext;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                Window.SetSoftInputMode(SoftInput.AdjustResize);
                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                base.OnCreate(savedInstanceState);

                // Create your application here
                SetContentView(Resource.Layout.AddStory_layout);
                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();

                GlobalContext = TabbedMainActivity.GetInstance();

                Thumbnail = Intent.GetStringExtra("Thumbnail") ?? UserDetails.Avatar;

                var dataUri = Intent.GetStringExtra("Uri") ?? "Data not available";
                if (dataUri != "Data not available" && !string.IsNullOrEmpty(dataUri)) PathStory = dataUri; // Uri file 
                var dataType = Intent.GetStringExtra("Type") ?? "Data not available";
                if (dataType != "Data not available" && !string.IsNullOrEmpty(dataType)) Type = dataType; // Type file  

                if (Type == "image")
                    SetImageStory(PathStory);
                else
                    SetVideoStory(PathStory);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                AddOrRemoveEvent(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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

        protected override void OnDestroy()
        {
            try
            {
                // Very important !
                StoriesProgress.Destroy();

                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnDestroy();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                StoryImageView = FindViewById<ImageView>(Resource.Id.imagstoryDisplay);
                StoryVideoView = FindViewById<VideoView>(Resource.Id.VideoView);
                PlayIconVideo = FindViewById<CircleButton>(Resource.Id.Videoicon_button);
                EmojisView = FindViewById<AppCompatImageView>(Resource.Id.emojiicon);
                EmojisIconEditText = FindViewById<EmojiconEditText>(Resource.Id.EmojiconEditText5);
                AddStoryButton = FindViewById<CircleButton>(Resource.Id.sendButton);
                RootView = FindViewById<RelativeLayout>(Resource.Id.storyDisplay);

                StoriesProgress = FindViewById<StoriesProgressView>(Resource.Id.stories);
                StoriesProgress.Visibility = ViewStates.Gone;
                EmojisView.Visibility = ViewStates.Gone;

                Methods.SetColorEditText(EmojisIconEditText, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);

                var emojisIcon = new EmojIconActions(this, RootView, EmojisIconEditText, EmojisView);
                emojisIcon.ShowEmojIcon();

                PlayIconVideo.Visibility = ViewStates.Gone;
                PlayIconVideo.Tag = "Play";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = GetString(Resource.String.Lbl_Addnewstory);
                    toolbar.SetTitleTextColor(Color.White);
                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    AddStoryButton.Click += AddStoryButtonOnClick;
                    StoryVideoView.Completion += StoryVideoViewOnCompletion;
                    PlayIconVideo.Click += PlayIconVideoOnClick;
                }
                else
                {
                    AddStoryButton.Click -= AddStoryButtonOnClick;
                    StoryVideoView.Completion -= StoryVideoViewOnCompletion;
                    PlayIconVideo.Click -= PlayIconVideoOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetImageStory(string url)
        {
            try
            {
                if (StoryImageView.Visibility == ViewStates.Gone)
                    StoryImageView.Visibility = ViewStates.Visible;

                var file = Uri.FromFile(new File(url));

                Glide.With(this).Load(file.Path).Apply(new RequestOptions()).Into(StoryImageView);

                // GlideImageLoader.LoadImage(this, file.Path, StoryImageView, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);

                if (StoryVideoView.Visibility == ViewStates.Visible)
                    StoryVideoView.Visibility = ViewStates.Gone;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetVideoStory(string url)
        {
            try
            {
                if (StoryImageView.Visibility == ViewStates.Visible)
                    StoryImageView.Visibility = ViewStates.Gone;

                if (StoryVideoView.Visibility == ViewStates.Gone)
                    StoryVideoView.Visibility = ViewStates.Visible;

                PlayIconVideo.Visibility = ViewStates.Visible;
                PlayIconVideo.Tag = "Play";
                PlayIconVideo.SetImageResource(Resource.Drawable.ic_play_arrow);

                if (StoryVideoView.IsPlaying)
                    StoryVideoView.Suspend();

                if (url.Contains("http"))
                {
                    StoryVideoView.SetVideoURI(Uri.Parse(url));
                }
                else
                {
                    var file = Uri.FromFile(new File(url));
                    StoryVideoView.SetVideoPath(file.Path);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        private void PlayIconVideoOnClick(object sender, EventArgs e)
        {
            try
            {
                if (PlayIconVideo.Tag.ToString() == "Play")
                {
                    MediaMetadataRetriever retriever;
                    if (PathStory.Contains("http"))
                    {
                        StoryVideoView.SetVideoURI(Uri.Parse(PathStory));

                        retriever = new MediaMetadataRetriever();
                        if ((int)Build.VERSION.SdkInt >= 14)
                            retriever.SetDataSource(PathStory, new Dictionary<string, string>());
                        else
                            retriever.SetDataSource(PathStory);
                    }
                    else
                    {
                        var file = Uri.FromFile(new File(PathStory));
                        StoryVideoView.SetVideoPath(file.Path);

                        retriever = new MediaMetadataRetriever();
                        //if ((int)Build.VERSION.SdkInt >= 14)
                        //    retriever.SetDataSource(file.Path, new Dictionary<string, string>());
                        //else
                        //    retriever.SetDataSource(file.Path);
                        retriever.SetDataSource(file.Path);
                    }
                    StoryVideoView.Start();

                    Duration = Long.ParseLong(retriever.ExtractMetadata(MetadataKey.Duration));
                    retriever.Release();

                    StoriesProgress.Visibility = ViewStates.Visible;
                    StoriesProgress.SetStoriesCount(1); // <- set stories
                    StoriesProgress.SetStoryDuration(Duration); // <- set a story duration
                    StoriesProgress.StartStories(); // <- start progress

                    PlayIconVideo.Tag = "Stop";
                    PlayIconVideo.SetImageResource(Resource.Drawable.ic_stop_white_24dp);
                }
                else
                {
                    StoriesProgress.Visibility = ViewStates.Gone;
                    StoriesProgress.Pause();

                    StoryVideoView.Pause();

                    PlayIconVideo.Tag = "Play";
                    PlayIconVideo.SetImageResource(Resource.Drawable.ic_play_arrow);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void StoryVideoViewOnCompletion(object sender, EventArgs e)
        {
            try
            {
                StoriesProgress.Visibility = ViewStates.Gone;
                StoriesProgress.Pause();
                StoryVideoView.Pause();

                PlayIconVideo.Tag = "Play";
                PlayIconVideo.SetImageResource(Resource.Drawable.ic_play_arrow);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //add
        private void AddStoryButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    var item = new FileUpload()
                    {
                        StoryFileType = Type,
                        StoryFilePath = PathStory,
                        StoryDescription = EmojisIconEditText.Text,
                        StoryTitle = EmojisIconEditText.Text,
                        StoryThumbnail = Thumbnail,
                    };

                    Intent intent = new Intent(this, typeof(StoryService));
                    intent.SetAction(StoryService.ActionStory);
                    intent.PutExtra("DataPost", JsonConvert.SerializeObject(item));
                    StartService(intent);

                    Finish();
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                AndHUD.Shared.Dismiss(this);
            }
        }

        #endregion
    }
}