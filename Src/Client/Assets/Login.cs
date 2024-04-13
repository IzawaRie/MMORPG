using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Login : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Network.NetClient.Instance.Init("172.41.94.76", 8000);
		Network.NetClient.Instance.Connect();

		SkillBridge.Message.NetMessage msg=new SkillBridge.Message.NetMessage();
		msg.Request=new SkillBridge.Message.NetMessageRequest();

		Network.NetClient.Instance.SendMessage(msg);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
