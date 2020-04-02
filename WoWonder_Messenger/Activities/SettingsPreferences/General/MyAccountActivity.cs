using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Linq;
using WoWonder.Helpers.Ads;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Fonts;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using WoWonderClient.Classes.Global;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace WoWonder.Activities.SettingsPreferences.General
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MyAccountActivity : AppCompatActivity, View.IOnClickListener, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private EditText TxtUsername, TxtEmail, TxtBirthday, TxtGender;
        private TextView TxtSave, IconName, IconEmail, IconBirthday, IconGender;
        private string GenderStatus = "";

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                SetTheme(AppSettings.SetTabDarkTheme ? Resource.Style.MyTheme_Dark_Base : Resource.Style.MyTheme_Base);

                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.Settings_MyAccount_Layout);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                Get_Data_User();
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
                TxtSave = FindViewById<TextView>(Resource.Id.toolbar_title);

                IconName = FindViewById<TextView>(Resource.Id.IconName);
                TxtUsername = FindViewById<EditText>(Resource.Id.NameEditText);

                IconEmail = FindViewById<TextView>(Resource.Id.IconEmail);
                TxtEmail = FindViewById<EditText>(Resource.Id.EmailEditText);

                IconBirthday = FindViewById<TextView>(Resource.Id.IconBirthday);
                TxtBirthday = FindViewById<EditText>(Resource.Id.BirthdayEditText);
                TxtBirthday.SetOnClickListener(this);

                IconGender = (TextView)FindViewById(Resource.Id.IconGender);
                TxtGender = (EditText)FindViewById(Resource.Id.GenderEditText);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconName, FontAwesomeIcon.User);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconEmail, FontAwesomeIcon.PaperPlane);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconBirthday, FontAwesomeIcon.BirthdayCake);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconGender, FontAwesomeIcon.VenusMars);

                Methods.SetColorEditText(TxtUsername, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtEmail, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtBirthday, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);
                Methods.SetColorEditText(TxtGender, AppSettings.SetTabDarkTheme ? Color.White : Color.Black);

                Methods.SetFocusable(TxtGender);

                AdsGoogle.Ad_AdMobNative(this);
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
                    toolbar.Title = GetText(Resource.String.Lbl_My_Account);
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
                    TxtGender.Touch += TxtGenderOnTouch;
                    TxtSave.Click += SaveData_OnClick;
                }
                else
                {
                    TxtGender.Touch -= TxtGenderOnTouch;
                    TxtSave.Click -= SaveData_OnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        private void TxtGenderOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                if (ListUtils.SettingsSiteList?.Genders.Count > 0)
                {
                    arrayAdapter.AddRange(from item in ListUtils.SettingsSiteList?.Genders select item.Value);
                }
                else
                {
                    arrayAdapter.Add(GetText(Resource.String.Radio_Male));
                    arrayAdapter.Add(GetText(Resource.String.Radio_Female));
                }

                dialogList.Title(GetText(Resource.String.Lbl_Gender));
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

        //Save data 
        private async void SaveData_OnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    DateTime date = DateTime.Parse(TxtBirthday.Text);
                    string newFormat = date.Day + "-" + date.Month + "-" + date.Year;

                    var dictionary = new Dictionary<string, string>
                    {
                        {"username", TxtUsername.Text.Replace(" ","")},
                        {"email", TxtEmail.Text},
                        {"birthday", newFormat},
                        {"gender", GenderStatus}
                    };

                    var (apiStatus, respond) = await WoWonderClient.Requests.RequestsAsync.Global.Update_User_Data(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is MessageObject result)
                        {
                            if (result.Message.Contains("updated"))
                            {
                                Toast.MakeText(this, result.Message, ToastLength.Short).Show();

                                var local = ListUtils.MyProfileList.FirstOrDefault();
                                if (local != null)
                                {
                                    local.Username = TxtUsername.Text.Replace(" ", "");
                                    local.Email = TxtEmail.Text;
                                    local.Birthday = newFormat;
                                    local.Gender = GenderStatus;
                                    local.GenderText = TxtGender.Text;

                                    var sqLiteDatabase = new SqLiteDatabase();
                                    sqLiteDatabase.Insert_Or_Update_To_MyProfileTable(local);
                                    sqLiteDatabase.Dispose();
                                }

                                AndHUD.Shared.Dismiss(this);
                            }
                            else
                            {
                                //Show a Error image with a message
                                AndHUD.Shared.ShowError(this, result.Message, MaskType.Clear, TimeSpan.FromSeconds(2));
                            }
                        }
                    }
                    else Methods.DisplayReportResult(this, respond);

                    AndHUD.Shared.Dismiss(this);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                //Show a Error image with a message
                AndHUD.Shared.ShowError(this, e.Message, MaskType.Clear, TimeSpan.FromSeconds(2));
                AndHUD.Shared.Dismiss(this);
            }
        }

        #endregion

        private void Get_Data_User()
        {
            try
            {
                var local = ListUtils.MyProfileList.FirstOrDefault();
                if (local != null)
                {
                    TxtUsername.Text = local.Username;
                    TxtEmail.Text = local.Email;

                    try
                    {
                        DateTime date = DateTime.Parse(local.Birthday);
                        string newFormat = date.Day + "/" + date.Month + "/" + date.Year;
                        TxtBirthday.Text = newFormat;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        TxtBirthday.Text = local.Birthday;
                    }

                    if (ListUtils.SettingsSiteList?.Genders.Count > 0)
                    {
                        var value = ListUtils.SettingsSiteList?.Genders?.FirstOrDefault(a => a.Key == local.Gender).Value;
                        if (value != null)
                        {
                            TxtGender.Text = value;
                            GenderStatus = local.Gender;
                        }
                        else
                        {
                            TxtGender.Text = GetText(Resource.String.Radio_Male);
                            GenderStatus = "male";
                        }
                    }
                    else
                    {
                        if (local.Gender == GetText(Resource.String.Radio_Male))
                        {
                            TxtGender.Text = GetText(Resource.String.Radio_Male);
                            GenderStatus = "male";
                        }
                        else if (local.Gender == GetText(Resource.String.Radio_Female))
                        {
                            TxtGender.Text = GetText(Resource.String.Radio_Female);
                            GenderStatus = "female";
                        }
                        else
                        {
                            TxtGender.Text = GetText(Resource.String.Radio_Male);
                            GenderStatus = "male";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region MaterialDialog

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            if (p1 == DialogAction.Positive)
            {

            }
            else if (p1 == DialogAction.Negative)
            {
                p0.Dismiss();
            }
        }

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                if (ListUtils.SettingsSiteList?.Genders.Count > 0)
                {
                    var key = ListUtils.SettingsSiteList?.Genders?.FirstOrDefault(a => a.Value == GenderStatus).Key;
                    if (key != null)
                    {
                        TxtGender.Text = itemString.ToString();
                        GenderStatus = key;
                    }
                    else
                    {
                        TxtGender.Text = itemString.ToString();
                        GenderStatus = "male";
                    }
                }
                else
                {
                    if (itemString.ToString() == GetText(Resource.String.Radio_Male))
                    {
                        TxtGender.Text = GetText(Resource.String.Radio_Male);
                        GenderStatus = "male";
                    }
                    else if (itemString.ToString() == GetText(Resource.String.Radio_Female))
                    {
                        TxtGender.Text = GetText(Resource.String.Radio_Female);
                        GenderStatus = "female";
                    }
                    else
                    {
                        TxtGender.Text = GetText(Resource.String.Radio_Male);
                        GenderStatus = "male";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        public void OnClick(View v)
        {
            try
            {
                if (v.Id == TxtBirthday.Id)
                {
                    var frag = PopupDialogController.DatePickerFragment.NewInstance(delegate (DateTime time)
                    {
                        TxtBirthday.Text = time.ToShortDateString();
                    });
                    frag.Show(SupportFragmentManager, PopupDialogController.DatePickerFragment.Tag);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}