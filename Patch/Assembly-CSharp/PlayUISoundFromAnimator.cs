using System;
using UnityEngine;

public class PlayUISoundFromAnimator : MonoBehaviour
{
	public void PlaySound(AudioClip clip)
	{
		SoundControllerInUI.PlayAudioClip(clip);
	}
}
