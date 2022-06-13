using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChannelSelector : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public void SetChannel(string channel)
	{
		this.Channel = channel;
		base.GetComponentInChildren<Text>().text = this.Channel;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Object.FindObjectOfType<ChatGui>().ShowChannel(this.Channel);
	}

	public void OnPointerClick()
	{
		Object.FindObjectOfType<ChatGui>().ShowChannel(this.Channel);
	}

	public string Channel;
}
