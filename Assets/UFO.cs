using UnityEngine;
using System.Collections;

public class UFO : MonoBehaviour {
	private float timer;
	private float firePeriod;
	private int pointValue;
	private Vector3 forceVector;
	public AudioClip fireNoise;
	public GameObject ufoBullet;

	public GameObject multiplierObject;

	public GameObject deathExplosion;
	public AudioClip deathKnell;

	private Camera camera;
	private Vector3 cameraBottomLeft;
	private Vector3 cameraTopRight;
	private Vector3 originInScreenCoords;
	// Use this for initialization
	void Start () {
		timer = 0;
		firePeriod = 2.0f;
		pointValue = 200;

		float rotation = Random.Range (0.0f, 360.0f);
		forceVector.x = 200.0f;
		Quaternion rot = Quaternion.Euler(new Vector3(0,rotation,0));
		
		gameObject.GetComponent<Rigidbody>().MoveRotation(rot);
		gameObject.GetComponent<Rigidbody>().AddRelativeForce(forceVector);
	}
	
	// Update is called once per frame
	void Update () {

		timer += Time.deltaTime;
		if (timer > firePeriod) {
			timer = 0;
			AudioSource.PlayClipAtPoint(fireNoise,
			                            new Vector3(0, 20, 0));
			
			/* we don’t want to spawn a Bullet inside our ship, so some
				Simple trigonometry is done here to spawn the bullet
				at the tip of where the ship is pointed.
				*/
			
			float bulletRotation = Random.Range (0.0f, 360.0f);
			Vector3 spawnPos = gameObject.transform.position;
			spawnPos.x += 1.5f * Mathf.Cos(bulletRotation * Mathf.PI/180);
			spawnPos.z -= 1.5f * Mathf.Sin(bulletRotation * Mathf.PI/180);
			// instantiate the Bullet
			GameObject ufoBulletObj = Instantiate(ufoBullet, spawnPos,
			                                   Quaternion.identity) as GameObject;
			
			// get the Bullet Script Component of the new Bullet instance
			UFOBullet b = ufoBulletObj.GetComponent<UFOBullet>();
			// set the direction the Bullet will travel in
			Quaternion rot = Quaternion.Euler(new
			                                  Vector3(0,bulletRotation,0));
			b.heading = rot;
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
		cameraTopRight = Camera.main.ScreenToWorldPoint(new Vector3 (Screen.width, Screen.height, originInScreenCoords.z));
		
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

	public void Die()
	{
		AudioSource.PlayClipAtPoint(deathKnell,
		                            gameObject.transform.position );
		Instantiate(deathExplosion, gameObject.transform.position,
		            Quaternion.AngleAxis(-90, Vector3.right) );
		GameObject obj = GameObject.Find("GlobalObject");
		Global g = obj.GetComponent<Global>();
		g.score += (pointValue * g.multiplier);

		/* Spawn 3 multipliers with very tiny velocities going in random directions */
		for (int i = 0; i < 3; i++) {
			float multiplierRotation = Random.Range (0.0f, 360.0f);
			Vector3 spawnPos = gameObject.transform.position;
			spawnPos.x += 0.5f * Mathf.Cos(multiplierRotation * Mathf.PI/180);
			spawnPos.z -= 0.5f * Mathf.Sin(multiplierRotation * Mathf.PI/180);
			GameObject multiplierObj = Instantiate (multiplierObject, spawnPos, Quaternion.identity) as GameObject;

			Multiplier m = multiplierObj.GetComponent<Multiplier>();
			// set the direction the Bullet will travel in
			Quaternion rot = Quaternion.Euler(new
			                                  Vector3(0,multiplierRotation,0));
			//m.heading = rot;
		}

		Destroy (gameObject);
	}
}
