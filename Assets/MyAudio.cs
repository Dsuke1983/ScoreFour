using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyAudio : MonoBehaviour {

	public static bool winSoundFlug = false;
	public static bool loseSoundFlug = false;
	public static bool put1SoundFlug = false;
	public static bool put2SoundFlug = false;
	public AudioClip winSound;
	public AudioClip loseSound;
	public AudioClip put1Sound;
	public AudioClip put2Sound;
	private AudioSource audioSource;

	// Use this for initialization
	void Start () {

		audioSource = gameObject.GetComponent<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update () {

		if (winSoundFlug == true) {

			audioSource.clip = winSound;
			audioSource.loop = false;
			audioSource.Play ();
			winSoundFlug = false;

		} else if (loseSoundFlug == true) {

			audioSource.clip = loseSound;
			audioSource.loop = false;
			audioSource.Play ();
			loseSoundFlug = false;

		} else if (put1SoundFlug == true) {

			audioSource.clip = put1Sound;
			audioSource.loop = false;
			audioSource.Play ();
			put1SoundFlug = false;

		} else if (put2SoundFlug == true) {

			audioSource.clip = put2Sound;
			audioSource.loop = false;
			audioSource.Play ();
			put2SoundFlug = false;

		}
	}
}
