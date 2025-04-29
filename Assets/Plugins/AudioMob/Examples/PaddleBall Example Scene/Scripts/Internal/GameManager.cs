using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
#endif

namespace AudioMobExamples.PaddleBall
{
	public class GameManager : MonoBehaviour
	{
		#region Variables

		[Header("Gameplay")]
		[SerializeField]
		[Tooltip("Indicates whether or not game has started yet")]
		private bool hasStarted;

		[SerializeField]
		[Tooltip("The initial text telling the user to touch the screen to start")]
		private GameObject startText;

		[SerializeField]
		[Tooltip("Reference to the ball prefab")]
		private GameObject ballPrefab;

		[SerializeField]
		[Tooltip("Reference to the player's paddle")]
		private PlayerPaddle playerPaddle;

		[SerializeField]
		[Tooltip("Reference to the enemy's paddle")]
		private EnemyMovement enemyPaddle;

		[SerializeField]
		[Tooltip("The player's goal collider")]
		private Collider playerGoalCollider;

		[SerializeField]
		[Tooltip("The enemy's goal collider")]
		private Collider enemyGoalCollider;

		private GameObject currentBall; // Reference to the current ball in the scene

		public GameObject CurrentBall
		{
			get => currentBall;
		}


		[Header("Scoring")]
		[SerializeField]
		[Tooltip("The player's score text")]
		private Text playerScoreText;

		[SerializeField]
		[Tooltip("The player's score text")]
		private Text enemyScoreText;

		[SerializeField]
		[Tooltip("The number of points required to win the game")]
		private int winScore;

		[SerializeField]
		[Tooltip("The Audio Source which will play a sound effect when a goal has been scored")]
		private AudioSource scoreAudioSource;

		private int playerScore; // Keeps track of player score
		private int enemyScore;  // Keeps track of enemy score


		[Header("Game Over")]
		[SerializeField]
		[Tooltip("The overlay which appears when the game is over")]
		private GameObject gameOverOverlay;

		[SerializeField]
		[Tooltip("The text informing the user who won the game")]
		private Text gameOverText;

		private bool isGameOver; // Keeps track of whether or not the game is over

		public bool IsGameOver
		{
			get => isGameOver;
		}


		[Header("Ad")]
		[SerializeField]
		[Tooltip("Reference to ad manager")]
		private PaddleBallAdManager adManager;

		#endregion


		#region Gameplay Methods

		private void Start()
		{
			isGameOver = false;
			Time.timeScale = 1;
		}

		private void Update()
		{
#if ENABLE_INPUT_SYSTEM
			if (!hasStarted && playerPaddle.clickPressedDown)
			{
				StartGame();
			}
#else
			if (!hasStarted && Input.GetMouseButtonDown(0))
			{
				StartGame();
			}
#endif
		}

		/// <summary>
		/// Called after player has tapped the screen.
		/// Initiates the beginning of the game by spawning the ball.
		/// </summary>
		private void StartGame()
		{
			hasStarted = true;
			startText.SetActive(false);
			RespawnBall();
		}

		/// <summary>
		/// Checks if anyone has reached the winning score.
		/// If so, calls GameOver method, passing in the name of the winner
		/// </summary>
		private void EndCheck()
		{
			if (playerScore >= winScore)
			{
				GameOver("You");
			}
			else if (enemyScore >= winScore)
			{
				GameOver("CPU");
			}
		}

		/// <summary>
		/// Ends the game and brings up the Game Over overlay
		/// </summary>
		/// <param name="winner"> The name of the winning player (you or CPU) for display on game over screen </param>
		private void GameOver(string winner)
		{
			isGameOver = true;
			Time.timeScale = 0;

			currentBall.SetActive(false);

			gameOverOverlay.SetActive(true);
			gameOverText.text = "GAME OVER!\n" + winner + " won";
		}

		/// <summary>
		/// Plays the score sound from the scoreAudioSource attached to game manager
		/// </summary>
		private void PlayScoreSound()
		{
			scoreAudioSource.Play();
		}

		/// <summary>
		/// Reloads the current scene
		/// </summary>
		public void Retry()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

#if ENABLE_INPUT_SYSTEM
		protected void OnEnable()
		{
			EnhancedTouchSupport.Enable();
		}

		protected void OnDisable()
		{
			EnhancedTouchSupport.Disable();
		}
#endif

		#endregion


		#region Ball Methods

		/// <summary>
		/// Creates ball if doesn't already exist. Otherwise, resets ball's velocity and position.
		/// </summary>
		private void RespawnBall()
		{
			if (!isGameOver)
			{
				if (!currentBall)
				{
					currentBall = Instantiate(ballPrefab);
					enemyPaddle.Ball = currentBall.transform;
				}
				else
				{
					currentBall.GetComponent<BallMovement>().ResetBall();
				}
			}
		}

		/// <summary>
		/// Called when a ball goes into a goal.
		/// Increments score and activates ball respawn
		/// </summary>
		/// <param name="goal"> The goal which the ball has entered</param>
		public void BallInGoal(Collider goal)
		{
			if (goal == playerGoalCollider)
			{
				enemyScore++;
				enemyScoreText.text = "" + enemyScore;
			}
			else if (goal == enemyGoalCollider)
			{
				playerScore++;
				playerScoreText.text = "" + playerScore;
			}

			EndCheck();
			RespawnBall();
			PlayScoreSound();
			
			adManager.ShowAdButton();
		}

		#endregion
	}
}
