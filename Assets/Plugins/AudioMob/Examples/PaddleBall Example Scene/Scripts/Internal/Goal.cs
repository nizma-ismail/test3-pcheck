using UnityEngine;

namespace AudioMobExamples.PaddleBall
{
	public class Goal : MonoBehaviour
    {
    	[SerializeField]
    	[Tooltip("Reference to game manager")]
    	private GameManager gameManager;
    	
    	private void OnTriggerEnter(Collider other)
    	{
	        // The ball has two colliders, so only register a hit for one of the colliders
	        if (other.GetComponent<BallMovement>() && other.isTrigger)
	        {
		        gameManager.BallInGoal(GetComponent<BoxCollider>());
	        }
        }
    }
}

