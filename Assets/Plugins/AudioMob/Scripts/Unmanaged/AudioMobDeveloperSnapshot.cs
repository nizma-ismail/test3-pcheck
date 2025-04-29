using AudioMob.Internal;
using AudioMob.Internal.Debugging;
using AudioMob.Internal.Utility;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace AudioMob.Unmanaged
{
	/// <summary>
	/// AudioMob will save setting information to help with debugging, only in Editor and Development Builds.
	/// </summary>
	public static class AudioMobDeveloperSnapshot
	{
		private const string NoResultsString = "No results found.";
		
		/// <summary>
		/// Creates a development snapshot for the development build.
		/// </summary>
		public static void CreateDevelopmentSnapshotForBuild(IAudioMobSettings settings)
		{
			if (Debug.isDebugBuild)
			{
				settings.DevelopmentSnapshot = CreateDevelopmentSnapshot();
			}
			else
			{
				settings.DevelopmentSnapshot = null;
			}
		}

		/// <summary>
		/// Creates and saves a development snapshot for use in logging.
		/// </summary>
		public static void CreateDevelopmentSnapshotForEditor()
		{
			IAudioMobSettings settings = AudioMobSettings.Instance;

			if (settings != null)
			{
				settings.DevelopmentSnapshot = CreateDevelopmentSnapshot();
			}
		}
		
		/// <summary>
		/// Creates a snapshot of PlayerSettings and validation results for the development build.
		/// </summary>
		private static DevelopmentSnapshot CreateDevelopmentSnapshot()
		{
#if UNITY_EDITOR
			return new DevelopmentSnapshot()
			{
				allowUnsafeCode = PlayerSettings.allowUnsafeCode,
				androidScriptingBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android).ToString(),
				iosScriptingBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.iOS).ToString(),
				androidApiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.Android).ToString(),
				iosApiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(BuildTargetGroup.iOS).ToString(),
				minSdkVersion = PlayerSettings.Android.minSdkVersion.ToString(),
				targetOSVersionString = PlayerSettings.iOS.targetOSVersionString,
				targetArchitectures = PlayerSettings.Android.targetArchitectures.ToString(),
				androidScriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android),
				iosScriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS),
				stripEngineCode = PlayerSettings.stripEngineCode,
				androidManagedStrippingLevel = PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.Android).ToString(),
				iosManagedStrippingLevel = PlayerSettings.GetManagedStrippingLevel(BuildTargetGroup.iOS).ToString(),
				androidTVCompatibility = PlayerSettings.Android.androidTVCompatibility,
				useOnDemandResources = PlayerSettings.iOS.useOnDemandResources,
				locationUsageDescription = PlayerSettings.iOS.locationUsageDescription,
				requiresPersistentWiFi = PlayerSettings.iOS.requiresPersistentWiFi
			};
#else
			return null;
#endif
		}
	}
}
