﻿using Assets.Scripts.Managers;
using Models;
using Network;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

 public class ChatService:Singleton<ChatService>
 {
    public ChatService()
    {
        MessageDistributer.Instance.Subscribe<ChatResponse>(this.OnChat);
    }

    public void Dispose()
    {
        MessageDistributer.Instance.Unsubscribe<ChatResponse>(this.OnChat);
    }
    public void Init()
    {
        
    }

    public void SendChat(ChatChannel channel, string content,int toId,string toName)
    {
        Debug.Log("SendChat");
        NetMessage message = new NetMessage();
        message.Request = new NetMessageRequest();
        message.Request.Chat = new ChatRequest();
        message.Request.Chat.Message = new ChatMessage();
        message.Request.Chat.Message.Message = content;
        message.Request.Chat.Message.Channel = channel;
        message.Request.Chat.Message.ToId = toId;
        message.Request.Chat.Message.ToName = toName;
        NetClient.Instance.SendMessage(message);
    }

    private void OnChat(object sender, ChatResponse message)
    {
        if (message.Result == Result.Success)
        {
            ChatManager.Instance.AddMessages(ChatChannel.Local, message.localMessages);
            ChatManager.Instance.AddMessages(ChatChannel.World, message.worldMessages);
            ChatManager.Instance.AddMessages(ChatChannel.System, message.systemMessages);
            ChatManager.Instance.AddMessages(ChatChannel.Team, message.teamMessages);
            ChatManager.Instance.AddMessages(ChatChannel.Guild, message.guildMessages);
            ChatManager.Instance.AddMessages(ChatChannel.Private, message.privateMessages);
        }
        else
        {
            ChatManager.Instance.AddSystemMessage(message.Errormsg);
        }
    }
}

