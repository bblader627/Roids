using UnityEngine;
using System.Collections;

public class CreateProjectile : MonoBehaviour {
	
	public GameObject Projectile;
	public float ProjSpeed = 1.0f;
	
	private Camera cam;
	
	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Ray cRay = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0.0f));
			
			GameObject inst = Instantiate(Projectile, cRay.origin, transform.rotation) as GameObject;
			inst.GetComponent<Rigidbody>().AddForce(cRay.direction * ProjSpeed, ForceMode.VelocityChange);
		}
	}
}
