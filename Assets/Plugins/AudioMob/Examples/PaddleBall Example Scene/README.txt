This is one of two example scenes which demonstrate how the AudioMob plugin could be used in the context of a simple game.
PaddleBall is a game similar to pong, where the player bounces a ball in their opponent's goal. 
Every time a goal is scored, the player is given the opportunity to listen to a rewarded, non-skippable ad. As they listen to the ad the size of their paddle is progressively enlarged.

The full AudioMob Plugin API can be found here: https://developer.audiomob.io/dashboard/plugin-api/latest

You will find two relevant scripts in the Scripts folder: PaddleBallAdManager.cs and AudioMobMixerDuckExample.cs.
Between them, the following API features are implemented: 
- Creating a custom implementation of the AudioMob ad UI
	- The banner ad texture is displayed not through a UI canvas, but on a billboard in the game's world space. 
- Custom events that trigger at specified times during an ad
	- Custom events provide instant rewards to player by progressively increasing the size of their paddle as the ad plays
- Setting ad requests to be from the live server or the test server through code
	- The ad manager checks an example config file, which determines which server should be used
- Transitioning between audio mixer snapshots when an ad begins playing 
	- Game SFX are lowered when an ad begins playing so that the ad can be heard properly

Specific implementation details are provided as comments within the scripts. 
