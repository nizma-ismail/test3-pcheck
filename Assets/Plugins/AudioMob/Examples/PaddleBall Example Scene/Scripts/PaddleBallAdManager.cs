using AudioMob;
using UnityEngine;
using UnityEngine.UI;

namespace AudioMobExamples.PaddleBall
{
	/// <summary>
	/// This class demonstrates some of AudioMob's more advanced features in the context of a simple game.
	/// Refer to the README.txt located in the PaddleBall example folder for an overview of the features demonstrated.
	/// 
	/// Read the full API here: https://developer.audiomob.io/dashboard/plugin-api/latest
	///
	/// This implementation allows the user to listen to an rewarded ad to increase the size of their paddle,
	/// thus making the game easier and providing an instant reward.
	/// </summary>
	public class PaddleBallAdManager : MonoBehaviour
	{
		#region Variables
		
		[SerializeField]
		[Tooltip("Reference to GameManager object")]
		private GameManager gameManager;

		[SerializeField]
		[Tooltip("Reference to the player's paddle")]
		private GameObject playerPaddle;

		[SerializeField]
		[Tooltip("The particle system attached to the player paddle")]
		private ParticleSystem playerPaddleParticles;

		[SerializeField]
		[Tooltip("Reference to the button which displays rewarded ad")]
		private Button playAdButton;
		
		[SerializeField]
		[Tooltip("Float representing percentage increase in scale of player paddle when an ad is playing")]
		private float playerPaddleScaleMultiplier;
		
		[Header("Playback Listeners")]
		[SerializeField]
		[Tooltip("List of float values (between 0 and 1) which denote when a callback will fire during an ad.\n" +
		         "e.g. 0.5 will fire a callback halfway through ad playback")]
		[Range(0, 1)]
		private float[] playbackListenersTimes;
		
		private IAudioMobPlugin audioMobPlugin;

		public bool IsPlaying // Public getter for the isPlaying variable
		{
			get => audioMobPlugin.AdHasBegunPlaying;
		}

		#endregion


		#region AudioMob Plugin Events

		/// <summary>
		/// Called when ad playback has begun.
		/// Enlarges the paddle while the ad is playing.
		/// </summary>
		private void OnAdPlaybackStarted(IAudioAd audioAd)
		{
			// Register all callbacks to AudioMob playback listeners
			for (int i = 0; i < playbackListenersTimes.Length; i++)
			{
				// Register 'EnlargePaddle' method to AudioMob playback listeners 
				audioMobPlugin.RegisterPlaybackCallback(EnlargePaddle, playbackListenersTimes[i]);
			}
		}
		
		/// <summary>
		/// Called when the ad is paused, removes the enlargement.
		/// </summary>
		private void OnAdPaused(AdPauseReason adPauseReason)
		{
			ResetPaddle();
		}
		
		/// <summary>
		/// Called when the ad is resumed, re-adds the enlargement.
		/// </summary>
		private void OnAdResumed()
		{
			EnlargePaddle();
		}

		/// <summary>
		/// Called when ad playback has been completed
		/// Resets the in-game ad billboard graphic and player paddle size
		/// </summary>
		/// <param name="obj"> Object containing information about how the ad playback was completed</param>
		private void OnAdPlaybackCompleted(AdPlaybackResult obj)
		{
			// This statement just prevents null reference errors if the game is being exited while an ad is playing.
			if (playerPaddle == null)
			{
				return;
			}
			
			// Reset size of paddle
			ResetPaddle();
			
			// UnRegister 'EnlargePaddle' method from AudioMob playback listeners.
			audioMobPlugin.UnregisterPlaybackCallback(EnlargePaddle);
		}

		#endregion


		#region Methods

		private void Start()
		{
			audioMobPlugin = AudioMobPlugin.Instance;
			audioMobPlugin.OnAdPlaybackStarted += OnAdPlaybackStarted;
			audioMobPlugin.OnAdPlaybackCompleted += OnAdPlaybackCompleted;
			audioMobPlugin.OnAdPaused += OnAdPaused;
			audioMobPlugin.OnAdResumed += OnAdResumed;
		}

		/// <summary>
		/// Requests and plays an ad from AudioMob.
		/// Also sets the audio source from which the ad plays to be in the ball
		/// </summary>
		public void PlayAd()
		{
			audioMobPlugin.RequestAndPlayAd();
			HideAdButton();
		}
		
		/// <summary>
		/// Requested when the player/cpu scores.
		/// Shows the 'play ad' button on the screen if an ad is not already playing.
		/// </summary>
		public void ShowAdButton()
		{
			if (!audioMobPlugin.AdHasBegunPlaying)
			{
				playAdButton.gameObject.SetActive(true);
				playAdButton.interactable = true;
			}
		}

		/// <summary>
		/// Called when the an ad starts playing.
		/// Sets the 'play ad' button to inactive.
		/// </summary>
		private void HideAdButton()
		{
			playAdButton.gameObject.SetActive(false);
		}

		/// <summary>
		/// Public method to set the 'play ad' button's interactable status
		/// </summary>
		/// <param name="interactable"> true for interactable, false for uninteractable </param>
		public void SetAdButtonInteractable(bool interactable)
		{
			playAdButton.interactable = interactable;
		}
		
		/// <summary>
		/// Calls EnlargePaddle method of PlayerPaddle which scales up paddle size.
		/// </summary>
		private void EnlargePaddle()
		{
			playerPaddle.GetComponent<PlayerPaddle>().EnlargePaddle(playerPaddleScaleMultiplier);
			playerPaddleParticles.Play();
		}	
		
		/// <summary>
		/// Calls ResetPaddle method of PlayerPaddle which resets paddle to its original size.
		/// </summary>
		private void ResetPaddle()
		{
			playerPaddle.GetComponent<PlayerPaddle>().ResetPaddle();
			playerPaddleParticles.Play();
		}
		
		#endregion
	}
}
