using Common.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStory : UIWindow
{
	public Text title;
	public Text description;

	StoryDefine story;
	// Use this for initialization
	void Start () {
		
	}
	
	public void SetStory(StoryDefine story)
	{
		this.story = story;
		this.title.text = story.Name;
		this.description.text = story.Description;
	}
	
	public void OnClickStart()
	{
		if(!StoryManager.Instance.StartStory(this.story.ID))
		{

		}
	}
}
