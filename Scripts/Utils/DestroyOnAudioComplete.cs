using UnityEngine;
using System.Collections;

public class DestroyOnAudioComplete : MonoBehaviour {

    public AudioSource audioSource;

	// Use this for initialization
	void Awake () {
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!audioSource.isPlaying) {
            Destroy(gameObject);
        }
	}
}
