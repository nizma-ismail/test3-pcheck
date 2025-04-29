using AudioMob;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioMobExamples
{
	/// <summary>
	/// An example of ducking the game audio when an AudioMob audio ad plays.
	/// The ad playing snapshot has lowered game volume, to allow the ad to be heard properly.
	/// </summary>
	[AddComponentMenu("AudioMob/Examples/Mixer Ducking")]
	public class AudioMobMixerDuckExample : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("The snapshot that is active while an ad is not playing.")]
		private AudioMixerSnapshot normalMixerSnapshot;

		[SerializeField]
		[Tooltip("The snapshot that is active while an ad is playing.")]
		private AudioMixerSnapshot adPlayingSnapshot;
		
		[SerializeField]
		[Min(0)]
		[Tooltip("How long we want the volume transition to/from muted to last.")]
		private float transitionTime = 0.25f;

		private IAudioMobPlugin audioMobPlugin;
		
		private void Awake()
		{
			Debug.Assert(normalMixerSnapshot, "Normal Mixer Snapshot reference is missing.");
			Debug.Assert(adPlayingSnapshot, "Ad Playing Snapshot reference is missing.");
		}

		private void Start()
		{
			audioMobPlugin = AudioMobPlugin.Instance;
			if (audioMobPlugin == null)
			{
				Debug.LogError("[AudioMobMixerDuckExample] No AudioMobPlugin found in scene.");
				Destroy(gameObject);
				return;
			}
			
			// Put the mixer in the correct state to begin.
			if (audioMobPlugin.AdHasBegunPlaying)
			{
				LowerGameVolumeForAd(null);
			}
			else
			{
				RestoreGameSnapshot();
			}
			
			// Subscribe to the AudioMob events that determine when you need to duck the audio.
			audioMobPlugin.OnAdPlaybackStarted += LowerGameVolumeForAd;
			audioMobPlugin.OnAdPlaybackCompleted += RestoreGameVolume;
		}

		private void OnDestroy()
		{
			// Unsubscribe from the events when the mixer is destroyed.
			if (audioMobPlugin != null)
			{
				audioMobPlugin.OnAdPlaybackStarted -= LowerGameVolumeForAd;
				audioMobPlugin.OnAdPlaybackCompleted -= RestoreGameVolume;
			}
		}

		private void LowerGameVolumeForAd(IAudioAd ad)
		{
			// Transition to the ad playing snapshot, lowering the game volume.
			adPlayingSnapshot.TransitionTo(transitionTime);
		}

		private void RestoreGameVolume(AdPlaybackResult playbackResult)
		{
			// Return the game sounds to their original volume. 
			normalMixerSnapshot.TransitionTo(transitionTime);
		}
		
		// We call this function at Start() just to ensure we are using the right snapshot
		private void RestoreGameSnapshot()
		{
			// Return the game sounds
			normalMixerSnapshot.TransitionTo(0);
		}
	}
}
