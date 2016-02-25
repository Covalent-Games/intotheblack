using UnityEngine;
using System.Collections;
using System;

public class TorpedoSplash : MonoBehaviour {

	public int Damage;

	void Start () {

		StartCoroutine(DestroySplashRoutine());
	}

	private IEnumerator DestroySplashRoutine() {

		yield return new WaitForFixedUpdate();
		Destroy(gameObject);
	}

	private void OnTriggerEnter2D(Collider2D collider) {

		if (collider.CompareTag("Enemy")) {
			Destructable ship = collider.GetComponent<Destructable>();
			// If health is less than 20, kill it. Otherwise do half it's health.
			ship.Health -= ship.Health < 20 ? ship.Health : ship.MaxHealth / 2;
		}
	}
}
