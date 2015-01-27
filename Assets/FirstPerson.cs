using UnityEngine;
using System.Collections;

public class FirstPerson : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Transform shipTransform = GameObject.FindGameObjectWithTag ("Ship").transform;
		transform.parent = shipTransform;
		transform.localPosition = Vector3.zero;
		//transform.localRotation = Quaternion.identity;
	}
}
