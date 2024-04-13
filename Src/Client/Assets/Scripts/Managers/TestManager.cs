using Common.Data;
using Manages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : Singleton<TestManager>
{
	public void Init()
	{
		NpcManager.Instance.RegisterNpcEvent(Common.Data.NpcFunction.InvokeShop, OnNpcInvokeShop);
        NpcManager.Instance.RegisterNpcEvent(Common.Data.NpcFunction.InvokeInsrance, OnNpcInvokeInsrance);
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private bool OnNpcInvokeShop(NpcDefine npc)
	{
		Debug.LogFormat("TestManager.OnNpcInvokeShop:NPC:[{0}:{1}] Type:{2} Func:{3}", npc.ID, npc.Name, npc.Type, npc.Function);
		UITest test = UIManager.Instance.Show<UITest>();
		test.SetTitle(npc.Name);
		return true;
	}

	private bool OnNpcInvokeInsrance(NpcDefine npc)
	{
        Debug.LogFormat("TestManager.OnNpcInvokeShop:NPC:[{0}:{1}] Type:{2} Func:{3}", npc.ID, npc.Name, npc.Type, npc.Function);
		MessageBox.Show("点击了NPC" + npc.Name, "对话");
		return true;
    }
}
