using UnityEngine;
using System.Collections;

namespace ZombieStory
{
	public class RandomParticlePoint : MonoBehaviour 
	{
	    [Range(0f, 1f)]
	    public float normalizedTime;


	    void OnValidate()
	    {
	        GetComponent<ParticleSystem>().Simulate (normalizedTime, true, true);
	    }
	}
}
