using UnityEngine;
using System.Collections;

[System.Serializable]
public struct WeaponItem {
    public string weaponId;
    public WeaponCategory weaponCategory;
    public GameObject template;
    public float roundsPerMinute;
    public int animationIndex;
    public string sfxName;
}

[System.Serializable]
public struct WeaponSfx {
    public string sfxId;
    public float volume;
    public AudioClip sfx;
}

public enum WeaponCategory
{
    NoWeapon = 0,
    Pistol,
    AssultRifle01,
    AssultRifle02,
    Shotgun,
    SniperRifle,
    Rifle,
    SubMachineGun,
    RPG,
    MiniGun,
    Grenades,
    Melee
}

public class WeaponRegistry : MonoBehaviour {

    public WeaponItem[] weapons;

    public WeaponSfx[] weaponSounds;

    public WeaponItem GetWeapon(string weaponId) {
        foreach (var weapon in weapons) {
            if (weapon.weaponId == weaponId) {
                return weapon;
            }
        }
        return new WeaponItem();
    }

    public WeaponSfx GetWeaponSfx(string sfxId) {
        foreach (var sfx in weaponSounds) {
            if (sfx.sfxId == sfxId) {
                return sfx;
            }
        }
        return new WeaponSfx();
    }
}
