using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Photon.Chat.UtilityScripts
{
	[RequireComponent(typeof(Text))]
	public class TextButtonTransition : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		public void Awake()
		{
			this._text = base.GetComponent<Text>();
		}

		public void OnEnable()
		{
			this._text.color = this.NormalColor;
		}

		public void OnDisable()
		{
			this._text.color = this.NormalColor;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (this.Selectable == null || this.Selectable.IsInteractable())
			{
				this._text.color = this.HoverColor;
			}
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (this.Selectable == null || this.Selectable.IsInteractable())
			{
				this._text.color = this.NormalColor;
			}
		}

		private Text _text;

		public Selectable Selectable;

		public Color NormalColor = Color.white;

		public Color HoverColor = Color.black;
	}
}
