using UnityEngine;

namespace AudioMobExamples.FrogRunner
{
	public class Despawner : MonoBehaviour
	{
		private void OnTriggerEnter2D(Collider2D other)
		{
			//destroy an obstacle if it touches this despawner
			if (other.GetComponent<Obstacle>())
			{
				Destroy(other.gameObject);
			}
		}
	}
}
