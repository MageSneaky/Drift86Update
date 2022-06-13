using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
[Serializable]
public class MusicPlayer : MonoBehaviour
{
	private void Start()
	{
		this.trackText.gameObject.transform.parent.gameObject.SetActive(false);
		this.GetActiveTracks();
		this.audioSource = base.GetComponent<AudioSource>();
		base.StartCoroutine(this.playTrack(this.enabledTracks));
	}

	private void Update()
	{
		if (!this.isPlaying)
		{
			base.StartCoroutine(this.playTrack(this.enabledTracks));
		}
	}

	private void GetActiveTracks()
	{
		this.enabledTracks.Clear();
		for (int i = 0; i < this.Playlist.Count; i++)
		{
			if (this.Playlist[i].enabled)
			{
				this.enabledTracks.Add(this.Playlist[i]);
			}
		}
	}

	private void UpdateIndex()
	{
		if (this.playRandomClips)
		{
			this.playlistIndex = UnityEngine.Random.Range(0, this.enabledTracks.Count);
			return;
		}
		this.playlistIndex++;
		if (this.playlistIndex >= this.enabledTracks.Count)
		{
			this.playlistIndex = 0;
		}
	}

	public IEnumerator playTrack(List<MusicPlayer.MusicTracks> tracks)
	{
		this.isPlaying = true;
		this.UpdateIndex();
		this.audioSource.clip = tracks[this.playlistIndex].track;
		this.audioSource.Play();
		if (this.messageOnPlaying)
		{
			this.trackText.gameObject.transform.parent.gameObject.SetActive(true);
			this.trackText.text = this.audioSource.clip.name;
			yield return new WaitForSeconds(2.5f);
			this.trackText.gameObject.transform.parent.gameObject.SetActive(false);
		}
		yield return new WaitForSeconds(this.audioSource.clip.length);
		this.isPlaying = false;
		yield break;
	}

	[Header("Text")]
	public Text trackText;

	[Header("Options")]
	public bool playRandomClips;

	public bool messageOnPlaying;

	private AudioSource audioSource;

	private bool isPlaying;

	private int playlistIndex = -1;

	[Header("Your Playlist")]
	public List<MusicPlayer.MusicTracks> Playlist = new List<MusicPlayer.MusicTracks>();

	private List<MusicPlayer.MusicTracks> enabledTracks = new List<MusicPlayer.MusicTracks>();

	[Serializable]
	public struct MusicTracks
	{
		public bool enabled;

		public AudioClip track;
	}
}
