using Android.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using WoWonder.Helpers.Fonts;

namespace WoWonder.Helpers.Utils
{
    public class EmptyStateInflater
    {
        public Button EmptyStateButton;
        private TextView EmptyStateIcon, DescriptionText, TitleText;

        public enum Type
        {
            NoConnection,
            NoSearchResult,
            SomThingWentWrong,
            NoUsers,
            NoFollow,
            NoNearBy,
            NoStory,
            NoCall,
            NoGroup,
            NoPage,
            NoMessages,
            NoFiles,
            NoGroupRequest,
            Gif,
            NoSessions
        }

        public void InflateLayout(View inflated, Type type)
        {
            try
            {
                EmptyStateIcon = (TextView)inflated.FindViewById(Resource.Id.emtyicon);
                TitleText = (TextView)inflated.FindViewById(Resource.Id.headText);
                DescriptionText = (TextView)inflated.FindViewById(Resource.Id.seconderyText);
                EmptyStateButton = (Button)inflated.FindViewById(Resource.Id.button);

                if (type == Type.NoConnection)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.IosThunderstormOutline);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoConnection_TitleText);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_NoConnection_DescriptionText);
                    EmptyStateButton.Text = Application.Context.GetText(Resource.String.Lbl_NoConnection_Button);
                }
                else if (type == Type.NoSearchResult)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Search);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoSearchResult_TitleText);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_NoSearchResult_DescriptionText);
                    EmptyStateButton.Text = Application.Context.GetText(Resource.String.Lbl_NoSearchResult_Button);
                }
                else if (type == Type.SomThingWentWrong)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Close);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_SomThingWentWrong_TitleText);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_SomThingWentWrong_DescriptionText);
                    EmptyStateButton.Text = Application.Context.GetText(Resource.String.Lbl_SomThingWentWrong_Button);
                }
                else if (type == Type.NoUsers)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Person);
                    EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoUsers_TitleText);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_NoUsers_DescriptionText);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoFollow)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Person);
                    EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoFollow_TitleText);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_NoFollow_DescriptionText);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoNearBy)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Person);
                    EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoUsers_TitleText);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_NearBy);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoStory)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.IosCameraOutline);
                    EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_lastStoriess);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_lastStoriess);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoCall)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.AndroidCall);
                    EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_calls);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_calls);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoGroup)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, EmptyStateIcon, FontAwesomeIcon.UserFriends);
                    EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_Group);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_Group);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoMessages)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.ChatbubbleWorking);
                    EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_Lastmessages);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_Lastmessages);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoFiles)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Document);
                    EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoAnyMedia);
                    DescriptionText.Text = " ";
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoGroupRequest)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, EmptyStateIcon, FontAwesomeIcon.UserFriends);
                    EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoAnyGroupRequest);
                    DescriptionText.Text = " ";
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.Gif)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, EmptyStateIcon, FontAwesomeIcon.Gift);
                    EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_Gif);
                    DescriptionText.Text = "";
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoSessions)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, EmptyStateIcon, FontAwesomeIcon.Fingerprint);
                    EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_Sessions);
                    DescriptionText.Text = "";
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoPage)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, EmptyStateIcon, FontAwesomeIcon.CalendarAlt);
                    EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_Page);
                    DescriptionText.Text = "";
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}