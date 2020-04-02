using Android.Content;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using System;

namespace WoWonder.Helpers.Utils
{
    public class SwipeViewPager : ViewPager
    {
        private bool SwipeAble;
        protected SwipeViewPager(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public SwipeViewPager(Context context) : base(context)
        {
            SwipeAble = true;
        }

        public SwipeViewPager(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            SwipeAble = true;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (SwipeAble)
            {
                return base.OnTouchEvent(e);
            }
            return false;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (SwipeAble)
            {
                return base.OnInterceptTouchEvent(ev);
            }
            return false;
        }

        public void SetSwipeAble(bool swipeAble)
        {
            SwipeAble = swipeAble;
        }

    }
}