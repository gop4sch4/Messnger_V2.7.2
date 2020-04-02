using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using Java.Lang;
using Newtonsoft.Json;
using System;
using WoWonder.Activities.Authentication;
using WoWonder.Activities.ChatWindow;
using WoWonder.Activities.Tab;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using Exception = System.Exception;

namespace WoWonder.Activities
{
    [Activity(Icon = "@mipmap/icon", MainLauncher = true, Theme = "@style/SplashScreenTheme", NoHistory = true, ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreenActivity : AppCompatActivity
    {
        private SqLiteDatabase DbDatabase = new SqLiteDatabase();

        protected override void OnResume()
        {
            try
            {
                base.OnResume();

                if ((int)Build.VERSION.SdkInt < 23)
                {
                    new Handler(Looper.MainLooper).Post(new Runnable(FirstRunExcite));
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                    {
                        new Handler(Looper.MainLooper).Post(new Runnable(FirstRunExcite));
                    }
                    else
                    {
                        RequestPermissions(new[]
                        {
                            Manifest.Permission.ReadExternalStorage,
                            Manifest.Permission.WriteExternalStorage,
                            Manifest.Permission.AccessMediaLocation,
                        }, 101);
                    }
                }
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
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 101)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        new Handler(Looper.MainLooper).Post(new Runnable(FirstRunExcite));
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                        Finish();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void FirstRunExcite()
        {
            try
            {
                DbDatabase = new SqLiteDatabase();
                DbDatabase.CheckTablesStatus();

                if (!string.IsNullOrEmpty(AppSettings.Lang))
                {
                    LangController.SetApplicationLang(this, AppSettings.Lang);
                }
                else
                {
                    UserDetails.LangName = Resources.Configuration.Locale.Language.ToLower();
                    LangController.SetApplicationLang(this, UserDetails.LangName);
                }

                var result = DbDatabase.Get_data_Login_Credentials();
                if (result != null)
                {
                    var settingsData = DbDatabase.GetSettings();
                    if (settingsData != null)
                        ListUtils.SettingsSiteList = settingsData;

                    DbDatabase = new SqLiteDatabase();

                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                        ListUtils.UserList = DbDatabase.Get_LastUsersChat_List();
                    else
                        ListUtils.UserChatList = DbDatabase.GetLastUsersChatList();

                    var data = Intent.GetStringExtra("UserID") ?? "Data not available";
                    if (data != "Data not available" && !string.IsNullOrEmpty(data))
                    {
                        Intent intent = new Intent(this, typeof(ChatWindowActivity));
                        intent.PutExtra("UserID", data); // to_id
                        intent.PutExtra("Notifier", "Notifier");
                        intent.PutExtra("App", "Timeline");
                        intent.PutExtra("Name", Intent.GetStringExtra("Name"));
                        intent.PutExtra("Username", Intent.GetStringExtra("Username"));
                        intent.PutExtra("Time", Intent.GetStringExtra("Time"));
                        intent.PutExtra("LastSeen", Intent.GetStringExtra("LastSeen"));
                        intent.PutExtra("About", Intent.GetStringExtra("About"));
                        intent.PutExtra("Address", Intent.GetStringExtra("Address"));
                        intent.PutExtra("Phone", Intent.GetStringExtra("Phone"));
                        intent.PutExtra("Website", Intent.GetStringExtra("Website"));
                        intent.PutExtra("Working", Intent.GetStringExtra("Working"));
                        StartActivity(intent);
                    }
                    else
                    {
                        switch (result.Status)
                        {
                            case "Active":
                                StartActivity(new Intent(this, typeof(TabbedMainActivity)));
                                break;
                            default:
                                StartActivity(CrossAppAuthentication() ? new Intent(this, typeof(FirstActivity)) : new Intent(this, typeof(LoginActivity)));
                                break;
                        }
                    }
                }
                else
                {
                    var data = Intent.GetStringExtra("UserID") ?? "Data not available";
                    if (data != "Data not available" && !string.IsNullOrEmpty(data))
                    {
                        Intent intent = new Intent(this, typeof(ChatWindowActivity));
                        intent.PutExtra("UserID", data); // to_id
                        intent.PutExtra("Notifier", "Notifier");
                        intent.PutExtra("App", "Timeline");
                        intent.PutExtra("Name", Intent.GetStringExtra("Name"));
                        intent.PutExtra("Username", Intent.GetStringExtra("Username"));
                        intent.PutExtra("Time", Intent.GetStringExtra("Time"));
                        intent.PutExtra("LastSeen", Intent.GetStringExtra("LastSeen"));
                        intent.PutExtra("About", Intent.GetStringExtra("About"));
                        intent.PutExtra("Address", Intent.GetStringExtra("Address"));
                        intent.PutExtra("Phone", Intent.GetStringExtra("Phone"));
                        intent.PutExtra("Website", Intent.GetStringExtra("Website"));
                        intent.PutExtra("Working", Intent.GetStringExtra("Working"));
                        StartActivity(intent);
                    }
                    else
                    {
                        StartActivity(CrossAppAuthentication() ? new Intent(this, typeof(FirstActivity)) : new Intent(this, typeof(LoginActivity)));
                    }


                }

                DbDatabase.Dispose();

                if (AppSettings.ShowAdMobBanner || AppSettings.ShowAdMobInterstitial || AppSettings.ShowAdMobRewardVideo || AppSettings.ShowAdMobNative)
                    MobileAds.Initialize(this, GetString(Resource.String.admob_app_id));
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }
        }

        private bool CrossAppAuthentication()
        {
            try
            {
                var loginTb = JsonConvert.DeserializeObject<DataTables.LoginTb>(Methods.ReadNoteOnSD());
                return loginTb != null && !string.IsNullOrEmpty(loginTb.AccessToken) && !string.IsNullOrEmpty(loginTb.Username);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}