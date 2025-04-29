using UnityEngine;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace AudioMobExamples.FrogRunner
{
	public class ConfigureEventSystem : MonoBehaviour
	{
#if ENABLE_INPUT_SYSTEM
		private void Awake()
		{
			if (gameObject.GetComponent<StandaloneInputModule>())
			{
				Destroy(gameObject.GetComponent<StandaloneInputModule>());
	            gameObject.AddComponent<InputSystemUIInputModule>();
			}
		}
#endif
	}
}
