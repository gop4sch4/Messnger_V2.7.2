using Android.Media;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Timers;
using WoWonder.Adapters;
using WoWonderClient.Classes.Message;

namespace WoWonder.Helpers.Model
{
    public class UserChatMessagesObject
    {
        [JsonProperty("api_status", NullValueHandling = NullValueHandling.Ignore)]
        public int Status { get; set; }

        [JsonProperty("api_text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }

        [JsonProperty("api_version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }

        [JsonProperty("typing", NullValueHandling = NullValueHandling.Ignore)]
        public int Typing { get; set; }

        [JsonProperty("messages", NullValueHandling = NullValueHandling.Ignore)]
        public List<MessageDataExtra> Messages { get; set; }
    }

    public class MessageDataExtra : MessageData
    {
        public new MediaPlayer MediaPlayer { get; set; }
        public new Timer MediaTimer { get; set; }
        public new Holders.SoundViewHolder SoundViewHolder { get; set; }
        public new Holders.MusicBarViewHolder MusicBarViewHolder { get; set; }

        public bool ShowTimeText { get; set; }
    }

    public class AdapterModelsClassUser
    {
        public long Id { get; set; }
        public MessageModelType TypeView { get; set; }
        public MessageDataExtra MesData { get; set; }

    }
    public class AdapterModelsClassGroup
    {
        public long Id { get; set; }
        public MessageModelType TypeView { get; set; }
        public MessageDataExtra MesData { get; set; }

    }

    public class AdapterModelsClassPage
    {
        public long Id { get; set; }
        public MessageModelType TypeView { get; set; }
        public MessageDataExtra MesData { get; set; }

    }
}