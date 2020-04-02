﻿using Android.App;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Request;
using Java.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WoWonder.Helpers.CacheLoaders;
using WoWonder.Helpers.Fonts;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using Console = System.Console;
using Object = Java.Lang.Object;

namespace WoWonder.Activities.SharedFiles.Adapter
{
    public class HSharedFilesAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        public event EventHandler<HSharedFilesAdapterViewHolderClickEventArgs> ItemClick;
        public event EventHandler<HSharedFilesAdapterViewHolderClickEventArgs> ItemLongClick;

        private readonly Activity ActivityContext;
        public ObservableCollection<Classes.SharedFile> SharedFilesList = new ObservableCollection<Classes.SharedFile>();
        private readonly string UserId;
        private readonly string TypeStyle;
        public HSharedFilesAdapter(Activity context, string userId, string typeStyle)
        {
            try
            {
                ActivityContext = context;
                UserId = userId;
                TypeStyle = typeStyle;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_SharedFiles_View
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_HSharedFiles_View, parent, false);

                var vh = new HSharedFilesAdapterViewHolder(itemView, OnClick, OnLongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is HSharedFilesAdapterViewHolder holder)
                {
                    var item = SharedFilesList[position];
                    if (item == null) return;
                    switch (item.FileType)
                    {
                        case "Video":
                            {
                                var fileName = item.FilePath.Split('/').Last();
                                var fileNameWithoutExtension = fileName.Split('.').First();

                                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.PlayIcon, IonIconsFonts.Play);
                                holder.PlayIcon.Visibility = ViewStates.Visible;

                                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.TypeIcon, IonIconsFonts.Videocamera);
                                holder.TypeIcon.Visibility = ViewStates.Visible;

                                var videoPlaceHolderImage = Methods.MultiMedia.GetMediaFrom_Gallery(Methods.Path.FolderDcimVideo + "/" + UserId, fileNameWithoutExtension + ".png");
                                if (videoPlaceHolderImage == "File Dont Exists")
                                {
                                    var bitmapImage = Methods.MultiMedia.Retrieve_VideoFrame_AsBitmap(ActivityContext, item.FilePath);
                                    Methods.MultiMedia.Export_Bitmap_As_Image(bitmapImage, fileNameWithoutExtension, Methods.Path.FolderDcimVideo + "/" + UserId);

                                    var imageVideo = Methods.Path.FolderDcimVideo + "/" + UserId + "/" + fileNameWithoutExtension + ".png";

                                    File file2 = new File(imageVideo);
                                    var photoUri = FileProvider.GetUriForFile(ActivityContext, ActivityContext.PackageName + ".fileprovider", file2);
                                    Glide.With(ActivityContext).Load(photoUri).Apply(new RequestOptions()).Into(holder.Image);
                                }
                                else
                                {
                                    File file2 = new File(videoPlaceHolderImage);
                                    var photoUri = FileProvider.GetUriForFile(ActivityContext, ActivityContext.PackageName + ".fileprovider", file2);
                                    Glide.With(ActivityContext).Load(photoUri).Apply(new RequestOptions()).Into(holder.Image);
                                }

                                break;
                            }
                        case "Gif":
                            {
                                holder.TypeIcon.Text = ActivityContext.GetText(Resource.String.Lbl_Gif);
                                FontUtils.SetFont(holder.TypeIcon, Fonts.SfSemibold);

                                holder.PlayIcon.Visibility = ViewStates.Gone;
                                holder.TypeIcon.Visibility = ViewStates.Visible;

                                File file2 = new File(item.FilePath);
                                var photoUri = FileProvider.GetUriForFile(ActivityContext, ActivityContext.PackageName + ".fileprovider", file2);
                                Glide.With(ActivityContext).Load(photoUri).Apply(new RequestOptions()).Into(holder.Image);
                                break;
                            }
                        case "Sticker":
                            {
                                holder.PlayIcon.Visibility = ViewStates.Gone;
                                holder.TypeIcon.Visibility = ViewStates.Gone;

                                File file2 = new File(item.FilePath);
                                var photoUri = FileProvider.GetUriForFile(ActivityContext, ActivityContext.PackageName + ".fileprovider", file2);
                                Glide.With(ActivityContext).Load(photoUri).Apply(new RequestOptions()).Into(holder.Image);
                                break;
                            }
                        case "Image":
                            {
                                holder.PlayIcon.Visibility = ViewStates.Gone;
                                holder.TypeIcon.Visibility = ViewStates.Gone;

                                File file2 = new File(item.FilePath);
                                var photoUri = FileProvider.GetUriForFile(ActivityContext, ActivityContext.PackageName + ".fileprovider", file2);
                                Glide.With(ActivityContext).Load(photoUri).Apply(new RequestOptions()).Into(holder.Image);
                                break;
                            }
                        case "Sounds":
                            holder.PlayIcon.Visibility = ViewStates.Gone;
                            holder.TypeIcon.Visibility = ViewStates.Gone;

                            Glide.With(ActivityContext).Load(ActivityContext.GetDrawable(Resource.Drawable.Audio_File)).Apply(new RequestOptions()).Into(holder.Image);
                            break;
                        case "File":
                            holder.PlayIcon.Visibility = ViewStates.Gone;
                            holder.TypeIcon.Visibility = ViewStates.Gone;

                            Glide.With(ActivityContext).Load(ActivityContext.GetDrawable(Resource.Drawable.Image_File)).Apply(new RequestOptions()).Into(holder.Image);
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        public override int ItemCount => SharedFilesList?.Count ?? 0;


        public Classes.SharedFile GetItem(int position)
        {
            return SharedFilesList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        void OnClick(HSharedFilesAdapterViewHolderClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(HSharedFilesAdapterViewHolderClickEventArgs args) => ItemLongClick?.Invoke(this, args);


        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = SharedFilesList[p0];
                if (item == null)
                    return d;

                d.Add(item.FilePath);

                return d;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var d = new List<string>();
                return d;
            }
        }

        public RequestBuilder GetPreloadRequestBuilder(Object p0)
        {
            return GlideImageLoader.GetPreLoadRequestBuilder(ActivityContext, p0.ToString(), ImageStyle.CenterCrop);
        }
    }

    public class HSharedFilesAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic
        public View MainView { get; set; }

        public ImageView Image { get; private set; }
        public TextView PlayIcon { get; private set; }
        public TextView TypeIcon { get; private set; }
        #endregion

        public HSharedFilesAdapterViewHolder(View itemView, Action<HSharedFilesAdapterViewHolderClickEventArgs> clickListener, Action<HSharedFilesAdapterViewHolderClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;
                Image = (ImageView)MainView.FindViewById(Resource.Id.Image);
                TypeIcon = (TextView)MainView.FindViewById(Resource.Id.typeicon);
                PlayIcon = (TextView)MainView.FindViewById(Resource.Id.playicon);


                //Create an Event
                MainView.Click += (sender, e) => clickListener(new HSharedFilesAdapterViewHolderClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new HSharedFilesAdapterViewHolderClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class HSharedFilesAdapterViewHolderClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}