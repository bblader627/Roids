using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 updatedPosition = gameObject.transform.position;
		updatedPosition.x += .001f;
		gameObject.transform.position = updatedPosition;
	}
}
