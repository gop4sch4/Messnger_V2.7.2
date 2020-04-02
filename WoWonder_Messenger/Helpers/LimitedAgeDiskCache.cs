using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Util;
using UniversalImageLoader.Cache.Disc;
using UniversalImageLoader.Utils;
using File = Java.IO.File;

namespace WoWonder.Helpers
{
  public  class LimitedAgeDiskCache : Java.Lang.Object, IDiskCache
    {
        private readonly long maxFileAge;

        private readonly Dictionary<File, Long> loadingDates = (Dictionary<File, Long>) Collections.SynchronizedMap(new Dictionary<File, Long>());
        public void Clear()
        {
             
        }

        public void Close()
        {
            
        }

        public File Get(string imageUri)
        {
             
        }

        public bool Remove(string imageUri)
        {
            
        }

        public bool Save(string imageUri, Bitmap bitmap)
        {
             
        }

        public bool Save(string imageUri, Stream imageStream, IoUtils.ICopyListener listener)
        {
             
        }

        public File Directory { get; }
    }
}