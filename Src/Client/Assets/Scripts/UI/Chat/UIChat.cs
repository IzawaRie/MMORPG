﻿using Assets.Scripts.Managers;
using Candlelight.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : MonoBehaviour 
{
	public HyperText textArea;//聊天内容显示区域

	public TabView channelTab;

	public InputField chatText;//聊天输入控件
	public Text chatTarget;

	public Dropdown channelSelect;

	// Use this for initialization
	void Start () 
	{
		this.channelTab.OnTabSelect += OnDisplayChannelSelected;
		ChatManager.Instance.OnChat +=RefreshUI;
	}
	
	private void OnDestroy()
	{
		ChatManager.Instance.OnChat -=RefreshUI;
	}
	// Update is called once per frame
	void Update () 
	{
		InputManager.Instance.IsInputMode = chatText.isFocused;
	}

	void OnDisplayChannelSelected(int idx)
	{
		ChatManager.Instance.displayChannel = (ChatManager.LocalChannel)idx;
		RefreshUI();
	}

	public void RefreshUI()
	{
		this.textArea.text = ChatManager.Instance.GetCurrentMessages();
		this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
		if (ChatManager.Instance.SendChannel == SkillBridge.Message.ChatChannel.Private)
		{
			this.chatTarget.gameObject.SetActive(true);
			if(ChatManager.Instance.PrivateID !=0)
			{
                this.chatTarget.text = ChatManager.Instance.PrivateName + ":";
            }
            else
                this.chatTarget.text = "<无>";
        }
		else
		{
			this.chatTarget.gameObject.SetActive(false);
		}
	}

	public void OnClickChatLink(HyperText.LinkInfo link)
	{
		if (string.IsNullOrEmpty(link.Name)) return;
		if(link.Name.StartsWith("c:"))
		{
			string[] strs = link.Name.Split(":".ToCharArray());
			UIPopCharMenu menu = UIManager.Instance.Show<UIPopCharMenu>();
			menu.targetId = int.Parse(strs[1]);
			menu.targetName = strs[2];
		}
	}

	public void OnClickSend()
	{
		OnEndInput(this.chatText.text);
	}

	public void OnEndInput(string text)
	{
		if (!string.IsNullOrEmpty(text.Trim()))
			this.SendChat(text);

		this.chatText.text = "";
	}

	void SendChat(string content)
	{
		ChatManager.Instance.SendChat(content, ChatManager.Instance.PrivateID, ChatManager.Instance.PrivateName);
	}

	public void OnSendChannelChanged()
	{
		int idx = channelSelect.GetComponent<Dropdown>().value;
		if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)idx+1)
			return;
		if(!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)idx+1))
		{
			return;
		}
		else
		{
			this.RefreshUI();
		}
	}
}
