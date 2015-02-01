using UnityEngine;
using System.Collections;

public class UFOBullet : MonoBehaviour {
	public Vector3 thrust;
	public Quaternion heading;
	
	private Camera camera;
	private Vector3 cameraBottomLeft;
	private Vector3 cameraTopRight;
	private Vector3 originInScreenCoords;
	private float timer;
	private float deathPeriod;
	
	// Use this for initialization
	void Start () {
		timer = 0.0f;
		deathPeriod = 2.0f;
		// travel straight in the X-axis
		thrust.x = 400.0f;
		// do not passively decelerate
		gameObject.rigidbody.drag = 0;
		// set the direction it will travel in
		gameObject.rigidbody.MoveRotation(heading);
		// apply thrust once, no need to apply it again since
		// it will not decelerate
		gameObject.rigidbody.AddRelativeForce(thrust);
	}
	// Update is called once per frame
	void Update () { 
		GameObject globalObj = GameObject.Find("GlobalObject");
		Global g = globalObj.GetComponent<Global>();
		//Physics engine handles movement, empty for now. }
		timer += Time.deltaTime;
		if (timer > deathPeriod) {
			timer = 0;
			Destroy (gameObject);
		}
	}
	
	void LateUpdate() {
		// Position updates when going outside screen bounds
		CheckForWrapAround ();
	}
	
	void CheckForWrapAround () {
		Vector3 position = transform.position;
		originInScreenCoords =
			Camera.main.WorldToScreenPoint(new Vector3(0,0,0));
		
		cameraBottomLeft = Camera.main.ScreenToWorldPoint(new Vector3 (0, 0, originInScreenCoords.z));
		cameraTopRight = Camera.main.ScreenToWorldPoint(new Vector3 (Camera.main.GetScreenWidth (), Camera.main.GetScreenHeight (), originInScreenCoords.z));
		
		// Check the top wall
		if (transform.position.z > cameraTopRight.z) {
			position.z = cameraBottomLeft.z + 0.1f;
		}
		
		// Check the bottom wall
		if (transform.position.z < cameraBottomLeft.z) {
			position.z = cameraTopRight.z - 0.1f;
		}
		
		// Check the left wall
		if(transform.position.x < cameraBottomLeft.x) {
			position.x = cameraTopRight.x - 0.1f;
		}
		
		// Check the right wall
		if (transform.position.x > cameraTopRight.x) {
			position.x = cameraBottomLeft.x + 0.1f;
		}
		
		// Set the transformation's position
		transform.position = position;
	}
	
	void OnCollisionEnter( Collision collision )
	{
		GameObject globalObj = GameObject.Find("GlobalObject");
		Global g = globalObj.GetComponent<Global>();
		// the Collision contains a lot of info, but it’s the colliding
		// object we’re most interested in.
		Collider collider = collision.collider;
		if( collider.CompareTag("Asteroids") )
		{
			Asteroid roid =
				collider.gameObject.GetComponent< Asteroid >();
			// let the other object handle its own death throes
			roid.Die();
			Destroy(gameObject);
			
		}
		else if(collider.CompareTag("MediumAsteroids")) {
			MediumAsteroid roid = collider.gameObject.GetComponent<MediumAsteroid>();
			roid.Die();
			Destroy(gameObject);
		}
		else
		{
			// if we collided with something else, print to console
			// what the other thing was
			Debug.Log ("Collided with " + collider.tag);
		}
	}

	public void Die() {
		Destroy (gameObject);
	}
}