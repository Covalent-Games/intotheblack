using UnityEngine;
using System.Collections;
using System;

public class FollowPlayer : MonoBehaviour {

	public float SkyboxRotationSpeed;

	private Transform _playerTransform;
	private Transform _skyboxCameraT;
	private Vector3 _previousPlayerPos;

	void Awake () {

		_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		_skyboxCameraT = GameObject.Find("SkyboxCamera").transform;
		_previousPlayerPos = _playerTransform.position;
	}
	
	void FixedUpdate () {

		Vector3 playerPos = _playerTransform.position;
		RotateBackground(playerPos);
		_previousPlayerPos = playerPos;
		playerPos.z = transform.position.z;
		transform.position = playerPos;
	}

	private void RotateBackground(Vector3 playerPos) {

		Vector3 rotation = playerPos - _previousPlayerPos;
		rotation *= Time.deltaTime * SkyboxRotationSpeed;
		_skyboxCameraT.Rotate(new Vector3(-rotation.y, rotation.x, 0f));
	}
}
