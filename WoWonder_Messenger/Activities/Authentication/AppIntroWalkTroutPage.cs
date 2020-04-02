using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using AppIntro;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WoWonder.Activities.SuggestedUsers;
using WoWonder.Activities.Tab;
using WoWonder.Frameworks.onesignal;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;

namespace WoWonder.Activities.Authentication
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/Theme.AppCompat.Light.NoActionBar", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.Orientation)]
    public class AppIntroWalkTroutPage : AppIntro2
    {
        private int Count;
        private string Caller = "";
        private RequestBuilder FullGlideRequestBuilder;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                AddSlide(new AnimFragment1());
                AddSlide(new AnimFragment2());
                AddSlide(new AnimFragment3());
                AddSlide(new AnimFragment4());

                if (AppSettings.WalkThroughSetFlowAnimation)
                    SetFlowAnimation();
                else if (AppSettings.WalkThroughSetZoomAnimation)
                    SetZoomAnimation();
                else if (AppSettings.WalkThroughSetSlideOverAnimation)
                    SetSlideOverAnimation();
                else if (AppSettings.WalkThroughSetDepthAnimation)
                    SetDepthAnimation();
                else if (AppSettings.WalkThroughSetFadeAnimation) SetFadeAnimation();

                ShowStatusBar(false);

                //SetNavBarColor(Color.ParseColor(AppSettings.MainColor));
                SetIndicatorColor(Color.ParseColor(AppSettings.MainColor), Color.ParseColor("#888888"));
                //SetBarColor(Color.ParseColor("#3F51B5"));
                // SetSeparatorColor(Color.ParseColor("#2196f3"));

                Caller = Intent.GetStringExtra("class");

                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt >= 23)
                {
                    RequestPermissions(new[]
                    {
                        Manifest.Permission.AccessFineLocation,
                        Manifest.Permission.AccessCoarseLocation
                    }, 1);
                }

                //OneSignal Notification  
                //====================================== 
                if (string.IsNullOrEmpty(UserDetails.DeviceId))
                    OneSignalNotification.RegisterNotificationDevice();

                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.Get_MyProfileData_Api(this) });

                FullGlideRequestBuilder = Glide.With(this).AsDrawable().SetDiskCacheStrategy(DiskCacheStrategy.Automatic).SkipMemoryCache(true).Override(200);

                List<string> stickerList = new List<string>();
                stickerList.AddRange(Stickers.StickerList1);
                stickerList.AddRange(Stickers.StickerList2);
                stickerList.AddRange(Stickers.StickerList3);
                stickerList.AddRange(Stickers.StickerList4);
                stickerList.AddRange(Stickers.StickerList5);
                stickerList.AddRange(Stickers.StickerList6);

                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        foreach (var item in stickerList)
                        {
                            FullGlideRequestBuilder.Load(item).Preload();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                if (grantResults[0] == Permission.Granted)
                {

                }
                else
                {
                    //Permission Denied :(
                    Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnSlideChanged()
        {
            try
            {
                base.OnSlideChanged();
                Pressed();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        public override void OnNextPressed()
        {
            try
            {
                base.OnNextPressed();
                Pressed();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Pressed()
        {
            try
            {
                if (Count == 0)
                {
                    if ((int)Build.VERSION.SdkInt >= 23)
                    {
                        if (AppSettings.ShowButtonContact)
                        {
                            RequestPermissions(new[]
                            {
                                Manifest.Permission.ReadContacts,
                                Manifest.Permission.ReadPhoneNumbers,
                                Manifest.Permission.GetAccounts,

                                Manifest.Permission.Camera
                            }, 2);
                        }
                        else
                        {
                            RequestPermissions(new[]
                            {
                                Manifest.Permission.Camera
                            }, 2);
                        }
                    }

                    Count++;
                }
                else if (Count == 1)
                {
                    if ((int)Build.VERSION.SdkInt >= 23)
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.WriteExternalStorage,
                            Manifest.Permission.AccessMediaLocation,
                        }, 3);
                    }
                    Count++;
                }
                else if (Count == 2)
                {
                    if ((int)Build.VERSION.SdkInt >= 23)
                    {
                        if (AppSettings.ShowButtonRecordSound)
                        {
                            RequestPermissions(new[]
                            {
                                Manifest.Permission.RecordAudio,
                                Manifest.Permission.ModifyAudioSettings
                            }, 4);
                        }
                    }

                    Count++;
                }
                else if (Count == 3)
                {
                    Count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Do something when users tap on Done button.
        public override void OnDonePressed()
        {
            try
            {
                if (Caller.Equals("register"))
                {
                    StartActivity(AppSettings.ShowSuggestedUsersOnRegister ? new Intent(this, typeof(SuggestionsUsersActivity)) : new Intent(this, typeof(TabbedMainActivity)));
                }
                else
                    StartActivity(new Intent(this, typeof(TabbedMainActivity)));

                Finish();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Do something when users tap on Skip button.
        public override void OnSkipPressed()
        {
            try
            {
                if (Caller.Equals("register"))
                {
                    StartActivity(AppSettings.ShowSuggestedUsersOnRegister ? new Intent(this, typeof(SuggestionsUsersActivity)) : new Intent(this, typeof(TabbedMainActivity)));

                }
                else
                    StartActivity(new Intent(this, typeof(TabbedMainActivity)));

                Finish();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}