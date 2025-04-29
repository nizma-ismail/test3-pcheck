This is one of two example scenes which demonstrate how the AudioMob plugin could be used in the context of a simple game.
FrogRunner is a very simple game in which the player taps the screen to jump over incoming obstacles.
Every 3 - 7 seconds, a non-rewarded, skippable ad is automatically played in the background. The player can skip these ads after 5 seconds.
The ads are delivered more often here than we would recommend in a published game. To maximise player retention we recommend serving no more than 3-5 ads in any 15-20 minute session.

The full AudioMob Plugin API can be found here: https://developer.audiomob.io/dashboard/plugin-api/latest

You will find relevant scripts in the root of the Scripts folder: FrogRunnerAdManager.cs, AudioMobMixerDuckExample.cs, and PauseAdReference.cs.
Between them, the following API features are implemented: 
- Skip Ad
	- The AudioMob Plugin is configured for skippable ads (this is done in the inspector when the AudioMobPlugin is selected)
- DeviceVolumeAboveThreshold
	- The game checks whether the player's mobile phone volume is high enough to play an ad, and will not begin playback if it isn't 
- Pause Ad functionality
	- When the game is paused, the ad is also paused
- GetTimeRemainingForCurrentAd() and Duration 
	- These two values are used to visually display how much of the current ad has been played so far in the form of a bar at the top of the screen
- Transitioning between audio mixer snapshots when an ad begins playing 
	- Game SFX are lowered when an ad begins playing so that the ad can be heard properly

Specific implementation details are provided as comments within the scripts. 
