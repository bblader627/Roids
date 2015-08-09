using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour
{
	public int pointValue;
	public GameObject deathExplosion;
	public AudioClip deathKnell;
	public Vector3 forceVector;

	private Camera camera;
	private Vector3 cameraBottomLeft;
	private Vector3 cameraTopRight;

	private Vector3 originInScreenCoords;
	public GameObject mediumAsteroid;

	void Start ()
	{
		float rotation = Random.Range (0.0f, 360.0f);
		forceVector.x = 200.0f;
		Quaternion qRotation = Quaternion.Euler (new Vector3 (0, rotation, 0));

		gameObject.GetComponent<Rigidbody> ().MoveRotation (qRotation);
		gameObject.GetComponent<Rigidbody> ().AddRelativeForce (forceVector);
	}

	void LateUpdate ()
	{
		// Position updates when going outside screen bounds
		CheckForWrapAround ();
	}
	
	void CheckForWrapAround ()
	{
		Vector3 position = transform.position;
		originInScreenCoords =
			Camera.main.WorldToScreenPoint (new Vector3 (0, 0, 0));
		
		cameraBottomLeft = Camera.main.ScreenToWorldPoint (new Vector3 (0, 0, originInScreenCoords.z));
		cameraTopRight = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, Screen.height, originInScreenCoords.z));
		
		// Check the top bounds
		if (transform.position.z > cameraTopRight.z) {
			position.z = cameraBottomLeft.z + 0.1f;
		}
		
		// Check the bottom bounds
		if (transform.position.z < cameraBottomLeft.z) {
			position.z = cameraTopRight.z - 0.1f;
		}
		
		// Check the left bounds
		if (transform.position.x < cameraBottomLeft.x) {
			position.x = cameraTopRight.x - 0.1f;
		}
		
		// Check the right bounds
		if (transform.position.x > cameraTopRight.x) {
			position.x = cameraBottomLeft.x + 0.1f;
		}
		
		// Set the transformation's position
		transform.position = position;
	}
	
	public void Die ()
	{
		AudioSource.PlayClipAtPoint (deathKnell,
		                            gameObject.transform.position);
		Instantiate (deathExplosion, gameObject.transform.position,
		            Quaternion.AngleAxis (-90, Vector3.right));

		Destroy (gameObject);

		// Instantiate the medium asteroids
		Instantiate (mediumAsteroid, gameObject.transform.position, Quaternion.AngleAxis (-90, Vector3.right));
	}
}
