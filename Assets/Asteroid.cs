using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour {

	// Use this for initialization
	public int pointValue;
	public GameObject deathExplosion;
	public AudioClip deathKnell;
	public Vector3 forceVector;

	private Camera camera;
	private Vector3 cameraBottomLeft;
	private Vector3 cameraTopRight;
	private Vector3 originInScreenCoords;
	
	public GameObject mediumAsteroid;

	void Start () {
		float rotation = Random.Range (0.0f, 360.0f);
		forceVector.x = 200.0f;
		Quaternion rot = Quaternion.Euler(new Vector3(0,rotation,0));

		gameObject.rigidbody.MoveRotation(rot);
		gameObject.rigidbody.AddRelativeForce(forceVector);

	}

	void FixedUpdate() {

	}

	// Update is called once per frame
	void Update () {
	
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
			Debug.Log("Exited top of window.");
		}
		
		// Check the bottom wall
		if (transform.position.z < cameraBottomLeft.z) {
			position.z = cameraTopRight.z - 0.1f;
			Debug.Log("Exited bottom of window.");
		}
		
		// Check the left wall
		if(transform.position.x < cameraBottomLeft.x) {
			position.x = cameraTopRight.x - 0.1f;
			Debug.Log ("Exited left of window.");
		}
		
		// Check the right wall
		if (transform.position.x > cameraTopRight.x) {
			position.x = cameraBottomLeft.x + 0.1f;
			Debug.Log ("Exited right of window.");
		}
		
		// Set the transformation's position
		transform.position = position;
	}
	
	public void Die()
	{
		AudioSource.PlayClipAtPoint(deathKnell,
		                            gameObject.transform.position );
		Instantiate(deathExplosion, gameObject.transform.position,
		            Quaternion.AngleAxis(-90, Vector3.right) );
		GameObject obj = GameObject.Find("GlobalObject");
		Global g = obj.GetComponent<Global>();
		g.score += pointValue;
		Destroy (gameObject);

		//then have to make new objects... for hte medium asteroids. how? Instansiate?
		Instantiate(mediumAsteroid, gameObject.transform.position, Quaternion.AngleAxis(-90, Vector3.right));
	}
}
