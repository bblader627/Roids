using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour
{
	public Vector3 forceVector;
	public float rotationSpeed;
	public float rotation;
	public GameObject bullet;
	public AudioClip fireNoise;
	public GameObject deathExplosion;
	public AudioClip deathKnell;
	private Camera mCamera;
	public Camera fpCamera;
	private Vector3 cameraBottomLeft;
	private Vector3 cameraTopRight;
	private Vector3 originInScreenCoords;
	private bool invincible; // For protecting during spawn and respawn
	private bool blinkStarted;

	void Start ()
	{
		mCamera = Camera.main;
		mCamera.enabled = true;
		fpCamera.enabled = false;

		invincible = true;
		blinkStarted = false;
		Invoke ("DisableInvincible", 5);

		forceVector.x = 2.0f;
		rotationSpeed = 2.0f;
	}

	void FixedUpdate ()
	{
		// Force thruster
		if (Input.GetAxisRaw ("Vertical") > 0) {
			gameObject.GetComponent<Rigidbody> ().AddRelativeForce (forceVector);
		}

		if (Input.GetAxisRaw ("Horizontal") > 0) {
			rotation += rotationSpeed;
			Quaternion qRotation = Quaternion.Euler (new Vector3 (0, rotation, 0));
			gameObject.GetComponent<Rigidbody> ().MoveRotation (qRotation);
		} else if (Input.GetAxisRaw ("Horizontal") < 0) {
			rotation -= rotationSpeed;
			Quaternion qRotation = Quaternion.Euler (new Vector3 (0, rotation, 0));
			gameObject.GetComponent<Rigidbody> ().MoveRotation (qRotation);
		}
	}

	void Update ()
	{
		// Firing bullet
		// Do not let the player fire the bullet if they are invincible
		if (Input.GetButtonDown ("Fire1") && invincible == false) {
			GameObject globalObject = GameObject.Find ("GlobalObject");
			Global global = globalObject.GetComponent<Global> ();

			Debug.Log ("Current number of bullets: " + global.numberOfBullets);

			if (global.numberOfBullets >= global.maxBullets) {
				Debug.Log ("Can't fire!" + rotation);
			} else {
				AudioSource.PlayClipAtPoint (fireNoise, new Vector3 (0, 20, 0));
				global.numberOfBullets++;

				// Spawn bullet outside of ship
				Vector3 spawnPos = gameObject.transform.position;
				spawnPos.x += 1.5f * Mathf.Cos (rotation * Mathf.PI / 180);
				spawnPos.z -= 1.5f * Mathf.Sin (rotation * Mathf.PI / 180);

				// Instantiate the Bullet
				GameObject bulletObj = Instantiate (bullet, spawnPos, Quaternion.identity) as GameObject;
				Bullet b = bulletObj.GetComponent<Bullet> ();
				Quaternion qRotation = Quaternion.Euler (new Vector3 (0, rotation, 0));
				b.heading = qRotation;
			}	
		}

		// Switch to main overhead camera
		if (Input.GetKeyDown ("1")) {
			mCamera.enabled = true;
			fpCamera.enabled = false;
		}

		// Switch to first person mode
		if (Input.GetKeyDown ("2")) {
			fpCamera.enabled = true;
			mCamera.enabled = false;
		}

		// Flash to indicate invincible
		Component halo = gameObject.GetComponent ("Halo");
		if (invincible && !blinkStarted) {
			blinkStarted = true;
			halo.GetType ().GetProperty ("enabled").SetValue (halo, true, null);
			StartCoroutine (Blink (5.0f));

		} else if (!invincible) {
			blinkStarted = false;
			halo.GetType ().GetProperty ("enabled").SetValue (halo, false, null);
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

		// Check the top bound
		if (transform.position.z > cameraTopRight.z) {
			position.z = cameraBottomLeft.z + 0.1f;
		}
		
		// Check the bottom bound
		if (transform.position.z < cameraBottomLeft.z) {
			position.z = cameraTopRight.z - 0.1f;
		}
		
		// Check the left bound
		if (transform.position.x < cameraBottomLeft.x) {
			position.x = cameraTopRight.x - 0.1f;
		}
		
		// Check the right bound
		if (transform.position.x > cameraTopRight.x) {
			position.x = cameraBottomLeft.x + 0.1f;
		}
		
		// Set the transformation's position
		transform.position = position;
	}

	void OnCollisionEnter (Collision collision)
	{
		if (invincible == true) {
			return;
		}

		GameObject globalObject = GameObject.Find ("GlobalObject");
		Global global = globalObject.GetComponent<Global> ();

		Collider collider = collision.collider;
		if (collider.CompareTag ("Asteroids")) {
			Asteroid roid = collider.gameObject.GetComponent< Asteroid > ();
			roid.Die ();

			AudioSource.PlayClipAtPoint (deathKnell, gameObject.transform.position);
			Instantiate (deathExplosion, gameObject.transform.position, Quaternion.AngleAxis (-90, Vector3.right));

			// Check how many lives we have left
			if (global.livesLeft == 0) {
				//No lives, you die, game over.
				Destroy (gameObject);
				Application.LoadLevel ("GameOverScene");
			} else {
				// Subtract a life, then respawn
				global.livesLeft--;
				Respawn ();
			}
		} else if (collider.CompareTag ("MediumAsteroids")) {
			MediumAsteroid roid = collider.gameObject.GetComponent<MediumAsteroid> ();
			roid.Die ();

			AudioSource.PlayClipAtPoint (deathKnell, gameObject.transform.position);
			Instantiate (deathExplosion, gameObject.transform.position, Quaternion.AngleAxis (-90, Vector3.right));

			// Check how many lives we have left
			if (global.livesLeft == 0) {
				// No lives, you die, game over.
				Destroy (gameObject);
				Application.LoadLevel ("GameOverScene");
			} else {
				// Subtract a life, then respawn
				global.livesLeft--;
				Respawn ();
			}
		} else if (collider.CompareTag ("UFOBullet")) {
			UFOBullet ufoBullet = collider.gameObject.GetComponent<UFOBullet> ();
			Destroy (ufoBullet);

			AudioSource.PlayClipAtPoint (deathKnell, gameObject.transform.position);
			Instantiate (deathExplosion, gameObject.transform.position, Quaternion.AngleAxis (-90, Vector3.right));

			// Check how many lives we have left
			if (global.livesLeft == 0) {
				// No lives, you die, game over.
				Destroy (gameObject);
				Application.LoadLevel ("GameOverScene");
			} else {
				// Subtract a life, then respawn
				global.livesLeft--;
				Respawn ();
			}
		} else if (collider.CompareTag ("UFO")) {
			UFO ufo = collider.gameObject.GetComponent<UFO> ();
			ufo.Die ();

			AudioSource.PlayClipAtPoint (deathKnell, gameObject.transform.position);
			Instantiate (deathExplosion, gameObject.transform.position, Quaternion.AngleAxis (-90, Vector3.right));

			// Check how many lives we have left
			if (global.livesLeft == 0) {
				// No lives, you die, game over.
				Destroy (gameObject);
				Application.LoadLevel ("GameOverScene");
			} else {
				// Subtract a life, then respawn
				global.livesLeft--;
				Respawn ();
			}
		} else if (collider.CompareTag ("Multiplier")) {
			Multiplier multiplier = collider.gameObject.GetComponent<Multiplier> ();
			multiplier.Die ();
		} else {
			Debug.Log ("Collided with " + collider.tag);
		}
	}

	public void EnableInvincible ()
	{
		invincible = true;
		Invoke ("DisableInvincible", 5);
	}

	void DisableInvincible ()
	{
		invincible = false;
	}

	public void Respawn ()
	{
		invincible = true;

		// Reset multiplier count
		GameObject globalObject = GameObject.Find ("GlobalObject");
		Global global = globalObject.GetComponent<Global> ();
		global.multiplier = 1;

		GetComponent<Rigidbody> ().Sleep (); //This resets the forces on the ship

		transform.position = new Vector3 (0.0f, 0.0f, 0.0f);
		Invoke ("DisableInvincible", 5);
	}

	IEnumerator Blink (float duration)
	{
		float endTime = Time.time + duration;
		Component halo = gameObject.GetComponent ("Halo");
		
		while (Time.time < endTime) {
			halo.GetType ().GetProperty ("enabled").SetValue (halo, false, null);
			yield return new WaitForSeconds (0.1f);
			halo.GetType ().GetProperty ("enabled").SetValue (halo, true, null);
			yield return new WaitForSeconds (0.1f);
		}
	}
}