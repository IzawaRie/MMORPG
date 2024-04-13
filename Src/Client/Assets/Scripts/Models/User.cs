using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Services;
using Entities;

namespace Models
{
    class User : Singleton<User>
    {
        SkillBridge.Message.NUserInfo userInfo;

        public Text money;
        public SkillBridge.Message.NUserInfo Info
        {
            get { return userInfo; }
        }


        public void SetupUserInfo(SkillBridge.Message.NUserInfo info)
        {
            this.userInfo = info;
        }

        public MapDefine CurrentMapData { get; set; }

        public Character CurrentCharacter { get; set; }
        public SkillBridge.Message.NCharacterInfo CurrentCharacterInfo { get; set; }

        public PlayerInputController CurrentCharacterObject { get; set; }

        public NTeamInfo TeamInfo { get; set; }
        public void AddGold(int gold)
        {
            this.CurrentCharacterInfo.Gold+= gold;
            money.text = this.CurrentCharacterInfo.Gold.ToString();
        }

        public int CurrentRide = 0;
        internal void Ride(int id)
        {
            if(CurrentRide != id)
            {
                CurrentRide = id;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride ,CurrentRide);
            }
            else
            {
                CurrentRide = 0;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, 0);
            }
        }

        public delegate void ChracterInitHandler();
        public event ChracterInitHandler OnChracterInit;

        internal void CharacterInited()
        {
            if(OnChracterInit != null)
                OnChracterInit();
        }
    }
}
