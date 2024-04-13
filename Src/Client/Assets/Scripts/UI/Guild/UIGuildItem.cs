using Assets.Scripts.Models;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildItem : ListView.ListViewItem
{
    public NGuildInfo Info;

	public Text Name;
	public Text ID;
    public Text MemberCount;
	public Text Leader;
    public void SetGuildInfo(NGuildInfo item)
	{
        Info = item; 
		ID.text = item.Id.ToString();
        Name.text = item.GuildName.ToString();
        MemberCount.text = item.memberCount.ToString();
        Leader.text = item.leaderName.ToString();
    }
}
