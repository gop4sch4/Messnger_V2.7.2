using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using System;
using WoWonder.Activities.ChatWindow.Adapters;
using WoWonder.Activities.StickersFragments;

namespace WoWonder.Activities.PageChat.Fragment
{
    public class PageChatStickersTabFragment : Android.Support.V4.App.Fragment
    {
        private TabLayout Tabs;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View MainTabPage = inflater.Inflate(Resource.Layout.Chat_StickersTab_Fragment, container, false);
                Tabs = MainTabPage.FindViewById<TabLayout>(Resource.Id.tabsSticker);
                ViewPager viewPager = MainTabPage.FindViewById<ViewPager>(Resource.Id.viewpagerSticker);
                //AppBarLayout appBarLayoutview = MainTabPage.FindViewById<AppBarLayout>(Resource.Id.appbarSticker);

                SetUpViewPager(viewPager);

                return MainTabPage;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private void SetUpViewPager(ViewPager viewPager)
        {
            try
            {
                StickersTabAdapter adapter = new StickersTabAdapter(ChildFragmentManager);
                if (AppSettings.ShowStickerStack0)
                    adapter.AddFragment(new StickerFragment1("PageChatWindowActivity"), "0");

                if (AppSettings.ShowStickerStack1)
                    adapter.AddFragment(new StickerFragment2("PageChatWindowActivity"), "1");

                if (AppSettings.ShowStickerStack2)
                    adapter.AddFragment(new StickerFragment3("PageChatWindowActivity"), "2");

                if (AppSettings.ShowStickerStack3)
                    adapter.AddFragment(new StickerFragment4("PageChatWindowActivity"), "3");

                if (AppSettings.ShowStickerStack4)
                    adapter.AddFragment(new StickerFragment5("PageChatWindowActivity"), "4");

                if (AppSettings.ShowStickerStack5)
                    adapter.AddFragment(new StickerFragment6("PageChatWindowActivity"), "5");

                if (AppSettings.ShowStickerStack6)
                    adapter.AddFragment(new StickerFragment7("PageChatWindowActivity"), "6");

                viewPager.Adapter = adapter;
                Tabs.SetupWithViewPager(viewPager);
                Tabs.SetBackgroundColor(!AppSettings.SetTabDarkTheme ? Color.ParseColor(AppSettings.StickersBarColor) : Color.ParseColor(AppSettings.StickersBarColorDark));

                if (Tabs.TabCount > 0)
                {
                    for (int i = 0; i <= Tabs.TabCount; i++)
                    {
                        var stickerReplacer = Tabs.GetTabAt(i);
                        if (stickerReplacer != null)
                        {
                            if (stickerReplacer.Text == "0")
                                stickerReplacer.SetIcon(Resource.Drawable.Sticker1).SetText("");

                            if (stickerReplacer.Text == "1")
                                stickerReplacer.SetIcon(Resource.Drawable.sticker2).SetText("");

                            if (stickerReplacer.Text == "2")
                                stickerReplacer.SetIcon(Resource.Drawable.Sticker3).SetText("");

                            if (stickerReplacer.Text == "3")
                                stickerReplacer.SetIcon(Resource.Drawable.Sticker4).SetText("");

                            if (stickerReplacer.Text == "4")
                                stickerReplacer.SetIcon(Resource.Drawable.Sticker5).SetText("");

                            if (stickerReplacer.Text == "5")
                                stickerReplacer.SetIcon(Resource.Drawable.Sticker6).SetText("");

                            if (stickerReplacer.Text == "6")
                                stickerReplacer.SetIcon(Resource.Drawable.Sticker7).SetText("");
                        }
                    }
                }
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

    }
}