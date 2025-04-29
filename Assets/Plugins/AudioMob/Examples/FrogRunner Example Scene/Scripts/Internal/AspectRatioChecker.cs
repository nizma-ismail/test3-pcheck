using UnityEngine;

namespace AudioMobExamples.FrogRunner
{
	public class AspectRatioChecker : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("The warning object that appears if camera is not in portrait")]
		private Camera cam; // Reference to the main camera

		private bool warningShown; // Tracks whether the warning message has already been shown


		void Start()
		{
			cam = Camera.main;
		}

		void Update()
		{
			// If camera's aspect ratio is landscape, show warning message to change game view aspect ratio to portrait
			if (cam.aspect >= 1 && !warningShown)
			{
				Debug.LogWarning("This demo is meant for portrait." +
				                 "\nPlease set game view aspect ratio to be in Portrait.");
				warningShown = true;
			}
			else if (cam.aspect < 1 && warningShown)
			{
				warningShown = false;
			}
		}
	}
}
