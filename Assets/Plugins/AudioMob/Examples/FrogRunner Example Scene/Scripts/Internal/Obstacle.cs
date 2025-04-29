using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AudioMobExamples.FrogRunner
{
	public class Obstacle : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("The speed obstacles will move at")]
		private float moveSpeed;

		void Update()
		{
			//Move the obstacle according to moveSpeed
			transform.position -= new Vector3(moveSpeed * Time.deltaTime, 0, 0);
		}
	}
}
