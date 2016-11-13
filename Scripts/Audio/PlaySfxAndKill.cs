using UnityEngine;
using System.Collections;

public class PlaySfxAndKill : MonoBehaviour {
    AudioSource source;
	void Awake () {
        source = GetComponent<AudioSource>();
	}
	
    public void PlayAudio(AudioClip clip, float volumeScale)
    {
        source.clip = clip;
        source.PlayOneShot(clip, volumeScale);
    }

	// Update is called once per frame
	void Update () {
	    if (!source.isPlaying)
        {
            Destroy(gameObject);
        }
	}
}
