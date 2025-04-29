using System;
using AudioMob.Internal;
using UnityEngine;
using AudioMob.Internal.DevicePlugins;
using AudioMob.Internal.Utility;

namespace AudioMob.Unmanaged
{
	/// <summary>
	/// Initializes the AudioMob Unity Plugin.
	/// </summary>
	public class AudioMobPluginInitialization
	{
		/// <summary>
		/// The mediation adaptor SDK to use in this project.
		/// </summary>
		public static IMediationAdapterSdk MediationAdapterSdk { private get; set; }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializePluginAutomatically()
		{
			try
			{
				if (AudioMobSettings.Instance.AutoInitializePlugin)
				{
					InitializePlugin();
				}
			}
			catch (Exception exception)
			{
				AMDebug.LogError($"Failed to auto-initialize AudioMob: {exception.Message}");
			}
		}

		/// <summary>
		/// Initialize the AudioMob Plugin.
		/// This should be done early in the game's runtime lifecycle, ideally at launch.
		/// This initialization can be set to happen automatically by checking "Automatically Initialize Plugin" in the AudioMob Settings Window.
		/// Optional callback invoked with 'true' on initialization success and 'false' if the initialization is un-successful.
		/// </summary>
		public static void InitializePlugin(Action<bool> callback = null)
		{
			try
			{
#if UNITY_EDITOR
				IAudioPlugin audioPlugin = new EditorAudioPlugin();
				IOpenMeasurementSdk openMeasurementSdk = null;
#elif UNITY_IOS
				IAudioPlugin audioPlugin = new IosAudioPlugin();
				IOpenMeasurementSdk openMeasurementSdk = new IosOpenMeasurementSdk();
#elif UNITY_ANDROID
				IAudioPlugin audioPlugin = new AndroidAudioPlugin();
				IOpenMeasurementSdk openMeasurementSdk = new AndroidOpenMeasurementSdk();
#else
				IAudioPlugin audioPlugin = new StandaloneAudioPlugin();
				IOpenMeasurementSdk openMeasurementSdk = null;
#endif

				ILocationService locationService = new AudioMobLocationService();

				audioPlugin.Start(nameof(DeviceAudioCheck));
				AudioMobPluginController.Init(audioPlugin, AudioMobSettings.Instance, locationService, openMeasurementSdk, MediationAdapterSdk);
				InvokeInitCallback(callback, true);
			}
			catch (Exception exception)
			{
				IExceptionHandler exceptionHandler = new ExceptionHandler();
				exceptionHandler.RecordException(new NativePluginInitialisationFailedException($"Failed to initialize AudioMob: {exception.Message}"));
				InvokeInitCallback(callback, false);
			}
		}

		private static void InvokeInitCallback(Action<bool> callback, bool initSuccess)
		{
			try
			{
				callback?.Invoke(initSuccess);
			}
			catch (Exception exception)
			{
				AMDebug.LogError($"Error caught in initialization callback invoke: {exception.Message}");
			}
		}
	}
}
