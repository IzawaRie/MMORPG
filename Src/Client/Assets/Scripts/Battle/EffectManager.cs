﻿using Common;
using Common.Battle;
using Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Battle
{
    public class EffectManager
    {
        private Creature Owner;

        Dictionary<BuffEffect, int> Effects = new Dictionary<BuffEffect, int>();

        internal bool HasEffect(BuffEffect effect)
        {
            int val;
            if (this.Effects.TryGetValue(effect, out val))
            {
                return val > 0;
            }
            return false;
        }

        public EffectManager(Creature owner)
        {
            this.Owner = owner;
        }

        internal void AddEffect(BuffEffect effect)
        {
            Debug.LogFormat("[{0}].AddEffect {1}", this.Owner.Name, effect);
            if (!this.Effects.ContainsKey(effect))
                this.Effects[effect] = 1;
            else
                this.Effects[effect]++;
        }

        internal void RemoveEffect(BuffEffect effect)
        {
            Debug.LogFormat("[{0}].RemoveEffect {1}", this.Owner.Name, effect);
            if (this.Effects[effect] > 0)
            {
                this.Effects[effect]--;
            }
        }
    }
}
