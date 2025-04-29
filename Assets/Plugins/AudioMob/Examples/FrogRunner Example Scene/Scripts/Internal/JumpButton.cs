using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


namespace AudioMobExamples.FrogRunner
{
	/// <summary>
	/// Class attached a transparent button placed in the back of the canvas.
	/// Allows the user to jump by tapping the screen
	/// </summary>
	public class JumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		private bool isPressed; // Bool indicating whether button is being pressed
		public bool IsPressed => isPressed;

		/// <summary>
		/// Event called when button is pressed
		/// </summary>
		/// <param name="eventData"> Data about the button press (unused in this method) </param>
		public void OnPointerDown(PointerEventData eventData)
		{
			isPressed = true;
		}

		/// <summary>
		/// Event called when button is released
		/// </summary>
		/// <param name="eventData"> Data about the button press (unused in this method) </param>
		public void OnPointerUp(PointerEventData eventData)
		{
			isPressed = false;
		}

		private void Update()
		{
			// An alternate way of jumping by using the space bar instead of pressing the button
			// Implementation depends on whether you are using the new or old input system

#if ENABLE_INPUT_SYSTEM // NEW input system
			if (Keyboard.current.spaceKey.wasPressedThisFrame)
			{
				isPressed = true;
			}

			if (Keyboard.current.spaceKey.wasReleasedThisFrame)
			{
				isPressed = false;
			}
#else // OLD input system
			if (Input.GetKeyDown("space"))
			{
				isPressed = true;
			}

			if (Input.GetKeyUp("space"))
			{
				isPressed = false;
			}
#endif
		}
	}
}
