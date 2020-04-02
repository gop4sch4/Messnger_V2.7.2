using AutoMapper;
using AutoMapper.Configuration;
using System;
using WoWonder.Helpers.Model;
using WoWonder.SQLite;
using WoWonderClient.Classes.Global;
using WoWonderClient.Classes.Message;

namespace WoWonder.Helpers.Utils
{
    public static class ClassMapper
    {
        public static void SetMappers()
        {
            try
            {
                var cfg = new MapperConfigurationExpression
                {
                    AllowNullCollections = true
                };

                cfg.CreateMap<MessageDataExtra, MessageData>();
                cfg.CreateMap<MessageData, MessageDataExtra>();

                cfg.CreateMap<GetSiteSettingsObject.Config, DataTables.SettingsTb>().ForMember(x => x.AutoIdSettings, opt => opt.Ignore());
                cfg.CreateMap<UserDataObject, DataTables.MyContactsTb>().ForMember(x => x.AutoIdMyFollowing, opt => opt.Ignore());
                cfg.CreateMap<UserDataObject, DataTables.MyFollowersTb>().ForMember(x => x.AutoIdMyFollowers, opt => opt.Ignore());
                cfg.CreateMap<UserDataObject, DataTables.MyProfileTb>().ForMember(x => x.AutoIdMyProfile, opt => opt.Ignore());
                cfg.CreateMap<ChatObject, DataTables.LastUsersTb>().ForMember(x => x.AutoIdLastUsers, opt => opt.Ignore());
                cfg.CreateMap<MessageDataExtra, DataTables.MessageTb>().ForMember(x => x.AutoIdMessage, opt => opt.Ignore());

                Mapper.Initialize(cfg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}