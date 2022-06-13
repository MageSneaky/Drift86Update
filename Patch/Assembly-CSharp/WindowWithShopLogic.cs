using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WindowWithShopLogic : WindowWithShowHideAnimators
{
	protected virtual void OnSelect()
	{
	}

	protected void RefreshButtonState(LockedContent lockContent)
	{
		this.PriceHolder.SetActive(false);
		this.CompleteTrackHolder.SetActive(false);
		if (lockContent.IsUnlocked)
		{
			this.LockedHolder.SetActive(false);
			this.SetButtonState(this.UnlockedState, new UnityAction(this.OnSelect));
			this.SelectButton.interactable = true;
			return;
		}
		this.LockedHolder.SetActive(true);
		if (lockContent.GetUnlockType == LockedContent.UnlockType.UnlockByMoney)
		{
			this.PriceHolder.SetActive(true);
			this.PriceText.text = string.Format("${0}", lockContent.GetPrice);
			this.SelectButton.interactable = lockContent.CanUnlock;
			UnityAction onClickAction = delegate()
			{
				if (lockContent.TryUnlock())
				{
					this.RefreshButtonState(lockContent);
				}
			};
			this.SetButtonState(this.MoneyLockState, onClickAction);
			return;
		}
		this.CompleteTrackHolder.SetActive(true);
		this.CompleteTrackText.text = string.Format("{0}: {1}", lockContent.GetCompleteTrackForUnlock.TrackName, lockContent.GetCompleteTrackForUnlock.RegimeSettings.RegimeCaption);
		this.SetButtonState(this.TrackLockState, null);
		this.SelectButton.interactable = false;
	}

	private void SetButtonState(ButtonState state, UnityAction onClickAction = null)
	{
		this.SelectButton.onClick.RemoveAllListeners();
		if (onClickAction != null)
		{
			this.SelectButton.onClick.AddListener(onClickAction);
		}
		this.SelectButton.colors = state.ColorBlock;
		this.ButtonText.text = state.ButtonStr;
	}

	[Header("Shop logic")]
	[SerializeField]
	private GameObject LockedHolder;

	[SerializeField]
	private GameObject PriceHolder;

	[SerializeField]
	private TextMeshProUGUI PriceText;

	[SerializeField]
	private GameObject CompleteTrackHolder;

	[SerializeField]
	private TextMeshProUGUI CompleteTrackText;

	[SerializeField]
	private ButtonState UnlockedState;

	[SerializeField]
	private ButtonState MoneyLockState;

	[SerializeField]
	private ButtonState TrackLockState;

	[SerializeField]
	protected Button SelectButton;

	[SerializeField]
	private TextMeshProUGUI ButtonText;
}
