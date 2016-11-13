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


    public static GameObject GetNearestObjectWithTag(Vector3 position, string tag)
    {
        var vehicles = GameObject.FindGameObjectsWithTag(tag);
        float nearestDistanceSq = float.MaxValue;
        GameObject nearestVehicle = null;
        foreach (var vehicle in vehicles)
        {
            var distanceSq = (vehicle.transform.position - position).sqrMagnitude;
            if (distanceSq < nearestDistanceSq)
            {
                nearestDistanceSq = distanceSq;
                nearestVehicle = vehicle;
            }
        }
        return nearestVehicle;
    }
}
