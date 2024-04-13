using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manages
{
    class NpcManager : Singleton<NpcManager>
    {
        public delegate bool NpcActionHandler(NpcDefine npc);

        Dictionary<NpcFunction, NpcActionHandler> eventMap =new Dictionary<NpcFunction, NpcActionHandler>();
        Dictionary<int,Vector3> npcPositons = new Dictionary<int,Vector3>();

        public void RegisterNpcEvent(NpcFunction function, NpcActionHandler action)
        {
            if (!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;
            }
            else
                eventMap[function] += action;
        }

        public NpcDefine GetNpcDefine(int npcId)
        {
            NpcDefine npc = null;
            DataManager.Instance.NPCs.TryGetValue(npcId, out npc);
            return npc;
        }

        public bool Interactive(int npcId)
        {
            if(DataManager.Instance.NPCs.ContainsKey(npcId))
            {
                var npc = DataManager.Instance.NPCs[npcId];
                return Interactive(npc);
            }
            else
                return false;
        }

        public bool Interactive(NpcDefine npc)
        {
            if (DoTaskInteractive(npc))
            {
                return true;
            }
            else if(npc.Type==NpcType.Functional)
            {
                return DoFuntionInteractive(npc);
            }
            return false;
                
        }

        private bool DoTaskInteractive(NpcDefine npc)
        {
            var status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
            if (status == NpcQuestStatus.None)
                return false;
            return QuestManager.Instance.OpenNpcQuest(npc.ID);
        }

        private bool DoFuntionInteractive(NpcDefine npc)
        {
            if(npc.Type!=NpcType.Functional)
            {
                return false;
            }
            if(!eventMap.ContainsKey(npc.Function))
            {
                return false;
            }
            return eventMap[npc.Function](npc);
        }

        internal void UpdateNpcPositon(int npc,Vector3 pos)
        {
            this.npcPositons[npc] = pos;
        }
        // Use this for initialization
        
        internal Vector3 GetNpcPosition(int npc)
        {
            return this.npcPositons[npc];
        }
    }
}

