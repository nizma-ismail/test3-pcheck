using AudioMob;
using UnityEngine;
using UnityEngine.UI;

namespace AudioMobExamples
{
	/// <summary>
	/// This class demonstrates AudioMob's key features via a basic implementation.
	/// Read the full API here: https://developer.audiomob.io/dashboard/plugin-api/latest
	/// </summary>
	[AddComponentMenu("AudioMob/Examples/Ad Manager")]
	public class AudioMobExampleAdManager : MonoBehaviour
	{
		[SerializeField]
    	[Tooltip("Used to display information about the state of AudioMob.")]
    	public Text infoText;

	    [SerializeField]
	    [Tooltip("Used to display information about the version of AudioMob Plugin.")]
	    private Text pluginVersionText; 
        
        private void Awake()
        {
	        Debug.Assert(infoText, $"[{nameof(AudioMobExampleAdManager)}] {nameof(infoText)} is null.");
	        Debug.Assert(pluginVersionText, $"[{nameof(AudioMobExampleAdManager)}] {nameof(pluginVersionText)} is null.");
        }

        private void Start()
		{
			// Assign callbacks to the AudioMob events.
			AudioMobPlugin.Instance.OnAdRequestCompleted += OnAdRequestCompleted;
			AudioMobPlugin.Instance.OnAdPlaybackStarted += OnAdPlaybackStarted;
			AudioMobPlugin.Instance.OnAdPlaybackCompleted += OnAdPlaybackCompleted;
			AudioMobPlugin.Instance.OnSkipButtonAvailable += OnSkipButtonAvailable;
			
			// Get the ad availability for this user.
			GetAdAvailability();

			pluginVersionText.text = AudioMobPlugin.PluginVersion;
		}
        
        private void OnDestroy()
        {
	        AudioMobPlugin.Instance.OnAdRequestCompleted -= OnAdRequestCompleted;
	        AudioMobPlugin.Instance.OnAdPlaybackStarted -= OnAdPlaybackStarted;
	        AudioMobPlugin.Instance.OnAdPlaybackCompleted -= OnAdPlaybackCompleted;
	        AudioMobPlugin.Instance.OnSkipButtonAvailable -= OnSkipButtonAvailable;
        }

        /// <summary>
        /// Request and play an audio ad using AudioMob.
        /// </summary>
		public void RequestAndPlayAudioMobAd()
		{
			infoText.text = "Ad Requested";
			AudioMobPlugin.Instance.RequestAndPlayAd();
		}

		private static void GetAdAvailability()
		{
			AudioMobPlugin.Instance.GetAdAvailability((adAvailability) =>
			{
				/* Write code here to:
				 - Show or hide a rewarded ad button to the user through-out runtime.  */
				Debug.Log($"[{nameof(AudioMobExampleAdManager)}] Ad Availability Result: Audio ads are {(!adAvailability.AdsAvailable ? "not " : string.Empty)}available for this user.");
			});
		}

		private void OnAdRequestCompleted(AdRequestResult adRequestResult, IAudioAd audioAd) // This callback is invoked when the ad request has been completed.
		{
			if (adRequestResult == AdRequestResult.Finished) // The ad has loaded
			{
				/* Write code here to:
				 - Get info about the audio via the IAudioAd argument, such as audio length or expiry time
				 - Check the time that the ad has taken to load. */
			}
			else // The ad didn't load, either because it failed to download or the ad bid wasn't successful.
			{
				/* Write code here to:
				- Disable any ad related UI you might be showing.
				- Continue the game without giving the player a reward. */
				
				switch (adRequestResult)
				{
					case AdRequestResult.NoAdAvailable:
						infoText.text = "No Ad Available";
						break;
					case AdRequestResult.SkippableRequestVolumeNotAudible:
						infoText.text = "Volume Not Audible";
						break;
					case AdRequestResult.Failed:
						infoText.text = "Ad Request Failed";
						break;
					case AdRequestResult.FrequencyCapReached:
						infoText.text = "Frequency Cap Reached";
						break;
				}
			}
		}

		private void OnAdPlaybackStarted(IAudioAd audioAd) // This callback is invoked when the ad begins to play.
		{
			/* Write code here to:
			 - Turn down your game volume.
			 - Turn off your game music.
			 - Give your players an instant reward? */

			infoText.text = "Ad Started Playing";
		}

		private void OnAdPlaybackCompleted(AdPlaybackResult adPlaybackResult) // This callback is invoked when the ad finishes playing.
		{
			/* Write code here to:
			  - Turn up your game volume.
			  - Turn on your game music.
			  - Disable any ad related UI you might be showing. */

			if (adPlaybackResult == AdPlaybackResult.Finished) // The ad was heard all the way to the end.
			{
				/* Write code here to:
				   - Give your player a reward for listening to the ad? */

				infoText.text = "Ad Finished Playing";
			}
			else if (infoText != null)
			{
				switch (adPlaybackResult)
				{
					case AdPlaybackResult.Skipped:
						infoText.text = "Ad Skipped";
						return;
					case AdPlaybackResult.Stopped:
						infoText.text = "Ad Stopped";
						break;
					case AdPlaybackResult.Canceled:
						infoText.text = "Ad Canceled";
						break;
					case AdPlaybackResult.Failed:
						infoText.text = "Ad Failed";
						break;
				}
			}
		}

		private void OnSkipButtonAvailable()
		{
			/* Write code here to:
			 - Display your own skip button if you have one. */

			infoText.text = "Skip Button Available";
		}
	}
}
