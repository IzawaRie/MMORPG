using Assets.Scripts.Managers;
using JetBrains.Annotations;
using Models;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagManager : Singleton<BagManager> {
	public int Unlocked;

	public BagItem[] items;

	NbagInfo Info;

	unsafe public void Init(NbagInfo info)
	{
		this.Info = info;
		this.Unlocked = info.Unlocked;
		items = new BagItem[this.Unlocked];
		if(info.Items != null&&info.Items.Length>=this.Unlocked)
		{
			Analyze(info.Items);
		}
		else
		{
			Info.Items = new byte[sizeof(BagItem)*this.Unlocked];
			Reset();
		}

	}

	public void Reset()
	{
		int i = 0;
		foreach(var kv in ItemManager.Instance.Items)
		{
			if(kv.Value.Count<=kv.Value.Define.StackLimit)
			{
				this.items[i].ItemId = (ushort)kv.Key;
				this.items[i].Count=(ushort)kv.Value.Count;
			}
			else
			{
				int count = kv.Value.Count;
				while(count>kv.Value.Define.StackLimit)
				{
					this.items[i].ItemId= (ushort)kv.Key;
					this.items[i].Count = (ushort)kv.Value.Define.StackLimit;
					i++;
					count-=kv.Value.Define.StackLimit;
				}
				this.items[i].ItemId=(ushort)kv.Key;
				this.items[i].Count = (ushort)count;
			}
			i++;
		}
	}
	unsafe void Analyze(byte[] data)
	{
		fixed(byte* pt = data)
		{
			for(int i=0;i<this.Unlocked;i++)
			{
				BagItem* item= (BagItem*)(pt+i*sizeof(BagItem));
				items[i] = *item;
			}
		}
	}

	unsafe public NbagInfo GetNbagInfo()
	{
		fixed(byte* pt = Info.Items)
		{
			for(int i=0;i<this.Unlocked;i++)
			{
				BagItem* item =(BagItem*)(pt+i*sizeof(BagItem));
				*item = items[i];
			}
		}
		return this.Info;
	}
	
   public void AddItem(int itemId,int count)
	{
		ushort addCount = (ushort)count;
		for(int i=0;i<items.Length;i++)
		{
			if (this.items[i].ItemId==itemId)
			{
				ushort canAdd = (ushort)(DataManager.Instance.Items[itemId].StackLimit - this.items[i].Count);
				if(canAdd>=addCount)
				{
					this.items[i].Count += addCount;
					addCount = 0;
					break;
				}
				else
				{
					this.items[i].Count += canAdd;
					addCount -= canAdd;
				}
			}
		}
		if(addCount>0)
		{
			for(int i=0;i<items.Length;i++)
			{
				if (this.items[i].ItemId==0)
				{
					this.items[i].ItemId = (ushort)itemId;
					this.items[i].Count = addCount;
					break;
				}
			}
		}
	}

	public void RemoveItem(int itemId,int count)
	{

	}
}
