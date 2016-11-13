using UnityEngine;
using System.Collections;

public class SfxPlayer : MonoBehaviour {
    public Transform parent;
    public UnityEngine.Audio.AudioMixerGroup mixerGroup;

    public void PlayAudio(AudioClip clip, float volumeScale)
    {
        if (clip != null)
        {
            var sfx = new GameObject();
            sfx.isStatic = true;
            var source = sfx.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = mixerGroup;

            var script = sfx.AddComponent<PlaySfxAndKill>();
            script.PlayAudio(clip, volumeScale);

            if (parent != null)
            {
                sfx.transform.parent = parent;
            }
        }
    }
}
