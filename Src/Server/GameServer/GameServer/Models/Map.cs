using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkillBridge.Message;

using Common;
using Common.Data;

using Network;
using GameServer.Managers;
using GameServer.Entities;
using System.ServiceModel.Channels;
using GameServer.Services;

namespace GameServer.Models
{
    class Map
    {
        internal class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            public MapCharacter(NetConnection<NetSession> conn, Character cha)
            {
                this.connection = conn;
                this.character = cha;
            }
        }

        public int ID
        {
            get { return this.Define.ID; }
        }
        public int InstanceID { get; set; }

        internal MapDefine Define;
        /// <summary>
        /// 地图中的角色，以CharacterID为Key
        /// </summary>
        Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();

        /// <summary>
        /// 刷怪管理器
        /// </summary>
        /// <param name="define"></param>
        SpawnManager spawnManager = new SpawnManager();

        public Battle.Battle Battle;

        public MonsterManager monsterManager = new MonsterManager();

        internal Map(MapDefine define, int instanceId)
        {
            this.Define = define;
            this.InstanceID = instanceId;
            this.spawnManager.Init(this);
            this.monsterManager.Init(this);
            this.Battle = new Battle.Battle(this);
        }

        internal void Update()
        {
            spawnManager.Update();
            this.Battle.Update();
        }

        /// <summary>
        /// 角色进入地图
        /// </summary>
        /// <param name="character"></param>
        internal void CharacterEnter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", this.Define.ID, character.Id);
            this.AddCharacter(conn, character);

            conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;

            foreach (var kv in this.MapCharacters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                if (kv.Value.character != character)
                    this.AddCharacterEnterMap(kv.Value.connection, character.Info);
            }

            foreach (var kv in this.monsterManager.Monsters)
            {
                conn.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.Info);
            }
            conn.SendResponse();
        }

        internal void AddCharacter(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("AddCharacter: Map:{0} characterId:{1}", this.Define.ID, character.Id);
            character.Info.mapId = this.ID;
            if(!this.MapCharacters.ContainsKey(character.Id))
            {
                this.MapCharacters[character.Id] = new MapCharacter(conn, character);
            }
        }

        internal void CharacterLeave(Character cha)
        {
            Log.InfoFormat("CharacterLeave: Map:{0} characterId:{1}", this.Define.ID, cha.Id);

            foreach(var kv in this.MapCharacters)
            {
                this.SendCharacterLeaveMap(kv.Value.connection, cha);
            }
            this.MapCharacters.Remove(cha.Id);
            //NetMessage message = new NetMessage();
            //message.Response = new NetMessageResponse();
            //message.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            //message.Response.mapCharacterEnter.Characters.Clear();
        }
       
        void AddCharacterEnterMap(NetConnection<NetSession> conn,NCharacterInfo character)
        {
            if(conn.Session.Response.mapCharacterEnter==null)
            {
                conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
                conn.Session.Response.mapCharacterEnter.mapId =this.Define.ID;
            }
            conn.Session.Response.mapCharacterEnter.Characters.Add(character);
            conn.SendResponse();
        }
        void SendCharacterLeaveMap(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("SendCharacterLeaveMap To {0}:{1} :Map:{2} Character:{3}:{4}", conn.Session.Character.Id, conn.Session.Character.Info.Name, this.Define.ID, character.Id, character.Info.Name);
            conn.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            conn.Session.Response.mapCharacterLeave.entityId=character.entityId;
            conn.SendResponse();
        }

        internal void UpdateEntity(NEntitySync entity)
        {
            foreach(var kv in this.MapCharacters)
            {
                if(kv.Value.character.entityId==entity.Id)
                {
                    kv.Value.character.Position=entity.Entity.Position;
                    kv.Value.character.Direction=entity.Entity.Direction;
                    kv.Value.character.Speed=entity.Entity.Speed;
                    if(entity.Event == EntityEvent.Ride)
                    {
                        kv.Value.character.Ride = entity.Param;
                    }
                }
                else
                {
                    MapService.Instance.SendEntityUpdate(kv.Value.connection, entity);
                }
            }
        }

        ///<summary>
        ///怪物进入地图
        ///</summary>
        ///<param name="character"></param>
        internal void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("MonsterEnter:Map{0} monsterId:{1}", this.Define.ID, monster.Id);
            monster.OnEnterMap(this);
            foreach(var kv in this.MapCharacters)
            {
                this.AddCharacterEnterMap(kv.Value.connection, monster.Info);
            }
        }

        internal void BroadcastBattleResponse(NetMessageResponse response)
        {
            foreach(var kv in this.MapCharacters)
            {
                if(response.skillCast != null)
                    kv.Value.connection.Session.Response.skillCast = response.skillCast;
                if(response.skillHits != null)
                    kv.Value.connection.Session.Response.skillHits = response.skillHits;
                if(response.buffRes != null) 
                    kv.Value.connection.Session.Response.buffRes = response.buffRes;
                kv.Value.connection.SendResponse();
            }
        }
    }
}
