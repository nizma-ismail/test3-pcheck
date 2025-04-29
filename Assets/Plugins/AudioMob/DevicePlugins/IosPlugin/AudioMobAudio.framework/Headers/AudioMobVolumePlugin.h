//
//  AudioMobVolumePlugin.h
//  AudioMobAudio
//
//  Created by Manjit Bedi on 2020-05-11.
//  Copyright © 2020 AudioMob. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import <MediaPlayer/MediaPlayer.h>

#ifndef AudioMobVolumePlugin_h
#define AudioMobVolumePlugin_h

extern NSString * const AUMVolumeChangedNotification;
extern NSString * const AUMSilentModeChangedNotification;

extern NSString * const AUMVolumeKey;

@interface AUMVolumePlugin: NSObject

/**
   Class method, returns true if silent mode is activated.
 */
+ (BOOL)silentModeIsActivated;

/**
 Class method, detects whether audio source is playing in background.
 and detects whether another app, with a nonmixable audio session, is playing audio.
 */
+ (BOOL)audioSourceIsPlaying:(BOOL) muteOtherAudioSources;

/**
   Class method, get the current system volume.
*/
+ (float)getSystemAudioVolume:(BOOL) setActiveSession;

/**
    Set the system volume on a device. This employs a hack as there is no  public Apple API.
 */
+ (void)setSystemVolume:(float) volume;

/**
    This method gets called when the ad audio begins playing, tells background music to dip.
*/
+ (void)audioFocusOn:(BOOL) respectSilentMode;

/**
    This method would get called when ad audio has finished playing.
 */
+ (void)audioFocusOff;

/**
    Returns true if there is a value stored in NSUserDefaults for the given key.
 */
+ (bool)hasUserDefaultValue:(NSString *) key;

/**
    Returns the string stored in NSUserDefaults for the given key or null if it doesn't exist.
 */
+ (NSString *)readUserDefaultStringValue:(NSString *) key;

/**
    Returns the int stored in NSUserDefaults for the given key or null if it doesn't exist.
 */
+ (int)readUserDefaultIntValue:(NSString *) key;

/**
    Returns network type.
 */
+ (NSString *)getNetworkType;

/**
    Returns carrier name.
 */
+ (NSString *)getCarrierName;

/**
    Returns device model.
 */
+ (NSString *)getDeviceModel;

/**
   Returns true if silent mode is activated.
 */
+ (BOOL)headphonesAreConnected;

/**
   Initiates a clickthrough action.
 */
+ (void)performClickthrough:(NSString *)targetUrl;

@end


#endif /* AudioMobVolumePlugin_h */
