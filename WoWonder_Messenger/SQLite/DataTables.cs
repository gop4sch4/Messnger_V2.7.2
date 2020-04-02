using SQLite;
using WoWonder.Helpers.Model;
using WoWonderClient.Classes.Global;

namespace WoWonder.SQLite
{
    public class DataTables
    {
        [Table("LoginTb")]
        public class LoginTb
        {
            [PrimaryKey, AutoIncrement]
            public int AutoIdLogin { get; set; }

            public string UserId { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string AccessToken { get; set; }
            public string Cookie { get; set; }
            public string Email { get; set; }
            public string Status { get; set; }
            public string Lang { get; set; }
            public string DeviceId { get; set; }
        }

        [Table("SettingsTb")]
        public class SettingsTb : GetSiteSettingsObject.Config
        {
            [PrimaryKey, AutoIncrement]
            public int AutoIdSettings { get; set; }

            public new string CurrencyArray { get; set; }
            public new string CurrencySymbolArray { get; set; }
            public new string PageCategories { get; set; }
            public new string GroupCategories { get; set; }
            public new string BlogCategories { get; set; }
            public new string ProductsCategories { get; set; }
            public new string JobCategories { get; set; }
            public new string Genders { get; set; }
            public new string Family { get; set; }
            public new string MovieCategory { get; set; }
            public new string PostColors { get; set; }
            public new string PostReactionsTypes { get; set; }
            public new string Fields { get; set; }
        }


        [Table("MyContactsTb")]
        public class MyContactsTb : UserDataObject
        {
            [PrimaryKey, AutoIncrement]
            public int AutoIdMyFollowing { get; set; }

            public new string Details { get; set; }
        }

        [Table("MyFollowersTb")]
        public class MyFollowersTb : UserDataObject
        {
            [PrimaryKey, AutoIncrement]
            public int AutoIdMyFollowers { get; set; }

            public new string Details { get; set; }
        }

        [Table("MyProfileTb")]
        public class MyProfileTb : UserDataObject
        {
            [PrimaryKey, AutoIncrement]
            public int AutoIdMyProfile { get; set; }

            public new string Details { get; set; }
        }

        [Table("SearchFilterTb")]
        public class SearchFilterTb
        {
            [PrimaryKey, AutoIncrement]
            public int AutoIdSearchFilter { get; set; }

            public string Gender { get; set; }
            public string Country { get; set; }
            public string Status { get; set; }
            public string Verified { get; set; }
            public string ProfilePicture { get; set; }
            public string FilterByAge { get; set; }
            public string AgeFrom { get; set; }
            public string AgeTo { get; set; }
        }

        [Table("NearByFilterTb")]
        public class NearByFilterTb
        {
            [PrimaryKey, AutoIncrement]
            public int AutoIdNearByFilter { get; set; }

            public int DistanceValue { get; set; }
            public string Gender { get; set; }
            public int Status { get; set; }
            public string Relationship { get; set; }
        }

        [Table("FilterLastChatTb")]
        public class FilterLastChatTb
        {
            [PrimaryKey, AutoIncrement]
            public int AutoIdLastChatFilter { get; set; }

            public string Status { get; set; }
            public string Type { get; set; }
        }

        [Table("LastUsersTb")]
        public class LastUsersTb
        {
            [PrimaryKey, AutoIncrement]
            public int AutoIdLastUsers { get; set; }

            public string GroupId { get; set; }
            public string UserId { get; set; }
            public string GroupName { get; set; }
            public string Avatar { get; set; }
            public string Time { get; set; }
            public string UserData { get; set; }
            public string Owner { get; set; }
            public string LastMessage { get; set; }
            public string Parts { get; set; }
            public string ChatTime { get; set; }
            public string ChatType { get; set; }
            public string PageId { get; set; }
            public string PageName { get; set; }
            public string PageTitle { get; set; }
            public string PageDescription { get; set; }
            public string Cover { get; set; }
            public string PageCategory { get; set; }
            public string Website { get; set; }
            public string Facebook { get; set; }
            public string Google { get; set; }
            public string Vk { get; set; }
            public string Twitter { get; set; }
            public string Linkedin { get; set; }
            public string Company { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string CallActionType { get; set; }
            public string CallActionTypeUrl { get; set; }
            public string BackgroundImage { get; set; }
            public string BackgroundImageStatus { get; set; }
            public string Instgram { get; set; }
            public string Youtube { get; set; }
            public string Verified { get; set; }
            public string Active { get; set; }
            public string Registered { get; set; }
            public string Boosted { get; set; }
            public string About { get; set; }
            public string Id { get; set; }
            public string Type { get; set; }
            public string Url { get; set; }
            public string Name { get; set; }
            public string Rating { get; set; }
            public string Category { get; set; }
            public string IsPageOnwer { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string RelationshipId { get; set; }
            public string Working { get; set; }
            public string WorkingLink { get; set; }
            public string School { get; set; }
            public string Gender { get; set; }
            public string Birthday { get; set; }
            public string CountryId { get; set; }
            public string Instagram { get; set; }
            public string Language { get; set; }
            public string EmailCode { get; set; }
            public string Src { get; set; }
            public string IpAddress { get; set; }
            public string FollowPrivacy { get; set; }
            public string FriendPrivacy { get; set; }
            public string PostPrivacy { get; set; }
            public string MessagePrivacy { get; set; }
            public string ConfirmFollowers { get; set; }
            public string ShowActivitiesPrivacy { get; set; }
            public string BirthPrivacy { get; set; }
            public string VisitPrivacy { get; set; }
            public string Lastseen { get; set; }
            public string Showlastseen { get; set; }
            public string EmailNotification { get; set; }
            public string ELiked { get; set; }
            public string EWondered { get; set; }
            public string EShared { get; set; }
            public string EFollowed { get; set; }
            public string ECommented { get; set; }
            public string EVisited { get; set; }
            public string ELikedPage { get; set; }
            public string EMentioned { get; set; }
            public string EJoinedGroup { get; set; }
            public string EAccepted { get; set; }
            public string EProfileWallPost { get; set; }
            public string ESentmeMsg { get; set; }
            public string ELastNotif { get; set; }
            public string NotificationSettings { get; set; }
            public string Status { get; set; }
            public string Admin { get; set; }
            public string StartUp { get; set; }
            public string StartUpInfo { get; set; }
            public string StartupFollow { get; set; }
            public string StartupImage { get; set; }
            public string LastEmailSent { get; set; }
            public string PhoneNumber { get; set; }
            public string SmsCode { get; set; }
            public string IsPro { get; set; }
            public string ProTime { get; set; }
            public string ProType { get; set; }
            public string Joined { get; set; }
            public string CssFile { get; set; }
            public string Timezone { get; set; }
            public string Referrer { get; set; }
            public string RefUserId { get; set; }
            public string Balance { get; set; }
            public string PaypalEmail { get; set; }
            public string NotificationsSound { get; set; }
            public string OrderPostsBy { get; set; }
            public string SocialLogin { get; set; }
            public string AndroidMDeviceId { get; set; }
            public string IosMDeviceId { get; set; }
            public string AndroidNDeviceId { get; set; }
            public string IosNDeviceId { get; set; }
            public string WebDeviceId { get; set; }
            public string Wallet { get; set; }
            public string Lat { get; set; }
            public string Lng { get; set; }
            public string LastLocationUpdate { get; set; }
            public string ShareMyLocation { get; set; }
            public string LastDataUpdate { get; set; }
            public string Details { get; set; }
            public string SidebarData { get; set; }
            public string LastAvatarMod { get; set; }
            public string LastCoverMod { get; set; }
            public string Points { get; set; }
            public string DailyPoints { get; set; }
            public string PointDayExpire { get; set; }
            public string LastFollowId { get; set; }
            public string ShareMyData { get; set; }
            public string LastLoginData { get; set; }
            public string TwoFactor { get; set; }
            public string NewEmail { get; set; }
            public string TwoFactorVerified { get; set; }
            public string NewPhone { get; set; }
            public string InfoFile { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Zip { get; set; }
            public string SchoolCompleted { get; set; }
            public string WeatherUnit { get; set; }
            public string AvatarOrg { get; set; }
            public string CoverOrg { get; set; }
            public string CoverFull { get; set; }
            public string LastseenUnixTime { get; set; }
            public string LastseenStatus { get; set; }
            public string MessageCount { get; set; }
        }

        [Table("LastUsersChatTb")]
        public class LastUsersChatTb
        {
            [PrimaryKey, AutoIncrement]
            public int AutoIdLastUsers { get; set; }
            public string OrderId { get; set; }
            public string UserData { get; set; } //UserDataObject
            public string LastMessageData { get; set; }  //SendMessageObject.MessageData
        }


        [Table("MessageTb")]
        public class MessageTb : MessageDataExtra
        {
            [PrimaryKey, AutoIncrement]
            public int AutoIdMessage { get; set; }

            public new string Product { get; set; }
            public new string MessageUser { get; set; }
            public new string UserData { get; set; }

            //not Important
            public new string MediaPlayer { get; set; }
            public new string MediaTimer { get; set; }
            public new string SoundViewHolder { get; set; }
            public new string MusicBarViewHolder { get; set; }
        }

        [Table("CallUserTb")]
        public class CallUserTb
        {
            [PrimaryKey, AutoIncrement] public int IdCallUser { get; set; }

            public string VideoCall { get; set; }

            public string UserId { get; set; }
            public string Avatar { get; set; }
            public string Name { get; set; }

            //Data
            public string CallId { get; set; }
            public string AccessToken { get; set; }
            public string AccessToken2 { get; set; }
            public string FromId { get; set; }
            public string ToId { get; set; }
            public string Active { get; set; }
            public string Called { get; set; }
            public string Time { get; set; }
            public string Declined { get; set; }
            public string Url { get; set; }
            public string Status { get; set; }
            public string RoomName { get; set; }
            public string Type { get; set; }

            //Style       
            public string TypeIcon { get; set; }
            public string TypeColor { get; set; }
        }

    }
}
