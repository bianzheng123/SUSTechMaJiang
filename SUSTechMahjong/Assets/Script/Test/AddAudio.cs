using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAudio : MonoBehaviour {
    private AudioSource audioSource;
	// Use this for initialization
	void Start () {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = ResManager.LoadAudio("GameLayer/Audios/female/gang");
        audioSource.Play();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
