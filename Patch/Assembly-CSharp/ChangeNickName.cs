using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChangeNickName : Singleton<ChangeNickName>
{
	protected override void AwakeSingleton()
	{
		if (!PlayerPrefs.HasKey("nn"))
		{
			this.OnApplyNickname();
			this.Holder.SetActive(false);
		}
		else
		{
			this.OnApplyNickname();
			this.Holder.SetActive(false);
		}
		this.ApplyButton.onClick.AddListener(new UnityAction(this.OnApplyNickname));
		this.CancelButton.onClick.AddListener(new UnityAction(this.OnCancel));
	}

	public void Show()
	{
		this.InputField.text = PlayerProfile.NickName;
		this.Holder.SetActive(true);
	}

	private void OnApplyNickname()
	{
		PlayerProfile.NickName = PlayerPrefs.GetString("MyName");
		this.Holder.SetActive(false);
	}

	private void OnCancel()
	{
		this.Holder.SetActive(false);
	}

	[SerializeField]
	private GameObject Holder;

	[SerializeField]
	private Button ApplyButton;

	[SerializeField]
	private Button CancelButton;

	[SerializeField]
	private TMP_InputField InputField;
}
