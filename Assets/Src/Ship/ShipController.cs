using UnityEngine;
using System.Collections;
using System;

public class ShipController : MonoBehaviour {

	public bool Boosting = false;
	public float BoostCooldown = 8f;
	public float BoostCooldownCurrent = 0f;
	public float RotationSpeed = .85f;

	private Transform _camTransform;
	private ShipWeapons _shipWeapons;
	private Rigidbody2D _rigidBody;
	[SerializeField]
	private float _speed = 5f;
	[SerializeField]
	[Range(1f, 2f)]
	private float _thrust = 1.2f;

	void Awake () {

		_camTransform = Camera.main.transform;
		_shipWeapons = GetComponent<ShipWeapons>();
		_rigidBody = GetComponent<Rigidbody2D>();
	}
	
	void Update() {

		BoostCooldownCurrent -= Time.deltaTime;
	}

	public void MoveShip(Vector2 direction, bool rotateShip = true) {

		// Change rotation
		if (rotateShip) {
			RotateShip(direction); 
		}
		// Change velocity;
		_rigidBody.AddForce(direction * _speed * _thrust);
		_rigidBody.velocity = Vector2.ClampMagnitude(_rigidBody.velocity, _speed);
	
	}

	public void Boost(Vector2 direction) {

		if (BoostCooldownCurrent <=0) {
			Boosting = true;
			_speed *= 2;
			BoostCooldownCurrent = BoostCooldown;
			_rigidBody.velocity = Vector3.ClampMagnitude(
				transform.TransformDirection(Vector3.up) * _speed/1.5f, _speed);
			Invoke("ReduceBoost", 2f);
		}
	}

	private void ReduceBoost() {

		_speed /= 2;
		Boosting = false;
	}

	public void RotateShip(Vector2 direction) {

		Quaternion lookRotation =
			Quaternion.LookRotation(Vector3.forward, (Vector2)transform.position + (direction * 99999));
		transform.rotation =
			Quaternion.RotateTowards(transform.rotation, lookRotation, Time.deltaTime * 360 * RotationSpeed);
	}

	public void SlowShip() {

		_rigidBody.velocity = 
			Vector2.ClampMagnitude(
				_rigidBody.velocity, 
				Mathf.MoveTowards(_rigidBody.velocity.magnitude, 0f, Time.fixedDeltaTime * 2f));
	}
}
