using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Java.Util;
using System;
using System.Globalization;
using System.Threading;
using WoWonder.Helpers.Model;

namespace WoWonder.Helpers.Controller
{
    public class LangController : ContextWrapper
    {
        private Context Context;

        protected LangController(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public LangController(Context context) : base(context)
        {
            Context = context;
        }

        public static ContextWrapper Wrap(Context context, string language)
        {
            try
            {
                Configuration config = context.Resources.Configuration;

                var sysLocale = config.Locales.Get(0);

                if (!language.Equals("") && !sysLocale.Language.Equals(language))
                {
                    sysLocale = new Locale(language);
                    Locale.Default = sysLocale;
                }
                SetCulture(language);
                config.SetLocale(sysLocale);

                var ss = context.Resources.Configuration.Locale;
                Console.WriteLine(ss);
                //SharedPref.SharedData.Edit().PutString("Lang_key", language).Commit();

                //context = context.CreateConfigurationContext(config);
#pragma warning disable 618
                context.Resources.UpdateConfiguration(config, null);
#pragma warning restore 618

                return new LangController(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new LangController(context);
            }
        }

        public static void SetDefaultAppSettings()
        {
            try
            {
                //Shared_Data.Edit().PutString("Lang_key", "Auto").Commit();
                if (AppSettings.Lang != "")
                {
                    if (AppSettings.Lang == "ar")
                    {
                        //SharedPref.SharedData.Edit().PutString("Lang_key", "ar").Commit();
                        AppSettings.Lang = "ar";
                        AppSettings.FlowDirectionRightToLeft = true;
                    }
                    else
                    {
                        // SharedPref.SharedData.Edit().PutString("Lang_key", AppSettings.Lang).Commit();
                        AppSettings.FlowDirectionRightToLeft = false;
                    }
                }
                else
                {
                    AppSettings.FlowDirectionRightToLeft = false;

                    //var Lang = SharedPref.SharedData.GetString("Lang_key", AppSettings.Lang);
                    //if (Lang == "ar")
                    //{
                    //    SharedPref.SharedData.Edit().PutString("Lang_key", "ar").Commit();
                    //    AppSettings.Lang = "ar";
                    //    AppSettings.FlowDirectionRightToLeft = true;
                    //}
                    //else if (Lang == "Auto")
                    //{
                    //    SharedPref.SharedData.Edit().PutString("Lang_key", "Auto").Commit();
                    //}
                    //else
                    //{
                    //    SharedPref.SharedData.Edit().PutString("Lang_key", Lang).Commit();
                    //}
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public static void SetApplicationLang(Context activityContext, string lang)
        {
            try
            {
                var res = activityContext.Resources; // Get the string 

                Configuration config = activityContext.Resources.Configuration;

                AppSettings.Lang = lang;

                var locale = new Locale(lang);

                Configuration conf = res.Configuration;
                conf.SetLocale(locale);

                Locale.Default = locale;

                if ((int)Build.VERSION.SdkInt > 17)
                    conf.SetLayoutDirection(locale);

                DisplayMetrics dm = res.DisplayMetrics;

                UserDetails.LangName = lang;
                AppSettings.Lang = lang;
                AppSettings.FlowDirectionRightToLeft = config.Locale.Language.Contains("ar");

                if ((int)Build.VERSION.SdkInt >= 24)
                {
                    LocaleList localeList = new LocaleList(locale);
                    LocaleList.Default = localeList;
                    conf.Locales = localeList;

                    //Locale.SetDefault(Locale.Category.Display, locale);
                    activityContext = activityContext.CreateConfigurationContext(conf);
                    //return activityContext;
                }

                conf.Locale = locale;
#pragma warning disable 618
                res.UpdateConfiguration(conf, dm);
#pragma warning restore 618

                //SetDefaultAppSettings();
                Wrap(activityContext, config.Locale.Language);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //public static ContextWrapper SetApplicationLang(Context context, string language)
        //{
        //    try
        //    {
        //        Locale locale = new Locale(language);
        //        Locale.SetDefault(Locale.Category.Display, locale);

        //        LocaleList localeList = new LocaleList(locale);
        //        LocaleList.Default = localeList;

        //        Resources res = context.Resources;
        //        Configuration config = new Configuration(res.Configuration);
        //        if ((int)Build.VERSION.SdkInt > 17)
        //        {
        //            config.SetLocale(locale);
        //            config.Locale = locale;
        //            config.SetLayoutDirection(locale);

        //            CultureInfo myCulture = new CultureInfo(language);
        //            CultureInfo.DefaultThreadCurrentCulture = myCulture;

        //            localeList = new LocaleList(locale);
        //            LocaleList.Default = localeList;
        //            config.Locales = localeList;

        //            context = context.CreateConfigurationContext(config);

        //            res.UpdateConfiguration(config, res.DisplayMetrics);
        //        }

        //        var lang = context.Resources.Configuration.Locale.DisplayLanguage.ToLower();

        //        UserDetails.LangName = language;
        //        AppSettings.Lang = language;
        //        AppSettings.FlowDirectionRightToLeft = config.Locale.Language.Contains("ar");

        //        Toast.MakeText(context, "set good lang :" + config.Locale.Language, ToastLength.Long).Show();

        //        return new LangController(context);
        //    }
        //    catch (Exception exception)
        //    {
        //        Toast.MakeText(context, exception.Message, ToastLength.Long).Show();

        //        Console.WriteLine(exception);
        //        return new LangController(context);
        //    }
        //}

        //public static ContextWrapper SetApplicationLang(Context context, string language)
        //{
        //    try
        //    {
        //        Configuration config = context.Resources.Configuration;
        //        Locale sysLocale;
        //        if (Build.VERSION.SdkInt > BuildVersionCodes.N)
        //        {
        //            sysLocale = config.Locales.Get(0);
        //        }
        //        else
        //        {
        //            sysLocale = config.Locale;
        //        }

        //        if (!language.Equals("") && !sysLocale.Language.Equals(language))
        //        {
        //            Locale locale = new Locale(language);
        //            Locale.SetDefault(Locale.Category.Display , locale);
        //            if (Build.VERSION.SdkInt > BuildVersionCodes.N)
        //            {
        //                config.SetLocale(locale);
        //            }
        //            else
        //            {
        //                config.Locale = locale; 
        //            } 
        //        }

        //        if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1)
        //        {
        //            context = context.CreateConfigurationContext(config);
        //        }
        //        else
        //        {
        //            context.Resources.UpdateConfiguration(config, context.Resources.DisplayMetrics);
        //        }

        //        UserDetails.LangName = language;
        //        AppSettings.Lang = language;
        //        AppSettings.FlowDirectionRightToLeft = config.Locale.Language.Contains("ar");

        //        SetCulture(language);

        //        return new ContextWrapper(context);

        //        //Resources res = context.Resources;
        //        //Configuration config = new Configuration();

        //        //Locale locale = new Locale(language);
        //        //Locale.SetDefault(Locale.Category.Display, locale);

        //        //LocaleList localeList = new LocaleList(locale);
        //        //LocaleList.Default = localeList;

        //        //Locale.Builder dd = new Locale.Builder().SetLanguage(language);
        //        //dd.Build();

        //        //if ((int)Build.VERSION.SdkInt > 17)
        //        //{
        //        //    var sysLocale = config.Locales.Get(0);

        //        //    config.SetLocale(locale);
        //        //    config.Locale = locale;

        //        //    config.SetLayoutDirection(locale);

        //        //    if (!language.Equals("") && !sysLocale.Language.Equals(language))
        //        //    {
        //        //        sysLocale = new Locale(language);
        //        //        Locale.Default = sysLocale;
        //        //    }

        //        //    SetCulture(language);

        //        //    config.SetLocale(sysLocale);

        //        //    context = context.CreateConfigurationContext(config);

        //        //    //res.UpdateConfiguration(config, res.DisplayMetrics);
        //        //}

        //        //var lang = context.Resources.Configuration.Locale.DisplayLanguage.ToLower();

        //        //UserDetails.LangName = language;
        //        //AppSettings.Lang = language;
        //        //AppSettings.FlowDirectionRightToLeft = config.Locale.Language.Contains("ar");

        //        //return new LangController(context);
        //    }
        //    catch (Exception exception)
        //    {
        //        Console.WriteLine(exception);
        //        return new LangController(context);
        //    }

        //}

        private static void SetCulture(string language)
        {
            try
            {
                CultureInfo myCulture = new CultureInfo(language);
                CultureInfo.DefaultThreadCurrentCulture = myCulture;
                Thread.CurrentThread.CurrentCulture = myCulture;
                Thread.CurrentThread.CurrentUICulture = myCulture;

                new ChineseLunisolarCalendar();
                new HebrewCalendar();
                new HijriCalendar();
                new JapaneseCalendar();
                new JapaneseLunisolarCalendar();
                new KoreanCalendar();
                new KoreanLunisolarCalendar();
                new PersianCalendar();
                new TaiwanCalendar();
                new TaiwanLunisolarCalendar();
                new ThaiBuddhistCalendar();
                new UmAlQuraCalendar();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

}