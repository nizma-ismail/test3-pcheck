using UnityEngine;

namespace AudioMobExamples.PaddleBall
{
	public class EnemyMovement : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Reference to the current ball in the scene")]
		private Transform ball;

		public Transform Ball
		{
			set => ball = value;
		}

		[SerializeField]
		[Tooltip("Reference to the table collider")]
		private BoxCollider tableCollider;
		
		[SerializeField]
		[Range(0f, 1f)]
		[Tooltip("How good the enemy AI is from 0 (Incompetent) to 1 (Literally Unbeatable)")]
		private float enemyTrackingAccuracy;

		private Vector3 currentPos; // current position of enemy paddle

		void Start()
		{
			currentPos = transform.position; // Set current position to be where the paddle already is
		}

		void Update()
		{
			if (ball != null)
			{
				// Interpolate between current x position and ball's x position according to enemyTrackingAccuracy
				currentPos.x = LimitPaddleXPosition(
					Mathf.Lerp(currentPos.x, ball.position.x, enemyTrackingAccuracy));
				transform.position = currentPos;
			}


		}
		
		/// <summary>
		/// Limits the horizontal position that the paddle be at according to the paddle and table width
		/// </summary>
		/// <param name="requestedXPosition"> Paddle position before being bounded by edges of the table</param>
		/// <returns> The new position of the paddle, which will never go past the edges of the table </returns>
		private float LimitPaddleXPosition(float requestedXPosition)
		{
			float paddleWidth = GetComponent<BoxCollider>().bounds.extents.x;
			float tableWidth = tableCollider.bounds.extents.x;

			return Mathf.Clamp(requestedXPosition, -tableWidth + paddleWidth, tableWidth - paddleWidth);
		}
	}
}
