using UnityEngine;
using System.Collections;
using DAShooter;



public class PlayerController : MonoBehaviour {
    public string weaponId;
    public Transform weaponOffset;
    public WeaponRegistry weaponRegistry;
    public PlayerShooting shooting;
    public float timeToHoster = 2;
   
    WeaponItem currentWeapon;
    Animator anim;

    void Awake() {
        anim = GetComponent<Animator>();
    }

    void Start() {
        SetWeapon(weaponId);
    }

    void SetWeapon(string weaponId) {
        var item = weaponRegistry.GetWeapon(weaponId);
        var sfx = weaponRegistry.GetWeaponSfx(item.sfxName);

        // Destroy the existing weapon
        GameUtils.DestroyAllChildren(weaponOffset);

        if (item.template != null) {
            var weapon = GameObject.Instantiate(item.template);
            weapon.transform.SetParent(weaponOffset, false);
        }

        currentWeapon = item;

        if (item.roundsPerMinute == 0) {
            item.roundsPerMinute = 1;
        }

        shooting.timeBetweenBullets = 1.0f / item.roundsPerMinute;
        shooting.GunAudio.clip = sfx.sfx;
    }

    void FixedUpdate() {
        bool isShooting = shooting.TimeSinceLastShot < timeToHoster;
        int weaponAnimationId = isShooting ? currentWeapon.animationIndex : 0;
        anim.SetInteger("WeaponType_int", weaponAnimationId);

    }
}
