using DT.Xamarin.Agora;

namespace WoWonder.Frameworks.Agora
{
    public class AgoraRtcAudioCallHandler : IRtcEngineEventHandler
    {
        private readonly AgoraAudioCallActivity Context;

        public AgoraRtcAudioCallHandler(AgoraAudioCallActivity activity)
        {
            Context = activity;
        }

        public override void OnConnectionLost()
        {
            Context.OnConnectionLost();
        }

        public override void OnUserOffline(int p0, int p1)
        {
            Context.OnUserOffline(p0, p1);
        }

        public override void OnNetworkQuality(int p0, int p1, int p2)
        {
            Context.OnNetworkQuality(p0, p1, p2);
        }

        public override void OnUserJoined(int p0, int p1)
        {
            Context.OnUserJoined(p0, p1);
        }
    }
}