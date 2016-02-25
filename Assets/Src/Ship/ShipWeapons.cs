using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Random = UnityEngine.Random;
using System;

public class ShipWeapons : MonoBehaviour {

	public Transform[] FirePositions;
	public int GunDamage = 1;
	public int TorpedoDamage = 10;
	public int TorpedoFireRate = 1;
	public float TorpedoCooldownCurrent = 0f;
	public float TorpedoCooldown = 15f;
	public float ShieldCooldownCurrent = 0f;
	public float ShieldCooldown = 20f;
	internal bool Shielded = false;

	private List<Projectile> _projectileList = new List<Projectile>();
	[SerializeField]
	internal float RateOfFire;
	private float fireDelay = 0;
	private GameObject _projectilePrefab;
	private GameObject _torpedoPrefab;
	private GameObject _shield;
	private AudioSource _audio;

	void Awake() {

		if (tag == "Player") {
			_projectilePrefab = (GameObject)Resources.Load("Prefabs/PlayerProjectile");
			_torpedoPrefab = (GameObject)Resources.Load("Prefabs/PhotonTorpedo");
		} else if (tag == "Enemy") {
			_projectilePrefab = (GameObject)Resources.Load("Prefabs/EnemyProjectile");
		}
		_shield = transform.GetChild(1).gameObject;
		_audio = GetComponent<AudioSource>();
	}

	internal void ActivateShield() {
		
		if (ShieldCooldownCurrent <= 0) {
			Shielded = true;
			ShieldCooldownCurrent = ShieldCooldown;
			_shield.SetActive(true);
		}
	}

	void Update () {

		fireDelay -= Time.deltaTime;
		TorpedoCooldownCurrent -= Time.deltaTime;
		ShieldCooldownCurrent -= Time.deltaTime;
	}

	internal void FirePrimaryWeapons() {

		if (fireDelay <= 0) {
			fireDelay = 1f / RateOfFire;
			LaunchProjectile(GetNextProjectile());
			_audio.PlayOneShot(SoundHolder.Instance.PulseLaser_01, .5f);
			_audio.pitch = Random.Range(0.85f, 1.15f);
			if (CompareTag("Enemy")) {
				_audio.pitch += .4f;
			}
			_audio.volume = 1 - (Vector3.Distance(GameManager.Player.transform.position, transform.position) / 20f);
		}
	}

	internal void FireTorpedo() {

		if (TorpedoCooldownCurrent <= 0f) {
			TorpedoCooldownCurrent = TorpedoCooldown;
			StartCoroutine(FireTorpedoRoutine());
		}
	}

	private IEnumerator FireTorpedoRoutine() {

		GameObject torpedoGo;
		PhotonTorpedo torpedo;
		for (int i = 0; i < TorpedoFireRate; i++) {
			torpedoGo = (GameObject)Instantiate(_torpedoPrefab, transform.position, Quaternion.identity);
			torpedo = torpedoGo.GetComponent<PhotonTorpedo>();
			torpedo.Damage = TorpedoDamage;
			yield return new WaitForSeconds(0.5f);
		}
	}

	private void LaunchProjectile(Projectile projectile) {

		projectile.transform.rotation = transform.rotation;

		projectile.Damage = GunDamage;
		projectile.gameObject.SetActive(true);

		TrailRenderer tr = projectile.GetComponent<TrailRenderer>();

		if (CompareTag("Enemy")) {
			projectile.transform.position = transform.position;
		} else if (RateOfFire == 4f) {
			tr.material.SetColor("_TintColor", Color.green);
			projectile.transform.position = FirePositions[0].position;
		} else if (RateOfFire == 5f) {
			tr.material.SetColor("_TintColor", Color.blue);
			projectile.transform.position = FirePositions[0].position;
		} else if (RateOfFire == 6f) {
			tr.material.SetColor("_TintColor", Color.yellow);
			projectile.transform.position = FirePositions[0].position;
		} else if (RateOfFire == 7f) {
			tr.material.SetColor("_TintColor", new Color(1f, .4f, 0f));
			projectile.transform.position = FirePositions[0].position;
		} else if (RateOfFire == 8f) {
			tr.material.SetColor("_TintColor", Color.red);
			projectile.transform.position = FirePositions[0].position;
		}
	}

	private Projectile GetNextProjectile() {

		Projectile projectile = null;

		for (int i = 0; i < _projectileList.Count; i++) {
			if (!_projectileList[i].gameObject.activeSelf) {
				projectile = _projectileList[i];
				break;
			}
		}

		if (projectile == null) {
			projectile = InstantiateProjectile();
		}

		return projectile;
	}

	private Projectile InstantiateProjectile() {

		GameObject go = Instantiate(_projectilePrefab);
		go.SetActive(false);
		Projectile projectile = go.GetComponent<Projectile>();
		_projectileList.Add(projectile);
		return projectile;
	}
}
