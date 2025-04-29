using UnityEngine;

namespace AudioMobExamples.FrogRunner
{
	public class BGScroller : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("Two instances of the same background image")]
		private GameObject[] bgObjects;

		[SerializeField]
		[Tooltip("Reference to Main Camera")]
		private Camera cam;

		[SerializeField]
		[Tooltip("Speed that this background element will move")]
		private float scrollSpeed;

		private float imgWidth; //The width of the background objects

		private void Start()
		{
			//get image width
			imgWidth = bgObjects[0].GetComponent<SpriteRenderer>().bounds.size.x;
		}

		private void Update()
		{
			MoveObjects();
			ReplaceObject();
		}

		/// <summary>
		/// Moves both background objects at the same speed
		/// </summary>
		private void MoveObjects()
		{
			foreach (GameObject bgObject in bgObjects)
			{
				bgObject.transform.position -= new Vector3(scrollSpeed * Time.deltaTime, 0, 0);
			}
		}

		/// <summary>
		/// Checks if a background object has gone off camera, and subsequently moves it to the right of the other object
		/// </summary>
		private void ReplaceObject()
		{
			float camHeight = cam.orthographicSize * 2;
			float camWidth = camHeight * cam.aspect;

			foreach (GameObject bgObject in bgObjects)
			{
				if ((bgObject.transform.position.x + (imgWidth / 2)) < (0 - (camWidth / 2)))
				{
					bgObject.transform.position += new Vector3(imgWidth * bgObjects.Length, 0, 0);
				}
			}
		}
	}
}
