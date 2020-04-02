using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using WoWonder.Helpers.Utils;
using WoWonderClient.Classes.Global;
using WoWonderClient.Requests;

namespace WoWonder.Activities.Authentication
{
    [Activity(Icon = "@mipmap/icon", Theme = "@style/ProfileTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class ForgetPasswordActivity : AppCompatActivity
    {
        #region Variables Basic

        private Button BtnSend;
        private EditText EmailEditext;
        private ProgressBar ProgressBar;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                // Set our view from the "ForgetPassword_Layout" layout resource
                SetContentView(Resource.Layout.ForgetPassword_Layout);

                EmailEditext = FindViewById<EditText>(Resource.Id.emailfield);
                BtnSend = FindViewById<Button>(Resource.Id.send);
                ProgressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

                ProgressBar.Visibility = ViewStates.Invisible;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();

                //Add Event
                BtnSend.Click += BtnSendOnClick;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();

                //Close Event
                BtnSend.Click -= BtnSendOnClick;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void BtnSendOnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                if (!string.IsNullOrEmpty(EmailEditext.Text))
                {
                    if (Methods.CheckConnectivity())
                    {
                        var check = Methods.FunString.IsEmailValid(EmailEditext.Text);
                        if (!check)
                        {
                            Methods.DialogPopup.InvokeAndShowDialog(this,
                                GetText(Resource.String.Lbl_VerificationFailed),
                                GetText(Resource.String.Lbl_IsEmailValid), GetText(Resource.String.Lbl_Ok));
                        }
                        else
                        {
                            ProgressBar.Visibility = ViewStates.Visible;
                            var (apiStatus, respond) =
                                await RequestsAsync.Global.Get_Reset_Password_Email(EmailEditext.Text);
                            if (apiStatus == 200)
                            {
                                if (respond is StatusObject result)
                                {
                                    Console.WriteLine(result);
                                    ProgressBar.Visibility = ViewStates.Invisible;
                                    Methods.DialogPopup.InvokeAndShowDialog(this,
                                        GetText(Resource.String.Lbl_CheckYourEmail),
                                        GetText(Resource.String.Lbl_WeSentEmailTo).Replace("@", EmailEditext.Text),
                                        GetText(Resource.String.Lbl_Ok));
                                }
                            }
                            else if (apiStatus == 400)
                            {
                                if (respond is ErrorObject error)
                                {
                                    var errortext = error._errors.ErrorText;
                                    Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security), errortext, GetText(Resource.String.Lbl_Ok));
                                }

                                ProgressBar.Visibility = ViewStates.Invisible;
                            }
                            else if (apiStatus == 404)
                            {

                                ProgressBar.Visibility = ViewStates.Invisible;
                                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_Security),
                                    GetText(Resource.String.Lbl_Error_Login), GetText(Resource.String.Lbl_Ok));
                            }
                        }
                    }
                    else
                    {
                        ProgressBar.Visibility = ViewStates.Invisible;
                        Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_VerificationFailed),
                            GetText(Resource.String.Lbl_Something_went_wrong), GetText(Resource.String.Lbl_Ok));
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                ProgressBar.Visibility = ViewStates.Invisible;
                Methods.DialogPopup.InvokeAndShowDialog(this, GetText(Resource.String.Lbl_VerificationFailed),
                    exception.ToString(), GetText(Resource.String.Lbl_Ok));
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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