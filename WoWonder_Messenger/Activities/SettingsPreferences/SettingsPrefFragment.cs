using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.OS;
using Android.Provider;
using Android.Support.V7.App;
using Android.Support.V7.Preferences;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WoWonder.Activities.DefaultUser;
using WoWonder.Activities.SettingsPreferences.General;
using WoWonder.Activities.SettingsPreferences.InviteFriends;
using WoWonder.Activities.Tab;
using WoWonder.Frameworks.Floating;
using WoWonder.Frameworks.onesignal;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using WoWonderClient;
using WoWonderClient.Requests;
using Boolean = System.Boolean;
using Exception = System.Exception;
using String = System.String;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.SettingsPreferences
{
    public class SettingsPrefFragment : PreferenceFragmentCompat, ISharedPreferencesOnSharedPreferenceChangeListener, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback, MaterialDialog.IInputCallback
    {
        private Preference EditProfile, BlockedUsers, AccountPref, PasswordPref, GeneralInvitePref, GeneralCallPref, SupportHelpPref, SupportLogoutPref, SupportDeleteAccountPref, SupportReportPref, AboutMePref, TwoFactorPref, ManageSessionsPref;
        //private ListPreference LangPref;
        private Preference PrivacyFollowPref;
        private Preference PrivacyBirthdayPref;
        private Preference PrivacyMessagePref;
        private SwitchPreferenceCompat NotificationPopupPref, ShowOnlineUsersPref, ChatHeadsPref;
        private CheckBoxPreference NotificationPlaySoundPref;
        private Preference NightMode;
        private readonly Activity ActivityContext;
        private string SAbout = "", SWhoCanFollowMe = "0", SWhoCanMessageMe = "0", SWhoCanSeeMyBirthday = "0", TypeDialog = "";
        public static bool SSoundControl = true;
        public static bool SChatHead = true;

        #region General

        public SettingsPrefFragment(Activity activity)
        {
            try
            {
                ActivityContext = activity;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // create ContextThemeWrapper from the original Activity Context with the custom theme
                Context contextThemeWrapper = AppSettings.SetTabDarkTheme ? new ContextThemeWrapper(ActivityContext, Resource.Style.SettingsThemeDark) : new ContextThemeWrapper(ActivityContext, Resource.Style.SettingsTheme);

                // clone the inflater using the ContextThemeWrapper
                LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);

                View view = base.OnCreateView(localInflater, container, savedInstanceState);

                return view;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            try
            {
                // Load the preferences from an XML resource
                AddPreferencesFromResource(Resource.Xml.SettingsPrefs);

                MainSettings.SharedData = PreferenceManager.SharedPreferences;

                InitComponent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();
                PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnPause()
        {
            try
            {
                base.OnPause();
                PreferenceScreen.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                MainSettings.SharedData = PreferenceManager.SharedPreferences;
                PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);

                EditProfile = FindPreference("editprofile_key");
                //LangPref = (ListPreference) FindPreference("Lang_key");
                AboutMePref = FindPreference("about_me_key");
                BlockedUsers = FindPreference("blocked_key");
                AccountPref = FindPreference("editAccount_key");
                PasswordPref = FindPreference("editpassword_key");
                GeneralInvitePref = FindPreference("invite_key");
                GeneralCallPref = FindPreference("Call_key");
                TwoFactorPref = FindPreference("Twofactor_key");
                ManageSessionsPref = FindPreference("ManageSessions_key");
                NightMode = FindPreference("Night_Mode_key");
                SupportReportPref = FindPreference("Report_key");
                SupportHelpPref = FindPreference("help_key");
                SupportLogoutPref = FindPreference("logout_key");
                SupportDeleteAccountPref = FindPreference("deleteaccount_key");
                PrivacyFollowPref = FindPreference("whocanfollow_key");
                PrivacyMessagePref = FindPreference("whocanMessage_key");
                PrivacyBirthdayPref = FindPreference("whocanseemybirthday_key");
                NotificationPopupPref = (SwitchPreferenceCompat)FindPreference("notifications_key");
                ShowOnlineUsersPref = (SwitchPreferenceCompat)FindPreference("onlineuser_key");
                ChatHeadsPref = (SwitchPreferenceCompat)FindPreference("chatheads_key");
                NotificationPlaySoundPref = (CheckBoxPreference)FindPreference("checkBox_PlaySound_key");

                //==================CategoryAccount_key======================
                var mCategoryAccount = (PreferenceCategory)FindPreference("CategoryAccount_key");
                if (!AppSettings.ShowSettingsAccount)
                    mCategoryAccount.RemovePreference(AccountPref);

                if (!AppSettings.ShowSettingsBlockedUsers)
                    mCategoryAccount.RemovePreference(BlockedUsers);

                //==================SecurityAccount_key======================
                var mCategorySecurity = (PreferenceCategory)FindPreference("SecurityAccount_key");
                if (!AppSettings.ShowSettingsPassword)
                    mCategorySecurity.RemovePreference(PasswordPref);

                if (!AppSettings.ShowSettingsTwoFactor)
                    mCategorySecurity.RemovePreference(TwoFactorPref);

                if (!AppSettings.ShowSettingsManageSessions)
                    mCategorySecurity.RemovePreference(ManageSessionsPref);

                //==================category_Support======================
                var mCategorySupport = (PreferenceCategory)FindPreference("category_Support");

                if (!AppSettings.ShowSettingsDeleteAccount)
                    mCategorySupport.RemovePreference(SupportDeleteAccountPref);

                //Add Click event to Preferences
                EditProfile.Intent = new Intent(ActivityContext, typeof(MyProfileActivity));
                BlockedUsers.Intent = new Intent(ActivityContext, typeof(BlockedUsersActivity));
                AccountPref.Intent = new Intent(ActivityContext, typeof(MyAccountActivity));
                PasswordPref.Intent = new Intent(ActivityContext, typeof(PasswordActivity));
                GeneralInvitePref.Intent = new Intent(ActivityContext, typeof(InviteFriendsActivity));

                //Update Preferences data on Load
                OnSharedPreferenceChanged(MainSettings.SharedData, "about_me_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "whocanfollow_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "whocanMessage_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "whocanseemybirthday_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "notifications_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "chatheads_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "onlineuser_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "checkBox_PlaySound_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "Night_Mode_key");

                EditProfile.IconSpaceReserved = false;
                //LangPref.IconSpaceReserved = false;
                AboutMePref.IconSpaceReserved = false;
                BlockedUsers.IconSpaceReserved = false;
                AccountPref.IconSpaceReserved = false;
                PasswordPref.IconSpaceReserved = false;
                GeneralInvitePref.IconSpaceReserved = false;
                GeneralCallPref.IconSpaceReserved = false;
                TwoFactorPref.IconSpaceReserved = false;
                ManageSessionsPref.IconSpaceReserved = false;
                NightMode.IconSpaceReserved = false;
                SupportReportPref.IconSpaceReserved = false;
                SupportHelpPref.IconSpaceReserved = false;
                SupportLogoutPref.IconSpaceReserved = false;
                SupportDeleteAccountPref.IconSpaceReserved = false;
                PrivacyFollowPref.IconSpaceReserved = false;
                PrivacyMessagePref.IconSpaceReserved = false;
                PrivacyBirthdayPref.IconSpaceReserved = false;
                NotificationPopupPref.IconSpaceReserved = false;
                ShowOnlineUsersPref.IconSpaceReserved = false;
                ChatHeadsPref.IconSpaceReserved = false;
                NotificationPlaySoundPref.IconSpaceReserved = false;

                //Support
                SupportReportPref.PreferenceClick += SupportReportPrefOnPreferenceClick;
                SupportHelpPref.PreferenceClick += SupportHelpPrefOnPreferenceClick;
                //Add OnChange event to Preferences
                //LangPref.PreferenceChange += Lang_Pref_PreferenceChange;
                NotificationPopupPref.PreferenceChange += NotificationPopupPrefPreferenceChange;
                ChatHeadsPref.PreferenceChange += ChatHeadsPrefOnPreferenceChange;
                ShowOnlineUsersPref.PreferenceChange += ShowOnlineUsers_Pref_PreferenceChange;
                NotificationPlaySoundPref.PreferenceChange += NotificationPlaySoundPrefPreferenceChange;
                //Event Click Items
                SupportLogoutPref.PreferenceClick += SupportLogout_OnPreferenceClick;
                SupportDeleteAccountPref.PreferenceClick += SupportDeleteAccountPrefOnPreferenceClick;
                NightMode.PreferenceChange += NightModeOnPreferenceChange;
                TwoFactorPref.PreferenceClick += TwoFactorPrefOnPreferenceClick;
                ManageSessionsPref.PreferenceClick += ManageSessionsPrefOnPreferenceClick;

                ChatHeadsPref.Checked = InitFloating.CanDrawOverlays(ActivityContext);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            try
            {
                var datauser = ListUtils.MyProfileList.FirstOrDefault();
                if (key.Equals("about_me_key"))
                {
                    // Set summary to be the user-description for the selected value
                    Preference etp = FindPreference("about_me_key");

                    if (datauser != null)
                    {
                        SAbout = WoWonderTools.GetAboutFinal(datauser);

                        MainSettings.SharedData.Edit().PutString("about_me_key", SAbout).Commit();
                        etp.Summary = SAbout;
                    }

                    string getValue = MainSettings.SharedData.GetString("about_me_key", SAbout);
                    etp.Summary = getValue;
                }
                else if (key.Equals("whocanfollow_key"))
                {
                    // Set summary to be the user-description for the selected value
                    Preference etp = FindPreference("whocanfollow_key");

                    string getValue = MainSettings.SharedData.GetString("whocanfollow_key", datauser?.FollowPrivacy ?? String.Empty);
                    if (getValue == "0")
                    {
                        etp.Summary = ActivityContext.GetText(Resource.String.Lbl_Everyone);
                        SWhoCanFollowMe = "0";
                    }
                    else if (getValue == "1")
                    {
                        etp.Summary = ActivityContext.GetText(Resource.String.Lbl_People_i_Follow);
                        SWhoCanFollowMe = "1";
                    }
                    else
                    {
                        etp.Summary = getValue;
                    }
                }
                else if (key.Equals("whocanMessage_key"))
                {
                    // Set summary to be the user-description for the selected value
                    Preference etp = FindPreference("whocanMessage_key");

                    string getValue = MainSettings.SharedData.GetString("whocanMessage_key", datauser?.MessagePrivacy ?? String.Empty);
                    if (getValue == "0")
                    {
                        etp.Summary = ActivityContext.GetText(Resource.String.Lbl_Everyone);
                        SWhoCanMessageMe = "0";
                    }
                    else if (getValue == "1")
                    {
                        etp.Summary = ActivityContext.GetText(Resource.String.Lbl_People_i_Follow);
                        SWhoCanMessageMe = "1";
                    }
                    else if (getValue == "2")
                    {
                        etp.Summary = ActivityContext.GetText(Resource.String.Lbl_No_body);
                        SWhoCanMessageMe = "2";
                    }
                    else
                    {
                        etp.Summary = getValue;
                    }
                }
                else if (key.Equals("whocanseemybirthday_key"))
                {
                    // Set summary to be the user-description for the selected value
                    Preference etp = FindPreference("whocanseemybirthday_key");

                    string getValue = MainSettings.SharedData.GetString("whocanseemybirthday_key", datauser?.BirthPrivacy ?? String.Empty);
                    if (getValue == "0")
                    {
                        etp.Summary = ActivityContext.GetText(Resource.String.Lbl_Everyone);
                        SWhoCanSeeMyBirthday = "0";
                    }
                    else if (getValue == "1")
                    {
                        etp.Summary = ActivityContext.GetText(Resource.String.Lbl_People_i_Follow);
                        SWhoCanSeeMyBirthday = "1";
                    }
                    else if (getValue == "2")
                    {
                        etp.Summary = ActivityContext.GetText(Resource.String.Lbl_No_body);
                        SWhoCanSeeMyBirthday = "1";
                    }
                    else
                    {
                        etp.Summary = getValue;
                    }
                }
                else if (key.Equals("notifications_key"))
                {
                    bool getValue = MainSettings.SharedData.GetBoolean("notifications_key", true);
                    NotificationPopupPref.Checked = getValue;
                }
                else if (key.Equals("onlineuser_key"))
                {
                    bool getValue = MainSettings.SharedData.GetBoolean("onlineuser_key", true);
                    ShowOnlineUsersPref.Checked = getValue;
                }
                else if (key.Equals("chatheads_key"))
                {
                    bool getValue = MainSettings.SharedData.GetBoolean("chatheads_key", InitFloating.CanDrawOverlays(ActivityContext));
                    ChatHeadsPref.Checked = getValue;
                    SChatHead = getValue;
                }
                else if (key.Equals("checkBox_PlaySound_key"))
                {
                    bool getValue = MainSettings.SharedData.GetBoolean("checkBox_PlaySound_key", true);
                    NotificationPlaySoundPref.Checked = getValue;
                    SSoundControl = getValue;
                }
                else if (key.Equals("Night_Mode_key"))
                {
                    // Set summary to be the user-description for the selected value
                    Preference etp = FindPreference("Night_Mode_key");

                    string getValue = MainSettings.SharedData.GetString("Night_Mode_key", string.Empty);
                    if (getValue == MainSettings.LightMode)
                    {
                        etp.Summary = ActivityContext.GetString(Resource.String.Lbl_Light);
                    }
                    else if (getValue == MainSettings.DarkMode)
                    {
                        etp.Summary = ActivityContext.GetString(Resource.String.Lbl_Dark);
                    }
                    else if (getValue == MainSettings.DefaultMode)
                    {
                        etp.Summary = ActivityContext.GetString(Resource.String.Lbl_SetByBattery);
                    }
                    else
                    {
                        etp.Summary = getValue;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region Event

        //TwoFactor
        private void TwoFactorPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(TwoFactorAuthActivity));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //ManageSessions
        private void ManageSessionsPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(ManageSessionsActivity));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //NightMode
        private void NightModeOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.Handled)
                {
                    var etp = (ListPreference)sender;
                    var value = eventArgs.NewValue.ToString();

                    if (value == MainSettings.LightMode)
                    {
                        //Set Light Mode 
                        etp.Summary = ActivityContext.GetString(Resource.String.Lbl_Light);
                        NightMode.Summary = ActivityContext.GetString(Resource.String.Lbl_Light);

                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
                        AppSettings.SetTabDarkTheme = false;
                        MainSettings.SharedData.Edit().PutString("Night_Mode_key", MainSettings.LightMode).Commit();

                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                        {
                            ActivityContext.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                            ActivityContext.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                        }

                        Intent intent = new Intent(ActivityContext, typeof(SplashScreenActivity));
                        intent.AddCategory(Intent.CategoryHome);
                        intent.SetAction(Intent.ActionMain);
                        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        ActivityContext.StartActivity(intent);
                        ActivityContext.FinishAffinity();
                    }
                    else if (value == MainSettings.DarkMode)
                    {
                        //Set Dark Mode
                        etp.Summary = ActivityContext.GetString(Resource.String.Lbl_Dark);
                        NightMode.Summary = ActivityContext.GetString(Resource.String.Lbl_Dark);

                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
                        AppSettings.SetTabDarkTheme = true;
                        MainSettings.SharedData.Edit().PutString("Night_Mode_key", MainSettings.DarkMode).Commit();

                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                        {
                            ActivityContext.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                            ActivityContext.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                        }

                        Intent intent = new Intent(ActivityContext, typeof(SplashScreenActivity));
                        intent.AddCategory(Intent.CategoryHome);
                        intent.SetAction(Intent.ActionMain);
                        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        ActivityContext.StartActivity(intent);
                        ActivityContext.FinishAffinity();
                    }
                    else if (value == MainSettings.DefaultMode)
                    {
                        etp.Summary = ActivityContext.GetString(Resource.String.Lbl_SetByBattery);
                        NightMode.Summary = ActivityContext.GetString(Resource.String.Lbl_SetByBattery);

                        MainSettings.SharedData.Edit().PutString("Night_Mode_key", MainSettings.DefaultMode).Commit();

                        if ((int)Build.VERSION.SdkInt >= 29)
                        {
                            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightFollowSystem;

                            var currentNightMode = Resources.Configuration.UiMode & UiMode.NightMask;
                            switch (currentNightMode)
                            {
                                case UiMode.NightNo:
                                    // Night mode is not active, we're using the light theme
                                    AppSettings.SetTabDarkTheme = false;
                                    break;
                                case UiMode.NightYes:
                                    // Night mode is active, we're using dark theme
                                    AppSettings.SetTabDarkTheme = true;
                                    break;
                            }
                        }
                        else
                        {
                            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightAuto;

                            var currentNightMode = Resources.Configuration.UiMode & UiMode.NightMask;
                            switch (currentNightMode)
                            {
                                case UiMode.NightNo:
                                    // Night mode is not active, we're using the light theme
                                    AppSettings.SetTabDarkTheme = false;
                                    break;
                                case UiMode.NightYes:
                                    // Night mode is active, we're using dark theme
                                    AppSettings.SetTabDarkTheme = true;
                                    break;
                            }

                            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                            {
                                ActivityContext.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                                ActivityContext.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                            }

                            Intent intent = new Intent(ActivityContext, typeof(SplashScreenActivity));
                            intent.AddCategory(Intent.CategoryHome);
                            intent.SetAction(Intent.ActionMain);
                            intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                            ActivityContext.StartActivity(intent);
                            ActivityContext.FinishAffinity();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Delete Account
        private void SupportDeleteAccountPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                TypeDialog = "Delete";

                var dialog = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                dialog.Title(Resource.String.Lbl_Warning);
                dialog.Content(ActivityContext.GetText(Resource.String.Lbl_Are_you_DeleteAccount) + " " + AppSettings.ApplicationName);
                dialog.PositiveText(ActivityContext.GetText(Resource.String.Lbl_Ok)).OnPositive(this);
                dialog.NegativeText(ActivityContext.GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                dialog.Build().Show();
                dialog.AlwaysCallSingleChoiceCallback();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ShowOnlineUsers_Pref_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            try
            {
                SwitchPreferenceCompat etp = (SwitchPreferenceCompat)sender;
                var value = e.NewValue.ToString();
                etp.Checked = Boolean.Parse(value);

                MainSettings.SharedData.Edit().PutBoolean("onlineuser_key", etp.Checked).Commit();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Help
        private void SupportHelpPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                SupportHelpPref.Intent = new Intent(ActivityContext, typeof(LocalWebViewActivity));
                SupportHelpPref.Intent.PutExtra("Type", ActivityContext.GetText(Resource.String.Lbl_Help));
                SupportHelpPref.Intent.PutExtra("URL", Client.WebsiteUrl + "/terms/about-us");
                ActivityContext.StartActivity(SupportHelpPref.Intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Report >> Contact Us
        private void SupportReportPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                SupportReportPref.Intent = new Intent(ActivityContext, typeof(LocalWebViewActivity));
                SupportReportPref.Intent.PutExtra("Type", ActivityContext.GetText(Resource.String.Lbl_Report_Problem));
                SupportReportPref.Intent.PutExtra("URL", Client.WebsiteUrl + "/contact-us");
                ActivityContext.StartActivity(SupportReportPref.Intent);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Logout
        private void SupportLogout_OnPreferenceClick(object sender, Preference.PreferenceClickEventArgs preferenceClickEventArgs)
        {
            try
            {
                TypeDialog = "Logout";

                var dialog = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                dialog.Title(Resource.String.Lbl_Warning);
                dialog.Content(ActivityContext.GetText(Resource.String.Lbl_Are_you_logout));
                dialog.PositiveText(ActivityContext.GetText(Resource.String.Lbl_Ok)).OnPositive(this);
                dialog.NegativeText(ActivityContext.GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                dialog.AlwaysCallSingleChoiceCallback();
                dialog.Build().Show();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Notification >> Play Sound 
        private void NotificationPlaySoundPrefPreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            try
            {
                if (e.Handled)
                {
                    CheckBoxPreference etp = (CheckBoxPreference)sender;
                    var value = e.NewValue.ToString();
                    etp.Checked = Boolean.Parse(value);
                    if (etp.Checked)
                    {
                        SSoundControl = true;
                    }
                    else
                    {
                        SSoundControl = false;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Notifcation >> Popup 
        private void NotificationPopupPrefPreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            try
            {
                if (e.Handled)
                {
                    SwitchPreferenceCompat etp = (SwitchPreferenceCompat)sender;
                    var value = e.NewValue.ToString();
                    etp.Checked = Boolean.Parse(value);
                    if (etp.Checked)
                    {
                        OneSignalNotification.RegisterNotificationDevice();
                    }
                    else
                    {
                        OneSignalNotification.Un_RegisterNotificationDevice();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //ChatHeads
        private void ChatHeadsPrefOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        {
            try
            {
                if (e.Handled)
                {
                    SwitchPreferenceCompat etp = (SwitchPreferenceCompat)sender;
                    var value = e.NewValue.ToString();
                    etp.Checked = Boolean.Parse(value);
                    SChatHead = etp.Checked;

                    if (SChatHead && !InitFloating.CanDrawOverlays(ActivityContext))
                    {
                        Intent intent = new Intent(Settings.ActionManageOverlayPermission, Uri.Parse("package:" + Application.Context.PackageName));
                        ActivityContext.StartActivityForResult(intent, InitFloating.ChatHeadDataRequestCode);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Lang
        //private void Lang_Pref_PreferenceChange(object sender, Preference.PreferenceChangeEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Handled)
        //        {
        //            ListPreference etp = (ListPreference) sender;
        //            var value = e.NewValue;

        //            MainSettings.SetApplicationLang(value.ToString());

        //            Toast.MakeText(ActivityContext, GetText(Resource.String.Lbl_Closed_App), ToastLength.Long).Show();

        //            Intent intent = new Intent(ActivityContext, typeof(SplashScreenActivity));
        //            intent.AddCategory(Intent.CategoryHome);
        //            intent.SetAction(Intent.ActionMain);
        //            intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
        //            ActivityContext.StartActivity(intent);
        //            ActivityContext.FinishAffinity();
        //        }
        //    }
        //    catch (Exception exception)
        //    {
        //        Console.WriteLine(exception);
        //    }
        //}

        #endregion

        public override bool OnPreferenceTreeClick(Preference preference)
        {
            try
            {
                if (preference.Key == "about_me_key")
                {
                    TypeDialog = "About";
                    var dialog = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);
                    dialog.Title(GetString(Resource.String.Lbl_About));
                    dialog.Input(GetString(Resource.String.Lbl_About), preference.Summary, false, this);
                    dialog.InputType(InputTypes.TextFlagImeMultiLine);
                    dialog.PositiveText(GetText(Resource.String.Lbl_Save)).OnPositive(this);
                    dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                    dialog.AlwaysCallSingleChoiceCallback();
                    dialog.Build().Show();
                }
                else if (preference.Key == "Call_key")
                {
                    TypeDialog = "Call";
                    var dialog = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);
                    dialog.Title(GetText(Resource.String.Lbl_Warning));
                    dialog.Content(GetText(Resource.String.Lbl_Clear_call_log));
                    dialog.PositiveText(GetText(Resource.String.Lbl_Yes)).OnPositive((materialDialog, action) =>
                    {
                        try
                        {
                            TabbedMainActivity.GetInstance().LastCallsTab?.MAdapter?.MCallUser?.Clear();
                            TabbedMainActivity.GetInstance().LastCallsTab?.MAdapter?.NotifyDataSetChanged();

                            TabbedMainActivity.GetInstance().LastCallsTab?.ShowEmptyPage();

                            Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_Done), ToastLength.Long).Show();

                            SqLiteDatabase dbDatabase = new SqLiteDatabase();
                            dbDatabase.Clear_CallUser_List();
                            dbDatabase.Dispose();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    });
                    dialog.NegativeText(GetText(Resource.String.Lbl_No)).OnNegative(this);
                    dialog.AlwaysCallSingleChoiceCallback();
                    dialog.Build().Show();


                }
                else if (preference.Key == "Night_Mode_key")
                {
                    TypeDialog = "NightMode";

                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                    dialogList.Title(Resource.String.Lbl_Theme);

                    arrayAdapter.Add(GetText(Resource.String.Lbl_Light));
                    arrayAdapter.Add(GetText(Resource.String.Lbl_Dark));

                    if ((int)Build.VERSION.SdkInt >= 29)
                        arrayAdapter.Add(GetText(Resource.String.Lbl_SetByBattery));

                    dialogList.Items(arrayAdapter);
                    dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show();
                }
                else if (preference.Key == "whocanfollow_key")
                {
                    TypeDialog = "WhoCanFollow";

                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                    dialogList.Title(Resource.String.Lbl_Who_can_follow_me);

                    arrayAdapter.Add(GetText(Resource.String.Lbl_Everyone)); //>> value = 0
                    arrayAdapter.Add(GetText(Resource.String.Lbl_People_i_Follow)); //>> value = 1

                    dialogList.Items(arrayAdapter);
                    dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show();
                }
                else if (preference.Key == "whocanseemybirthday_key")
                {
                    TypeDialog = "Birthday";

                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                    dialogList.Title(Resource.String.Lbl_Who_can_see_my_birthday);

                    arrayAdapter.Add(GetText(Resource.String.Lbl_Everyone)); //>> value = 0
                    arrayAdapter.Add(GetText(Resource.String.Lbl_People_i_Follow)); //>> value = 1
                    arrayAdapter.Add(GetText(Resource.String.Lbl_No_body)); //>> value = 2

                    dialogList.Items(arrayAdapter);
                    dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show();
                }
                else if (preference.Key == "whocanMessage_key")
                {
                    TypeDialog = "Message";

                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialDialog.Builder(ActivityContext).Theme(AppSettings.SetTabDarkTheme ? Theme.Dark : Theme.Light);

                    dialogList.Title(Resource.String.Lbl_Who_can_message_me);

                    arrayAdapter.Add(GetText(Resource.String.Lbl_Everyone)); //>> value = 0
                    arrayAdapter.Add(GetText(Resource.String.Lbl_People_i_Follow)); //>> value = 1
                    arrayAdapter.Add(GetText(Resource.String.Lbl_No_body)); //>> value = 2

                    dialogList.Items(arrayAdapter);
                    dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                    dialogList.AlwaysCallSingleChoiceCallback();
                    dialogList.ItemsCallback(this).Build().Show();
                }


                return base.OnPreferenceTreeClick(preference);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return base.OnPreferenceTreeClick(preference);
            }
        }

        #region MaterialDialog

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (TypeDialog == "Delete")
                {
                    if (p1 == DialogAction.Positive)
                    {
                        var intent = new Intent(ActivityContext, typeof(DeleteAccountActivity));
                        ActivityContext.StartActivity(intent);
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
                else if (TypeDialog == "Logout")
                {
                    if (p1 == DialogAction.Positive)
                    {
                        // Check if we're running on Android 5.0 or higher
                        if ((int)Build.VERSION.SdkInt < 23)
                        {
                            Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_You_will_be_logged), ToastLength.Long).Show();
                            ApiRequest.Logout(ActivityContext);
                        }
                        else
                        {
                            if (ActivityContext.CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                                ActivityContext.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                            {
                                Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_You_will_be_logged),
                                    ToastLength.Long).Show();
                                ApiRequest.Logout(ActivityContext);
                            }
                            else
                                RequestPermissions(new[]
                                {
                                    Manifest.Permission.ReadExternalStorage,
                                    Manifest.Permission.WriteExternalStorage
                                }, 101);
                        }
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
                else
                {
                    if (p1 == DialogAction.Positive)
                    {
                    }
                    else if (p1 == DialogAction.Negative)
                    {
                        p0.Dismiss();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                string text = itemString.ToString();
                var dataUser = ListUtils.MyProfileList.FirstOrDefault();

                if (TypeDialog == "NightMode")
                {
                    string getValue = MainSettings.SharedData.GetString("Night_Mode_key", string.Empty);

                    if (text == GetString(Resource.String.Lbl_Light) && getValue != MainSettings.LightMode)
                    {
                        //Set Light Mode   
                        NightMode.Summary = ActivityContext.GetString(Resource.String.Lbl_Light);

                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightNo;
                        AppSettings.SetTabDarkTheme = false;
                        MainSettings.SharedData.Edit().PutString("Night_Mode_key", MainSettings.LightMode).Commit();

                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                        {
                            ActivityContext.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                            ActivityContext.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                        }

                        Intent intent = new Intent(ActivityContext, typeof(SplashScreenActivity));
                        intent.AddCategory(Intent.CategoryHome);
                        intent.SetAction(Intent.ActionMain);
                        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        ActivityContext.StartActivity(intent);
                        ActivityContext.FinishAffinity();
                    }
                    else if (text == GetString(Resource.String.Lbl_Dark) && getValue != MainSettings.DarkMode)
                    {
                        NightMode.Summary = ActivityContext.GetString(Resource.String.Lbl_Dark);

                        AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightYes;
                        AppSettings.SetTabDarkTheme = true;
                        MainSettings.SharedData.Edit().PutString("Night_Mode_key", MainSettings.DarkMode).Commit();

                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                        {
                            ActivityContext.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                            ActivityContext.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                        }

                        Intent intent = new Intent(ActivityContext, typeof(SplashScreenActivity));
                        intent.AddCategory(Intent.CategoryHome);
                        intent.SetAction(Intent.ActionMain);
                        intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                        ActivityContext.StartActivity(intent);
                        ActivityContext.FinishAffinity();
                    }
                    else if (text == GetString(Resource.String.Lbl_SetByBattery) && getValue != MainSettings.DefaultMode)
                    {
                        NightMode.Summary = ActivityContext.GetString(Resource.String.Lbl_SetByBattery);
                        MainSettings.SharedData.Edit().PutString("Night_Mode_key", MainSettings.DefaultMode).Commit();

                        if ((int)Build.VERSION.SdkInt >= 29)
                        {
                            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightFollowSystem;

                            var currentNightMode = Resources.Configuration.UiMode & UiMode.NightMask;
                            switch (currentNightMode)
                            {
                                case UiMode.NightNo:
                                    // Night mode is not active, we're using the light theme
                                    AppSettings.SetTabDarkTheme = false;
                                    break;
                                case UiMode.NightYes:
                                    // Night mode is active, we're using dark theme
                                    AppSettings.SetTabDarkTheme = true;
                                    break;
                            }
                        }
                        else
                        {
                            AppCompatDelegate.DefaultNightMode = AppCompatDelegate.ModeNightAuto;

                            var currentNightMode = Resources.Configuration.UiMode & UiMode.NightMask;
                            switch (currentNightMode)
                            {
                                case UiMode.NightNo:
                                    // Night mode is not active, we're using the light theme
                                    AppSettings.SetTabDarkTheme = false;
                                    break;
                                case UiMode.NightYes:
                                    // Night mode is active, we're using dark theme
                                    AppSettings.SetTabDarkTheme = true;
                                    break;
                            }

                            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                            {
                                ActivityContext.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                                ActivityContext.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                            }

                            Intent intent = new Intent(ActivityContext, typeof(SplashScreenActivity));
                            intent.AddCategory(Intent.CategoryHome);
                            intent.SetAction(Intent.ActionMain);
                            intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.ClearTask);
                            ActivityContext.StartActivity(intent);
                            ActivityContext.FinishAffinity();
                        }
                    }

                }
                else if (TypeDialog == "WhoCanFollow")
                {
                    if (text == GetString(Resource.String.Lbl_Everyone))
                    {
                        MainSettings.SharedData.Edit().PutString("WhoCanFollow", "0").Commit();
                        PrivacyFollowPref.Summary = text;
                        SWhoCanFollowMe = "0";
                    }
                    else if (text == GetString(Resource.String.Lbl_People_i_Follow))
                    {
                        MainSettings.SharedData.Edit().PutString("WhoCanFollow", "1").Commit();
                        PrivacyFollowPref.Summary = text;
                        SWhoCanFollowMe = "1";
                    }

                    if (dataUser != null)
                    {
                        dataUser.FollowPrivacy = SWhoCanFollowMe;
                    }

                    if (Methods.CheckConnectivity())
                    {
                        var dataPrivacy = new Dictionary<string, string>
                        {
                            {"follow_privacy", SWhoCanFollowMe}
                        };
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Update_User_Data(dataPrivacy) });
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                    }
                }
                else if (TypeDialog == "Birthday")
                {
                    if (text == GetString(Resource.String.Lbl_Everyone))
                    {
                        MainSettings.SharedData.Edit().PutString("whocanseemybirthday_key", "0").Commit();
                        PrivacyBirthdayPref.Summary = text;
                        SWhoCanSeeMyBirthday = "0";
                    }
                    else if (text == GetString(Resource.String.Lbl_People_i_Follow))
                    {
                        MainSettings.SharedData.Edit().PutString("whocanseemybirthday_key", "1").Commit();
                        PrivacyBirthdayPref.Summary = text;
                        SWhoCanSeeMyBirthday = "1";
                    }
                    else if (text == GetString(Resource.String.Lbl_No_body))
                    {
                        MainSettings.SharedData.Edit().PutString("whocanseemybirthday_key", "2").Commit();
                        PrivacyBirthdayPref.Summary = text;
                        SWhoCanSeeMyBirthday = "2";
                    }

                    if (dataUser != null)
                    {
                        dataUser.BirthPrivacy = SWhoCanSeeMyBirthday;
                    }

                    if (Methods.CheckConnectivity())
                    {
                        var dataPrivacy = new Dictionary<string, string>
                        {
                            {"birth_privacy", SWhoCanSeeMyBirthday}
                        };
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Update_User_Data(dataPrivacy) });
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                    }
                }
                else if (TypeDialog == "Message")
                {
                    if (text == GetString(Resource.String.Lbl_Everyone))
                    {
                        MainSettings.SharedData.Edit().PutString("whocanMessage_key", "0").Commit();
                        PrivacyMessagePref.Summary = text;
                        SWhoCanMessageMe = "0";
                    }
                    else if (text == GetString(Resource.String.Lbl_People_i_Follow))
                    {
                        MainSettings.SharedData.Edit().PutString("whocanMessage_key", "1").Commit();
                        PrivacyMessagePref.Summary = text;
                        SWhoCanMessageMe = "1";
                    }
                    else if (text == GetString(Resource.String.Lbl_No_body))
                    {
                        MainSettings.SharedData.Edit().PutString("whocanMessage_key", "2").Commit();
                        PrivacyMessagePref.Summary = text;
                        SWhoCanMessageMe = "2";
                    }

                    if (dataUser != null)
                    {
                        dataUser.MessagePrivacy = SWhoCanMessageMe;
                    }

                    if (Methods.CheckConnectivity())
                    {
                        var dataPrivacy = new Dictionary<string, string>
                        {
                            {"message_privacy", SWhoCanMessageMe}
                        };
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Update_User_Data(dataPrivacy) });
                    }
                    else
                    {
                        Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnInput(MaterialDialog p0, ICharSequence p1)
        {
            try
            {
                if (p1.Length() <= 0) return;

                var strName = p1.ToString();
                if (!string.IsNullOrEmpty(strName) || !string.IsNullOrWhiteSpace(strName))
                {
                    if (TypeDialog == "About")
                    {
                        MainSettings.SharedData.Edit().PutString("about_me_key", strName).Commit();
                        AboutMePref.Summary = strName;

                        var dataUser = ListUtils.MyProfileList.FirstOrDefault();
                        if (dataUser != null)
                        {
                            dataUser.About = strName;
                            SAbout = strName;
                        }

                        if (Methods.CheckConnectivity())
                        {
                            var dataPrivacy = new Dictionary<string, string>
                            {
                                {"about", strName}
                            };

                            PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => RequestsAsync.Global.Update_User_Data(dataPrivacy) });
                        }
                        else
                        {
                            Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                if (requestCode == InitFloating.ChatHeadDataRequestCode && InitFloating.CanDrawOverlays(ActivityContext))
                {
                    SChatHead = true;
                    MainSettings.SharedData.Edit().PutBoolean("chatheads_key", SChatHead).Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}