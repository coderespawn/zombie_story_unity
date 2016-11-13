using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

namespace ZombieStory
{
    public class PlayerController : MonoBehaviour
    {
        public string startWeapon;
        public Transform weaponOffset;
        public WeaponRegistry weaponRegistry;
        public float timeToHoster = 2;
        public float headTiltTime = 0.2f;
        public float maxVehicleMountDistance = 2.0f;
        public GameObject playerPawn;
        public CameraFollow cameraFollow;

        PlayerShooting shooting;
        CharacterController character;
        WeaponItem currentWeapon;
        Animator anim;
        CarUserControl mountedVehicle;
        bool weaponAiming = false;
        float weaponHeadTilt = 0;

        

        Tween<LinearTweenPolicy> headTilt = new Tween<LinearTweenPolicy>();

        void Awake()
        {
            anim = playerPawn.GetComponentInChildren<Animator>();
            shooting = playerPawn.GetComponentInChildren<PlayerShooting>();
            character = playerPawn.GetComponent<CharacterController>();
            Possess(playerPawn);
        }
        
        void Possess(GameObject pawn)
        {
            cameraFollow.target = pawn.transform;
        }

        void OnWeaponAimStart()
        {
            headTilt.Init(headTilt.Value, weaponHeadTilt, headTiltTime, TweenMode.EaseIn);
        }
        void OnWeaponAimStop()
        {
            headTilt.Init(headTilt.Value, 0, headTiltTime, TweenMode.EaseIn);
        }

        void Start()
        {
            SetWeapon(startWeapon);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                ChangeToNextWeapon();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                if (mountedVehicle != null)
                {
                    ExitVehicle();
                } else
                {
                    EnterNearestVehicle();
                }
            }
        }

        void EnterNearestVehicle()
        {
            var vehicle = GameUtils.GetNearestObjectWithTag(playerPawn.transform.position, GameTags.Vehicle);
            if (vehicle == null) return;

            var distanceToVehicle = (playerPawn.transform.position - vehicle.transform.position).magnitude;
            if (distanceToVehicle > maxVehicleMountDistance)
            {
                // nearest vehicle is too far to mount
                return;
            }

            // Enter the vehicle
            mountedVehicle = vehicle.GetComponent<CarUserControl>();
            if (mountedVehicle == null) return;

            mountedVehicle.enabled = true;
            mountedVehicle.mounted = true;
            playerPawn.gameObject.SetActive(false);
            Possess(mountedVehicle.gameObject);
            cameraFollow.zoomFactor = 2.0f;
        } 

        void ExitVehicle()
        {
            mountedVehicle.mounted = false;
            // Place the player next to the vehicle
            Transform vehicleTransform = mountedVehicle.gameObject.transform;
            var newPlayerPosition = vehicleTransform.position + vehicleTransform.rotation * new Vector3(2, 0.25f, 0);

            playerPawn.gameObject.transform.position = newPlayerPosition;
            playerPawn.gameObject.SetActive(true);
            Possess(playerPawn);
            cameraFollow.zoomFactor = 1.0f;
            mountedVehicle = null;
        }


        void ChangeToNextWeapon()
        {
            SetWeapon(weaponRegistry.GetNextWeapon(currentWeapon));
        }

        void SetWeapon(string weaponId)
        {
            var item = weaponRegistry.GetWeapon(weaponId);
            SetWeapon(item);
        }

        void SetWeapon(WeaponItem item)
        {
            var sfx = weaponRegistry.GetWeaponSfx(item.sfxName);

            // Destroy the existing weapon
            GameUtils.DestroyAllChildren(weaponOffset);

            if (item.template != null)
            {
                var weapon = GameObject.Instantiate(item.template);
                weapon.transform.SetParent(weaponOffset, false);
            }

            currentWeapon = item;

            var weaponAudio = weaponRegistry.GetWeaponSfx(currentWeapon.sfxName);
            shooting.OnWeaponChanged(currentWeapon, weaponAudio);

            if (item.roundsPerSecond == 0)
            {
                item.roundsPerSecond = 1;
            }
        }

        void FixedUpdate()
        {
            bool oldAimingState = weaponAiming;
            weaponAiming = shooting.TimeSinceLastShot < timeToHoster;
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

            // Notify if the weapon aim state has changed
            if (weaponAiming != oldAimingState)
            {
                if (weaponAiming)
                {
                    OnWeaponAimStart();
                }
                else
                {
                    OnWeaponAimStop();
                }
            }
            
            bool fullAuto = currentWeapon.fullAuto;
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
        }

        public void SetupHeadTilt()
        {
            if (weaponAiming)
            {
                int weaponIndex = currentWeapon.animationIndex;
                if (weaponIndex == (int)WeaponCategory.NoWeapon)
                {
                    weaponHeadTilt = 0;
                }
                else if (weaponIndex == (int)WeaponCategory.Pistol)
                {
                    weaponHeadTilt = -0.3f;
                }
                else if (weaponIndex == (int)WeaponCategory.Grenades)
                {
                    weaponHeadTilt = 0;
                }
                else
                {
                    weaponHeadTilt = -0.9f;
                }
            }

            float currentTilt = headTilt.Value;
            anim.SetFloat("Head_Horizontal_f", headTilt.Value);
        }
    }
}
