using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityEngine.InputSystem;
#endif

namespace AudioMobExamples.PaddleBall
{
	public class PlayerPaddle : MonoBehaviour
	{
		#region Variables

		[SerializeField]
		[Tooltip("Reference to the game manager")]
		private GameManager gameManager;

		[SerializeField]
		[Tooltip("Reference to the tabletop collider")]
		private BoxCollider tableCollider;

		[Header("Child GameObject Components")]
		[SerializeField]
		[Tooltip("Left component of paddle")]
		private GameObject paddleLeft;

		[SerializeField]
		[Tooltip("Right component of paddle")]
		private GameObject paddleRight;

		[SerializeField]
		[Tooltip("Center component of paddle")]
		private GameObject paddleCenter;


		private BoxCollider playerCollider; // Reference to the player paddle collider
		private Camera cam;                 // Reference to the main camera

		private Plane paddlePlane; // The plane that the mouse/touch is raycasted to, to determine position of paddle 
		private bool isDragging;   // Is true when the player is dragging the paddle

		private float lastXPosition; // Keeps track of the x position of the pointer/ touch in world space every frame

		private Vector3 initialPaddleCenterScale;   // The initial scale of the center component of the paddle
		private Vector3 initialPaddleLeftPosition;  // The initial position of the left component of the paddle
		private Vector3 initialPaddleRightPosition; // The initial position of the right component of the paddle

#if ENABLE_INPUT_SYSTEM
		[Header("Click Action")]
		[SerializeField]
		[Tooltip("Action which is triggered from a mouse click or finger press")]
		private InputAction clickAction;

		public bool clickPressedDown;
#endif

		#endregion


		#region Paddle Movement Methods

		private void Start()
		{
			playerCollider = GetComponent<BoxCollider>();
			cam = Camera.main;
			paddlePlane = new Plane(Vector3.up, transform.position.y);

			SaveInitialPaddleState();
			SetUpClickAction();
		}

		void Update()
		{
			if (!gameManager.IsGameOver)
			{
				// Player input is designed to work for both new and old input systems, in editor and on mobile devices
				// Receive location of mouse pointer/ touch in world space
				Ray ray;
#if ENABLE_INPUT_SYSTEM
				ray = NewInputSystemRegisterInput();
#else
				ray = OldInputSystemRegisterInput();
#endif

				// Once input is received, move paddle if necessary
				if (isDragging)
				{
					DragPaddle(ray);
				}
			}
		}

#if ENABLE_INPUT_SYSTEM
		/// <summary>
		/// Processes the player's mouse / touch input using Unity's new input system
		/// </summary>
		/// <returns> A ray, casted from the camera to the mouse/ touch input </returns>
		private Ray NewInputSystemRegisterInput()
		{

	#if UNITY_EDITOR
			// Cast a ray from camera to mouse pointer
			Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue()); 
	#else
			// Create default ray which points away from table
			Ray ray = cam.ScreenPointToRay(cam.transform.position); 
			
			// If there is a touch active, adjust ray to point from screen to touch position
			if (clickPressedDown)
			{
				ray = cam.ScreenPointToRay(Touch.activeTouches[0].screenPosition); 
			}
					
	#endif
			// Player has started dragging if they have clicked the playerpaddle
			if (clickPressedDown && !isDragging && paddlePlane.Raycast(ray, out float clickDistance))
			{
				isDragging = true;
				lastXPosition = ray.GetPoint(clickDistance).x; // Record the position where they began dragging 
			}
    
			// Is no longer dragging if player releases mouse
			else if (!clickPressedDown && isDragging) 
			{
				isDragging = false;
			}

			return ray;
		}
#endif

#if !ENABLE_INPUT_SYSTEM
		/// <summary>
		/// Processes the player's mouse / touch input using Unity's old input system
		/// </summary>
		/// <returns> A ray, casted from the camera to the mouse/ touch input </returns>
		private Ray OldInputSystemRegisterInput()
		{
			Ray ray = cam.ScreenPointToRay(Input.mousePosition); // Cast a ray from camera to mouse pointer 

			// Player has started dragging if they have clicked the playerpaddle
			if (Input.GetMouseButtonDown(0) && !isDragging && paddlePlane.Raycast(ray, out float clickDistance))
			{
				isDragging = true;
				lastXPosition = ray.GetPoint(clickDistance).x; // Record the position where they began dragging 
			}

			// Is no longer dragging if player releases mouse
			else if (Input.GetMouseButtonUp(0) && isDragging)
			{
				isDragging = false;
			}

			return ray;
		}
#endif

		/// <summary>
		/// Method called to update paddle position according to mouse/ touch input
		/// </summary>
		/// <param name="ray">Ray cast from the camera to the mouse/ touch input</param>
		private void DragPaddle(Ray ray)
		{
			if (paddlePlane.Raycast(ray, out float dragDistance))
			{
				float newXPos = ray.GetPoint(dragDistance).x; // Get current pointer position
				float deltaXPos = newXPos - lastXPosition;    // Get change in pointer position since last frame
				lastXPosition = newXPos;

				// Update paddle position by the amount the pointer position changed since last frame
				UpdatePaddlePosition(deltaXPos);
			}
		}


		/// <summary>
		/// Limits the horizontal position that the paddle can be dragged to according to the paddle and table width
		/// </summary>
		/// <param name="requestedXPosition"> Paddle position before being bounded by edges of the table</param>
		/// <returns> The new position of the paddle, which will never go past the edges of the table </returns>
		private float LimitPaddleXPosition(float requestedXPosition)
		{
			float paddleWidth = playerCollider.bounds.extents.x;
			float tableWidth = tableCollider.bounds.extents.x;

			return Mathf.Clamp(requestedXPosition, -tableWidth + paddleWidth, tableWidth - paddleWidth);
		}
		
		/// <summary>
		/// Updates the paddle position based on additional input and limits it within bounds
		/// </summary>
		/// <param name="deltaXPos"></param> Change in pointer position since last frame
		private void UpdatePaddlePosition(float deltaXPos = 0f)
		{
			Vector3 paddlePosition = transform.position;
			paddlePosition = new Vector3(LimitPaddleXPosition(paddlePosition.x + deltaXPos),
				paddlePosition.y, paddlePosition.z);
			transform.position = paddlePosition;
		}
		
		/// <summary>
		/// Creates the action which responds to both mouse and touch presses
		/// </summary>
		private void SetUpClickAction()
		{
#if ENABLE_INPUT_SYSTEM // Only do this is if the new input system has been enabled
			clickAction = new InputAction(type: InputActionType.Button);

			clickAction.AddBinding("<Mouse>/press");
			clickAction.AddBinding("<Touchscreen>/press");
				
			clickAction.started += ctx => clickPressedDown = true;
			clickAction.canceled += ctx => clickPressedDown = false;
			
			clickAction.Enable();
#endif
		}

		#endregion


		#region Paddle Scaling Methods

		/// <summary>
		/// Saves the initial scale of paddle's center component and initial position of paddle's outer components.
		/// Useful for resetting paddle back to initial state after an ad has been played. 
		/// </summary>
		private void SaveInitialPaddleState()
		{
			initialPaddleCenterScale = paddleCenter.transform.localScale;
			initialPaddleLeftPosition = paddleLeft.transform.localPosition;
			initialPaddleRightPosition = paddleRight.transform.localPosition;
		}

		/// <summary>
		/// Increases the size of the paddle using a specified multiplier 
		/// </summary>
		/// <param name="multiplier"> The paddle size multiplier </param>
		public void EnlargePaddle(float multiplier)
		{
			// Increase x scale of centre component of paddle
			Vector3 centerLocalScale = paddleCenter.transform.localScale;
			float newXScale = centerLocalScale.x * multiplier;
			centerLocalScale = new Vector3(newXScale, centerLocalScale.y, centerLocalScale.z);
			paddleCenter.transform.localScale = centerLocalScale;

			// Reposition left and right components of paddle
			float newXPosition = (newXScale - 1f) / 5f;

			Vector3 leftLocalPosition = paddleLeft.transform.localPosition;
			leftLocalPosition = new Vector3(-newXPosition, leftLocalPosition.y, leftLocalPosition.z);
			paddleLeft.transform.localPosition = leftLocalPosition;

			Vector3 rightLocalPosition = paddleRight.transform.localPosition;
			rightLocalPosition = new Vector3(newXPosition, rightLocalPosition.y, rightLocalPosition.z);
			paddleRight.transform.localPosition = rightLocalPosition;

			// Update the size of the paddle collider
			RescalePaddleCollider();
			
			// Limit paddle within board
			UpdatePaddlePosition();
		}

		/// <summary>
		/// Called When ad has finished playing.
		/// Resets paddle back to its original size.
		/// </summary>
		public void ResetPaddle()
		{
			paddleCenter.transform.localScale = initialPaddleCenterScale;
			paddleLeft.transform.localPosition = initialPaddleLeftPosition;
			paddleRight.transform.localPosition = initialPaddleRightPosition;

			RescalePaddleCollider();
		}

		/// <summary>
		/// rescales the paddle collider to fit the whole paddle after its center has been resized
		/// </summary>
		private void RescalePaddleCollider()
		{
			// Find total length of all child colliders
			float totalLength = 0;
			totalLength += paddleLeft.GetComponent<BoxCollider>().size.x;
			totalLength += paddleRight.GetComponent<BoxCollider>().size.x;
			totalLength += paddleCenter.GetComponent<BoxCollider>().size.x * paddleCenter.transform.localScale.x;

			// Update parent collider to be the same size as calculated total length
			Vector3 playerColliderSize = playerCollider.size;
			playerColliderSize = new Vector3(totalLength, playerColliderSize.y, playerColliderSize.z);
			playerCollider.size = playerColliderSize;
		}

		#endregion
	}
}
