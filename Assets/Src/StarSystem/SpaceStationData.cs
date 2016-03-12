using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class SpaceStationData {

	public Guid ID;
	[NonSerialized]
	public Transform StationTransform;
	
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

	public SpaceStationData() {

		Position = new Vector3(
			UnityEngine.Random.Range(-40, 40),
			UnityEngine.Random.Range(-40, 40));
		ID = Guid.NewGuid();
	}

	public SpaceStationData (Vector3 position) {

		Position = position;
		ID = Guid.NewGuid();
	}
}
