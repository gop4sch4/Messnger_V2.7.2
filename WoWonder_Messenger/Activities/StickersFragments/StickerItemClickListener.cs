using Android.App;
using Android.Graphics;
using Android.Support.V4.View.Animation;
using Android.Widget;
using Java.Lang;
using System;
using System.Linq;
using WoWonder.Activities.ChatWindow;
using WoWonder.Activities.ChatWindow.Adapters;
using WoWonder.Activities.GroupChat;
using WoWonder.Activities.PageChat;
using WoWonder.Helpers.Controller;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonderClient.Classes.Message;
using Exception = System.Exception;

namespace WoWonder.Activities.StickersFragments
{
    public class StickerItemClickListener
    {
        private readonly StickerRecylerAdapter.StickerAdapter StickerAdapter;
        private readonly string Type;
        private readonly ChatWindowActivity ChatWindow;
        private readonly GroupChatWindowActivity GroupActivityView;
        private readonly PageChatWindowActivity PageActivityView;
        private readonly string TimeNow = DateTime.Now.ToString("hh:mm");

        public StickerItemClickListener(Activity activity, string type, StickerRecylerAdapter.StickerAdapter stickerAdapter)
        {
            try
            {
                Type = type;
                StickerAdapter = stickerAdapter;

                StickerAdapter.OnItemClick += StickerAdapterOnOnItemClick;

                switch (Type)
                {
                    // Create your fragment here
                    case "ChatWindowActivity":
                        ChatWindow = (ChatWindowActivity)activity;
                        break;
                    case "PageChatWindowActivity":
                        PageActivityView = (PageChatWindowActivity)activity;
                        break;
                    case "GroupChatWindowActivity":
                        GroupActivityView = (GroupChatWindowActivity)activity;
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void StickerAdapterOnOnItemClick(object sender, StickerRecylerAdapter.AdapterClickEvents adapterClickEvents)
        {
            try
            {
                var stickerUrl = StickerAdapter.GetItem(adapterClickEvents.Position);
                var unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

                if (Type == "ChatWindowActivity")
                {
                    MessageDataExtra m1 = new MessageDataExtra
                    {
                        Id = unixTimestamp.ToString(),
                        FromId = UserDetails.UserId,
                        ToId = ChatWindow.Userid,
                        Media = stickerUrl,
                        TimeText = TimeNow,
                        Position = "right",
                        ModelType = MessageModelType.RightSticker
                    };

                    ChatWindow.MAdapter.DifferList.Add(new AdapterModelsClassUser()
                    {
                        TypeView = MessageModelType.RightSticker,
                        Id = Long.ParseLong(m1.Id),
                        MesData = m1
                    });

                    var indexMes = ChatWindow.MAdapter.DifferList.IndexOf(ChatWindow.MAdapter.DifferList.FirstOrDefault(a => a.MesData == m1));
                    if (indexMes > -1)
                    {
                        ChatWindow.MAdapter.NotifyItemInserted(indexMes);
                        ChatWindow.MRecycler.ScrollToPosition(ChatWindow.MAdapter.ItemCount - 1);
                    }

                    if (Methods.CheckConnectivity())
                    {
                        //Sticker Send Function
                        MessageController.SendMessageTask(ChatWindow, ChatWindow.Userid, unixTimestamp.ToString(), "", "", "", stickerUrl, "sticker" + adapterClickEvents.Position).ConfigureAwait(false);
                    }
                    else
                    {
                        Toast.MakeText(ChatWindow, ChatWindow.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    }

                    try
                    {
                        var interplator = new FastOutSlowInInterpolator();
                        ChatWindow.ChatStickerButton.Tag = "Closed";

                        ChatWindow.ResetButtonTags();
                        ChatWindow.ChatStickerButton.Drawable.SetTint(Color.ParseColor("#888888"));
                        ChatWindow.TopFragmentHolder.Animate().SetInterpolator(interplator).TranslationY(1200).SetDuration(300);
                        ChatWindow.SupportFragmentManager.BeginTransaction().Remove(ChatWindow.ChatStickersTabBoxFragment).Commit();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
                else if (Type == "GroupChatWindowActivity")
                {
                    MessageDataExtra m1 = new MessageDataExtra
                    {
                        Id = unixTimestamp.ToString(),
                        FromId = UserDetails.UserId,
                        GroupId = GroupActivityView.GroupId,
                        Media = stickerUrl,
                        TimeText = TimeNow,
                        Position = "right",
                        ModelType = MessageModelType.RightSticker
                    };

                    GroupActivityView.MAdapter.DifferList.Add(new AdapterModelsClassGroup()
                    {
                        TypeView = MessageModelType.RightSticker,
                        Id = Long.ParseLong(m1.Id),
                        MesData = m1
                    });

                    var indexMes = GroupActivityView.MAdapter.DifferList.IndexOf(GroupActivityView.MAdapter.DifferList.FirstOrDefault(a => a.MesData == m1));
                    if (indexMes > -1)
                    {
                        GroupActivityView.MAdapter.NotifyItemInserted(indexMes);
                        GroupActivityView.MRecycler.ScrollToPosition(GroupActivityView.MAdapter.ItemCount - 1);
                    }

                    if (Methods.CheckConnectivity())
                    {
                        //Sticker Send Function
                        GroupMessageController.SendMessageTask(GroupActivityView, GroupActivityView.GroupId, unixTimestamp.ToString(), "", "", "", stickerUrl, "sticker" + adapterClickEvents.Position).ConfigureAwait(false);
                    }
                    else
                    {
                        Toast.MakeText(GroupActivityView, GroupActivityView.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    }

                    try
                    {
                        var interplator = new FastOutSlowInInterpolator();
                        GroupActivityView.ChatStickerButton.Tag = "Closed";

                        GroupActivityView.ResetButtonTags();
                        GroupActivityView.ChatStickerButton.Drawable.SetTint(Color.ParseColor("#888888"));
                        GroupActivityView.TopFragmentHolder.Animate().SetInterpolator(interplator).TranslationY(1200).SetDuration(300);
                        GroupActivityView.SupportFragmentManager.BeginTransaction().Remove(GroupActivityView.ChatStickersTabBoxFragment).Commit();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
                else if (Type == "PageChatWindowActivity")
                {
                    MessageDataExtra m1 = new MessageDataExtra
                    {
                        Id = unixTimestamp.ToString(),
                        FromId = UserDetails.UserId,
                        PageId = PageActivityView.PageId,
                        Media = stickerUrl,
                        TimeText = TimeNow,
                        Position = "right",
                        ModelType = MessageModelType.RightSticker
                    };

                    PageActivityView.MAdapter.DifferList.Add(new AdapterModelsClassPage()
                    {
                        TypeView = MessageModelType.RightSticker,
                        Id = Long.ParseLong(m1.Id),
                        MesData = m1
                    });

                    var indexMes = PageActivityView.MAdapter.DifferList.IndexOf(PageActivityView.MAdapter.DifferList.FirstOrDefault(a => a.MesData == m1));
                    if (indexMes > -1)
                    {
                        PageActivityView.MAdapter.NotifyItemInserted(indexMes);
                        PageActivityView.MRecycler.ScrollToPosition(PageActivityView.MAdapter.ItemCount - 1);
                    }

                    if (Methods.CheckConnectivity())
                    {
                        //Sticker Send Function
                        PageMessageController.SendMessageTask(PageActivityView, PageActivityView.PageId, PageActivityView.UserId, unixTimestamp.ToString(), "", "", "", stickerUrl, "sticker" + adapterClickEvents.Position).ConfigureAwait(false);
                    }
                    else
                    {
                        Toast.MakeText(PageActivityView, PageActivityView.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                    }

                    try
                    {
                        var interplator = new FastOutSlowInInterpolator();
                        PageActivityView.ChatStickerButton.Tag = "Closed";

                        PageActivityView.ResetButtonTags();
                        PageActivityView.ChatStickerButton.Drawable.SetTint(Color.ParseColor("#888888"));
                        PageActivityView.TopFragmentHolder.Animate().SetInterpolator(interplator).TranslationY(1200).SetDuration(300);
                        PageActivityView.SupportFragmentManager.BeginTransaction().Remove(PageActivityView.ChatStickersTabBoxFragment).Commit();
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}