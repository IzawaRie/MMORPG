using Common;
using Common.Battle;
using GameServer.Entities;
using System;
using System.Collections.Generic;

namespace GameServer.Battle
{
    internal class EffectManager
    {
        private Creature Owner;

        Dictionary<BuffEffect, int> Effects = new Dictionary<BuffEffect, int>();

        internal bool HasEffect(BuffEffect effect)
        {
            if(this.Effects.TryGetValue(effect, out int val))
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
            Log.InfoFormat("[{0}].AddEffect {1}", this.Owner.Name, effect);
            if (!Effects.ContainsKey(effect))
                this.Effects[effect] = 1;
            else
                this.Effects[effect]++;
        }

        internal void RemoveEffect(BuffEffect effect)
        {
            Log.InfoFormat("[{0}].RemoveEffect {1}",this.Owner.Name, effect);
            if (this.Effects[effect] > 0)
            {
                this.Effects[effect]--;
            }
        }
    }
}
