using UnityEngine;
using System.Collections;

public class SwapMaterials : MonoBehaviour {

	public Material CheapMaterial;
	public Material ExpensiveMaterial;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Keypad1))
			renderer.material = CheapMaterial;
		else if(Input.GetKeyDown(KeyCode.Keypad2))
			renderer.material = ExpensiveMaterial;
	}
}
