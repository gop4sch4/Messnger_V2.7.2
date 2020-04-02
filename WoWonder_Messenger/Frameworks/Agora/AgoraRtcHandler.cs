using DT.Xamarin.Agora;

namespace WoWonder.Frameworks.Agora
{
    public class AgoraRtcHandler : IRtcEngineEventHandler
    {
        private readonly AgoraVideoCallActivity Context;

        public AgoraRtcHandler(AgoraVideoCallActivity activity)
        {
            Context = activity;
        }

        public override void OnFirstRemoteVideoDecoded(int p0, int p1, int p2, int p3)
        {
            Context.OnFirstRemoteVideoDecoded(p0, p1, p2, p3);
        }

        public override void OnConnectionLost()
        {
            Context.OnConnectionLost();
        }

        public override void OnUserOffline(int p0, int p1)
        {
            Context.OnUserOffline(p0, p1);
        }

        public override void OnUserMuteVideo(int p0, bool p1)
        {
            Context.OnUserMuteVideo(p0, p1);
        }

        public override void OnFirstLocalVideoFrame(int p0, int p1, int p2)
        {
            Context.OnFirstLocalVideoFrame(p0, p1, p2);
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

    public static class EnumExtensions
    {
        public static string GetModeString(EncryptionType value)
        {
            switch (value)
            {
                case EncryptionType.Xts128:
                    return "aes-128-xts";
                case EncryptionType.Xts256:
                    return "aes-256-xts";
            }

            return string.Empty;
        }

        public static string GetDescriptionString(EncryptionType value)
        {
            switch (value)
            {
                case EncryptionType.Xts128:
                    return "AES 128";
                case EncryptionType.Xts256:
                    return "AES 256";
            }

            return string.Empty;
        }

        public enum EncryptionType
        {
            Xts128 = 0, // = "aes-128-xts",
            Xts256 = 1 // = "aes-256-xts"
        }
    }
}