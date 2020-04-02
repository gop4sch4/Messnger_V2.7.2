using Android.Support.V4.App;
using Java.Lang;
using System.Collections.Generic;

namespace WoWonder.Activities.ChatWindow.Adapters
{
    public class StickersTabAdapter : FragmentPagerAdapter
    {
        public List<Android.Support.V4.App.Fragment> Fragments { get; set; }
        public List<string> FragmentNames { get; set; }

        public StickersTabAdapter(FragmentManager sfm) : base(sfm)
        {
            Fragments = new List<Android.Support.V4.App.Fragment>();
            FragmentNames = new List<string>();
        }

        public void AddFragment(Android.Support.V4.App.Fragment fragment, string name)
        {
            Fragments.Add(fragment);
            FragmentNames.Add(name);
        }

        public override int Count
        {
            get { return Fragments.Count; }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return Fragments[position];
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new String(FragmentNames[position]);
        }
    }
}