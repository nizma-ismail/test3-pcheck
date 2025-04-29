#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import <MediaPlayer/MediaPlayer.h>
#import <simd/SIMD.h>

// The iOS system volume device plug-in is a iOS framework
#import <AudioMobAudio/AudioMobVolumePlugin.h>

#import "UnityAppController.h"

// This header includes some methods that are being used to communicate with & direct the Unity game engine.
#include "UnityInterface.h"

@interface AudioMobPlugin: NSObject

/**
    The name of the game object which will receive messages when the system volume is changed.  This object would exist
    in the current Unity game scene.
 */
@property (nonatomic) NSString* delegateGOName;

/**
    Specify the format precision when the volume value is converted to a string.
 */
@property (nonatomic) int precision;

@property (strong, nonatomic) AUMVolumePlugin *plugIn;

@end

/**
    An iOS plug-in being used with Unity to observer and get the system volume of the iOS device.
 */
@implementation AudioMobPlugin


/**
    Init an instance of the object.
 */
- (id)init {
    self = [super init];
    
    // Default precision for the number of decimals for a string representation of the system volume.
    self.precision = 2;
    
    // Create an instance of the plug- in object
    self.plugIn = [[AUMVolumePlugin alloc] init];

    // The volume plug-in will post a notification when the system volume changes.
    [[NSNotificationCenter defaultCenter] addObserver:self
             selector:@selector(receiveVolumeChangeNotification:)
             name:AUMVolumeChangedNotification
             object:nil];
      
    // Add observer for silent mode changes
    [[NSNotificationCenter defaultCenter] addObserver:self
             selector:@selector(receiveSilentModeChangedNotification:)
             name:AUMSilentModeChangedNotification
             object:nil];

	// Add observer for audio session interruption
	[[NSNotificationCenter defaultCenter] addObserver:self
			selector:@selector(receiveAudioSessionInterruptedNotification:)
			name:AVAudioSessionInterruptionNotification
			object:nil];
                          
    return self;
}

/**
   @brief handle the system volume change on the device notification.
*/
- (void)receiveVolumeChangeNotification:(NSNotification *) notification {
    if (notification.userInfo != NULL) {
        float volume = [(NSNumber *) notification.userInfo[AUMVolumeKey] floatValue];
       
        // Convert from NSStrings to C strings
        const char *formattedNumber = [[NSString stringWithFormat:@"%.*f", self.precision, volume] UTF8String];
        const char *delegateGameObjectName = [self.delegateGOName UTF8String];
               
        // Send a message to the Unity engine run-time. VolumeChanged is the method name on the recipient game object.
        UnitySendMessage(delegateGameObjectName, "VolumeChanged", formattedNumber);
    }
}

/**
   @brief handle the silent mode changing notification.
*/
- (void)receiveSilentModeChangedNotification:(NSNotification *) notification {
    if (notification.userInfo != NULL) {
        bool silentMode = [(NSNumber *) notification.userInfo[@"silentMode"] boolValue];

        // The UnitySendMessage works with C strings
        // The NSString needs to be converted
        // Convert the booleans to a string;  using a JSON chracter string as precedent
        NSString *temp =  silentMode ? @"true" : @"false";
        const char *boolString = [temp UTF8String];
        const char *delegateGameObjectName = [self.delegateGOName UTF8String];

        // Send a message to the Unity engine run-time. SilentModeChanged is the method name on the recipient game object.
        UnitySendMessage(delegateGameObjectName, "SilentModeChanged", boolString);
    }
}

/**
@brief handle the audio session interrupted notification.
*/
- (void)receiveAudioSessionInterruptedNotification:(NSNotification *) notification {

	if(notification.userInfo != NULL){

		const char *delegateGameObjectName = [self.delegateGOName UTF8String];

		NSDictionary *userInfo = notification.userInfo;

		NSNumber *typeValue = userInfo[AVAudioSessionInterruptionTypeKey];
		AVAudioSessionInterruptionType type = (AVAudioSessionInterruptionType)[typeValue unsignedIntegerValue];

		switch (type) {
			case AVAudioSessionInterruptionTypeBegan:
			UnitySendMessage(delegateGameObjectName, "AudioSessionInterruption", "");
			break;
			
			default:
			break;
		}
	}
}


@end

// Keep a reference to the plug-in instance.
static AudioMobPlugin *audioMobPlugin = nil;

// Change value at run-time as needed.
static bool respectSilentMode = false;

// MARK: utils

// Converts C style string to NSString
NSString* CreateNSString (const char* string) {
    if (string) {
        return [NSString stringWithUTF8String: string];
    } else {
        return [NSString stringWithUTF8String: ""];
    }
}

// Copies the string so Unity can use it and handle it's memory management.
char* copyStringForUnity(const char* string)
{
    if (string == NULL)
        return NULL;

    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);

    return res;
}



// MARK: bridging functions to the Unity game engine.

extern "C"
{
    /**
        @brief create the plug-in & set the name for the delegate game object (GO).
    */
    void Audiomob_Start(const char *delegateGOName)
    {
        if (audioMobPlugin == nil) {
            audioMobPlugin = [[AudioMobPlugin alloc] init];
            audioMobPlugin.delegateGOName = CreateNSString(delegateGOName);
        }
    }

    /**
        @brief get the current system volume.
    */
    float Audiomob_GetSystemAudioVolume(bool setActiveSession) {
        return [AUMVolumePlugin getSystemAudioVolume:setActiveSession];
    }

    /**
     @brief detects whether audio source with/without a nonmixable audio session, is playing in background.
     */
    bool Audiomob_AudioSourceIsPlaying(bool muteOtherAudioSources){
        return [AUMVolumePlugin audioSourceIsPlaying:muteOtherAudioSources];
    }

    /**
        @brief returns true if silent mode is activated.
    */
    bool Audiomob_SilentModeIsActivated() {
        return [AUMVolumePlugin silentModeIsActivated];
    }

    /**
        @brief set the number of digits for precision when creating a string from the float value for the system volume.
    */
    void Audiomob_SetSystemVolumePrecision(int precision) {
        if (audioMobPlugin != nil) {
            audioMobPlugin.precision = precision;
        }
    }
    
    
    void Audiomob_RespectSilentMode(bool inputRespectSilentMode)
    {
        respectSilentMode = inputRespectSilentMode;
    }
    
    /**
        @brief this method is called just before ad audio is played with true & called after ad audio has finished with false.
        @see AudioFocusOn, AudioFocusOff
    */
    void Audiomob_SetAudioFocus(bool focus) {
        if (focus) {
            // Ask OS plugin to turn focus on (reacts differently based on whether or not we respect silent mode).
            [AUMVolumePlugin audioFocusOn: respectSilentMode];
            
        } else {
            [AUMVolumePlugin audioFocusOff];
        }
    }
    
    /**
        @brief set the sytsem volume in iOS;  this employs a hack but it works.
        @see SetSystemVolume
    */
    void Audiomob_SetSystemVolume(float volume) {
        volume = simd_clamp(volume, 0.0f, 1.0f);
        [AUMVolumePlugin setSystemVolume: volume];
    }

    /**
        @brief Returns true if there is a value stored in NSUserDefaults for the given key.
    */
    bool Audiomob_HasUserDefaultValue(const char * key) {
        NSString * stringKey = CreateNSString(key);
        return [AUMVolumePlugin hasUserDefaultValue: stringKey];
    }

    /**
        @brief Returns the string stored in NSUserDefaults for the given key or null if it doesn't exist.
    */
    char* Audiomob_ReadUserDefaultStringValue(const char * key) {
        NSString * stringKey = CreateNSString(key);
        NSString * stringValue = [AUMVolumePlugin readUserDefaultStringValue: stringKey];
        return copyStringForUnity([stringValue UTF8String]);
    }

    /**
        @brief Returns the int stored in NSUserDefaults for the given key or null if it doesn't exist.
    */
    int Audiomob_ReadUserDefaultIntValue(const char * key) {
        NSString * stringKey = CreateNSString(key);
        return [AUMVolumePlugin readUserDefaultIntValue: stringKey];
    }

    /**
        @brief Returns network type.
    */
    char* Audiomob_GetNetworkType() {
        NSString * stringValue = [AUMVolumePlugin getNetworkType];
        return copyStringForUnity([stringValue UTF8String]);
    }

    /**
        @brief Returns carrier name.
    */
    char* Audiomob_GetCarrierName() {
        NSString * stringValue = [AUMVolumePlugin getCarrierName];
        return copyStringForUnity([stringValue UTF8String]);
    }

    /**
        @brief Returns device model.
    */
    char* Audiomob_GetDeviceModel() {
        NSString * stringValue = [AUMVolumePlugin getDeviceModel];
        return copyStringForUnity([stringValue UTF8String]);
    }

    /**
        @brief Returns true if silent mode is activated.
    */
    bool Audiomob_HeadphonesAreConnected() {
        
        return [AUMVolumePlugin headphonesAreConnected];
    }

    /**
        @brief Initiates a clickthrough.
    */
    void Audiomob_PerformClickthrough(const char * targetUrl) {
        NSString * compatibleUrl = CreateNSString(targetUrl);
        [AUMVolumePlugin performClickthrough: compatibleUrl];
    }
}
