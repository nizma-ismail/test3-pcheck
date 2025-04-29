using UnityEngine;

namespace AudioMobExamples.FrogRunner
{
	public class Player : MonoBehaviour
	{
		#region Vairables

		[SerializeField]
		[Tooltip("Reference to GameManager component in scene")]
		private GameManager gameManager;

		[SerializeField]
		[Tooltip("Is true when player is on the ground")]
		private bool grounded;

		[SerializeField]
		[Tooltip("The strength of the jump force applied to the player")]
		private float jumpForce;

		[SerializeField]
		[Tooltip("Button that covers the whole screen which triggers player jump")]
		private JumpButton jumpButton;

		[SerializeField]
		[Tooltip("The maximum time that the player can hold down the jump button for")]
		private float maxJumpTime;

		[SerializeField]
		[Tooltip("The minimum time that the player can jump for")]
		private float minJumpTime; // If jump button is released before this time is up, player will continue to jump

		[SerializeField]
		[Tooltip("Animator used to change between player animations")]
		private Animator animator;

		[SerializeField]
		[Tooltip("array of audio sources to do with player movement")]
		private AudioSource[] audioSources;

		private Rigidbody2D rb2D; // Reference to RigidBody2d Component of player
		private float jumpTime;   // The amount of time the player has held down the jump button
		private bool canJump;     // Keeps track of whether player is allowed to jump

		#endregion


		#region Methods

		private void Start()
		{
			rb2D = GetComponent<Rigidbody2D>(); // Assign rigidbody component to variable
		}

		private void FixedUpdate()
		{
			if (JumpCheck())
			{
				Jump();

				//If this is the first click, turn off the info text
				if (!gameManager.GameStarted)
				{
					gameManager.ShowInfoText(false);
				}
			}
			// Keep adding jump force to player until minimum jump time has been reached
			else if (!grounded && jumpTime > 0 && jumpTime < minJumpTime)
			{
				jumpTime += Time.deltaTime;
				Jump();
			}

			if (!grounded)
			{
				CheckFalling();
			}
		}

		/// <summary>
		/// Performed every frame to check if the player can jump any more.
		/// The longer jump is held down, the higher the jump will be.
		/// </summary>
		private bool JumpCheck()
		{
			switch (jumpButton.IsPressed)
			{
				case true when jumpTime <= maxJumpTime && canJump:
					jumpTime += Time.deltaTime;
					return true;

				case false when grounded:
					jumpTime = 0;
					canJump = true;
					break;

				case true when jumpTime > maxJumpTime:
				case false when jumpTime > 0:
					canJump = false;
					break;
			}

			return false;
		}

		/// <summary>
		/// Adds jump force to player
		/// </summary>
		private void Jump()
		{
			rb2D.linearVelocity = Vector2.up * jumpForce;
		}

		/// <summary>
		/// Called while jumping to check if falling animation should be displayed 
		/// </summary>
		private void CheckFalling()
		{
			float yVel = rb2D.GetPointVelocity(transform.localPosition).y;

			if (yVel < 0 && !animator.GetBool("JumpDown"))
			{
				animator.SetBool("JumpDown", true);
			}
		}

		/// <summary>
		/// Is called by the player animation keyframes to trigger player sound effects
		/// </summary>
		/// <param name="s"> String identifier as to what sound should be played</param>
		private void PlaySound(string s)
		{
			switch (s)
			{
				case "run1":
					audioSources[0].Play();
					break;
				case "run2":
					audioSources[1].Play();
					break;
				case "jump":
					audioSources[2].Play();
					break;
			}
		}

		#endregion


		#region Collider Trigger Events

		private void OnTriggerEnter2D(Collider2D other)
		{
			//make grounded = true if player is touching the ground
			if (other.GetComponent<Ground>())
			{
				if (!grounded)
				{
					grounded = true;
					animator.SetBool("Grounded", true);
					animator.SetBool("JumpDown", false);
				}
			}

			//end the game if the player hits an obstacle
			if (other.GetComponent<Obstacle>())
			{
				gameManager.GameOver();
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			//make grounded = false if player is not touching the ground
			if (other.GetComponent<Ground>())
			{
				grounded = false;
				animator.SetBool("Grounded", false);
			}
		}

		#endregion
	}
}
