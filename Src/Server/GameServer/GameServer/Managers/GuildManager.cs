﻿using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using System;
using System.Collections.Generic;
using SkillBridge.Message;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class GuildManager : Singleton<GuildManager>
    {
        public Dictionary<int,Guild> Guilds =new Dictionary<int, Guild>();
        private HashSet<string> GuildNames =new HashSet<string>();

        public void Init()
        {
            this.Guilds.Clear();
            foreach(var guild in DBService.Instance.Entities.TGuilds)
            {
                this.AddGuild(new Guild(guild));
            }
        }

        void AddGuild(Guild guild)
        {
            this.Guilds.Add(guild.Id, guild);
            this.GuildNames.Add(guild.Name);
            guild.timestamp = TimeUtil.timestamp;
        }

        public bool CheckNameExisted(string name)
        {
            return GuildNames.Contains(name);
        }

        public bool CreateGuild(string name,string notice,Character leader)
        {
            DateTime now = DateTime.Now;
            TGuild dbGuild = DBService.Instance.Entities.TGuilds.Create();
            dbGuild.Name = name;
            dbGuild.Notice = notice;
            dbGuild.LeaderID = leader.Id;
            dbGuild.LeaderName = leader.Name;
            dbGuild.CreateTime = now;
            DBService.Instance.Entities.TGuilds.Add(dbGuild);
            DBService.Instance.Save();
            dbGuild = DBService.Instance.Entities.TGuilds.FirstOrDefault(v => v.LeaderID == leader.Id);
            Guild guild = new Guild(dbGuild);
            guild.AddMember(leader.Id, leader.Name, leader.Data.Class, (int)leader.Data.Level, GuildTitle.President);
            leader.Guild = guild;
            leader.Data.GuildId = dbGuild.Id;
            DBService.Instance.Save();
            this.AddGuild(guild);

            return true;
        }

        internal Guild GetGuild(int guildId)
        {
            if(guildId == 0) return null;
            Guild guild = null;
            this.Guilds.TryGetValue(guildId, out guild);
            return guild;
        }

        internal List<NGuildInfo> GetGuildsInfo()
        {
            List<NGuildInfo> result =new List<NGuildInfo> ();
            foreach(var kv in this.Guilds)
            {
                result.Add(kv.Value.GuildInfo(null));
            }
            return result;
        }
    }
}
