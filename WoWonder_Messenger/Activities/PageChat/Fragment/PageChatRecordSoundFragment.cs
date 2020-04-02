using Android.Graphics;
using Android.OS;
using Android.Support.V4.View.Animation;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using System;
using System.Timers;
using WoWonder.Activities.SettingsPreferences;
using WoWonder.Helpers.Utils;

namespace WoWonder.Activities.PageChat.Fragment
{
    public class PageChatRecordSoundFragment : Android.Support.V4.App.Fragment
    {
        private CircleButton RecourdPlaybutton;
        private CircleButton Recourdclosebutton;
        private SeekBar VoiceSeekbar;
        private string RecourdFilePath;
        private Methods.AudioRecorderAndPlayer AudioPlayerClass;
        private PageChatWindowActivity MainActivityview;
        private Timer TimerSound;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View chatRecourdSoundFragmentView = inflater.Inflate(Resource.Layout.Chat_Recourd_Sound_Fragment, container, false);

                RecourdFilePath = Arguments.GetString("FilePath");

                MainActivityview = ((PageChatWindowActivity)Activity);
                MainActivityview.ChatSendButton.SetImageResource(Resource.Drawable.ic_send_up_arrow);
                MainActivityview.ChatSendButton.Tag = "Audio";

                RecourdPlaybutton = chatRecourdSoundFragmentView.FindViewById<CircleButton>(Resource.Id.playButton);
                Recourdclosebutton = chatRecourdSoundFragmentView.FindViewById<CircleButton>(Resource.Id.closeRecourdButton);

                VoiceSeekbar = chatRecourdSoundFragmentView.FindViewById<SeekBar>(Resource.Id.voiceseekbar);
                VoiceSeekbar.ProgressChanged += VoiceSeekbar_ProgressChanged;
                VoiceSeekbar.Progress = 0;
                Recourdclosebutton.Click += Recourdclosebutton_Click;
                RecourdPlaybutton.Click += RecourdPlaybutton_Click;
                RecourdPlaybutton.Tag = "Stop";

                AudioPlayerClass = new Methods.AudioRecorderAndPlayer(MainActivityview.PageId);
                TimerSound = new Timer();

                return chatRecourdSoundFragmentView;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        private void VoiceSeekbar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {

        }

        private void Recourdclosebutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(RecourdFilePath))
                    AudioPlayerClass.StopAudioPlay();

                if (SettingsPrefFragment.SSoundControl)
                {
                    Methods.AudioRecorderAndPlayer.PlayAudioFromAsset("Error.mp3");
                }

                var fragmentHolder = Activity.FindViewById<FrameLayout>(Resource.Id.TopFragmentHolder);

                AudioPlayerClass.Delete_Sound_Path(RecourdFilePath);
                var interplator = new FastOutSlowInInterpolator();
                fragmentHolder.Animate().SetInterpolator(interplator).TranslationY(1200).SetDuration(300);
                Activity.SupportFragmentManager.BeginTransaction().Remove(this).Commit();

                MainActivityview.ChatSendButton.SetImageResource(Resource.Drawable.microphone);
                MainActivityview.ChatSendButton.Tag = "Free";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void RecourdPlaybutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(RecourdFilePath))
                {
                    if (RecourdPlaybutton.Tag.ToString() == "Stop")
                    {
                        RecourdPlaybutton.Tag = "Playing";
                        RecourdPlaybutton.SetColor(Color.ParseColor("#efefef"));
                        RecourdPlaybutton.SetImageResource(Resource.Drawable.ic_stop_dark_arrow);

                        AudioPlayerClass.PlayAudioFromPath(RecourdFilePath);
                        VoiceSeekbar.Max = AudioPlayerClass.Player.Duration;
                        TimerSound.Interval = 1000;
                        TimerSound.Elapsed += TimerSound_Elapsed;
                        TimerSound.Start();
                    }
                    else
                    {
                        RestPlayButton();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void TimerSound_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (AudioPlayerClass.Player.CurrentPosition + 50 >= AudioPlayerClass.Player.Duration &&
                    AudioPlayerClass.Player.CurrentPosition + 50 <= AudioPlayerClass.Player.Duration + 20)
                {
                    VoiceSeekbar.Progress = AudioPlayerClass.Player.Duration;
                    RestPlayButton();
                    TimerSound.Stop();
                }
                else if (VoiceSeekbar.Max != AudioPlayerClass.Player.Duration && AudioPlayerClass.Player.Duration == 0)
                {
                    RestPlayButton();
                    VoiceSeekbar.Max = AudioPlayerClass.Player.Duration;
                }
                else
                {
                    VoiceSeekbar.Progress = AudioPlayerClass.Player.CurrentPosition;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void RestPlayButton()
        {
            try
            {
                MainActivityview.RunOnUiThread(() =>
                {
                    RecourdPlaybutton.Tag = "Stop";
                    RecourdPlaybutton.SetColor(Color.White);
                    RecourdPlaybutton.SetImageResource(Resource.Drawable.ic_play_dark_arrow);
                    AudioPlayerClass.Player.Stop();
                    VoiceSeekbar.Progress = 0;
                });

                TimerSound.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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

        public override void OnDestroy()
        {
            try
            {

                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}