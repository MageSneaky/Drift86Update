using System;
using GameBalance;
using UnityEngine;

public class LockedContent : ScriptableObject
{
	public LockedContent.UnlockType GetUnlockType
	{
		get
		{
			return this.Unlock;
		}
	}

	public int GetPrice
	{
		get
		{
			return this.Price;
		}
	}

	public TrackPreset GetCompleteTrackForUnlock
	{
		get
		{
			return this.CompleteTrackForUnlock;
		}
	}

	public bool IsUnlocked
	{
		get
		{
			LockedContent.UnlockType unlock = this.Unlock;
			if (unlock != LockedContent.UnlockType.UnlockByMoney)
			{
				return unlock != LockedContent.UnlockType.UnlockByTrackComleted || PlayerProfile.TrackIsComplited(this.CompleteTrackForUnlock);
			}
			return PlayerProfile.ObjectIsBought(this);
		}
	}

	public bool CanUnlock
	{
		get
		{
			return this.Unlock == LockedContent.UnlockType.UnlockByMoney && PlayerProfile.Money >= this.Price;
		}
	}

	public bool TryUnlock()
	{
		if (this.Unlock == LockedContent.UnlockType.UnlockByMoney && PlayerProfile.Money >= this.Price)
		{
			PlayerProfile.Money -= this.Price;
			PlayerProfile.SetObjectAsBought(this);
			return true;
		}
		return false;
	}

	[SerializeField]
	[HideInInspector]
	protected LockedContent.UnlockType Unlock;

	[SerializeField]
	[HideInInspector]
	protected int Price;

	[SerializeField]
	[HideInInspector]
	protected TrackPreset CompleteTrackForUnlock;

	public enum UnlockType
	{
		Unlocked,
		UnlockByMoney,
		UnlockByTrackComleted
	}
}
