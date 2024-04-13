using Common;
using GameServer.Entities;
using GameServer.Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;

namespace GameServer.Services
{
    class ArenaService : Singleton<ArenaService>
    {
        public ArenaService() 
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ArenaChallengeRequest>(this.OnArenaChallengeRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ArenaChallengeResponse>(this.OnArenaChallengeResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ArenaReadyRequest>(this.OnArenaReady);
        }

        public void Dispose()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<ArenaChallengeRequest>(this.OnArenaChallengeRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<ArenaChallengeResponse>(this.OnArenaChallengeResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Unsubscribe<ArenaReadyRequest>(this.OnArenaReady);
        }

        public void Init()
        {
            ArenaManager.Instance.Init();
        }

        private void OnArenaChallengeRequest(NetConnection<NetSession> sender, ArenaChallengeRequest request)
        {
            Character chracter = sender.Session.Character;
            Log.InfoFormat("OnArenaChallengeRequest :: RedId: {0} RedName: {1} BlueID:{2} BlueName:{3}", request.ArenaInfo.Red.EntityId, request.ArenaInfo.Red.Name, request.ArenaInfo.Blue.EntityId, request.ArenaInfo.Blue.Name);
            NetConnection<NetSession> blue = null;
            if(request.ArenaInfo.Blue.EntityId > 0)
            {
                blue = SessionManager.Instance.GetSession(request.ArenaInfo.Blue.EntityId);
            }
            if(blue == null)
            { 
                sender.Session.Response.arenaChallengeRes = new ArenaChallengeResponse();
                sender.Session.Response.arenaChallengeRes.Result = Result.Failed;
                sender.Session.Response.arenaChallengeRes.Errormsg = "好友不存在或者不在线";
                sender.SendResponse();
                return;
            }
            Log.InfoFormat("ForwardArenaChallengeRequest :: RedId:{0} RedName:{1} BlueID:{2} BlueName:{3}", request.ArenaInfo.Red.EntityId, request.ArenaInfo.Red.Name, request.ArenaInfo.Blue.EntityId, request.ArenaInfo.Blue.Name);
            blue.Session.Response.areanaChallengeReq = request;
            blue.SendResponse();
        }

        private void OnArenaChallengeResponse(NetConnection<NetSession> sender, ArenaChallengeResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnArenaChallengeResponse :: RedId: {0} RedName: {1} BlueID:{2} BlueName:{3}", response.ArenaInfo.Red.EntityId, response.ArenaInfo.Red.Name, response.ArenaInfo.Blue.EntityId, response.ArenaInfo.Blue.Name);
            var requester = SessionManager.Instance.GetSession(response.ArenaInfo.Red.EntityId);
            if(requester == null)
            {
                sender.Session.Response.arenaChallengeRes.Result = Result.Failed;
                sender.Session.Response.arenaChallengeRes.Errormsg = "挑战者已下线";
                sender.SendResponse();
                return;
            }

            if(response.Result == Result.Failed)
            {
                requester.Session.Response.arenaChallengeRes = response;
                requester.Session.Response.arenaChallengeRes.Result = Result.Failed;
                requester.SendResponse();
                return;
            }

            var arena = ArenaManager.Instance.NewArena(response.ArenaInfo, requester,sender);
            this.SendArenaBegin(arena);
        }

        private void SendArenaBegin(Arena arena)
        {
            var arenaBegin = new ArenaBeginResponse();
            arenaBegin.Result = Result.Failed;
            arenaBegin.Errormsg = "对方不在线";
            arenaBegin.ArenaInfo = arena.ArenaInfo;
            arena.Red.Session.Response.arenaBegin = arenaBegin;
            arena.Red.SendResponse();
            arena.Blue.Session.Response.arenaBegin = arenaBegin;
            arena.Blue.SendResponse();
        }

        private void OnArenaReady(NetConnection<NetSession> sender, ArenaReadyRequest message)
        {
            Arena arena = ArenaManager.Instance.GetArena(message.arenaId);
            arena.EntityReady(message.entityId);
        }

        internal void SendArenaReady(Arena arena)
        {
            var arenaReady = new ArenaReadyResponse();
            arenaReady.Round = arena.Round;
            arenaReady.ArenaInfo = arena.ArenaInfo;
            arena.Red.Session.Response.arenaReady = arenaReady;
            arena.Red.SendResponse();
            arena.Blue.Session.Response.arenaReady = arenaReady;
            arena.Blue.SendResponse();
        }

        internal void SendArenaRoundStart(Arena arena)
        {
            var roundStart = new ArenaRoundStartResponse();
            roundStart.Round = arena.Round;
            roundStart.ArenaInfo = arena.ArenaInfo;
            arena.Red.Session.Response.arenaRoundStart = roundStart;
            arena.Red.SendResponse();
            arena.Blue.Session.Response.arenaRoundStart = roundStart;
            arena.Blue.SendResponse();
        }

        internal void SendArenaRoundEnd(Arena arena)
        {
            var roundEnd = new ArenaRoundEndResponse();
            roundEnd.Round = arena.Round;
            roundEnd.ArenaInfo = arena.ArenaInfo;
            arena.Red.Session.Response.arenaRoundEnd = roundEnd;
            arena.Red.SendResponse();
            arena.Blue.Session.Response.arenaRoundEnd = roundEnd;
            arena.Blue.SendResponse();
        }
    }
}
