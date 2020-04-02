using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Bumptech.Glide;
using Bumptech.Glide.Integration.RecyclerView;
using Bumptech.Glide.Request;
using Bumptech.Glide.Util;
using Com.Theartofdev.Edmodo.Cropper;
using Java.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WoWonder.Activities.DefaultUser.Adapters;
using WoWonder.Activities.Tab;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.GroupChat;
using WoWonderClient.Requests;
using Console = System.Console;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Uri = Android.Net.Uri;

namespace WoWonder.Activities.GroupChat
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class EditGroupActivity : AppCompatActivity, MaterialDialog.ISingleButtonCallback,
        AppBarLayout.IOnOffsetChangedListener
    {
        #region Variables Basic

        private TextView TxtAddUser, BtnExitGroup;
        private EditText TxtGroupName;
        private ImageView ImageGroup;
        private Button BtnImage, BtnDeleteGroup;
        private ContactsAdapter MAdapter;
        private RecyclerView MRecycler;
        private LinearLayoutManager LayoutManager;
        private string GroupPathImage = "", UsersIds, Type = "", GroupId;
        private List<UserDataObject> NewUserList;
        private ChatObject GroupData;
        private AppBarLayout AppBarLayout;
        private CollapsingToolbarLayout CollapsingToolbar;
        private FloatingActionButton BtnAdd;
        private int Position;
        private TabbedMainActivity GlobalContext;

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
                SetContentView(Resource.Layout.CreateGroupLayout);

                string obj = Intent.GetStringExtra("GroupObject") ?? "";
                if (!string.IsNullOrEmpty(obj))
                {
                    GroupData = JsonConvert.DeserializeObject<ChatObject>(obj);
                    GroupId = GroupData.GroupId;
                }

                GlobalContext = TabbedMainActivity.GetInstance();

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                SetRecyclerViewAdapters();

                LoadContacts();
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
                CollapsingToolbar = (CollapsingToolbarLayout)FindViewById(Resource.Id.collapsingToolbar);
                CollapsingToolbar.Title = "";

                AppBarLayout = FindViewById<AppBarLayout>(Resource.Id.appBarLayout);
                AppBarLayout.SetExpanded(true);
                AppBarLayout.AddOnOffsetChangedListener(this);


                MRecycler = (RecyclerView)FindViewById(Resource.Id.userRecyler);
                TxtGroupName = FindViewById<EditText>(Resource.Id.groupName);
                ImageGroup = FindViewById<ImageView>(Resource.Id.groupCover);
                BtnImage = FindViewById<Button>(Resource.Id.btn_selectimage);

                TxtAddUser = FindViewById<TextView>(Resource.Id.addUser);

                BtnAdd = FindViewById<FloatingActionButton>(Resource.Id.fab);

                BtnDeleteGroup = FindViewById<Button>(Resource.Id.deleteGroupButton);
                BtnDeleteGroup.Visibility = ViewStates.Visible;

                BtnExitGroup = FindViewById<TextView>(Resource.Id.exitGroupButton);
                BtnExitGroup.Visibility = ViewStates.Visible;

                if (AppSettings.SetTabDarkTheme)
                {
                    TxtGroupName.SetTextColor(Color.White);
                    TxtGroupName.SetHintTextColor(Color.White);
                }
                else
                {
                    TxtGroupName.SetTextColor(Color.ParseColor("#444444"));
                    TxtGroupName.SetHintTextColor(Color.ParseColor("#444444"));
                }
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
                    toolbar.Title = GetString(Resource.String.Lbl_CreateGroup);
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

        private void SetRecyclerViewAdapters()
        {
            try
            {
                MAdapter = new ContactsAdapter(this, false, ContactsAdapter.TypeTextSecondary.About)
                {
                    UserList = new ObservableCollection<UserDataObject>(),
                };
                LayoutManager = new LinearLayoutManager(this);
                MRecycler.SetLayoutManager(LayoutManager);
                MRecycler.HasFixedSize = true;
                MRecycler.SetItemViewCacheSize(10);
                MRecycler.GetLayoutManager().ItemPrefetchEnabled = true;
                var sizeProvider = new FixedPreloadSizeProvider(10, 10);
                var preLoader = new RecyclerViewPreloader<UserDataObject>(this, MAdapter, sizeProvider, 10);
                MRecycler.AddOnScrollListener(preLoader);
                MRecycler.SetAdapter(MAdapter);

                MRecycler.Visibility = ViewStates.Visible;

                // Add first image Default  
                MAdapter.UserList.Add(new UserDataObject()
                {
                    UserId = "0",
                    Avatar = "addImage",
                    Name = GetString(Resource.String.Lbl_AddParticipants),
                    About = "....."
                });
                MAdapter.NotifyDataSetChanged();
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
                    BtnAdd.Click += TxtAddOnClick;
                    TxtAddUser.Click += TxtAddUserOnClick;
                    BtnImage.Click += BtnImageOnClick;
                    MAdapter.ItemClick += MAdapterOnItemClick;
                    BtnExitGroup.Click += BtnExitGroupOnClick;
                    BtnDeleteGroup.Click += BtnDeleteGroupOnClick;
                    MAdapter.ItemLongClick += MAdapterOnItemLongClick;
                }
                else
                {
                    BtnAdd.Click -= TxtAddOnClick;
                    TxtAddUser.Click -= TxtAddUserOnClick;
                    BtnImage.Click -= BtnImageOnClick;
                    MAdapter.ItemClick -= MAdapterOnItemClick;
                    BtnExitGroup.Click -= BtnExitGroupOnClick;
                    BtnDeleteGroup.Click -= BtnDeleteGroupOnClick;
                    MAdapter.ItemLongClick -= MAdapterOnItemLongClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        private void BtnImageOnClick(object sender, EventArgs e)
        {
            try
            {
                OpenDialogGallery(); //requestCode >> 500 => Image Gallery
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Edit Group
        private async void TxtAddOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)
                        .Show();
                }
                else
                {
                    if (string.IsNullOrEmpty(TxtGroupName.Text) || string.IsNullOrWhiteSpace(TxtGroupName.Text))
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Please_enter_name), ToastLength.Short).Show();
                        return;
                    }

                    if (TxtGroupName.Text.Length < 4 && TxtGroupName.Text.Length > 15)
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_ErrorLengthGroupName), ToastLength.Short)
                            .Show();
                        return;
                    }

                    if (GroupPathImage == GroupData.Avatar)
                    {
                        GroupPathImage = "";
                    }

                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    var (apiStatus, respond) = await RequestsAsync.GroupChat.EditGroupChat(GroupId, TxtGroupName.Text,
                            GroupPathImage);
                    if (apiStatus == 200)
                    {
                        if (respond is CreateGroupChatObject result)
                        {
                            //Add new item to my Group list
                            if (result.Data?.Count > 0)
                            {
                                RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                                        {
                                            var adapter = GlobalContext?.LastChatTab?.MAdapter;
                                            var data = adapter?.ChatList.FirstOrDefault(a => a.GroupId == GroupId);
                                            if (data != null)
                                            {
                                                var index = adapter.ChatList.IndexOf(data);
                                                if (index > -1)
                                                {
                                                    GroupData = result.Data[0];
                                                    adapter.ChatList[index] = result.Data[0];

                                                    adapter.NotifyDataSetChanged();

                                                }
                                            }
                                        }
                                        else
                                        {
                                            var adapter = GlobalContext?.LastGroupChatsTab?.MAdapter;
                                            var data = adapter?.LastGroupList.FirstOrDefault(a => a.GroupId == GroupData.GroupId);
                                            if (data != null)
                                            {
                                                var index = adapter.LastGroupList.IndexOf(data);
                                                if (index > -1)
                                                {
                                                    GroupData = result.Data[0];
                                                    adapter.LastGroupList[index] = result.Data[0];

                                                    adapter.NotifyDataSetChanged();

                                                }
                                            }
                                        }
                                    }
                                    catch (Exception exception)
                                    {
                                        Console.WriteLine(exception);
                                    }
                                });
                            }

                            AndHUD.Shared.ShowSuccess(this);

                            var resultIntent = new Intent();
                            resultIntent.PutExtra("GroupName", TxtGroupName.Text);
                            SetResult(Result.Ok, resultIntent);

                            Finish();
                        }
                    }
                    else Methods.DisplayReportResult(this, respond);

                    AndHUD.Shared.Dismiss(this);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                AndHUD.Shared.Dismiss(this);
            }
        }

        //Add User
        private void TxtAddUserOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivityForResult(new Intent(this, typeof(MentionActivity)), 3);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //leave group chat
        private void BtnExitGroupOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
                else
                {
                    Type = "Exit";

                    var dialog = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme ? AFollestad.MaterialDialogs.Theme.Dark : AFollestad.MaterialDialogs.Theme.Light);
                    dialog.Content(GetText(Resource.String.Lbl_AreYouSureExitGroup));
                    dialog.PositiveText(GetText(Resource.String.Lbl_Exit)).OnPositive(this);
                    dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                    dialog.AlwaysCallSingleChoiceCallback();
                    dialog.Build().Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //delete group chat
        private void BtnDeleteGroupOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!Methods.CheckConnectivity())
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short)
                        .Show();
                }
                else
                {
                    Type = "Delete";

                    var dialog = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme
                        ? AFollestad.MaterialDialogs.Theme.Dark
                        : AFollestad.MaterialDialogs.Theme.Light);
                    dialog.Content(GetText(Resource.String.Lbl_AreYouSureDeleteGroup));
                    dialog.PositiveText(GetText(Resource.String.Lbl_DeleteGroup)).OnPositive(this);
                    dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                    dialog.AlwaysCallSingleChoiceCallback();
                    dialog.Build().Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void MAdapterOnItemClick(object sender, ContactsAdapterClickEventArgs adapterClickEvents)
        {
            try
            {
                var position = adapterClickEvents.Position;
                if (position >= 0)
                {
                    var item = MAdapter.GetItem(position);
                    if (item == null) return;
                    if (item.Avatar != "addImage") return;
                    StartActivityForResult(new Intent(this, typeof(MentionActivity)), 3);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void MAdapterOnItemLongClick(object sender, ContactsAdapterClickEventArgs e)
        {
            try
            {
                var position = e.Position;
                if (position >= 0)
                {
                    var item = MAdapter.GetItem(position);
                    if (item == null) return;
                    if (item.Avatar == "addImage") return;
                    Type = "RemoveUser";
                    Position = e.Position;
                    var dialog = new MaterialDialog.Builder(this).Theme(AppSettings.SetTabDarkTheme
                        ? AFollestad.MaterialDialogs.Theme.Dark
                        : AFollestad.MaterialDialogs.Theme.Light);
                    dialog.Title(GetString(Resource.String.Lbl_Remove) + " " + WoWonderTools.GetNameFinal(item));
                    dialog.PositiveText(GetText(Resource.String.Lbl_Yes)).OnPositive(this);
                    dialog.NegativeText(GetText(Resource.String.Lbl_No)).OnNegative(this);
                    dialog.AlwaysCallSingleChoiceCallback();
                    dialog.Build().Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Permissions && Result

        //Result
        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);
                //If its from Camera or Gallery
                if (requestCode == CropImage.CropImageActivityRequestCode)
                {
                    var result = CropImage.GetActivityResult(data);

                    if (resultCode == Result.Ok)
                    {
                        if (result.IsSuccessful)
                        {
                            var resultUri = result.Uri;

                            if (!string.IsNullOrEmpty(resultUri.Path))
                            {
                                GroupPathImage = resultUri.Path;
                                File file2 = new File(resultUri.Path);
                                var photoUri =
                                    FileProvider.GetUriForFile(this, PackageName + ".fileprovider", file2);
                                Glide.With(this).Load(photoUri).Apply(new RequestOptions()).Into(ImageGroup);

                                //GlideImageLoader.LoadImage(this, resultUri.Path, ImageGroup, ImageStyle.RoundedCrop, ImagePlaceholders.Drawable);
                            }
                            else
                            {
                                Toast.MakeText(this, GetText(Resource.String.Lbl_Something_went_wrong),
                                    ToastLength.Long).Show();
                            }
                        }
                    }
                }
                else if (requestCode == 3)
                {
                    var users = data.GetStringExtra("Users") ?? "";
                    NewUserList = JsonConvert.DeserializeObject<List<UserDataObject>>(users);

                    UsersIds = "";
                    foreach (var user in NewUserList)
                    {
                        UsersIds += user.UserId + ",";

                        var dataUser =
                            MAdapter.UserList.FirstOrDefault(attachments => attachments.UserId == user.UserId);
                        if (dataUser == null)
                        {
                            MAdapter.UserList.Add(user);
                        }
                    }

                    UsersIds = UsersIds.Remove(UsersIds.Length - 1, 1);

                    MAdapter.NotifyDataSetChanged();

                    var (apiStatus, respond) = await RequestsAsync.GroupChat
                        .AddOrRemoveUserToGroup(GroupData.GroupId, UsersIds, "add_user").ConfigureAwait(false);
                    if (apiStatus == 200)
                    {

                    }
                    else Methods.DisplayReportResult(this, respond);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 108)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        OpenDialogGallery();
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long)
                            .Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        private void LoadContacts()
        {
            try
            {
                if (GroupData != null)
                {
                    GroupPathImage = GroupData.Avatar;
                    GlideImageLoader.LoadImage(this, GroupData.Avatar, ImageGroup, ImageStyle.CenterCrop,
                        ImagePlaceholders.Drawable);

                    TxtGroupName.Text = Methods.FunString.DecodeString(GroupData.GroupName);

                    if (!(GroupData?.Parts?.Count > 0)) return;
                    foreach (var dataPart in (GroupData?.Parts).Where(dataPart => dataPart != null))
                    {
                        MAdapter.UserList.Insert(1, dataPart);
                    }

                    MAdapter.NotifyDataSetChanged();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void OpenDialogGallery()
        {
            try
            {
                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    Methods.Path.Chack_MyFolder();

                    //Open Image 
                    var myUri = Uri.FromFile(new File(Methods.Path.FolderDcimImage,
                        Methods.GetTimestamp(DateTime.Now) + ".jpeg"));
                    CropImage.Builder()
                        .SetInitialCropWindowPaddingRatio(0)
                        .SetAutoZoomEnabled(true)
                        .SetMaxZoom(4)
                        .SetGuidelines(CropImageView.Guidelines.On)
                        .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                        .SetOutputUri(myUri).Start(this);
                }
                else
                {
                    if (!CropImage.IsExplicitCameraPermissionRequired(this) &&
                        CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted)
                    {
                        Methods.Path.Chack_MyFolder();

                        //Open Image 
                        var myUri = Uri.FromFile(new File(Methods.Path.FolderDcimImage,
                            Methods.GetTimestamp(DateTime.Now) + ".jpeg"));
                        CropImage.Builder()
                            .SetInitialCropWindowPaddingRatio(0)
                            .SetAutoZoomEnabled(true)
                            .SetMaxZoom(4)
                            .SetGuidelines(CropImageView.Guidelines.On)
                            .SetCropMenuCropButtonTitle(GetText(Resource.String.Lbl_Done))
                            .SetOutputUri(myUri).Start(this);
                    }
                    else
                    {
                        new PermissionsController(this).RequestPermission(108);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region appBarLayout

        public void OnOffsetChanged(AppBarLayout appBarLayout, int verticalOffset)
        {
            try
            {
                int minHeight = ViewCompat.GetMinimumHeight(CollapsingToolbar) * 2;
                float scale = (float)(minHeight + verticalOffset) / minHeight;

                BtnAdd.ScaleX = scale >= 0 ? scale : 0;
                BtnAdd.ScaleY = scale >= 0 ? scale : 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region MaterialDialog

        public async void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                    if (!Methods.CheckConnectivity())
                    {
                        Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection),
                            ToastLength.Short).Show();
                    }
                    else
                    {
                        if (Type == "Exit")
                        {
                            //Show a progress
                            AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                            var (apiStatus, respond) = await RequestsAsync.GroupChat.ExitGroupChat(GroupId);
                            if (apiStatus == 200)
                            {
                                if (respond is AddOrRemoveUserToGroupObject result)
                                {
                                    Console.WriteLine(result.MessageData);

                                    Toast.MakeText(this, GetString(Resource.String.Lbl_GroupSuccessfullyLeaved), ToastLength.Short).Show();

                                    //remove new item to my Group list  
                                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                                    {
                                        var adapter = GlobalContext?.LastChatTab.MAdapter;
                                        var data = adapter?.ChatList?.FirstOrDefault(a => a.GroupId == GroupId);
                                        if (data != null)
                                        {
                                            adapter.ChatList.Remove(data);
                                            adapter.NotifyItemRemoved(adapter.ChatList.IndexOf(data));
                                        }
                                    }
                                    else
                                    {
                                        var adapter = GlobalContext?.LastGroupChatsTab.MAdapter;
                                        var data = adapter?.LastGroupList?.FirstOrDefault(a => a.GroupId == GroupId);
                                        if (data != null)
                                        {
                                            adapter.LastGroupList.Remove(data);
                                            adapter.NotifyItemRemoved(adapter.LastGroupList.IndexOf(data));
                                        }
                                    }

                                    AndHUD.Shared.ShowSuccess(this);
                                    Finish();
                                }
                            }
                            else Methods.DisplayReportResult(this, respond);

                            AndHUD.Shared.Dismiss();
                        }
                        else if (Type == "Delete")
                        {
                            //Show a progress
                            AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                            var (apiStatus, respond) = await RequestsAsync.GroupChat.DeleteGroupChat(GroupId);
                            if (apiStatus == 200)
                            {
                                if (respond is AddOrRemoveUserToGroupObject result)
                                {
                                    Console.WriteLine(result.MessageData);
                                    Toast.MakeText(this, GetString(Resource.String.Lbl_GroupSuccessfullyDeleted),
                                        ToastLength.Short).Show();

                                    //remove item to my Group list  
                                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                                    {
                                        var adapter = GlobalContext?.LastChatTab.MAdapter;
                                        var data = adapter?.ChatList?.FirstOrDefault(a => a.GroupId == GroupId);
                                        if (data != null)
                                        {
                                            adapter.ChatList.Remove(data);
                                            adapter.NotifyItemRemoved(adapter.ChatList.IndexOf(data));
                                        }
                                    }
                                    else
                                    {
                                        var adapter = GlobalContext?.LastGroupChatsTab.MAdapter;
                                        var data = adapter?.LastGroupList?.FirstOrDefault(a => a.GroupId == GroupId);
                                        if (data != null)
                                        {
                                            adapter.LastGroupList.Remove(data);
                                            adapter.NotifyItemRemoved(adapter.LastGroupList.IndexOf(data));
                                        }
                                    }

                                    AndHUD.Shared.ShowSuccess(this);
                                    Finish();
                                }
                            }
                            else Methods.DisplayReportResult(this, respond);

                            AndHUD.Shared.Dismiss();
                        }
                        else if (Type == "RemoveUser")
                        {
                            var itemUser = MAdapter.GetItem(Position);
                            if (itemUser != null)
                            {
                                MAdapter.UserList.Remove(itemUser);
                                MAdapter.NotifyItemRemoved(Position);

                                var (apiStatus, respond) = await RequestsAsync.GroupChat
                                    .AddOrRemoveUserToGroup(GroupId, itemUser.UserId, "remove_user")
                                    .ConfigureAwait(false);
                                if (apiStatus == 200)
                                {

                                }
                                else Methods.DisplayReportResult(this, respond);
                            }
                        }
                    }
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