using UnityEngine;
using System.Collections;
using System;

public class PlayerShipShield : MonoBehaviour {

	private Destructable _player;
	private ShipWeapons _weapons;
	private int _currentHealth;

	private void Awake() {

		_player = transform.parent.GetComponent<Destructable>();
		_weapons = transform.parent.GetComponent<ShipWeapons>();
	}

	private void Start() {

		OnEnable();
	}

	private void OnEnable() {

		_currentHealth = (int)(_player.MaxHealth / 10f);
	}

	private void OnCollisionEnter2D(Collision2D collision) {

		Debug.Log("shielded!");

		Projectile projectile = collision.gameObject.GetComponent<Projectile>();

		if (projectile != null) {
			_currentHealth -= projectile.Damage;
			CheckIfDepleted();
		}
	}

	private void OnTriggerEnter2D(Collider2D collider) {

		Projectile projectile = collider.GetComponent<Projectile>();

		if (projectile != null) {
			_currentHealth -= projectile.Damage;
			CheckIfDepleted();
		}
	}

	private void CheckIfDepleted() {
		
		if (_currentHealth <= 0) {
			_weapons.Shielded = false;
			gameObject.SetActive(false);
		}
	}
}
