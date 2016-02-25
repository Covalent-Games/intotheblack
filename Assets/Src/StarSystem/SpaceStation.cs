using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class SpaceStation {

	public Guid ID;
	
	private float _xPosition;
	private float _yPosition;

	public Vector3 Position {
		get {
			return new Vector3(_xPosition, _yPosition, 0f);
		}
		private set {
			_xPosition = value.x;
			_yPosition = value.y;
		}
	}

	public SpaceStation() {

		Position = new Vector3(
			UnityEngine.Random.Range(-200, 200),
			UnityEngine.Random.Range(-200, 200));
		ID = Guid.NewGuid();
	}

	public SpaceStation (Vector3 position) {

		Position = position;
		ID = Guid.NewGuid();
	}
}
