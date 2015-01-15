using UnityEngine;
using System.Collections;

public class Asteroid : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Die()
	{
		// Destroy removes the gameObject from the scene and
		// marks it for garbage collection
		Destroy (gameObject);
	}
}
