using Common.Data;
using Manages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : Singleton<StoryManager> 
{
	public void Init()
	{
		NpcManager.Instance.RegisterNpcEvent(NpcFunction.InvokeStory, OnOpenStory);
	}
	
	private bool OnOpenStory(NpcDefine npc)
	{
		this.ShowStoryUI(npc.Param);
		return true;
	}

	public void ShowStoryUI(int storyId)
	{
		StoryDefine story;
		if(DataManager.Instance.Storys.TryGetValue(storyId, out story))
		{
			UIStory uIStory = UIManager.Instance.Show<UIStory>();
			if(uIStory != null)
			{
				uIStory.SetStory(story);
			}
		}
	}

	public bool StartStory(int storyId)
	{
		StoryService.Instance.SendStartStory(storyId);
		return true;
	}

	internal void OnStoryStart(int storyId)
	{

	}
}
