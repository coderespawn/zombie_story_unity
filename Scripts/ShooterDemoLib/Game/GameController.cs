//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DungeonArchitect;
using DungeonArchitect.Utils;
using DungeonArchitect.Navigation;

namespace ZombieStory
{
	public class GameController : MonoBehaviour {

        public AudioSource backgroundMusic;

		private static GameController instance;
		public Dungeon dungeon;
		public DungeonNavMesh navMesh;
		public GameObject minimap;
        
		public GameObject levelLoadingScreen;
		public Text textBuildingLayout;
		public Text textBuildingNavMesh;

		LevelNpcSpawner npcSpawner;
		string labelBuildingLayout = "Building Layout... ";
		string labelBuildingNavmesh = "Building Navmesh... ";

        Tween<CubicTweenPolicy> gameSoundVolume = new Tween<CubicTweenPolicy>();

        public static GameController Instance {
			get {
				return instance;
			}
		}

		void Awake() {
			instance = this;
			npcSpawner = GetComponent<LevelNpcSpawner>();
            backgroundMusic = GetComponent<AudioSource>();

            CreateNewLevel();
            gameSoundVolume.Init(0, 1, 2, TweenMode.EaseIn);
        }

        void Update()
        {
            // Update the game volume
            HandleAudio();
        }

        void HandleAudio()
        {
            gameSoundVolume.Update(Time.deltaTime);
            backgroundMusic.volume = gameSoundVolume.Value;
        }

        public void CreateNewLevel() {
            if (dungeon != null)
            {
                // Assing a different seed to create a new layout
                int seed = Mathf.FloorToInt(Random.value * int.MaxValue);
                dungeon.Config.Seed = (uint)seed;

                // Rebuild a new dungeon
                StartCoroutine(RebuildLevel(dungeon));
            } else
            {
                Debug.LogWarning("Dungeon references it not assigned");
            }
        }

        IEnumerator RebuildLevel(Dungeon dungeon) {
            textBuildingNavMesh.gameObject.SetActive(false);
            levelLoadingScreen.SetActive(true);
            if (minimap != null)
            {
                minimap.SetActive(false);
            }

            textBuildingLayout.text = labelBuildingLayout;
			textBuildingLayout.gameObject.SetActive(true);
			yield return 0;
            
			dungeon.DestroyDungeon();
			yield return 0;

			dungeon.Build();
			
			textBuildingLayout.text = labelBuildingLayout + "DONE!";

			textBuildingNavMesh.text = labelBuildingNavmesh;
			textBuildingNavMesh.gameObject.SetActive(true);
			yield return 0;
            

			RebuildNavigation();

			npcSpawner.OnPostDungeonBuild(dungeon, dungeon.ActiveModel);

			levelLoadingScreen.SetActive(false);
            if (minimap != null)
            {
                minimap.SetActive(true);
            }

			// reset player health
			var player = GameObject.FindGameObjectWithTag(GameTags.Player);
			if (player != null) {
				var health = player.GetComponent<PlayerHealth>();
				if (health != null) {
					health.currentHealth = health.startingHealth;
				}
			}

			// Destroy any npc too close to the player
			var enemyControllers = GameObject.FindObjectsOfType<AIController>();
			var playerPosition = player.transform.position;
            foreach (var enemyController in enemyControllers)
            {
                var enemy = enemyController.gameObject;
				var distance = (playerPosition - enemy.transform.position).magnitude;
				if (distance < 1) {
					Destroy (enemy);
				}
			}
	    }

		public void RebuildNavigation() {
            if (navMesh != null)
            {
                navMesh.Build();
            } else
            {
                Debug.LogWarning("Navigation reference it not assigned. Cannot build navigation");
            }
        }
	}
}