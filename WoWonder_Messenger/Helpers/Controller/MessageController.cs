using Android.Widget;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WoWonder.Activities.ChatWindow;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Activities.Tab;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonder.SQLite;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Message;
using WoWonderClient.Requests;
using MessageData = WoWonder.Helpers.Model.MessageDataExtra;

namespace WoWonder.Helpers.Controller
{
    public static class MessageController
    {
        //############# DON'T  MODIFY HERE #############
        private static ChatObject Datauser;
        private static UserDataObject UserData;
        private static GetUsersListObject.User DataUserChat;

        private static ChatWindowActivity WindowActivity;

        private static TabbedMainActivity GlobalContext;
        //========================= Functions =========================
        public static async Task SendMessageTask(ChatWindowActivity windowActivity, string userid, string messageId, string text = "", string contact = "", string pathFile = "", string imageUrl = "", string stickerId = "", string gifUrl = "")
        {
            try
            {
                WindowActivity = windowActivity;
                if (windowActivity.DataUser != null)
                    Datauser = windowActivity.DataUser;
                else if (windowActivity.UserData != null)
                    UserData = windowActivity.UserData;
                else if (windowActivity.DataUserChat != null)
                    DataUserChat = windowActivity.DataUserChat;

                GlobalContext = TabbedMainActivity.GetInstance();

                StartApiService(userid, messageId, text, contact, pathFile, imageUrl, stickerId, gifUrl);

                await Task.Delay(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void StartApiService(string userid, string messageId, string text = "", string contact = "", string pathFile = "", string imageUrl = "", string stickerId = "", string gifUrl = "")
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(WindowActivity, WindowActivity?.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => SendMessage(userid, messageId, text, contact, pathFile, imageUrl, stickerId, gifUrl) });
        }

        private static async Task SendMessage(string userid, string messageId, string text = "", string contact = "", string pathFile = "", string imageUrl = "", string stickerId = "", string gifUrl = "")
        {
            var (apiStatus, respond) = await RequestsAsync.Message.Send_Message(userid, messageId, text, contact, pathFile, imageUrl, stickerId, gifUrl);
            if (apiStatus == 200)
            {
                if (respond is SendMessageObject result)
                {
                    UpdateLastIdMessage(result);
                }
            }
            else Methods.DisplayReportResult(WindowActivity, respond);
        }

        private static void UpdateLastIdMessage(SendMessageObject chatMessages)
        {
            try
            {
                foreach (var messageInfo in chatMessages.MessageData)
                {
                    MessageData m = new MessageData
                    {
                        Id = messageInfo.Id,
                        FromId = messageInfo.FromId,
                        GroupId = messageInfo.GroupId,
                        ToId = messageInfo.ToId,
                        Text = messageInfo.Text,
                        Media = messageInfo.Media,
                        MediaFileName = messageInfo.MediaFileName,
                        MediaFileNames = messageInfo.MediaFileNames,
                        Time = messageInfo.Time,
                        Seen = messageInfo.Seen,
                        DeletedOne = messageInfo.DeletedOne,
                        DeletedTwo = messageInfo.DeletedTwo,
                        SentPush = messageInfo.SentPush,
                        NotificationId = messageInfo.NotificationId,
                        TypeTwo = messageInfo.TypeTwo,
                        Stickers = messageInfo.Stickers,
                        TimeText = messageInfo.TimeText,
                        Position = messageInfo.Position,
                        ModelType = messageInfo.ModelType,
                        FileSize = messageInfo.FileSize,
                        MediaDuration = messageInfo.MediaDuration,
                        MediaIsPlaying = messageInfo.MediaIsPlaying,
                        ContactNumber = messageInfo.ContactNumber,
                        ContactName = messageInfo.ContactName,
                        ProductId = messageInfo.ProductId,
                        MessageUser = messageInfo.MessageUser,
                        Product = messageInfo.Product,
                        MessageHashId = messageInfo.MessageHashId,
                    };

                    var typeModel = WindowActivity?.MAdapter?.GetTypeModel(m) ?? MessageModelType.None;
                    if (typeModel == MessageModelType.None)
                        continue;

                    var message = WoWonderTools.MessageFilter(messageInfo.ToId, m, typeModel);
                    message.ModelType = typeModel;

                    AdapterModelsClassUser checker = WindowActivity?.MAdapter?.DifferList?.FirstOrDefault(a => a.MesData.Id == messageInfo.MessageHashId);
                    if (checker != null)
                    {
                        //checker.TypeView = typeModel;
                        checker.MesData = message;

                        if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                        {
                            var updaterUser = GlobalContext?.LastChatTab?.MAdapter?.ChatList?.FirstOrDefault(a => a.UserId == message.ToId);
                            if (updaterUser != null)
                            {
                                var index = GlobalContext.LastChatTab.MAdapter.ChatList.IndexOf(GlobalContext.LastChatTab.MAdapter.ChatList.FirstOrDefault(x => x.UserId == message.ToId));
                                if (index > -1)
                                {
                                    if (typeModel == MessageModelType.RightGif)
                                        updaterUser.LastMessage.LastMessageClass.Text = WindowActivity?.GetText(Resource.String.Lbl_SendGifFile);
                                    else if (typeModel == MessageModelType.RightText)
                                        updaterUser.LastMessage.LastMessageClass.Text = !string.IsNullOrEmpty(message.Text) ? Methods.FunString.DecodeString(message.Text) : WindowActivity?.GetText(Resource.String.Lbl_SendMessage);
                                    else if (typeModel == MessageModelType.RightSticker)
                                        updaterUser.LastMessage.LastMessageClass.Text = WindowActivity?.GetText(Resource.String.Lbl_SendStickerFile);
                                    else if (typeModel == MessageModelType.RightContact)
                                        updaterUser.LastMessage.LastMessageClass.Text = WindowActivity?.GetText(Resource.String.Lbl_SendContactnumber);
                                    else if (typeModel == MessageModelType.RightFile)
                                        updaterUser.LastMessage.LastMessageClass.Text = WindowActivity?.GetText(Resource.String.Lbl_SendFile);
                                    else if (typeModel == MessageModelType.RightVideo)
                                        updaterUser.LastMessage.LastMessageClass.Text = WindowActivity?.GetText(Resource.String.Lbl_SendVideoFile);
                                    else if (typeModel == MessageModelType.RightImage)
                                        updaterUser.LastMessage.LastMessageClass.Text = WindowActivity?.GetText(Resource.String.Lbl_SendImageFile);
                                    else if (typeModel == MessageModelType.RightAudio)
                                        updaterUser.LastMessage.LastMessageClass.Text = WindowActivity?.GetText(Resource.String.Lbl_SendAudioFile);

                                    GlobalContext.RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            GlobalContext?.LastChatTab?.MAdapter?.ChatList.Move(index, 0);
                                            GlobalContext?.LastChatTab?.MAdapter?.NotifyItemMoved(index, 0);
                                            GlobalContext?.LastChatTab?.MAdapter?.NotifyItemChanged(index, "WithoutBlob");
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }
                                    });

                                    SqLiteDatabase dbSqLite = new SqLiteDatabase();
                                    //Update All data users to database
                                    dbSqLite.Insert_Or_Update_LastUsersChat(GlobalContext, new ObservableCollection<ChatObject>() { updaterUser });
                                    dbSqLite.Dispose();
                                }
                            }
                            else
                            {
                                //insert new user  
                                var data = ConvertData(checker.MesData);
                                if (data != null)
                                {
                                    GlobalContext?.RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            GlobalContext?.LastChatTab.MAdapter.ChatList.Insert(0, data);
                                            GlobalContext?.LastChatTab.MAdapter.NotifyItemInserted(0);
                                            GlobalContext?.LastChatTab.MRecycler.ScrollToPosition(GlobalContext.LastChatTab.MAdapter.ChatList.IndexOf(data));
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }
                                    });

                                    //Update All data users to database
                                    //dbDatabase.Insert_Or_Update_LastUsersChat(new ObservableCollection<GetUsersListObject.User>
                                    //{
                                    //    data
                                    //});
                                }
                            }
                        }
                        else
                        {
                            var updaterUser = GlobalContext?.LastMessagesTab?.MAdapter?.MLastMessagesUser?.FirstOrDefault(a => a.UserId == message.ToId);
                            if (updaterUser != null)
                            {
                                var index = GlobalContext.LastMessagesTab.MAdapter.MLastMessagesUser.IndexOf(GlobalContext.LastMessagesTab.MAdapter.MLastMessagesUser.FirstOrDefault(x => x.UserId == message.ToId));
                                if (index > -1)
                                {
                                    if (typeModel == MessageModelType.RightGif)
                                        updaterUser.LastMessage.Text = WindowActivity?.GetText(Resource.String.Lbl_SendGifFile);
                                    else if (typeModel == MessageModelType.RightText)
                                        updaterUser.LastMessage.Text = !string.IsNullOrEmpty(message.Text) ? Methods.FunString.DecodeString(message.Text) : WindowActivity?.GetText(Resource.String.Lbl_SendMessage);
                                    else if (typeModel == MessageModelType.RightSticker)
                                        updaterUser.LastMessage.Text = WindowActivity?.GetText(Resource.String.Lbl_SendStickerFile);
                                    else if (typeModel == MessageModelType.RightContact)
                                        updaterUser.LastMessage.Text = WindowActivity?.GetText(Resource.String.Lbl_SendContactnumber);
                                    else if (typeModel == MessageModelType.RightFile)
                                        updaterUser.LastMessage.Text = WindowActivity?.GetText(Resource.String.Lbl_SendFile);
                                    else if (typeModel == MessageModelType.RightVideo)
                                        updaterUser.LastMessage.Text = WindowActivity?.GetText(Resource.String.Lbl_SendVideoFile);
                                    else if (typeModel == MessageModelType.RightImage)
                                        updaterUser.LastMessage.Text = WindowActivity?.GetText(Resource.String.Lbl_SendImageFile);
                                    else if (typeModel == MessageModelType.RightAudio)
                                        updaterUser.LastMessage.Text = WindowActivity?.GetText(Resource.String.Lbl_SendAudioFile);

                                    GlobalContext.RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            GlobalContext?.LastMessagesTab?.MAdapter?.MLastMessagesUser.Move(index, 0);
                                            GlobalContext?.LastMessagesTab?.MAdapter?.NotifyItemMoved(index, 0);
                                            GlobalContext?.LastMessagesTab?.MAdapter?.NotifyItemChanged(index, "WithoutBlob");
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }
                                    });
                                    SqLiteDatabase dbSqLite = new SqLiteDatabase();
                                    //Update All data users to database
                                    dbSqLite.Insert_Or_Update_LastUsersChat(GlobalContext, new ObservableCollection<GetUsersListObject.User>() { updaterUser });
                                    dbSqLite.Dispose();
                                }
                            }
                            else
                            {
                                //insert new user  
                                var data = ConvertDataChat(checker.MesData);
                                if (data != null)
                                {
                                    GlobalContext?.RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            GlobalContext?.LastMessagesTab?.MAdapter.MLastMessagesUser.Insert(0, data);
                                            GlobalContext?.LastMessagesTab?.MAdapter.NotifyItemInserted(0);
                                            GlobalContext?.LastMessagesTab?.MRecycler.ScrollToPosition(GlobalContext.LastMessagesTab.MAdapter.MLastMessagesUser.IndexOf(data));
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }
                                    });

                                    //Update All data users to database
                                    //dbDatabase.Insert_Or_Update_LastUsersChat(new ObservableCollection<GetUsersListObject.User>
                                    //{
                                    //    data
                                    //});
                                }
                            }
                        }

                        //checker.Media = media;
                        //Update All data users to database
                        SqLiteDatabase dbDatabase = new SqLiteDatabase();
                        dbDatabase.Insert_Or_Update_To_one_MessagesTable(checker.MesData);
                        dbDatabase.Dispose();

                        GlobalContext?.RunOnUiThread(() =>
                        {
                            try
                            {
                                //Update data RecyclerView Messages.
                                if (message.ModelType != MessageModelType.RightSticker || message.ModelType != MessageModelType.RightImage || message.ModelType != MessageModelType.RightVideo)
                                    WindowActivity?.Update_One_Messages(checker.MesData);

                                if (SettingsPrefFragment.SSoundControl)
                                    Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Popup_SendMesseges.mp3");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        });
                    }
                }

                Datauser = null;
                DataUserChat = null;
                UserData = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static GetUsersListObject.User ConvertDataChat(MessageData ms)
        {
            try
            {
                if (DataUserChat != null)
                {
                    GetUsersListObject.User user = new GetUsersListObject.User
                    {
                        UserId = ms.ToId,
                        Username = DataUserChat.Username,
                        Avatar = DataUserChat.Avatar,
                        Cover = DataUserChat.Cover,
                        LastseenTimeText = DataUserChat.LastseenTimeText,
                        Lastseen = DataUserChat.Lastseen,
                        Url = DataUserChat.Url,
                        Name = DataUserChat.Name,
                        LastseenUnixTime = DataUserChat.LastseenUnixTime,
                        ChatColor = AppSettings.MainColor,
                        Verified = DataUserChat.Verified,
                        LastMessage = new GetUsersListObject.LastMessage()
                        {
                            Id = ms.Id,
                            FromId = ms.FromId,
                            GroupId = ms.GroupId,
                            ToId = ms.ToId,
                            Text = ms.Text,
                            Media = ms.Media,
                            MediaFileName = ms.MediaFileName,
                            MediaFileNames = ms.MediaFileNames,
                            Time = ms.Time,
                            Seen = ms.Seen,
                            DeletedOne = ms.DeletedOne,
                            DeletedTwo = ms.DeletedTwo,
                            SentPush = ms.SentPush,
                            NotificationId = ms.NotificationId,
                            TypeTwo = ms.TypeTwo,
                            Stickers = ms.Stickers
                        }
                    };
                    return user;
                }

                if (Datauser != null)
                {
                    GetUsersListObject.User user = new GetUsersListObject.User
                    {
                        UserId = ms.ToId,
                        Username = Datauser.Username,
                        Avatar = Datauser.Avatar,
                        Cover = Datauser.Cover,
                        Lastseen = Datauser.Lastseen,
                        Url = Datauser.Url,
                        Name = Datauser.Name,
                        LastseenUnixTime = Datauser.LastseenUnixTime,
                        Verified = Datauser.Verified,
                        LastMessage = new GetUsersListObject.LastMessage
                        {
                            Id = ms.Id,
                            FromId = ms.FromId,
                            GroupId = ms.GroupId,
                            ToId = ms.ToId,
                            Text = ms.Text,
                            Media = ms.Media,
                            MediaFileName = ms.MediaFileName,
                            MediaFileNames = ms.MediaFileNames,
                            Time = ms.Time,
                            Seen = ms.Seen,
                            DeletedOne = ms.DeletedOne,
                            DeletedTwo = ms.DeletedTwo,
                            SentPush = ms.SentPush,
                            NotificationId = ms.NotificationId,
                            TypeTwo = ms.TypeTwo,
                            Stickers = ms.Stickers
                        }
                    };
                    return user;
                }
                if (UserData != null)
                {
                    GetUsersListObject.User user = new GetUsersListObject.User
                    {
                        UserId = ms.ToId,
                        Username = UserData.Username,
                        Avatar = UserData.Avatar,
                        Cover = UserData.Cover,
                        LastseenTimeText = UserData.LastseenTimeText,
                        Lastseen = UserData.Lastseen,
                        Url = UserData.Url,
                        Name = UserData.Name,
                        LastseenUnixTime = UserData.LastseenUnixTime,
                        ChatColor = AppSettings.MainColor,
                        Verified = UserData.Verified,
                        LastMessage = new GetUsersListObject.LastMessage()
                        {
                            Id = ms.Id,
                            FromId = ms.FromId,
                            GroupId = ms.GroupId,
                            ToId = ms.ToId,
                            Text = ms.Text,
                            Media = ms.Media,
                            MediaFileName = ms.MediaFileName,
                            MediaFileNames = ms.MediaFileNames,
                            Time = ms.Time,
                            Seen = ms.Seen,
                            DeletedOne = ms.DeletedOne,
                            DeletedTwo = ms.DeletedTwo,
                            SentPush = ms.SentPush,
                            NotificationId = ms.NotificationId,
                            TypeTwo = ms.TypeTwo,
                            Stickers = ms.Stickers
                        }
                    };
                    return user;
                }

                return DataUserChat;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private static ChatObject ConvertData(MessageData ms)
        {
            try
            {
                if (Datauser != null)
                {
                    ChatObject user = new ChatObject
                    {
                        UserId = ms.ToId,
                        Username = Datauser.Username,
                        Avatar = Datauser.Avatar,
                        Cover = Datauser.Cover,
                        Lastseen = Datauser.Lastseen,
                        Url = Datauser.Url,
                        Name = Datauser.Name,
                        LastseenUnixTime = Datauser.LastseenUnixTime,
                        Verified = Datauser.Verified,
                        LastMessage = ms
                    };
                    user.LastMessage.LastMessageClass.ChatColor = ms.ChatColor ?? AppSettings.MainColor;

                    return user;
                }
                if (UserData != null)
                {
                    ChatObject user = new ChatObject
                    {
                        UserId = ms.ToId,
                        Username = UserData.Username,
                        Avatar = UserData.Avatar,
                        Cover = UserData.Cover,
                        Lastseen = UserData.Lastseen,
                        Url = UserData.Url,
                        Name = UserData.Name,
                        LastseenUnixTime = UserData.LastseenUnixTime,
                        Verified = UserData.Verified,
                        LastMessage = ms,
                    };
                    user.LastMessage.LastMessageClass.ChatColor = ms.ChatColor ?? AppSettings.MainColor;
                    return user;
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public static void UpdateRecyclerLastMessageView(GetUsersListObject.LastMessage result, GetUsersListObject.User user, int index, TabbedMainActivity context)
        {
            try
            {
                if (IsImageExtension(result.MediaFileName))
                {
                    result.Text = WindowActivity?.GetText(Resource.String.Lbl_SendImageFile);
                }
                else if (IsVideoExtension(result.MediaFileName))
                {
                    result.Text = WindowActivity?.GetText(Resource.String.Lbl_SendVideoFile);
                }
                else if (IsAudioExtension(result.MediaFileName))
                {
                    result.Text = WindowActivity?.GetText(Resource.String.Lbl_SendAudioFile);
                }
                else if (IsFileExtension(result.MediaFileName))
                {
                    result.Text = WindowActivity?.GetText(Resource.String.Lbl_SendFile);
                }
                else if (result.MediaFileName.Contains(".gif") || result.MediaFileName.Contains(".GIF"))
                {
                    result.Text = WindowActivity?.GetText(Resource.String.Lbl_SendGifFile);
                }
                else
                {
                    result.Text = Methods.FunString.DecodeString(result.Text);
                }

                context.RunOnUiThread(() =>
                {
                    try
                    {
                        switch (AppSettings.LastChatSystem)
                        {
                            case SystemApiGetLastChat.New:
                                context.LastChatTab?.MAdapter?.NotifyItemChanged(index);
                                break;
                            default:
                                context.LastMessagesTab?.MAdapter?.NotifyItemChanged(index);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });

                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                //Update All data users to database
                dbDatabase.Insert_Or_Update_LastUsersChat(GlobalContext, new ObservableCollection<GetUsersListObject.User>() { user });
                dbDatabase.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
        }

        private static readonly string[] ImageValidExtensions = { ".jpg", ".bmp", ".gif", ".png", ".jpeg", ".tif" };
        private static readonly string[] VideoValidExtensions = { ".mp4", ".avi", ".mov", ".flv", ".wmv", ".divx", ".mpeg", ".mpeg2" };
        private static readonly string[] AudioValidExtensions = { ".mp3", ".wav", ".aiff", ".pcm", ".wmv" };
        private static readonly string[] FilesValidExtensions = { ".zip", ".pdf", ".doc", ".xml", ".txt" };

        public static bool IsImageExtension(string text)
        {
            return ImageValidExtensions.Any(text.Contains);
        }

        public static bool IsVideoExtension(string text)
        {
            return VideoValidExtensions.Any(text.Contains);
        }
        public static bool IsAudioExtension(string text)
        {
            return AudioValidExtensions.Any(text.Contains);
        }

        public static bool IsFileExtension(string text)
        {
            return FilesValidExtensions.Any(text.Contains);
        }
    }
}