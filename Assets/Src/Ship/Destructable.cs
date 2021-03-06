﻿using UnityEngine;
using UnityEngine.Extensions;
using System.Collections;
using System;

public class Destructable : MonoBehaviour {

	public int Health{
		get	{
			return _health;
		}
		set	{
			_health = value;
			if (_health <= 0) {
				Die();
			}
			if (_health > MaxHealth) {
				_health = MaxHealth;
			}
		}
	}
	[SerializeField]
	internal int MaxHealth;
	[SerializeField]
	private int _health;
	private Rigidbody2D _rb2d;

	public Component PrimaryWeaponComponent;
	public Component SecondaryWeaponComponent;
	public Component ShieldComponent;
	public Component HullComponent;
	public Component EngineComponent;


	void Start() {

		MaxHealth = PlayerData.State.MaxHealth == 0 ? 50 : PlayerData.State.MaxHealth;
		Health = PlayerData.State.CurrentHealth == 0 ? MaxHealth : PlayerData.State.CurrentHealth;
		//Health = MaxHealth;
		_rb2d = GetComponent<Rigidbody2D>();
		if (tag == "Player") {
			StartCoroutine(HealOverTimeRoutine());
			SetUpAbilities();
		}
	}

	private IEnumerator HealOverTimeRoutine() {
		
		while (gameObject.activeSelf) {
			yield return new WaitForSeconds(5f);
			Health += 1;
		}
	}

	private void SetUpAbilities() {

		this.PrimaryWeaponComponent = new Component();
		this.SecondaryWeaponComponent = new Component();
		this.ShieldComponent = new Component();
		this.HullComponent = new Component();
		this.EngineComponent = new Component();

		this.ShieldComponent.CreateRandomComponent((int)ComponentTypeEnum.Sheild, 1);
	}

	private void Die() {
		
		if (gameObject.tag == "Player") {
			GameObject.FindGameObjectWithTag("GameManager").GetComponent<OverlayUI>().DisplayGameOverText();
			GameManager.GameOver();
		} else {
			// Each kill brings hostility down by 0.75% (each enemy is = 1% of total hostility)
			StarSystemData.StarSystemLoaded.Hostility -= 0.0075f;
			PlayerData.State.CurrentPlayerExperience += MaxHealth;	// Gain experience == enemy's health. TODO: make enemy experience a function of that enemy. Make Enemy class?
			if (GameManager.ActiveEnemies.Contains(gameObject)) {
				GameManager.ActiveEnemies.Remove(gameObject);
			}
			Transform t = transform.FindChildRecursive("DeathExplosion");
			if (t != null) {
				ParticleSystem ps = t.GetComponent<ParticleSystem>();
				t.parent = null;
				ps.Play();
			}
			Destroy(gameObject);
		}
		AudioSource.PlayClipAtPoint(
			SoundHolder.Instance.Explosions[UnityEngine.Random.Range(0, SoundHolder.Instance.Explosions.Length)],
			Camera.main.transform.position,
			1 - (Vector3.Distance(GameManager.Player.transform.position, transform.position) / 20f));
	}

	internal void Sethealth(int v) {

		MaxHealth = v;
		Health = v;
	}

	private void OnCollisionEnter2D(Collision2D collision) {

		if (collision.gameObject.CompareTag("Asteroid")) {
			Health -= (int)(MaxHealth * 
				(collision.transform.localScale.magnitude / 10f) *
				(_rb2d.velocity.magnitude / 3f));

			// Prevent player from getting experience from an asteroid kill
			if (Health <= 0) {
				if (CompareTag("Enemy")) {
					// If this isn't exactly the opposite of how experience is gained, the player will get mad.
					PlayerData.State.CurrentPlayerExperience -= MaxHealth;
				}
			}
		}
	}
}
