using UnityEngine;
using System.Collections;


namespace ZombieStory
{
    public class PlayerController : MonoBehaviour
    {
        public string startWeapon;
        public Transform weaponOffset;
        public WeaponRegistry weaponRegistry;
        public float timeToHoster = 2;
        public float headTiltTime = 0.2f;
        
        PlayerShooting shooting;
        CharacterController character;
        WeaponItem currentWeapon;
        Animator anim;
        public bool weaponAiming = false;

        Tween<LinearTweenPolicy> headTilt = new Tween<LinearTweenPolicy>();

        void Awake()
        {
            anim = GetComponentInChildren<Animator>();
            shooting = GetComponentInChildren<PlayerShooting>();
            character = GetComponent<CharacterController>();
        }
        
        void OnWeaponAimStart()
        {
            headTilt.Init(headTilt.Value, -0.9f, headTiltTime, TweenMode.EaseIn);
        }
        void OnWeaponAimStop()
        {
            headTilt.Init(headTilt.Value, 0, headTiltTime, TweenMode.EaseIn);
        }

        void Start()
        {
            SetWeapon(startWeapon);
        }

        void SetWeapon(string weaponId)
        {
            var item = weaponRegistry.GetWeapon(weaponId);
            var sfx = weaponRegistry.GetWeaponSfx(item.sfxName);

            // Destroy the existing weapon
            GameUtils.DestroyAllChildren(weaponOffset);

            if (item.template != null)
            {
                var weapon = GameObject.Instantiate(item.template);
                weapon.transform.SetParent(weaponOffset, false);
            }

            currentWeapon = item;

            if (item.roundsPerMinute == 0)
            {
                item.roundsPerMinute = 1;
            }

            shooting.timeBetweenBullets = 1.0f / item.roundsPerMinute;
            //shooting.GunAudio.clip = sfx.sfx;
        }

        void FixedUpdate()
        {
            {
                bool isAimingNew = shooting.TimeSinceLastShot < timeToHoster;
                if (weaponAiming != isAimingNew)
                {
                    weaponAiming = isAimingNew;
                    if (weaponAiming)
                    {
                        OnWeaponAimStart();
                    } else
                    {
                        OnWeaponAimStop();
                    }
                }
            }
            headTilt.Update(Time.fixedDeltaTime);

            int weaponAnimationId = weaponAiming ? currentWeapon.animationIndex : 0;
            anim.SetInteger("WeaponType_int", weaponAnimationId);

            float speed = 0.0f;
            if (character != null)
            {
                speed = character.velocity.magnitude;
            }

            if (anim != null)
            {
                anim.SetFloat("Speed_f", speed);
            }

            SetupBodyOrientation(speed);
            SetupHeadTilt();

            bool fullAuto = true;
            anim.SetBool("Shoot_b", shooting.IsShooting);
            anim.SetBool("FullAuto_b", fullAuto);
        }

        enum PlayerMoveAnimState
        {
            Idle,
            Walk,
            Run
        }

        public void SetupBodyOrientation(float speed) {
            int weaponIndex = currentWeapon.animationIndex;
            PlayerMoveAnimState animState = PlayerMoveAnimState.Idle;
            if (speed <= 1e-4f)
            {
                animState = PlayerMoveAnimState.Idle;
            }
            else if (speed <= 0.1f)
            {
                animState = PlayerMoveAnimState.Idle;
            }
            else
            {
                animState = PlayerMoveAnimState.Run;
            }

            float bodyH = 0;
            float bodyV = 0;

            if (weaponIndex == (int)WeaponCategory.NoWeapon)
            {
                bodyH = 0;
                bodyV = 0;
            }
            else if (weaponIndex == (int)WeaponCategory.Pistol)
            {
                switch(animState)
                {
                    case PlayerMoveAnimState.Idle: bodyH = 0; bodyV = 0; break;
                    case PlayerMoveAnimState.Walk: bodyH = 0; bodyV = 0; break;
                    case PlayerMoveAnimState.Run:  bodyH = 0.2f; bodyV = 0; break;
                }
            }
            else
            {
                switch (animState)
                {
                    case PlayerMoveAnimState.Idle: bodyH = 0.6f; bodyV = 0; break;
                    case PlayerMoveAnimState.Walk: bodyH = 0.6f; bodyV = 0; break;
                    case PlayerMoveAnimState.Run: bodyH = 0.6f; bodyV = 0.3f; break;
                }
            }

            anim.SetFloat("Body_Horizontal_f", bodyH);
            anim.SetFloat("Body_Vertical_f", bodyV);

            //float maxHeadTiltH = -0.9f;
            //float targetheadTilt = bodyH / 0.6f * maxHeadTiltH;


        }

        public void SetupHeadTilt()
        {
            float headH = 0;

            if (weaponAiming)
            {
                int weaponIndex = currentWeapon.animationIndex;
                if (weaponIndex == (int)WeaponCategory.NoWeapon)
                {
                    headH = 0;
                }
                else if (weaponIndex == (int)WeaponCategory.Pistol)
                {
                    headH = -0.3f;
                }
                else if (weaponIndex == (int)WeaponCategory.Grenades)
                {
                    headH = 0;
                }
                else
                {
                    headH = -0.9f;
                }
            }

            float currentTilt = headTilt.Value;
            
            anim.SetFloat("Head_Horizontal_f", headTilt.Value);
        }
    }
}
