using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace AudioMobExamples.FrogRunner
{
	public class GameManager : MonoBehaviour
	{
		#region Variables

		[Header("Obstacle Spawning")]
		[SerializeField]
		[Tooltip("The point where obstacles should spawn from")]
		private Transform obsSpawnPoint;

		[SerializeField]
		[Tooltip("The obstacle prefab")]
		private GameObject[] obPrefabs;

		[SerializeField]
		[Tooltip("The minimum gap distance allowed between any 2 obstacles")]
		private float minObDist;

		[SerializeField]
		[Tooltip("The maximum gap distance allowed between any 2 obstacles")]
		private float maxObDist;

		private GameObject lastOb; // The last obstacle to have been spawned

		private float nextObDist; // The distance the next obstacle will spawn from the last


		[Header("Scoring")]
		[SerializeField]
		[Tooltip("The text object which displays the score")]
		private Text scoreText;

		[SerializeField]
		[Tooltip("The multiplier applied to score to ensure it doesn't climb too quickly")]
		private float scoreDamp;

		private float score; // The player's score as a float (displayed as int)


		[Header("Game State")]
		[SerializeField]
		private GameObject gameOverOverlay;

		[SerializeField]
		private Button pauseButton;

		[SerializeField]
		[Tooltip("The text object that displays \"tap to jump\" at the beginning of the game")]
		private GameObject infoText;

		private bool paused = false; // Boolean indicating whether the game is paused 
		public bool Paused => paused;

		private bool gameStarted; // Boolean indicating whether the game has started
		public bool GameStarted => gameStarted;


		[Header("Ad Progress Bar")]
		[SerializeField]
		[Tooltip("The Progress Bar that will fill up as the ad plays")]
		private Image progressBar;

		[SerializeField]
		[Tooltip("The canvas object that holds the progress bar and its frame")]
		private GameObject progressBarHolder;

		#endregion


		#region Methods For Gameplay

		private void Awake()
		{
			// Assign certain component references to their respective managers
			FrogRunnerAdManager adManager = FindObjectOfType<FrogRunnerAdManager>();

			AssignProgressBar(adManager);
			AssignGameManager(adManager);
		}

		private void Start()
		{
			Time.timeScale = 1; // Make sure game is unpaused
			ShowInfoText(true); // Display Initial info text
			SpawnObstacle();    // Spawn the first obstacle
		}

		private void Update()
		{
			// Check whether to spawn the next obstacle according to nextObDist
			if (obsSpawnPoint.position.x - lastOb.transform.position.x >= nextObDist)
			{
				SpawnObstacle();
			}

			// Update the score 
			UpdateScore();
		}

		/// <summary>
		/// Called to turn on or off info text: "tap to jump"
		/// </summary>
		/// <param name="display"> true (on) or false (off)</param>
		public void ShowInfoText(bool display)
		{
			infoText.SetActive(display);
		}

		/// <summary>
		/// Instantiates an obstacle, saves a reference to it and generates the next spawn distance
		/// </summary>
		private void SpawnObstacle()
		{
			GameObject obstacle = obPrefabs[Random.Range(0, obPrefabs.Length)];
			lastOb = Instantiate(obstacle, obsSpawnPoint.position, obstacle.transform.rotation);
			GenerateObstacleSpawnDistance();
		}

		/// <summary>
		/// Generates the distance the next spawned obstacle will be from the last
		/// </summary>
		private void GenerateObstacleSpawnDistance()
		{
			nextObDist = Random.Range(minObDist, maxObDist);
		}

		/// <summary>
		/// Method to update score, dampened by scoreDamp.
		/// Score is a float to ensure slow climb but is displayed to user as an int 
		/// </summary>
		private void UpdateScore()
		{
			score += (Time.timeScale * scoreDamp);
			scoreText.text = "" + Mathf.RoundToInt(score);
		}

		/// <summary>
		/// Toggles the pause state of the game
		/// </summary>
		public void PauseGame()
		{
			if (!paused)
			{
				paused = true;
				Time.timeScale = 0;
			}
			else
			{
				paused = false;
				Time.timeScale = 1;
			}
		}

		/// <summary>
		/// Ends the game and brings up the Game Over overlay
		/// </summary>
		public void GameOver()
		{
			Time.timeScale = 0; 
			pauseButton.interactable = false;
			gameOverOverlay.SetActive(true);
		}

		/// <summary>
		/// Reloads the scene to restart the game
		/// </summary>
		public void Retry()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		#endregion


		#region Methods For Assigning Components

		/// <summary>
		/// Every time the game restarts, the ad manager needs to be able to find the progress bar again.
		/// Assigns the progress bar and its holder to the correct variables in the ad manager.
		/// </summary>
		private void AssignProgressBar(FrogRunnerAdManager adManager)
		{
			adManager.ProgressBar = progressBar;
			adManager.ProgressBarHolder = progressBarHolder;
		}

		/// <summary>
		/// Every time the game restarts, the ad manager needs to be able to find this game manager object again.
		/// Assigns this game manager to the correct variable in the ad manager.
		/// </summary>
		private void AssignGameManager(FrogRunnerAdManager adManager)
		{
			adManager.GameManager = this;
		}
		#endregion
	}
}
