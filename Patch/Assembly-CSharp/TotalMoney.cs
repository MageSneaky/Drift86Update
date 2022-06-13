using System;
using TMPro;
using UnityEngine;

public class TotalMoney : MonoBehaviour
{
	private void Awake()
	{
		PlayerProfile.OnMoneyChanged = (Action<int>)Delegate.Combine(PlayerProfile.OnMoneyChanged, new Action<int>(this.SetMoney));
		this.SetMoney(PlayerProfile.Money);
	}

	private void Update()
	{
		this.TotalMoneyText.SetActive(Singleton<WindowsController>.Instance.HasWindowsHistory);
	}

	private void OnDestroy()
	{
		PlayerProfile.OnMoneyChanged = (Action<int>)Delegate.Remove(PlayerProfile.OnMoneyChanged, new Action<int>(this.SetMoney));
	}

	private void SetMoney(int money)
	{
		this.TotalMoneyText.text = string.Format("${0}", money);
	}

	[SerializeField]
	private TextMeshProUGUI TotalMoneyText;
}
