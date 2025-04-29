using AudioMob;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace AudioMobExamples.FrogRunner
{
	/// <summary>
	/// This class demonstrates some of AudioMob's key features in the context of a simple game.
	/// Read the full API here: https://developer.audiomob.io/dashboard/plugin-api/latest
	///
	/// This implementation plays a skippable audio ad at random time intervals.
	/// There is no reward given to the user for listening to these ads all the way through.
	///
	/// The plugin's DontDestroyOnLoad option has been enabled to allow ads to persist through scene resets
	/// This can be seen by viewing the AudioMobPlugin in the inspector
	/// </summary>
	public class FrogRunnerAdManager : MonoBehaviour
	{
		private const int DefaultTimeToNextAd = 10;

		#region Variables

		private static FrogRunnerAdManager instance;
		
		[SerializeField]
		[Tooltip("The minimum time until next ad should be played")]
		private float minTime;

		[SerializeField]
		[Tooltip("The maximum time until next ad should be played")]
		private float maxTime;

		private float showAdTimer;  // To keep track of how long it has been since ad last finished
		private float timeToNextAd; // The time to wait until next ad should be played
		private bool adRequested;   // Keeps track of whether an ad has just been requested
		private bool adPauseState;  // Keeps track of AdPause state button pressed.
		private int interval = 50;
		private Text adInfoText = null;
		
		[Header("Ad Progress Bar")]
		private Image progressBar; // The Progress Bar that will fill up as the ad plays

		public Image ProgressBar
		{
			set => progressBar = value;
		}

		private GameObject progressBarHolder; // The canvas object that holds the progress bar and its frame

		public GameObject ProgressBarHolder
		{
			set => progressBarHolder = value;
		}

		[Header("Game Manager")]
		private GameManager gameManager; // Reference to the game manager 

		public GameManager GameManager
		{
			set => gameManager = value;
		}
		
		#endregion


		#region Events

		/// <summary>
		/// Event called by AudioMobPlugin. Is invoked when an ad request has started. 
		/// </summary>
		private void OnAdRequestStarted()
		{
			adRequested = true;
			
			if (adInfoText != null)
			{
				adInfoText.text = string.Empty;
			}
		}


		/// <summary>
		/// Event called by AudioMobPlugin. Is invoked when an ad request has been completed 
		/// </summary>
		/// <param name="result"> Status of the result of the ad request</param>
		/// <param name="audioAd"> Details about the requested ad</param>
		private void OnAdRequestCompleted(AdRequestResult result, IAudioAd audioAd)
		{
			string alert = string.Empty;
			switch (result)
			{
				case AdRequestResult.NoAdAvailable:
					alert= "No ad available";
					adRequested = false;
					ResetAdTimer();
					break;
				case AdRequestResult.SkippableRequestVolumeNotAudible:
					alert = "Volume Not Audible";
					adRequested = false;
					ResetAdTimer();
					break;
				case AdRequestResult.Failed:
					alert = "Ad failed to load";
					adRequested = false;
					ResetAdTimer();
					break;
			}

			if (adInfoText != null && !string.IsNullOrEmpty(alert))
			{
				adInfoText.text = alert;
			}
		}

		/// <summary>
		/// Event called by AudioMobPlugin. Is invoked when an ad begins to play
		/// </summary>
		/// <param name="ad"> Details of the started ad instance </param>
		private void OnAdPlaybackStarted(IAudioAd ad)
		{
			progressBarHolder.SetActive(true);
			//Check if the game is paused when the ad starts playing
			if (gameManager.Paused)
			{
				ToggleAdPauseState();
			}
		}

		/// <summary>
		/// Event called by AudioMobPlugin. Is invoked when an ad finishes playing
		/// </summary>
		/// <param name="result"> The result of the ad playback </param>
		private void OnAdPlaybackCompleted(AdPlaybackResult result)
		{
			if (progressBarHolder != null)
			{
				progressBarHolder.SetActive(false);
			}

			ResetAdTimer();
			
			adRequested = false;
		}

		#endregion


		#region Methods
		
		private void Start()
		{
			adInfoText = GameObject.Find("Alert Text")?.GetComponent<Text>();
			
			if (instance != null && instance != this)
			{
				Destroy(gameObject); // This GameObject already exists in the scene, so delete this instance
			}
			else
			{
				// This is the first instance of this GameObject, so make it a singleton
				DontDestroyOnLoad(this);
				instance = this;

				// Subscribe at SceneManager.sceneLoaded to get a function called every time we load the scene
				SceneManager.sceneLoaded += OnSceneLoaded;

				// Assign the AudioMobPlugin to a variable and the callbacks we'll need
				AudioMobPlugin.Instance.OnAdRequestStarted += OnAdRequestStarted;
				AudioMobPlugin.Instance.OnAdRequestCompleted += OnAdRequestCompleted;
				AudioMobPlugin.Instance.OnAdPlaybackStarted += OnAdPlaybackStarted;
				AudioMobPlugin.Instance.OnAdPlaybackCompleted += OnAdPlaybackCompleted;
			}

			Debug.Assert(AudioMobPlugin.Instance != null, "AudioMob Plugin is null.");
			
			// When game starts, if there's no ad playing, calculate the time until the next ad plays
			if (! AudioMobPlugin.Instance.AdHasBegunPlaying)
			{
				CalcTimeToNextAd();
			}
			
			

			adPauseState = false;
		}
		
		private void Update()
		{
			showAdTimer += Time.deltaTime; // Update the timer
			
			// No need to check if it's time to play an ad every single frame.
			if (Time.frameCount % interval == 0)
			{
				PlayAudioAdOnTime();
			}

			if (AudioMobPlugin.Instance.AdHasBegunPlaying)
			{
				// Make sure the progress bar is active
				if (! progressBarHolder.activeSelf)
				{
					progressBarHolder.SetActive(true);
				}

				// Fill the progress bar according to how much of the ad has played so far
				float barFillValue;
				barFillValue = Mathf.Clamp01(1 - AudioMobPlugin.Instance.GetTimeRemainingForCurrentAd() / AudioMobPlugin.Instance.Duration);

				progressBar.fillAmount = barFillValue;
			}
		}

		private void PlayAudioAdOnTime()
		{
			// If enough time has passed and device volume is above threshold, request and play an ad
			if (showAdTimer >= timeToNextAd && ! adRequested)
			{
				RequestAndPlayAudioAd();
			}
		}

		private void OnDestroy()
		{
			// unSubscribe at SceneManager.sceneLoaded to get a function called every time we load the scene
			SceneManager.sceneLoaded -= OnSceneLoaded;
		
			AudioMobPlugin.Instance.OnAdRequestCompleted -= OnAdRequestCompleted;
			AudioMobPlugin.Instance.OnAdPlaybackStarted -= OnAdPlaybackStarted;
			AudioMobPlugin.Instance.OnAdPlaybackCompleted -= OnAdPlaybackCompleted;
		}

		// <summary>
		// The reference to adInfoText needs to be re-created when loading a new scene.
		// </summary>
		private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
		{
			adInfoText = GameObject.Find("Alert Text")?.GetComponent<Text>();
		}

		/// <summary>
		/// Asks AudioMobPlugin to request and play an ad, and tracks whether ad has been requested 
		/// </summary>
		public void RequestAndPlayAudioAd()
		{
			AudioMobPlugin.Instance.RequestAndPlayAd();
		}

		/// <summary>
		/// Calculates the time in seconds that should go by before next ad plays, assigns it to timeToNextAd.
		///
		/// In this demo, the time until a new ad is served ranges between 3-7 seconds.
		///
		/// In production, we recommend serving no more than 3-5 ads
		/// in any 15-20 minute session to maximise player retention.
		/// </summary>
		private void CalcTimeToNextAd()
		{
			timeToNextAd = Random.Range(minTime, maxTime);
		}

		/// <summary>
		/// Resets the timer and calculates a new wait time for the next ad
		/// </summary>
		private void ResetAdTimer()
		{
			showAdTimer = 0;
			timeToNextAd = DefaultTimeToNextAd;
		}

		/// <summary>
		/// Activated by pressing the pause button.
		/// Pauses an ad if it was playing, resumes it if it was paused
		/// </summary>
		public void ToggleAdPauseState()
		{
			adPauseState = ! adPauseState; // Toggle the Ad Pause state.

			// Always call the PauseAd() method when button is clicked
			// because this needs to be added in AdPauseReasons list irrespective of Silent mode state.
			if (adPauseState)
			{
				AudioMobPlugin.Instance.PauseAd();
			}
			else
			{
				AudioMobPlugin.Instance.ResumePausedAd();
			}
		}

		#endregion
	}
}
