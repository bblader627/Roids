using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
	public Vector3 thrust;
	public Quaternion heading;
	private Camera camera;
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
		GameObject globalObject = GameObject.Find ("GlobalObject");
		Global global = globalObject.GetComponent<Global> ();

		timer += Time.deltaTime;
		if (timer > deathPeriod) {
			timer = 0;
			global.numberOfBullets--;
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
		originInScreenCoords = Camera.main.WorldToScreenPoint (new Vector3 (0, 0, 0));
		
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

		transform.position = position;
	}

	void OnCollisionEnter (Collision collision)
	{
		GameObject globalObject = GameObject.Find ("GlobalObject");
		Global global = globalObject.GetComponent<Global> ();
		Collider collider = collision.collider;

		if (collider.CompareTag ("Asteroids")) {
			Asteroid roid = collider.gameObject.GetComponent< Asteroid > ();

			roid.Die ();
			global.numberOfBullets--;
			global.score += (50 * global.multiplier);
			Destroy (gameObject);
		} else if (collider.CompareTag ("MediumAsteroids")) {
			MediumAsteroid roid = collider.gameObject.GetComponent<MediumAsteroid> ();

			roid.Die ();
			global.numberOfBullets--;
			global.score += (100 * global.multiplier);
			Destroy (gameObject);
		} else if (collider.CompareTag ("UFO")) {
			UFO ufo = collider.gameObject.GetComponent<UFO> ();

			global.numberOfBullets--;
			ufo.Die ();
			Destroy (gameObject);
		} else if (collider.CompareTag ("UFOBullet")) {
			UFOBullet ufoBullet = collider.gameObject.GetComponent<UFOBullet> ();

			global.numberOfBullets--;
			ufoBullet.Die ();
			Destroy (gameObject);
		} else {
			Debug.Log ("Collided with " + collider.tag);
		}
	}
}