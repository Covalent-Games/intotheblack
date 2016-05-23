using UnityEngine;
using System.Collections;

public class ExpansionDot : MonoBehaviour {

	public Vector3 To;
	public float Speed = 1;

	private void Update() {

		transform.position = Vector2.MoveTowards(transform.position, To, Time.deltaTime * Speed);
		
		if (transform.position == To) {
			// This is normally bad, but we've got a lot of wiggle room on the galaxy map, so we're keeping it simple.
			StartCoroutine(Die());
		}
	}

	IEnumerator Die() {

		yield return new WaitForSeconds(GetComponent<TrailRenderer>().time);
		Destroy(gameObject);
	}
}
