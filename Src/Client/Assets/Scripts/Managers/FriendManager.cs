using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendManager : Singleton<FriendManager> 
{
	//所有好友
	public List<NFriendInfo> allFriends;

	public void Init(List<NFriendInfo> friends)
	{
		this.allFriends = friends;
	}
}
