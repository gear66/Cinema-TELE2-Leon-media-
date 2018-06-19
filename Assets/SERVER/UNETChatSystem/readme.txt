If you are upgrading from version 1.0 to 1.1 please read the changelog - there are some significant changes to the setup which allow easier channel management

Note the scripts in Scripts/Sample directory were created quickly for the demo scene. The primary ChatSystem assets are contained within Scripts/ChatSystem directory.
There will be a small amount of integration work required. PlayerController references in ChatSystem should be replaced with whatever script you have for Player control and "Team" identification.
Within ChatSystem#OpenChat and ChatSystem#ReactivatePlayerAndDeselectInputField you may want to disable or enable (respectively) your player's moving and performing actions. 
An isometric mouse moving game it may not be necessary. First or third person games are more likely to desire this.

To use the Chat System:
1. Import package into your Unity 5 Project.
2. Drag Chat Panel Prefab to your desired location on a Canvas
3. Integrate with your Player object, something similar to this, perhaps (see PlayerController.cs):
		if (Input.GetKeyUp(KeyCode.Y) && (!ChatSystemIsOpen() || chatIdentifier.InputField.text == ""))
        {
            chatSystem.OpenChat(true, 0);
            chatSystem.messageIsForAllChat = true;
        }
        else if (Input.GetKeyUp(KeyCode.U) && (!ChatSystemIsOpen() || chatIdentifier.InputField.text == ""))
        {
            chatSystem.OpenChat(true, currentTeamChatChannel);
            chatSystem.messageIsForAllChat = false;
        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            chatSystem.ForceCloseChat();
        }
		
4. Customize ChatSystem Channels as desired. Colors are shown in editor. 
  * For different channel names, as shown in () after player name, you may edit ChatSystem#GetTeamName
  * To add new channels, you may add cases to ChatSystem#GetTeamName and ChatSystem#GetColorFromTeam. I recommend following the existing pattern for the channels included.
    * If you wanted to get fancy you could use a the Strategy pattern for channel selection if you have a large number

5. OPTIONAL - Configure Word Filters (No code required, regular expression knowledge is beneficial)
  * Under the Word Filters list in the Inspector, adjust the size of the list
  * You will be presented with 3 inputs: Regular Expression, Replace With, and Ignore Case
    * Regular Expression - this can be a word like "cats", or a complex regular expression. I have provided a sample of both regular expression and simple word matching in the default prefab.
	* Replace With - this is the text that will be inserted in place of your "Regular Expression" bad word
	* Ignore Case - this is useful to more easily catch people doing CaTs to get around your "cats" filter
  * Add as many as you like!
6. OPTIONAL - Commands (Some coding required since they will execute arbitrary functions, similar to the default UI Controls in Unity)
  * Under Commands, increase the list size to however many commands you want to add
  * You will be presented with 3 options: Name, Function To Call, and Call Function Then Send Message
    * Name - this is the name of the command. Users will execute this command by typing /[name]. In the example there is "help", so user would type "/help" to activate this command
	* Function To Call - this is an arbitrary action to call when this command is matched. 
	* Call Function Then Send Message - True or False, if it is True, the Chat System will intelligently decide if it is necessary to send a message after this command. 
	  * In the demo scene this is used for dynamically changing chat channel targets. When a user types "/team hello team" it will change the target to that user's team chat channel, then send "hello team" to that channel.


You likely will also have to disable player input while typing, this can be achieved by disabling local player in ChatSystem#OpenChat method, and re-enabling their input in ChatSystem#ReactivatePlayerAndDeselectInputField

If for some reason you require additional information in your ChatMessages, the ChatMessage class within ChatSystem is extensible and it should be fairly clear how to add what you need from the existing code.
Currently ChatMessages support: 
	string Message - message player has sent
	uint Channel - that the message is sent on
	string SenderName - the name of the player who has sent the message
	
If you would like to extend this to, for exmaple always show the player's team in parenthesis you could do the following:
1. Add public int Team to ChatEntry struct
2. In ChatMessage#Serialize add: writer.Write(entry.Team);
3. In ChatMessage#Deserialize add: entry.Team = reader.ReadInt32();
4. Adjust prefab to how you wish for the team to be displayed, and within ChatSystem#CreatePrefabAndAddToScreen, populate it accordingly
5. Also adjust ChatEntry creation in ChatSystem#UpdateChatMessages


If you experience any issues please send an email to: support@llama.software