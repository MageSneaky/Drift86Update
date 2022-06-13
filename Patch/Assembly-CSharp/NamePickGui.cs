using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ChatGui))]
public class NamePickGui : MonoBehaviour
{
	public void Start()
	{
		this.chatNewComponent = Object.FindObjectOfType<ChatGui>();
		string @string = PlayerPrefs.GetString("NamePickUserName");
		if (!string.IsNullOrEmpty(@string))
		{
			this.idInput.text = @string;
		}
	}

	public void EndEditOnEnter()
	{
		if (Input.GetKey(13) || Input.GetKey(271))
		{
			this.StartChat();
		}
	}

	public void StartChat()
	{
		ChatGui chatGui = Object.FindObjectOfType<ChatGui>();
		if (PlayerPrefs.HasKey("DisplayName"))
		{
			chatGui.UserName = PlayerPrefs.GetString("DisplayName");
		}
		else
		{
			chatGui.UserName = PlayerPrefs.GetString("MyName");
		}
		chatGui.Connect();
		base.enabled = false;
		PlayerPrefs.SetString("NamePickUserName", chatGui.UserName);
	}

	public void CheckFoStart()
	{
		if (this.UIFP.activeSelf)
		{
			this.StartChat();
			return;
		}
		this.inputfieldinput.GetComponent<InputField>().Select();
		this.inputfieldinput.GetComponent<InputField>().interactable = false;
		this.inputfieldinput.GetComponent<InputField>().interactable = true;
		this.inputfieldinput.GetComponent<InputField>().Select();
	}

	private const string UserNamePlayerPref = "NamePickUserName";

	public ChatGui chatNewComponent;

	public InputField idInput;

	public GameObject CanvasChat;

	public GameObject UIFP;

	public GameObject inputfieldinput;
}
