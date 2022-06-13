using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
	public static bool HasActiveMessageBox
	{
		get
		{
			return MessageBox.ActiveMessageBox != null;
		}
	}

	private void Awake()
	{
		if (!Application.isPlaying)
		{
			Object.DestroyImmediate(base.gameObject);
		}
		this.ApplyButton.onClick.AddListener(new UnityAction(this.OnApply));
		this.CancelButton.onClick.AddListener(new UnityAction(this.OnCancel));
	}

	public static void Show(string message)
	{
		MessageBox.ActiveMessageBox = Object.Instantiate<MessageBox>(B.ResourcesSettings.MessageBox);
		MessageBox.ActiveMessageBox.Init(message, null, null, "Apply", "OK");
	}

	public static void Show(string message, Action applyAction, Action cancelAction, string applyButtonText = "Apply", string cancelButtonText = "OK")
	{
		MessageBox.ActiveMessageBox = Object.Instantiate<MessageBox>(B.ResourcesSettings.MessageBox);
		MessageBox.ActiveMessageBox.Init(message, applyAction, cancelAction, applyButtonText, cancelButtonText);
	}

	private void Init(string message, Action applyAction = null, Action cancelAction = null, string applyButtonText = "Apply", string cancelButtonText = "OK")
	{
		this.MessageText.text = message;
		this.ApplyAction = applyAction;
		this.CancelAction = cancelAction;
		this.ApplyButtonText.text = applyButtonText;
		this.CancelButtonText.text = cancelButtonText;
		this.ApplyButton.SetActive(applyAction != null);
		this.CancelButton.Select();
	}

	public void OnApply()
	{
		this.ApplyAction.SafeInvoke();
		Object.Destroy(base.gameObject);
	}

	public void OnCancel()
	{
		this.CancelAction.SafeInvoke();
		Object.Destroy(base.gameObject);
	}

	[SerializeField]
	private TextMeshProUGUI MessageText;

	[SerializeField]
	private Button ApplyButton;

	[SerializeField]
	private Button CancelButton;

	[SerializeField]
	private TextMeshProUGUI ApplyButtonText;

	[SerializeField]
	private TextMeshProUGUI CancelButtonText;

	private Action ApplyAction;

	private Action CancelAction;

	private static MessageBox ActiveMessageBox;
}
