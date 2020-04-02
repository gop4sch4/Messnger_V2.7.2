//Use DoughouzChecker last version 3.0 to 
//build your own certifcate 
//For Full Documention 
//https://paper.dropbox.com/doc/WoWonder-Messenger-2.0--ARqO4OoLf_KXGWT63gNm0pOuAQ-M6qrJYGQ0C0NZlhZ3PqI7
//CopyRight DoughouzLight
//For the accuracy of the icon and logo, please use this website " http://nsimage.brosteins.com " and add images according to size in folders " mipmap " 

using WoWonder.Helpers.Model;

namespace WoWonder
{
    public static class AppSettings
    {
        public static string TripleDesAppServiceProvider = "NFYnA2qriwLLUe74dlNM9zzLCu7lwrZ0Fs6fvwEq0L+KOIqX/6CCDAPEbi1cN73KUCuYmTUKp9jfRPR/hDZ+cSd2X6rLYWl1w8SCgWlh1KUNL6zPRdBhKlut94eDswlSCynzfU51v2pVqD31oogh07GyBGlO4hYEnECwiQS5TM8bUdGFWAlv0aa3b4Ua4VgduC1kTmk9teQeuYuuu70uKLNqrCeQjNQU1HFr3sVX7C+lZVejNMbhicZzMB5PGMkM1kIfX2uQ2UZdkXR47MgccrD4RVkiXS/aPBUm3K1uPYG5VRVRJFce83Px4j0MtIGH/qKHS488Dl8yZ8GuxmJCKQDkfbUH/SJCXmkJFvUGhU1ZqvpnJi0FfxSULVNSrE+g";

        //Main Settings >>>>>
        //*********************************************************

        public static string Version = "2.7";
        public static string ApplicationName = "Badami Messenger";

        // Friend system = 0 , follow system = 1
        public static int ConnectivitySystem = 1;

        public static SystemApiGetLastChat LastChatSystem = SystemApiGetLastChat.Old; //#New

        //Main Colors >>
        //*********************************************************
        public static string MainColor = "#a84849";
        public static string StoryReadColor = "#808080";

        //Language Settings >> http://www.lingoes.net/en/translator/langcode.htm
        //*********************************************************
        public static bool FlowDirectionRightToLeft = false;
        public static string Lang = ""; //Default language ar_AE

        //Notification Settings >>
        //*********************************************************
        public static bool ShowNotification = true;
        public static string OneSignalAppId = "ff9df4e4-6217-4d94-9cd8-8af481c3e954";

        //Error Report Mode
        //*********************************************************
        public static bool SetApisReportMode = false;

        //Code Time Zone (true => Get from Internet , false => Get From #CodeTimeZone )
        //*********************************************************
        public static bool AutoCodeTimeZone = true;
        public static string CodeTimeZone = "UTC";

        //Set Theme Full Screen App
        //*********************************************************
        public static bool EnableFullScreenApp = true;

        //AdMob >> Please add the code ad in the Here and analytic.xml 
        //*********************************************************
        public static bool ShowAdMobBanner = false;
        public static bool ShowAdMobInterstitial = false;
        public static bool ShowAdMobRewardVideo = false;
        public static bool ShowAdMobNative = false;

        public static string AdInterstitialKey = "ca-app-pub-5135691635931982/4466434529";
        public static string AdRewardVideoKey = "ca-app-pub-5135691635931982/7731317149";
        public static string AdAdMobNativeKey = "ca-app-pub-5135691635931982/7916685759";

        //Three times after entering the ad is displayed
        public static int ShowAdMobInterstitialCount = 3;
        public static int ShowAdMobRewardedVideoCount = 3;

        //Social Logins >>
        //If you want login with facebook or google you should change id key in the analytic.xml file or AndroidManifest.xml
        //Facebook >> ../values/analytic.xml .. 
        //Google >> ../Properties/AndroidManifest.xml .. line 37
        //*********************************************************
        public static bool ShowFacebookLogin = false;
        public static bool ShowGoogleLogin = true;

        public static readonly string ClientId = "73090624867-82mgcm4saeiao5nslja1nhckqs4hq02h.apps.googleusercontent.com";
        public static readonly string ClientSecret = "5x4IMG4jY7LjyUBUjvPhknYL";

        //Chat Window Activity >>
        //*********************************************************
        //if you want this feature enabled go to Properties -> AndroidManefist.xml and remove comments from below code
        //Just replace it with this 5 lines of code
        /*
         <uses-permission android:name="android.permission.READ_CONTACTS" />
         <uses-permission android:name="android.permission.READ_PHONE_NUMBERS" /> 
         <uses-permission android:name="android.permission.GET_ACCOUNTS" />
         <uses-permission android:name="android.permission.SEND_SMS" />
         */
        public static bool ShowButtonContact = false;
        public static bool InvitationSystem = false;  //Invite friends section 
        /////////////////////////////////////

        public static bool ShowButtonCamera = true;
        public static bool ShowButtonImage = true;
        public static bool ShowButtonVideo = true;
        public static bool ShowButtonAttachFile = true;
        public static bool ShowButtonColor = true;
        public static bool ShowButtonStickers = true;
        public static bool ShowButtonMusic = true;
        public static bool ShowButtonGif = true;

        public static bool ShowMusicBar = false;

        //Set a story duration >> 10 Sec
        public static long StoryDuration = 10000L; //#New

        //Record Sound Style & Text
        //*********************************************************
        public static bool ShowButtonRecordSound = true;

        // Chat Group >>
        //*********************************************************
        public static bool EnableChatGroup = true;

        // Chat Page >>
        //*********************************************************
        public static bool EnableChatPage = true;

        // User Profile >>
        //*********************************************************
        public static bool EnableShowPhoneNumber = true;

        //*********************************************************
        /// <summary>
        ///  Currency
        /// CurrencyStatic = true : get currency from app not api 
        /// CurrencyStatic = false : get currency from api (default)
        /// </summary>
        public static readonly bool CurrencyStatic = false; //#New
        public static readonly string CurrencyIconStatic = "$"; //#New
        public static readonly string CurrencyCodeStatic = "USD"; //#New

        // Video/Audio Call Settings >>
        //*********************************************************
        public static bool EnableAudioVideoCall = true;

        public static bool EnableAudioCall = true;
        public static bool EnableVideoCall = true;

        public static bool UseAgoraLibrary = true;
        public static bool UseTwilioLibrary = false;

        // Walkthrough Settings >>
        //*********************************************************
        public static bool ShowWalkTroutPage = true;

        public static bool WalkThroughSetFlowAnimation = true;
        public static bool WalkThroughSetZoomAnimation = false;
        public static bool WalkThroughSetSlideOverAnimation = false;
        public static bool WalkThroughSetDepthAnimation = false;
        public static bool WalkThroughSetFadeAnimation = false;

        // Register Settings >>
        //*********************************************************
        public static bool ShowGenderOnRegister = true;

        //Last_Messages Page >>
        //*********************************************************
        public static bool ShowOnlineOfflineMessage = true;

        public static int RefreshChatActivitiesSeconds = 6000; // 6 Seconds
        public static int MessageRequestSpeed = 4000; // 4 Seconds

        public static bool RenderPriorityFastPostLoad = true;

        //Bypass Web Erros 
        //*********************************************************
        public static bool TurnTrustFailureOnWebException = false;
        public static bool TurnSecurityProtocolType3072On = false;

        // Stickers Packs Settings >>
        //*********************************************************
        public static int StickersOnEachRow = 3;
        public static string StickersBarColor = "#efefef";
        public static string StickersBarColorDark = "#282828";

        public static bool ShowStickerStack0 = true;
        public static bool ShowStickerStack1 = true;
        public static bool ShowStickerStack2 = true;
        public static bool ShowStickerStack3 = true;
        public static bool ShowStickerStack4 = true;
        public static bool ShowStickerStack5 = true;
        public static bool ShowStickerStack6 = false;

        public static bool SetTabDarkTheme = false;

        public static bool ShowSuggestedUsersOnRegister = true;
        public static bool ImageCropping = true;

        //Settings Page >> General Account
        public static bool ShowSettingsAccount = true;//#New
        public static bool ShowSettingsPassword = true;//#New
        public static bool ShowSettingsBlockedUsers = true;//#New
        public static bool ShowSettingsDeleteAccount = true;//#New
        public static bool ShowSettingsTwoFactor = true;//#New
        public static bool ShowSettingsManageSessions = true;//#New

        //Options chat heads (Bubbles) 
        //*********************************************************
        //Always , Hide , FullScreen
        public static string DisplayModeSettings = "Always";//#New

        //Default , Left  , Right , Nearest , Fix , Thrown
        public static string MoveDirectionSettings = "Right";//#New

        //Circle , Rectangle
        public static string ShapeSettings = "Circle";//#New

        // Last position
        public static bool IsUseLastPosition = true; //#New

    }
}