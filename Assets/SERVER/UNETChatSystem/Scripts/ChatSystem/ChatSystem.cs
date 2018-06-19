using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

//this class should be attached to the root object for chat system since it will manage fade in/out of visibility
public class ChatSystem : NetworkBehaviour
{
    [SerializeField]
    private SyncListChatMessage chatMessages = new SyncListChatMessage();

    private List<UIChatMessage> messagesOnUI;

    private NetworkClient networkClient;
    //network channel in which to send messages across
    private const short messageChannel = 100;

    [SerializeField]
    private UIChatMessage chatMessagePrefab;
    //probably you will  want to adjust this based on expected chat message load. You could even modify it so we have max_messages per channel (in and MMO setting perhaps?)
    [SerializeField]
    private int MAX_MESSAGES = 5;
    [SerializeField]
    private const float DELAY_BEFORE_HIDING_CHAT = 10f; //in seconds
    [SerializeField]
    private bool openChatOnNewMessageReceived = true;
    //play with this value to adjust fade speed
    private float FADE_SPEED = 5f;
    //sometimes it magically triggers before the expected time, allowing small lieniency value will make it close properly
    private const float LIENIENCY = 0.5f;

    //You will probably have another class to manage these, but for simplicity of the demo's sake I have them here:
    public const int UNSET_TEAM = 0; // unlikely case, failsafe, should be used for "all chat"
                                     //public const int SPECTATOR_INDEX = 1; // the channel index for spectators. perhaps you want to disallow spectators from all chatting if they can see everyone.
                                     //public const int TERRORIST_INDEX = 2; // the channel index for terrorists
                                     //public const int COUNTER_TERRORIST_INDEX = 3; // the channel index for counter-terrorists

    //in version 1.1 we have replaced the above (commented lines) with a more robust way of referencing and creating ChatChannels
    [SerializeField]
    private List<ChatChannel> chatChannels;

    public ContentSizeFitter contentPanel;
    public CanvasGroup canvasGroup;
    public ChatIdentifier chatPanelIdentifier;

    private bool lerpAlphaOfChat;
    private float timeLastChatEntryHappened;

    private float targetAlpha;

    private uint channelToSend;

    //it's magic! not really. This is used to generate hex values for the color specified in the Editor. There's no simple Color.toHex()
    private const string hexValues = "0123456789ABCDEF";

    //private List<PlayerController> cachedPlayers;
    [SerializeField]
    public bool EnableWordFilter;
    [SerializeField]
    private List<WordFilter> WordFilters = new List<WordFilter>();
    [SerializeField]
    private List<Command> Commands = new List<Command>();

    void Start()
    {
        networkClient = NetworkManager.singleton.client;
        chatPanelIdentifier = GameObject.FindObjectOfType<ChatIdentifier>();
        //contentPanel = chatPanelIdentifier.GetComponentInChildren<ContentSizeFitter>();
        //canvasGroup = GetComponent<CanvasGroup>();
        NetworkServer.RegisterHandler(messageChannel, ReceivedMessage);
        chatMessages.Callback += OnChatMessagesUpdated;
        messagesOnUI = new List<UIChatMessage>();

        for (int i = 0; i < WordFilters.Count; i++)
        {
            WordFilters[i].regex = new Regex(WordFilters[i].RegularExpression, WordFilters[i].IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
        }

        //uncomment this only if you are trying to maintain local cache of players. Doing this requires a bit of extra work and creating a custom 
        //NetworkManager class. If you do not want to do this, the chat system will still work.
        //cachedPlayers = new List<PlayerController>(GameObject.FindObjectsOfType<PlayerController>());
    }

    private void OnChatMessagesUpdated(SyncListStruct<ChatEntry>.Operation op, int itemIndex)
    {
        Debug.Log("Operation: " + op.ToString() + " index: " + itemIndex);
        if (SyncListStruct<ChatEntry>.Operation.OP_ADD.ToString().Equals(op.ToString()))
        {
            Debug.Log("updating and creating prefab.");
            //swap the two lines below if you are going to maintain a local cache of players to prevent searching entire scene for them.
            //uint playerTeam = cachedPlayers.Find(player => player.isLocalPlayer).teamIndex;
            uint playerTeam = new List<PlayerController2>(GameObject.FindObjectsOfType<PlayerController2>()).Find(player => player.isLocalPlayer).teamIndex;

            //if you are in the wrong channel, do not create text prefab for that message
            if (chatMessages[itemIndex].Channel == UNSET_TEAM || chatMessages[itemIndex].Channel == playerTeam)
            {
                CreatePrefabAndAddToScreen(chatMessages[itemIndex]);
                if (openChatOnNewMessageReceived)
                {
                    OpenChat(false);
                }
                timeLastChatEntryHappened = Time.time;
                Invoke("TryToHideChat", DELAY_BEFORE_HIDING_CHAT);
            }
            //and if you are feeling adventurous, you may think about even removing it from the list.
        }
        //last condition should only happen when user is first connecting to a game in progress, and they have not filled up their queue
        else if (SyncListString.Operation.OP_REMOVEAT.ToString().Equals(op.ToString()) && !isServer && messagesOnUI.Count > MAX_MESSAGES)
        {
            Debug.Log("Destroying message: " + itemIndex);
            Destroy(messagesOnUI[itemIndex]);
            messagesOnUI.RemoveAt(itemIndex);
        }
    }

    private void ReceivedMessage(NetworkMessage message)
    {
        ChatMessage chatMessage = message.ReadMessage<ChatMessage>();

        chatMessages.Add(chatMessage.entry);

        //since we only get 1 message at a time, removing the 0 index = the oldest message. 
        //if you have max messages per channel...requires a tad of work on your end :(...  
        //you should filter chatMessages per channel and see if any exceed the limit, and remove the oldest from that channel.
        if (chatMessages.Count > MAX_MESSAGES)
        {
            chatMessages.RemoveAt(0);
            Destroy(messagesOnUI[0]);
            messagesOnUI.RemoveAt(0);
        }
    }

    //Uncomment this only if you are going to maintain a local cache of players and are using a custom NetworkManager. See below for more information
    //[Command]
    //public void CmdOnPlayerConnectOrDisconnect()
    //{
    //    cachedPlayers = new List<PlayerController>(GameObject.FindObjectsOfType<PlayerController>());
    //}

    /* If you want to maintain the player list within this ChatSystem (which is much better in terms of performance)
     * you will need to create a new NetworkManager class, for example:
      public class DefaultNetworkManager : NetworkManager
      {
        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            //we want to do the normal stuff first so our player should be initialized
            base.OnServerAddPlayer(conn, playerControllerId);
            chatSystem.CmdOnPlayerConnectOrDisconnect();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            //we do want to do our stuff before the standard things so we still have the player object
            chatSystem.CmdOnPlayerConnectOrDisconnect();
            base.OnClientDisconnect(conn);
        }
     }

     * Then in the editor, set this script as the NetworkManager script.
     * After you have done that, uncomment the lines relating the cachedPlayers in this script
     * and it should be working.
     */

    private void CreatePrefabAndAddToScreen(ChatEntry message)
    {
        UIChatMessage newMessage = Instantiate(chatMessagePrefab);
        newMessage.GetComponent<RectTransform>().SetParent(contentPanel.GetComponent<RectTransform>(), false);

        newMessage.MessageText.text = "<color=\"" + GetHexValueForColor(chatChannels.Find(channel => channel.Channel == message.Channel).color) + "\">" + "(" + chatChannels.Find(channel => channel.Channel == message.Channel).Name + ") " + message.SenderName + ":</color> ";
        newMessage.MessageText.color = Color.white;
        newMessage.MessageText.text += EnableWordFilter ? ReplaceFilteredWords(message.Message) : message.Message;
        messagesOnUI.Add(newMessage);

        //this will try to hide the chat after 10 seconds. If a new message comes in, the timeLastChatEntryHappened will be updated so still we should have DELAY_BEFORE_HIDING_CHAT seconds before it hides
        Debug.Log("Going to invoke TryToHideChat in " + DELAY_BEFORE_HIDING_CHAT + " seconds. @(" + (Time.time + DELAY_BEFORE_HIDING_CHAT) + ")");
        Invoke("TryToHideChat", DELAY_BEFORE_HIDING_CHAT);
        timeLastChatEntryHappened = Time.time;

        //frequently the last message is not properly scrolled into view due to some internals of Unity UI, putting a brief delay ensures proper scrolling
        Invoke("ScrollToBottom", 0.15f);
    }

    private void ScrollToBottom()
    {
        chatPanelIdentifier.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
    }

    //some magic to convert the rgb to hex value we can use in RichText (eliminates need for secondary text element for player name)
    private string GetHexValueForColor(Color color)
    {
        string hexValue = "#";
        float redFloat = color.r * 255f;
        float greenFloat = color.g * 255f;
        float blueFloat = color.b * 255f;

        hexValue += GetHex(Mathf.FloorToInt(redFloat / 16)) + GetHex(Mathf.FloorToInt(redFloat) % 16) + GetHex(Mathf.FloorToInt(greenFloat / 16)) + GetHex(Mathf.FloorToInt(greenFloat) % 16) + GetHex(Mathf.FloorToInt(blueFloat / 16)) + GetHex(Mathf.FloorToInt(blueFloat) % 16);

        return hexValue;
    }

    //helper for the above
    private string GetHex(int value)
    {
        return hexValues[value].ToString();
    }

    public void UpdateChatMessages()
    {
        //again this is a performance improvement, see above commented section for usage
        //PlayerController playerController = cachedPlayers.Find(playerControllerId => player.isLocalPlayer);
        PlayerController2 playerController = new List<PlayerController2>(GameObject.FindObjectsOfType<PlayerController2>()).Find(player => player.isLocalPlayer);
        //perhaps send playerController to ReactivatePlayerAndDeselectInputField() so you don't have to do the above search again.
        ReactivatePlayerAndDeselectInputField();

        if (chatPanelIdentifier.InputField.text != "")
        {
            bool isCommand = false;
            foreach (Command command in Commands)
            {
                if (chatPanelIdentifier.InputField.text.StartsWith("/" + command.Name))
                {
                    isCommand = true;
                    command.FunctionToCall.Invoke();

                    //if we've selected to send the message after FunctionToCall.Invoke(), we will try to send it. But if the user didn't type anything after the command, then it doesn't make sense to send an empty message 
                    if (command.CallFunctionThenSendMessage && !string.IsNullOrEmpty(chatPanelIdentifier.InputField.text.Trim()))
                    {
                        isCommand = false; //setting this to false will ensure the chat message is sent, since as you can see below, if(!isCommand) is required to send a message.
                        chatPanelIdentifier.InputField.text = chatPanelIdentifier.InputField.text.Substring(command.Name.Length + 1).Trim(); //trimming it clears any additional spaces left at the beginning or end
                    }

                    break;
                }
            }

            if (!isCommand)
            {
                string localPlayerName = "Player";
                if (playerController != null)
                {
                    localPlayerName = playerController.name;
                }

                ChatEntry entryToSend = new ChatEntry();

                entryToSend.Channel = channelToSend;
                entryToSend.Message = chatPanelIdentifier.InputField.text;
                entryToSend.SenderName = localPlayerName;

                networkClient.Send(messageChannel, new ChatMessage(entryToSend));
            }

            chatPanelIdentifier.InputField.text = "";
        }

        //this will try to hide the chat after DELAY_BEFORE_HIDING_CHAT seconds. If a new message comes in, the timeLastChatEntryHappened will be updated so still we should have DELAY_BEFORE_HIDING_CHAT seconds before it hides
        Invoke("TryToHideChat", DELAY_BEFORE_HIDING_CHAT);
        timeLastChatEntryHappened = Time.time;
    }

    public void ToggleWordFilter(bool enabled)
    {
        EnableWordFilter = enabled;
    }

    private string ReplaceFilteredWords(string rawText)
    {
        string replacedText = rawText;

        foreach (WordFilter filter in WordFilters)
        {
            if (filter.regex.IsMatch(replacedText))
            {
                replacedText = filter.regex.Replace(replacedText, filter.ReplaceWith);
            }
        }

        return replacedText;
    }

    private void ReactivatePlayerAndDeselectInputField()
    {
        chatPanelIdentifier.InputField.DeactivateInputField();
        //perhaps you would like to re-activate player's ability to control here. Should be the inverse of what OpenChat() does
    }

    private void TryToHideChat()
    {
        if (Time.time >= ((timeLastChatEntryHappened + DELAY_BEFORE_HIDING_CHAT) - LIENIENCY) && !chatPanelIdentifier.InputField.isFocused)
        {
            ForceCloseChat();
        }
    }

    public void ForceCloseChat()
    {
        lerpAlphaOfChat = true;
        targetAlpha = 0;
        EventSystem.current.SetSelectedGameObject(null);

        ReactivatePlayerAndDeselectInputField();
    }


    //legacy way to open chat. By specifying a channel (see OpenChat(bool, int)) the user is greeted with a message indicating where the message will be sent.
    public void OpenChat(bool focusInputField)
    {
        chatPanelIdentifier.InputField.placeholder.GetComponent<Text>().text = "Enter message...";
        lerpAlphaOfChat = true;
        targetAlpha = 1;
        if (focusInputField)
        {
            chatPanelIdentifier.InputField.ActivateInputField();
            chatPanelIdentifier.InputField.Select();
        }

        //perhaps disable your player's ability to move (keyboard input)?
    }

    //This is now the preferred way to open the chat. Specify the channel (a valid one) and we will notify the user which channel name their message will go to.
    public void OpenChat(bool focusInputField, uint channel)
    {
        //note that with 5.3 there is some undesirable behavior here. The message is updated, but placeholders disappear before user starts typing.
        //This was fixed in 5.4. However the chat system is still working fine on 5.3 so I didn't want to force a version upgrade on anyone currently on 5.3. 
        //This feature is quirky until you upgrade to 5.4.
        chatPanelIdentifier.InputField.placeholder.GetComponent<Text>().text = "Enter message (" + chatChannels.Find(chatChannel => chatChannel.Channel == channel).Name + ")...";
        chatPanelIdentifier.InputField.ForceLabelUpdate();

        channelToSend = channel;

        lerpAlphaOfChat = true;
        targetAlpha = 1;
        if (focusInputField)
        {
            chatPanelIdentifier.InputField.ActivateInputField();
            chatPanelIdentifier.InputField.Select();
            //perhaps disable your player's ability to move (keyboard input)?
        }
    }

    //Use this to target a specific, known channel
    public void ChangeTargetChannel(int channel)
    {
        OpenChat(true, (uint)channel);
    }

    //In the example, this is what we call to reach current user's team chat. Since there are 3 team chat channels, we need to find which one the current user is in.
    public void ChangeToCurrentUserTeamChannel()
    {
        //If you are doing the more sophisticated setup with cachedPlayers, you can do something like this:
        /*
         * PlayerController currentPlayer = cachedPlayers.Find((player) => player.isLocalPlayer);
         * OpenChat(true, currentPlayer.teamIndex);
         * chatPanelIdentifier.InputField.text = ""; //this clears the text from your command
         */
        //You may also want to do the check as is done below to ensure you don't get errors if the player disconnects or cache is out of sync for some reason

        //for simpler setups, this is ok, but is not efficient.
        PlayerController2 currentPlayer = new List<PlayerController2>(GameObject.FindObjectsOfType<PlayerController2>()).Find((player) => player.isLocalPlayer);
        if (currentPlayer != null)
        {
            OpenChat(false, currentPlayer.teamIndex);
            string text = chatPanelIdentifier.InputField.text.Trim();
            if (!text.Contains(" ")) //if user types "/team " or "/team" then presses enter, we should clear the text, but if they type "/team hello" then we should send hello to that channel.
            {
                chatPanelIdentifier.InputField.text = "";
            }
        }
        else
        {
            Debug.LogWarning("Unable to find local player!");
        }
    }

    private void Update()
    {
        if (lerpAlphaOfChat)
        {
            //for instantaneous visibility, you can just set canvasGroup.alpha = targetAlpha
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * FADE_SPEED);

            if (canvasGroup.alpha < 0.001f || canvasGroup.alpha > 0.999f)
            {
                lerpAlphaOfChat = false;
            }
        }
    }

    public void GenerateHelp()
    {
        UIChatMessage newMessage = Instantiate(chatMessagePrefab);
        newMessage.GetComponent<RectTransform>().SetParent(contentPanel.GetComponent<RectTransform>(), false);

        newMessage.MessageText.color = Color.yellow;
        newMessage.MessageText.text = "Help content here.\n<b>/help</b> to view this info again.\n<b>/team</b> to change your message target to team chat\n<b>/all</b> to change your message target to all chat\nTo call some other function, add it to the <b><i>Commands</i></b> list on the Unity Inspector and tie it to some function.\nBecause this Chat Entry has <b>Rich Text</b> support, you can do\n<size=24>a lot</size> of <size=33><color=orange>f</color><color=red>a</color><color=lime>n</color><color=cyan>c</color><color=fuchsia>y</color> formatting</size> here as well.";
        messagesOnUI.Add(newMessage);

        //this will try to hide the chat after 10 seconds. If a new message comes in, the timeLastChatEntryHappened will be updated so still we should have DELAY_BEFORE_HIDING_CHAT seconds before it hides
        Debug.Log("Going to invoke TryToHideChat in " + DELAY_BEFORE_HIDING_CHAT + " seconds. @(" + (Time.time + DELAY_BEFORE_HIDING_CHAT) + ")");
        Invoke("TryToHideChat", DELAY_BEFORE_HIDING_CHAT);
        timeLastChatEntryHappened = Time.time;

        //frequently the last message is not properly scrolled into view due to some internals of Unity UI, putting a brief delay ensures proper scrolling
        Invoke("ScrollToBottom", 0.15f);
    }

    //allows easier editor modifications of Chat Channels
    [Serializable]
    public struct ChatChannel
    {
        public string Name;
        public Color color;
        public uint Channel;
    }

    [Serializable]
    public class WordFilter
    {
        public string RegularExpression; //this is the regular expression that will be tested against
        public string ReplaceWith; //this is what we will replace the text we match with. For example if you block "cats", and this is set to "****" we will put "****" in place of "cats"
        public bool IgnoreCase; //if we should ignore case of the match
        public Regex regex;
        /*
         * Example setup
         * Filter = new Regex("cats")
         * ReplaceWith = "*"
         * StaticSizeReplace = false
         * 
         * User inputs: "I love cats!"
         * Message sent over network: "I love ****!"
         * 
         * Example setup 2
         * Filter = new Regex("cats")
         * ReplaceWith = "*"
         * StaticSizeReplace = true
         * StaticReplacementSize = 10
         * 
         * User inputs: "I love cats!"
         * Message sent over network: "I love **********!"
         * 
         */
    }

    [Serializable]
    public struct Command
    {
        public string Name; //this is the text the user will have to enter to execute this command 
        public UnityEvent FunctionToCall; //This is the function to execute when user enters a designated command
        public bool CallFunctionThenSendMessage; //If this is true, the /[name] will be stripped out of the message, and sent after FunctionToCall.Invoke() has been called.
    }

    //if you for some reason need more data transferred, you will probably need to do some reading, maybe here is a good starting point: http://docs.unity3d.com/Manual/UNetStateSync.html
    /*
        note that you should probably limit both:
            message length (capped to 150 in demo, by InputField) and 
            sender name
        or you will end up sending a ton of data at once if someone sends a really long message, or sets a really long name
    */
    private struct ChatEntry
    {
        public string Message;
        public uint Channel;
        public string SenderName;
    }

    //unless you know what you are doing, I would avoid touching this
    private class ChatMessage : MessageBase
    {
        public ChatEntry entry;

        public ChatMessage(ChatEntry entry)
        {
            this.entry = entry;
        }

        public ChatMessage()
        {
            entry = new ChatEntry();
        }

        public override void Serialize(NetworkWriter writer)
        {
            writer.WritePackedUInt32(entry.Channel);
            writer.Write(entry.Message);
            writer.Write(entry.SenderName);
        }

        public override void Deserialize(NetworkReader reader)
        {
            entry.Channel = reader.ReadPackedUInt32();
            entry.Message = reader.ReadString();
            entry.SenderName = reader.ReadString();
        }

    }

    //you can't directly just do SyncListStruct<YourClass>
    private class SyncListChatMessage : SyncListStruct<ChatEntry> { }
}

#if UNITY_EDITOR
//Custom Drawer for ChatChannels. This is exclusively to show the Inspector nicer inside the Unity Editor
[CustomEditor(typeof(ChatSystem.ChatChannel))]
public class ChatChannelEditor : Editor
{
    SerializedProperty Name;
    SerializedProperty color;
    SerializedProperty Channel;

    void OnEnable()
    {
        // Setup the SerializedProperties.
        Name = serializedObject.FindProperty("Name");
        color = serializedObject.FindProperty("color");
        Channel = serializedObject.FindProperty("Channel");
    }

    void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        this.name = Name.ToString();
        Rect channelRect = new Rect(position.x, position.y, position.width - 90, position.height);
        Rect nameRect = new Rect(position.x + 35, position.y, 30, position.height);
        Rect colorRect = new Rect(position.x + 90, position.y, 50, position.height);

        EditorGUI.PropertyField(channelRect, Channel, GUIContent.none);
        EditorGUI.PropertyField(nameRect, Name, GUIContent.none);
        EditorGUI.PropertyField(colorRect, color, GUIContent.none);

        EditorGUI.EndProperty();
    }
}

//Custom drawer for Word Filter, very similar to above.
[CustomEditor(typeof(ChatSystem.WordFilter))]
public class WordFilterEditor : Editor
{
    SerializedProperty RegularExpression;
    SerializedProperty ReplaceWith;
    SerializedProperty IgnoreCase;

    void OnEnable()
    {
        RegularExpression = serializedObject.FindProperty("RegularExpression");
        ReplaceWith = serializedObject.FindProperty("ReplaceWith");
        IgnoreCase = serializedObject.FindProperty("IgnoreCase");
    }

    void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        this.name = RegularExpression.ToString();
        Rect RegularExpressionRect = new Rect(position.x, position.y, position.width - 90, position.height);
        Rect ReplaceWithRect = new Rect(position.x + 35, position.y, 30, position.height);
        Rect IgnoreCaseRect = new Rect(position.x + 70, position.y, 30, position.height);

        EditorGUI.PropertyField(RegularExpressionRect, RegularExpression, GUIContent.none);
        EditorGUI.PropertyField(ReplaceWithRect, ReplaceWith, GUIContent.none);
        EditorGUI.PropertyField(IgnoreCaseRect, IgnoreCase, GUIContent.none);

        EditorGUI.EndProperty();
    }
}

//Custom drawer for Commands, very similar to above.
[CustomEditor(typeof(ChatSystem.WordFilter))]
public class CommandEditor : Editor
{
    SerializedProperty Name;
    SerializedProperty FunctionToCall;
    SerializedProperty CallFunctionThenSendMessage;

    void OnEnable()
    {
        // Setup the SerializedProperties.
        Name = serializedObject.FindProperty("Name");
        FunctionToCall = serializedObject.FindProperty("FunctionToCall");
        CallFunctionThenSendMessage = serializedObject.FindProperty("CallFunctionThenSendMessage");
    }

    void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        this.name = Name.ToString();
        Rect NameRect = new Rect(position.x, position.y, position.width - 90, position.height);
        Rect FunctionToCallRect = new Rect(position.x + 30, position.y, 30, position.height);
        Rect CallFunctionThenSendMessageRect = new Rect(position.x + 30, position.y, 30, position.height);

        EditorGUI.PropertyField(NameRect, Name, GUIContent.none);
        EditorGUI.PropertyField(FunctionToCallRect, FunctionToCall, GUIContent.none);
        EditorGUI.PropertyField(CallFunctionThenSendMessageRect, CallFunctionThenSendMessage, GUIContent.none);

        EditorGUI.EndProperty();
    }
}
#endif
