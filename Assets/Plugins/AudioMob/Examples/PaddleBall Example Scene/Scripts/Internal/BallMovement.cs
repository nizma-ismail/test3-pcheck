using UnityEngine;
using Random = UnityEngine.Random;

namespace AudioMobExamples.PaddleBall
{
	public class BallMovement : MonoBehaviour
	{
		#region Variables

		[Header("General Movement")]
		[SerializeField]
		[Tooltip("The sphere collider attached to the ball that is a trigger")]
		private SphereCollider ballTriggerCollider;

		[SerializeField]
		[Tooltip("The sphere collider attached to the ball that deals with physics collisions")]
		private SphereCollider ballPhysicsCollider;

		[SerializeField]
		[Tooltip("Multiplier for initial force imposed on the ball")]
		private float initialForceMult;

		[SerializeField]
		[Tooltip("The position the ball will be reset to when respawned")]
		private Vector3 respawnPosition;

		[SerializeField]
		[Tooltip("Value to multiply z component of velocity by every time the ball hits a paddle")]
		private float zVelocityMultiplier;

		[SerializeField]
		[Tooltip("The maximum z component of velocity the ball should be able to travel at")]
		private float maxZVelocity;

		[SerializeField]
		[Tooltip("The maximum x component of velocity the ball should be able to travel at")]
		private float maxXVelocity;


		private Rigidbody ballRigidbody; //The rigidbody component 
		private Vector3 ballVelocity;    // The velocity assigned to the ball every frame
		private Collider lastCollider;   // Keeps track of the last collider the ball hit


		[Header("Wall Collision")]
		[SerializeField]
		[Tooltip("Amount of time ball is allowed to touch the wall before giving it an outward push")]
		private float ballTouchWallMax;

		private float ballTouchWallCount; // Keeps track of how long the ball has been in contact with a wall

		[SerializeField]
		[Tooltip("The radius that the ball's physics collider should expand to when pushing it off the wall")]
		private float physicsColliderExpandRadius;

		private float physicsColliderInitialRadius; // Keeps track of the initial radius of the ball's physics collider


		[Header("AudioSources")]
		[SerializeField]
		[Tooltip("The Audio Source which will play the ball bounce noise")]
		private AudioSource bounceAudioSource;

		#endregion


		#region Collision Methods

		private void OnTriggerEnter(Collider other)
		{
			// Ball has hit a paddle
			if (other.transform.root.GetComponentInChildren<PlayerPaddle>()
			    || other.transform.root.GetComponentInChildren<EnemyMovement>())
			{
				//Only bounce the ball once each time it touches a different paddle (prevents glitchy movement)
				if (other != lastCollider)
				{
					lastCollider = other;

					InvertVelocity(2);      // Invert z direction of ball velocity 
					IncreaseSpeed();        // Make the ball faster
					SetNewXVelocity(other); // Determine the angle the ball will bounce off the paddle
				}
			}
			// Ball has hit a wall
			else if (other.transform.root.GetComponentInChildren<TableWall>())
			{
				InvertVelocity(0); // Invert x direction of ball velocity 
			}

			PlayBounceSound();
		}

		private void OnTriggerStay(Collider other)
		{
			// Time how long ball has been touching wall
			if (other.transform.GetComponent<TableWall>())
			{
				ballTouchWallCount += Time.deltaTime;

				// Push the ball slightly away from the wall if it has been hugging it for too long
				if (ballTouchWallCount > ballTouchWallMax)
				{
					ballPhysicsCollider.radius = physicsColliderExpandRadius;
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			// Ball has stopped touching wall, reset count variables and physics collider radius
			if (other.transform.root.GetComponentInChildren<TableWall>())
			{
				ballTouchWallCount = 0;
				ballPhysicsCollider.radius = physicsColliderInitialRadius;
			}
		}

		#endregion


		#region Ball Physics Methods

		/// <summary>
		/// Invert velocity of ball in a given direction (x,y,z)
		/// </summary>
		/// <param name="vector3Idx">
		/// The index of the Vector3 velocity to be inverted. (0 = x, 1 = y, 2 = z)
		/// </param>
		private void InvertVelocity(int vector3Idx)
		{
			ballVelocity[vector3Idx] = -ballVelocity[vector3Idx];
		}

		/// <summary>
		/// multiplies the z component of the ball's velocity by speedMultiplier so long as it is less than maxZVelocity
		/// </summary>
		private void IncreaseSpeed()
		{
			// Multiply speed if it is less than max
			if (Mathf.Abs(ballVelocity.z) < maxZVelocity)
			{
				ballVelocity.z *= zVelocityMultiplier;
			}

			// If z velocity is greater than max, make it equal to max (respective to direction of ball movement)
			if (ballVelocity.z > 0 && ballVelocity.z > maxZVelocity)
			{
				ballVelocity.z = maxZVelocity;
			}
			else if (ballVelocity.z < 0 && ballVelocity.z < -maxZVelocity)
			{
				ballVelocity.z = -maxZVelocity;
			}
		}

		/// <summary>
		/// Sets the sideways velocity of the ball depending on how close to the center of the paddle the ball was hit.
		/// Closer to the center = less sideways velocity
		/// </summary>
		/// <param name="paddleCollider"> The box collider of the paddle that the ball has just touched</param>
		private void SetNewXVelocity(Collider paddleCollider)
		{
			// Calculate the maximum distance that the ball could be from the paddle
			float paddleColliderExtentsLength = paddleCollider.bounds.extents.x;
			float ballExtentsLength = ballTriggerCollider.bounds.extents.x;
			float maxDistance = paddleColliderExtentsLength + ballExtentsLength;

			// Calculate how far to the left/right the ball is away from the paddle as a ratio of the maxDistance
			float positionDifference = transform.position.x - paddleCollider.transform.position.x;
			float positionRatio = positionDifference / maxDistance;

			// Calculate new x velocity of the ball depending on how far away the ball was from the center of the paddle
			ballVelocity.x = maxXVelocity * positionRatio;
		}

		/// <summary>
		/// Called after every goal.
		/// Resets the velocity and position of the ball.
		/// </summary>
		public void ResetBall()
		{
			transform.position = respawnPosition;
			ballVelocity = Vector3.back * initialForceMult + GetRandomXVelocity(); // Give ball initial velocity

			ballRigidbody.angularVelocity = Vector3.zero; // Reset rotation

			lastCollider = null;
		}

		/// <summary>
		/// Generates a random direction for the ball to start moving in
		/// </summary>
		/// <returns> Vector3 with random x magnitude (bounded) </returns>
		private Vector3 GetRandomXVelocity()
		{
			float xVal = Random.Range(-2f, 2f);
			return new Vector3(xVal, 0, 0);
		}

		#endregion


		#region Other Methods

		void Start()
		{
			ballRigidbody = GetComponent<Rigidbody>(); // Assign rigidbody component
			respawnPosition =
				transform.position; // Set the respawn position of the ball to be the place it is first instantiated
			physicsColliderInitialRadius =
				ballPhysicsCollider.radius; // Assign initial radius of ball's physics collider
			ResetBall();
		}

		void FixedUpdate()
		{
			ballRigidbody.linearVelocity = ballVelocity; //Update the ball's velocity
		}

		void LateUpdate()
		{
			// Freeze X and Z axis of rotation, allowing ball to spin around y axis only
			transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
		}

		/// <summary>
		/// Plays the bounce sound from the bounceAudioSource attached to the ball
		/// </summary>
		private void PlayBounceSound()
		{
			if (gameObject.activeSelf)
			{
				bounceAudioSource.Play();
			}
		}

		#endregion
	}
}
