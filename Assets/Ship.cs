using UnityEngine;
using System.Collections;
public class Ship : MonoBehaviour {
	public Vector3 forceVector;
	public float rotationSpeed;
	public float rotation;
	public GameObject bullet; // the GameObject to spawn
	public AudioClip fireNoise;

	public GameObject deathExplosion;
	public AudioClip deathKnell;

	private Camera mCamera;
	public Camera fpCamera;
	private Vector3 cameraBottomLeft;
	private Vector3 cameraTopRight;
	private Vector3 originInScreenCoords;

	private bool invincible; //for protecting during spawn and respawn
	public GameObject shield;

	// Use this for initialization
	void Start () {
		mCamera = Camera.main;
		mCamera.enabled = true;
		fpCamera.enabled = false;

		//GameObject shield = GameObject.FindGameObjectWithTag ("Shield");

		invincible = true;
		Invoke ("DisableInvincible", 5);

		// Vector3 default initializes all components to 0.0f
		forceVector.x = 2.0f;
		rotationSpeed = 2.0f;
	}
	/* forced changes to rigid body physics parameters should be done
through the FixedUpdate() method, not the Update() method
*/
	void FixedUpdate()
	{
		// force thruster
		if( Input.GetAxisRaw("Vertical") > 0 )
		{
			gameObject.rigidbody.AddRelativeForce(forceVector);
		}
		if( Input.GetAxisRaw("Horizontal") > 0 )
		{
			rotation += rotationSpeed;
			Quaternion rot = Quaternion.Euler(new
			                                  Vector3(0,rotation,0));
			gameObject.rigidbody.MoveRotation(rot);
			//gameObject.transform.Rotate(0, 2.0f, 0.0f );
		}
		else if( Input.GetAxisRaw("Horizontal") < 0 )
		{
			rotation -= rotationSpeed;
			Quaternion rot = Quaternion.Euler(new
			                                  Vector3(0,rotation,0));
			gameObject.rigidbody.MoveRotation(rot);
			//gameObject.transform.Rotate(0, -2.0f, 0.0f );
		}
	}

	// Update is called once per frame

	void Update () {
		// Firing bullet
		// Do not let the player fire the bullet if they are invincible
		if(Input.GetButtonDown("Fire1") && invincible == false)
		{
			//check if you can fire first
			GameObject globalObj = GameObject.Find("GlobalObject");
			Global g = globalObj.GetComponent<Global>();

			Debug.Log("Current number of bullets: " + g.numberOfBullets);

			if(g.numberOfBullets >= g.maxBullets) {
				//then you can't fire.
				Debug.Log ("Can't fire!" + rotation);
			}
			else {
				//Debug.Log ("Fire! " + rotation);
				AudioSource.PlayClipAtPoint(fireNoise,
				                            new Vector3(0, 20, 0));
				//increase your count
				g.numberOfBullets++;

				/* we don’t want to spawn a Bullet inside our ship, so some
				Simple trigonometry is done here to spawn the bullet
				at the tip of where the ship is pointed.
				*/
				Vector3 spawnPos = gameObject.transform.position;
				spawnPos.x += 1.5f * Mathf.Cos(rotation * Mathf.PI/180);
				spawnPos.z -= 1.5f * Mathf.Sin(rotation * Mathf.PI/180);
				// instantiate the Bullet
				GameObject bulletObj = Instantiate(bullet, spawnPos,
				                             Quaternion.identity) as GameObject;

				// get the Bullet Script Component of the new Bullet instance
				Bullet b = bulletObj.GetComponent<Bullet>();
				// set the direction the Bullet will travel in
				Quaternion rot = Quaternion.Euler(new
				                                  Vector3(0,rotation,0));
				b.heading = rot;
			}	
		}

		//Switch to main overhead camera
		if (Input.GetKeyDown ("1")) {
			mCamera.enabled = true;
			fpCamera.enabled = false;
		}

		//switch to first person mode
		if(Input.GetKeyDown("2")) {
			fpCamera.enabled = true;
			mCamera.enabled = false;
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

		if (invincible == true) {
			return;
		}

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
			// The ship is destroyed (add sound)
			//Destroy(gameObject);

			AudioSource.PlayClipAtPoint(deathKnell,
			                            gameObject.transform.position );
			Instantiate(deathExplosion, gameObject.transform.position,
			            Quaternion.AngleAxis(-90, Vector3.right) );

			//Now check how many lives we have left
			if(g.livesLeft == 0) {
				//No lives, you die, game over.
				Destroy(gameObject);
				Application.LoadLevel("GameOverScene");
			}
			else {
				//subtract a life, then respawn
				//add explosion sound and effect
				g.livesLeft--;
				Respawn();
			}
		}
		else if(collider.CompareTag("MediumAsteroids")) {
			MediumAsteroid roid = collider.gameObject.GetComponent<MediumAsteroid>();
			roid.Die();

			AudioSource.PlayClipAtPoint(deathKnell,
			                            gameObject.transform.position );
			Instantiate(deathExplosion, gameObject.transform.position,
			            Quaternion.AngleAxis(-90, Vector3.right) );

			//Now check how many lives we have left
			if(g.livesLeft == 0) {
				//No lives, you die, game over.
				Destroy(gameObject);
				Application.LoadLevel("GameOverScene");
			}
			else {
				//subtract a life, then respawn
				//add explosion sound and effect
				g.livesLeft--;
				Respawn();
			}
		}
		else if(collider.CompareTag("UFOBullet")) {
			UFOBullet ufoBullet = collider.gameObject.GetComponent<UFOBullet>();
			Destroy(ufoBullet);

			AudioSource.PlayClipAtPoint(deathKnell,
			                            gameObject.transform.position );
			Instantiate(deathExplosion, gameObject.transform.position,
			            Quaternion.AngleAxis(-90, Vector3.right) );


			//Now check how many lives we have left
			if(g.livesLeft == 0) {
				//No lives, you die, game over.
				Destroy(gameObject);
				Application.LoadLevel("GameOverScene");
			}
			else {
				//subtract a life, then respawn
				//add explosion sound and effect
				g.livesLeft--;
				Respawn();
			}
		}
		else if(collider.CompareTag("UFO")) {
			UFO ufo = collider.gameObject.GetComponent<UFO>();
			ufo.Die();

			AudioSource.PlayClipAtPoint(deathKnell,
			                            gameObject.transform.position );
			Instantiate(deathExplosion, gameObject.transform.position,
			            Quaternion.AngleAxis(-90, Vector3.right) );

			//Now check how many lives we have left
			if(g.livesLeft == 0) {
				//No lives, you die, game over.
				Destroy(gameObject);
				Application.LoadLevel("GameOverScene");
			}
			else {
				//subtract a life, then respawn
				//add explosion sound and effect
				g.livesLeft--;
				Respawn();
			}
		}
		else if(collider.CompareTag("Multiplier")) {
			Multiplier multiplier = collider.gameObject.GetComponent<Multiplier>();
			multiplier.Die();
		}
		else
		{
			// if we collided with something else, print to console
			// what the other thing was
			Debug.Log ("Collided with " + collider.tag);
		}
	}

	public void EnableInvincible() {
		invincible = true;
		shield.SetActive (true);
		shield.renderer.enabled = true;

		Invoke ("DisableInvincible", 5);
	}

	void DisableInvincible() {
		shield = GameObject.FindGameObjectWithTag ("Shield");
		invincible = false;

		shield.SetActive (false);
	}

	void Respawn() {
		invincible = true;
		shield.SetActive(true);

		//reset multiplier count
		GameObject globalObj = GameObject.Find("GlobalObject");
		Global g = globalObj.GetComponent<Global>();
		g.multiplier = 1;

		//Respawn is gonna have to have a way to keep the ship from dying right away
		//add respawn noise
		rigidbody.Sleep (); //This should reset the forces on the ship.

		//I guess I would set position back to origin?
		transform.position = new Vector3 (0.0f, 0.0f, 0.0f);

		//invoke method to reenable collider in 3 seconds
		Invoke ("DisableInvincible", 5);
	}
}