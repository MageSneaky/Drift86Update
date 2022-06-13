using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class ChatGui : MonoBehaviour, IChatClientListener
{
	public string UserName { get; set; }

	public void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		this.stopsendmsg = 0;
		this.UserIdText.text = "";
		this.StateText.text = "";
		this.StateText.gameObject.SetActive(true);
		this.UserIdText.gameObject.SetActive(true);
		this.Title.SetActive(true);
		this.ChatPanel.gameObject.SetActive(false);
		this.ConnectingLabel.SetActive(false);
		if (string.IsNullOrEmpty(this.UserName))
		{
			this.UserName = "user" + Environment.TickCount % 99;
		}
		this.chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings;
		bool flag = !string.IsNullOrEmpty(this.chatAppSettings.AppIdChat);
		this.missingAppIdErrorPanel.SetActive(!flag);
		this.UserIdFormPanel.gameObject.SetActive(flag);
		if (!flag)
		{
			Debug.LogError("You need to set the chat app ID in the PhotonServerSettings file in order to continue.");
		}
	}

	public void Connect()
	{
		this.UserIdFormPanel.gameObject.SetActive(false);
		this.chatClient = new ChatClient(this, ConnectionProtocol.Udp);
		if (PlayerPrefs.GetString("Settings_MyRegion") == "sa")
		{
			this.chatClient.ChatRegion = "us";
		}
		else
		{
			this.chatClient.ChatRegion = PlayerPrefs.GetString("Settings_MyRegion");
		}
		this.chatClient.UseBackgroundWorkerForSending = true;
		this.chatClient.Connect(this.chatAppSettings.AppIdChat, "1.0", new Photon.Chat.AuthenticationValues(this.UserName));
		this.ChannelToggleToInstantiate.gameObject.SetActive(false);
		Debug.Log("Connecting as: " + this.UserName);
		this.ConnectingLabel.SetActive(true);
	}

	public void OnDestroy()
	{
		if (this.chatClient != null)
		{
			this.chatClient.Disconnect();
		}
	}

	public void OnApplicationQuit()
	{
		if (this.chatClient != null)
		{
			this.chatClient.Disconnect();
		}
	}

	public void Update()
	{
		if (this.chatClient != null)
		{
			this.chatClient.Service();
		}
		if (this.StateText == null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		this.StateText.gameObject.SetActive(this.ShowState);
	}

	public void OnEnterSend()
	{
		if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
		{
			this.SendChatMessage(this.InputFieldChat.text);
			this.InputFieldChat.text = "";
		}
	}

	public void OnClickSend()
	{
		if (this.InputFieldChat != null)
		{
			this.SendChatMessage(this.InputFieldChat.text);
			this.InputFieldChat.text = "";
		}
	}

	private void SendChatMessage(string inputLine)
	{
		if (string.IsNullOrEmpty(inputLine))
		{
			return;
		}
		if ("test".Equals(inputLine))
		{
			if (this.TestLength != this.testBytes.Length)
			{
				this.testBytes = new byte[this.TestLength];
			}
			this.chatClient.SendPrivateMessage(this.chatClient.AuthValues.UserId, this.testBytes, true);
		}
		bool flag = this.chatClient.PrivateChannels.ContainsKey(this.selectedChannelName);
		string target = string.Empty;
		if (flag)
		{
			target = this.selectedChannelName.Split(new char[]
			{
				':'
			})[1];
		}
		if (inputLine[0].Equals('\\'))
		{
			string[] array = inputLine.Split(new char[]
			{
				' '
			}, 2);
			if (array[0].Equals("\\help"))
			{
				this.PostHelpToCurrentChannel();
			}
			if (array[0].Equals("\\state"))
			{
				int num = 0;
				List<string> list = new List<string>();
				list.Add("i am state " + num);
				string[] array2 = array[1].Split(new char[]
				{
					' ',
					','
				});
				if (array2.Length != 0)
				{
					num = int.Parse(array2[0]);
				}
				if (array2.Length > 1)
				{
					list.Add(array2[1]);
				}
				this.chatClient.SetOnlineStatus(num, list.ToArray());
				return;
			}
			if ((array[0].Equals("\\subscribe") || array[0].Equals("\\s")) && !string.IsNullOrEmpty(array[1]))
			{
				this.chatClient.Subscribe(array[1].Split(new char[]
				{
					' ',
					','
				}));
				return;
			}
			if ((array[0].Equals("\\unsubscribe") || array[0].Equals("\\u")) && !string.IsNullOrEmpty(array[1]))
			{
				this.chatClient.Unsubscribe(array[1].Split(new char[]
				{
					' ',
					','
				}));
				return;
			}
			if (array[0].Equals("\\clear"))
			{
				if (flag)
				{
					this.chatClient.PrivateChannels.Remove(this.selectedChannelName);
					return;
				}
				ChatChannel chatChannel;
				if (this.chatClient.TryGetChannel(this.selectedChannelName, flag, out chatChannel))
				{
					chatChannel.ClearMessages();
					return;
				}
			}
			else if (array[0].Equals("\\msg") && !string.IsNullOrEmpty(array[1]))
			{
				string[] array3 = array[1].Split(new char[]
				{
					' ',
					','
				}, 2);
				if (array3.Length < 2)
				{
					return;
				}
				string target2 = array3[0];
				string message = array3[1];
				this.chatClient.SendPrivateMessage(target2, message, false);
				return;
			}
			else
			{
				if ((!array[0].Equals("\\join") && !array[0].Equals("\\j")) || string.IsNullOrEmpty(array[1]))
				{
					Debug.Log("The command '" + array[0] + "' is invalid.");
					return;
				}
				string[] array4 = array[1].Split(new char[]
				{
					' ',
					','
				}, 2);
				if (this.channelToggles.ContainsKey(array4[0]))
				{
					this.ShowChannel(array4[0]);
					return;
				}
				this.chatClient.Subscribe(new string[]
				{
					array4[0]
				});
				return;
			}
		}
		else
		{
			if (flag)
			{
				this.chatClient.SendPrivateMessage(target, inputLine, false);
				return;
			}
			this.chatClient.PublishMessage(this.selectedChannelName, inputLine, false);
		}
	}

	public void PostHelpToCurrentChannel()
	{
		Text currentChannelText = this.CurrentChannelText;
		currentChannelText.text += ChatGui.HelpText;
	}

	public void DebugReturn(DebugLevel level, string message)
	{
		if (level == DebugLevel.ERROR)
		{
			Debug.LogError(message);
			return;
		}
		if (level == DebugLevel.WARNING)
		{
			Debug.LogWarning(message);
			return;
		}
		Debug.Log(message);
	}

	public void OnConnected()
	{
		if (this.ChannelsToJoinOnConnect != null && this.ChannelsToJoinOnConnect.Length != 0)
		{
			this.chatClient.Subscribe(this.ChannelsToJoinOnConnect, this.HistoryLengthToFetch);
		}
		this.ConnectingLabel.SetActive(false);
		this.UserIdText.text = "Connected as " + this.UserName;
		this.ChatPanel.gameObject.SetActive(true);
		if (this.FriendsList != null && this.FriendsList.Length != 0)
		{
			this.chatClient.AddFriends(this.FriendsList);
			foreach (string text in this.FriendsList)
			{
				if (this.FriendListUiItemtoInstantiate != null && text != this.UserName)
				{
					this.InstantiateFriendButton(text);
				}
			}
		}
		if (this.FriendListUiItemtoInstantiate != null)
		{
			this.FriendListUiItemtoInstantiate.SetActive(false);
		}
		this.chatClient.SetOnlineStatus(2);
	}

	public void OnDisconnected()
	{
		this.ConnectingLabel.SetActive(false);
	}

	public void OnChatStateChange(ChatState state)
	{
		this.StateText.text = state.ToString();
	}

	public void OnSubscribed(string[] channels, bool[] results)
	{
		foreach (string channelName in channels)
		{
			if (this.stopsendmsg == 0)
			{
				this.chatClient.PublishMessage(channelName, "says 'hi'.", false);
			}
			this.stopsendmsg = 1;
			if (this.ChannelToggleToInstantiate != null)
			{
				this.InstantiateChannelButton(channelName);
			}
		}
		this.ShowChannel(channels[0]);
	}

	private void InstantiateChannelButton(string channelName)
	{
		if (this.channelToggles.ContainsKey(channelName))
		{
			Debug.Log("Skipping creation for an existing channel toggle.");
			return;
		}
		Toggle toggle = UnityEngine.Object.Instantiate<Toggle>(this.ChannelToggleToInstantiate);
		toggle.gameObject.SetActive(true);
		toggle.GetComponentInChildren<ChannelSelector>().SetChannel(channelName);
		toggle.transform.SetParent(this.ChannelToggleToInstantiate.transform.parent, false);
		this.channelToggles.Add(channelName, toggle);
	}

	private void InstantiateFriendButton(string friendId)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.FriendListUiItemtoInstantiate);
		gameObject.gameObject.SetActive(true);
		FriendItem component = gameObject.GetComponent<FriendItem>();
		component.FriendId = friendId;
		gameObject.transform.SetParent(this.FriendListUiItemtoInstantiate.transform.parent, false);
		this.friendListItemLUT[friendId] = component;
	}

	public void OnUnsubscribed(string[] channels)
	{
		foreach (string text in channels)
		{
			if (this.channelToggles.ContainsKey(text))
			{
				UnityEngine.Object.Destroy(this.channelToggles[text].gameObject);
				this.channelToggles.Remove(text);
				Debug.Log("Unsubscribed from channel '" + text + "'.");
				if (text == this.selectedChannelName && this.channelToggles.Count > 0)
				{
					IEnumerator<KeyValuePair<string, Toggle>> enumerator = this.channelToggles.GetEnumerator();
					enumerator.MoveNext();
					KeyValuePair<string, Toggle> keyValuePair = enumerator.Current;
					this.ShowChannel(keyValuePair.Key);
					keyValuePair = enumerator.Current;
					keyValuePair.Value.isOn = true;
				}
			}
			else
			{
				Debug.Log("Can't unsubscribe from channel '" + text + "' because you are currently not subscribed to it.");
			}
		}
	}

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
		if (channelName.Equals(this.selectedChannelName))
		{
			this.ShowChannel(this.selectedChannelName);
		}
	}

	public void OnPrivateMessage(string sender, object message, string channelName)
	{
		this.InstantiateChannelButton(channelName);
		byte[] array = message as byte[];
		if (array != null)
		{
			Debug.Log("Message with byte[].Length: " + array.Length);
		}
		if (this.selectedChannelName.Equals(channelName))
		{
			this.ShowChannel(channelName);
		}
	}

	public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
	{
		Debug.LogWarning("status: " + string.Format("{0} is {1}. Msg:{2}", user, status, message));
		if (this.friendListItemLUT.ContainsKey(user))
		{
			FriendItem friendItem = this.friendListItemLUT[user];
			if (friendItem != null)
			{
				friendItem.OnFriendStatusUpdate(status, gotMessage, message);
			}
		}
	}

	public void OnUserSubscribed(string channel, string user)
	{
		Debug.LogFormat("OnUserSubscribed: channel=\"{0}\" userId=\"{1}\"", new object[]
		{
			channel,
			user
		});
	}

	public void OnUserUnsubscribed(string channel, string user)
	{
		Debug.LogFormat("OnUserUnsubscribed: channel=\"{0}\" userId=\"{1}\"", new object[]
		{
			channel,
			user
		});
	}

	public void AddMessageToSelectedChannel(string msg)
	{
		ChatChannel chatChannel = null;
		if (!this.chatClient.TryGetChannel(this.selectedChannelName, out chatChannel))
		{
			Debug.Log("AddMessageToSelectedChannel failed to find channel: " + this.selectedChannelName);
			return;
		}
		if (chatChannel != null)
		{
			chatChannel.Add("Bot", msg, 0);
		}
	}

	public void ShowChannel(string channelName)
	{
		if (string.IsNullOrEmpty(channelName))
		{
			return;
		}
		ChatChannel chatChannel = null;
		if (!this.chatClient.TryGetChannel(channelName, out chatChannel))
		{
			Debug.Log("ShowChannel failed to find channel: " + channelName);
			return;
		}
		this.selectedChannelName = channelName;
		this.CurrentChannelText.text = chatChannel.ToStringMessages();
		foreach (KeyValuePair<string, Toggle> keyValuePair in this.channelToggles)
		{
			keyValuePair.Value.isOn = (keyValuePair.Key == channelName);
		}
	}

	public void OpenDashboard()
	{
		Application.OpenURL("https://dashboard.photonengine.com");
	}

	public string[] ChannelsToJoinOnConnect;

	public string[] FriendsList;

	public int HistoryLengthToFetch;

	private string selectedChannelName;

	public ChatClient chatClient;

	protected internal AppSettings chatAppSettings;

	public GameObject missingAppIdErrorPanel;

	public GameObject ConnectingLabel;

	public RectTransform ChatPanel;

	public GameObject UserIdFormPanel;

	public InputField InputFieldChat;

	public Text CurrentChannelText;

	public Toggle ChannelToggleToInstantiate;

	private int stopsendmsg;

	public GameObject FriendListUiItemtoInstantiate;

	private readonly Dictionary<string, Toggle> channelToggles = new Dictionary<string, Toggle>();

	private readonly Dictionary<string, FriendItem> friendListItemLUT = new Dictionary<string, FriendItem>();

	public bool ShowState = true;

	public GameObject Title;

	public Text StateText;

	public Text UserIdText;

	private static string HelpText = "\n    -- HELP --\nTo subscribe to channel(s) (channelnames are case sensitive) :  \n\t<color=#E07B00>\\subscribe</color> <color=green><list of channelnames></color>\n\tor\n\t<color=#E07B00>\\s</color> <color=green><list of channelnames></color>\n\nTo leave channel(s):\n\t<color=#E07B00>\\unsubscribe</color> <color=green><list of channelnames></color>\n\tor\n\t<color=#E07B00>\\u</color> <color=green><list of channelnames></color>\n\nTo switch the active channel\n\t<color=#E07B00>\\join</color> <color=green><channelname></color>\n\tor\n\t<color=#E07B00>\\j</color> <color=green><channelname></color>\n\nTo send a private message: (username are case sensitive)\n\t\\<color=#E07B00>msg</color> <color=green><username></color> <color=green><message></color>\n\nTo change status:\n\t\\<color=#E07B00>state</color> <color=green><stateIndex></color> <color=green><message></color>\n<color=green>0</color> = Offline <color=green>1</color> = Invisible <color=green>2</color> = Online <color=green>3</color> = Away \n<color=green>4</color> = Do not disturb <color=green>5</color> = Looking For Group <color=green>6</color> = Playing\n\nTo clear the current chat tab (private chats get closed):\n\t<color=#E07B00>\\clear</color>";

	public int TestLength = 2048;

	private byte[] testBytes = new byte[2048];
}
