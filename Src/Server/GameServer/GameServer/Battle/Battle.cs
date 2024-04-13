﻿using GameServer.Core;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Battle
{
    class Battle
    {
        public Map Map;

        /// <summary>
        /// 所有参加战斗的单位
        /// </summary>
        Dictionary<int,Creature> AllUnits = new Dictionary<int, Creature>();

        Queue<NSkillCastInfo> Actions = new Queue<NSkillCastInfo>();

        List<NSkillCastInfo> CastSkills = new List<NSkillCastInfo>();

        List<NSkillHitInfo> Hits = new List<NSkillHitInfo>();

        List<NBuffInfo> BuffActions = new List<NBuffInfo>();

        List<Creature> DeathPool = new List<Creature>();

        public Battle(Map map) 
        {
            this.Map = map;
        }

        internal void ProcessBattleMessage(NetConnection<NetSession> sender,SkillCastRequest request)
        {
            Character character = sender.Session.Character;
            if(request.castInfo != null)
            {
                if (character.entityId != request.castInfo.casterId)
                    return;

                this.Actions.Enqueue(request.castInfo);
            }
        }

        internal void Update()
        {
            this.CastSkills.Clear();
            this.Hits.Clear();
            this.BuffActions.Clear();
            if(this.Actions.Count > 0)
            {
                NSkillCastInfo skillCast = this.Actions.Dequeue();
                this.ExecuteAction(skillCast);
            }

            this.UpdateUnits();

            this.BroadcastHitsMessage();
        }

        public void JoinBattle(Creature unit)
        {
            this.AllUnits[unit.entityId] = unit;
        }

        public void LeaveBattle(Creature unit)
        {
            this.AllUnits.Remove(unit.entityId);
        }

        void ExecuteAction(NSkillCastInfo cast)
        {
            BattleContext context = new BattleContext(this);
            context.Caster = EntityManager.Instance.GetCreature(cast.casterId);
            context.Target = EntityManager.Instance.GetCreature(cast.targetId);
            context.CastSkill = cast;
            context.Position = cast.Position;
            if (context.Caster != null)
                this.JoinBattle(context.Caster);
            if(context.Target != null)
                this.JoinBattle(context.Target);

            context.Caster.CastSkill(context, cast.skillId);
        }

        private void BroadcastHitsMessage()
        {
            if(this.Hits.Count == 0 && this.BuffActions.Count == 0 && this.CastSkills.Count == 0) return;
            NetMessageResponse message = new NetMessageResponse();
            if (this.CastSkills.Count > 0)
            {
                message.skillCast = new SkillCastResponse();
                message.skillCast.castInfoes.AddRange(this.CastSkills);
                message.skillCast.Result = Result.Success;
                message.skillCast.Errormsg = "";
            }

            if (this.Hits.Count > 0)
            {
                message.skillHits = new SkillHitResponse();
                message.skillHits.Hits.AddRange(this.Hits);
                message.skillHits.Result = Result.Success;
                message.skillHits.Errormsg = "";
            }

            if(this.BuffActions.Count > 0)
            {
                message.buffRes = new BuffResponse();
                message.buffRes.Buffs.AddRange(this.BuffActions);
                message.buffRes.Result = Result.Success;
                message.buffRes.Errormsg = "";
            }
            this.Map.BroadcastBattleResponse(message);
        }

        void UpdateUnits()
        {
            this.DeathPool.Clear();
            foreach (var kv in this.AllUnits)
            {
                kv.Value.Update();
                if (kv.Value.IsDeath)
                    this.DeathPool.Add(kv.Value);
            }

            foreach (var unit in this.DeathPool)
            {
                this.LeaveBattle(unit);
            }
        }

        internal List<Creature> FindUnitsInRange(Vector3Int pos, int range)
        {
            List<Creature> result = new List<Creature>();
            foreach(var unit in this.AllUnits)
            {
                if(unit.Value.Distance(pos) < range)
                {
                    result.Add(unit.Value);
                }
            }
            return result;
        }

        internal List<Creature> FindUnitsInMapRange(Vector3Int pos, int range)
        {
            return EntityManager.Instance.GetMapEntitiesInRange<Creature>(this.Map.ID, pos, range);
        }

        public void AddCastSkillInfo(NSkillCastInfo cast)
        {
            this.CastSkills.Add(cast);
        }

        public void AddHitInfo(NSkillHitInfo hit)
        {
            this.Hits.Add(hit);
        }

        public void AddBuffAction(NBuffInfo buff)
        {
            this.BuffActions.Add(buff);
        }
    }
}