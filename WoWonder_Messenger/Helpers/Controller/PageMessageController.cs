using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WoWonder.Activities.PageChat;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Activities.Tab;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Message;
using WoWonderClient.Classes.PageChat;
using WoWonderClient.Requests;
using MessageData = WoWonderClient.Classes.Message.MessageData;
namespace WoWonder.Helpers.Controller
{
    public static class PageMessageController
    {
        //############# DONT'T MODIFY HERE ############# 
        private static ChatObject PageData;
        private static PageClass DataProfilePage;
        private static PageChatWindowActivity MainWindowActivity;

        private static TabbedMainActivity GlobalContext;

        //========================= Functions ========================= 
        public static async Task SendMessageTask(PageChatWindowActivity windowActivity, string pageId, string id, string messageId, string text = "", string contact = "", string pathFile = "", string imageUrl = "", string stickerId = "", string gifUrl = "")
        {
            try
            {
                MainWindowActivity = windowActivity;
                if (windowActivity.PageData != null)
                    PageData = windowActivity.PageData;
                else if (windowActivity.DataProfilePage != null)
                    DataProfilePage = windowActivity.DataProfilePage;

                GlobalContext = TabbedMainActivity.GetInstance();

                StartApiService(pageId, id, messageId, text, contact, pathFile, imageUrl, stickerId, gifUrl);
                await Task.Delay(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void StartApiService(string pageId, string id, string messageId, string text = "", string contact = "", string pathFile = "", string imageUrl = "", string stickerId = "", string gifUrl = "")
        {
            if (!Methods.CheckConnectivity())
                Toast.MakeText(MainWindowActivity, MainWindowActivity.GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
            else
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => SendMessage(pageId, id, messageId, text, contact, pathFile, imageUrl, stickerId, gifUrl) });
        }

        private static async Task SendMessage(string pageId, string id, string messageId, string text = "", string contact = "", string pathFile = "", string imageUrl = "", string stickerId = "", string gifUrl = "")
        {
            var (apiStatus, respond) = await RequestsAsync.PageChat.Send_MessageToPageChat(pageId, id, messageId, text, contact, pathFile, imageUrl, stickerId, gifUrl);
            if (apiStatus == 200)
            {
                if (respond is PageSendMessageObject result)
                {
                    UpdateLastIdMessage(result.Data);
                }
            }
            else Methods.DisplayReportResult(MainWindowActivity, respond);
        }

        private static void UpdateLastIdMessage(List<MessageData> chatMessages)
        {
            try
            {
                foreach (var messageInfo in chatMessages)
                {
                    var typeModel = MainWindowActivity.MAdapter.GetTypeModel(messageInfo);
                    if (typeModel == MessageModelType.None)
                        continue;

                    var message = WoWonderTools.MessageFilter(messageInfo.PageId, messageInfo, typeModel);


                    message.ModelType = typeModel;

                    var checker = MainWindowActivity?.MAdapter.DifferList?.FirstOrDefault(a => a.MesData.Id == message.MessageHashId);
                    if (checker != null)
                    {
                        //checker.TypeView = typeModel;
                        checker.MesData = message;

                        if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                        {
                            var updaterUser = GlobalContext?.LastChatTab?.MAdapter?.ChatList.FirstOrDefault(a => a.UserId == message.ToId);
                            if (updaterUser != null)
                            {
                                var index = GlobalContext.LastChatTab.MAdapter.ChatList.IndexOf(GlobalContext.LastChatTab.MAdapter.ChatList.FirstOrDefault(x => x.PageId == message.PageId));
                                if (index > -1)
                                {
                                    if (typeModel == MessageModelType.RightGif)
                                        updaterUser.LastMessage.LastMessageClass.Text = GlobalContext?.GetText(Resource.String.Lbl_SendGifFile);
                                    else if (typeModel == MessageModelType.RightText)
                                        updaterUser.LastMessage.LastMessageClass.Text = !string.IsNullOrEmpty(message.Text) ? Methods.FunString.DecodeString(message.Text) : GlobalContext?.GetText(Resource.String.Lbl_SendMessage);
                                    else if (typeModel == MessageModelType.RightSticker)
                                        updaterUser.LastMessage.LastMessageClass.Text = GlobalContext?.GetText(Resource.String.Lbl_SendStickerFile);
                                    else if (typeModel == MessageModelType.RightContact)
                                        updaterUser.LastMessage.LastMessageClass.Text = GlobalContext?.GetText(Resource.String.Lbl_SendContactnumber);
                                    else if (typeModel == MessageModelType.RightFile)
                                        updaterUser.LastMessage.LastMessageClass.Text = GlobalContext?.GetText(Resource.String.Lbl_SendFile);
                                    else if (typeModel == MessageModelType.RightVideo)
                                        updaterUser.LastMessage.LastMessageClass.Text = GlobalContext?.GetText(Resource.String.Lbl_SendVideoFile);
                                    else if (typeModel == MessageModelType.RightImage)
                                        updaterUser.LastMessage.LastMessageClass.Text = GlobalContext?.GetText(Resource.String.Lbl_SendImageFile);
                                    else if (typeModel == MessageModelType.RightAudio)
                                        updaterUser.LastMessage.LastMessageClass.Text = GlobalContext?.GetText(Resource.String.Lbl_SendAudioFile);

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
                                }
                            }
                            else
                            {
                                GlobalContext?.RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        if (PageData != null)
                                        {
                                            GlobalContext?.LastChatTab.MAdapter.ChatList.Insert(0, PageData);
                                            GlobalContext?.LastChatTab.MAdapter.NotifyItemInserted(0);
                                            GlobalContext?.LastChatTab.MRecycler.ScrollToPosition(GlobalContext.LastChatTab.MAdapter.ChatList.IndexOf(PageData));
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                });
                            }
                        }
                        else
                        {
                            var updaterUser = GlobalContext?.LastPageChatsTab?.MAdapter?.LastPageList.FirstOrDefault(a => a.UserId == message.ToId);
                            if (updaterUser != null)
                            {
                                var index = GlobalContext.LastPageChatsTab.MAdapter.LastPageList.IndexOf(GlobalContext.LastPageChatsTab.MAdapter.LastPageList.FirstOrDefault(x => x.PageId == message.PageId));
                                if (index > -1)
                                {
                                    if (typeModel == MessageModelType.RightGif)
                                        updaterUser.LastMessage.Text = GlobalContext?.GetText(Resource.String.Lbl_SendGifFile);
                                    else if (typeModel == MessageModelType.RightText)
                                        updaterUser.LastMessage.Text = !string.IsNullOrEmpty(message.Text) ? Methods.FunString.DecodeString(message.Text) : GlobalContext?.GetText(Resource.String.Lbl_SendMessage);
                                    else if (typeModel == MessageModelType.RightSticker)
                                        updaterUser.LastMessage.Text = GlobalContext?.GetText(Resource.String.Lbl_SendStickerFile);
                                    else if (typeModel == MessageModelType.RightContact)
                                        updaterUser.LastMessage.Text = GlobalContext?.GetText(Resource.String.Lbl_SendContactnumber);
                                    else if (typeModel == MessageModelType.RightFile)
                                        updaterUser.LastMessage.Text = GlobalContext?.GetText(Resource.String.Lbl_SendFile);
                                    else if (typeModel == MessageModelType.RightVideo)
                                        updaterUser.LastMessage.Text = GlobalContext?.GetText(Resource.String.Lbl_SendVideoFile);
                                    else if (typeModel == MessageModelType.RightImage)
                                        updaterUser.LastMessage.Text = GlobalContext?.GetText(Resource.String.Lbl_SendImageFile);
                                    else if (typeModel == MessageModelType.RightAudio)
                                        updaterUser.LastMessage.Text = GlobalContext?.GetText(Resource.String.Lbl_SendAudioFile);

                                    GlobalContext.RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            GlobalContext?.LastPageChatsTab?.MAdapter?.LastPageList.Move(index, 0);
                                            GlobalContext?.LastPageChatsTab?.MAdapter?.NotifyItemMoved(index, 0);
                                            GlobalContext?.LastPageChatsTab?.MAdapter?.NotifyItemChanged(index, "WithoutBlob");
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e);
                                        }
                                    });
                                }
                            }
                            else
                            {
                                GlobalContext?.RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        if (DataProfilePage != null)
                                        {
                                            GlobalContext?.LastPageChatsTab?.MAdapter.LastPageList.Insert(0, DataProfilePage);
                                            GlobalContext?.LastPageChatsTab?.MAdapter.NotifyItemInserted(0);
                                            GlobalContext?.LastPageChatsTab?.MRecycler.ScrollToPosition(GlobalContext.LastPageChatsTab.MAdapter.LastPageList.IndexOf(DataProfilePage));
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                });
                            }
                        }

                        GlobalContext?.RunOnUiThread(() =>
                        {
                            try
                            {
                                //Update data RecyclerView Messages.
                                if (message.ModelType != MessageModelType.RightSticker || message.ModelType != MessageModelType.RightImage || message.ModelType != MessageModelType.RightVideo)
                                    MainWindowActivity.Update_One_Messeges(checker.MesData);

                                if (SettingsPrefFragment.SSoundControl)
                                    Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Popup_SendMesseges.mp3");
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        });
                    }

                    PageData = null;
                    DataProfilePage = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}