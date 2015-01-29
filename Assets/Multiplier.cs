using UnityEngine;
using System.Collections;

public class Multiplier : MonoBehaviour {

	public AudioClip pickupSound;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Die() {
		AudioSource.PlayClipAtPoint(pickupSound,
		                            gameObject.transform.position );
		//Instantiate(deathExplosion, gameObject.transform.position,
		//            Quaternion.AngleAxis(-90, Vector3.right) );

		GameObject obj = GameObject.Find("GlobalObject");
		Global g = obj.GetComponent<Global>();
		g.multiplier++;

		Destroy (gameObject);
	}
}
