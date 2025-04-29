using System;
using AudioMob.Internal;
using AudioMob.Internal.Editor;
using AudioMob.Internal.Utility;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace AudioMob.Unmanaged.Editor
{
	/// <summary>
	/// AudioMob Preprocess scripts for builds. Called before every build.
	/// </summary>
	public class AudioMobPreprocessBuild : IPreprocessBuildWithReport
	{
		// Required for IPreprocessBuildWithReport interface implementation, left as default.
		public int callbackOrder => 0;

		// We need to get PlayerSettings.muteOtherAudioSources at runtime, however there is no way to do so unless we save the value ourselves.
		// Before every build, ensure that the AudioMob settings stores the PlayerSetting's value for this field.
		public void OnPreprocessBuild(BuildReport report)
		{
			SavePlayerSettings();
			CompileTimeChecks compileTimeChecks = new CompileTimeChecks();
			compileTimeChecks.CheckIfAudioIsDisabled();
			
#if UNITY_ANDROID
			compileTimeChecks.CheckIfMuteOtherAudioSourcesIsEnabled();
#endif
			
			compileTimeChecks.CheckIfTestServerIsSelectedForProductionBuild();
		}
		
		private static void SavePlayerSettings()
		{
			try
			{
				IAudioMobSettings settings = AudioMobSettings.Instance;

				AudioMobDeveloperSnapshot.CreateDevelopmentSnapshotForBuild(settings);

				bool muteOtherAudioSources = PlayerSettings.muteOtherAudioSources;
				if (settings.MuteOtherAudioSources != muteOtherAudioSources)
				{
					try
					{
						EditorUtility.SetDirty(settings as AudioMobSettings);
					}
					catch (Exception)
					{
						AMDebug.LogError($"{AMDebug.Prefix} Failed to set AudioMob Settings asset as dirty, type is not AudioMobSettings");
					}
					
					settings.MuteOtherAudioSources = PlayerSettings.muteOtherAudioSources;
				}

				AssetDatabase.SaveAssets();
			}
			catch (Exception e)
			{
				AMDebug.LogError($"Unable to update muteOtherAudioSources AudioMob Settings: {e.Message}");
			}
		}
	}
}
