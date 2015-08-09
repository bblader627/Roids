using UnityEngine;
using System.Collections;

public class FirstPerson : MonoBehaviour
{
	void Update ()
	{
		Transform shipTransform = GameObject.FindGameObjectWithTag ("Ship").transform;

		transform.parent = shipTransform;
		transform.localPosition = Vector3.zero;
	}
}
