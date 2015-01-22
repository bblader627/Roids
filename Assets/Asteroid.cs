﻿using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour {

	// Use this for initialization
	public int pointValue;
	public GameObject deathExplosion;
	public AudioClip deathKnell;
	public Vector3 forceVector;


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