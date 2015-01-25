using UnityEngine;
using System.Collections;

public class Cam : MonoBehaviour {
	
	public float MovementSpeed = 1.0f;
	public float RotationSpeed = 1.0f;
	
	private bool mForward = false;
	private bool mBack = false;
	private bool mLeft = false;
	private bool mRight = false;
	private bool mUp = false;
	private bool mDown = false;
	
	private Vector3 PrevMousePos = Vector2.zero;
	private Vector3 MousePosChange = Vector2.zero;
	
	// Use this for initialization
	void Start ()
	{
		PrevMousePos = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.W))
			mForward = true;
		else if(Input.GetKeyUp(KeyCode.W))
			mForward = false;
		
		if(Input.GetKeyDown(KeyCode.S))
			mBack = true;
		else if(Input.GetKeyUp(KeyCode.S))
			mBack = false;
		
		if(Input.GetKeyDown(KeyCode.A))
			mLeft = true;
		else if(Input.GetKeyUp(KeyCode.A))
			mLeft = false;
		
		if(Input.GetKeyDown(KeyCode.D))
			mRight = true;
		else if(Input.GetKeyUp(KeyCode.D))
			mRight = false;
		
		if(Input.GetKeyDown(KeyCode.Q))
			mUp = true;
		else  if(Input.GetKeyUp(KeyCode.Q))
			mUp = false;
		
		if(Input.GetKeyDown(KeyCode.E))
			mDown = true;
		else  if(Input.GetKeyUp(KeyCode.E))
			mDown = false;
		
		
		MousePosChange = Input.mousePosition - PrevMousePos;
		PrevMousePos = Input.mousePosition;
		
		if(Input.GetMouseButton(0))
		{
			transform.Rotate(new Vector3(-MousePosChange.y * Time.deltaTime * RotationSpeed, MousePosChange.x * Time.deltaTime * RotationSpeed, 0.0f));	
		}
		
		if(mForward)
			transform.position += (transform.forward * Time.deltaTime * MovementSpeed);	
		
		if(mBack)
			transform.position += (-transform.forward * Time.deltaTime * MovementSpeed);	
		
		if(mLeft)
			transform.position += (-transform.right * Time.deltaTime * MovementSpeed);	
		
		if(mRight)
			transform.position += (transform.right * Time.deltaTime * MovementSpeed);
		
		if(mUp)
			transform.position += (transform.up * Time.deltaTime * MovementSpeed);
		
		if(mDown)
			transform.position += (-transform.up * Time.deltaTime * MovementSpeed); 
	}
}
