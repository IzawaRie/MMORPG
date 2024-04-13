using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : Singleton<ArenaManager> 
{
	public int Round = 0;
	private ArenaInfo ArenaInfo;
	public ArenaManager()
	{

	}

	public void EnterArena(ArenaInfo arenaInfo)
	{
		Debug.LogFormat("ArenaManager.EnterArena : {0}", arenaInfo.AreanaId);
		this.ArenaInfo = arenaInfo;
	}

	public void ExitArena(ArenaInfo arenaInfo)
	{
        Debug.LogFormat("ArenaManager.ExitArena : {0}", arenaInfo.AreanaId);
		this.ArenaInfo = null;
    }

	internal void SendReady()
	{
		Debug.LogFormat("ArenaManager.SendReady : {0}", this.ArenaInfo.AreanaId);
		ArenaService.Instance.SendArenaReadyRequest(this.ArenaInfo.AreanaId);
	}

	internal void OnReady(int round, ArenaInfo arenaInfo)
	{
        Debug.LogFormat("ArenaManager.OnReady : {0} Round : {1}", this.ArenaInfo.AreanaId,round);
		this.Round = round;
		if (UIArena.Instance != null)
			UIArena.Instance.ShowCountDown();
    }

    internal void OnRoundStart(int round, ArenaInfo arenaInfo)
    {
        Debug.LogFormat("ArenaManager.OnRoundStart : {0} Round : {1}", this.ArenaInfo.AreanaId, round);
        if (UIArena.Instance != null)
            UIArena.Instance.ShowRoundStart(round, arenaInfo);
    }

    internal void OnRoundEnd(int round, ArenaInfo arenaInfo)
    {
        Debug.LogFormat("ArenaManager.OnRoundEnd : {0} Round : {1}", this.ArenaInfo.AreanaId, round);
        if (UIArena.Instance != null)
            UIArena.Instance.ShowRoundResult(round, arenaInfo);
    }
}
