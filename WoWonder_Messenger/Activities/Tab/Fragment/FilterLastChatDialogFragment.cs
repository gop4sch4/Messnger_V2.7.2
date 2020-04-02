using AFollestad.MaterialDialogs;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System;
using System.Collections.Generic;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Helpers.Fonts;
using WoWonder.SQLite;
using Exception = System.Exception;

namespace WoWonder.Activities.Tab.Fragment
{
    public class FilterLastChatDialogFragment : BottomSheetDialogFragment, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private TabbedMainActivity ContextMain;
        private TextView IconBack, IconOnline, IconType, TxtTypeChat, TypeMoreIcon;
        private Button ButtonOffline, ButtonOnline, BothStatusButton, BtnApply;
        private RelativeLayout LayoutType;
        private string Status = "", Type = "all";

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            ContextMain = (TabbedMainActivity)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                Context contextThemeWrapper = AppSettings.SetTabDarkTheme ? new ContextThemeWrapper(Activity, Resource.Style.MyTheme_Dark_Base) : new ContextThemeWrapper(Activity, Resource.Style.MyTheme_Base);
                // clone the inflater using the ContextThemeWrapper
                LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);

                View view = localInflater.Inflate(Resource.Layout.FilterLastChatDialog, container, false);
                // View view = inflater.Inflate(Resource.Layout.BottomSheetFilterLastChat, container, false);

                InitComponent(view);

                IconBack.Click += IconBackOnClick;

                ButtonOffline.Click += ButtonOfflineOnClick;
                ButtonOnline.Click += ButtonOnlineOnClick;
                BothStatusButton.Click += BothStatusButtonOnClick;
                BtnApply.Click += BtnApplyOnClick;
                LayoutType.Click += LayoutTypeOnClick;
                GetFilter();

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
                IconBack = view.FindViewById<TextView>(Resource.Id.IconBack);

                IconOnline = view.FindViewById<TextView>(Resource.Id.IconOnline);
                ButtonOffline = view.FindViewById<Button>(Resource.Id.OfflineButton);
                ButtonOnline = view.FindViewById<Button>(Resource.Id.OnlineButton);
                BothStatusButton = view.FindViewById<Button>(Resource.Id.BothStatusButton);
                BtnApply = view.FindViewById<Button>(Resource.Id.ApplyButton);

                IconType = view.FindViewById<TextView>(Resource.Id.IconType);
                TxtTypeChat = view.FindViewById<TextView>(Resource.Id.TypeChat);
                TypeMoreIcon = view.FindViewById<TextView>(Resource.Id.TypeMoreIcon);

                LayoutType = view.FindViewById<RelativeLayout>(Resource.Id.LayoutType);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconType, IonIconsFonts.Chatbubbles);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, TypeMoreIcon, IonIconsFonts.ChevronRight);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconBack, IonIconsFonts.ChevronLeft);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconOnline, IonIconsFonts.Ionic);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Event

        //Back
        private void IconBackOnClick(object sender, EventArgs e)
        {
            try
            {
                Dismiss();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Save data 
        private void BtnApplyOnClick(object sender, EventArgs e)
        {
            try
            {
                var dbDatabase = new SqLiteDatabase();
                var newSettingsFilter = new DataTables.FilterLastChatTb
                {
                    Status = Status,
                    Type = Type,
                };
                dbDatabase.InsertOrUpdate_FilterLastChat(newSettingsFilter);
                dbDatabase.Dispose();

                ContextMain.LastChatTab.MAdapter.OnlineUsers = Status == "online" || Status == "both";
                ContextMain.LastChatTab.OnlineUsers = Status == "online" || Status == "both";

                MainSettings.SharedData.Edit().PutBoolean("onlineuser_key", ContextMain.LastChatTab.OnlineUsers).Commit();

                ContextMain.LastChatTab.MAdapter.ChatList.Clear();
                ContextMain.LastChatTab.MAdapter.NotifyDataSetChanged();

                if (Status == "both")
                    Status = "";

                ContextMain.LastChatTab.StartApiService(Type, Status);

                Dismiss();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Select Status >> Both (2)
        private void BothStatusButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                //follow_button_profile_friends >> Un click
                //follow_button_profile_friends_pressed >> click
                BothStatusButton.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                BothStatusButton.SetTextColor(Color.ParseColor("#ffffff"));

                ButtonOnline.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonOnline.SetTextColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                ButtonOffline.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonOffline.SetTextColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                Status = "both";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Select Status >> Online (on)
        private void ButtonOnlineOnClick(object sender, EventArgs e)
        {
            try
            {
                //follow_button_profile_friends >> Un click
                //follow_button_profile_friends_pressed >> click
                ButtonOnline.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                ButtonOnline.SetTextColor(Color.ParseColor("#ffffff"));

                BothStatusButton.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                BothStatusButton.SetTextColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                ButtonOffline.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonOffline.SetTextColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                Status = "online";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Select Status >> Offline (off)
        private void ButtonOfflineOnClick(object sender, EventArgs e)
        {
            try
            {
                //follow_button_profile_friends >> Un click
                //follow_button_profile_friends_pressed >> click
                ButtonOffline.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                ButtonOffline.SetTextColor(Color.ParseColor("#ffffff"));

                BothStatusButton.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                BothStatusButton.SetTextColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                ButtonOnline.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonOnline.SetTextColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                Status = "offline";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Type
        private void LayoutTypeOnClick(object sender, EventArgs e)
        {
            try
            {
                var dialogList = new MaterialDialog.Builder(Context).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);

                var arrayAdapter = new List<string>();
                arrayAdapter.Add(Context.GetString(Resource.String.Lbl_All));
                arrayAdapter.Add(Context.GetString(Resource.String.Lbl_User));
                arrayAdapter.Add(Context.GetString(Resource.String.Lbl_Group));
                arrayAdapter.Add(Context.GetString(Resource.String.Lbl_Page));

                dialogList.Title(GetText(Resource.String.Lbl_TypeChat));
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

        #endregion

        private void GetFilter()
        {
            try
            {
                var dbDatabase = new SqLiteDatabase();

                var data = dbDatabase.GetFilterLastChatById();
                if (data != null)
                {
                    Status = data.Status;
                    Type = data.Type;

                    TxtTypeChat.Text = Type;

                    //////////////////////////// Status //////////////////////////////
                    //Select Status >> Both (both)
                    //Select Status >> Online (on)
                    //Select Status >> Offline (off)

                    switch (data.Status)
                    {
                        case "offline":
                            ButtonOffline.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                            ButtonOffline.SetTextColor(Color.ParseColor("#ffffff"));

                            BothStatusButton.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                            BothStatusButton.SetTextColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                            ButtonOnline.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                            ButtonOnline.SetTextColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                            Status = "offline";
                            break;
                        case "online":
                            ButtonOnline.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                            ButtonOnline.SetTextColor(Color.ParseColor("#ffffff"));

                            BothStatusButton.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                            BothStatusButton.SetTextColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                            ButtonOffline.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                            ButtonOffline.SetTextColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                            Status = "online";
                            break;
                        case "both":
                            BothStatusButton.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                            BothStatusButton.SetTextColor(Color.ParseColor("#ffffff"));

                            ButtonOnline.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                            ButtonOnline.SetTextColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                            ButtonOffline.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                            ButtonOffline.SetTextColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                            Status = "both";
                            break;
                    }
                }
                else
                {
                    var newSettingsFilter = new DataTables.FilterLastChatTb
                    {
                        Status = "both",
                        Type = "all",
                    };
                    dbDatabase.InsertOrUpdate_FilterLastChat(newSettingsFilter);

                    Status = "both";
                    Type = "all";

                    TxtTypeChat.Text = Context.GetString(Resource.String.Lbl_All);

                    //////////////////////////// Status ////////////////////////////// 
                    BothStatusButton.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                    BothStatusButton.SetTextColor(Color.ParseColor("#ffffff"));

                    ButtonOnline.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    ButtonOnline.SetTextColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));

                    ButtonOffline.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    ButtonOffline.SetTextColor(AppSettings.SetTabDarkTheme ? Color.ParseColor("#ffffff") : Color.ParseColor("#444444"));
                }

                dbDatabase.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                string text = itemString.ToString();

                if (text == Context.GetString(Resource.String.Lbl_All))
                {
                    Type = "all";
                }
                else if (text == Context.GetString(Resource.String.Lbl_User))
                {
                    Type = "users";
                }
                else if (text == Context.GetString(Resource.String.Lbl_Group))
                {
                    Type = "groups";
                }
                else if (text == Context.GetString(Resource.String.Lbl_Page))
                {
                    Type = "pages";
                }

                TxtTypeChat.Text = text;
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

        #endregion

    }
}