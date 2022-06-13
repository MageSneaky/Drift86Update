using System;
using UnityEngine;
using UnityEngine.UI;

namespace AsImpL
{
	[RequireComponent(typeof(ObjectImporter))]
	public class ObjectImporterUI : MonoBehaviour
	{
		private void Awake()
		{
			if (this.progressSlider != null)
			{
				this.progressSlider.maxValue = 100f;
				this.progressSlider.gameObject.SetActive(false);
			}
			if (this.progressImage != null)
			{
				this.progressImage.gameObject.SetActive(false);
			}
			if (this.progressText != null)
			{
				this.progressText.gameObject.SetActive(false);
			}
			this.objImporter = base.GetComponent<ObjectImporter>();
		}

		private void OnEnable()
		{
			this.objImporter.ImportingComplete += this.OnImportComplete;
			this.objImporter.ImportingStart += this.OnImportStart;
		}

		private void OnDisable()
		{
			this.objImporter.ImportingComplete -= this.OnImportComplete;
			this.objImporter.ImportingStart -= this.OnImportStart;
		}

		private void Update()
		{
			bool flag = Loader.totalProgress.singleProgress.Count > 0;
			if (!flag)
			{
				return;
			}
			int numImportRequests = this.objImporter.NumImportRequests;
			int num = numImportRequests - Loader.totalProgress.singleProgress.Count;
			if (flag)
			{
				float num2 = 100f * (float)num / (float)numImportRequests;
				float num3 = 0f;
				foreach (SingleLoadingProgress singleLoadingProgress in Loader.totalProgress.singleProgress)
				{
					if (num3 < singleLoadingProgress.percentage)
					{
						num3 = singleLoadingProgress.percentage;
					}
				}
				num2 += num3 / (float)numImportRequests;
				if (this.progressSlider != null)
				{
					this.progressSlider.value = num2;
					this.progressSlider.gameObject.SetActive(flag);
				}
				if (this.progressImage != null)
				{
					this.progressImage.fillAmount = num2 / 100f;
					this.progressImage.gameObject.SetActive(flag);
				}
				if (this.progressText != null)
				{
					if (!flag)
					{
						this.progressText.gameObject.SetActive(false);
						this.progressText.text = "";
						return;
					}
					this.progressText.gameObject.SetActive(flag);
					this.progressText.text = "Loading " + Loader.totalProgress.singleProgress.Count + " objects...";
					string text = "";
					int num4 = 0;
					foreach (SingleLoadingProgress singleLoadingProgress2 in Loader.totalProgress.singleProgress)
					{
						if (num4 > 4)
						{
							text += "...";
							break;
						}
						if (!string.IsNullOrEmpty(singleLoadingProgress2.message))
						{
							if (num4 > 0)
							{
								text += "; ";
							}
							text += singleLoadingProgress2.message;
							num4++;
						}
					}
					if (text != "")
					{
						Text text2 = this.progressText;
						text2.text = text2.text + "\n" + text;
						return;
					}
				}
			}
			else
			{
				this.OnImportComplete();
			}
		}

		private void OnImportStart()
		{
			if (this.progressText != null)
			{
				this.progressText.text = "";
			}
			if (this.progressSlider != null)
			{
				this.progressSlider.value = 0f;
				this.progressSlider.gameObject.SetActive(true);
			}
			if (this.progressImage != null)
			{
				this.progressImage.fillAmount = 0f;
				this.progressImage.gameObject.SetActive(true);
			}
		}

		private void OnImportComplete()
		{
			if (this.progressText != null)
			{
				this.progressText.text = "";
			}
			if (this.progressSlider != null)
			{
				this.progressSlider.value = 100f;
				this.progressSlider.gameObject.SetActive(false);
			}
			if (this.progressImage != null)
			{
				this.progressImage.fillAmount = 1f;
				this.progressImage.gameObject.SetActive(false);
			}
		}

		[Tooltip("Text for activity messages")]
		public Text progressText;

		[Tooltip("Slider for the overall progress")]
		public Slider progressSlider;

		[Tooltip("Panel with the Image Type set to Filled")]
		public Image progressImage;

		private ObjectImporter objImporter;
	}
}
