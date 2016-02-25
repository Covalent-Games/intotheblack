using UnityEngine;
using System.Collections;

public class PhotonTorpedo : MonoBehaviour {

	public float Speed = 20f;
	[HideInInspector]
	public int Damage = 0;
	public Transform Target;
	public float SelfDestructTimeout;
	public GameObject SplashRadius; 

	private ParticleSystem _sparks;

	void Awake() {

		SplashRadius = transform.FindChild("SplashRadius").gameObject;
		Transform sparksT = transform.FindChild("Sparks");
		if (sparksT!= null) {
			_sparks = sparksT.GetComponent<ParticleSystem>();
		}
	}

	void OnEnable() {

		StartCoroutine(MoveForwardRoutine());
		Speed = UnityEngine.Random.Range(5f, 9f);
		Invoke("EndObject", SelfDestructTimeout);
	}

	private IEnumerator MoveForwardRoutine() {

		while (gameObject.activeSelf) {
			if (Target != null) {
				transform.position = Vector2.MoveTowards(
					transform.position,
					Target.position, Time.deltaTime * Speed);
				transform.rotation = 
					Quaternion.LookRotation(
						Vector3.forward,
						Target.position - transform.position);
			} else {
				transform.position = Vector2.MoveTowards(
					transform.position,
					transform.position + Vector3.up,
					Time.deltaTime * Speed);
				GetHomingWeaponTarget();
			}
			yield return null;
		}
	}

	public void GetHomingWeaponTarget() {

		float shortestDistance = 99f;
		Transform closestTarget = null;
		if (GameManager.ActiveEnemies.Count > 0) {
			for (int i = 0; i < GameManager.ActiveEnemies.Count; i++) {
				float d = Vector3.Distance(transform.position, GameManager.ActiveEnemies[i].transform.position);
				if (d < shortestDistance) {
					shortestDistance = d;
					closestTarget = GameManager.ActiveEnemies[i].transform;
				}
			}
		}
		Target = closestTarget;
	}

	private void EndObject() {

		if (_sparks != null) {
			_sparks.transform.parent = null;
			_sparks.Play();
		}
		// Splash radius automatically handles damage.
		SplashRadius.SetActive(true);
		SplashRadius.transform.parent = null;
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D collider) {

		EndObject();
	}
}
