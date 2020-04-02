using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.Content;
using AutoMapper;
using FloatingView.Lib;
using Java.Lang;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using WoWonder.Activities.ChatWindow;
using WoWonder.Frameworks.Floating;
using WoWonder.Frameworks.onesignal;
using WoWonder.Helpers.Model;
using WoWonder.Helpers.Utils;
using WoWonderClient;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Message;
using WoWonderClient.Classes.Posts;
using WoWonderClient.Classes.Product;
using Environment = System.Environment;
using Exception = System.Exception;


namespace WoWonder.SQLite
{
    public class SqLiteDatabase : IDisposable
    {
        //############# DON'T MODIFY HERE #############
        private static readonly string Folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public static readonly string PathCombine = Path.Combine(Folder, "BadamiMessenger.db");
        private SQLiteConnection Connection;

        //Open Connection in Database
        //*********************************************************

        #region Connection

        private SQLiteConnection OpenConnection()
        {
            try
            {
                Connection = new SQLiteConnection(PathCombine);
                return Connection;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public void CheckTablesStatus()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;

                    Connection.CreateTable<DataTables.LoginTb>();
                    Connection.CreateTable<DataTables.SettingsTb>();
                    Connection.CreateTable<DataTables.MyContactsTb>();
                    Connection.CreateTable<DataTables.MyFollowersTb>();
                    Connection.CreateTable<DataTables.MyProfileTb>();
                    Connection.CreateTable<DataTables.SearchFilterTb>();
                    Connection.CreateTable<DataTables.NearByFilterTb>();
                    Connection.CreateTable<DataTables.CallUserTb>();

                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                        Connection.CreateTable<DataTables.LastUsersTb>();
                    else
                        Connection.CreateTable<DataTables.LastUsersChatTb>();

                    Connection.CreateTable<DataTables.MessageTb>();
                    Connection.CreateTable<DataTables.FilterLastChatTb>();
                    Connection.Dispose();
                    Connection.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Close Connection in Database
        public void Dispose()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    Connection.Dispose();
                    Connection.Close();
                    GC.SuppressFinalize(this);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
        }

        public void ClearAll()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    Connection.DeleteAll<DataTables.LoginTb>();
                    Connection.DeleteAll<DataTables.SettingsTb>();
                    Connection.DeleteAll<DataTables.MyContactsTb>();
                    Connection.DeleteAll<DataTables.MyFollowersTb>();
                    Connection.DeleteAll<DataTables.MyProfileTb>();
                    Connection.DeleteAll<DataTables.SearchFilterTb>();
                    Connection.DeleteAll<DataTables.NearByFilterTb>();
                    Connection.DeleteAll<DataTables.CallUserTb>();

                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                        Connection.DeleteAll<DataTables.LastUsersTb>();
                    else
                        Connection.DeleteAll<DataTables.LastUsersChatTb>();

                    Connection.DeleteAll<DataTables.MessageTb>();
                    Connection.DeleteAll<DataTables.FilterLastChatTb>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);

            }
        }

        //Delete table 
        public void DropAll()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    Connection.DropTable<DataTables.LoginTb>();
                    Connection.DropTable<DataTables.SettingsTb>();
                    Connection.DropTable<DataTables.MyContactsTb>();
                    Connection.DropTable<DataTables.MyFollowersTb>();
                    Connection.DropTable<DataTables.MyProfileTb>();
                    Connection.DropTable<DataTables.SearchFilterTb>();
                    Connection.DropTable<DataTables.NearByFilterTb>();
                    Connection.DropTable<DataTables.CallUserTb>();

                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                        Connection.DropTable<DataTables.LastUsersTb>();
                    else
                        Connection.DropTable<DataTables.LastUsersChatTb>();

                    Connection.DropTable<DataTables.MessageTb>();
                    Connection.DropTable<DataTables.FilterLastChatTb>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);

            }
        }

        #endregion

        //########################## End SQLite_Entity ##########################

        //Start SQL_Commander >>  General 
        //*********************************************************

        #region General

        public void InsertRow(object row)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    Connection.Insert(row);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void UpdateRow(object row)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    Connection.Update(row);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void DeleteRow(object row)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    Connection.Delete(row);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void InsertListOfRows(List<object> row)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    Connection.InsertAll(row);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        //Start SQL_Commander >>  Custom 
        //*********************************************************

        #region Login

        //Insert Or Update data Login
        public void InsertOrUpdateLogin_Credentials(DataTables.LoginTb db)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    var dataUser = Connection.Table<DataTables.LoginTb>().FirstOrDefault();
                    if (dataUser != null)
                    {
                        dataUser.UserId = UserDetails.UserId;
                        dataUser.AccessToken = UserDetails.AccessToken;
                        dataUser.Cookie = UserDetails.Cookie;
                        dataUser.Username = UserDetails.Username;
                        dataUser.Password = UserDetails.Password;
                        dataUser.Status = UserDetails.Status;
                        dataUser.Lang = AppSettings.Lang;
                        dataUser.DeviceId = UserDetails.DeviceId;
                        dataUser.Email = UserDetails.Email;

                        Connection.Update(dataUser);
                    }
                    else
                    {
                        Connection.Insert(db);
                    }

                    Methods.GenerateNoteOnSD(JsonConvert.SerializeObject(db));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Get data Login
        public DataTables.LoginTb Get_data_Login_Credentials()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return null;
                    var dataUser = Connection.Table<DataTables.LoginTb>().FirstOrDefault();
                    if (dataUser != null)
                    {
                        UserDetails.Username = dataUser.Username;
                        UserDetails.FullName = dataUser.Username;
                        UserDetails.Password = dataUser.Password;
                        UserDetails.AccessToken = dataUser.AccessToken;
                        UserDetails.UserId = dataUser.UserId;
                        UserDetails.Status = dataUser.Status;
                        UserDetails.Cookie = dataUser.Cookie;
                        UserDetails.Email = dataUser.Email;

                        AppSettings.Lang = dataUser.Lang;
                        UserDetails.DeviceId = dataUser.DeviceId;

                        Current.AccessToken = dataUser.AccessToken;
                        ListUtils.DataUserLoginList.Add(dataUser);

                        return dataUser;
                    }

                    return null;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        #endregion

        #region Settings

        public void InsertOrUpdateSettings(GetSiteSettingsObject.Config settingsData)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    if (settingsData != null)
                    {
                        var select = Connection.Table<DataTables.SettingsTb>().FirstOrDefault();
                        if (select == null)
                        {
                            var db = Mapper.Map<DataTables.SettingsTb>(settingsData);

                            db.CurrencyArray = JsonConvert.SerializeObject(settingsData.CurrencyArray.CurrencyList);
                            db.CurrencySymbolArray = JsonConvert.SerializeObject(settingsData.CurrencySymbolArray.CurrencyList);
                            db.PageCategories = JsonConvert.SerializeObject(settingsData.PageCategories);
                            db.GroupCategories = JsonConvert.SerializeObject(settingsData.GroupCategories);
                            db.BlogCategories = JsonConvert.SerializeObject(settingsData.BlogCategories);
                            db.ProductsCategories = JsonConvert.SerializeObject(settingsData.ProductsCategories);
                            db.JobCategories = JsonConvert.SerializeObject(settingsData.JobCategories);
                            db.Genders = JsonConvert.SerializeObject(settingsData.Genders);
                            db.Family = JsonConvert.SerializeObject(settingsData.Family);
                            db.MovieCategory = JsonConvert.SerializeObject(settingsData.MovieCategory);
                            if (settingsData.PostColors != null)
                                db.PostColors = JsonConvert.SerializeObject(settingsData.PostColors.Value.PostColorsList);
                            db.PostReactionsTypes = JsonConvert.SerializeObject(settingsData.PostReactionsTypes);
                            db.Fields = JsonConvert.SerializeObject(settingsData.Fields);

                            Connection.Insert(db);
                        }
                        else
                        {
                            select = Mapper.Map<DataTables.SettingsTb>(settingsData);

                            select.CurrencyArray = JsonConvert.SerializeObject(settingsData.CurrencyArray.CurrencyList);
                            select.CurrencySymbolArray = JsonConvert.SerializeObject(settingsData.CurrencySymbolArray.CurrencyList);
                            select.PageCategories = JsonConvert.SerializeObject(settingsData.PageCategories);
                            select.GroupCategories = JsonConvert.SerializeObject(settingsData.GroupCategories);
                            select.BlogCategories = JsonConvert.SerializeObject(settingsData.BlogCategories);
                            select.ProductsCategories = JsonConvert.SerializeObject(settingsData.ProductsCategories);
                            select.JobCategories = JsonConvert.SerializeObject(settingsData.JobCategories);
                            select.Genders = JsonConvert.SerializeObject(settingsData.Genders);
                            select.Family = JsonConvert.SerializeObject(settingsData.Family);
                            select.MovieCategory = JsonConvert.SerializeObject(settingsData.MovieCategory);
                            if (settingsData.PostColors != null)
                                select.PostColors = JsonConvert.SerializeObject(settingsData.PostColors.Value.PostColorsList);
                            select.PostReactionsTypes = JsonConvert.SerializeObject(settingsData.PostReactionsTypes);
                            select.Fields = JsonConvert.SerializeObject(settingsData.Fields);

                            Connection.Update(select);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get Settings
        public GetSiteSettingsObject.Config GetSettings()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return null;
                    using (Connection = OpenConnection())
                    {
                        var select = Connection.Table<DataTables.SettingsTb>().FirstOrDefault();
                        if (select != null)
                        {
                            var db = Mapper.Map<GetSiteSettingsObject.Config>(select);
                            if (db != null)
                            {
                                GetSiteSettingsObject.Config asd = db;
                                asd.CurrencyArray = new GetSiteSettingsObject.CurrencyArray();
                                asd.CurrencySymbolArray = new GetSiteSettingsObject.CurrencySymbol();
                                asd.PageCategories = new Dictionary<string, string>();
                                asd.GroupCategories = new Dictionary<string, string>();
                                asd.BlogCategories = new Dictionary<string, string>();
                                asd.ProductsCategories = new Dictionary<string, string>();
                                asd.JobCategories = new Dictionary<string, string>();
                                asd.Genders = new Dictionary<string, string>();
                                asd.Family = new Dictionary<string, string>();
                                asd.MovieCategory = new Dictionary<string, string>();
                                asd.PostColors = new Dictionary<string, PostColorsObject>();
                                asd.PostReactionsTypes = new List<string>();
                                asd.Fields = new List<Field>();

                                if (!string.IsNullOrEmpty(select.CurrencyArray))
                                    asd.CurrencyArray = new GetSiteSettingsObject.CurrencyArray
                                    {
                                        CurrencyList = JsonConvert.DeserializeObject<List<string>>(select.CurrencyArray)
                                    };

                                if (!string.IsNullOrEmpty(select.CurrencySymbolArray))
                                    asd.CurrencySymbolArray = new GetSiteSettingsObject.CurrencySymbol
                                    {
                                        CurrencyList = JsonConvert.DeserializeObject<CurrencySymbolArray>(select.CurrencySymbolArray),
                                    };

                                if (!string.IsNullOrEmpty(select.PageCategories))
                                    asd.PageCategories = JsonConvert.DeserializeObject<Dictionary<string, string>>(select.PageCategories);

                                if (!string.IsNullOrEmpty(select.GroupCategories))
                                    asd.GroupCategories = JsonConvert.DeserializeObject<Dictionary<string, string>>(select.GroupCategories);

                                if (!string.IsNullOrEmpty(select.BlogCategories))
                                    asd.BlogCategories = JsonConvert.DeserializeObject<Dictionary<string, string>>(select.BlogCategories);

                                if (!string.IsNullOrEmpty(select.ProductsCategories))
                                    asd.ProductsCategories = JsonConvert.DeserializeObject<Dictionary<string, string>>(select.ProductsCategories);

                                if (!string.IsNullOrEmpty(select.JobCategories))
                                    asd.JobCategories = JsonConvert.DeserializeObject<Dictionary<string, string>>(select.JobCategories);

                                if (!string.IsNullOrEmpty(select.Genders))
                                    asd.Genders = JsonConvert.DeserializeObject<Dictionary<string, string>>(select.Genders);

                                if (!string.IsNullOrEmpty(select.Family))
                                    asd.Family = JsonConvert.DeserializeObject<Dictionary<string, string>>(select.Family);

                                if (!string.IsNullOrEmpty(select.MovieCategory))
                                    asd.MovieCategory = JsonConvert.DeserializeObject<Dictionary<string, string>>(select.Family);

                                if (!string.IsNullOrEmpty(select.PostColors))
                                    asd.PostColors = new GetSiteSettingsObject.PostColorUnion { PostColorsList = JsonConvert.DeserializeObject<Dictionary<string, PostColorsObject>>(select.PostColors) };

                                if (!string.IsNullOrEmpty(select.PostReactionsTypes))
                                    asd.PostReactionsTypes = JsonConvert.DeserializeObject<List<string>>(select.PostReactionsTypes);

                                if (!string.IsNullOrEmpty(select.Fields))
                                    asd.Fields = JsonConvert.DeserializeObject<List<Field>>(select.Fields);

                                //Products Categories
                                var listProducts = db.ProductsCategories.Select(cat => new Classes.Categories
                                {
                                    CategoriesId = cat.Key,
                                    CategoriesName = cat.Value,
                                    CategoriesColor = "#ffffff"
                                }).ToList();

                                ListUtils.ListCategoriesProducts.Clear();
                                ListUtils.ListCategoriesProducts = new ObservableCollection<Classes.Categories>(listProducts);

                                AppSettings.OneSignalAppId = asd.AndroidMPushId;
                                OneSignalNotification.RegisterNotificationDevice();

                                return asd;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        #endregion

        #region My Contacts >> Following

        //Insert data To My Contact Table
        public void Insert_Or_Replace_MyContactTable(ObservableCollection<UserDataObject> usersContactList)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    var result = Connection.Table<DataTables.MyContactsTb>().ToList();
                    List<DataTables.MyContactsTb> list = new List<DataTables.MyContactsTb>();
                    foreach (var info in usersContactList)
                    {
                        var db = Mapper.Map<DataTables.MyContactsTb>(info);
                        if (info.Details.DetailsClass != null)
                            db.Details = JsonConvert.SerializeObject(info.Details.DetailsClass);
                        list.Add(db);

                        var update = result.FirstOrDefault(a => a.UserId == info.UserId);
                        if (update != null)
                        {
                            update = db;
                            if (info.Details.DetailsClass != null)
                                update.Details = JsonConvert.SerializeObject(info.Details.DetailsClass);

                            Connection.Update(update);
                        }
                    }

                    if (list.Count <= 0) return;

                    Connection.BeginTransaction();
                    //Bring new  
                    var newItemList = list.Where(c => !result.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                    if (newItemList.Count > 0)
                        Connection.InsertAll(newItemList);

                    result = Connection.Table<DataTables.MyContactsTb>().ToList();
                    var deleteItemList = result.Where(c => !list.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                    if (deleteItemList.Count > 0)
                        foreach (var delete in deleteItemList)
                            Connection.Delete(delete);

                    Connection.Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Get data To My Contact Table
        public ObservableCollection<UserDataObject> Get_MyContact(/*int id = 0, int nSize = 20*/)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return new ObservableCollection<UserDataObject>();
                    // var query = Connection.Table<DataTables.MyContactsTb>().Where(w => w.AutoIdMyFollowing >= id).OrderBy(q => q.AutoIdMyFollowing).Take(nSize).ToList();

                    var select = Connection.Table<DataTables.MyContactsTb>().ToList();
                    if (select.Count > 0)
                    {
                        var list = new ObservableCollection<UserDataObject>();

                        foreach (var item in select)
                        {
                            UserDataObject infoObject = new UserDataObject
                            {
                                UserId = item.UserId,
                                Username = item.Username,
                                Email = item.Email,
                                FirstName = item.FirstName,
                                LastName = item.LastName,
                                Avatar = item.Avatar,
                                Cover = item.Cover,
                                BackgroundImage = item.BackgroundImage,
                                RelationshipId = item.RelationshipId,
                                Address = item.Address,
                                Working = item.Working,
                                Gender = item.Gender,
                                Facebook = item.Facebook,
                                Google = item.Google,
                                Twitter = item.Twitter,
                                Linkedin = item.Linkedin,
                                Website = item.Website,
                                Instagram = item.Instagram,
                                WebDeviceId = item.WebDeviceId,
                                Language = item.Language,
                                IpAddress = item.IpAddress,
                                PhoneNumber = item.PhoneNumber,
                                Timezone = item.Timezone,
                                Lat = item.Lat,
                                Lng = item.Lng,
                                About = item.About,
                                Birthday = item.Birthday,
                                Registered = item.Registered,
                                Lastseen = item.Lastseen,
                                LastLocationUpdate = item.LastLocationUpdate,
                                Balance = item.Balance,
                                Verified = item.Verified,
                                Status = item.Status,
                                Active = item.Active,
                                Admin = item.Admin,
                                IsPro = item.IsPro,
                                ProType = item.ProType,
                                School = item.School,
                                Name = item.Name,
                                AndroidMDeviceId = item.AndroidMDeviceId,
                                ECommented = item.ECommented,
                                AndroidNDeviceId = item.AndroidMDeviceId,
                                AvatarFull = item.AvatarFull,
                                BirthPrivacy = item.BirthPrivacy,
                                CanFollow = item.CanFollow,
                                ConfirmFollowers = item.ConfirmFollowers,
                                CountryId = item.CountryId,
                                EAccepted = item.EAccepted,
                                EFollowed = item.EFollowed,
                                EJoinedGroup = item.EJoinedGroup,
                                ELastNotif = item.ELastNotif,
                                ELiked = item.ELiked,
                                ELikedPage = item.ELikedPage,
                                EMentioned = item.EMentioned,
                                EProfileWallPost = item.EProfileWallPost,
                                ESentmeMsg = item.ESentmeMsg,
                                EShared = item.EShared,
                                EVisited = item.EVisited,
                                EWondered = item.EWondered,
                                EmailNotification = item.EmailNotification,
                                FollowPrivacy = item.FollowPrivacy,
                                FriendPrivacy = item.FriendPrivacy,
                                GenderText = item.GenderText,
                                InfoFile = item.InfoFile,
                                IosMDeviceId = item.IosMDeviceId,
                                IosNDeviceId = item.IosNDeviceId,
                                IsBlocked = item.IsBlocked,
                                IsFollowing = item.IsFollowing,
                                IsFollowingMe = item.IsFollowingMe,
                                LastAvatarMod = item.LastAvatarMod,
                                LastCoverMod = item.LastCoverMod,
                                LastDataUpdate = item.LastDataUpdate,
                                LastFollowId = item.LastFollowId,
                                LastLoginData = item.LastLoginData,
                                LastseenStatus = item.LastseenStatus,
                                LastseenTimeText = item.LastseenTimeText,
                                LastseenUnixTime = item.LastseenUnixTime,
                                MessagePrivacy = item.MessagePrivacy,
                                NewEmail = item.NewEmail,
                                NewPhone = item.NewPhone,
                                NotificationSettings = item.NotificationSettings,
                                NotificationsSound = item.NotificationsSound,
                                OrderPostsBy = item.OrderPostsBy,
                                PaypalEmail = item.PaypalEmail,
                                PostPrivacy = item.PostPrivacy,
                                Referrer = item.Referrer,
                                ShareMyData = item.ShareMyData,
                                ShareMyLocation = item.ShareMyLocation,
                                ShowActivitiesPrivacy = item.ShowActivitiesPrivacy,
                                TwoFactor = item.TwoFactor,
                                TwoFactorVerified = item.TwoFactorVerified,
                                Url = item.Url,
                                VisitPrivacy = item.VisitPrivacy,
                                Vk = item.Vk,
                                Wallet = item.Wallet,
                                WorkingLink = item.WorkingLink,
                                Youtube = item.Youtube,
                                City = item.City,
                                Points = item.Points,
                                DailyPoints = item.DailyPoints,
                                PointDayExpire = item.PointDayExpire,
                                State = item.State,
                                Zip = item.Zip,
                                Details = new DetailsUnion { DetailsClass = new Details() },
                                Selected = false,
                            };

                            if (!string.IsNullOrEmpty(item.Details))
                                infoObject.Details = new DetailsUnion
                                {
                                    DetailsClass = JsonConvert.DeserializeObject<Details>(item.Details)
                                };

                            list.Add(infoObject);
                        }

                        return list;
                    }
                    else
                    {
                        return new ObservableCollection<UserDataObject>();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ObservableCollection<UserDataObject>();
            }
        }

        public void Delete_UsersContact(string userId)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    var user = Connection.Table<DataTables.MyContactsTb>().FirstOrDefault(c => c.UserId == userId);
                    if (user != null)
                    {
                        Connection.Delete(user);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region My Contacts >> Following

        //Insert data To my Followers Table
        public void Insert_Or_Replace_MyFollowersTable(ObservableCollection<UserDataObject> myFollowersList)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    var result = Connection.Table<DataTables.MyFollowersTb>().ToList();
                    List<DataTables.MyFollowersTb> list = new List<DataTables.MyFollowersTb>();
                    foreach (var info in myFollowersList)
                    {
                        var db = Mapper.Map<DataTables.MyFollowersTb>(info);
                        if (info.Details.DetailsClass != null)
                            db.Details = JsonConvert.SerializeObject(info.Details.DetailsClass);
                        list.Add(db);

                        var update = result.FirstOrDefault(a => a.UserId == info.UserId);
                        if (update != null)
                        {
                            update = db;
                            if (info.Details.DetailsClass != null)
                                update.Details = JsonConvert.SerializeObject(info.Details.DetailsClass);

                            Connection.Update(update);
                        }
                    }

                    if (list.Count <= 0) return;

                    Connection.BeginTransaction();
                    //Bring new  
                    var newItemList = list.Where(c => !result.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                    if (newItemList.Count > 0)
                        Connection.InsertAll(newItemList);

                    result = Connection.Table<DataTables.MyFollowersTb>().ToList();
                    var deleteItemList = result.Where(c => !list.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                    if (deleteItemList.Count > 0)
                        foreach (var delete in deleteItemList)
                            Connection.Delete(delete);

                    Connection.Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Get data To my Followers Table
        public ObservableCollection<UserDataObject> Get_MyFollowers(/*int id = 0, int nSize = 20*/)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return null;
                    // var query = Connection.Table<DataTables.MyFollowersTb>().Where(w => w.AutoIdMyFollowing >= id).OrderBy(q => q.AutoIdMyFollowing).Take(nSize).ToList();

                    var select = Connection.Table<DataTables.MyFollowersTb>().ToList();
                    if (select.Count > 0)
                    {
                        var list = new ObservableCollection<UserDataObject>();
                        foreach (var item in select)
                        {
                            UserDataObject infoObject = new UserDataObject
                            {
                                UserId = item.UserId,
                                Username = item.Username,
                                Email = item.Email,
                                FirstName = item.FirstName,
                                LastName = item.LastName,
                                Avatar = item.Avatar,
                                Cover = item.Cover,
                                BackgroundImage = item.BackgroundImage,
                                RelationshipId = item.RelationshipId,
                                Address = item.Address,
                                Working = item.Working,
                                Gender = item.Gender,
                                Facebook = item.Facebook,
                                Google = item.Google,
                                Twitter = item.Twitter,
                                Linkedin = item.Linkedin,
                                Website = item.Website,
                                Instagram = item.Instagram,
                                WebDeviceId = item.WebDeviceId,
                                Language = item.Language,
                                IpAddress = item.IpAddress,
                                PhoneNumber = item.PhoneNumber,
                                Timezone = item.Timezone,
                                Lat = item.Lat,
                                Lng = item.Lng,
                                About = item.About,
                                Birthday = item.Birthday,
                                Registered = item.Registered,
                                Lastseen = item.Lastseen,
                                LastLocationUpdate = item.LastLocationUpdate,
                                Balance = item.Balance,
                                Verified = item.Verified,
                                Status = item.Status,
                                Active = item.Active,
                                Admin = item.Admin,
                                IsPro = item.IsPro,
                                ProType = item.ProType,
                                School = item.School,
                                Name = item.Name,
                                AndroidMDeviceId = item.AndroidMDeviceId,
                                ECommented = item.ECommented,
                                AndroidNDeviceId = item.AndroidMDeviceId,
                                AvatarFull = item.AvatarFull,
                                BirthPrivacy = item.BirthPrivacy,
                                CanFollow = item.CanFollow,
                                ConfirmFollowers = item.ConfirmFollowers,
                                CountryId = item.CountryId,
                                EAccepted = item.EAccepted,
                                EFollowed = item.EFollowed,
                                EJoinedGroup = item.EJoinedGroup,
                                ELastNotif = item.ELastNotif,
                                ELiked = item.ELiked,
                                ELikedPage = item.ELikedPage,
                                EMentioned = item.EMentioned,
                                EProfileWallPost = item.EProfileWallPost,
                                ESentmeMsg = item.ESentmeMsg,
                                EShared = item.EShared,
                                EVisited = item.EVisited,
                                EWondered = item.EWondered,
                                EmailNotification = item.EmailNotification,
                                FollowPrivacy = item.FollowPrivacy,
                                FriendPrivacy = item.FriendPrivacy,
                                GenderText = item.GenderText,
                                InfoFile = item.InfoFile,
                                IosMDeviceId = item.IosMDeviceId,
                                IosNDeviceId = item.IosNDeviceId,
                                IsBlocked = item.IsBlocked,
                                IsFollowing = item.IsFollowing,
                                IsFollowingMe = item.IsFollowingMe,
                                LastAvatarMod = item.LastAvatarMod,
                                LastCoverMod = item.LastCoverMod,
                                LastDataUpdate = item.LastDataUpdate,
                                LastFollowId = item.LastFollowId,
                                LastLoginData = item.LastLoginData,
                                LastseenStatus = item.LastseenStatus,
                                LastseenTimeText = item.LastseenTimeText,
                                LastseenUnixTime = item.LastseenUnixTime,
                                MessagePrivacy = item.MessagePrivacy,
                                NewEmail = item.NewEmail,
                                NewPhone = item.NewPhone,
                                NotificationSettings = item.NotificationSettings,
                                NotificationsSound = item.NotificationsSound,
                                OrderPostsBy = item.OrderPostsBy,
                                PaypalEmail = item.PaypalEmail,
                                PostPrivacy = item.PostPrivacy,
                                Referrer = item.Referrer,
                                ShareMyData = item.ShareMyData,
                                ShareMyLocation = item.ShareMyLocation,
                                ShowActivitiesPrivacy = item.ShowActivitiesPrivacy,
                                TwoFactor = item.TwoFactor,
                                TwoFactorVerified = item.TwoFactorVerified,
                                Url = item.Url,
                                VisitPrivacy = item.VisitPrivacy,
                                Vk = item.Vk,
                                Wallet = item.Wallet,
                                WorkingLink = item.WorkingLink,
                                Youtube = item.Youtube,
                                City = item.City,
                                Points = item.Points,
                                DailyPoints = item.DailyPoints,
                                PointDayExpire = item.PointDayExpire,
                                State = item.State,
                                Zip = item.Zip,
                                Details = new DetailsUnion { DetailsClass = new Details() },
                                Selected = false,
                            };

                            if (!string.IsNullOrEmpty(item.Details))
                                infoObject.Details = new DetailsUnion
                                {
                                    DetailsClass = JsonConvert.DeserializeObject<Details>(item.Details)
                                };

                            list.Add(infoObject);
                        }

                        return list;
                    }
                    else
                    {
                        return new ObservableCollection<UserDataObject>();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ObservableCollection<UserDataObject>();
            }
        }

        #endregion

        // Get data One user To My Contact Table
        public UserDataObject Get_DataOneUser(string userName)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return null;
                    var item = Connection.Table<DataTables.MyContactsTb>().FirstOrDefault(a => a.Username == userName || a.Name == userName);
                    if (item != null)
                    {
                        UserDataObject infoObject = new UserDataObject
                        {
                            UserId = item.UserId,
                            Username = item.Username,
                            Email = item.Email,
                            FirstName = item.FirstName,
                            LastName = item.LastName,
                            Avatar = item.Avatar,
                            Cover = item.Cover,
                            BackgroundImage = item.BackgroundImage,
                            RelationshipId = item.RelationshipId,
                            Address = item.Address,
                            Working = item.Working,
                            Gender = item.Gender,
                            Facebook = item.Facebook,
                            Google = item.Google,
                            Twitter = item.Twitter,
                            Linkedin = item.Linkedin,
                            Website = item.Website,
                            Instagram = item.Instagram,
                            WebDeviceId = item.WebDeviceId,
                            Language = item.Language,
                            IpAddress = item.IpAddress,
                            PhoneNumber = item.PhoneNumber,
                            Timezone = item.Timezone,
                            Lat = item.Lat,
                            Lng = item.Lng,
                            About = item.About,
                            Birthday = item.Birthday,
                            Registered = item.Registered,
                            Lastseen = item.Lastseen,
                            LastLocationUpdate = item.LastLocationUpdate,
                            Balance = item.Balance,
                            Verified = item.Verified,
                            Status = item.Status,
                            Active = item.Active,
                            Admin = item.Admin,
                            IsPro = item.IsPro,
                            ProType = item.ProType,
                            School = item.School,
                            Name = item.Name,
                            AndroidMDeviceId = item.AndroidMDeviceId,
                            ECommented = item.ECommented,
                            AndroidNDeviceId = item.AndroidMDeviceId,
                            AvatarFull = item.AvatarFull,
                            BirthPrivacy = item.BirthPrivacy,
                            CanFollow = item.CanFollow,
                            ConfirmFollowers = item.ConfirmFollowers,
                            CountryId = item.CountryId,
                            EAccepted = item.EAccepted,
                            EFollowed = item.EFollowed,
                            EJoinedGroup = item.EJoinedGroup,
                            ELastNotif = item.ELastNotif,
                            ELiked = item.ELiked,
                            ELikedPage = item.ELikedPage,
                            EMentioned = item.EMentioned,
                            EProfileWallPost = item.EProfileWallPost,
                            ESentmeMsg = item.ESentmeMsg,
                            EShared = item.EShared,
                            EVisited = item.EVisited,
                            EWondered = item.EWondered,
                            EmailNotification = item.EmailNotification,
                            FollowPrivacy = item.FollowPrivacy,
                            FriendPrivacy = item.FriendPrivacy,
                            GenderText = item.GenderText,
                            InfoFile = item.InfoFile,
                            IosMDeviceId = item.IosMDeviceId,
                            IosNDeviceId = item.IosNDeviceId,
                            IsBlocked = item.IsBlocked,
                            IsFollowing = item.IsFollowing,
                            IsFollowingMe = item.IsFollowingMe,
                            LastAvatarMod = item.LastAvatarMod,
                            LastCoverMod = item.LastCoverMod,
                            LastDataUpdate = item.LastDataUpdate,
                            LastFollowId = item.LastFollowId,
                            LastLoginData = item.LastLoginData,
                            LastseenStatus = item.LastseenStatus,
                            LastseenTimeText = item.LastseenTimeText,
                            LastseenUnixTime = item.LastseenUnixTime,
                            MessagePrivacy = item.MessagePrivacy,
                            NewEmail = item.NewEmail,
                            NewPhone = item.NewPhone,
                            NotificationSettings = item.NotificationSettings,
                            NotificationsSound = item.NotificationsSound,
                            OrderPostsBy = item.OrderPostsBy,
                            PaypalEmail = item.PaypalEmail,
                            PostPrivacy = item.PostPrivacy,
                            Referrer = item.Referrer,
                            ShareMyData = item.ShareMyData,
                            ShareMyLocation = item.ShareMyLocation,
                            ShowActivitiesPrivacy = item.ShowActivitiesPrivacy,
                            TwoFactor = item.TwoFactor,
                            TwoFactorVerified = item.TwoFactorVerified,
                            Url = item.Url,
                            VisitPrivacy = item.VisitPrivacy,
                            Vk = item.Vk,
                            Wallet = item.Wallet,
                            WorkingLink = item.WorkingLink,
                            Youtube = item.Youtube,
                            City = item.City,
                            DailyPoints = item.DailyPoints,
                            PointDayExpire = item.PointDayExpire,
                            State = item.State,
                            Zip = item.Zip,
                            Details = new DetailsUnion { DetailsClass = new Details() },
                            Selected = false,
                        };

                        if (!string.IsNullOrEmpty(item.Details))
                            infoObject.Details = new DetailsUnion
                            {
                                DetailsClass = JsonConvert.DeserializeObject<Details>(item.Details)
                            };

                        return infoObject;
                    }
                    else
                    {
                        var userFollowers = Connection.Table<DataTables.MyFollowersTb>().FirstOrDefault(a => a.Username == userName || a.Name == userName);
                        if (userFollowers != null)
                        {
                            UserDataObject infoObject = new UserDataObject
                            {
                                UserId = userFollowers.UserId,
                                Username = userFollowers.Username,
                                Email = userFollowers.Email,
                                FirstName = userFollowers.FirstName,
                                LastName = userFollowers.LastName,
                                Avatar = userFollowers.Avatar,
                                Cover = userFollowers.Cover,
                                BackgroundImage = userFollowers.BackgroundImage,
                                RelationshipId = userFollowers.RelationshipId,
                                Address = userFollowers.Address,
                                Working = userFollowers.Working,
                                Gender = userFollowers.Gender,
                                Facebook = userFollowers.Facebook,
                                Google = userFollowers.Google,
                                Twitter = userFollowers.Twitter,
                                Linkedin = userFollowers.Linkedin,
                                Website = userFollowers.Website,
                                Instagram = userFollowers.Instagram,
                                WebDeviceId = userFollowers.WebDeviceId,
                                Language = userFollowers.Language,
                                IpAddress = userFollowers.IpAddress,
                                PhoneNumber = userFollowers.PhoneNumber,
                                Timezone = userFollowers.Timezone,
                                Lat = userFollowers.Lat,
                                Lng = userFollowers.Lng,
                                About = userFollowers.About,
                                Birthday = userFollowers.Birthday,
                                Registered = userFollowers.Registered,
                                Lastseen = userFollowers.Lastseen,
                                LastLocationUpdate = userFollowers.LastLocationUpdate,
                                Balance = userFollowers.Balance,
                                Verified = userFollowers.Verified,
                                Status = userFollowers.Status,
                                Active = userFollowers.Active,
                                Admin = userFollowers.Admin,
                                IsPro = userFollowers.IsPro,
                                ProType = userFollowers.ProType,
                                School = userFollowers.School,
                                Name = userFollowers.Name,
                                AndroidMDeviceId = userFollowers.AndroidMDeviceId,
                                ECommented = userFollowers.ECommented,
                                AndroidNDeviceId = userFollowers.AndroidMDeviceId,
                                AvatarFull = userFollowers.AvatarFull,
                                BirthPrivacy = userFollowers.BirthPrivacy,
                                CanFollow = userFollowers.CanFollow,
                                ConfirmFollowers = userFollowers.ConfirmFollowers,
                                CountryId = userFollowers.CountryId,
                                EAccepted = userFollowers.EAccepted,
                                EFollowed = userFollowers.EFollowed,
                                EJoinedGroup = userFollowers.EJoinedGroup,
                                ELastNotif = userFollowers.ELastNotif,
                                ELiked = userFollowers.ELiked,
                                ELikedPage = userFollowers.ELikedPage,
                                EMentioned = userFollowers.EMentioned,
                                EProfileWallPost = userFollowers.EProfileWallPost,
                                ESentmeMsg = userFollowers.ESentmeMsg,
                                EShared = userFollowers.EShared,
                                EVisited = userFollowers.EVisited,
                                EWondered = userFollowers.EWondered,
                                EmailNotification = userFollowers.EmailNotification,
                                FollowPrivacy = userFollowers.FollowPrivacy,
                                FriendPrivacy = userFollowers.FriendPrivacy,
                                GenderText = userFollowers.GenderText,
                                InfoFile = userFollowers.InfoFile,
                                IosMDeviceId = userFollowers.IosMDeviceId,
                                IosNDeviceId = userFollowers.IosNDeviceId,
                                IsBlocked = userFollowers.IsBlocked,
                                IsFollowing = userFollowers.IsFollowing,
                                IsFollowingMe = userFollowers.IsFollowingMe,
                                LastAvatarMod = userFollowers.LastAvatarMod,
                                LastCoverMod = userFollowers.LastCoverMod,
                                LastDataUpdate = userFollowers.LastDataUpdate,
                                LastFollowId = userFollowers.LastFollowId,
                                LastLoginData = userFollowers.LastLoginData,
                                LastseenStatus = userFollowers.LastseenStatus,
                                LastseenTimeText = userFollowers.LastseenTimeText,
                                LastseenUnixTime = userFollowers.LastseenUnixTime,
                                MessagePrivacy = userFollowers.MessagePrivacy,
                                NewEmail = userFollowers.NewEmail,
                                NewPhone = userFollowers.NewPhone,
                                NotificationSettings = userFollowers.NotificationSettings,
                                NotificationsSound = userFollowers.NotificationsSound,
                                OrderPostsBy = userFollowers.OrderPostsBy,
                                PaypalEmail = userFollowers.PaypalEmail,
                                PostPrivacy = userFollowers.PostPrivacy,
                                Referrer = userFollowers.Referrer,
                                ShareMyData = userFollowers.ShareMyData,
                                ShareMyLocation = userFollowers.ShareMyLocation,
                                ShowActivitiesPrivacy = userFollowers.ShowActivitiesPrivacy,
                                TwoFactor = userFollowers.TwoFactor,
                                TwoFactorVerified = userFollowers.TwoFactorVerified,
                                Url = userFollowers.Url,
                                VisitPrivacy = userFollowers.VisitPrivacy,
                                Vk = userFollowers.Vk,
                                Wallet = userFollowers.Wallet,
                                WorkingLink = userFollowers.WorkingLink,
                                Youtube = userFollowers.Youtube,
                                City = userFollowers.City,
                                DailyPoints = userFollowers.DailyPoints,
                                PointDayExpire = userFollowers.PointDayExpire,
                                State = userFollowers.State,
                                Zip = userFollowers.Zip,
                                Details = new DetailsUnion { DetailsClass = new Details() },
                                Selected = false,
                            };

                            if (!string.IsNullOrEmpty(userFollowers.Details))
                                infoObject.Details = new DetailsUnion
                                {
                                    DetailsClass = JsonConvert.DeserializeObject<Details>(userFollowers.Details)
                                };

                            return infoObject;
                        }

                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        #region My Profile

        //Insert Or Update data My Profile Table
        public void Insert_Or_Update_To_MyProfileTable(UserDataObject info)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    var resultInfoTb = Connection.Table<DataTables.MyProfileTb>().FirstOrDefault();
                    if (resultInfoTb != null)
                    {
                        resultInfoTb = new DataTables.MyProfileTb
                        {
                            UserId = info.UserId,
                            Username = info.Username,
                            Email = info.Email,
                            FirstName = info.FirstName,
                            LastName = info.LastName,
                            Avatar = info.Avatar,
                            Cover = info.Cover,
                            BackgroundImage = info.BackgroundImage,
                            RelationshipId = info.RelationshipId,
                            Address = info.Address,
                            Working = info.Working,
                            Gender = info.Gender,
                            Facebook = info.Facebook,
                            Google = info.Google,
                            Twitter = info.Twitter,
                            Linkedin = info.Linkedin,
                            Website = info.Website,
                            Instagram = info.Instagram,
                            WebDeviceId = info.WebDeviceId,
                            Language = info.Language,
                            IpAddress = info.IpAddress,
                            PhoneNumber = info.PhoneNumber,
                            Timezone = info.Timezone,
                            Lat = info.Lat,
                            Lng = info.Lng,
                            About = info.About,
                            Birthday = info.Birthday,
                            Registered = info.Registered,
                            Lastseen = info.Lastseen,
                            LastLocationUpdate = info.LastLocationUpdate,
                            Balance = info.Balance,
                            Verified = info.Verified,
                            Status = info.Status,
                            Active = info.Active,
                            Admin = info.Admin,
                            IsPro = info.IsPro,
                            ProType = info.ProType,
                            School = info.School,
                            Name = info.Name,
                            AndroidMDeviceId = info.AndroidMDeviceId,
                            ECommented = info.ECommented,
                            AndroidNDeviceId = info.AndroidMDeviceId,
                            AvatarFull = info.AvatarFull,
                            BirthPrivacy = info.BirthPrivacy,
                            CanFollow = info.CanFollow,
                            ConfirmFollowers = info.ConfirmFollowers,
                            CountryId = info.CountryId,
                            EAccepted = info.EAccepted,
                            EFollowed = info.EFollowed,
                            EJoinedGroup = info.EJoinedGroup,
                            ELastNotif = info.ELastNotif,
                            ELiked = info.ELiked,
                            ELikedPage = info.ELikedPage,
                            EMentioned = info.EMentioned,
                            EProfileWallPost = info.EProfileWallPost,
                            ESentmeMsg = info.ESentmeMsg,
                            EShared = info.EShared,
                            EVisited = info.EVisited,
                            EWondered = info.EWondered,
                            EmailNotification = info.EmailNotification,
                            FollowPrivacy = info.FollowPrivacy,
                            FriendPrivacy = info.FriendPrivacy,
                            GenderText = info.GenderText,
                            InfoFile = info.InfoFile,
                            IosMDeviceId = info.IosMDeviceId,
                            IosNDeviceId = info.IosNDeviceId,
                            IsBlocked = info.IsBlocked,
                            IsFollowing = info.IsFollowing,
                            IsFollowingMe = info.IsFollowingMe,
                            LastAvatarMod = info.LastAvatarMod,
                            LastCoverMod = info.LastCoverMod,
                            LastDataUpdate = info.LastDataUpdate,
                            LastFollowId = info.LastFollowId,
                            LastLoginData = info.LastLoginData,
                            LastseenStatus = info.LastseenStatus,
                            LastseenTimeText = info.LastseenTimeText,
                            LastseenUnixTime = info.LastseenUnixTime,
                            MessagePrivacy = info.MessagePrivacy,
                            NewEmail = info.NewEmail,
                            NewPhone = info.NewPhone,
                            NotificationSettings = info.NotificationSettings,
                            NotificationsSound = info.NotificationsSound,
                            OrderPostsBy = info.OrderPostsBy,
                            PaypalEmail = info.PaypalEmail,
                            PostPrivacy = info.PostPrivacy,
                            Referrer = info.Referrer,
                            ShareMyData = info.ShareMyData,
                            ShareMyLocation = info.ShareMyLocation,
                            ShowActivitiesPrivacy = info.ShowActivitiesPrivacy,
                            TwoFactor = info.TwoFactor,
                            TwoFactorVerified = info.TwoFactorVerified,
                            Url = info.Url,
                            VisitPrivacy = info.VisitPrivacy,
                            Vk = info.Vk,
                            Wallet = info.Wallet,
                            WorkingLink = info.WorkingLink,
                            Youtube = info.Youtube,
                            City = info.City,
                            Points = info.Points,
                            DailyPoints = info.DailyPoints,
                            PointDayExpire = info.PointDayExpire,
                            State = info.State,
                            Zip = info.Zip,
                            Details = string.Empty,
                            Selected = false,
                        };

                        if (info.Details.DetailsClass != null)
                            resultInfoTb.Details = JsonConvert.SerializeObject(info.Details.DetailsClass);

                        Connection.Update(resultInfoTb);
                    }
                    else
                    {
                        DataTables.MyProfileTb db = new DataTables.MyProfileTb
                        {
                            UserId = info.UserId,
                            Username = info.Username,
                            Email = info.Email,
                            FirstName = info.FirstName,
                            LastName = info.LastName,
                            Avatar = info.Avatar,
                            Cover = info.Cover,
                            BackgroundImage = info.BackgroundImage,
                            RelationshipId = info.RelationshipId,
                            Address = info.Address,
                            Working = info.Working,
                            Gender = info.Gender,
                            Facebook = info.Facebook,
                            Google = info.Google,
                            Twitter = info.Twitter,
                            Linkedin = info.Linkedin,
                            Website = info.Website,
                            Instagram = info.Instagram,
                            WebDeviceId = info.WebDeviceId,
                            Language = info.Language,
                            IpAddress = info.IpAddress,
                            PhoneNumber = info.PhoneNumber,
                            Timezone = info.Timezone,
                            Lat = info.Lat,
                            Lng = info.Lng,
                            About = info.About,
                            Birthday = info.Birthday,
                            Registered = info.Registered,
                            Lastseen = info.Lastseen,
                            LastLocationUpdate = info.LastLocationUpdate,
                            Balance = info.Balance,
                            Verified = info.Verified,
                            Status = info.Status,
                            Active = info.Active,
                            Admin = info.Admin,
                            IsPro = info.IsPro,
                            ProType = info.ProType,
                            School = info.School,
                            Name = info.Name,
                            AndroidMDeviceId = info.AndroidMDeviceId,
                            ECommented = info.ECommented,
                            AndroidNDeviceId = info.AndroidMDeviceId,
                            AvatarFull = info.AvatarFull,
                            BirthPrivacy = info.BirthPrivacy,
                            CanFollow = info.CanFollow,
                            ConfirmFollowers = info.ConfirmFollowers,
                            CountryId = info.CountryId,
                            EAccepted = info.EAccepted,
                            EFollowed = info.EFollowed,
                            EJoinedGroup = info.EJoinedGroup,
                            ELastNotif = info.ELastNotif,
                            ELiked = info.ELiked,
                            ELikedPage = info.ELikedPage,
                            EMentioned = info.EMentioned,
                            EProfileWallPost = info.EProfileWallPost,
                            ESentmeMsg = info.ESentmeMsg,
                            EShared = info.EShared,
                            EVisited = info.EVisited,
                            EWondered = info.EWondered,
                            EmailNotification = info.EmailNotification,
                            FollowPrivacy = info.FollowPrivacy,
                            FriendPrivacy = info.FriendPrivacy,
                            GenderText = info.GenderText,
                            InfoFile = info.InfoFile,
                            IosMDeviceId = info.IosMDeviceId,
                            IosNDeviceId = info.IosNDeviceId,
                            IsBlocked = info.IsBlocked,
                            IsFollowing = info.IsFollowing,
                            IsFollowingMe = info.IsFollowingMe,
                            LastAvatarMod = info.LastAvatarMod,
                            LastCoverMod = info.LastCoverMod,
                            LastDataUpdate = info.LastDataUpdate,
                            LastFollowId = info.LastFollowId,
                            LastLoginData = info.LastLoginData,
                            LastseenStatus = info.LastseenStatus,
                            LastseenTimeText = info.LastseenTimeText,
                            LastseenUnixTime = info.LastseenUnixTime,
                            MessagePrivacy = info.MessagePrivacy,
                            NewEmail = info.NewEmail,
                            NewPhone = info.NewPhone,
                            NotificationSettings = info.NotificationSettings,
                            NotificationsSound = info.NotificationsSound,
                            OrderPostsBy = info.OrderPostsBy,
                            PaypalEmail = info.PaypalEmail,
                            PostPrivacy = info.PostPrivacy,
                            Referrer = info.Referrer,
                            ShareMyData = info.ShareMyData,
                            ShareMyLocation = info.ShareMyLocation,
                            ShowActivitiesPrivacy = info.ShowActivitiesPrivacy,
                            TwoFactor = info.TwoFactor,
                            TwoFactorVerified = info.TwoFactorVerified,
                            Url = info.Url,
                            VisitPrivacy = info.VisitPrivacy,
                            Vk = info.Vk,
                            Wallet = info.Wallet,
                            WorkingLink = info.WorkingLink,
                            Youtube = info.Youtube,
                            City = info.City,
                            Points = info.Points,
                            DailyPoints = info.DailyPoints,
                            PointDayExpire = info.PointDayExpire,
                            State = info.State,
                            Zip = info.Zip,
                            Details = string.Empty,
                            Selected = false,
                        };

                        if (info.Details.DetailsClass != null)
                            db.Details = JsonConvert.SerializeObject(info.Details.DetailsClass);
                        Connection.Insert(db);
                    }

                    UserDetails.Avatar = info.Avatar;
                    UserDetails.Cover = info.Cover;
                    UserDetails.Username = info.Username;
                    UserDetails.FullName = info.Name;
                    UserDetails.Email = info.Email;

                    ListUtils.MyProfileList = new ObservableCollection<UserDataObject>();
                    ListUtils.MyProfileList.Clear();
                    ListUtils.MyProfileList.Add(info);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Get data To My Profile Table
        public UserDataObject Get_MyProfile()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return null;
                    var item = Connection.Table<DataTables.MyProfileTb>().FirstOrDefault();
                    if (item != null)
                    {
                        UserDataObject infoObject = new UserDataObject
                        {
                            UserId = item.UserId,
                            Username = item.Username,
                            Email = item.Email,
                            FirstName = item.FirstName,
                            LastName = item.LastName,
                            Avatar = item.Avatar,
                            Cover = item.Cover,
                            BackgroundImage = item.BackgroundImage,
                            RelationshipId = item.RelationshipId,
                            Address = item.Address,
                            Working = item.Working,
                            Gender = item.Gender,
                            Facebook = item.Facebook,
                            Google = item.Google,
                            Twitter = item.Twitter,
                            Linkedin = item.Linkedin,
                            Website = item.Website,
                            Instagram = item.Instagram,
                            WebDeviceId = item.WebDeviceId,
                            Language = item.Language,
                            IpAddress = item.IpAddress,
                            PhoneNumber = item.PhoneNumber,
                            Timezone = item.Timezone,
                            Lat = item.Lat,
                            Lng = item.Lng,
                            About = item.About,
                            Birthday = item.Birthday,
                            Registered = item.Registered,
                            Lastseen = item.Lastseen,
                            LastLocationUpdate = item.LastLocationUpdate,
                            Balance = item.Balance,
                            Verified = item.Verified,
                            Status = item.Status,
                            Active = item.Active,
                            Admin = item.Admin,
                            IsPro = item.IsPro,
                            ProType = item.ProType,
                            School = item.School,
                            Name = item.Name,
                            AndroidMDeviceId = item.AndroidMDeviceId,
                            ECommented = item.ECommented,
                            AndroidNDeviceId = item.AndroidMDeviceId,
                            AvatarFull = item.AvatarFull,
                            BirthPrivacy = item.BirthPrivacy,
                            CanFollow = item.CanFollow,
                            ConfirmFollowers = item.ConfirmFollowers,
                            CountryId = item.CountryId,
                            EAccepted = item.EAccepted,
                            EFollowed = item.EFollowed,
                            EJoinedGroup = item.EJoinedGroup,
                            ELastNotif = item.ELastNotif,
                            ELiked = item.ELiked,
                            ELikedPage = item.ELikedPage,
                            EMentioned = item.EMentioned,
                            EProfileWallPost = item.EProfileWallPost,
                            ESentmeMsg = item.ESentmeMsg,
                            EShared = item.EShared,
                            EVisited = item.EVisited,
                            EWondered = item.EWondered,
                            EmailNotification = item.EmailNotification,
                            FollowPrivacy = item.FollowPrivacy,
                            FriendPrivacy = item.FriendPrivacy,
                            GenderText = item.GenderText,
                            InfoFile = item.InfoFile,
                            IosMDeviceId = item.IosMDeviceId,
                            IosNDeviceId = item.IosNDeviceId,
                            IsBlocked = item.IsBlocked,
                            IsFollowing = item.IsFollowing,
                            IsFollowingMe = item.IsFollowingMe,
                            LastAvatarMod = item.LastAvatarMod,
                            LastCoverMod = item.LastCoverMod,
                            LastDataUpdate = item.LastDataUpdate,
                            LastFollowId = item.LastFollowId,
                            LastLoginData = item.LastLoginData,
                            LastseenStatus = item.LastseenStatus,
                            LastseenTimeText = item.LastseenTimeText,
                            LastseenUnixTime = item.LastseenUnixTime,
                            MessagePrivacy = item.MessagePrivacy,
                            NewEmail = item.NewEmail,
                            NewPhone = item.NewPhone,
                            NotificationSettings = item.NotificationSettings,
                            NotificationsSound = item.NotificationsSound,
                            OrderPostsBy = item.OrderPostsBy,
                            PaypalEmail = item.PaypalEmail,
                            PostPrivacy = item.PostPrivacy,
                            Referrer = item.Referrer,
                            ShareMyData = item.ShareMyData,
                            ShareMyLocation = item.ShareMyLocation,
                            ShowActivitiesPrivacy = item.ShowActivitiesPrivacy,
                            TwoFactor = item.TwoFactor,
                            TwoFactorVerified = item.TwoFactorVerified,
                            Url = item.Url,
                            VisitPrivacy = item.VisitPrivacy,
                            Vk = item.Vk,
                            Wallet = item.Wallet,
                            WorkingLink = item.WorkingLink,
                            Youtube = item.Youtube,
                            City = item.City,
                            Points = item.Points,
                            DailyPoints = item.DailyPoints,
                            PointDayExpire = item.PointDayExpire,
                            State = item.State,
                            Zip = item.Zip,
                            Details = new DetailsUnion { DetailsClass = new Details() },
                            Selected = false,
                        };

                        if (!string.IsNullOrEmpty(item.Details))
                            infoObject.Details = new DetailsUnion
                            {
                                DetailsClass = JsonConvert.DeserializeObject<Details>(item.Details)
                            };

                        UserDetails.Avatar = item.Avatar;
                        UserDetails.Cover = item.Cover;
                        UserDetails.Username = item.Username;
                        UserDetails.FullName = item.Name;
                        UserDetails.Email = item.Email;

                        ListUtils.MyProfileList = new ObservableCollection<UserDataObject>();
                        ListUtils.MyProfileList.Clear();
                        ListUtils.MyProfileList.Add(infoObject);

                        return infoObject;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        #endregion

        #region Search Filter 

        public void InsertOrUpdate_SearchFilter(DataTables.SearchFilterTb dataFilter)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    var data = Connection.Table<DataTables.SearchFilterTb>().FirstOrDefault();
                    if (data == null)
                    {
                        Connection.Insert(dataFilter);
                    }
                    else
                    {
                        data.Gender = dataFilter.Gender;
                        data.Country = dataFilter.Country;
                        data.Status = dataFilter.Status;
                        data.Verified = dataFilter.Verified;
                        data.ProfilePicture = dataFilter.ProfilePicture;
                        data.FilterByAge = dataFilter.FilterByAge;
                        data.AgeFrom = dataFilter.AgeFrom;
                        data.AgeTo = dataFilter.AgeTo;

                        Connection.Update(data);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public DataTables.SearchFilterTb GetSearchFilterById()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var data = Connection?.Table<DataTables.SearchFilterTb>().FirstOrDefault();
                    return data;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        #endregion

        #region Near By Filter 

        public void InsertOrUpdate_NearByFilter(DataTables.NearByFilterTb dataFilter)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    var data = Connection.Table<DataTables.NearByFilterTb>().FirstOrDefault();
                    if (data == null)
                    {
                        Connection.Insert(dataFilter);
                    }
                    else
                    {
                        data.DistanceValue = dataFilter.DistanceValue;
                        data.Gender = dataFilter.Gender;
                        data.Status = dataFilter.Status;
                        data.Relationship = dataFilter.Relationship;

                        Connection.Update(data);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public DataTables.NearByFilterTb GetNearByFilterById()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var data = Connection?.Table<DataTables.NearByFilterTb>().FirstOrDefault();
                    return data;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        #endregion

        #region Last Chat Filter

        public void InsertOrUpdate_FilterLastChat(DataTables.FilterLastChatTb dataFilter)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    var data = Connection.Table<DataTables.FilterLastChatTb>().FirstOrDefault();
                    if (data == null)
                    {
                        Connection.Insert(dataFilter);
                    }
                    else
                    {
                        data.Status = dataFilter.Status;

                        Connection.Update(data);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public DataTables.FilterLastChatTb GetFilterLastChatById()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    var data = Connection?.Table<DataTables.FilterLastChatTb>().FirstOrDefault();
                    return data;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        #endregion

        #region Message

        //Insert data To Message Table
        public void Insert_Or_Replace_MessagesTable(ObservableCollection<AdapterModelsClassUser> messageList)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;

                    List<DataTables.MessageTb> listOfDatabaseForInsert = new List<DataTables.MessageTb>();

                    // get data from database
                    var resultMessage = Connection.Table<DataTables.MessageTb>().ToList();

                    foreach (var messages in messageList)
                    {
                        var maTb = Mapper.Map<DataTables.MessageTb>(messages.MesData);

                        if (messages.MesData.Product?.ProductClass != null)
                            maTb.Product = JsonConvert.SerializeObject(messages.MesData.Product?.ProductClass);

                        maTb.MessageUser = JsonConvert.SerializeObject(messages.MesData.MessageUser);
                        maTb.UserData = JsonConvert.SerializeObject(messages.MesData.UserData);

                        var dataCheck = resultMessage.FirstOrDefault(a => a.Id == messages.MesData.Id);
                        if (dataCheck != null)
                        {
                            var checkForUpdate = resultMessage.FirstOrDefault(a => a.Id == dataCheck.Id);
                            if (checkForUpdate != null)
                            {
                                checkForUpdate = maTb;
                                //listOfDatabaseForUpdate.Add(checkForUpdate);
                                Connection.Update(checkForUpdate);
                            }
                            else
                            {
                                listOfDatabaseForInsert.Add(maTb);
                            }
                        }
                        else
                        {
                            listOfDatabaseForInsert.Add(maTb);
                        }
                    }

                    Connection.BeginTransaction();

                    //Bring new  
                    if (listOfDatabaseForInsert.Count > 0)
                    {
                        Connection.InsertAll(listOfDatabaseForInsert);
                    }

                    Connection.Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Update one Messages Table
        public void Insert_Or_Update_To_one_MessagesTable(MessageDataExtra item)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;

                    var maTb = Mapper.Map<DataTables.MessageTb>(item);

                    if (item.Product?.ProductClass != null)
                        maTb.Product = JsonConvert.SerializeObject(item.Product?.ProductClass);

                    maTb.MessageUser = JsonConvert.SerializeObject(item.MessageUser);
                    maTb.UserData = JsonConvert.SerializeObject(item.UserData);

                    var data = Connection.Table<DataTables.MessageTb>().FirstOrDefault(a => a.Id == item.Id);
                    if (data != null)
                    {
                        data = maTb;
                        Connection.Update(data);
                    }
                    else
                    {
                        //Insert  one Messages Table
                        Connection.Insert(maTb);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Get data To Messages
        public string GetMessages_List(string fromId, string toId, string beforeMessageId)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return null;
                    var beforeQ = "";
                    if (beforeMessageId != "0")
                    {
                        beforeQ = "AND Id < " + beforeMessageId + " AND Id <> " + beforeMessageId + " ";
                    }

                    var query2 = Connection.Query<DataTables.MessageTb>("SELECT * FROM MessageTB WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" + toId + " and ToId=" + fromId + ")) " + beforeQ);

                    List<DataTables.MessageTb> query = query2.Where(w => w.FromId == fromId && w.ToId == toId || w.ToId == fromId && w.FromId == toId).OrderBy(q => q.Time).TakeLast(100).ToList();

                    if (query.Count > 0)
                    {
                        foreach (var item in query)
                        {
                            var check = ChatWindowActivity.GetInstance()?.MAdapter.DifferList.FirstOrDefault(a => a.MesData.Id == item.Id);
                            if (check != null)
                                continue;

                            var maTb = Mapper.Map<MessageDataExtra>(item);

                            if (!string.IsNullOrEmpty(item.Product))
                                maTb.Product = new ProductUnion()
                                {
                                    ProductClass = JsonConvert.DeserializeObject<ProductDataObject>(item.Product)
                                };

                            if (!string.IsNullOrEmpty(item.MessageUser))
                                maTb.MessageUser = JsonConvert.DeserializeObject<UserDataObject>(item.MessageUser);

                            if (!string.IsNullOrEmpty(item.UserData))
                                maTb.UserData = JsonConvert.DeserializeObject<UserDataObject>(item.UserData);

                            var type = ChatWindowActivity.GetInstance().MAdapter.GetTypeModel(maTb);
                            if (type == MessageModelType.None)
                                continue;

                            if (beforeMessageId == "0")
                            {
                                ChatWindowActivity.GetInstance()?.MAdapter.DifferList.Add(new AdapterModelsClassUser
                                {
                                    TypeView = type,
                                    Id = Long.ParseLong(item.Id),
                                    MesData = WoWonderTools.MessageFilter(toId, maTb, type)
                                });
                            }
                            else
                            {
                                ChatWindowActivity.GetInstance()?.MAdapter.DifferList.Insert(0, new AdapterModelsClassUser
                                {
                                    TypeView = type,
                                    Id = Long.ParseLong(item.Id),
                                    MesData = WoWonderTools.MessageFilter(toId, maTb, type)
                                });
                            }
                        }
                        return "1";
                    }

                    return "0";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "0";
            }
        }

        //Get data To where first Messages >> load more
        public ObservableCollection<DataTables.MessageTb> GetMessageList(string fromId, string toId, string beforeMessageId)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return null;
                    var beforeQ = "";
                    if (beforeMessageId != "0")
                    {
                        beforeQ = "AND Id < " + beforeMessageId + " AND Id <> " + beforeMessageId + " ";
                    }

                    var query2 = Connection.Query<DataTables.MessageTb>("SELECT * FROM MessageTB WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" + toId + " and ToId=" + fromId + ")) " + beforeQ);

                    List<DataTables.MessageTb> query = query2.Where(w => w.FromId == fromId && w.ToId == toId || w.ToId == fromId && w.FromId == toId).TakeLast(35).ToList();

                    query.Reverse();
                    return new ObservableCollection<DataTables.MessageTb>(query);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ObservableCollection<DataTables.MessageTb>();
            }
        }

        //Remove data To Messages Table
        public void Delete_OneMessageUser(string messageId)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    var user = Connection.Table<DataTables.MessageTb>().FirstOrDefault(c => c.Id.ToString() == messageId);
                    if (user != null)
                    {
                        Connection.Delete(user);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void DeleteAllMessagesUser(string fromId, string toId)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    var query = Connection.Query<DataTables.MessageTb>("Delete FROM MessageTB WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" + toId + " and ToId=" + fromId + "))");
                    Console.WriteLine(query);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ClearAll_Messages()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    Connection.DeleteAll<DataTables.MessageTb>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Call_User

        public void Insert_CallUser(Classes.CallUser user)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    var result = Connection.Table<DataTables.CallUserTb>().ToList();
                    var check = result.FirstOrDefault(a => a.Time == user.Time);
                    if (check == null && !string.IsNullOrEmpty(user.Id))
                    {
                        DataTables.CallUserTb cv = new DataTables.CallUserTb
                        {
                            CallId = user.Id,
                            UserId = user.UserId,
                            Avatar = user.Avatar,
                            Name = user.Name,
                            AccessToken = user.AccessToken,
                            AccessToken2 = user.AccessToken2,
                            FromId = user.FromId,
                            Active = user.Active,
                            Time = user.Time,
                            Status = user.Status,
                            RoomName = user.RoomName,
                            Type = user.Type,
                            TypeIcon = user.TypeIcon,
                            TypeColor = user.TypeColor
                        };
                        Connection.Insert(cv);
                    }
                    else
                    {
                        check = new DataTables.CallUserTb
                        {
                            CallId = user.Id,
                            UserId = user.UserId,
                            Avatar = user.Avatar,
                            Name = user.Name,
                            AccessToken = user.AccessToken,
                            AccessToken2 = user.AccessToken2,
                            FromId = user.FromId,
                            Active = user.Active,
                            Time = user.Time,
                            Status = user.Status,
                            RoomName = user.RoomName,
                            Type = user.Type,
                            TypeIcon = user.TypeIcon,
                            TypeColor = user.TypeColor
                        };

                        Connection.Update(check);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public ObservableCollection<Classes.CallUser> Get_CallUserList()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return new ObservableCollection<Classes.CallUser>();
                    var list = new ObservableCollection<Classes.CallUser>();
                    var result = Connection.Table<DataTables.CallUserTb>().ToList();
                    if (result.Count <= 0) return new ObservableCollection<Classes.CallUser>();
                    foreach (var cv in result.Select(item => new Classes.CallUser
                    {
                        Id = item.CallId,
                        UserId = item.UserId,
                        Avatar = item.Avatar,
                        Name = item.Name,
                        AccessToken = item.AccessToken,
                        AccessToken2 = item.AccessToken2,
                        FromId = item.FromId,
                        Active = item.Active,
                        Time = item.Time,
                        Status = item.Status,
                        RoomName = item.RoomName,
                        Type = item.Type,
                        TypeIcon = item.TypeIcon,
                        TypeColor = item.TypeColor
                    }))
                    {
                        list.Add(cv);
                    }

                    return new ObservableCollection<Classes.CallUser>(list.OrderBy(a => a.Time).ToList());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ObservableCollection<Classes.CallUser>();
            }
        }

        public void Clear_CallUser_List()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    Connection.DeleteAll<DataTables.CallUserTb>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Last User Chat

        //Insert Or Update data To Users Table
        public void Insert_Or_Update_LastUsersChat(Context context, ObservableCollection<ChatObject> chatList, bool showFloating = false)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;

                    var result = Connection.Table<DataTables.LastUsersTb>().ToList();
                    List<DataTables.LastUsersTb> list = new List<DataTables.LastUsersTb>();
                    foreach (var item in chatList)
                    {
                        DataTables.LastUsersTb db = new DataTables.LastUsersTb
                        {
                            Id = item.Id,
                            UserId = item.UserId,
                            Username = item.Username,
                            Email = item.Email,
                            FirstName = item.FirstName,
                            LastName = item.LastName,
                            Avatar = item.Avatar,
                            Cover = item.Cover,
                            BackgroundImage = item.BackgroundImage,
                            RelationshipId = item.RelationshipId,
                            Address = item.Address,
                            Working = item.Working,
                            Gender = item.Gender,
                            Facebook = item.Facebook,
                            Google = item.Google,
                            Twitter = item.Twitter,
                            Linkedin = item.Linkedin,
                            Website = item.Website,
                            Instagram = item.Instagram,
                            WebDeviceId = item.WebDeviceId,
                            Language = item.Language,
                            IpAddress = item.IpAddress,
                            PhoneNumber = item.PhoneNumber,
                            Timezone = item.Timezone,
                            Lat = item.Lat,
                            Lng = item.Lng,
                            About = item.About,
                            Birthday = item.Birthday,
                            Registered = item.Registered,
                            Lastseen = item.Lastseen,
                            LastLocationUpdate = item.LastLocationUpdate,
                            Balance = item.Balance,
                            Verified = item.Verified,
                            Status = item.Status,
                            Active = item.Active,
                            Admin = item.Admin,
                            IsPro = item.IsPro,
                            ProType = item.ProType,
                            School = item.School,
                            Name = item.Name,
                            AndroidMDeviceId = item.AndroidMDeviceId,
                            ECommented = item.ECommented,
                            AndroidNDeviceId = item.AndroidNDeviceId,
                            BirthPrivacy = item.BirthPrivacy,
                            ConfirmFollowers = item.ConfirmFollowers,
                            CountryId = item.CountryId,
                            EAccepted = item.EAccepted,
                            EFollowed = item.EFollowed,
                            EJoinedGroup = item.EJoinedGroup,
                            ELastNotif = item.ELastNotif,
                            ELiked = item.ELiked,
                            ELikedPage = item.ELikedPage,
                            EMentioned = item.EMentioned,
                            EProfileWallPost = item.EProfileWallPost,
                            ESentmeMsg = item.ESentmeMsg,
                            EShared = item.EShared,
                            EVisited = item.EVisited,
                            EWondered = item.EWondered,
                            EmailNotification = item.EmailNotification,
                            FollowPrivacy = item.FollowPrivacy,
                            FriendPrivacy = item.FriendPrivacy,
                            InfoFile = item.InfoFile,
                            IosMDeviceId = item.IosMDeviceId,
                            IosNDeviceId = item.IosNDeviceId,
                            LastAvatarMod = item.LastAvatarMod,
                            LastCoverMod = item.LastCoverMod,
                            LastDataUpdate = item.LastDataUpdate,
                            LastFollowId = item.LastFollowId,
                            LastseenStatus = item.LastseenStatus,
                            LastseenUnixTime = item.LastseenUnixTime,
                            MessagePrivacy = item.MessagePrivacy,
                            NewEmail = item.NewEmail,
                            NewPhone = item.NewPhone,
                            NotificationSettings = item.NotificationSettings,
                            NotificationsSound = item.NotificationsSound,
                            OrderPostsBy = item.OrderPostsBy,
                            PaypalEmail = item.PaypalEmail,
                            PostPrivacy = item.PostPrivacy,
                            Referrer = item.Referrer,
                            ShareMyData = item.ShareMyData,
                            ShareMyLocation = item.ShareMyLocation,
                            ShowActivitiesPrivacy = item.ShowActivitiesPrivacy,
                            TwoFactor = item.TwoFactor,
                            TwoFactorVerified = item.TwoFactorVerified,
                            Url = item.Url,
                            VisitPrivacy = item.VisitPrivacy,
                            Vk = item.Vk,
                            Wallet = item.Wallet,
                            WorkingLink = item.WorkingLink,
                            Youtube = item.Youtube,
                            City = item.City,
                            Points = item.Points,
                            DailyPoints = item.DailyPoints,
                            PointDayExpire = item.PointDayExpire,
                            State = item.State,
                            Zip = item.Zip,
                            AvatarOrg = item.AvatarOrg,
                            BackgroundImageStatus = item.BackgroundImageStatus,
                            Boosted = item.Boosted,
                            CallActionType = item.CallActionType,
                            CallActionTypeUrl = item.CallActionTypeUrl,
                            Category = item.Category,
                            ChatTime = item.ChatTime,
                            ChatType = item.ChatType,
                            Company = item.Company,
                            CoverFull = item.CoverFull,
                            CoverOrg = item.CoverOrg,
                            CssFile = item.CssFile,
                            EmailCode = item.EmailCode,
                            GroupId = item.GroupId,
                            Instgram = item.Instgram,
                            Joined = item.Joined,
                            LastEmailSent = item.LastEmailSent,
                            PageCategory = item.PageCategory,
                            PageDescription = item.PageDescription,
                            PageId = item.PageId,
                            PageName = item.PageName,
                            PageTitle = item.PageTitle,
                            Password = item.Password,
                            Phone = item.Phone,
                            GroupName = item.GroupName,
                            ProTime = item.ProTime,
                            Rating = item.Rating,
                            RefUserId = item.RefUserId,
                            SchoolCompleted = item.SchoolCompleted,
                            Showlastseen = item.Showlastseen,
                            SidebarData = item.SidebarData,
                            SmsCode = item.SmsCode,
                            SocialLogin = item.SocialLogin,
                            Src = item.Src,
                            StartUp = item.StartUp,
                            StartupFollow = item.StartupFollow,
                            StartupImage = item.StartupImage,
                            StartUpInfo = item.StartUpInfo,
                            Time = item.Time,
                            Type = item.Type,
                            WeatherUnit = item.WeatherUnit,
                            MessageCount = item.MessageCount,
                            IsPageOnwer = (item.IsPageOnwer != null && item.IsPageOnwer.Value).ToString(),
                            UserData = JsonConvert.SerializeObject(item.UserData),
                        };

                        if (item.LastMessage.LastMessageClass != null)
                            db.LastMessage = JsonConvert.SerializeObject(item.LastMessage.LastMessageClass);

                        db.Parts = JsonConvert.SerializeObject(item.Parts);

                        //db.Details = JsonConvert.SerializeObject(info.Details?.DetailsClass);
                        list.Add(db);

                        var update = result.FirstOrDefault(a => a.UserId == item.UserId);
                        if (update != null)
                        {
                            update = db;

                            if (item.LastMessage.LastMessageClass != null)
                                update.LastMessage = JsonConvert.SerializeObject(item.LastMessage.LastMessageClass);

                            update.Parts = JsonConvert.SerializeObject(item.Parts);

                            var chk = IdMesgList.FirstOrDefault(a => a == item.LastMessage.LastMessageClass.Id);

                            if (showFloating && item.LastMessage.LastMessageClass != null && item.LastMessage.LastMessageClass.Seen == "0" && chk == null && item.LastMessage.LastMessageClass.FromId != UserDetails.UserId && Methods.AppLifecycleObserver.AppState == Methods.AppLifecycleObserver.AppLifeState.Background)
                            {
                                var floating = new FloatingObject()
                                {
                                    ChatType = item.ChatType,
                                    UserId = item.UserId,
                                    PageId = item.PageId,
                                    GroupId = item.GroupId,
                                    Avatar = item.Avatar,
                                    ChatColor = "",
                                    LastSeen = item.Lastseen,
                                    LastSeenUnixTime = item.LastseenUnixTime,
                                    Name = item.Name,
                                    MessageCount = item.LastMessage.LastMessageClass.MessageCount ?? "1"
                                };

                                switch (item.ChatType)
                                {
                                    case "user":
                                        floating.Name = item.Name;
                                        break;
                                    case "page":
                                        floating.Name = item.PageName;
                                        break;
                                    case "group":
                                        floating.Name = item.GroupName;
                                        break;
                                }

                                if (Build.VERSION.SdkInt <= BuildVersionCodes.LollipopMr1 || InitFloating.CanDrawOverlays(context))
                                {
                                    IdMesgList.Add(item.LastMessage.LastMessageClass.Id);

                                    if (InitFloating.FloatingObject == null && ChatHeadService.RunService)
                                        return;

                                    // launch service 
                                    Intent intent = new Intent(context, typeof(ChatHeadService));
                                    intent.PutExtra(ChatHeadService.ExtraCutoutSafeArea, FloatingViewManager.FindCutoutSafeArea(new Activity()));
                                    intent.PutExtra("UserData", JsonConvert.SerializeObject(floating));
                                    ContextCompat.StartForegroundService(context, intent);
                                }
                            }

                            //update.Details = JsonConvert.SerializeObject(info.Details?.DetailsClass);

                            Connection.Update(update);
                        }
                    }

                    if (list.Count <= 0) return;

                    Connection.BeginTransaction();
                    //Bring new  
                    var newItemList = list.Where(c => !result.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                    if (newItemList.Count > 0)
                        Connection.InsertAll(newItemList);

                    result = Connection.Table<DataTables.LastUsersTb>().ToList();
                    var deleteItemList = result.Where(c => !list.Select(fc => fc.UserId).Contains(c.UserId)).ToList();
                    if (deleteItemList.Count > 0)
                        foreach (var delete in deleteItemList)
                            Connection.Delete(delete);

                    Connection.Commit();
                }

                ListUtils.UserList = chatList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Get data To Users Table
        public ObservableCollection<ChatObject> Get_LastUsersChat_List()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return null;
                    var select = Connection.Table<DataTables.LastUsersTb>().ToList();
                    if (select.Count > 0)
                    {
                        var list = new List<ChatObject>();

                        foreach (DataTables.LastUsersTb item in select)
                        {
                            ChatObject db = new ChatObject()
                            {
                                Id = item.Id,
                                UserId = item.UserId,
                                Username = item.Username,
                                Email = item.Email,
                                FirstName = item.FirstName,
                                LastName = item.LastName,
                                Avatar = item.Avatar,
                                Cover = item.Cover,
                                BackgroundImage = item.BackgroundImage,
                                RelationshipId = item.RelationshipId,
                                Address = item.Address,
                                Working = item.Working,
                                Gender = item.Gender,
                                Facebook = item.Facebook,
                                Google = item.Google,
                                Twitter = item.Twitter,
                                Linkedin = item.Linkedin,
                                Website = item.Website,
                                Instagram = item.Instagram,
                                WebDeviceId = item.WebDeviceId,
                                Language = item.Language,
                                IpAddress = item.IpAddress,
                                PhoneNumber = item.PhoneNumber,
                                Timezone = item.Timezone,
                                Lat = item.Lat,
                                Lng = item.Lng,
                                About = item.About,
                                Birthday = item.Birthday,
                                Registered = item.Registered,
                                Lastseen = item.Lastseen,
                                LastLocationUpdate = item.LastLocationUpdate,
                                Balance = item.Balance,
                                Verified = item.Verified,
                                Status = item.Status,
                                Active = item.Active,
                                Admin = item.Admin,
                                IsPro = item.IsPro,
                                ProType = item.ProType,
                                School = item.School,
                                Name = item.Name,
                                AndroidMDeviceId = item.AndroidMDeviceId,
                                ECommented = item.ECommented,
                                AndroidNDeviceId = item.AndroidNDeviceId,
                                BirthPrivacy = item.BirthPrivacy,
                                ConfirmFollowers = item.ConfirmFollowers,
                                CountryId = item.CountryId,
                                EAccepted = item.EAccepted,
                                EFollowed = item.EFollowed,
                                EJoinedGroup = item.EJoinedGroup,
                                ELastNotif = item.ELastNotif,
                                ELiked = item.ELiked,
                                ELikedPage = item.ELikedPage,
                                EMentioned = item.EMentioned,
                                EProfileWallPost = item.EProfileWallPost,
                                ESentmeMsg = item.ESentmeMsg,
                                EShared = item.EShared,
                                EVisited = item.EVisited,
                                EWondered = item.EWondered,
                                EmailNotification = item.EmailNotification,
                                FollowPrivacy = item.FollowPrivacy,
                                FriendPrivacy = item.FriendPrivacy,
                                InfoFile = item.InfoFile,
                                IosMDeviceId = item.IosMDeviceId,
                                IosNDeviceId = item.IosNDeviceId,
                                LastAvatarMod = item.LastAvatarMod,
                                LastCoverMod = item.LastCoverMod,
                                LastDataUpdate = item.LastDataUpdate,
                                LastFollowId = item.LastFollowId,
                                LastseenStatus = item.LastseenStatus,
                                LastseenUnixTime = item.LastseenUnixTime,
                                MessagePrivacy = item.MessagePrivacy,
                                NewEmail = item.NewEmail,
                                NewPhone = item.NewPhone,
                                NotificationSettings = item.NotificationSettings,
                                NotificationsSound = item.NotificationsSound,
                                OrderPostsBy = item.OrderPostsBy,
                                PaypalEmail = item.PaypalEmail,
                                PostPrivacy = item.PostPrivacy,
                                Referrer = item.Referrer,
                                ShareMyData = item.ShareMyData,
                                ShareMyLocation = item.ShareMyLocation,
                                ShowActivitiesPrivacy = item.ShowActivitiesPrivacy,
                                TwoFactor = item.TwoFactor,
                                TwoFactorVerified = item.TwoFactorVerified,
                                Url = item.Url,
                                VisitPrivacy = item.VisitPrivacy,
                                Vk = item.Vk,
                                Wallet = item.Wallet,
                                WorkingLink = item.WorkingLink,
                                Youtube = item.Youtube,
                                City = item.City,
                                Points = item.Points,
                                DailyPoints = item.DailyPoints,
                                PointDayExpire = item.PointDayExpire,
                                State = item.State,
                                Zip = item.Zip,
                                AvatarOrg = item.AvatarOrg,
                                BackgroundImageStatus = item.BackgroundImageStatus,
                                Boosted = item.Boosted,
                                CallActionType = item.CallActionType,
                                CallActionTypeUrl = item.CallActionTypeUrl,
                                Category = item.Category,
                                ChatTime = item.ChatTime,
                                ChatType = item.ChatType,
                                Company = item.Company,
                                CoverFull = item.CoverFull,
                                CoverOrg = item.CoverOrg,
                                CssFile = item.CssFile,
                                EmailCode = item.EmailCode,
                                GroupId = item.GroupId,
                                Instgram = item.Instgram,
                                Joined = item.Joined,
                                LastEmailSent = item.LastEmailSent,
                                PageCategory = item.PageCategory,
                                PageDescription = item.PageDescription,
                                PageId = item.PageId,
                                PageName = item.PageName,
                                PageTitle = item.PageTitle,
                                Password = item.Password,
                                Phone = item.Phone,
                                GroupName = item.GroupName,
                                ProTime = item.ProTime,
                                Rating = item.Rating,
                                RefUserId = item.RefUserId,
                                SchoolCompleted = item.SchoolCompleted,
                                Showlastseen = item.Showlastseen,
                                SidebarData = item.SidebarData,
                                SmsCode = item.SmsCode,
                                SocialLogin = item.SocialLogin,
                                Src = item.Src,
                                StartUp = item.StartUp,
                                StartupFollow = item.StartupFollow,
                                StartupImage = item.StartupImage,
                                StartUpInfo = item.StartUpInfo,
                                Time = item.Time,
                                Type = item.Type,
                                WeatherUnit = item.WeatherUnit,
                                MessageCount = item.MessageCount,
                                LastMessage = new LastMessageUnion()
                                {
                                    LastMessageClass = new MessageData()
                                },
                                Owner = Convert.ToBoolean(item.Owner),
                                Parts = new List<UserDataObject>(),
                                UserData = new UserDataObject(),
                            };

                            if (!string.IsNullOrEmpty(item.IsPageOnwer))
                                db.IsPageOnwer = Convert.ToBoolean(item.IsPageOnwer);

                            if (!string.IsNullOrEmpty(item.UserData))
                                db.UserData = JsonConvert.DeserializeObject<UserDataObject>(item.UserData);

                            if (!string.IsNullOrEmpty(item.LastMessage))
                            {
                                db.LastMessage = new LastMessageUnion()
                                {
                                    LastMessageClass = JsonConvert.DeserializeObject<MessageDataExtra>(item.LastMessage),
                                };
                            }

                            if (!string.IsNullOrEmpty(item.Parts))
                                db.Parts = JsonConvert.DeserializeObject<List<UserDataObject>>(item.Parts);

                            //if (!string.IsNullOrEmpty(item.Details))
                            //    db.Details = new DetailsUnion
                            //    {
                            //        DetailsClass = JsonConvert.DeserializeObject<Details>(item.Details)
                            //    };

                            list.Add(db);
                        }

                        return new ObservableCollection<ChatObject>(list);
                    }
                    else
                    {
                        return new ObservableCollection<ChatObject>();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ObservableCollection<ChatObject>();
            }
        }

        //Remove data from Users Table
        public void Delete_LastUsersChat(string userId)
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;

                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                    {
                        var user = Connection.Table<DataTables.LastUsersTb>().FirstOrDefault(c => c.UserId == userId);
                        if (user != null)
                        {
                            Connection.Delete(user);
                        }
                    }
                    else
                    {
                        var user = Connection.Table<DataTables.LastUsersChatTb>().FirstOrDefault(c => JsonConvert.DeserializeObject<UserDataObject>(c.UserData).UserId == userId);
                        if (user != null)
                        {
                            Connection.Delete(user);
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Remove All data To Users Table
        public void ClearAll_LastUsersChat()
        {
            try
            {
                using (Connection = OpenConnection())
                {
                    if (Connection == null) return;
                    if (AppSettings.LastChatSystem == SystemApiGetLastChat.New)
                    {
                        Connection.DeleteAll<DataTables.LastUsersTb>();
                    }
                    else
                    {
                        Connection.DeleteAll<DataTables.LastUsersChatTb>();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Last_User_Chat >> Old

        private static List<string> IdMesgList = new List<string>();
        //Insert Or Update data To Users Table
        public void Insert_Or_Update_LastUsersChat(Context context, ObservableCollection<GetUsersListObject.User> lastUsersList, bool showFloating = false)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {

                    var result = Connection.Table<DataTables.LastUsersChatTb>().ToList();

                    List<DataTables.LastUsersChatTb> list = new List<DataTables.LastUsersChatTb>();
                    foreach (var user in lastUsersList)
                    {
                        var item = new DataTables.LastUsersChatTb
                        {
                            OrderId = user.ChatTime,
                            UserData = JsonConvert.SerializeObject(user),
                            LastMessageData = JsonConvert.SerializeObject(user.LastMessage)
                        };
                        list.Add(item);

                        var update = result.FirstOrDefault(a => a.OrderId == user.ChatTime);
                        if (update != null)
                        {
                            update = item;
                            Connection.Update(update);

                            var chk = IdMesgList.FirstOrDefault(a => a == user.LastMessage.Id);
                            if (showFloating && user.LastMessage != null && user.LastMessage.Seen == "0" && chk == null && user.LastMessage.FromId != UserDetails.UserId && Methods.AppLifecycleObserver.AppState == Methods.AppLifecycleObserver.AppLifeState.Background)
                            {
                                var floating = new FloatingObject()
                                {
                                    ChatType = "user",
                                    UserId = user.UserId,
                                    PageId = "",
                                    GroupId = "",
                                    Avatar = user.Avatar,
                                    ChatColor = user.ChatColor,
                                    LastSeen = user.Lastseen,
                                    LastSeenUnixTime = user.LastseenUnixTime,
                                    Name = user.Name,
                                    MessageCount = "1"
                                };

                                if (Build.VERSION.SdkInt <= BuildVersionCodes.LollipopMr1 || InitFloating.CanDrawOverlays(context))
                                {
                                    IdMesgList.Add(user.LastMessage.Id);

                                    if (InitFloating.FloatingObject == null && ChatHeadService.RunService)
                                        return;

                                    // launch service 
                                    Intent intent = new Intent(context, typeof(ChatHeadService));
                                    intent.PutExtra(ChatHeadService.ExtraCutoutSafeArea, FloatingViewManager.FindCutoutSafeArea(new Activity()));
                                    intent.PutExtra("UserData", JsonConvert.SerializeObject(floating));
                                    ContextCompat.StartForegroundService(context, intent);
                                }
                            }
                        }
                    }

                    if (list.Count <= 0) return;

                    Connection.BeginTransaction();
                    //Bring new  
                    var newItemList = list.Where(c => !result.Select(fc => fc.UserData).Contains(c.UserData)).ToList();
                    if (newItemList.Count > 0)
                        Connection.InsertAll(newItemList);

                    result = Connection.Table<DataTables.LastUsersChatTb>().ToList();
                    var deleteItemList = result.Where(c => !list.Select(fc => fc.UserData).Contains(c.UserData)).ToList();
                    if (deleteItemList.Count > 0)
                        foreach (var delete in deleteItemList)
                            Connection.Delete(delete);

                    Connection.Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Get data To Last Users Chat
        public ObservableCollection<GetUsersListObject.User> GetLastUsersChatList()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var result = Connection.Table<DataTables.LastUsersChatTb>().OrderByDescending(a => a.OrderId).ToList();
                    if (result.Count <= 0) return new ObservableCollection<GetUsersListObject.User>();
                    var list = (from user in result
                                let userData = JsonConvert.DeserializeObject<GetUsersListObject.User>(user.UserData)
                                let lastMessageData = JsonConvert.DeserializeObject<GetUsersListObject.LastMessage>(user.LastMessageData)
                                where userData != null && lastMessageData != null
                                select new GetUsersListObject.User
                                {
                                    UserId = userData.UserId,
                                    Username = userData.Username,
                                    Avatar = userData.Avatar,
                                    Cover = userData.Cover,
                                    LastseenTimeText = userData.LastseenTimeText,
                                    Lastseen = userData.Lastseen,
                                    Url = userData.Url,
                                    Name = userData.Name,
                                    LastseenUnixTime = userData.LastseenUnixTime,
                                    ChatColor = userData.ChatColor,
                                    Verified = userData.Verified,
                                    LastMessage = lastMessageData,
                                    OldAvatar = userData.OldAvatar,
                                    OldCover = userData.OldCover,
                                }).ToList();

                    return new ObservableCollection<GetUsersListObject.User>(list);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ObservableCollection<GetUsersListObject.User>();
            }
        }

        #endregion
    }
}