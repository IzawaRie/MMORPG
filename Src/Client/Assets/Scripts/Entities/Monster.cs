using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Entities
{
    public class Monster : Creature
    {
        public Monster(NCharacterInfo info) : base(info)
        {
            this.Attributes.HP = this.Attributes.MaxHP;
            this.Attributes.MP = this.Attributes.MaxMP;
        }
    }
}
