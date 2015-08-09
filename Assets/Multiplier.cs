using UnityEngine;
using System.Collections;

public class Multiplier : MonoBehaviour
{
	public AudioClip pickupSound;
	private float timer;
	private float dyingWarning;
	private bool blinkStarted;

	void Start ()
	{
		timer = 0.0f;
		dyingWarning = 15.0f;
		blinkStarted = false;
	}

	void Update ()
	{
		timer += Time.deltaTime;
		if (timer > 20.0f) {
			Destroy (gameObject);
		} else if ((timer >= dyingWarning) && (blinkStarted == false)) {
			blinkStarted = true;
			StartCoroutine (Blink (5.0f));
		}
	}

	public void Die ()
	{
		AudioSource.PlayClipAtPoint (pickupSound, gameObject.transform.position);

		GameObject globalObj = GameObject.Find ("GlobalObject");
		Global global = globalObj.GetComponent<Global> ();
		global.multiplier++;

		Destroy (gameObject);
	}

	IEnumerator Blink (float duration)
	{
		float endTime = Time.time + duration;
		Component halo = gameObject.GetComponent ("Halo");

		while (Time.time < endTime) {
			halo.GetType ().GetProperty ("enabled").SetValue (halo, false, null);
			yield return new WaitForSeconds (0.1f);
			halo.GetType ().GetProperty ("enabled").SetValue (halo, true, null);
			yield return new WaitForSeconds (0.5f);
		}
	}
}
