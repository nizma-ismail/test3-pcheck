using AudioMob.Internal;
using UnityEngine;

namespace AudioMob.Unmanaged
{
	/// <summary>
	/// Location services for AudioMob.
	/// If location services are disabled, the location code won't be compiled to device.
	/// </summary>
	public class AudioMobLocationService : ILocationService
	{
		/// <summary>
		/// Specifies whether location service is enabled.
		/// </summary>
		public bool IsEnabledByUser
		{
			get
			{
#if AUDIOMOB_USE_LOCATION_SERVICES
                return Input.location.isEnabledByUser;
#else
				return false;
#endif
			}
		}

		/// <summary>
		/// Returns location service status.
		/// </summary>
		public GeoStatus Status
		{
			get
			{
#if AUDIOMOB_USE_LOCATION_SERVICES
                switch (Input.location.status)
                {
                    case LocationServiceStatus.Failed:
                        return GeoStatus.Failed;
                    case LocationServiceStatus.Initializing:
                        return GeoStatus.Initializing;
                    case LocationServiceStatus.Stopped:
                        return GeoStatus.Stopped;
                    case LocationServiceStatus.Running:
                        return GeoStatus.Running;
                    default:
                        return GeoStatus.Failed;
                }
#else
				return GeoStatus.Stopped;
#endif
			}
		}

		/// <summary>
		/// Starts location service updates.
		/// </summary>
		public void Start(float desiredAccuracyInMeters)
		{
#if AUDIOMOB_USE_LOCATION_SERVICES
            Input.location.Start(desiredAccuracyInMeters);
#endif
		}

		/// <summary>
		/// Last measured device geographical location.
		/// </summary>
		public LocationInfo LastData
		{
			get
			{
#if AUDIOMOB_USE_LOCATION_SERVICES
                return Input.location.lastData;
#else
				return new LocationInfo();
#endif
			}
		}
	}
}
