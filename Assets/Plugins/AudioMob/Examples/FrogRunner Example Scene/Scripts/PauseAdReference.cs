using System.Collections;
using System.Collections.Generic;
using AudioMobExamples.FrogRunner;
using UnityEngine;

namespace AudioMobExamples.FrogRunner
{
	/// <summary>
	/// This class exists to find the current instance of the ad manager and trigger the ToggleAdPauseState() method.
	/// Is triggered every time the pause button is pressed.
	/// </summary>
	public class PauseAdReference : MonoBehaviour
	{
		public void TriggerPauseAd()
		{
			FrogRunnerAdManager adManager =
				GameObject.Find("Frog Runner Ad Manager").GetComponent<FrogRunnerAdManager>();
			adManager.ToggleAdPauseState();
		}
	}
}
