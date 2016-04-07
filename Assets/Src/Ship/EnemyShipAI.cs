﻿using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class EnemyShipAI : MonoBehaviour {

	[SerializeField]
	private LayerMask _rayMask;
	private ShipController _shipController;
	private ShipWeapons _shipWeapons;
	private Destructable _shipStats;
	private GameObject _player;
	private Rigidbody2D _playerRb;
	[SerializeField]
	private AIState _state = AIState.Intelligent;

	private enum AIState {
		Aggressive,
		Neutral,
		Intelligent,
		Retreating,
	}

	void Awake() {

		_shipController = GetComponent<ShipController>();
		_shipWeapons = GetComponent<ShipWeapons>();
		_shipStats = GetComponent<Destructable>();
		_player = GameManager.Player;
		_playerRb = _player.GetComponent<Rigidbody2D>();
	}

	void Start() {

		_state = GetRandomState();
		if (_state == AIState.Intelligent) {
			_shipWeapons.GunDamage = Random.Range(1, PlayerData.State.PlayerLevel);
			_shipController.RotationSpeed *= 2.5f;
			Transform t = transform.GetChild(0);
			t.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("ImportedImages/smartEnemy");
		}
	}

	void Update() {

		if (ShouldBeMoving()) {
			_shipController.MoveShip(transform.up, false);
		}
		_shipController.RotateShip(GetHeading());

		if (ShouldBeShooting()) {
			_shipWeapons.FirePrimaryWeapons(); 
		}

		ChangeState();
	}

	private void ChangeState() {
		
		switch (_state) {
			case AIState.Aggressive:
				break;
			case AIState.Intelligent:
			case AIState.Neutral:
				if (_shipStats.Health / (float)_shipStats.MaxHealth < 0.2f) {
					_state = AIState.Retreating;
				}
				break;
		}
	}

	private Vector2 GetHeading() {

		Vector2 heading = Vector2.zero;
		switch (_state) {
			case AIState.Aggressive:
			case AIState.Neutral:
				heading = _player.transform.position - transform.position;
				break;
			case AIState.Retreating:
				heading = transform.position - _player.transform.position;
				break;
			case AIState.Intelligent:
				// attempt to flank
				float d = Vector3.Distance(_player.transform.position, transform.position);
				float aimAdjust = _playerRb.velocity.magnitude * (d / 12f);
				if (aimAdjust > d -2) {
					heading = _player.transform.position - transform.position;
				} else {
					Vector3 velocityDirectionAverage = 
						(Vector3.Normalize((Vector3)_playerRb.velocity + _player.transform.up));
					heading =
					(_player.transform.position + (velocityDirectionAverage * aimAdjust)) - transform.position;
				}				
				break;
		}

		return Vector3.Normalize(heading);
	}

	private bool ShouldBeMoving() {

		bool value = false;
		float distance;
		switch (_state) {
			case AIState.Intelligent:
			case AIState.Neutral:
				distance = Random.Range(5, 8);
				value = Vector2.Distance(transform.position, _player.transform.position) > distance;
				break;
			case AIState.Aggressive:
				distance = Random.Range(3.5f, 6f);
				value = Vector2.Distance(transform.position, _player.transform.position) > distance;
				break;
			case AIState.Retreating:
				//TODO: This might need something different, but we'll see...
				value = true;
				break;
		}

		return value;		
	}

	private bool ShouldBeShooting() {

		RaycastHit2D hitinfo =
			Physics2D.Raycast(transform.position, transform.TransformDirection(Vector2.up), _rayMask.value);
		if (hitinfo != null) {
			return true;
		}
		return false;
	}

	private AIState GetRandomState() {

		Array values = Enum.GetValues(typeof(AIState));
		// Subtract one from length to exlude retreating from starting states.
		return (AIState)values.GetValue(UnityEngine.Random.Range(0, values.Length-1));
	}
}
