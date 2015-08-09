using UnityEngine;
using System.Collections;

public class UFOBullet : MonoBehaviour
{
	public Vector3 thrust;
	public Quaternion heading;
	new private Camera camera;
	private Vector3 cameraBottomLeft;
	private Vector3 cameraTopRight;
	private Vector3 originInScreenCoords;
	private float timer;
	private float deathPeriod;

	void Start ()
	{
		timer = 0.0f;
		deathPeriod = 2.0f;
		// Travel straight in the X-axis
		thrust.x = 400.0f;
		// Do not passively decelerate
		gameObject.GetComponent<Rigidbody> ().drag = 0;
		// Set the direction it will travel in
		gameObject.GetComponent<Rigidbody> ().MoveRotation (heading);
		// Apply thrust once, no need to apply it again since
		// it will not decelerate
		gameObject.GetComponent<Rigidbody> ().AddRelativeForce (thrust);
	}

	void Update ()
	{
		timer += Time.deltaTime;
		if (timer > deathPeriod) {
			timer = 0;
			Destroy (gameObject);
		}
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
		
		// Check the top wall
		if (transform.position.z > cameraTopRight.z) {
			position.z = cameraBottomLeft.z + 0.1f;
		}
		
		// Check the bottom wall
		if (transform.position.z < cameraBottomLeft.z) {
			position.z = cameraTopRight.z - 0.1f;
		}
		
		// Check the left wall
		if (transform.position.x < cameraBottomLeft.x) {
			position.x = cameraTopRight.x - 0.1f;
		}
		
		// Check the right wall
		if (transform.position.x > cameraTopRight.x) {
			position.x = cameraBottomLeft.x + 0.1f;
		}
		
		// Set the transformation's position
		transform.position = position;
	}
	
	void OnCollisionEnter (Collision collision)
	{
		Collider collider = collision.collider;
		if (collider.CompareTag ("Asteroids")) {
			Asteroid roid = collider.gameObject.GetComponent< Asteroid > ();
			roid.Die ();
			Destroy (gameObject);
			
		} else if (collider.CompareTag ("MediumAsteroids")) {
			MediumAsteroid roid = collider.gameObject.GetComponent<MediumAsteroid> ();
			roid.Die ();
			Destroy (gameObject);
		} else {
			Debug.Log ("Collided with " + collider.tag);
		}
	}

	public void Die ()
	{
		Destroy (gameObject);
	}
}