using UnityEngine;
using System.Collections;

public class GameUtils {

    public static void DestroyAllChildren(Transform transform) {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
    }

    public static void PlayAudioAndForget(AudioClip clip, float volume) {
        var audioObject = new GameObject();
        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.volume = volume;
        source.clip = clip;
        source.Play();

        audioObject.AddComponent<DestroyOnAudioComplete>();
    }

}
