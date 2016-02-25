using UnityEngine;
using System.Collections;
using System;

public class Projectile : MonoBehaviour {

	public static Transform Container;

	public float Speed = 20f;
	public int Damage = 0;

	private ParticleSystem _sparks;
	private TrailRenderer _trailRenderer;
	private float _trailTime;

	void Awake() {

		_trailRenderer = GetComponent<TrailRenderer>();
		_trailTime = _trailRenderer.time;
		Transform sparksT = transform.FindChild("Sparks");
		if (sparksT) {
			_sparks = sparksT.GetComponent<ParticleSystem>();
		}
		transform.SetParent(Container);
	}

	void OnEnable() {

		StartCoroutine(MoveForwardRoutine());
		StartCoroutine(ResetTrailRoutine());
		_sparks.transform.SetParent(transform);
		_sparks.transform.localPosition = Vector3.zero;
	}

	private IEnumerator ResetTrailRoutine() {

		yield return null;
		_trailRenderer.time = _trailTime;
	}

	private IEnumerator MoveForwardRoutine() {
		
		while (gameObject.activeSelf) {
			transform.position = Vector2.MoveTowards(
				transform.position, 
				transform.position + (transform.up * Speed), Time.deltaTime * Speed);
			yield return null;
			if (Vector3.Distance(GameManager.Player.transform.position, transform.position) > 30f) {
				_trailRenderer.time = 0;
				gameObject.SetActive(false);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D collider) {

		Destructable stats = collider.GetComponent<Destructable>();
		if (stats != null) {
			stats.Health -= Damage;
		}

		if (_sparks != null) {
			_sparks.Play();
			_sparks.transform.parent = null;
		}
		gameObject.SetActive(false);
	}
}
