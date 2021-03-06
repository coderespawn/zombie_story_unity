﻿using UnityEngine;

namespace ZombieStory
{
    public delegate void OnShootingStarted();
    public delegate void OnShootingStopped();

    public class PlayerShooting : MonoBehaviour
    {
        public int damagePerShot = 20;                  // The damage inflicted by each bullet.
        public float timeBetweenBullets = 0.15f;        // The time between each shot.
        public float range = 100f;                      // The distance the gun can fire.
        public GameObject gunBarrel;
        
        public event OnShootingStarted ShootingStarted;
        public event OnShootingStopped ShootingStopped;

        float timeSinceLastShot = 100;                                    // A timer to determine when to fire.
        public float TimeSinceLastShot
        {
            get { return timeSinceLastShot; }
        }

        public bool IsShooting
        {
            get { return shooting; }
        }

        public Ray shootRay;                                   // A ray from the gun end forwards.
        //public Light faceLight;								// Duh

        RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
        ParticleSystem gunParticles;                    // Reference to the particle system.
        LineRenderer gunLine;                           // Reference to the line renderer.
        Light gunLight;                                 // Reference to the light component.
        SfxPlayer sfxPlayer;
        WeaponSfx weaponSfx;
        float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.
        bool shooting = false;

        void Awake ()
        {
            // Set up the references.
            gunParticles = gunBarrel.GetComponent<ParticleSystem> ();
            gunLine = gunBarrel.GetComponent <LineRenderer> ();
            gunLight = gunBarrel.GetComponent<Light> ();
            //faceLight = GetComponentInChildren<Light> ();
            sfxPlayer = GetComponent<SfxPlayer>();
        }


        void Update ()
        {
            // Add the time since Update was last called to the timer.
            timeSinceLastShot += Time.deltaTime;

            bool isShootingThisFrame = shooting;

            // If the Fire1 button is being press and it's time to fire...
			if(Input.GetButton ("Fire1") && TimeSinceLastShot >= timeBetweenBullets && Time.timeScale != 0)
            {
                // ... shoot the gun.
                Shoot ();
                shooting = true;
            }

			// If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
            if(TimeSinceLastShot >= timeBetweenBullets * effectsDisplayTime)
            {
                // ... disable the effects.
                DisableEffects ();
                shooting = false;
            }

            if (isShootingThisFrame != shooting)
            {
                // The state has changed.  Notify the listeners
                if (shooting && ShootingStarted != null)
                {
                    ShootingStarted();
                }
                else if (!shooting && ShootingStopped != null)
                {
                    ShootingStopped();
                }
            }
        }

        public void OnWeaponChanged(WeaponItem weapon, WeaponSfx weaponSfx)
        {
            this.weaponSfx = weaponSfx;
            gunBarrel.transform.localPosition = weapon.barrelOffset;
            timeBetweenBullets = 1.0f / weapon.roundsPerSecond;
        }

        public void DisableEffects ()
        {
            // Disable the line renderer and the light.
            gunLine.enabled = false;
			//faceLight.enabled = false;
            gunLight.enabled = false;
        }


        void Shoot ()
        {
            // Reset the timer.
            timeSinceLastShot = 0f;

            sfxPlayer.PlayAudio(weaponSfx.sfx, weaponSfx.volume);

            // Enable the lights.
            gunLight.enabled = true;
			//faceLight.enabled = true;

            // Stop the particles from playing if they were, then start the particles.
            gunParticles.Stop ();
            gunParticles.Play ();

            // Enable the line renderer and set it's first position to be the end of the gun.
            gunLine.enabled = true;
            gunLine.SetPosition (0, gunBarrel.transform.position);

            // Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
            shootRay.origin = gunBarrel.transform.position;
            shootRay.direction = gunBarrel.transform.forward;

            // Perform the raycast against gameobjects on the shootable layer and if it hits something...
            if(Physics.Raycast (shootRay, out shootHit, range))
            {
                // Try and find an EnemyHealth script on the gameobject hit.
                EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();

                // If the EnemyHealth component exist...
                if(enemyHealth != null)
                {
                    // ... the enemy should take damage.
                    enemyHealth.TakeDamage (damagePerShot, shootHit.point);
                }

                // Set the second position of the line renderer to the point the raycast hit.
                gunLine.SetPosition (1, shootHit.point);
            }
            // If the raycast didn't hit anything on the shootable layer...
            else
            {
                // ... set the second position of the line renderer to the fullest extent of the gun's range.
                gunLine.SetPosition (1, shootRay.origin + shootRay.direction * range);
            }
        }
    }
}