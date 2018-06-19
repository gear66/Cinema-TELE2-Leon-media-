using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using System.Collections;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace Prototype.NetworkLobby
{
    public class LobbyManager : NetworkLobbyManager
    {
        static short MsgKicked = MsgType.Highest + 1;

        static public LobbyManager s_Singleton;
        public GameObject engine;

        static public WebSocket ws;

        [Header("Unity UI Lobby")]
        [Tooltip("Time in second between all players ready & match start")]
        public float prematchCountdown = 5.0f;

        [Space]
        [Header("UI Reference")]
        public LobbyTopPanel topPanel;
        public string UserName = "Client";
        public string LobbyName = "1";

        public RectTransform mainMenuPanel;
        public RectTransform lobbyPanel;

        public LobbyInfoPanel infoPanel;
        public LobbyCountdownPanel countdownPanel;
        public GameObject addPlayerButton;

        protected RectTransform currentPanel;

        public Button backButton;

        public Text statusInfo;
        public Text hostInfo;

        public GameObject numpad;

        //Client numPlayers from NetworkManager is always 0, so we count (throught connect/destroy in LobbyPlayer) the number
        //of players, so that even client know how many player there is.
        [HideInInspector]
        public int _playerNumber = 0;

        //used to disconnect a client properly when exiting the matchmaker
        [HideInInspector]
        public bool _isMatchmaking = false;

        protected bool _disconnectServer = false;

        protected ulong _currentMatchID;

        protected LobbyHook _lobbyHooks;

        private bool startDemo = false;

        public static Dictionary<string, RectTransform> screens;
        public static Dictionary<string, Action<Payload>> serverCommands;
        public static Dictionary<string, Action<Payload>> playerCommands;
        public static Dictionary<string, Action<Payload>> adminCommands;
        public static Dictionary<string, Dictionary<string, Action<Payload>>> commandsSet;

        public float min;
        public float max;
        public bool diapasonSet = false;
        public bool toggling = false;
        public bool toggle = true;

        private void Update()
        {
            if (startDemo)
            {
                playerCommands["beginDemo"](new Payload());
                startDemo = false;
            }

            if (diapasonSet)
            {
                playerCommands["setMinMax"](new Payload());
                diapasonSet = false;
            }

            if (toggling)
            {
                playerCommands["toggleOnline"](new Payload());
                toggling = false;
            }
        }

        void Start()
        {
            s_Singleton = this;
            _lobbyHooks = GetComponent<Prototype.NetworkLobby.LobbyHook>();
            currentPanel = mainMenuPanel;

            backButton.gameObject.SetActive(false);
            GetComponent<Canvas>().enabled = true;

            adminCommands = new Dictionary<string, Action<Payload>> {
                { "playerConnected", (payload) => {
                        User user = payload.user;
                        GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

                        LobbyPlayer newPlayer = obj.GetComponent<LobbyPlayer>();

                        newPlayer.playerName = user.userName;
                        newPlayer.nameInput.text = user.userName;

                        LobbyPlayerList._instance.AddPlayer(newPlayer);
                    }
                }
            };

            playerCommands = new Dictionary<string, Action<Payload>> {
                { "refreshData", (payload) => {
                        Debug.Log("Calling refresh data");
                        User user = new User();
                        user.userName = UserName;
                        user.userType = "Player";

                        Payload newPayload = new Payload();
                        newPayload.user = user;
                        newPayload.lobby = LobbyName;
                        newPayload.stateData = payload.stateData;

                        Command command = new Command();
                        command.setType = "playerCommands";
                        command.command = "refreshData";

                        Message message = new Message();
                        message.payload = newPayload;
                        message.command = command;
                        message.user = user;

                        string json = JsonConvert.SerializeObject(message);
                        ws.Send(json);
                    }
                },
                { "startDemo", (payload) => {
                        startDemo = true;
                    }
                },
                { "toggleOnline", (payload) => {
                    engine = GameObject.Find("ENGINE");
                    engine.GetComponent<engineClient>().OnlineVideo(toggle);
                    }
                },
                { "toggleOnlineVid", (payload) => {
                    Debug.Log("Toggler called");
                    toggling = true;
                    toggle = payload.onlineVid;
                    }
                },
                { "beginDemo", (payload) => {
                        Debug.Log("GO DEMO");
                        engine = GameObject.Find("ENGINE");
                        engine.GetComponent<engineClient>().StartDemo();
                    }
                },
                { "setMinMax", (payload) => {
                        Debug.Log("Setting diapason");
                        numpad.GetComponent<speedtest>().speedMin = min;
                        numpad.GetComponent<speedtest>().speedMax = max;
                    }
                },
                { "onSpeedTest", (payload) => {
                        Debug.Log("Setting diapason");
                        min = payload.speedTest * 2;
                        max = payload.speedTest * 2 + 2;
                        diapasonSet = true;
                    }
                }
            };

            serverCommands = new Dictionary<string, Action<Payload>> {
                { "joinLobby", (payload) => {

                        User user = new User();
                        user.userName = UserName;
                        user.userType = "Player";

                        Payload newPayload = new Payload();
                        newPayload.user = user;
                        newPayload.lobby = LobbyName;

                        Command command = new Command();
                        command.setType = "serverCommands";
                        command.command = "joinLobby";

                        Message message = new Message();
                        message.payload = newPayload;
                        message.command = command;

                        string json = JsonConvert.SerializeObject(message);
                        ws.Send(json);
                    }
                }
            };

            screens = new Dictionary<string, RectTransform> {
                { "mainMenu", s_Singleton.mainMenuPanel},
                { "lobby", s_Singleton.lobbyPanel }
            };

            commandsSet = new Dictionary<string, Dictionary<string, Action<Payload>>> {
                { "playerCommands", playerCommands },
                { "adminCommands", adminCommands },
                { "serverCommands", serverCommands }
            };

            //ws = new WebSocket("ws://localhost:8999");
            ws = new WebSocket("ws://cinematele2.herokuapp.com/");


            User admin = new User();
            admin.userType = "Player";
            admin.userName = UserName;

            Payload initPayload = new Payload();
            initPayload.user = admin;

            Command newCommand = new Command();
            newCommand.setType = "serverCommands";
            newCommand.command = "reg";

            Message newMessage = new Message();
            newMessage.command = newCommand;
            newMessage.payload = initPayload;

            string jsonAdmin = JsonConvert.SerializeObject(newMessage);
            ws.OnMessage += (sender, e) => {
                Message message = JsonConvert.DeserializeObject<Message>(e.Data);
                Debug.Log(message);
                Command command = message.command;

                Dictionary<string, Action<Payload>> currentCommandSet = commandsSet[command.setType];

                currentCommandSet[command.command](message.payload);
            };

            ws.Connect();
            ws.Send(jsonAdmin);

            DontDestroyOnLoad(gameObject);

            SetServerInfo("Offline", "None");
        }

        public LobbyPlayer GetPlayer(Payload payload)
        {
            User user = payload.user;
            GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

            LobbyPlayer newPlayer = obj.GetComponent<LobbyPlayer>();

            newPlayer.playerName = user.userName;
            newPlayer.nameInput.text = user.userName;

            return newPlayer;
        }

        public override void OnLobbyClientSceneChanged(NetworkConnection conn)
        {
            if (SceneManager.GetSceneAt(0).name == lobbyScene)
            {
                if (topPanel.isInGame)
                {
                    ChangeTo(lobbyPanel);
                    if (_isMatchmaking)
                    {
                        if (conn.playerControllers[0].unetView.isServer)
                        {
                            backDelegate = StopHostClbk;
                        }
                        else
                        {
                            backDelegate = StopClientClbk;
                        }
                    }
                    else
                    {
                        if (conn.playerControllers[0].unetView.isClient)
                        {
                            backDelegate = StopHostClbk;
                        }
                        else
                        {
                            backDelegate = StopClientClbk;
                        }
                    }
                }
                else
                {
                    ChangeTo(mainMenuPanel);
                }

                topPanel.ToggleVisibility(true);
                topPanel.isInGame = false;
            }
            else
            {
                ChangeTo(null);

                Destroy(GameObject.Find("MainMenuUI(Clone)"));

                //backDelegate = StopGameClbk;
                topPanel.isInGame = true;
                topPanel.ToggleVisibility(false);
            }
        }

        public void ConnectToLobby()
        {
            serverCommands["joinLobby"](new Payload());

            User admin = new User();
            admin.userType = "Player";
            admin.userName = UserName;

            Payload initPayload = new Payload();
            initPayload.user = admin;

            LobbyPlayerList._instance.AddPlayer(GetPlayer(initPayload));
        }

        public void ChangeTo(RectTransform newPanel)
        {
            if (currentPanel != null)
            {
                currentPanel.gameObject.SetActive(false);
            }

            if (newPanel != null)
            {
                newPanel.gameObject.SetActive(true);
            }

            currentPanel = newPanel;

            if (currentPanel != mainMenuPanel)
            {
                backButton.gameObject.SetActive(true);
            }
            else
            {
                backButton.gameObject.SetActive(false);
                SetServerInfo("Offline", "None");
                _isMatchmaking = false;
            }
        }

        public void DisplayIsConnecting()
        {
            var _this = this;
            infoPanel.Display("Connecting...", "Cancel", () => { _this.backDelegate(); });
        }

        public void SetServerInfo(string status, string host)
        {
            statusInfo.text = status;
            hostInfo.text = host;
        }


        public delegate void BackButtonDelegate();
        public BackButtonDelegate backDelegate;
        public void GoBackButton()
        {
            backDelegate();
            topPanel.isInGame = false;
        }

        // ----------------- Server management

        public void AddLocalPlayer()
        {
            TryToAddPlayer();
        }

        public void RemovePlayer(LobbyPlayer player)
        {
            player.RemovePlayer();
        }

        public void SimpleBackClbk()
        {
            ChangeTo(mainMenuPanel);
        }

        public void StopHostClbk()
        {
            if (_isMatchmaking)
            {
                matchMaker.DestroyMatch((NetworkID)_currentMatchID, 0, OnDestroyMatch);
                _disconnectServer = true;
            }
            else
            {
                StopHost();
            }


            ChangeTo(mainMenuPanel);
        }

        public void StopClientClbk()
        {
            StopClient();

            if (_isMatchmaking)
            {
                StopMatchMaker();
            }

            ChangeTo(mainMenuPanel);
        }

        public void StopServerClbk()
        {
            StopServer();
            ChangeTo(mainMenuPanel);
        }

        class KickMsg : MessageBase { }
        public void KickPlayer(NetworkConnection conn)
        {
            conn.Send(MsgKicked, new KickMsg());
        }




        public void KickedMessageHandler(NetworkMessage netMsg)
        {
            infoPanel.Display("Kicked by Server", "Close", null);
            netMsg.conn.Disconnect();
        }

        //===================

        public override void OnStartHost()
        {
            base.OnStartHost();

            ChangeTo(lobbyPanel);
            backDelegate = StopHostClbk;
            SetServerInfo("Hosting", networkAddress);
        }

        public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
        {
            base.OnMatchCreate(success, extendedInfo, matchInfo);
            _currentMatchID = (System.UInt64)matchInfo.networkId;
        }

        public override void OnDestroyMatch(bool success, string extendedInfo)
        {
            base.OnDestroyMatch(success, extendedInfo);
            if (_disconnectServer)
            {
                StopMatchMaker();
                StopHost();
            }
        }

        //allow to handle the (+) button to add/remove player
        public void OnPlayersNumberModified(int count)
        {
            _playerNumber += count;

            int localPlayerCount = 0;
            foreach (PlayerController p in ClientScene.localPlayers)
                localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

            addPlayerButton.SetActive(localPlayerCount < maxPlayersPerConnection && _playerNumber < maxPlayers);
        }

        // ----------------- Server callbacks ------------------

        //we want to disable the button JOIN if we don't have enough player
        //But OnLobbyClientConnect isn't called on hosting player. So we override the lobbyPlayer creation
        public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
        {
            GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;

            LobbyPlayer newPlayer = obj.GetComponent<LobbyPlayer>();
            newPlayer.ToggleJoinButton(numPlayers + 1 >= minPlayers);


            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
                }
            }

            return obj;
        }

        public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
        {
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers + 1 >= minPlayers);
                }
            }
        }

        public override void OnLobbyServerDisconnect(NetworkConnection conn)
        {
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                LobbyPlayer p = lobbySlots[i] as LobbyPlayer;

                if (p != null)
                {
                    p.RpcUpdateRemoveButton();
                    p.ToggleJoinButton(numPlayers >= minPlayers);
                }
            }

        }

        public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
        {
            //This hook allows you to apply state data from the lobby-player to the game-player
            //just subclass "LobbyHook" and add it to the lobby object.

            if (_lobbyHooks)
                _lobbyHooks.OnLobbyServerSceneLoadedForPlayer(this, lobbyPlayer, gamePlayer);

            return true;
        }

        // --- Countdown management

        public override void OnLobbyServerPlayersReady()
        {
            bool allready = true;
            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                if (lobbySlots[i] != null)
                    allready &= lobbySlots[i].readyToBegin;
            }

            if (allready)
                StartCoroutine(ServerCountdownCoroutine());
        }

        public IEnumerator ServerCountdownCoroutine()
        {
            float remainingTime = prematchCountdown;
            int floorTime = Mathf.FloorToInt(remainingTime);

            while (remainingTime > 0)
            {
                yield return null;

                remainingTime -= Time.deltaTime;
                int newFloorTime = Mathf.FloorToInt(remainingTime);

                if (newFloorTime != floorTime)
                {//to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
                    floorTime = newFloorTime;

                    for (int i = 0; i < lobbySlots.Length; ++i)
                    {
                        if (lobbySlots[i] != null)
                        {//there is maxPlayer slots, so some could be == null, need to test it before accessing!
                            (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(floorTime);
                        }
                    }
                }
            }

            for (int i = 0; i < lobbySlots.Length; ++i)
            {
                if (lobbySlots[i] != null)
                {
                    (lobbySlots[i] as LobbyPlayer).RpcUpdateCountdown(0);
                }
            }

            ServerChangeScene(playScene);
        }

        // ----------------- Client callbacks ------------------

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            infoPanel.gameObject.SetActive(false);

            conn.RegisterHandler(MsgKicked, KickedMessageHandler);

            if (!NetworkServer.active)
            {//only to do on pure client (not self hosting client)
                ChangeTo(lobbyPanel);
                backDelegate = StopClientClbk;
                SetServerInfo("Client", networkAddress);
            }
        }


        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            ChangeTo(mainMenuPanel);
        }

        public override void OnClientError(NetworkConnection conn, int errorCode)
        {
            ChangeTo(mainMenuPanel);
            //infoPanel.Display("Cient error : " + (errorCode == 6 ? "timeout" : errorCode.ToString()), "Close", null);
        }
    }
}