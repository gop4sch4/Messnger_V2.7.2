using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;
using Com.Luseen.Autolinklibrary;
using System;
using System.Linq;
using WoWonder.Activities.DefaultUser;
using WoWonder.Helpers.Controller;
using WoWonder.SQLite;

namespace WoWonder.Helpers.Utils
{
    public class TextSanitizer
    {
        private readonly AutoLinkTextView AutoLinkTextView;
        private readonly Activity Activity;

        public TextSanitizer(AutoLinkTextView linkTextView, Activity activity)
        {
            try
            {
                AutoLinkTextView = linkTextView;
                Activity = activity;
                AutoLinkTextView.AutoLinkOnClick += AutoLinkTextViewOnAutoLinkOnClick;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void Load(string text)
        {
            try
            {
                AutoLinkTextView.AddAutoLinkMode(AutoLinkMode.ModePhone, AutoLinkMode.ModeEmail, AutoLinkMode.ModeHashtag, AutoLinkMode.ModeUrl, AutoLinkMode.ModeMention, AutoLinkMode.ModeCustom);
                AutoLinkTextView.SetPhoneModeColor(ContextCompat.GetColor(Activity, Resource.Color.left_ModePhone_color));
                AutoLinkTextView.SetEmailModeColor(ContextCompat.GetColor(Activity, Resource.Color.left_ModeEmail_color));
                AutoLinkTextView.SetHashtagModeColor(ContextCompat.GetColor(Activity, Resource.Color.left_ModeHashtag_color));
                AutoLinkTextView.SetUrlModeColor(ContextCompat.GetColor(Activity, Resource.Color.left_ModeUrl_color));
                AutoLinkTextView.SetMentionModeColor(Color.ParseColor(AppSettings.MainColor));
                var textSplit = text.Split('/');
                if (textSplit.Count() > 1)
                {
                    AutoLinkTextView.SetCustomModeColor(ContextCompat.GetColor(Activity, Resource.Color.left_ModeUrl_color));
                    AutoLinkTextView.SetCustomRegex(@"\b(" + textSplit.LastOrDefault() + @")\b");
                }

                string lastString = text.Replace(" /", " ");
                if (!string.IsNullOrEmpty(lastString))
                    AutoLinkTextView.SetAutoLinkText(lastString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void AutoLinkTextViewOnAutoLinkOnClick(object sender, AutoLinkOnClickEventArgs autoLinkOnClickEventArgs)
        {
            try
            {
                var typetext = Methods.FunString.Check_Regex(autoLinkOnClickEventArgs.P1.Replace(" ", ""));
                if (typetext == "Email")
                {
                    Methods.App.SendEmail(Activity, autoLinkOnClickEventArgs.P1.Replace(" ", ""));
                }
                else if (typetext == "Website")
                {
                    string url = autoLinkOnClickEventArgs.P1.Replace(" ", "");
                    if (!autoLinkOnClickEventArgs.P1.Contains("http"))
                    {
                        url = "http://" + autoLinkOnClickEventArgs.P1.Replace(" ", "");
                    }

                    //var intent = new Intent(Activity, typeof(LocalWebViewActivity));
                    //intent.PutExtra("URL", url);
                    //intent.PutExtra("Type", url);
                    //Activity.StartActivity(intent);
                    new IntentController(Activity).OpenBrowserFromApp(url);
                }
                else if (typetext == "Hashtag")
                {

                }
                else if (typetext == "Mention")
                {
                    var dataUSer = ListUtils.MyProfileList.FirstOrDefault();
                    string name = autoLinkOnClickEventArgs.P1.Replace("@", "").Replace(" ", "");

                    var sqlEntity = new SqLiteDatabase();
                    var user = sqlEntity.Get_DataOneUser(name);
                    sqlEntity.Dispose();

                    if (user != null)
                    {
                        WoWonderTools.OpenProfile(Activity, user.UserId, user);
                    }
                    else
                    {
                        if (name == dataUSer?.Name || name == dataUSer?.Username)
                        {
                            var intent = new Intent(Activity, typeof(MyProfileActivity));
                            Activity.StartActivity(intent);
                        }
                        else
                        {
                            var intent = new Intent(Activity, typeof(SearchActivity));
                            intent.PutExtra("Key", autoLinkOnClickEventArgs.P1.Replace("@", "").Replace(" ", ""));
                            Activity.StartActivity(intent);
                        }
                    }
                }
                else if (typetext == "Number")
                {
                    Methods.App.SaveContacts(Activity, autoLinkOnClickEventArgs.P1.Replace(" ", ""), "", "2");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}