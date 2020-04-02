using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WoWonder.Helpers.Model;
using WoWonder.SQLite;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Message;

namespace WoWonder.Helpers.Utils
{
    public static class ListUtils
    {
        //############# DON'T MODIFY HERE #############
        //List Items Declaration 
        //*********************************************************

        public static GetSiteSettingsObject.Config SettingsSiteList;
        public static readonly ObservableCollection<DataTables.LoginTb> DataUserLoginList = new ObservableCollection<DataTables.LoginTb>();
        public static ObservableCollection<UserDataObject> MyProfileList = new ObservableCollection<UserDataObject>();
        public static ObservableCollection<ChatObject> UserList = new ObservableCollection<ChatObject>();
        public static ObservableCollection<GetUsersListObject.User> UserChatList = new ObservableCollection<GetUsersListObject.User>();
        public static ObservableCollection<UserDataObject> ListCachedDataNearby = new ObservableCollection<UserDataObject>();
        public static ObservableCollection<UserDataObject> FriendRequestsList = new ObservableCollection<UserDataObject>();
        public static ObservableCollection<Classes.Categories> ListCategoriesProducts = new ObservableCollection<Classes.Categories>();
        public static ObservableCollection<Classes.SharedFile> ListSharedFiles = new ObservableCollection<Classes.SharedFile>();
        public static ObservableCollection<Classes.SharedFile> LastSharedFiles = new ObservableCollection<Classes.SharedFile>();
        public static ObservableCollection<GetGeneralDataObject.GroupChatRequest> GroupRequestsList = new ObservableCollection<GetGeneralDataObject.GroupChatRequest>();

        public static void ClearAllList()
        {
            try
            {
                DataUserLoginList.Clear();
                MyProfileList.Clear();
                UserList.Clear();
                UserChatList.Clear();
                ListCachedDataNearby.Clear();
                FriendRequestsList.Clear();
                ListCategoriesProducts.Clear();
                ListSharedFiles.Clear();
                LastSharedFiles.Clear();
                GroupRequestsList.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void AddRange<T>(ObservableCollection<T> collection, IEnumerable<T> items)
        {
            try
            {
                items.ToList().ForEach(collection.Add);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public static List<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            var list = new List<List<T>>();

            for (int i = 0; i < locations.Count; i += nSize)
            {
                list.Add(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            }

            return list;
        }

        public static IEnumerable<T> TakeLast<T>(IEnumerable<T> source, int n)
        {
            var enumerable = source as T[] ?? source.ToArray();

            return enumerable.Skip(Math.Max(0, enumerable.Count() - n));
        }

    }
}